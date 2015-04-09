using Insyston.Operations.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Insyston.Operations.Logging;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.Business.Receipts.Model;
using Insyston.Operations.WPF.ViewModel.Common;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.WPF.ViewModel.Common.Controls;
using Insyston.Operations.Security;
using business = Insyston.Operations.Business.Common;
using System.Threading;
using Insyston.Operations.Common;

namespace Insyston.Operations.WPF.ViewModel.Receipts.DishonourReversal
{
    public class MultipleReversalReAllocationViewModel : OldViewModelBase, IDataErrorInfo
    {
        private readonly int receiptBatchID, reasonSelected;

        private int reallocateTo;
        private int? reallocateToObjectID;
        private DateTime receiptDate;
        private bool isChanged, returnValue;
        private string reference;

        private List<string> reallocateReceiptToList;
        private List<DishonourReversalReceiptSummary> reversalReceipts;

        public InteractionRequest<PopupWindow> Popup { get; private set; }
        public InteractionRequest<ConfirmationWindowViewModel> UIConfirmation { get; private set; }

        public delegate void ClearGridFilterEventHandler();
        public event ClearGridFilterEventHandler ClearGridFilter;

        public DelegateCommand<string> Save { get; private set; }
        public DelegateCommand ReAllocationSearch { get; private set; }

        public delegate void GridViewTotalsChangedHandler();

        public MultipleReversalReAllocationViewModel(int batchID, int reason, List<DishonourReversalReceiptSummary> receipts)
        {
            receiptBatchID = batchID;
            reasonSelected = reason;
            reversalReceipts = receipts;

            try
            {
                SetReceiptDefaults(receiptBatchID);

                Save = new DelegateCommand<string>(OnSave);
                ReAllocationSearch = new DelegateCommand(OnReAllocationSearch);

                Popup = new InteractionRequest<PopupWindow>();
                UIConfirmation = new InteractionRequest<ConfirmationWindowViewModel>();
                IconFileName = "ReAllocation.jpg";
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error occurred while initializing Reversal Receipts Re-Allocation Screen", "Receipt Reallocation - Error");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public List<string> ReAllocateReceiptToList
        {
            get
            {
                return reallocateReceiptToList;
            }
            set
            {
                if (reallocateReceiptToList != value)
                {
                    reallocateReceiptToList = value;
                    RaisePropertyChanged("ReAllocateReceiptTo");
                }
            }
        }

        public List<DishonourReversalReceiptSummary> ReversalReceipts
        {
            get
            {
                return reversalReceipts;
            }
            set
            {
                if (reversalReceipts != value)
                {
                    reversalReceipts = value;
                    RaisePropertyChanged("ReversalReceipts");
                }
            }
        }

        public int ReAllocateTo
        {
            get
            {
                return reallocateTo;
            }
            set
            {
                if (reallocateTo != value)
                {
                    reallocateTo = value;
                    ReAllocateToObjectID = null;
                    RaisePropertyChanged("ReAllocateTo");
                    RaisePropertyChanged("ReAllocateToObjectCaption");
                    RaisePropertyChanged("IsReAllocateToSuspense");
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

        public string ReAllocateToObjectCaption
        {
            get
            {
                if (reallocateTo == (int)ReAllocateReceiptTo.Contract)
                {
                    return "Contract No.";
                }
                else if (reallocateTo == (int)ReAllocateReceiptTo.Quote)
                {
                    return "Quote No.";
                }

                return string.Empty;
            }
        }

        public int? ReAllocateToObjectID
        {
            get
            {
                return reallocateToObjectID;
            }
            set
            {
                if (reallocateToObjectID != value)
                {
                    reallocateToObjectID = value;
                    RaisePropertyChanged("ReAllocateToObjectID");
                }
            }
        }

        public bool ReturnValue
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

        public bool IsReAllocateToSuspense
        {
            get
            {
                return (reallocateTo == (int)ReAllocateReceiptTo.Suspense);
            }
        }

        private void OnReAllocationSearch()
        {
            IsBusy = true;
            if (reallocateTo == (int)ReAllocateReceiptTo.Contract)
            {
                PopupWindow<ContractSearch> contractPopup;
                DelegateSearch<ContractSearch> contractSearch = new DelegateSearch<ContractSearch>(ContractFunctions.Search);
                contractPopup = new PopupWindow<ContractSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<ContractSearch>(contractSearch, "Search Contract"), true);

                Popup.Raise(contractPopup, (popupCallBack) =>
                {
                    if (popupCallBack.ReturnValue != null)
                    {
                        ContractSearch contract = (ContractSearch)popupCallBack.ReturnValue;                        
                        ReAllocateToObjectID = contract.ContractID;
                    }

                    popupCallBack = null;
                });
            }
            else if (reallocateTo == (int)ReAllocateReceiptTo.Quote)
            {
                PopupWindow<QuoteSearch> quotePopup;
                DelegateSearch<QuoteSearch> quoteSearch = new DelegateSearch<QuoteSearch>(business.QuoteFunctions.Search);
                quotePopup = new PopupWindow<QuoteSearch>(ViewAssemblies.ViewCommonAssembly, "Controls.Search", new SearchViewModel<QuoteSearch>(quoteSearch, "Search Invoice"), true);

                Popup.Raise(quotePopup, (popupCallBack) =>
                {
                    if (popupCallBack.ReturnValue != null)
                    {
                        QuoteSearch quote = (QuoteSearch)popupCallBack.ReturnValue;                        
                        ReAllocateToObjectID = quote.QuoteNo;
                    }
                });
            }

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
                                SaveReceipt();
                            }
                        });
                }
                else
                {
                    SaveReceipt();
                }
            }
        }

        private void SaveReceipt()
        {
            ReceiptBatch receiptBatch;
            Receipt receipt = null;
            OpenItemReceiptAllocation openItemReceiptAllocation;

            try
            {
                receiptBatch = ReceiptBatchFunctions.Get(receiptBatchID);

                if (receiptBatch.Receipts == null)
                {
                    receiptBatch.Receipts = new List<Receipt>();
                }

                foreach (DishonourReversalReceiptSummary reversalReceipt in reversalReceipts)
                {
                    receipt = new Receipt();

                    switch (reallocateTo)
                    {
                        case (int)ReAllocateReceiptTo.Contract:
                            receipt.ApplyToTypeID = (int)ReceiptApplyTo.Contract;
                            receipt.ContractID = ReAllocateToObjectID;

                            if (reversalReceipt.ApplyToObjectID.HasValue)
                            {
                                receipt.InternalReference = reversalReceipt.ApplyToObjectID.Value.ToString();
                            }
                            break;
                        case (int)ReAllocateReceiptTo.Quote:
                            receipt.ApplyToTypeID = (int)ReceiptApplyTo.Quote;
                            receipt.QuoteID = ReAllocateToObjectID;

                            if (reversalReceipt.ApplyToObjectID.HasValue)
                            {
                                receipt.InternalReference = reversalReceipt.ApplyToObjectID.Value.ToString();
                            }
                            break;
                        case (int)ReAllocateReceiptTo.Suspense:
                            receipt.ApplyToTypeID = (int)ReceiptApplyTo.Suspense;
                            break;
                    }

                    receipt.ReceiptBatchID = receiptBatchID;
                    receipt.Reference = Reference;

                    receipt.LastUserID = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId; ;
                    receipt.LastDateModified = DateTime.Now;
                    receipt.ReceiptDate = DateTime.Today; // receiptDate;

                    receipt.NetAmountReceived = reversalReceipt.NetAmount * (-1);
                    receipt.GSTAmountReceived = reversalReceipt.GSTAmount * (-1);
                    receipt.FIDAmountReceived = reversalReceipt.FIDAmount * (-1);
                    receipt.SDAmountReceived = reversalReceipt.SDAmount * (-1);
                    receipt.GrossAmountReceived = reversalReceipt.GrossAmount * (-1);

                    receipt.ReversalReceiptDetail = new ReversalReceiptDetail { LinkedReceiptID = reversalReceipt.ID, ReasonCodeID = reasonSelected, ReversalTypeID = (int)ReversalTypes.Reallocation };

                    if (reallocateTo != (int)ReAllocateReceiptTo.Suspense)
                    {
                        foreach (OpenItemReceiptAllocation allocation in DishonourReversalFunctions.GetOpenItemReceiptAllocations(reversalReceipt.ID))
                        {
                            openItemReceiptAllocation = new OpenItemReceiptAllocation();

                            openItemReceiptAllocation.OpenItemID = allocation.OpenItemID;
                            openItemReceiptAllocation.NetAmountApplied = allocation.NetAmountApplied;
                            openItemReceiptAllocation.GSTAmountApplied = allocation.GSTAmountApplied;
                            openItemReceiptAllocation.FIDAmountApplied = allocation.FIDAmountApplied;
                            openItemReceiptAllocation.SDAmountApplied = allocation.SDAmountApplied;
                            openItemReceiptAllocation.GrossAmountApplied = allocation.GrossAmountApplied;
                            receipt.OpenItemReceiptAllocations.Add(openItemReceiptAllocation);
                        }
                    }

                    receiptBatch.NumberOfEntries++;
                    receiptBatch.GrossBatchTotal += receipt.GrossAmountReceived;

                    receiptBatch.Receipts.Add(receipt);
                }

                ReceiptBatchFunctions.Save(receiptBatch);
                ReturnValue = true;

                Close();
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error occurred while Saving Re-Allocation Receipt", "Receipt Reallocation - Error");
            }
            finally
            {
                IsBusy = false;
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
                    case "reallocatetoobjectid":
                        if (reallocateToObjectID.HasValue)
                        {
                            if (ReAllocateTo == (int)ReAllocateReceiptTo.Contract && business.ContractFunctions.ContractExists(reallocateToObjectID.Value) == false)
                            {
                                return "Contract No. is Invalid";
                            }
                            else if (ReAllocateTo == (int)ReAllocateReceiptTo.Quote && business.QuoteFunctions.QuoteExists(reallocateToObjectID.Value) == false)
                            {
                                return "Quote No. is Invalid";
                            }
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

            receiptBatch = ReceiptBatchFunctions.Get(receiptBatchID);
            ReceiptDate = receiptBatch.ReceiptDate;
            
            ReAllocateReceiptToList = new List<string>();

            foreach (string option in Enum.GetNames(typeof(ReAllocateReceiptTo)))
            {
                ReAllocateReceiptToList.Add(Regex.Replace(option, "([a-z])([A-Z])", "$1 $2"));
            }
        }
    }
}
