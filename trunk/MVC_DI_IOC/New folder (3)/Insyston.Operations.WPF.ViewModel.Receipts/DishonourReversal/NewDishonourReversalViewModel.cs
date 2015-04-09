using Insyston.Operations.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Insyston.Operations.Logging;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Business.Receipts.Model;
using Insyston.Operations.WPF.ViewModel.Common;
using business = Insyston.Operations.Business.Common;
using Insyston.Operations.WPF.ViewModel.Common.Controls;
using Insyston.Operations.Security;
using System.Threading;
using Insyston.Operations.Common;

namespace Insyston.Operations.WPF.ViewModel.Receipts.DishonourReversal
{
    public class NewDishonourReversalViewModel : OldViewModelBase, IDataErrorInfo
    {
        private readonly ReceiptBatchType batchType;

        private string title, clientName;
        private int receiptID, reasonSelected;
        private int? applyToObjectID, reAllocationReceiptID;
        internal readonly int ReceiptBatchID, BatchStatus, BankAccountID;
        public readonly bool IsResetAllocation;
        private string reference;

        private DateTime receiptDate;
        protected bool isChanged, isReallocateOK;
        private readonly bool isReceiptEditable;

        private List<DropdownList> reasonCodes;        
        private Receipt receipt;
        private List<OpenItemSearch> openItems;
        private Tuple<int, int?, string> returnValue;

        public InteractionRequest<ConfirmationWindowViewModel> UIConfirmation { get; private set; }
        public InteractionRequest<PopupWindow> Popup { get; private set; }

        public delegate void ClearGridFilterEventHandler();
        public event ClearGridFilterEventHandler ClearGridFilter;

        public DelegateCommand<string> Save { get; private set; }
        public DelegateCommand Search { get; private set; }
        public DelegateCommand ResetAllocation { get; private set; }
        
        public delegate void GridViewTotalsChangedHandler();
        public event GridViewTotalsChangedHandler GridViewTotalsChanged;

        public NewDishonourReversalViewModel()
        {
        }

        public NewDishonourReversalViewModel(ReceiptBatchType receiptbatchType, int batchID, int batchStatus, int receiptid = 0)
        {
            Receipt receipt, reAllocationReceipt;

            batchType = receiptbatchType;
            ReceiptBatchID = batchID;
            BatchStatus = batchStatus;
            receiptID = receiptid;
            LockTableName = "ReceiptBatch";
            LockUniqueIdentifier = batchID.ToString();
            isReceiptEditable = batchStatus == (int)ReceiptBatchStatus.Created && IsLocked;

            try
            {
                ReasonCodes = DishonourReversalFunctions.GetReasonCodes(batchType);
                BankAccountID = ReceiptBatchFunctions.Get(ReceiptBatchID).BankAccountID.GetValueOrDefault();

                Search = new DelegateCommand(OnSearch);
                Save = new DelegateCommand<string>(OnSave);
                ResetAllocation = new DelegateCommand(OnResetAllocation);
                
                UIConfirmation = new InteractionRequest<ConfirmationWindowViewModel>();
                Popup = new InteractionRequest<PopupWindow>();

                SetIcon();

                if (receiptID > 0)
                {
                    receipt = ReceiptFunctions.Get(receiptID);
                    ReceiptDate = receipt.ReceiptDate;
                    Receipt = new Receipt();
                    Reference = receipt.Reference;

                    if (batchType == ReceiptBatchType.Dishonour)
                    {
                        OriginalReceiptID = receipt.DishonourReceiptDetail.DishonouredReceiptID;
                        reasonSelected = receipt.DishonourReceiptDetail.ReasonCodeID.GetValueOrDefault();
                    }
                    else if (batchType == ReceiptBatchType.Reversals)
                    {
                        reasonSelected = receipt.ReversalReceiptDetail.ReasonCodeID.GetValueOrDefault();

                        if (receipt.ReversalReceiptDetail.ReversalTypeID == (int)ReversalTypes.Reversal)
                        {
                            OriginalReceiptID = receipt.ReversalReceiptDetail.LinkedReceiptID.GetValueOrDefault();
                            ReAllocationReceiptID = DishonourReversalFunctions.GetReAllocationReceiptID(receiptID);

                            if (reAllocationReceiptID.HasValue)
                            {
                                reAllocationReceipt = ReceiptFunctions.Get(reAllocationReceiptID.Value, false);

                                if (reAllocationReceipt.ApplyToTypeID == (int)ReceiptApplyTo.Contract)
                                {
                                    ReturnValue = new Tuple<int, int?, string>((int)ReAllocateReceiptTo.Contract, reAllocationReceipt.ContractID, reAllocationReceipt.Reference);
                                }
                                else if (reAllocationReceipt.ApplyToTypeID == (int)ReceiptApplyTo.Quote)
                                {
                                    ReturnValue = new Tuple<int, int?, string>((int)ReAllocateReceiptTo.Quote, reAllocationReceipt.QuoteID, reAllocationReceipt.Reference);
                                }
                                else
                                {
                                    ReturnValue = new Tuple<int, int?, string>((int)ReAllocateReceiptTo.Suspense, reAllocationReceipt.ContractID, reAllocationReceipt.Reference);
                                }
                            }
                        }
                        else
                        {
                            ReAllocationReceiptID = receiptID;

                            if (receipt.ApplyToTypeID == (int)ReceiptApplyTo.Contract)
                            {
                                ReturnValue = new Tuple<int, int?, string>((int)ReAllocateReceiptTo.Contract, receipt.ContractID, receipt.Reference);
                            }
                            else if (receipt.ApplyToTypeID == (int)ReceiptApplyTo.Quote)
                            {
                                ReturnValue = new Tuple<int, int?, string>((int)ReAllocateReceiptTo.Quote, receipt.QuoteID, receipt.Reference);
                            }
                            else
                            {
                                ReturnValue = new Tuple<int, int?, string>((int)ReAllocateReceiptTo.Suspense, receipt.ContractID, receipt.Reference);
                            }

                            receiptID = receipt.ReversalReceiptDetail.LinkedReceiptID.GetValueOrDefault();
                            receipt = ReceiptFunctions.Get(receiptID);
                            Reference = receipt.Reference;
                            OriginalReceiptID = receipt.ReversalReceiptDetail.LinkedReceiptID;
                            IsResetAllocation = true;
                        }
                    }
                }
                else
                {
                    SetReceiptDefaults(ReceiptBatchID);
                }
               
                RaisePropertyChanged("BatchType");
                RaisePropertyChanged("IsReceiptSelected");
                RaisePropertyChanged("IsSuspense");
                RaisePropertyChanged("IsReceiptEditable");
                RaisePropertyChanged("AmountAppliedCaption");
                RaisePropertyChanged("IsReceiptinEditMode");
                RaisePropertyChanged("IsReversalBatch");
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error occurred while initializing Add Receipt Screen", "Add Receipt - Error");
            }
            finally
            {
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

        public Tuple<int, int?, string> ReturnValue
        {
            get
            {
                return returnValue;
            }
            set
            {
                if (returnValue != value)
                {
                    returnValue = value;
                    RaisePropertyChanged("ReturnValue");
                }
            }
        }

        public List<DropdownList> ReasonCodes
        {
            get
            {
                return reasonCodes;
            }
            set
            {
                if (reasonCodes != value)
                {
                    reasonCodes = value;
                    RaisePropertyChanged("ReasonCodes");
                }
            }
        }

        public List<OpenItemSearch> OpenItems
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
                    RaisePropertyChanged("OpenItems");
                }
            }
        }   
        
        public int ReceiptID
        {
            get
            {
                return receiptID;
            }
            set
            {
                if (receiptID != value)
                {
                    receiptID = value;
                    RaisePropertyChanged("ReceiptID");
                    RaisePropertyChanged("IsReceiptinEditMode");
                }
            }
        }

        public int? ReAllocationReceiptID
        {
            get
            {
                return reAllocationReceiptID;
            }
            set
            {
                if (reAllocationReceiptID != value)
                {
                    reAllocationReceiptID = value;
                    RaisePropertyChanged("ReAllocationReceiptID");
                    RaisePropertyChanged("IsReceiptinEditMode");
                }
            }
        }

        public string Reference
        {
            get
            {
                return reference;
            }
            set
            {
                if (reference != value)
                {
                    reference = value;
                    RaisePropertyChanged("Reference");
                }
            }
        }

        public int? OriginalReceiptID
        {
            get
            {
                if (receipt.ID != 0)
                {
                    return receipt.ID;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                DropdownList client = null;

                if (value.HasValue)
                {
                    if (receipt.ID != value.Value)
                    {
                        receipt.ID = value.GetValueOrDefault();
                        receipt.SystemConstant = null;
                        OpenItems = null;
                        ApplyToObjectID = null;

                        if (receiptID != 0 || DishonourReversalFunctions.IsValidDishonourReversalReceipt((int)batchType, receipt.ID, receiptID))
                        {
                            Receipt = ReceiptFunctions.Get(receipt.ID);

                            if (batchType == ReceiptBatchType.Reversals)
                            {
                                ReceiptDate = receipt.ReceiptDate;
                            }

                            if (receipt.ApplyToTypeID != (int)ReceiptApplyTo.Suspense)
                            {
                                client = ClientFunctions.GetReceiptClient(receipt.ID);

                                ClientName = client.Description;
                            }

                            if (receipt.ApplyToTypeID == (int)ReceiptApplyTo.Contract)
                            {
                                ApplyToObjectID = receipt.ContractID;
                            }
                            else if (receipt.ApplyToTypeID == (int)ReceiptApplyTo.Quote)
                            {
                                ApplyToObjectID = receipt.QuoteID;
                            }
                            else if (receipt.ApplyToTypeID == (int)ReceiptApplyTo.Invoice)
                            {
                                ApplyToObjectID = business.InvoiceFunctions.GetInvoiceNo(receipt.InvoiceAssetId.Value);
                            }
                            else if (receipt.ApplyToTypeID == (int)ReceiptApplyTo.Client)
                            {
                                ApplyToObjectID = client.ID;
                            }

                            OpenItems = DishonourReversalFunctions.GetAllocatedOpenItems(receipt.ID);
                        }

                        RaisePropertyChanged("OriginalReceiptID");
                        RaisePropertyChanged("ApplyToLabelCaption");
                        RaisePropertyChanged("AccountNumberCaption");
                        RaisePropertyChanged("IsReceiptSelected");
                        RaisePropertyChanged("IsSuspense");
                        RaisePropertyChanged("BankName");
                        RaisePropertyChanged("BSBNo");
                        RaisePropertyChanged("AccountName");
                        RaisePropertyChanged("AccountNumber");
                    }
                }
                else
                {
                    receipt.ID = 0;
                    ClientName = string.Empty;
                    ApplyToObjectID = null;
                    OpenItems = null;
                    receiptID = 0;

                    RaisePropertyChanged("OriginalReceiptID");
                    RaisePropertyChanged("ApplyToLabelCaption");
                    RaisePropertyChanged("AccountNumberCaption");
                    RaisePropertyChanged("IsReceiptSelected");
                    RaisePropertyChanged("IsSuspense");
                    RaisePropertyChanged("BankName");
                    RaisePropertyChanged("BSBNo");
                    RaisePropertyChanged("AccountName");
                    RaisePropertyChanged("AccountNumber");
                    RaisePropertyChanged("ReceiptID");
                    RaisePropertyChanged("IsReceiptinEditMode");
                }
            }
        }

        public DateTime ReceiptDate
        {
            get
            {
                return receiptDate;
            }
            set
            {
                if (receiptDate != value)
                {
                    receiptDate = value;
                    RaisePropertyChanged("ReceiptDate");
                }
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

      
        public string ApplyToLabelCaption
        {
            get
            {   
                if(Receipt.ID != 0 && Receipt.SystemConstant != null)
                {
                    return Receipt.SystemConstant.UserDescription.Trim() + " No.";
                }

                return "Contract No.";
            }
        }       

        public int ReasonSelected
        {
            get
            {
                return reasonSelected;
            }
            set
            {
                if (reasonSelected != value)
                {
                    reasonSelected = value;
                    RaisePropertyChanged("ReasonSelected");
                    RaisePropertyChanged("IsRequiresResetAllocation");
                    RaisePropertyChanged("IsSaveEnabled");
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
                if (applyToObjectID != value)
                {
                    applyToObjectID = value;
                    RaisePropertyChanged("ApplyToObjectID");
                }
            }
        }       

        public string BankName
        {
            get
            {
                if (IsReceiptSelected && Receipt.ChequeReceiptDetail != null)
                {
                    return Receipt.ChequeReceiptDetail.BankName;
                }
                else if (IsReceiptSelected && Receipt.DirectDebitReceiptDetails.Count > 0)
                {
                    return Receipt.DirectDebitReceiptDetails.FirstOrDefault().BankDetail.BankName;
                }
                else if (IsReceiptSelected && Receipt.CreditCardReceiptDetails.Count > 0)
                {
                    return Receipt.CreditCardReceiptDetails.FirstOrDefault().BankDetail.BankName;
                }

                return string.Empty;
            }
        }

        public string BSBNo
        {
            get
            {
                if (IsReceiptSelected && Receipt.ChequeReceiptDetail != null)
                {
                    return FormatBSBNo(Receipt.ChequeReceiptDetail.BSBNumber);
                }
                else if (IsReceiptSelected && Receipt.DirectDebitReceiptDetails.Count > 0)
                {
                    return FormatBSBNo(Receipt.DirectDebitReceiptDetails.FirstOrDefault().BankDetail.BSBNO);
                }
                else if (IsReceiptSelected && Receipt.CreditCardReceiptDetails.Count > 0)
                {
                    return FormatBSBNo(Receipt.CreditCardReceiptDetails.FirstOrDefault().BankDetail.BSBNO);
                }

                return string.Empty;
            }
        }

        public string AccountName
        {
            get
            {
                if (IsReceiptSelected && Receipt.ChequeReceiptDetail != null)
                {
                    return Receipt.ChequeReceiptDetail.AccountName;
                }
                else if (IsReceiptSelected && Receipt.DirectDebitReceiptDetails.Count > 0)
                {
                    return Receipt.DirectDebitReceiptDetails.FirstOrDefault().BankDetail.AccountName;
                }
                else if (IsReceiptSelected && Receipt.CreditCardReceiptDetails.Count > 0)
                {
                    return Receipt.CreditCardReceiptDetails.FirstOrDefault().BankDetail.AccountName;
                }

                return string.Empty;
            }
        }

        public string AccountNumber
        {
            get
            {
                if (IsReceiptSelected && Receipt.ChequeReceiptDetail != null)
                {
                    return Receipt.ChequeReceiptDetail.ChequeNumber;
                }
                else if (IsReceiptSelected && Receipt.DirectDebitReceiptDetails.Count > 0)
                {
                    return Receipt.DirectDebitReceiptDetails.FirstOrDefault().BankDetail.AccountNumber;
                }
                else if (IsReceiptSelected && Receipt.CreditCardReceiptDetails.Count > 0)
                {
                    return Receipt.CreditCardReceiptDetails.FirstOrDefault().BankDetail.AccountNumber;
                }

                return string.Empty;
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

        public string ReceiptDateCaption
        {
            get
            {
                if (batchType == ReceiptBatchType.Dishonour)
                {
                    return "Dish. Date:";
                }
                else if (batchType == ReceiptBatchType.Reversals)
                {
                    return "Reversal Date:";
                }

                return string.Empty;
            }
        }

        public string AmountAppliedCaption
        {
            get
            {
                if (batchType == ReceiptBatchType.Dishonour)
                {
                    return "Dish. Amount";
                }
                else
                {
                    return "Reversal Amount";
                }
            }
        }

        public string AccountNumberCaption
        {
            get
            {
                if (IsReceiptSelected && Receipt.ChequeReceiptDetail != null)
                {
                    return "Cheque Number";
                }

                return "Account Number";
            }
        }

        public bool IsApplyPaymentsApplicable
        {
            get
            {
                if(receipt.ContractID != null)
                {                
                    return true;    
                }

                return false;
            }
            set
            {
            }
        }

        public bool IsReceiptSelected
        {
            get
            {
                return Receipt.SystemConstant != null && OriginalReceiptID.HasValue;
            }
        }

        public bool IsSuspense
        {
            get
            {
                if (IsReceiptSelected == false)
                {
                    return true;
                }
                else
                {
                    return receipt.ApplyToTypeID == (int)ReceiptApplyTo.Suspense;
                }
            }
        }       

        public bool IsReceiptEditable
        {
            get
            {
                return isReceiptEditable;
            }
        }

        public bool IsReceiptinEditMode
        {
            get
            {
                return receiptID != 0;
            }
        }

        public bool IsReAllocationinEditMode
        {
            get
            {
                return reAllocationReceiptID != 0;
            }
        }

        public bool IsReversalBatch
        {
            get
            {
                return batchType == ReceiptBatchType.Reversals;
            }
        }

        public bool IsRequiresResetAllocation
        {
            get
            {
                if (IsReversalBatch && reasonSelected != 0)
                {
                    return DishonourReversalFunctions.GetRequiresReallocation(reasonSelected);
                }

                return false;
            }
        }

        public bool IsSaveEnabled
        {
            get
            {
                return !IsRequiresResetAllocation || ReturnValue != null;
            }
        }

        private void OnSearch()
        {
            PopupWindow<DishonourReversalReceiptSearchBase> popupWindow;

            IsBusy = true;
            DelegateSearchFilter<DishonourReversalReceiptSearchBase> receiptSearch = new DelegateSearchFilter<DishonourReversalReceiptSearchBase>(DishonourReversalFunctions.SearchDishonourReversalReceipts);            
            popupWindow = new PopupWindow<DishonourReversalReceiptSearchBase>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<DishonourReversalReceiptSearchBase>(receiptSearch, "Search Receipt", (int)batchType, BankAccountID), true);

            Popup.Raise(popupWindow, (popupCallBack) =>
            {
                if (popupCallBack.ReturnValue != null)
                {
                    DishonourReversalReceiptSearchBase receipt = (DishonourReversalReceiptSearchBase)popupCallBack.ReturnValue;                    
                    OriginalReceiptID = receipt.ReceiptID;
                }
            });

            IsBusy = false;
        }
       
        private void OnSave(string isNext)
        {
            isChanged = true;

            if (Validate())
            {                
                if (ReceiptDate > DateTime.Today)
                {
                    UIConfirmation.Raise(
                        new ConfirmationWindowViewModel(this) { Content = "Receipt Date is in the future. Select OK to continue or Cancel to modify", Icon = "Question.ico", Title = "Receipt Date Confirmation" },
                        (popupCallBack) =>
                        {
                            if (popupCallBack.Confirmed)
                            {
                                SaveReceipt(isNext);
                            }
                        });
                }
                else
                {
                    SaveReceipt(isNext);
                }
            }
        }

        private void SaveReceipt(string isNext)
        {
            ReceiptBatch receiptBatch;
            Receipt receipt = null;
            OpenItemReceiptAllocation openItemReceiptAllocation;
            isChanged = true;

            try
            {
                receiptBatch = ReceiptBatchFunctions.Get(ReceiptBatchID);

                receipt = new Receipt();
                receipt.ID = receiptID;
                receipt.ApplyToTypeID = Receipt.ApplyToTypeID;
                receipt.InternalReference = Receipt.InternalReference;

                receipt.ReceiptBatchID = ReceiptBatchID;
                receipt.ContractID = Receipt.ContractID;
                receipt.QuoteID = Receipt.QuoteID;
                receipt.InvoiceAssetId = Receipt.InvoiceAssetId;
                receipt.Reference = reference;
                receipt.LastUserID = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
                receipt.LastDateModified = DateTime.Now;
                receipt.ReceiptDate = receiptDate;

                receipt.NetAmountReceived = Receipt.NetAmountReceived.GetValueOrDefault() * -1;
                receipt.GSTAmountReceived = Receipt.GSTAmountReceived.GetValueOrDefault() * -1;
                receipt.FIDAmountReceived = Receipt.FIDAmountReceived.GetValueOrDefault() * -1;
                receipt.SDAmountReceived = Receipt.SDAmountReceived.GetValueOrDefault() * -1;
                receipt.GrossAmountReceived = Receipt.GrossAmountReceived * -1;

                if (batchType == ReceiptBatchType.Dishonour)
                {
                    receipt.DishonourReceiptDetail = new DishonourReceiptDetail { ReceiptID = receiptID, DishonouredReceiptID = Receipt.ID, ReasonCodeID = reasonSelected != 0 ? (int?)reasonSelected : null };
                }
                else
                {
                    receipt.ReversalReceiptDetail = new ReversalReceiptDetail { ReceiptID = receiptID, LinkedReceiptID = Receipt.ID, ReasonCodeID = reasonSelected != 0 ? (int?)reasonSelected : null, ReversalTypeID = (int)ReversalTypes.Reversal };
                }

                foreach (OpenItemReceiptAllocation allocation in DishonourReversalFunctions.GetOpenItemReceiptAllocations(Receipt.ID))
                {
                    openItemReceiptAllocation = new OpenItemReceiptAllocation();

                    openItemReceiptAllocation.ReceiptID = receiptID;
                    openItemReceiptAllocation.OpenItemID = allocation.OpenItemID;
                    openItemReceiptAllocation.NetAmountApplied = allocation.NetAmountApplied * -1;
                    openItemReceiptAllocation.GSTAmountApplied = allocation.GSTAmountApplied * -1;
                    openItemReceiptAllocation.FIDAmountApplied = allocation.FIDAmountApplied * -1;
                    openItemReceiptAllocation.SDAmountApplied = allocation.SDAmountApplied * -1;
                    openItemReceiptAllocation.GrossAmountApplied = allocation.GrossAmountApplied * -1;
                    receipt.OpenItemReceiptAllocations.Add(openItemReceiptAllocation);
                }

                if (receiptBatch.Receipts == null)
                {
                    receiptBatch.Receipts = new List<Receipt>();
                }

                if (receiptID == 0)
                {
                    receiptBatch.NumberOfEntries++;
                }
                else
                {
                    receiptBatch.GrossBatchTotal -= ReceiptFunctions.Get(receiptID).GrossAmountReceived;
                }

                receiptBatch.GrossBatchTotal += receipt.GrossAmountReceived;

                receiptBatch.Receipts.Add(receipt);

                if (batchType == ReceiptBatchType.Reversals && ReturnValue != null && (ReturnValue.Item1 == (int)ReAllocateReceiptTo.Suspense || ReturnValue.Item2.HasValue))
                {
                    receipt = new Receipt();

                    receipt.ID = reAllocationReceiptID.GetValueOrDefault();

                    switch (ReturnValue.Item1)
                    {
                        case (int)ReAllocateReceiptTo.Contract:
                            receipt.ApplyToTypeID = (int)ReceiptApplyTo.Contract;
                            receipt.ContractID = ReturnValue.Item2;
                            receipt.InternalReference = receipt.ContractID.Value.ToString();
                            break;
                        case (int)ReAllocateReceiptTo.Quote:
                            receipt.ApplyToTypeID = (int)ReceiptApplyTo.Quote;
                            receipt.QuoteID = ReturnValue.Item2;
                            receipt.InternalReference = receipt.QuoteID.Value.ToString();
                            break;
                        case (int)ReAllocateReceiptTo.Suspense:
                            receipt.ApplyToTypeID = (int)ReceiptApplyTo.Suspense;
                            break;
                    }

                    receipt.ReceiptBatchID = ReceiptBatchID;
                    receipt.Reference = ReturnValue.Item3;
                    receipt.LastUserID = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
                    receipt.LastDateModified = DateTime.Now;
                    receipt.ReceiptDate = receiptDate;

                    receipt.NetAmountReceived = Receipt.NetAmountReceived.GetValueOrDefault();
                    receipt.GSTAmountReceived = Receipt.GSTAmountReceived.GetValueOrDefault();
                    receipt.FIDAmountReceived = Receipt.FIDAmountReceived.GetValueOrDefault();
                    receipt.SDAmountReceived = Receipt.SDAmountReceived.GetValueOrDefault();
                    receipt.GrossAmountReceived = Receipt.GrossAmountReceived;

                    if (receiptID == 0)
                    {
                        receipt.ReversalReceiptDetail = new ReversalReceiptDetail { ReasonCodeID = reasonSelected, ReversalTypeID = (int)ReversalTypes.Reallocation };
                        receipt.ReversalReceiptDetail.Receipt = receiptBatch.Receipts.FirstOrDefault();
                    }
                    else
                    {
                        receipt.ReversalReceiptDetail = new ReversalReceiptDetail { ReceiptID = reAllocationReceiptID.GetValueOrDefault(), LinkedReceiptID = receiptID, ReasonCodeID = reasonSelected, ReversalTypeID = (int)ReversalTypes.Reallocation };
                    }

                    if (ReturnValue.Item1 != (int)ReAllocateReceiptTo.Suspense)
                    {
                        foreach (OpenItemReceiptAllocation allocation in DishonourReversalFunctions.GetOpenItemReceiptAllocations(Receipt.ID))
                        {
                            openItemReceiptAllocation = new OpenItemReceiptAllocation();

                            openItemReceiptAllocation.ReceiptID = reAllocationReceiptID.GetValueOrDefault();
                            openItemReceiptAllocation.OpenItemID = allocation.OpenItemID;
                            openItemReceiptAllocation.NetAmountApplied = allocation.NetAmountApplied;
                            openItemReceiptAllocation.GSTAmountApplied = allocation.GSTAmountApplied;
                            openItemReceiptAllocation.FIDAmountApplied = allocation.FIDAmountApplied;
                            openItemReceiptAllocation.SDAmountApplied = allocation.SDAmountApplied;
                            openItemReceiptAllocation.GrossAmountApplied = allocation.GrossAmountApplied;
                            receipt.OpenItemReceiptAllocations.Add(openItemReceiptAllocation);
                        }
                    }

                    if (reAllocationReceiptID.GetValueOrDefault() == 0)
                    {
                        receiptBatch.NumberOfEntries++;
                    }
                    else
                    {
                        receiptBatch.GrossBatchTotal -= ReceiptFunctions.Get(reAllocationReceiptID.GetValueOrDefault()).GrossAmountReceived;
                    }

                    receiptBatch.GrossBatchTotal += receipt.GrossAmountReceived;
                    receiptBatch.Receipts.Add(receipt);
                }

                ReceiptBatchFunctions.Save(receiptBatch);

                isChanged = false;

                if (Convert.ToBoolean(isNext))
                {
                    OriginalReceiptID = null;
                    SetReceiptDefaults(ReceiptBatchID);
                    SetIcon();
                    ReturnValue = null;
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
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnResetAllocation()
        {
            PopupWindow popupWindow;

            IsBusy = true;
            popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "DishonourReversal.ReversalReceiptReallocation", true);
            popupWindow.Parameters.Add(this);
            popupWindow.Parameters.Add(reAllocationReceiptID.GetValueOrDefault());

            Popup.Raise(popupWindow, popupCallBack =>
            {
                if (popupCallBack.ReturnValue != null)
                {
                    ReturnValue = (Tuple<int, int?, string>)popupCallBack.ReturnValue;
                    RaisePropertyChanged("IsSaveEnabled");
                }
            });

            IsBusy = false;
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
                    case "originalreceiptid":
                        if (isReallocateOK)
                        {
                            return string.Empty;
                        }

                        if (Receipt.ID == 0)
                        {
                            return "Receipt No. is Required";
                        }
                        else if (DishonourReversalFunctions.IsValidDishonourReversalReceipt((int)batchType, Receipt.ID, receiptID) == false)
                        {
                            return "Receipt No. is Invalid";
                        }
                        break;                                            
                }             
   
                return string.Empty;
            }
        }

        private void SetReceiptDefaults(int receiptBatchID)
        {
            ReceiptBatch receiptBatch;
            DropdownList reason;

            Receipt = new Receipt();
            receiptBatch = ReceiptBatchFunctions.Get(receiptBatchID);
            ReceiptDate = receiptBatch.ReceiptDate;

            reason = reasonCodes.Where(item => item.Description == "None").FirstOrDefault();

            if (reason != null)
            {
                ReasonSelected = reason.ID;
            }           
        }

        private string FormatBSBNo(string bsbNo)
        {
            if (string.IsNullOrEmpty(bsbNo) == false && bsbNo.Length > 3 && bsbNo.IndexOf("-") == -1)
            {
                return bsbNo.Substring(0, 3) + "-" + bsbNo.Substring(3);
            }

            return bsbNo;
        }

        private void SetIcon()
        {
            switch (batchType)
            {
                case ReceiptBatchType.Dishonour:
                    if (receiptID == 0)
                    {
                        IconFileName = "Add.jpg";
                        Title = "New Dishonour Receipt";
                    }
                    else if (isReceiptEditable == false)
                    {
                        IconFileName = "Edit.jpg";
                        Title = "View Dishonour Receipt";
                        IconFileName = "View.jpg";
                    }
                    else
                    {
                        IconFileName = "Edit.jpg";
                        Title = "Modify Dishonour Receipt";
                    }
                    break;
                case ReceiptBatchType.Reversals:
                    if (receiptID == 0)
                    {
                        IconFileName = "Add.jpg";
                        Title = "New Reversal Receipt";
                    }
                    else if (isReceiptEditable == false)
                    {
                        IconFileName = "Edit.jph";
                        Title = "View Reversal Receipt";
                        IconFileName = "View.jpg";
                    }
                    else
                    {
                        IconFileName = "Edit.jpg";
                        Title = "Modify Reversal Receipt";
                    }
                    break;                
            }
        }      
    }
}
