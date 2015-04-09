using Insyston.Operations.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Insyston.Operations.Logging;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Business.Receipts.Model;
using Insyston.Operations.WPF.ViewModel.Common;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.WPF.ViewModel.Common.Controls;
using Insyston.Operations.Security;
using System.Threading;
using Insyston.Operations.Common;

namespace Insyston.Operations.WPF.ViewModel.Receipts.DishonourReversal
{
    public class AddReceiptsViewModel : OldViewModelBase, IDataErrorInfo
    {
        private List<DropdownList> selectList;
        private List<DishonourReversalReceiptSearch> dishnourReversalReceipts;
        private ObservableCollection<object> selectedReceipts;

        private ReceiptBatch receiptBatch;
        private int batchType, selectFrom, selectedReceipt;
        private int? selectFromObjectID;
        private DateTime receiptDate;
        private bool isChanged;

        public DelegateCommand Save { get; private set; }
        public DelegateCommand Cancel { get; private set; }
        public DelegateCommand SearchCommand { get; private set; }
        public DelegateCommand<ObservableCollection<object>> ReceiptSelectedCommand { get; private set; }

        public InteractionRequest<ConfirmationWindowViewModel> UIConfirmation { get; private set; }
        public InteractionRequest<PopupWindow> Popup { get; private set; }

        public AddReceiptsViewModel()
        {
            ReceiptBatch = new ReceiptBatch();
            ReceiptBatch.ReceiptDate = DateTime.Today;
            Popup = new InteractionRequest<PopupWindow>();
        }

        public AddReceiptsViewModel(int receiptbatchType, int batchID)
        {
            BatchType batch;

            try
            {
                batchType = receiptbatchType;
                ReceiptBatch = ReceiptBatchFunctions.Get(batchID);
                ReceiptDate = ReceiptBatch.ReceiptDate;

                Save = new DelegateCommand(OnSave);
                SearchCommand = new DelegateCommand(OnSearch);
                ReceiptSelectedCommand = new DelegateCommand<ObservableCollection<object>>(ReceiptSelected);

                UIConfirmation = new InteractionRequest<ConfirmationWindowViewModel>();
                Popup = new InteractionRequest<PopupWindow>();
                SelectList = BatchTypeFunctions.GetSelectList(batchType);
                SetReceiptBatchDefaults();

                IconFileName = "AddMultiple.jpg";
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error encountered while initializing Add Receipts.", "Add Receipts - Error");
            }
            finally
            {
                IsBusy = false;
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
                        if (batchType == (int)ReceiptBatchType.Dishonour)
                        {
                            DishonourReversalReceipts = DishonourReversalFunctions.GetDishonourReceipts(selectFrom, 0, receiptBatch.BankAccountID.GetValueOrDefault());
                        }
                        else
                        {
                            DishonourReversalReceipts = DishonourReversalFunctions.GetReversalReceipts(selectFrom, 0, receiptBatch.BankAccountID.GetValueOrDefault());
                        }
                    }
                    else
                    {
                        DishonourReversalReceipts = null;
                    }

                    RaisePropertyChanged("SelectFrom");
                    RaisePropertyChanged("ApplyToLabelCaption");
                    RaisePropertyChanged("IsSelectFromContract");
                    RaisePropertyChanged("IsSelectFromQuote");
                    RaisePropertyChanged("IsSuspense");
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
                        if (batchType == (int)ReceiptBatchType.Dishonour)
                        {
                            DishonourReversalReceipts = DishonourReversalFunctions.GetDishonourReceipts(selectFrom, selectFromObjectID.Value, receiptBatch.BankAccountID.GetValueOrDefault());
                        }
                        else
                        {
                            DishonourReversalReceipts = DishonourReversalFunctions.GetReversalReceipts(selectFrom, selectFromObjectID.Value, receiptBatch.BankAccountID.GetValueOrDefault());
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

        public int DishonourColumnWidth
        {
            get
            {
                if (batchType == (int)ReceiptBatchType.Dishonour || batchType == (int)ReceiptBatchType.Reversals)
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

                if (item != null)
                {
                    return item.Description + " No.";
                }

                return string.Empty;
            }
        }

        public bool IsSelectListApplicable
        {
            get
            {
                return true;
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

        public bool IsReversalBatch
        {
            get
            {
                return batchType == (int)ReceiptBatchType.Reversals;
            }
        }

        private void SetReceiptBatchDefaults()
        {
            LXMSystemReceiptDefault batchTypeDefaults;

            if (batchType != 0)
            {
                batchTypeDefaults = BatchTypeFunctions.GetReceiptBatchSystemDefaults(batchType);

                if (selectList.Count > 0)
                {
                    SelectFrom = selectList.FirstOrDefault().ID;
                }

                if (batchTypeDefaults != null)
                {                    
                    if (batchTypeDefaults.DefaultSelectListID.HasValue && batchTypeDefaults.DefaultSelectListID.Value > 0)
                    {
                        SelectFrom = batchTypeDefaults.DefaultSelectListID.Value;
                    }                    
                }                
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
                        else
                        {
                            if (selectedReceipts.Select(item => ((DishonourReversalReceiptSearch)item).BankAccountID).Distinct().Count() > 1)
                            {
                                return "Please select Receipt(s) from a single Bank Account";
                            }
                        }
                        break;
                }

                return string.Empty;
            }
        }

        private void OnSave()
        {
            isChanged = true;

            if (Validate() == true)
            {
                if (ReceiptBatch.ReceiptDate > DateTime.Today)
                {
                    UIConfirmation.Raise(
                        new ConfirmationWindowViewModel(this) { Content = "Receipt Date is in the future. Select OK to continue or Cancel to modify", Icon = "Question.ico", Title = "Receipt Date Confirmation" },
                        (popupCallBack) =>
                        {
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
            OpenItemReceiptAllocation newAllocation;
            Receipt receipt = null;

            try
            {
                isChanged = true;

                if (selectedReceipts != null)
                {
                    receiptBatch.BankAccountID = ((DishonourReversalReceiptSearch)selectedReceipts.FirstOrDefault()).BankAccountID;
                    receiptBatch.FilterDescription = "Bank: " + ((DishonourReversalReceiptSearch)selectedReceipts.FirstOrDefault()).BankName;

                    foreach (DishonourReversalReceiptSearch selectedReceipt in selectedReceipts)
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

                        receipt.ReceiptBatchID = receiptBatch.ID;
                        receipt.ContractID = selectedReceipt.ContractNo;
                        receipt.QuoteID = selectedReceipt.QuoteID;

                        if (batchType == (int)ReceiptBatchType.Dishonour)
                        {
                            receipt.Reference = "Dishonour";
                        }
                        else
                        {
                            receipt.Reference = "Reversal";
                        }

                        receipt.LastUserID = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
                        receipt.LastDateModified = DateTime.Now;

                        if (batchType == (int)ReceiptBatchType.Reversals)
                        {
                            receipt.ReceiptDate = selectedReceipt.ReceiptDate;
                        }
                        else
                        {
                            receipt.ReceiptDate = ReceiptDate;
                        }

                        receipt.NetAmountReceived = selectedReceipt.NetAmountReceived.GetValueOrDefault() * -1;
                        receipt.GSTAmountReceived = selectedReceipt.GSTAmountReceived.GetValueOrDefault() * -1;
                        receipt.FIDAmountReceived = selectedReceipt.FIDAmountReceived.GetValueOrDefault() * -1;
                        receipt.SDAmountReceived = selectedReceipt.SDAmountReceived.GetValueOrDefault() * -1;
                        receipt.GrossAmountReceived = selectedReceipt.AmountReceived * -1;

                        if (batchType == (int)ReceiptBatchType.Dishonour)
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

                        receiptBatch.Receipts.Add(receipt);
                        receiptBatch.GrossBatchTotal += receipt.GrossAmountReceived;
                        receiptBatch.NumberOfEntries++;
                    }

                    ReceiptBatchFunctions.Save(receiptBatch);
                }

                Close();
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error encountered while Saving receipt entries.", "Add Receipts - Error");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnSearch()
        {
            IsBusy = true;
            switch (SelectFrom)
            {
                case (int)ReceiptBatchSelectList.Contract:
                    PopupWindow<ContractReceiptSearch> contractPopup;
                    DelegateSearchFilter<ContractReceiptSearch> contractSearch;

                    contractSearch = new DelegateSearchFilter<ContractReceiptSearch>(ContractFunctions.SearchContractReceipt);
                    contractPopup = new PopupWindow<ContractReceiptSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search"
                            , new SearchViewModel<ContractReceiptSearch>(contractSearch, "Search Contract", batchType, receiptBatch.BankAccountID.GetValueOrDefault()), true);

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

                    quoteSearch = new DelegateSearchFilter<QuoteReceiptSearch>(QuoteFunctions.SearchQuoteReceipt);
                    quotePopup = new PopupWindow<QuoteReceiptSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<QuoteReceiptSearch>(quoteSearch, "Search Quote", batchType, receiptBatch.BankAccountID.GetValueOrDefault()), true);

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

                    clientSearch = new DelegateSearchFilter<ClientReceiptSearch>(ClientFunctions.SearchClientReceipt);
                    clientPopup = new PopupWindow<ClientReceiptSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<ClientReceiptSearch>(clientSearch, "Search Client", batchType, receiptBatch.BankAccountID.GetValueOrDefault()), true);

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
                    batchPopup = new PopupWindow<BatchReceiptSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<BatchReceiptSearch>(batchSearch, "Search Receipt Batch", batchType, receiptBatch.BankAccountID.GetValueOrDefault()), true);

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
                    isValid = ContractFunctions.ContractExistsinReceipt(selectFromObjectID.Value);
                    break;
                case (int)ReceiptBatchSelectList.Quote:
                    isValid = QuoteFunctions.QuoteExistsinReceipt(selectFromObjectID.Value);
                    break;
                case (int)ReceiptBatchSelectList.Client:
                    isValid = ClientFunctions.ClientExistsinReceipt(selectFromObjectID.Value);
                    break;
                case (int)ReceiptBatchSelectList.Batch:
                    isValid = ReceiptBatchFunctions.BatchExists(selectFromObjectID.Value);
                    break;
            }

            return isValid;
        }

        private void ReceiptSelected(ObservableCollection<object> selectedItems)
        {
            selectedReceipts = selectedItems;
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
