using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModel.Events;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Insyston.Operations.Logging;
using System.Text;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.WPF.ViewModel.Common;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.Business.Receipts.Model;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Business.Common;
using Insyston.Operations.Security;
using Insyston.Operations.WPF.View.Receipts;
using System.Threading;
using Insyston.Operations.Common;

namespace Insyston.Operations.WPF.ViewModel.Receipts
{
    [Export(typeof(ReceiptsSummaryViewModel))]
    public class ReceiptsSummaryViewModel : OldViewModelBase, INavigationAware
    {
        private readonly IEventAggregator _EventAggregator;
        private readonly IRegionManager regionManager;
        public InteractionRequest<PopupWindow> Popup { get; private set; }
        public InteractionRequest<ConfirmationWindowViewModel> UIConfirmation { get; private set; }

        public DelegateCommand Back { get; private set; }
        public DelegateCommand Refresh { get; private set; }
        public DelegateCommand Add { get; private set; }
        public DelegateCommand AddMultiple { get; private set; }
        public DelegateCommand Edit { get; private set; }
        public DelegateCommand Delete { get; private set; }
        public DelegateCommand StatusForward { get; private set; }
        public DelegateCommand StatusBackward { get; private set; }
        public DelegateCommand ImportExcel { get; private set; }
        public DelegateCommand Assign { get; private set; }
        public DelegateCommand<ObservableCollection<Object>> DishonourReversalBatchesSelected { get; private set; }

        private ReceiptBatchType receiptBatchType;
        private string batchType;
        private ReceiptBatch receiptBatch;
        private List<ReceiptSummary> receipts;
        private List<DishonourReversalReceiptSummary> dishonourReversalReceipts;
        private List<DropdownList> reasonCodes;
        private List<string> dishonourReversalOptions;

        private ReceiptSummary selectedReceipt;
        private DishonourReversalReceiptSummary selectedDishonourReversalReceipt;
        private ObservableCollection<object> selectedDishonourReversalReceipts;
        private int batchID, receiptID, selectedDishonourReversalOption, reasonSelected;

        public delegate void ClearGridFilterHandler();
        public event ClearGridFilterHandler ClearGridFilter;

        [ImportingConstructor]
        public ReceiptsSummaryViewModel(IEventAggregator evtAggregator, IRegionManager regManager)
        {
            _EventAggregator = evtAggregator;
            regionManager = regManager;
            DishonourReversalOptions = new List<string>();
            
            Back = new DelegateCommand(OnBack);
            Refresh = new DelegateCommand(OnRefresh);            
            Popup = new InteractionRequest<PopupWindow>();
            UIConfirmation = new InteractionRequest<ConfirmationWindowViewModel>();

            Add = new DelegateCommand(OnAdd);
            AddMultiple = new DelegateCommand(OnAddMultiple);
            Edit = new DelegateCommand(OnEdit);
            Delete = new DelegateCommand(OnDelete);
            StatusForward = new DelegateCommand(OnStatusForward);
            StatusBackward = new DelegateCommand(OnStatusBackward);
            ImportExcel = new DelegateCommand(OnImportExcel);
            Assign = new DelegateCommand(OnAssign);
            DishonourReversalBatchesSelected = new DelegateCommand<ObservableCollection<object>>(OnDishonourReversalBatchesSelected);

            LockTableName = "ReceiptBatch";
        }

        public string BatchType
        {
            get
            {
                return batchType;
            }
            set
            {
                batchType = value;
                RaisePropertyChanged("BatchType");
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

        public List<ReceiptSummary> Receipts
        {
            get
            {
                return receipts;
            }
            set
            {
                receipts = value;
                RaisePropertyChanged("Receipts");
            }

        }

        public List<DishonourReversalReceiptSummary> DishonourReversalReceipts
        {
            get
            {
                return dishonourReversalReceipts;
            }
            set
            {
                if (dishonourReversalReceipts != value)
                {
                    dishonourReversalReceipts = value;
                    RaisePropertyChanged("DishonourReversalReceipts");
                }
            }
        }
       
        public ReceiptSummary SelectedReceipt
        {
            get
            {
                return selectedReceipt;
            }
            set
            {
                if (selectedReceipt != value)
                {
                    selectedReceipt = value;
                    RaisePropertyChanged("SelectedReceipt");

                    if (value != null)
                    {
                        receiptID = value.ID;
                    }
                    else
                    {
                        receiptID = 0;
                    }

                    RaisePropertyChanged("IsReceiptSelected");
                }
            }
        }

        public DishonourReversalReceiptSummary SelectedDishonourReversalReceipt
        {
            get
            {
                return selectedDishonourReversalReceipt;
            }
            set
            {
                if (selectedDishonourReversalReceipt != value)
                {
                    selectedDishonourReversalReceipt = value;
                    RaisePropertyChanged("SelectedDishonourReversalReceipt");

                    if (value != null)
                    {
                        receiptID = value.ID;
                    }
                    else
                    {
                        receiptID = 0;
                    }

                    RaisePropertyChanged("IsReceiptSelected");
                    RaisePropertyChanged("IsReasonApplicable");
                }
            }
        }

        public List<String> DishonourReversalOptions
        {
            get
            {
                return dishonourReversalOptions;
            }
            set
            {
                if (dishonourReversalOptions != value)
                {
                    dishonourReversalOptions = value;
                    RaisePropertyChanged("DishonourReversalOptions");
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

        public string StatusForwardText
        {            
            get
            {
                ReceiptBatchStatus batchStatus;

                if (receiptBatch != null)
                {
                    batchStatus = ((ReceiptBatchStatus)receiptBatch.BatchStatusID);

                    if (batchStatus == ReceiptBatchStatus.Created)
                    {
                        return "Move Status to " + ReceiptBatchStatus.Pending.ToString();
                    }
                    else if (batchStatus == ReceiptBatchStatus.Pending)
                    {
                        return "Move Status to " + ReceiptBatchStatus.Posted.ToString();
                    }
                }

                return string.Empty;
            }
            set
            {                
            }
        }

        public string StatusBackwardText
        {
            get
            {
                ReceiptBatchStatus batchStatus;

                if (receiptBatch != null)
                {
                    batchStatus = ((ReceiptBatchStatus)receiptBatch.BatchStatusID);

                    if (batchStatus == ReceiptBatchStatus.Pending)
                    {
                        return "Revert Status to " + ReceiptBatchStatus.Created.ToString();
                    }
                }

                return string.Empty;
            }
            set
            {
            }
        }

        public int SelectedDishonourReversalOption
        {
            get
            {
                return selectedDishonourReversalOption;
            }
            set
            {
                if (selectedDishonourReversalOption != value)
                {
                    selectedDishonourReversalOption = value;
                    RaisePropertyChanged("SelectedDishonourReversalOption");
                    RaisePropertyChanged("IsReasonApplicable");
                }
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
                    RaisePropertyChanged("ResonSelected");
                }
            }
        }

        public string EditToolTipText
        {
            get
            {
                if (IsLocked && IsReceiptCreated)
                {
                    return "Edit Receipt";
                }
                else
                {
                    return "View Receipt";
                }
            }
            set
            {
            }
        }

        public string DDCCAcountNoCaption
        {
            get
            {
                if (receiptBatchType == ReceiptBatchType.DirectDebit)
                {
                    return "Account No.";
                }
                else if (receiptBatchType == ReceiptBatchType.CreditCard)
                {
                    return "Card No.";
                }

                return string.Empty;
            }
        }

        public string ReceiptDateCaption
        {
            get
            {
                if (receiptBatchType == ReceiptBatchType.Dishonour)
                {
                    return "Dish. Date";
                }
                else
                {
                    return "Reversed Date";
                }
            }
        }

        public string LinkedReceiptIDCaption
        {
            get
            {
                if (receiptBatchType == ReceiptBatchType.Dishonour)
                {
                    return "Dish. Receipt ID";
                }
                else
                {
                    return "Reversed Receipt ID";
                }
            }
        }

        public bool IsStatusForwardVisible
        {
            get
            {
                return string.IsNullOrEmpty(StatusForwardText) == false;
            }
            set
            {
            }
        }

        public bool IsStatusBackwardVisible
        {
            get
            {
                return string.IsNullOrEmpty(StatusBackwardText) == false;
            }
            set
            {
            }
        }

        public bool IsReceiptSelected
        {
            get
            {
                return (receiptID != 0);
            }
            set
            {
            }
        }

        public bool IsBatchPosted
        {
            get
            {
                if (receiptBatch != null && ((ReceiptBatchStatus)receiptBatch.BatchStatusID) == ReceiptBatchStatus.Posted)
                {
                    return true;
                }

                return false;
            }
        }
        
        public bool IsDateRangeApplicable
        {
            get
            {
                if (receiptBatchType == ReceiptBatchType.DirectDebit || receiptBatchType == ReceiptBatchType.CreditCard || receiptBatchType == ReceiptBatchType.AutoReceipts)
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsStatusForwardAllowed
        {
            get
            {
                if (receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals)
                {
                    return dishonourReversalReceipts != null && dishonourReversalReceipts.Count > 0;
                }
                else
                {
                    return Receipts != null && Receipts.Count > 0;
                }
            }
            set
            {
            }
        }

        public bool IsReceiptCreated
        {
            get
            {
                return ReceiptBatch != null && ReceiptBatch.BatchStatusID == (int)ReceiptBatchStatus.Created;
            }
            set
            {
            }
        }

        public bool IsExcelImportVisible
        {
            get
            {
                return ReceiptBatch != null && ReceiptBatch.BatchStatusID == (int)ReceiptBatchStatus.Created && (receiptBatchType == ReceiptBatchType.CashCheque 
                        || receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals);
            }
            set
            {
            }
        }

        public bool IsCashChequeBatch
        {
            get
            {
                return (receiptBatchType == ReceiptBatchType.CashCheque);
            }
        }

        public bool IsDDCCBatch
        {
            get
            {
                return (receiptBatchType == ReceiptBatchType.DirectDebit || receiptBatchType == ReceiptBatchType.CreditCard);
            }
            set
            {
            }
        }

        public bool IsAddMultipleVisible
        {
            get
            {
                return (receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals) && IsReceiptCreated;
            }
        }

        public bool IsDishonourReversalBatch
        {
            get
            {
                return (receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals);
            }
        }

        public bool IsAssignReasonVisible
        {
            get
            {
                return (receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals) && ((ReceiptBatchStatus)receiptBatch.BatchStatusID) == ReceiptBatchStatus.Created;
            }
        }

        public bool IsEditVisible
        {
            get
            {
                return IsLocked || !IsReceiptCreated;
            }
        }     

        public bool IsReasonApplicable
        {
            get
            {
                return IsReceiptSelected && (selectedDishonourReversalOption == (int)DishonourReceiptOptions.AssignReason);
            }
        }

        public bool IsDishonourBatch
        {
            get
            {
                return (receiptBatchType == ReceiptBatchType.Dishonour);
            }
        }

        private void OnBack()
        {
            ReceiptBatchStatus batchStatus;
            UriQuery query = new UriQuery();

            _EventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Start);            

            batchStatus = ((ReceiptBatchStatus)receiptBatch.BatchStatusID);

            switch (batchStatus)
            {              
                case ReceiptBatchStatus.Created:
                    query.Add("Status", ReceiptBatchStatus.Created.ToString());
                    Shared.SetReceiptNavigation(new NavigationItem { BatchStatus = receiptBatch.BatchStatusID, ReceiptText = batchStatus.ToString() });
                    regionManager.RequestNavigate(Regions.ContentRegion, new Uri("/ReceiptsBatchSummary" + query.ToString(), UriKind.Relative));
                    break;
                case ReceiptBatchStatus.Pending:
                    query.Add("Status", ReceiptBatchStatus.Pending.ToString());
                    Shared.SetReceiptNavigation(new NavigationItem { BatchStatus = receiptBatch.BatchStatusID, ReceiptText = batchStatus.ToString() });
                    regionManager.RequestNavigate(Regions.ContentRegion, new Uri("/ReceiptsBatchSummary" + query.ToString(), UriKind.Relative));
                    break;
                case ReceiptBatchStatus.Posted:
                    if (string.IsNullOrEmpty(receiptBatch.BatchMonth) == false)
                    {
                        query.Add("BatchMonth", receiptBatch.BatchMonth);
                        Shared.SetReceiptNavigation(new NavigationItem { BatchStatus = receiptBatch.BatchStatusID, ReceiptText = receiptBatch.BatchMonth, BatchMonth = receiptBatch.BatchMonth });
                    }
                    else
                    {
                        query.Add("Status", ReceiptBatchStatus.Posted.ToString());
                        Shared.SetReceiptNavigation(new NavigationItem { BatchStatus = receiptBatch.BatchStatusID, ReceiptText = batchStatus.ToString() });
                    }

                    regionManager.RequestNavigate(Regions.ContentRegion, new Uri("/ReceiptsBatchSummary" + query.ToString(), UriKind.Relative));
                    break;
            }            

            _EventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Stop);
        }

        private void OnAdd()
        {
            PopupWindow popupWindow;

            IsBusy = true;
            if (receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals)
            {
                popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "DishonourReversal.NewDishonourReversalReceipt", true);            
            }
            else
            {
                popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "CommonControls.NewReceipt", true);                
            }

            popupWindow.Parameters.Add(receiptBatchType);
            popupWindow.Parameters.Add(batchID);
            popupWindow.Parameters.Add(receiptBatch.BatchStatusID);
            popupWindow.Parameters.Add(0);

            Popup.Raise(popupWindow);
            OnRefresh();
            IsBusy = false;
        }

        public void OnAddMultiple()
        {
            PopupWindow popupWindow;

            IsBusy = true;
            popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "DishonourReversal.AddReceipts", true);
            popupWindow.Parameters.Add((int)receiptBatchType);
            popupWindow.Parameters.Add(batchID);
            Popup.Raise(popupWindow, (popupCallBack) => { OnRefresh(); });
            IsBusy = false;
        }

        private void OnEdit()
        {
            PopupWindow popupWindow;

            if (receiptID == 0)
            {
                return;
            }

            IsBusy = true;
            if (receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals)
            {
                popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "DishonourReversal.NewDishonourReversalReceipt", true);
            }
            else
            {
                popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "CommonControls.NewReceipt", true);
            }

            popupWindow.Parameters.Add(receiptBatchType);
            popupWindow.Parameters.Add(batchID);
            popupWindow.Parameters.Add(receiptBatch.BatchStatusID);
            popupWindow.Parameters.Add(receiptID);

            Popup.Raise(popupWindow, (popupCallBack) => { OnRefresh(); });
            IsBusy = false;
        }

        private void OnDelete()
        {
            DishonourReversalReceiptSummary reversalReceipt;
            StringBuilder receiptIDs;
            IsBusy = true;

            try
            {
                receiptIDs = new StringBuilder();

                if (receiptBatchType != ReceiptBatchType.Dishonour && receiptBatchType != ReceiptBatchType.Reversals)
                {
                    if(ReceiptFunctions.IsLinkedReceipt(receiptID) == false)
                    {
                        ReceiptFunctions.Delete(receiptBatchType, receiptID);
                    }
                    else
                    {
                        receiptIDs.Append(receiptID.ToString());
                    }
                }
                else
                {
                    if (selectedDishonourReversalReceipts.Where(reversal => ((DishonourReversalReceiptSummary)reversal).ReversalTypeID == (int)ReversalTypes.Reallocation).Count() > 0)
                    {
                        UIConfirmation.Raise(
                            new ConfirmationWindowViewModel(this) { Content = "One or more Receipt(s) have corresponding Reversal Entries\n\nDo you want to also delete the associated Reversal Record(s)", Title = "Reversal Receipt Delete Confirmation" },
                            (popupCallBack) =>
                            {
                                foreach (DishonourReversalReceiptSummary receipt in selectedDishonourReversalReceipts)
                                {                                    
                                    if (receipt.ReversalTypeID == (int)ReversalTypes.Reallocation)
                                    {                                       
                                        ReceiptFunctions.Delete(receiptBatchType, receipt.ID);

                                        if (popupCallBack.Confirmed)
                                        {
                                            ReceiptFunctions.Delete(receiptBatchType, receipt.LinkedReceiptID.GetValueOrDefault());
                                        }
                                    }
                                    else if (receipt.ReversalTypeID == (int)ReversalTypes.Reversal)
                                    {
                                        reversalReceipt = dishonourReversalReceipts.Where(item => item.LinkedReceiptID == receipt.ID).FirstOrDefault();

                                        if (reversalReceipt != null)
                                        {
                                            ReceiptFunctions.Delete(receiptBatchType, reversalReceipt.ID);
                                        }

                                        ReceiptFunctions.Delete(receiptBatchType, receipt.ID);
                                    }
                                }

                                OnRefresh();
                                RaisePropertyChanged("StatusForwardText");
                                RaisePropertyChanged("StatusBackwardText");
                                RaisePropertyChanged("IsStatusForwardVisible");
                                RaisePropertyChanged("IsStatusBackwardVisible");
                                RaisePropertyChanged("IsStatusForwardAllowed");
                                IsBusy = false;
                            });

                        return;
                    }
                    else
                    {
                        foreach (DishonourReversalReceiptSummary receipt in selectedDishonourReversalReceipts)
                        {
                            if (receiptBatchType == ReceiptBatchType.Reversals)
                            {
                                reversalReceipt = dishonourReversalReceipts.Where(item => item.LinkedReceiptID == receipt.ID).FirstOrDefault();

                                if (reversalReceipt != null)
                                {
                                    ReceiptFunctions.Delete(receiptBatchType, reversalReceipt.ID);
                                }

                                ReceiptFunctions.Delete(receiptBatchType, receipt.ID);
                            }
                            else
                            {
                                if(ReceiptFunctions.IsLinkedReceipt(receipt.ID) == false)
                                {
                            
                                    ReceiptFunctions.Delete(receiptBatchType, receipt.ID);
                                }
                                else
                                {
                                    if (receiptIDs.Length > 0)
                                    {
                                        receiptIDs.Append(",");
                                    }

                                    receiptIDs.Append(receipt.ID.ToString());
                                }
                            }                            
                        }
                    }
                }

                if(receiptIDs.Length > 0)
                {
                    ShowMessage("Following Receipt(s) can not be deleted since they are linked to other Receipt(s)\n" + receiptIDs.ToString(), "Receipt Deletion - Validation");
                }

                OnRefresh();
                RaisePropertyChanged("StatusForwardText");
                RaisePropertyChanged("StatusBackwardText");
                RaisePropertyChanged("IsStatusForwardVisible");
                RaisePropertyChanged("IsStatusBackwardVisible");
                RaisePropertyChanged("IsStatusForwardAllowed");
                IsBusy = false;
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error encountered while deleting Receipt(s).", "Receipt Delete - Error");
                IsBusy = false;
            }
        }
        
        private void OnRefresh()
        {
            IsBusy = true;
            ReceiptBatch = ReceiptBatchFunctions.Get(batchID, true);
            LoadReceiptDetails(batchID);
            Shared.LoadReceiptNavigation();
            ClearGridFilter();
            RaisePropertyChanged("StatusForwardText");
            RaisePropertyChanged("StatusBackwardText");
            RaisePropertyChanged("IsStatusForwardVisible");
            RaisePropertyChanged("IsStatusBackwardVisible");
            RaisePropertyChanged("IsStatusForwardAllowed");
            IsBusy = false;
        }

        private void OnStatusForward()
        {
            PopupWindow popupWindow;
            ReceiptBatchStatus batchStatus, newStatus;

            try
            {
                batchStatus = ((ReceiptBatchStatus)receiptBatch.BatchStatusID);

                if (batchStatus == ReceiptBatchStatus.Created)
                {
                    if (receiptBatchType == ReceiptBatchType.DirectDebit || receiptBatchType == ReceiptBatchType.CreditCard)
                    {
                        popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "DDCC.DDCCStatusChange", true);
                        popupWindow.Parameters.Add(receiptBatchType);
                        IsBusy = true;
                        Popup.Raise(popupWindow);
                        OnBack();
                        IsBusy = false;
                    }
                    else
                    {
                        if (receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals)
                        {
                            if (BatchTypeFunctions.GetReceiptBatchSystemDefaults((int)receiptBatchType).EnforceReasonCode.GetValueOrDefault() && DishonourReversalFunctions.IsReceiptExistsWithoutReasonCode((int)receiptBatchType, receiptBatch.ID))
                            {
                                ShowMessage(receiptBatchType.ToString() + " Reason must be Entered for all Receipts before moving the Batch to Pending", receiptBatchType.ToString() + " Reason - Validation");
                                return;
                            }
                        }

                        newStatus = ReceiptBatchStatus.Pending;
                        receiptBatch.BatchStatusID = (int)newStatus;

                        ReceiptBatchFunctions.UpdateStatus(receiptBatch.ID, (int)newStatus, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId);

                        Shared.MoveNavigationItem(new NavigationItem { BatchStatus = (int)batchStatus, BatchMonth = receiptBatch.BatchMonth, ReceiptID = receiptBatch.ID }, receiptBatch.BatchStatusID);
                        receiptBatch.BatchStatusID = (int)batchStatus;
                        OnBack();
                    }
                }
                else
                {
                    newStatus = ReceiptBatchStatus.Posted;
                    receiptBatch.PostedDate = DateTime.Today;
                    receiptBatch.BatchMonth = SystemFunctions.GetAccountingParam().StartDate.ToString("MMM yyyy");

                    receiptBatch.BatchStatusID = (int)newStatus;

                    ReceiptBatchFunctions.UpdateStatus(receiptBatch.ID, (int)newStatus, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId);

                    Shared.MoveNavigationItem(new NavigationItem { BatchStatus = (int)batchStatus, BatchMonth = receiptBatch.BatchMonth, ReceiptID = receiptBatch.ID }, receiptBatch.BatchStatusID);
                    receiptBatch.BatchStatusID = (int)batchStatus;
                    OnBack();
                }

                UnLock();
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error encountered while moving Receipt Batch", "Receipt Batch - Error");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnStatusBackward()
        {
            ReceiptBatchStatus batchStatus, newStatus;

            try
            {
                batchStatus = ((ReceiptBatchStatus)receiptBatch.BatchStatusID);

                if (batchStatus == ReceiptBatchStatus.Posted)
                {
                    newStatus = ReceiptBatchStatus.Pending;
                    receiptBatch.PostedDate = null;
                }
                else
                {
                    newStatus = ReceiptBatchStatus.Created;
                }

                receiptBatch.BatchStatusID = (int)newStatus;
                ReceiptBatchFunctions.UpdateStatus(receiptBatch.ID, (int)newStatus, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId);

                if (Shared.MoveNavigationItem(new NavigationItem { BatchStatus = (int)batchStatus, BatchMonth = receiptBatch.BatchMonth, ReceiptID = receiptBatch.ID }, receiptBatch.BatchStatusID))
                {
                    receiptBatch.BatchMonth = null;
                }

                receiptBatch.BatchStatusID = (int)batchStatus;
                OnBack();
                UnLock();
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error encountered while moving Receipt Batch", "Receipt Batch - Error");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnImportExcel()
        {
            PopupWindow popupWindow;

            IsBusy = true;
            if (receiptBatchType == ReceiptBatchType.CashCheque)
            {
                popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "CashCheque.CashReceiptExcelImport", true);
                popupWindow.Parameters.Add(receiptBatch.ID);
                Popup.Raise(popupWindow);
                OnRefresh();
            }
            else if (receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals)
            {
                popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "DishonourReversal.DishonourReversalReceiptExcelImport", true);
                popupWindow.Parameters.Add(receiptBatchType);
                popupWindow.Parameters.Add(receiptBatch.ID);
                Popup.Raise(popupWindow);
                OnRefresh();
            }
            IsBusy = false;
        }

        private void OnAssign()
        {
            List<DishonourReversalReceiptSummary> reversalReceipts;
            List<int> receiptIDs;
            PopupWindow popupWindow;

            IsBusy = true;

            try
            {
                if (selectedDishonourReversalOption != (int)DishonourReceiptOptions.AssignReason)
                {
                    if (selectedDishonourReversalOption == (int)DishonourReceiptOptions.CreateCharge)
                    {
                        receiptIDs = selectedDishonourReversalReceipts.Where(item => ((DishonourReversalReceiptSummary)item).Reason != "None"
                            && ((DishonourReversalReceiptSummary)item).ApplyToType == ReceiptApplyTo.Contract.ToString()).Select(item => ((DishonourReversalReceiptSummary)item).ID).ToList();
                    }
                    else
                    {
                        receiptIDs = selectedDishonourReversalReceipts.Where(item => ((DishonourReversalReceiptSummary)item).Reason != "None").Select(item => ((DishonourReversalReceiptSummary)item).ID).ToList();
                    }
                }
                else
                {
                    receiptIDs = selectedDishonourReversalReceipts.Select(item => ((DishonourReversalReceiptSummary)item).ID).ToList();
                }

                if (receiptBatchType == ReceiptBatchType.Dishonour)
                {
                    DishonourReversalFunctions.UpdateDishonourReceiptOption(receiptIDs, SelectedDishonourReversalOption, reasonSelected, reasonCodes.Where(reason => reason.ID == reasonSelected).FirstOrDefault().Description == "None");
                }
                else if (receiptBatchType == ReceiptBatchType.Reversals)
                {
                    reversalReceipts = new List<DishonourReversalReceiptSummary>();

                    if (DishonourReversalFunctions.GetRequiresReallocation(reasonSelected))
                    {
                        foreach (DishonourReversalReceiptSummary receipt in selectedDishonourReversalReceipts)
                        {
                            if (receipt.ReversalTypeID == (int)ReversalTypes.Reversal && dishonourReversalReceipts.Where(item => item.ReversalTypeID == (int)ReversalTypes.Reallocation && item.LinkedReceiptID == receipt.ID).Count() == 0)
                            {
                                reversalReceipts.Add(receipt);
                            }
                        }
                    }

                    if (reversalReceipts.Count > 0)
                    {
                        IsBusy = true;
                        popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "DishonourReversal.MultipleReversalReAllocation", true);
                        popupWindow.Parameters.Add(batchID);
                        popupWindow.Parameters.Add(reasonSelected);
                        popupWindow.Parameters.Add(reversalReceipts);
                        Popup.Raise(popupWindow);

                        if ((bool)popupWindow.ReturnValue)
                        {
                            DishonourReversalFunctions.UpdateReversalReceiptOption(receiptIDs, SelectedDishonourReversalOption, reasonSelected);
                            OnRefresh();
                        }

                        IsBusy = false;
                        return;
                    }
                    else
                    {
                        DishonourReversalFunctions.UpdateReversalReceiptOption(receiptIDs, SelectedDishonourReversalOption, reasonSelected);
                    }
                }

                OnRefresh();
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                ShowErrorMessage("Error encountered during Receipt(s) assignment", "Receipt - Error");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _EventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Start);
            receiptBatchType = (ReceiptBatchType)Convert.ToInt32(navigationContext.Parameters["BatchType"]);
            batchID = Convert.ToInt32(navigationContext.Parameters["BatchID"]);
            ReceiptBatch = ReceiptBatchFunctions.Get(batchID, true);
            LoadReceiptDetails(batchID);

            LockUniqueIdentifier = batchID.ToString();
            receiptID = 0;
            BatchType = navigationContext.Parameters["BatchType"] + " Batch";
            RaisePropertyChanged("NewReceiptIcon");
            RaisePropertyChanged("EditReceiptIcon");
            RaisePropertyChanged("DeleteReceiptIcon");
            RaisePropertyChanged("IsReceiptSelected");
            RaisePropertyChanged("IsBatchPosted");
            RaisePropertyChanged("IsDateRangeApplicable");
            RaisePropertyChanged("StatusForwardText");
            RaisePropertyChanged("StatusBackwardText");
            RaisePropertyChanged("IsStatusForwardVisible");
            RaisePropertyChanged("IsStatusBackwardVisible");
            RaisePropertyChanged("IsStatusForwardAllowed");
            RaisePropertyChanged("IsExcelImportVisible");
            RaisePropertyChanged("IsReceiptCreated");
            RaisePropertyChanged("EditToolTipText");
            RaisePropertyChanged("DDCCAcountNoCaption");

            _EventAggregator.GetEvent<NavigationChanged>().Publish(Shared.ReceiptNavigation.NavigatingToPath);
            _EventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Stop);

            RaisePropertyChanged("IsDDCCBatch");
            RaisePropertyChanged("IsCashChequeBatch");
            RaisePropertyChanged("IsAddMultipleVisible");
            RaisePropertyChanged("IsDishonourReversalBatch");
            RaisePropertyChanged("IsAssignReasonVisible");
            RaisePropertyChanged("LinkedReceiptIDCaption");
            RaisePropertyChanged("ReceiptDateCaption");
            RaisePropertyChanged("IsDishonourBatch");
            RaisePropertyChanged("IsEditVisible");
        }

        private void OnDishonourReversalBatchesSelected(ObservableCollection<object> selectedItems)
        {
            selectedDishonourReversalReceipts = selectedItems;
        }

        private void LoadReceiptDetails(int receiptBatchID)
        {
            switch (receiptBatchType)
            {
                case ReceiptBatchType.CashCheque:
                    Receipts = CashReceiptBatchFunctions.GetReceiptsSummary(receiptBatchID);
                    break;
                case ReceiptBatchType.DirectDebit:
                case ReceiptBatchType.CreditCard:
                case ReceiptBatchType.AutoReceipts:
                    Receipts = DDCCBatchFunctions.GetReceiptsSummary(receiptBatchType, receiptBatchID);
                    break;
                case ReceiptBatchType.Dishonour:
                case ReceiptBatchType.Reversals:
                    DishonourReversalReceipts = DishonourReversalFunctions.GetReceiptsSummary(receiptBatchType, receiptBatchID);
                    break;
            }

            if (receiptBatchType == ReceiptBatchType.Dishonour || receiptBatchType == ReceiptBatchType.Reversals)
            {
                dishonourReversalOptions.Clear();

                foreach (string option in Enum.GetNames(receiptBatchType == ReceiptBatchType.Dishonour ? typeof(DishonourReceiptOptions) : typeof(ReversalReceiptOptions)))
                {
                    DishonourReversalOptions.Add(Regex.Replace(option, "([a-z])([A-Z])", "$1 $2"));
                }

                ReasonCodes = DishonourReversalFunctions.GetReasonCodes(receiptBatchType);

                SelectedDishonourReversalOption = 0;
                ReasonSelected = ReasonCodes.Where(reason => reason.Description == "None").FirstOrDefault().ID;
                RaisePropertyChanged("SelectedDishonourReversalOption");
                RaisePropertyChanged("ReasonSelected");
            }
        }

        protected override void OnRecordLockChanged()
        {
            RaisePropertyChanged("IsEditVisible");
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {            
        }        
    }
}
