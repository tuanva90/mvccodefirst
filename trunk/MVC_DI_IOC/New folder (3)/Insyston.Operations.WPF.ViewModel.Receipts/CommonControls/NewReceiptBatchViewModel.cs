using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System.Collections.ObjectModel;
using Insyston.Operations.Logging;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Business.Receipts.Model;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.WPF.ViewModel.Common;
using Insyston.Operations.Business.Common;
using Insyston.Operations.Security;
using model = Insyston.Operations.Model;
using businessModel = Insyston.Operations.Business.Receipts.Model;
using Insyston.Operations.WPF.ViewModel.Common.Controls;
using receipts = Insyston.Operations.Business.Receipts;
using System.Threading;
using Insyston.Operations.Common;

namespace Insyston.Operations.WPF.ViewModel.Receipts.CommonControls
{
    [Export(typeof(NewReceiptBatchViewModel))]
    public class NewReceiptBatchViewModel : OldViewModelBase, IDataErrorInfo
    {
        private List<BatchType> batchTypes;
        private List<DropdownList> createFromList, selectList;
        private List<InternalCompanyBank> internalCompanyBankList;
        private List<DishonourReversalReceiptSearch> dishnourReversalReceipts;
        private ObservableCollection<object> selectedinternalCompanyBanks;        
        private ObservableCollection<object> selectedReceipts;

        private ReceiptBatch receiptBatch;
        private NavigationItem returnValue;
        private int batchType, createFrom, selectFrom, selectedReceipt;
        private int? selectFromObjectID;
        private string title, selectedBatch;
        private DateTime fromDate, toDate;
        private bool isBankSelected, isChanged;     
        
        private ReceiptBatchType receiptBatchType;

        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand SearchCommand { get; private set; }
        public DelegateCommand<ObservableCollection<object>> InterCompanyBankSelectedCommand { get; private set; }
        public DelegateCommand<ObservableCollection<object>> ReceiptSelectedCommand { get; private set; }

        public InteractionRequest<ConfirmationWindowViewModel> UIConfirmation { get; private set; }
        public InteractionRequest<PopupWindow> Popup { get; private set; }      
        
        public NewReceiptBatchViewModel()
        {
            BatchTypes = BatchTypeFunctions.GetBatchTypes();
            ReceiptBatch = new ReceiptBatch();
            ReceiptBatch.ReceiptDate = DateTime.Today;
            SelectedBatch = string.Empty;
            Popup = new InteractionRequest<PopupWindow>();
        }

        public NewReceiptBatchViewModel(int batchType)
        {
            BatchType batch;

            try
            {
                BatchTypes = BatchTypeFunctions.GetBatchTypes();
                ReceiptBatch = new ReceiptBatch();

                if (batchType == (int)ReceiptBatchType.Dishonour)
                {
                    ReceiptBatch.ReceiptDate = DateTime.Today.AddDays(-1);
                }
                else
                {
                    ReceiptBatch.ReceiptDate = DateTime.Today;
                }

                SelectedBatch = string.Empty;

                if (batchType != 0)
                {
                    batch = BatchTypes.Where(batchtype => batchtype.ID == batchType).FirstOrDefault();

                    if (batch != null)
                    {
                        BatchType = batch.ID;
                        SelectedBatch = batch.Description;
                    }
                }

                FromDate = DateTime.Today;
                ToDate = DateTime.Today;

                SaveCommand = new DelegateCommand(Save);
                CancelCommand = new DelegateCommand(Cancel);
                SearchCommand = new DelegateCommand(Search);
                InterCompanyBankSelectedCommand = new DelegateCommand<ObservableCollection<object>>(InterCompanyBankSelected);
                ReceiptSelectedCommand = new DelegateCommand<ObservableCollection<object>>(ReceiptSelected);

                UIConfirmation = new InteractionRequest<ConfirmationWindowViewModel>();
                Popup = new InteractionRequest<PopupWindow>();
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error encountered while initializing New Receipt Batch.", "Create Batch - Error");
            }
            finally
            {
                IsBusy = false;
            }
        }       

        public List<BatchType> BatchTypes
        {
            get
            {
                return batchTypes;
            }
            set
            {
                if (batchTypes != value)
                {
                    batchTypes = value;                    
                    RaisePropertyChanged("BatchTypes");
                }
            }
        }

        public List<DropdownList> CreateFromList
        {
            get
            {
                return createFromList;
            }
            set
            {
                if (createFromList != value)
                {
                    createFromList = value;
                    RaisePropertyChanged("CreateFromList");
                }
            }
        }

        public List<DropdownList> SelectList
        {
            get
            {
                return selectList;
            }
            set
            {
                if (selectList != value)
                {
                    selectList = value;
                    RaisePropertyChanged("SelectList");
                }
            }
        }

        public List<InternalCompanyBank> InternalCompanyBankList
        {
            get
            {
                return internalCompanyBankList;
            }
            set
            {
                if (internalCompanyBankList != value)
                {
                    internalCompanyBankList = value;
                    RaisePropertyChanged("InternalCompanyBankList");
                }
            }
        }

        public ReceiptBatch ReceiptBatch
        {
            get
            {
                return receiptBatch;
            }
            set
            {
                if (receiptBatch != value)
                {
                    receiptBatch = value;
                    RaisePropertyChanged("ReceiptBatch");
                }
            }
        }
       
        public int CreateFrom
        {
            get
            {
                return createFrom;
            }
            set
            {
                if (createFrom != value)
                {
                    createFrom = value;
                    RaisePropertyChanged("CreateFrom");
                    RaisePropertyChanged("IsDateRangeApplicable");

                    if(receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals)
                    {
                        selectedReceipts = null;
                        RaisePropertyChanged("IsSelectListApplicable");
                    }
                }
            }
        }                                     

        public int SelectFrom
        {
            get
            {
                return selectFrom;
            }
            set
            {
                if (selectFrom != value)
                {
                    selectFrom = value;
                    SelectFromObjectID = null;

                    if (selectFrom == (int)ReceiptBatchSelectList.Suspense)
                    {
                        if (receiptBatchType == ReceiptBatchType.Dishonour)
                        {
                            DishonourReversalReceipts = DishonourReversalFunctions.GetDishonourReceipts(selectFrom);
                        }
                        else
                        {
                            DishonourReversalReceipts = DishonourReversalFunctions.GetReversalReceipts(selectFrom);
                        }
                    }
                    else
                    {
                        DishonourReversalReceipts = null;                                                                                                                      
                    }

                    selectedReceipts = null;
                    RaisePropertyChanged("SelectFrom");
                    RaisePropertyChanged("ApplyToLabelCaption");
                    RaisePropertyChanged("IsSelectFromContract");
                    RaisePropertyChanged("IsSelectFromQuote");
                    RaisePropertyChanged("IsSuspense");
                }
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

        public DateTime FromDate
        {
            get
            {
                return fromDate;
            }
            set
            {
                if (fromDate != value)
                {
                    fromDate = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }

        public DateTime ToDate
        {
            get
            {
                return toDate;
            }
            set
            {
                if (toDate != value)
                {
                    toDate = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }        

        public InternalCompanyBank InternalCompanyBank
        {           
            set
            {                
                if (value != null && ((isBankSelected && ReceiptBatch.BankAccountID != value.BankID) || (IsIntercompanySelected && ReceiptBatch.InternalCompanyNodeID != value.NodeID)))
                {
                    ReceiptBatch.BankAccountID = value.BankID;
                    ReceiptBatch.InternalCompanyNodeID = value.NodeID;

                    if (isBankSelected)
                    {
                        ReceiptBatch.FilterDescription = "Bank: " + value.BankName;
                    }
                    else if (IsIntercompanySelected)
                    {
                        ReceiptBatch.FilterDescription = "Internal Company: " + value.InternalCompany;
                    }

                    RaisePropertyChanged("InternalCompanyBank");
                }
                else if (value == null)
                {
                    ReceiptBatch.BankAccountID = 0;
                    receiptBatch.InternalCompanyNodeID = null;

                    RaisePropertyChanged("InternalCompanyBank");
                }
            }
        }

        public DishonourReversalReceiptSearch SelectedReceipt
        {
            set
            {
                if (value != null)
                {
                    selectedReceipt = value.ReceiptID;
                }
                else
                {
                    selectedReceipt = 0;
                }
            }
        }

        public int BatchType
        {
            get
            {
                return batchType;
            }
            set
            {
                BatchType batch;

                if (batchType != value)
                {
                    IsBusy = true;
                    CreateFrom = 0;
                    batchType = value;
                    ReceiptBatch.BatchTypeID = value;
                    
                    RaisePropertyChanged("BatchType");

                    batch = batchTypes.Where(batchtype => batchtype.ID == batchType).FirstOrDefault();
                    receiptBatchType = (ReceiptBatchType)batchType;                    

                    if (batch != null)
                    {                        
                        Title = "New " + batch.Description + " Batch";
                        SelectedBatch = batch.Description;
                    }
                    else
                    {
                        Title = "New Receipt Batch";
                        SelectedBatch = string.Empty;
                    }

                    CreateFromList = BatchTypeFunctions.GetCreateFromList(batchType);

                    if (receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals)
                    {
                        SelectList = BatchTypeFunctions.GetSelectList(batchType);
                    }

                    SetReceiptBatchDefaults();
                    SetIcon();
                    isChanged = false;
                    ClearErrorMessages();

                    RaisePropertyChanged("DishonourColumnWidth");
                    IsBusy = false;
                }
            }
        }

        public string SelectedBatch
        {
            get
            {
                return selectedBatch;
            }
            set
            {
                if (selectedBatch != value)
                {
                    selectedBatch = value;
                    RaisePropertyChanged("SelectedBatch");
                }
            }
        }

        public NavigationItem ReturnValue
        {
            get
            {
                return returnValue;
            }
            set
            {
                returnValue = value;
                RaisePropertyChanged("ReturnValue");
            }
        }

        public int? SelectFromObjectID
        {
            get
            {
                return selectFromObjectID;
            }
            set
            {
                if (selectFromObjectID != value)
                {
                    selectFromObjectID = value;

                    if (value.HasValue && IsValidObjectID())
                    {
                        if (receiptBatchType == ReceiptBatchType.Dishonour)
                        {
                            DishonourReversalReceipts = DishonourReversalFunctions.GetDishonourReceipts(selectFrom, selectFromObjectID.Value);
                        }
                        else
                        {
                            DishonourReversalReceipts = DishonourReversalFunctions.GetReversalReceipts(selectFrom, selectFromObjectID.Value);
                        }
                    }
                    else
                    {
                        DishonourReversalReceipts = null;
                    }

                    RaisePropertyChanged("SelectFromObjectID");
                }
            }
        }

        public List<DishonourReversalReceiptSearch> DishonourReversalReceipts
        {
            get
            {
                return dishnourReversalReceipts;
            }
            set
            {
                if (dishnourReversalReceipts != value)
                {
                    dishnourReversalReceipts = value;
                    RaisePropertyChanged("DishonourReversalReceipts");
                }
            }
        }

        public int DishonourColumnWidth
        {
            get
            {
                if(receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals)
                {
                    return 30;
                }

                return 0;
            }
        }

        public string ApplyToLabelCaption
        {
            get
            {
                DropdownList item;

                item = selectList.Where(select => select.ID == selectFrom).FirstOrDefault();
                
                if(item != null)
                {
                    return item.Description + " No.";
                }

                return string.Empty;
            }            
        }        

        public bool IsBankSelected
        {
            get
            {
                return isBankSelected;
            }
            set
            {
                IsBusy = true;
                isBankSelected = value;
                LoadInternalCompanyBankList();
                selectedinternalCompanyBanks = null;
                RaisePropertyChanged("IsBankSelected");
                IsBusy = false;
            }
        }

        public bool IsIntercompanySelected
        {
            get
            {
                return !isBankSelected;
            }
            set
            {
                IsBankSelected = !value;
                selectedinternalCompanyBanks = null;
                RaisePropertyChanged("IsIntercompanySelected");
            }
        }

        public bool IsDateRangeApplicable
        {
            get
            {
                return createFrom == (int)ReceiptBatchCreateFrom.DateRange;
            }
            set
            {
            }
        }

        public bool IsSelectListApplicable
        {
            get
            {
                return createFrom == (int)ReceiptBatchCreateFrom.SelectList;
            }
        }

        public bool IsSuspense
        {
            get
            {
                return selectFrom == (int)ReceiptBatchSelectList.Suspense;
            }
        }

        public bool IsSelectFromContract
        {
            get
            {
                return IsSuspense || selectFrom == (int)ReceiptBatchSelectList.Contract;
            }
        }

        public bool IsSelectFromQuote
        {
            get
            {
                return IsSuspense || selectFrom == (int)ReceiptBatchSelectList.Quote;
            }
        }

        public bool IsAutoReceipts
        {
            get
            {
                return (receiptBatchType == ReceiptBatchType.AutoReceipts);
            }
        }

        private void SetReceiptBatchDefaults()
        {
            LXMSystemReceiptDefault batchTypeDefaults;

            if(batchType != 0)
            {
                batchTypeDefaults = BatchTypeFunctions.GetReceiptBatchSystemDefaults(batchType);

                if ((receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals) && selectList.Count > 0)
                {
                    SelectFrom = selectList.FirstOrDefault().ID;
                }

                if (batchTypeDefaults != null)
                {
                    if (receiptBatchType == ReceiptBatchType.CashCheque || receiptBatchType == ReceiptBatchType.DirectDebit || receiptBatchType == ReceiptBatchType.CreditCard || receiptBatchType == ReceiptBatchType.AutoReceipts)
                    {
                        if (batchTypeDefaults.DefaultProcess.HasValue && batchTypeDefaults.DefaultProcess.Value == true)
                        {
                            IsBankSelected = false;
                        }
                        else
                        {
                            IsBankSelected = true;
                        }
                    }

                    if (batchTypeDefaults.DefaultCreateFromID.HasValue && batchTypeDefaults.DefaultCreateFromID.Value > 0)
                    {
                        CreateFrom = batchTypeDefaults.DefaultCreateFromID.Value;
                    }

                    if (batchTypeDefaults.DefaultSelectListID.HasValue && batchTypeDefaults.DefaultSelectListID.Value > 0)
                    {
                        SelectFrom = batchTypeDefaults.DefaultSelectListID.Value;
                    }
                }
                else
                {
                    IsBankSelected = true;

                    if(createFromList.Count > 0)
                    {
                        CreateFrom = createFromList.First().ID;
                    }

                    if (selectList != null && selectList.Count > 0)
                    {
                        SelectFrom = selectList.First().ID;
                    }
                }
            }
        }

        private void SetIcon()
        {
            switch (receiptBatchType)
            {
                case ReceiptBatchType.CashCheque:
                    IconFileName = "NewCashBatch.jpg";
                    break;
                case ReceiptBatchType.DirectDebit:
                    IconFileName = "NewDirectDebitBatch.jpg";
                    break;
                case ReceiptBatchType.CreditCard:
                    IconFileName = "NewCreditCardBatch.jpg";
                    break;
                case ReceiptBatchType.Dishonour:
                    IconFileName = "NewDishonourBatch.jpg";
                    break;
                case ReceiptBatchType.Reversals:
                    IconFileName = "NewReversalBatch.jpg";
                    break;
                case ReceiptBatchType.AutoReceipts:
                    IconFileName = "NewAutoBatch.jpg";
                    break;
            }
        }

        private void LoadInternalCompanyBankList()
        {
            switch(receiptBatchType)
            {
                case ReceiptBatchType.CashCheque:
                    if (IsBankSelected)
                    {
                        InternalCompanyBankList = CashReceiptBatchFunctions.GetReceiptBanksList();
                    }
                    else
                    {
                        InternalCompanyBankList = CashReceiptBatchFunctions.GetReceiptInternalCompaniesList();
                    }
                    break;
                case ReceiptBatchType.DirectDebit:
                case ReceiptBatchType.CreditCard:
                case ReceiptBatchType.AutoReceipts:
                    if (IsBankSelected)
                    {
                        InternalCompanyBankList = DDCCBatchFunctions.GetReceiptBanksList(receiptBatchType);
                    }
                    else
                    {
                        InternalCompanyBankList = DDCCBatchFunctions.GetReceiptInternalCompaniesList(receiptBatchType);
                    }
                    break;
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

                switch (columnName)
                {
                    case "CreateFrom":
                        if (CreateFrom == 0)
                        {
                            return "Select an option Create Batch From";
                        }
                        break;
                    case "InternalCompanyBank":
                        if (isBankSelected && ReceiptBatch.BankAccountID == 0)
                        {
                            return "Select a Bank from the List";
                        }
                        else if(ReceiptBatch.BankAccountID == 0)
                        {
                            return "Select an Internal Company from the List";
                        }
                        break;
                    case "ToDate":
                        if (IsDateRangeApplicable && (receiptBatchType == ReceiptBatchType.DirectDebit || receiptBatchType == ReceiptBatchType.CreditCard || receiptBatchType == ReceiptBatchType.AutoReceipts))
                        {
                            if (toDate < fromDate)
                            {
                                return "To Date should be greater than From Date";
                            }
                        }
                        break;
                    case "SelectFromObjectID":
                        if (selectFrom != (int)ReceiptBatchSelectList.Suspense)
                        {
                            if (selectFromObjectID.HasValue == false)
                            {
                                return ApplyToLabelCaption + " is Required";
                            }
                            else if (IsValidObjectID() == false)
                            {
                                return ApplyToLabelCaption + " is Invalid";
                            }
                        }
                        break;
                    case "SelectedReceipt":
                        if (selectedReceipt == 0 || selectedReceipt == -1)
                        {
                            return "Select a Receipt from the List";
                        }
                        break;
                }

                return string.Empty;
            }
        }

        private void Save()
        {
            isChanged = true;

            if (Validate() == true)
            {
                if (ReceiptBatch.ReceiptDate > DateTime.Today)
                {
                    UIConfirmation.Raise(
                        new ConfirmationWindowViewModel(this) { Content = "Receipt Date is in the future. Select OK to continue or Cancel to modify", Icon = "Question.ico", Title = "Receipt Date Confirmation" },
                        (popupCallBack) => {
                            if (popupCallBack.Confirmed)
                            {
                                SaveReceiptBatch();
                            }
                        });
                }
                else
                {
                    SaveReceiptBatch();
                }                
            }           
        }

        private void SaveReceiptBatch()
        {
            PopupWindow popupWindow;
            AccountingParam accountingParam;
            ReceiptBatch newReceiptBatch;
            Receipt receipt = null;
            List<OpenItemReceiptAllocation> receiptAllocations = null;
            List<businessModel.DirectDebitException> DDExceptions = null;
            List<businessModel.CreditCardException> CCExceptions = null;
            IEnumerable<int> openItemIDs = null;
            OpenItemReceiptAllocation newAllocation;

            try
            {
                int contractID = 0;
                isChanged = true;
                bool isNew;

                accountingParam = SystemFunctions.GetAccountingParam();
                isNew = receiptBatch.ID == 0;

                receiptBatch.BatchStatusID = (int)ReceiptBatchStatus.Created;

                if (accountingParam.CurrentAccountingDate.HasValue)
                {
                    receiptBatch.AccountingDate = accountingParam.CurrentAccountingDate;
                    receiptBatch.BatchMonth = accountingParam.CurrentAccountingDate.Value.ToString("MMM yyyy");
                }
                else
                {
                    receiptBatch.AccountingDate = receiptBatch.ReceiptDate;
                    receiptBatch.BatchMonth = receiptBatch.ReceiptDate.ToString("MMM yyyy");
                }

                receiptBatch.CreatedByUserID = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;

                if (receiptBatchType == ReceiptBatchType.CashCheque)
                {
                    ReceiptBatchFunctions.Save(receiptBatch);

                    if (isNew && createFromList.Where(from => from.ID == createFrom).FirstOrDefault().Description.ToLower().Trim() == ReceiptBatchCreateFrom.Import.ToString().ToLower().Trim())
                    {
                        ReturnValue = new NavigationItem() { ReceiptID = receiptBatch.ID, ReceiptText = receiptBatch.ID.ToString() + "-" + SelectedBatch, BatchStatus = (int)ReceiptBatchStatus.Created, BatchTypeID = receiptBatch.BatchTypeID };

                        IsBusy = true;
                        popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "CashCheque.CashReceiptExcelImport", true);
                        popupWindow.Parameters.Add(receiptBatch.ID);
                        Popup.Raise(popupWindow);
                        receiptBatch = ReceiptBatchFunctions.Get(receiptBatch.ID);
                        IsBusy = false;
                    }
                    else
                    {
                        ReturnValue = new NavigationItem() { ReceiptID = receiptBatch.ID, ReceiptText = receiptBatch.ID.ToString() + "-" + SelectedBatch, BatchStatus = (int)ReceiptBatchStatus.Created, BatchTypeID = receiptBatch.BatchTypeID };
                        Close();
                    }
                }
                else if (receiptBatchType == ReceiptBatchType.DirectDebit || receiptBatchType == ReceiptBatchType.CreditCard || receiptBatchType == ReceiptBatchType.AutoReceipts)
                {
                    if (IsDateRangeApplicable)
                    {
                        receiptAllocations = DDCCBatchFunctions.GetDDCCOpenItemsReceiptAllocation(receiptBatchType, fromDate, toDate, IsIntercompanySelected, selectedinternalCompanyBanks);

                        openItemIDs = receiptAllocations.Select(item => item.OpenItemID).Distinct();

                        if (receiptBatchType == ReceiptBatchType.DirectDebit)
                        {
                            DDExceptions = DDCCBatchFunctions.GetDDExceptions(fromDate, toDate, IsIntercompanySelected,
                                ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId, selectedinternalCompanyBanks);
                        }
                        else if (receiptBatchType == ReceiptBatchType.CreditCard)
                        {
                            CCExceptions = DDCCBatchFunctions.GetCCExceptions(fromDate, toDate, IsIntercompanySelected, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId, selectedinternalCompanyBanks);
                        }
                    }

                    if (receiptBatchType == ReceiptBatchType.AutoReceipts)
                    {
                        newReceiptBatch = receiptBatch.Copy();

                        if (IsDateRangeApplicable)
                        {
                            newReceiptBatch.DateRangeFilterDescription = "From " + fromDate.ToString("dd/MM/yyyy") + " to " + toDate.ToString("dd/MM/yyyy");
                            newReceiptBatch.Receipts = new List<Receipt>();

                            foreach (OpenItemReceiptAllocation allocation in receiptAllocations.OrderBy(item => item.OpenItem.ContractID))
                            {
                                if (contractID != allocation.OpenItem.Contract.ContractId)
                                {
                                    receipt = new Receipt();
                                    receipt.ApplyToTypeID = (int)ReceiptApplyTo.Contract;
                                    receipt.ContractID = allocation.OpenItem.Contract.ContractId;
                                    receipt.InternalReference = receipt.ContractID.ToString();
                                    receipt.LastUserID = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
                                    receipt.LastDateModified = DateTime.Now;
                                    receipt.ReceiptDate = receiptBatch.ReceiptDate;

                                    receipt.GSTAmountReceived = 0;
                                    receipt.SDAmountReceived = 0;
                                    receipt.FIDAmountReceived = 0;
                                    receipt.NetAmountReceived = 0;
                                    receipt.GrossAmountReceived = 0;

                                    receipt.OpenItemReceiptAllocations = new List<OpenItemReceiptAllocation>();
                                    newReceiptBatch.Receipts.Add(receipt);
                                    newReceiptBatch.NumberOfEntries += 1;
                                    contractID = allocation.OpenItem.Contract.ContractId;
                                }

                                receipt.GSTAmountReceived += allocation.GSTAmountApplied.GetValueOrDefault();
                                receipt.SDAmountReceived += allocation.SDAmountApplied.GetValueOrDefault();
                                receipt.FIDAmountReceived += allocation.FIDAmountApplied.GetValueOrDefault();
                                receipt.NetAmountReceived += allocation.NetAmountApplied;
                                receipt.GrossAmountReceived += allocation.GrossAmountApplied.GetValueOrDefault();
                                newReceiptBatch.GrossBatchTotal += allocation.GrossAmountApplied.GetValueOrDefault();
                                allocation.OpenItem.Contract = null;
                                allocation.OpenItem = null;

                                receipt.OpenItemReceiptAllocations.Add(allocation);
                            }
                        }

                        ReceiptBatchFunctions.Save(newReceiptBatch, openItemIDs, fromDate, toDate);
                        ReturnValue = new NavigationItem() { ReceiptID = newReceiptBatch.ID, ReceiptText = newReceiptBatch.ID.ToString() + "-" + SelectedBatch, BatchStatus = (int)ReceiptBatchStatus.Created, BatchTypeID = newReceiptBatch.BatchTypeID };
                    }
                    else
                    {
                        foreach (InternalCompanyBank internalCompBank in selectedinternalCompanyBanks)
                        {
                            newReceiptBatch = receiptBatch.Copy();
                            newReceiptBatch.BankAccountID = internalCompBank.BankID;
                            newReceiptBatch.InternalCompanyNodeID = internalCompBank.NodeID;

                            if (isBankSelected)
                            {
                                newReceiptBatch.FilterDescription = "Bank: " + internalCompBank.BankName;
                            }
                            else if (IsIntercompanySelected)
                            {
                                newReceiptBatch.FilterDescription = "Internal Company: " + internalCompBank.InternalCompany;
                            }

                            if (IsDateRangeApplicable)
                            {
                                newReceiptBatch.DateRangeFilterDescription = "From " + fromDate.ToString("dd/MM/yyyy") + " to " + toDate.ToString("dd/MM/yyyy");
                                newReceiptBatch.Receipts = new List<Receipt>();

                                foreach (OpenItemReceiptAllocation allocation in receiptAllocations.Where(item => IsIntercompanySelected ? item.OpenItem.Contract.InternalCoyNodeId == internalCompBank.NodeID :
                                        item.OpenItem.Contract.LessorBankAccountId == internalCompBank.BankID))
                                {
                                    if (contractID != allocation.OpenItem.Contract.ContractId)
                                    {
                                        receipt = new Receipt();
                                        receipt.ApplyToTypeID = (int)ReceiptApplyTo.Contract;
                                        receipt.ContractID = allocation.OpenItem.Contract.ContractId;
                                        receipt.InternalReference = receipt.ContractID.ToString();
                                        receipt.LastUserID = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
                                        receipt.LastDateModified = DateTime.Now;
                                        receipt.ReceiptDate = receiptBatch.ReceiptDate;

                                        receipt.GSTAmountReceived = 0;
                                        receipt.SDAmountReceived = 0;
                                        receipt.FIDAmountReceived = 0;
                                        receipt.NetAmountReceived = 0;
                                        receipt.GrossAmountReceived = 0;

                                        if (receiptBatchType == ReceiptBatchType.DirectDebit)
                                        {
                                            receipt.DirectDebitReceiptDetails = new List<DirectDebitReceiptDetail>();
                                            receipt.DirectDebitReceiptDetails.Add(new DirectDebitReceiptDetail { LesseeBankAccountID = allocation.OpenItem.Contract.LesseeBankAccountId.GetValueOrDefault() });
                                        }
                                        else if (receiptBatchType == ReceiptBatchType.CreditCard)
                                        {
                                            receipt.CreditCardReceiptDetails = new List<CreditCardReceiptDetail>();
                                            receipt.CreditCardReceiptDetails.Add(new CreditCardReceiptDetail { LesseeBankAccountID = allocation.OpenItem.Contract.LesseeBankAccountId.GetValueOrDefault() });
                                        }

                                        receipt.OpenItemReceiptAllocations = new List<OpenItemReceiptAllocation>();
                                        newReceiptBatch.Receipts.Add(receipt);
                                        newReceiptBatch.NumberOfEntries += 1;
                                        contractID = allocation.OpenItem.Contract.ContractId;
                                    }

                                    receipt.GSTAmountReceived += allocation.GSTAmountApplied.GetValueOrDefault();
                                    receipt.SDAmountReceived += allocation.SDAmountApplied.GetValueOrDefault();
                                    receipt.FIDAmountReceived += allocation.FIDAmountApplied.GetValueOrDefault();
                                    receipt.NetAmountReceived += allocation.NetAmountApplied;
                                    receipt.GrossAmountReceived += allocation.GrossAmountApplied.GetValueOrDefault();
                                    newReceiptBatch.GrossBatchTotal += allocation.GrossAmountApplied.GetValueOrDefault();
                                    allocation.OpenItem.Contract = null;
                                    allocation.OpenItem = null;

                                    receipt.OpenItemReceiptAllocations.Add(allocation);
                                }

                                if (receiptBatchType == ReceiptBatchType.DirectDebit)
                                {
                                    newReceiptBatch.DirectDebitExceptions = DDExceptions.Where(item => (IsIntercompanySelected ? item.InternalCoyNodeId == internalCompBank.NodeID :
                                        (item.LessorBankAccountID == internalCompBank.BankID || item.LessorBankAccountID == null || item.LessorBankAccountID == -1))).
                                        Select(item => new model.DirectDebitException { AccountNumber = item.AccountNumber, AccountTitle = item.AccountTitle, BSBNumber = item.BSBNumber,
                                            CompanyName = item.CompanyName, ContractID = item.ContractID, CustomerName = item.CustomerName, DateCreated = item.DateCreated, Exception = item.Exception,
                                            GrossAmount = item.GrossAmount, ID = item.ID, LesseeBankAccountID = item.LesseeBankAccountID, LessorAccountNumber = item.LessorAccountNumber, LessorAccountTitle = item.LessorAccountTitle,
                                            LessorBankAccountID = item.LessorBankAccountID, LessorBSBNumber = item.LessorBSBNumber, PaymentDate = item.PaymentDate,  ReceiptBatchID = item.ReceiptBatchID, 
                                            UpdateRequired = item.UpdateRequired, UserID = item.UserID }).ToList();
                                }
                                else if (receiptBatchType == ReceiptBatchType.CreditCard)
                                {
                                    newReceiptBatch.CreditCardExceptions = CCExceptions.Where(item => (IsIntercompanySelected ? item.InternalCoyNodeId == internalCompBank.NodeID :
                                            (item.LessorBankAccountID == internalCompBank.BankID || item.LessorBankAccountID == null || item.LessorBankAccountID == -1))).Select(item => 
                                                new model.CreditCardException { AccountTitle = item.AccountTitle, CardNumber = item.CardNumber, CompanyName = item.CompanyName, CDTFMerchantNumber = item.CDTFMerchantNumber, 
                                                    ContractID = item.ContractID, CustomerName = item.CustomerName, DateCreated = item.DateCreated, Exception = item.Exception, ExpiryDate = item.ExpiryDate,
                                                    GrossAmount = item.GrossAmount, ID = item.ID, LesseeBankAccountID = item.LesseeBankAccountID, LessorAccountTitle = item.LessorAccountTitle, LessorBankAccountID = item.LessorBankAccountID,
                                                    MerchantNumber = item.MerchantNumber, PaymentDate = item.PaymentDate, ReceiptBatchID = item.ReceiptBatchID, UserID = item.UserID }).ToList();
                                }
                            }

                            ReceiptBatchFunctions.Save(newReceiptBatch, openItemIDs, fromDate, toDate);

                            ReturnValue = new NavigationItem() { ReceiptID = newReceiptBatch.ID, ReceiptText = newReceiptBatch.ID.ToString() + "-" + SelectedBatch, BatchStatus = (int)ReceiptBatchStatus.Created, BatchTypeID = newReceiptBatch.BatchTypeID };
                        }
                    }

                    Close();
                }
                else if (receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals)
                {
                    if (selectedReceipts != null)
                    {
                        var banksList = selectedReceipts.Select(item => ((DishonourReversalReceiptSearch)item).BankAccountID).Distinct().ToList();

                        foreach (int bankID in banksList)
                        {
                            newReceiptBatch = receiptBatch.Copy();
                            newReceiptBatch.BankAccountID = bankID;
                            newReceiptBatch.FilterDescription = "Bank: " + ((DishonourReversalReceiptSearch)selectedReceipts.Where(item => ((DishonourReversalReceiptSearch)item).BankAccountID == bankID).FirstOrDefault()).BankName;
                            newReceiptBatch.Receipts.Clear();

                            foreach (DishonourReversalReceiptSearch selectedReceipt in selectedReceipts.Where(item => ((DishonourReversalReceiptSearch)item).BankAccountID == bankID))
                            {
                                receipt = new Receipt();

                                if (selectedReceipt.ContractNo != null)
                                {
                                    receipt.ApplyToTypeID = (int)ReceiptApplyTo.Contract;
                                    receipt.InternalReference = selectedReceipt.ContractNo.ToString();
                                }
                                else
                                {
                                    receipt.ApplyToTypeID = (int)ReceiptApplyTo.Quote;
                                    receipt.InternalReference = selectedReceipt.QuoteID.ToString();
                                }
                                receipt.ContractID = selectedReceipt.ContractNo;
                                receipt.QuoteID = selectedReceipt.QuoteID;

                                if (receiptBatchType == ReceiptBatchType.Dishonour)
                                {
                                    receipt.Reference = "Dishonour";
                                }
                                else
                                {
                                    receipt.Reference = "Reversal";
                                }

                                receipt.LastUserID = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
                                receipt.LastDateModified = DateTime.Now;
                                receipt.ReceiptDate = receiptBatch.ReceiptDate;

                                receipt.NetAmountReceived = selectedReceipt.NetAmountReceived.GetValueOrDefault() * -1;
                                receipt.GSTAmountReceived = selectedReceipt.GSTAmountReceived.GetValueOrDefault() * -1;
                                receipt.FIDAmountReceived = selectedReceipt.FIDAmountReceived.GetValueOrDefault() * -1;
                                receipt.SDAmountReceived = selectedReceipt.SDAmountReceived.GetValueOrDefault() * -1;
                                receipt.GrossAmountReceived = selectedReceipt.AmountReceived * -1;

                                if (receiptBatchType == ReceiptBatchType.Dishonour)
                                {
                                    receipt.DishonourReceiptDetail = new DishonourReceiptDetail { DishonouredReceiptID = selectedReceipt.ReceiptID };
                                }
                                else
                                {
                                    receipt.ReversalReceiptDetail = new ReversalReceiptDetail { LinkedReceiptID = selectedReceipt.ReceiptID, ReversalTypeID = (int)ReversalTypes.Reversal };
                                }

                                foreach (OpenItemReceiptAllocation allocation in DishonourReversalFunctions.GetOpenItemReceiptAllocations(selectedReceipt.ReceiptID))
                                {
                                    newAllocation = new OpenItemReceiptAllocation();

                                    newAllocation.OpenItemID = allocation.OpenItemID;
                                    newAllocation.NetAmountApplied = allocation.NetAmountApplied * -1;
                                    newAllocation.GSTAmountApplied = allocation.GSTAmountApplied * -1;
                                    newAllocation.FIDAmountApplied = allocation.FIDAmountApplied * -1;
                                    newAllocation.SDAmountApplied = allocation.SDAmountApplied * -1;
                                    newAllocation.GrossAmountApplied = allocation.GrossAmountApplied * -1;
                                    receipt.OpenItemReceiptAllocations.Add(newAllocation);
                                }

                                newReceiptBatch.Receipts.Add(receipt);
                                newReceiptBatch.GrossBatchTotal += receipt.GrossAmountReceived;
                                newReceiptBatch.NumberOfEntries++;
                            }

                            ReceiptBatchFunctions.Save(newReceiptBatch);
                            ReturnValue = new NavigationItem() { ReceiptID = newReceiptBatch.ID, ReceiptText = newReceiptBatch.ID.ToString() + "-" + SelectedBatch, BatchStatus = (int)ReceiptBatchStatus.Created, BatchTypeID = newReceiptBatch.BatchTypeID };
                        }

                        Close();
                    }
                    else
                    {
                        ReceiptBatchFunctions.Save(receiptBatch);

                        if (isNew && createFromList.Where(from => from.ID == createFrom).FirstOrDefault().Description.ToLower().Trim() == ReceiptBatchCreateFrom.Import.ToString().ToLower().Trim())
                        {
                            ReturnValue = new NavigationItem() { ReceiptID = receiptBatch.ID, ReceiptText = receiptBatch.ID.ToString() + "-" + SelectedBatch, BatchStatus = (int)ReceiptBatchStatus.Created, BatchTypeID = receiptBatch.BatchTypeID };

                            IsBusy = true;
                            popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "DishonourReversal.DishonourReversalReceiptExcelImport", true);
                            popupWindow.Parameters.Add(receiptBatchType);
                            popupWindow.Parameters.Add(receiptBatch.ID);
                            Popup.Raise(popupWindow);
                            receiptBatch = ReceiptBatchFunctions.Get(receiptBatch.ID);
                            IsBusy = false;
                        }
                        else
                        {
                            ReturnValue = new NavigationItem() { ReceiptID = receiptBatch.ID, ReceiptText = receiptBatch.ID.ToString() + "-" + SelectedBatch, BatchStatus = (int)ReceiptBatchStatus.Created, BatchTypeID = receiptBatch.BatchTypeID };
                            Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error encountered while Creating New Batch.", "Create Batch - Error");
            }
            finally
            {
                IsBusy = false;                     
            }
        }

        private void Search()
        {
            IsBusy = true;
            switch (SelectFrom)
            {
                case (int)ReceiptBatchSelectList.Contract:
                    PopupWindow<ContractReceiptSearch> contractPopup;
                    DelegateSearchFilter<ContractReceiptSearch> contractSearch;

                    contractSearch = new DelegateSearchFilter<ContractReceiptSearch>(receipts.ContractFunctions.SearchContractReceipt);
                    contractPopup = new PopupWindow<ContractReceiptSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<ContractReceiptSearch>(contractSearch, "Search Contract", batchType), true);

                    Popup.Raise(contractPopup, (popupCallBack) =>
                    {
                        if (popupCallBack.ReturnValue != null)
                        {
                            ContractReceiptSearch contract = (ContractReceiptSearch)popupCallBack.ReturnValue;                            
                            SelectFromObjectID = contract.ContractID;                           
                        }

                        popupCallBack = null;
                    });
                    break;
                case (int)ReceiptBatchSelectList.Quote:
                    PopupWindow<QuoteReceiptSearch> quotePopup;
                    DelegateSearchFilter<QuoteReceiptSearch> quoteSearch;

                    quoteSearch = new DelegateSearchFilter<QuoteReceiptSearch>(receipts.QuoteFunctions.SearchQuoteReceipt);
                    quotePopup = new PopupWindow<QuoteReceiptSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<QuoteReceiptSearch>(quoteSearch, "Search Quote", batchType), true);

                    Popup.Raise(quotePopup, (popupCallBack) =>
                    {
                        if (popupCallBack.ReturnValue != null)
                        {
                            QuoteReceiptSearch quote = (QuoteReceiptSearch)popupCallBack.ReturnValue;                            
                            SelectFromObjectID = quote.QuoteNo;
                        }

                        popupCallBack = null;
                    });
                    break;
                case (int)ReceiptBatchSelectList.Client:
                    PopupWindow<ClientReceiptSearch> clientPopup;
                    DelegateSearchFilter<ClientReceiptSearch> clientSearch;

                    clientSearch = new DelegateSearchFilter<ClientReceiptSearch>(receipts.ClientFunctions.SearchClientReceipt);
                    clientPopup = new PopupWindow<ClientReceiptSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<ClientReceiptSearch>(clientSearch, "Search Client", batchType), true);

                    Popup.Raise(clientPopup, (popupCallBack) =>
                    {
                        if (popupCallBack.ReturnValue != null)
                        {
                            ClientReceiptSearch client = (ClientReceiptSearch)popupCallBack.ReturnValue;                            
                            SelectFromObjectID = client.ClientNo;
                        }

                        popupCallBack = null;
                    });
                    break;
                case (int)ReceiptBatchSelectList.Batch:
                    PopupWindow<BatchReceiptSearch> batchPopup;
                    DelegateSearchFilter<BatchReceiptSearch> batchSearch;

                    batchSearch = new DelegateSearchFilter<BatchReceiptSearch>(ReceiptBatchFunctions.SearchBatchReceipt);
                    batchPopup = new PopupWindow<BatchReceiptSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<BatchReceiptSearch>(batchSearch, "Search Receipt Batch", batchType), true);

                    Popup.Raise(batchPopup, (popupCallBack) =>
                    {
                        if (popupCallBack.ReturnValue != null)
                        {
                            BatchReceiptSearch batch = (BatchReceiptSearch)popupCallBack.ReturnValue;                            
                            SelectFromObjectID = batch.BatchNo;
                        }

                        popupCallBack = null;
                    });
                    break;
            }

            IsBusy = false;
        }

        private bool IsValidObjectID()
        {
            bool isValid = false;

            switch (selectFrom)
            {
                case (int)ReceiptBatchSelectList.Contract:
                    isValid = receipts.ContractFunctions.ContractExistsinReceipt(selectFromObjectID.Value);
                    break;
                case (int)ReceiptBatchSelectList.Quote:
                    isValid = receipts.QuoteFunctions.QuoteExistsinReceipt(selectFromObjectID.Value);
                    break;
                case (int)ReceiptBatchSelectList.Client:
                    isValid = receipts.ClientFunctions.ClientExistsinReceipt(selectFromObjectID.Value);
                    break;
                case (int)ReceiptBatchSelectList.Batch:
                    isValid = ReceiptBatchFunctions.BatchExists(selectFromObjectID.Value);
                    break;
            }

            return isValid;
        }

        private void InterCompanyBankSelected(ObservableCollection<object> selectedItems)
        {
            selectedinternalCompanyBanks = selectedItems;
        }

        private void ReceiptSelected(ObservableCollection<object> selectedItems)
        {
            selectedReceipts = selectedItems;
        }

        private void Cancel()
        {
            if (receiptBatch.ID > 0)
            {
                ReturnValue = new NavigationItem() { ReceiptID = receiptBatch.ID, ReceiptText = receiptBatch.ID.ToString() + "-" + SelectedBatch, BatchStatus = (int)ReceiptBatchStatus.Created, BatchTypeID = receiptBatch.BatchTypeID };
            }
            else
            {
                ReturnValue = null;
            }

            Close();
        }

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }
    }
}
