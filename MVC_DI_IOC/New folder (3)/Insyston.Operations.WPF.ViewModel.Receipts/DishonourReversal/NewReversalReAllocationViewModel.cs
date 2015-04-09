using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.Common;
using Insyston.Operations.WPF.ViewModel.Common;
using Insyston.Operations.WPF.ViewModel.Common.Controls;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using business = Insyston.Operations.Business.Common;

namespace Insyston.Operations.WPF.ViewModel.Receipts.DishonourReversal
{
    public class NewReversalReAllocationViewModel : NewDishonourReversalViewModel, IDataErrorInfo
    {
        private List<string> reallocateReceiptToList;
        
        private int reallocateTo;
        private int? reallocateToObjectID;

        public DelegateCommand ReAllocationSearch { get; private set; }
        public DelegateCommand ReAllocateOK { get; private set; }

        public NewReversalReAllocationViewModel(NewDishonourReversalViewModel viewModel, int reallocationReceiptID)
            : base(ReceiptBatchType.Reversals, viewModel.ReceiptBatchID, viewModel.BatchStatus)
        {
            Receipt = viewModel.Receipt;
            ClientName = viewModel.ClientName;
            ReAllocationReceiptID = reallocationReceiptID;
            ApplyToObjectID = viewModel.ApplyToObjectID;
            IconFileName = viewModel.IconFileName;
            OpenItems = viewModel.OpenItems;            

            if (viewModel.ReturnValue != null)
            {
                ReAllocateTo = viewModel.ReturnValue.Item1;
                ReAllocateToObjectID = viewModel.ReturnValue.Item2;
                Reference = viewModel.ReturnValue.Item3;
            }

            ReAllocateReceiptToList = new List<string>();

            foreach (string option in Enum.GetNames(typeof(ReAllocateReceiptTo)))
            {
                ReAllocateReceiptToList.Add(Regex.Replace(option, "([a-z])([A-Z])", "$1 $2"));
            }

            ReAllocationSearch = new DelegateCommand(OnReAllocationSearch);
            ReAllocateOK = new DelegateCommand(OnReAllocateOK);
            SetIcon();
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

        private void OnReAllocateOK()
        {
            isChanged = true;
            isReallocateOK = true;

            if (Validate())
            {
                ReturnValue = new Tuple<int, int?, string>(reallocateTo, reallocateToObjectID, Reference);
                RaisePropertyChanged("IsSaveEnabled");
                Close();
            }

            isReallocateOK = false;
        }   

        string IDataErrorInfo.this[string columnName]
        {
            get 
            {
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

        private void SetIcon()
        {
            if (ReAllocationReceiptID != 0)
            {
                if (IsReceiptEditable)
                {
                    Title = "Modify Receipt ReAllocation";
                }
                else
                {
                    Title = "View Receipt ReAllocation";
                    IconFileName = "View.jpg";
                }
            }
            else
            {
                Title = "Receipt ReAllocation";
            }
        }     
    }
}
