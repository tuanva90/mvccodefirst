using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Insyston.Operations.Model;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.Business.Receipts.Model;
using Insyston.Operations.WPF.ViewModel.Common;
using business = Insyston.Operations.Business.Common;
using Insyston.Operations.Logging;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Security;
using Insyston.Operations.WPF.ViewModel.Common.Controls;
using Insyston.Operations.Business.OpenItems;
using System.Threading;
using Insyston.Operations.Common;

namespace Insyston.Operations.WPF.ViewModel.Receipts.CommonControls
{
    [Export(typeof(NewReceiptViewModel))]
    public class NewReceiptViewModel : OldViewModelBase, IDataErrorInfo
    {
        private readonly ReceiptBatchType batchType;
        LXMSystemReceiptDefault batchTypeDefaults;

        private string title, clientName;
        private bool isChequeExpanded;
        private int receiptID;
        private int? applyToObjectID;
        private ReceiptApplyTo receiptApplyTo;
        private int paymentType, ddccAccountID;
        private decimal amountReceived;
        private bool isChanged, includeFuture, includeClosed;
        private readonly bool isReceiptLoading, isReceiptEditable;

        private List<DropdownList> applyToList, ddccAccounts;
        private List<PaymentType> cashChequePaymentTypes;
        private Receipt receipt;
        private ObservableCollection<OpenItemSearch> openItems;

        public InteractionRequest<ConfirmationWindowViewModel> UIConfirmation { get; private set; }
        public InteractionRequest<PopupWindow> Popup { get; set; }   

        public delegate void ClearGridFilterEventHandler();
        public event ClearGridFilterEventHandler ClearGridFilter;

        public DelegateCommand<string> Save { get; private set; }
        public DelegateCommand Search { get; private set; }
        public DelegateCommand OtherCharge { get; private set; }
        public DelegateCommand CalculateBalance { get; private set; }
        public DelegateCommand<bool?> ResetAllocation { get; private set; }

        public delegate void GridViewTotalsChangedHandler();
        public event GridViewTotalsChangedHandler GridViewTotalsChanged;

        public NewReceiptViewModel()
        {            
        }

        public NewReceiptViewModel(ReceiptBatchType receiptbatchType, int receiptBatchID, int batchStatus, int receiptid = 0)
        {
            batchType = receiptbatchType;
            receiptID = receiptid;            
            ApplyToList = BatchTypeFunctions.GetApplyToList((int)batchType);
            LockTableName = "ReceiptBatch";
            LockUniqueIdentifier = receiptBatchID.ToString();
            isReceiptEditable = batchStatus == (int)ReceiptBatchStatus.Created && IsLocked;
            
            try
            {
                Save = new DelegateCommand<string>(OnSave);
                Search = new DelegateCommand(OnSearch);
                OtherCharge = new DelegateCommand(OnOtherCharge);
                CalculateBalance = new DelegateCommand(OnCalculateBalance);
                ResetAllocation = new DelegateCommand<bool?>(SearchOpenItems);

                UIConfirmation = new InteractionRequest<ConfirmationWindowViewModel>();
                Popup = new InteractionRequest<PopupWindow>();
                
                SetReceiptDefaults(receiptBatchID);
                SetIcon();

                if (receiptID > 0)
                {
                    isReceiptLoading = true;
                    Receipt = ReceiptFunctions.Get(receiptID);
                    AmountReceived = receipt.GrossAmountReceived;

                    receiptApplyTo = (ReceiptApplyTo)Enum.Parse(typeof(ReceiptApplyTo), receipt.ApplyToTypeID.ToString());

                    if (receipt.CashReceiptDetails != null && receipt.CashReceiptDetails.Count > 0)
                    {
                        Receipt.ChequeReceiptDetail = new ChequeReceiptDetail { PaymentTypeID = 0 };
                        PaymentType = receipt.CashReceiptDetails.FirstOrDefault().PaymentTypeID;
                    }
                    else if (receipt.ChequeReceiptDetail != null)
                    {
                        PaymentType = receipt.ChequeReceiptDetail.PaymentTypeID;
                    }

                    if (receiptApplyTo == ReceiptApplyTo.Quote)
                    {
                        ApplyToObjectID = receipt.QuoteID;
                    }
                    else if (receiptApplyTo != ReceiptApplyTo.Suspense)
                    {
                        switch (receiptApplyTo)
                        {
                            case ReceiptApplyTo.Contract:
                                ApplyToObjectID = receipt.ContractID;
                                break;
                            case ReceiptApplyTo.Client:
                                ApplyToObjectID = business.ContractFunctions.GetClientID(receipt.ContractID.GetValueOrDefault());
                                break;
                            case ReceiptApplyTo.Invoice:
                                ApplyToObjectID = business.InvoiceFunctions.GetInvoiceNo(receipt.InvoiceAssetId.GetValueOrDefault());
                                break;
                        }
                    }

                    if (batchType == ReceiptBatchType.DirectDebit)
                    {
                        DDCCAccountID = receipt.DirectDebitReceiptDetails.FirstOrDefault().BankDetail.BankDetailsId;
                    }
                    else if(batchType == ReceiptBatchType.CreditCard)
                    {
                        DDCCAccountID = receipt.CreditCardReceiptDetails.FirstOrDefault().BankDetail.BankDetailsId;
                    }                   

                    isReceiptLoading = false;
                }

                RaisePropertyChanged("BatchType");
                RaisePropertyChanged("IsReceiptEditable");
                RaisePropertyChanged("DDCCAccountNoLabelCaption");
                RaisePropertyChanged("IsReceiptinEditMode");
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error occurred while initializing Add Receipt Screen", "Add Receipt - Error");
                IsBusy = false;
            }
        }

        public Receipt Receipt
        {
            get
            {
                return receipt;                
            }
            set
            {
                if (receipt != value)
                {
                    receipt = value;
                    RaisePropertyChanged("Receipt");
                }
            }
        }

        public ObservableCollection<OpenItemSearch> OpenItems
        {
            get
            {
                return openItems;
            }
            set
            {
                if (openItems != value)
                {
                    openItems = value;

                    if (openItems != null && openItems.Count > 0)
                    {
                        foreach (OpenItemSearch openItem in openItems)
                        {                            
                            openItem.ErrorsChanged += OpenItem_Errors;
                        }
                    }
                    
                    ClearErrorMessages();
                    RaisePropertyChanged("OpenItems");
                }
            }
        }     

        public List<DropdownList> ApplyToList
        {
            get
            {
                return applyToList;
            }
            set
            {
                if (applyToList != value)
                {
                    applyToList = value;
                    RaisePropertyChanged("ApplyToList");
                }
            }
        }

        public List<PaymentType> CashChequePaymentTypes
        {
            get
            {
                return cashChequePaymentTypes;
            }
            set
            {
                if (cashChequePaymentTypes != value)
                {
                    cashChequePaymentTypes = value;
                    RaisePropertyChanged("CashChequePaymentTypes");
                }
            }
        }

        public List<DropdownList> DDCCAccounts
        {
            get
            {
                return ddccAccounts;
            }
            set
            {
                if (ddccAccounts != value)
                {
                    ddccAccounts = value;
                    RaisePropertyChanged("DDCCAccounts");
                }
            }
        }

        public BankDetail DDCCBankDetail
        {
            get
            {
                if (batchType == ReceiptBatchType.DirectDebit)
                {
                    return receipt.DirectDebitReceiptDetails.FirstOrDefault().BankDetail;
                }
                else if(batchType == ReceiptBatchType.CreditCard)
                {
                    return receipt.CreditCardReceiptDetails.FirstOrDefault().BankDetail;
                }               

                return null;
            }
            set
            {
                if (batchType == ReceiptBatchType.DirectDebit && receipt.DirectDebitReceiptDetails.FirstOrDefault().BankDetail != value)
                {
                    receipt.DirectDebitReceiptDetails.FirstOrDefault().BankDetail = value;
                }
                else if (batchType == ReceiptBatchType.CreditCard && receipt.CreditCardReceiptDetails.FirstOrDefault().BankDetail != value)
                {
                    receipt.CreditCardReceiptDetails.FirstOrDefault().BankDetail = value;
                }               
            }
        }

        public string BatchType
        {
            get
            {
                return batchType.ToString();
            }
            set
            {                
            }
        }

        public int PaymentType
        {
            get
            {
                return paymentType;
            }
            set
            {
                if (paymentType != value)
                {
                    paymentType = value;                    
                    IsChequeExpanded = cashChequePaymentTypes != null && cashChequePaymentTypes.Where(payType => payType.PaymentTypeId == value && payType.SCPaymentTypeId == (int)SCPaymentTypes.Cheque).Count() > 0;

                    if (Receipt.ChequeReceiptDetail != null)
                    {
                        if (isChequeExpanded)
                        {
                            Receipt.ChequeReceiptDetail.PaymentTypeID = paymentType;
                        }
                        else
                        {
                            Receipt.ChequeReceiptDetail.PaymentTypeID = 0;
                        }
                    }

                    RaisePropertyChanged("PaymentType");
                    RaisePropertyChanged("IsChequeEnabled");
                }
            }
        }        

        public int ApplyTo
        {
            get
            {
                return receipt.ApplyToTypeID;
            }
            set
            {
                if (receipt.ApplyToTypeID != value)
                {
                    receipt.ApplyToTypeID = value;
                    receiptApplyTo = ((ReceiptApplyTo)Enum.Parse(typeof(ReceiptApplyTo), receipt.ApplyToTypeID.ToString()));

                    ApplyToObjectID = null;
                    DDCCAccounts = null;
                    DDCCAccountID = 0;

                    RaisePropertyChanged("ApplyToLabelCaption");
                    RaisePropertyChanged("IsApplyPaymentsApplicable");
                    RaisePropertyChanged("IsContractNoApplicable");
                    RaisePropertyChanged("IsSuspense");
                    RaisePropertyChanged("ApplyTo");                    
                    ClearErrorMessages();
                }
            }
        }

        public int? ApplyToObjectID
        {
            get
            {
                return applyToObjectID;
            }
            set
            {
                int leeseeAccountID=0;

                if (applyToObjectID != value)
                {
                    applyToObjectID = value;                    

                    if (value.HasValue && IsValidObjectID())
                    {
                        switch (receiptApplyTo)
                        {
                            case ReceiptApplyTo.Client:
                                ClientName = business.ClientFunctions.GetClientName(value.Value);
                                break;
                            case ReceiptApplyTo.Contract:
                                ClientName = business.ContractFunctions.GetClientName(value.Value, out leeseeAccountID);
                                break;
                            case ReceiptApplyTo.Invoice:
                                ClientName = business.InvoiceFunctions.GetClientName(value.Value);
                                break;
                            case ReceiptApplyTo.Quote:
                                ClientName = business.QuoteFunctions.GetClientName(value.Value, out leeseeAccountID);
                                break;
                        }

                        if (batchType == ReceiptBatchType.DirectDebit || batchType == ReceiptBatchType.CreditCard)
                        {
                            DDCCAccounts = DDCCBatchFunctions.GetDDCCBankAccounts(batchType, receiptApplyTo, applyToObjectID.Value, leeseeAccountID);
                            DDCCAccountID = 0;

                            if (DDCCAccounts != null  && DDCCAccounts.Count > 0 && leeseeAccountID != 0)
                            {
                                DDCCAccountID = leeseeAccountID;
                            }
                        }

                        if (IsApplyPaymentsApplicable)
                        {
                            SearchOpenItems();
                        }
                    }
                    else
                    {
                        ClientName = string.Empty;
                        OpenItems = null;
                        DDCCAccounts = null;
                        DDCCAccountID = 0;
                        OnCalculateBalance();
                    }

                    RaisePropertyChanged("ApplyToObjectID");
                    RaisePropertyChanged("IsAddOtherChargeApplicable");
                }
            }
        }

        public decimal AmountReceived
        {
            get
            {
                return amountReceived;
            }
            set
            {
                if (amountReceived != value)
                {
                    amountReceived = value;

                    if (applyToObjectID.HasValue && IsValidObjectID() && IsApplyPaymentsApplicable)
                    {
                        SearchOpenItems();
                    }
                    else
                    {
                        OnCalculateBalance();
                    }

                    RaisePropertyChanged("AmountReceived");
                }
            }
        }

        public string PendingBalanceText
        {
            get
            {
                decimal dblAmtDue;

                if (openItems != null)
                {
                    dblAmtDue = openItems.Sum(item => item.AmountDue);
                    if (dblAmtDue - amountReceived > 0)
                    {
                        return "** Open items have Pending Receipts of " + String.Format(" {0:C}", dblAmtDue - amountReceived);
                    }
                }
                
                return string.Empty;                
            }
            set
            {
            }
        }

        public decimal OutOfBalance
        {
            get
            {
                decimal dblAmtApplied = 0;

                if (openItems != null)
                {
                    dblAmtApplied = openItems.Sum(item => item.AmountApplied);                    
                }

                return Math.Abs(amountReceived - dblAmtApplied);
            }
            set
            {
            }
        }

        public string ClientName
        {
            get
            {
                return clientName;
            }
            set
            {
                if (clientName != value)
                {
                    clientName = value;
                    RaisePropertyChanged("ClientName");
                }
            }
        }


        public int DDCCAccountID
        {
            get
            {
                return ddccAccountID;
            }
            set
            {
                BankDetail bankDetail;

                if (ddccAccountID != value)
                {
                    ddccAccountID = value;

                    if (value > 0)
                    {
                        if (batchType == ReceiptBatchType.DirectDebit)
                        {
                            receipt.DirectDebitReceiptDetails.FirstOrDefault().LesseeBankAccountID = ddccAccountID;
                        }
                        else if(batchType == ReceiptBatchType.CreditCard)
                        {
                            receipt.CreditCardReceiptDetails.FirstOrDefault().LesseeBankAccountID = ddccAccountID;
                        }
                       
                        bankDetail = DDCCBatchFunctions.GetBankDetails(ddccAccountID);
                        DDCCBankDetail.AccountName = bankDetail.AccountName;
                        DDCCBankDetail.BankName = bankDetail.BankName;
                        DDCCBankDetail.BSBNO = bankDetail.BSBNO;
                    }
                    else
                    {
                        DDCCBankDetail.AccountName = string.Empty;
                        DDCCBankDetail.BankName = string.Empty;
                        DDCCBankDetail.BSBNO = string.Empty;
                    }

                    RaisePropertyChanged("DDCCAccountID");
                }
            }
        }

        public string ApplyToLabelCaption
        {
            get
            {
                return receiptApplyTo.ToString() + " No.";                
            }
            set
            {
            }
        }        

        public string DDCCAccountNoLabelCaption
        {
            get
            {
                if (batchType == ReceiptBatchType.DirectDebit)
                {
                    return "Account No.:";
                }
                else if (batchType == ReceiptBatchType.CreditCard)
                {
                    return "Card No.:";
                }

                return string.Empty;
            }
        }

        public bool IsChequeEnabled
        {
            get
            {
                return cashChequePaymentTypes.Where(payType => payType.PaymentTypeId == paymentType && payType.SCPaymentTypeId == (int)SCPaymentTypes.Cheque).Count() > 0;
            }
            set
            {
            }
        }

        public bool IsChequeExpanded
        {
            get
            {
                return isChequeExpanded;
            }
            set
            {
                if (isChequeExpanded != value)
                {
                    isChequeExpanded = value;
                    RaisePropertyChanged("IsChequeExpanded");
                }
            }
        }

        public bool IsApplyPaymentsApplicable
        {
            get
            {
                switch (receiptApplyTo)
                {
                    case ReceiptApplyTo.Contract:
                    case ReceiptApplyTo.Client:
                    case ReceiptApplyTo.Invoice:
                        return true;
                    case ReceiptApplyTo.Quote:                    
                    case ReceiptApplyTo.Suspense:
                        return false;
                }

                return false;
            }
            set
            {
            }
        }

        public bool IsContractNoApplicable
        {
            get
            {
                switch (receiptApplyTo)
                {
                    case ReceiptApplyTo.Client:
                    case ReceiptApplyTo.Invoice:
                        return true;
                    default:
                        return false;
                }
            }
            set
            {
            }
        }

        public bool IsSuspense
        {
            get
            {
                return (receiptApplyTo == ReceiptApplyTo.Suspense);
            }
            set
            {
            }
        }

        public bool IsReceiptEditable
        {
            get
            {
                return isReceiptEditable;
            }
            set
            {                
            }
        }

        public bool IsAddOtherChargeApplicable
        {
            get
            {
                if(applyToObjectID.HasValue && applyToObjectID != 0 && (receiptApplyTo == ReceiptApplyTo.Contract || receiptApplyTo == ReceiptApplyTo.Invoice))
                {
                    return IsValidObjectID();
                }

                return false;
            }
        }

        public bool IncludeFuture
        {
            get
            {
                return includeFuture;
            }
            set
            {
                if (includeFuture != value)
                {
                    includeFuture = value;
                    SearchOpenItems(true);
                    RaisePropertyChanged("IncludeFuture");
                }
            }
        }       

        public bool IncludeClosed
        {
            get
            {
                return includeClosed;
            }
            set
            {
                if (includeClosed != value)
                {
                    includeClosed = value;
                    SearchOpenItems(true);
                    RaisePropertyChanged("IncludeClosed");
                }
            }
        }

        public bool IsReceiptinEditMode
        {
            get
            {
                return receiptID != 0;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (title != value)
                {
                    title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }       

        public string Error
        {
            get 
            {
                return null;
            }
        }        

        public string this[string columnName]
        {
            get 
            {
                if (isChanged == false)
                {
                    return string.Empty;
                } 

                switch (columnName.ToLower())
                {
                    case "applytoobjectid":
                        return ValidateObjectID(); 
                    case "amountreceived":
                        if (amountReceived == 0)
                        {
                            return "Receipt Amount is Required";
                        }
                        break;
                    case "ddccaccountid":
                        if ((batchType == ReceiptBatchType.DirectDebit || batchType == ReceiptBatchType.CreditCard) && ddccAccountID <= 0)
                        {
                            return "Account No. is Required";
                        }
                        break;
                }

                return string.Empty;
            }
        }

        private string ValidateObjectID()
        {
            int contractNo;
            bool isValid = true;

            if (applyToObjectID.HasValue)
            {
                isValid = IsValidObjectID();

                if (isValid == false)
                {
                    return ApplyToLabelCaption + " is Invalid";
                }
                else if (receiptApplyTo == ReceiptApplyTo.Quote)
                {
                    if (business.QuoteFunctions.IsWithDrawn(applyToObjectID.Value))
                    {
                        return "Quote has been withdrawn";
                    }
                    else
                    {
                        contractNo = business.QuoteFunctions.GetSettledContract(applyToObjectID.Value);

                        if (contractNo != -1)
                        {
                            return "Quote has been settled. Please apply receipt to Contract No.: " + contractNo;
                        }
                    }
                }
            }
            else if(isChanged)
            {
                return ApplyToLabelCaption + " is Required";
            }

            return string.Empty;
        }   
       
        private void OnSave(string isNext)
        {
            bool isChequePayment = false;
            
            isChanged = true;

            if (Validate())
            {
                if (cashChequePaymentTypes != null)
                {
                    isChequePayment = cashChequePaymentTypes.Where(payType => payType.PaymentTypeId == paymentType && payType.SCPaymentTypeId == (int)SCPaymentTypes.Cheque).Count() > 0;
                }

                if (IsApplyPaymentsApplicable && OutOfBalance != 0)
                {
                    ShowMessage("Please balance receipt before continuing", "Receipt Balance - Error");
                    return;
                }

                if (Receipt.ReceiptDate > DateTime.Today)
                {
                    UIConfirmation.Raise(
                        new ConfirmationWindowViewModel(this) { Content = "Receipt Date is in the future. Select OK to continue or Cancel to modify", Icon = "Question.ico", Title = "Receipt Date Confirmation" },
                        (popupCallBack) =>
                        {
                            if (popupCallBack.Confirmed)
                            {
                                SaveReceipt(isNext, isChequePayment);
                            }
                        });
                }
                else
                {
                    SaveReceipt(isNext, isChequePayment);
                }
            }
        }

        private void SaveReceipt(string isNext, bool isChequePayment)
        {
            List<Receipt> receipts;
            Receipt receipt = null;
            OpenItemReceiptAllocation openItemReceiptAllocation;
            CashReceiptDetail cashReceiptDetail;
            int refNo = 0;
            isChanged = true;

            try
            {
                receipts = new List<Receipt>();

                if (IsApplyPaymentsApplicable)
                {
                    foreach (OpenItemSearch openItem in openItems.Where(item => item.AmountDue > 0).OrderBy(item => item.OpenItemID))
                    {
                        if (refNo != openItem.ContractNo || IsApplyPaymentsApplicable == false)
                        {
                            receipt = new Receipt();

                            if (receipts.Count == 0 && receiptID != 0)
                            {
                                receipt.ID = Receipt.ID;
                            }

                            receipt.ReceiptBatchID = Receipt.ReceiptBatchID;
                            receipt.ReceiptDate = Receipt.ReceiptDate;
                            receipt.ApplyToTypeID = Receipt.ApplyToTypeID;
                            receipt.Reference = Receipt.Reference;
                            receipt.LastUserID = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
                            receipt.ContractID = openItem.ContractNo;
                            receipt.InternalReference = receipt.ContractID.ToString();

                            if (receiptApplyTo == ReceiptApplyTo.Invoice)
                            {
                                receipt.InvoiceAssetId = business.InvoiceFunctions.GetInvoiceAssetID(openItem.ContractNo, applyToObjectID.GetValueOrDefault());
                                receipt.InternalReference = receipt.InvoiceAssetId.GetValueOrDefault().ToString();
                            }

                            if (batchType == ReceiptBatchType.CashCheque)
                            {
                                if (isChequePayment)
                                {
                                    receipt.ChequeReceiptDetail = new ChequeReceiptDetail();
                                    receipt.ChequeReceiptDetail.AccountName = Receipt.ChequeReceiptDetail.AccountName;
                                    receipt.ChequeReceiptDetail.BankName = Receipt.ChequeReceiptDetail.BankName;
                                    receipt.ChequeReceiptDetail.BSBNumber = Receipt.ChequeReceiptDetail.BSBNumber;
                                    receipt.ChequeReceiptDetail.ChequeNumber = Receipt.ChequeReceiptDetail.ChequeNumber;
                                    receipt.ChequeReceiptDetail.PaymentTypeID = (int)paymentType;
                                    receipt.ChequeReceiptDetail.ReceiptID = receipt.ID;
                                    receipt.ChequeReceiptDetail.Receipt = receipt;
                                }
                                else
                                {
                                    receipt.ChequeReceiptDetail = null;
                                    cashReceiptDetail = new CashReceiptDetail();
                                    cashReceiptDetail.ReceiptID = receipt.ID;

                                    if (receipts.Count == 0 && Receipt.CashReceiptDetails != null && Receipt.CashReceiptDetails.Count > 0)
                                    {
                                        cashReceiptDetail.ID = Receipt.CashReceiptDetails.FirstOrDefault().ID;
                                    }

                                    cashReceiptDetail.PaymentTypeID = (int)paymentType;
                                    receipt.CashReceiptDetails = new List<CashReceiptDetail>();
                                    receipt.CashReceiptDetails.Add(cashReceiptDetail);
                                }
                            }
                            else if (batchType == ReceiptBatchType.DirectDebit)
                            {
                                receipt.DirectDebitReceiptDetails = new List<DirectDebitReceiptDetail>();

                                if (receipts.Count == 0)
                                {
                                    receipt.DirectDebitReceiptDetails.Add(new DirectDebitReceiptDetail { LesseeBankAccountID = DDCCAccountID, ID = Receipt.DirectDebitReceiptDetails.FirstOrDefault().ID, ReceiptID = Receipt.ID });
                                }
                                else
                                {
                                    receipt.DirectDebitReceiptDetails.Add(new DirectDebitReceiptDetail { LesseeBankAccountID = DDCCAccountID });
                                }
                            }
                            else if (batchType == ReceiptBatchType.CreditCard)
                            {
                                receipt.CreditCardReceiptDetails = new List<CreditCardReceiptDetail>();

                                if (receipts.Count == 0)
                                {
                                    receipt.CreditCardReceiptDetails.Add(new CreditCardReceiptDetail { LesseeBankAccountID = DDCCAccountID, ID = Receipt.CreditCardReceiptDetails.FirstOrDefault().ID, ReceiptID = Receipt.ID });
                                }
                                else
                                {
                                    receipt.CreditCardReceiptDetails.Add(new CreditCardReceiptDetail { LesseeBankAccountID = DDCCAccountID });
                                }
                            }                          

                            receipt.OpenItemReceiptAllocations = new List<OpenItemReceiptAllocation>();
                            receipts.Add(receipt);
                            refNo = openItem.ContractNo;
                        }

                        openItemReceiptAllocation = new OpenItemReceiptAllocation();
                        openItemReceiptAllocation.OpenItemID = openItem.OpenItemID;
                        openItemReceiptAllocation.ReceiptID = receiptID;
                        openItemReceiptAllocation.GrossAmountDue = openItem.AmountDue;
                        openItemReceiptAllocation.GrossAmountApplied = Convert.ToDecimal(openItem.AmountApplied);

                        receipt.OpenItemReceiptAllocations.Add(openItemReceiptAllocation);
                    }
                }
                else
                {
                    receipt = new Receipt();

                    receipt.ID = Receipt.ID;
                    receipt.ReceiptBatchID = Receipt.ReceiptBatchID;
                    receipt.ReceiptDate = Receipt.ReceiptDate;
                    receipt.ApplyToTypeID = Receipt.ApplyToTypeID;
                    receipt.Reference = Receipt.Reference;
                    receipt.LastUserID = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
                    receipt.GrossAmountReceived = amountReceived;
                    receipt.NetAmountReceived = amountReceived;

                    if (receiptApplyTo == ReceiptApplyTo.Quote)
                    {
                        receipt.QuoteID = applyToObjectID.GetValueOrDefault();
                        receipt.InternalReference = receipt.QuoteID.ToString();
                    }

                    if (batchType == ReceiptBatchType.CashCheque)
                    {
                        if (isChequePayment)
                        {
                            receipt.ChequeReceiptDetail = new ChequeReceiptDetail();
                            receipt.ChequeReceiptDetail.AccountName = Receipt.ChequeReceiptDetail.AccountName;
                            receipt.ChequeReceiptDetail.BankName = Receipt.ChequeReceiptDetail.BankName;
                            receipt.ChequeReceiptDetail.BSBNumber = Receipt.ChequeReceiptDetail.BSBNumber;
                            receipt.ChequeReceiptDetail.ChequeNumber = Receipt.ChequeReceiptDetail.ChequeNumber;
                            receipt.ChequeReceiptDetail.PaymentTypeID = (int)paymentType;
                            receipt.ChequeReceiptDetail.ReceiptID = receipt.ID;
                            receipt.ChequeReceiptDetail.Receipt = receipt;
                        }
                        else
                        {
                            receipt.ChequeReceiptDetail = null;
                            receipt.CashReceiptDetails = new List<CashReceiptDetail>();
                            receipt.CashReceiptDetails.Add(new CashReceiptDetail() { ReceiptID = receipt.ID, PaymentTypeID = (int)paymentType });

                            if (receipts.Count == 0 && Receipt.CashReceiptDetails != null && Receipt.CashReceiptDetails.Count > 0)
                            {
                                receipt.CashReceiptDetails.FirstOrDefault().ID = Receipt.CashReceiptDetails.FirstOrDefault().ID;
                            }
                        }
                    }
                    else if (batchType == ReceiptBatchType.DirectDebit)
                    {
                        receipt.DirectDebitReceiptDetails = new List<DirectDebitReceiptDetail>();

                        if (receipts.Count == 0)
                        {
                            receipt.DirectDebitReceiptDetails.Add(new DirectDebitReceiptDetail { LesseeBankAccountID = DDCCAccountID, ID = Receipt.DirectDebitReceiptDetails.FirstOrDefault().ID, ReceiptID = Receipt.ID });
                        }
                        else
                        {
                            receipt.DirectDebitReceiptDetails.Add(new DirectDebitReceiptDetail { LesseeBankAccountID = DDCCAccountID });
                        }
                    }
                    else if (batchType == ReceiptBatchType.CreditCard)
                    {
                        receipt.CreditCardReceiptDetails = new List<CreditCardReceiptDetail>();

                        if (receipts.Count == 0)
                        {
                            receipt.CreditCardReceiptDetails.Add(new CreditCardReceiptDetail { LesseeBankAccountID = DDCCAccountID, ID = Receipt.CreditCardReceiptDetails.FirstOrDefault().ID, ReceiptID = Receipt.ID });
                        }
                        else
                        {
                            receipt.CreditCardReceiptDetails.Add(new CreditCardReceiptDetail { LesseeBankAccountID = DDCCAccountID });
                        }
                    }                    

                    receipts.Add(receipt);
                }

                ReceiptFunctions.Save(receipts);

                isChanged = false;

                if (Convert.ToBoolean(isNext))
                {
                    receiptID = 0;
                    SetReceiptDefaults(Receipt.ReceiptBatchID);
                    SetIcon();
                    RaisePropertyChanged("IsReceiptinEditMode");
                }
                else
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error occurred while Saving Receipt", "Add Receipt - Error");
                IsBusy = false;
            }
        }

        private void OnSearch()
        {
            IsBusy = true;
            switch (receiptApplyTo)
            {
                case ReceiptApplyTo.Client:
                    PopupWindow<ClientSearch> clientPopup;
                    DelegateSearch<ClientSearch> clientSearch = new DelegateSearch<ClientSearch>(business.ClientFunctions.Search);
                    clientPopup = new PopupWindow<ClientSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<ClientSearch>(clientSearch, "Search Client"), true);

                    Popup.Raise(clientPopup, (popupCallBack) =>
                    {
                        if (popupCallBack.ReturnValue != null)
                        {
                            ClientSearch client = (ClientSearch)popupCallBack.ReturnValue;                        
                            ApplyToObjectID = client.ClientNo;
                            ClientName = client.LegalName;
                            RaisePropertyChanged("ApplyToObjectID");
                        }
                    });
                    break;
                case ReceiptApplyTo.Contract:
                    PopupWindow<ContractSearch> contractPopup;
                    DelegateSearch<ContractSearch> contractSearch;
                    DelegateSearchFilter<ContractSearch> ddccContractSearch;
                    object[] filter;

                    if (batchType == ReceiptBatchType.DirectDebit || batchType == ReceiptBatchType.CreditCard || batchType == ReceiptBatchType.AutoReceipts)
                    {
                        if (batchType == ReceiptBatchType.DirectDebit)
                        {
                            filter = new object[] { (int)SCPaymentTypes.DirectDebit, (int)SCPaymentTypes.DirectDebit50 };
                        }
                        else if (batchType == ReceiptBatchType.CreditCard)
                        {
                            filter = new object[] { (int)SCPaymentTypes.CreditCard };
                        }
                        else
                        {
                            filter = new object[] { (int)SCPaymentTypes.AutoReceipt };
                        }

                        ddccContractSearch = new DelegateSearchFilter<ContractSearch>(DDCCBatchFunctions.ContractSearch);
                        contractPopup = new PopupWindow<ContractSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<ContractSearch>(ddccContractSearch, "Search Contract", filter), true);
                    }
                    else
                    {
                        contractSearch = new DelegateSearch<ContractSearch>(ContractFunctions.Search);
                        contractPopup = new PopupWindow<ContractSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<ContractSearch>(contractSearch, "Search Contract"), true);
                    }

                    Popup.Raise(contractPopup, (popupCallBack) =>
                    {
                        if (popupCallBack.ReturnValue != null)
                        {
                            ContractSearch contract = (ContractSearch)popupCallBack.ReturnValue;                            
                            ApplyToObjectID = contract.ContractID;
                            ClientName = contract.ClientName;
                            RaisePropertyChanged("ApplyToObjectID");
                        }

                        popupCallBack = null;
                    });
                    break;

                case ReceiptApplyTo.Invoice:
                    PopupWindow<InvoiceSearch> invoicePopup;
                    DelegateSearch<InvoiceSearch> invoiceSearch = new DelegateSearch<InvoiceSearch>(business.InvoiceFunctions.Search);
                    invoicePopup = new PopupWindow<InvoiceSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<InvoiceSearch>(invoiceSearch, "Search Invoice"), true);

                    Popup.Raise(invoicePopup, (popupCallBack) =>
                    {
                        if (popupCallBack.ReturnValue != null)
                        {
                            InvoiceSearch invoice = (InvoiceSearch)popupCallBack.ReturnValue;                            
                            ApplyToObjectID = invoice.InvoiceNo;
                            ClientName = invoice.CustomerName;
                            RaisePropertyChanged("ApplyToObjectID");
                        }
                    });
                    break;
                case ReceiptApplyTo.Quote:
                    PopupWindow<QuoteSearch> quotePopup;
                    DelegateSearch<QuoteSearch> quoteSearch = new DelegateSearch<QuoteSearch>(business.QuoteFunctions.Search);
                    quotePopup = new PopupWindow<QuoteSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<QuoteSearch>(quoteSearch, "Search Quote"), true);

                    Popup.Raise(quotePopup, (popupCallBack) =>
                    {
                        if (popupCallBack.ReturnValue != null)
                        {
                            QuoteSearch quote = (QuoteSearch)popupCallBack.ReturnValue;
                            
                            ApplyToObjectID = quote.QuoteNo;
                            ClientName = quote.ClientName;
                            RaisePropertyChanged("ApplyToObjectID");
                        }

                    });
                    break;
            }
            IsBusy = false;
        }

        private void OnCalculateBalance()
        {
            RaisePropertyChanged("OutOfBalance");
            RaisePropertyChanged("PendingBalanceText");
        }

        private void OnOtherCharge()
        {
            PopupWindow popupWindow;

            IsBusy = true;
            popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "CommonControls.AddOtherCharge", true);
            popupWindow.Parameters.Add(applyToObjectID.Value);
            Popup.Raise(popupWindow, (popupCallBack) => { if (popupCallBack.ReturnValue != null && Convert.ToBoolean(popupCallBack.ReturnValue) == true) SearchOpenItems(true); });
            IsBusy = false;
        }

        private void SearchOpenItems(bool? cacheAppliedAmount = false)
        {
            OpenItemSearch openItem;
            Dictionary<int, decimal> openItemAmtsApplied;
            openItemAmtsApplied = new Dictionary<int, decimal>();

            if (IsApplyPaymentsApplicable == false || applyToObjectID.HasValue == false || IsValidObjectID() == false)
            {
                return;
            }

            IsBusy = true;

            if (cacheAppliedAmount.GetValueOrDefault() && amountReceived != 0 && openItems != null && openItems.Count > 0)
            {
                foreach (OpenItemSearch item in openItems)
                {
                    if (item.AmountApplied != 0)
                    {
                        openItemAmtsApplied.Add(item.OpenItemID, item.AmountApplied);
                    }
                }
            }

            OpenItems = null;

            if (batchType == ReceiptBatchType.CashCheque)
            {
                OpenItems = OpenItemFunctions.GetOpenItemsSearch(receiptApplyTo, applyToObjectID.Value, includeFuture, includeClosed, (ReceiptPrimaryAllocation)Convert.ToInt32(batchTypeDefaults.ApplyToOption.GetValueOrDefault()), receiptID);
            }
            else if(batchType == ReceiptBatchType.DirectDebit || batchType == ReceiptBatchType.CreditCard || batchType == ReceiptBatchType.AutoReceipts)
            {
                OpenItems = OpenItemFunctions.GetDDCCOpenItemsSearch(batchType, receiptApplyTo, applyToObjectID.Value, includeFuture, includeClosed, 
                    (ReceiptPrimaryAllocation)Convert.ToInt32(batchTypeDefaults.ApplyToOption.GetValueOrDefault()), receiptID);
            }

            if (isReceiptLoading)
            {
                IsBusy = false;
                return;
            }

            if (cacheAppliedAmount.GetValueOrDefault() && openItemAmtsApplied != null && openItemAmtsApplied.Count > 0)
            {
                foreach (KeyValuePair<int, decimal> item in openItemAmtsApplied)
                {
                    openItem = openItems.Where(opitem => opitem.OpenItemID == item.Key).FirstOrDefault();

                    if (openItem != null)
                    {
                        openItem.AmountApplied = item.Value;
                    }
                }
            }
            else if(amountReceived != 0)
            {
                OpenItemFunctions.ApplyPayments(batchType, OpenItems, amountReceived, receiptApplyTo, applyToObjectID.GetValueOrDefault(), batchTypeDefaults, receiptID);
            }

            if (amountReceived != 0)
            {
                GridViewTotalsChanged();
            }

            IsBusy = false;

            RaisePropertyChanged("PendingBalanceText");
            RaisePropertyChanged("OutOfBalance");
            ClearGridFilter();
        }

        private void SetReceiptDefaults(int receiptBatchID)
        {                        
            Receipt = new Receipt();            
            Receipt.ReceiptBatchID = receiptBatchID;
            Receipt.ReceiptDate = DateTime.Today;
            ApplyTo = (int)ReceiptApplyTo.Contract;

            AmountReceived = 0;
            PaymentType = 0;

            if (batchType != 0)
            {
                batchTypeDefaults = BatchTypeFunctions.GetReceiptBatchSystemDefaults((int)batchType);
            }

            if (batchType == ReceiptBatchType.CashCheque)
            {
                CashChequePaymentTypes = PaymentTypeFunctions.GetCashChequePaymentTypes();

                if(CashChequePaymentTypes.Count > 0)
                {
                    PaymentType = CashChequePaymentTypes.FirstOrDefault().PaymentTypeId;
                }

                Receipt.ChequeReceiptDetail = new ChequeReceiptDetail { PaymentTypeID = 0 };
            }

            if (batchType == ReceiptBatchType.DirectDebit)
            {
                receipt.DirectDebitReceiptDetails = new List<DirectDebitReceiptDetail>();
                receipt.DirectDebitReceiptDetails.Add(new DirectDebitReceiptDetail() { BankDetail = new BankDetail { } });
            }
            else if(batchType == ReceiptBatchType.CreditCard)
            {
                receipt.CreditCardReceiptDetails = new List<CreditCardReceiptDetail>();
                receipt.CreditCardReceiptDetails.Add(new CreditCardReceiptDetail() { BankDetail = new BankDetail { } });
            }                

            RaisePropertyChanged("Receipt");
        }

        void OpenItem_Errors(object sender, DataErrorsChangedEventArgs e)
        {
            OpenItemSearch openItem;

            openItem = sender as OpenItemSearch;

            if (openItem != null)
            {
                if (openItem.HasErrors)
                {
                    AddErrorMessage(e.PropertyName + openItem.OpenItemID.ToString(), openItem.GetErrorMessage(e.PropertyName).FirstOrDefault());
                }
                else
                {
                    RemoveErrorMessage(e.PropertyName + openItem.OpenItemID.ToString());
                }
            }
        }

        private bool IsValidObjectID()
        {
            bool isValid = false;

            switch (receiptApplyTo)
            {
                case ReceiptApplyTo.Client:
                    isValid = business.ClientFunctions.ClientExists(applyToObjectID.Value);
                    break;
                case ReceiptApplyTo.Contract:
                    isValid = business.ContractFunctions.ContractExists(applyToObjectID.Value);
                    break;
                case ReceiptApplyTo.Invoice:
                    isValid = business.InvoiceFunctions.InvoiceExists(applyToObjectID.Value);
                    break;
                case ReceiptApplyTo.Quote:
                    isValid = business.QuoteFunctions.QuoteExists(applyToObjectID.Value);
                    break;
            }

            return isValid;
        }

        private void SetIcon()
        {
            if (receiptID == 0)
            {
                IconFileName = "Add.jpg";
            }
            else
            {
                IconFileName = "Edit.jpg";
            }

            switch (batchType)
            {
                case ReceiptBatchType.CashCheque:
                    if (receiptID == 0)
                    {
                        Title = "New Cash Receipt";
                    }
                    else if (isReceiptEditable == false)
                    {
                        Title = "View Cash Receipt";
                        IconFileName = "View.jpg";
                    }
                    else
                    {                        
                        Title = "Modify Cash Receipt";
                    }
                    break;
                case ReceiptBatchType.DirectDebit:
                    if (receiptID == 0)
                    {
                        Title = "New Direct Debit Receipt";
                    }
                    else if (isReceiptEditable == false)
                    {
                        Title = "View Direct Debit Receipt";
                        IconFileName = "View.jpg";
                    }
                    else
                    {
                        Title = "Modify Direct Debit Receipt";
                    }
                    break;
                case ReceiptBatchType.CreditCard:
                    if (receiptID == 0)
                    {
                        Title = "New Credit Card Receipt";
                    }
                    else if (isReceiptEditable == false)
                    {
                        Title = "View Credit Card Receipt";
                        IconFileName = "View.jpg";
                    }
                    else
                    {
                        Title = "Modify Credit Card Receipt";
                    }
                    break;
                case ReceiptBatchType.Dishonour:
                    if (receiptID == 0)
                    {
                        Title = "New Dishonour Receipt";
                    }
                    else if (isReceiptEditable == false)
                    {
                        Title = "View Dishonour Receipt";
                        IconFileName = "View.jpg";
                    }
                    else
                    {
                        Title = "Modify Dishonour Receipt";
                    }
                    break;
                case ReceiptBatchType.Reversals:
                    if (receiptID == 0)
                    {
                        Title = "New Reversal Receipt";
                    }
                    else if (isReceiptEditable == false)
                    {
                        Title = "View Reversal Receipt";
                        IconFileName = "View.jpg";
                    }
                    else
                    {
                        Title = "Modify Reversal Receipt";
                    }
                    break;
                case ReceiptBatchType.AutoReceipts:
                    if (receiptID == 0)
                    {
                        Title = "New Auto Receipt";
                    }
                    else if (isReceiptEditable == false)
                    {
                        Title = "View Auto Receipt";
                        IconFileName = "View.jpg";
                    }
                    else
                    {
                        Title = "Modify Auto Receipt";
                    }
                    break;
            }            
        }      
    }
}
