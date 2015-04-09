using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Insyston.Operations.WPF.ViewModel.Events;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Insyston.Operations.Logging;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.Business.Receipts.Model;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Security;
using Insyston.Operations.WPF.View.Receipts;
using System.Threading;
using Insyston.Operations.Common;
using Insyston.Operations.WPF.ViewModel.Receipts.DDCC.ViewModel;

namespace Insyston.Operations.WPF.ViewModel.Receipts
{
    [Export(typeof(ReceiptsBatchSummaryViewModel))]
    public class ReceiptsBatchSummaryViewModel : OldViewModelBase, INavigationAware
    {
        public InteractionRequest<PopupWindow> Popup { get; private set; }
        private List<ReceiptBatchSummary> receiptBatchSummaries;
        private List<ReceiptBatchMonthSummary> receiptBatchMonthSummaries;
        private ReceiptBatchSummary selectedReceiptBatch;
        private ReceiptBatchMonthSummary selectReceiptMonth;

        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public DelegateCommand Back { get; private set; }
        public DelegateCommand Refresh { get; private set; }
        public DelegateCommand OpenReceiptBatch { get; private set; }
        public DelegateCommand OpenReceiptMonth { get; private set; }
        public DelegateCommand StatusForward { get; private set; }
        public DelegateCommand StatusBackward { get; private set; }

        private ReceiptBatchStatus batchStatus;
        private bool isSummaryVisisble;
        private string batchMonth;
        private string lastClicked;

        public delegate void ClearGridEventHandler(object sender, bool isSummary);
        public event ClearGridEventHandler ClearGridFilters;

        [ImportingConstructor]
        public ReceiptsBatchSummaryViewModel(IEventAggregator evtAggregator, IRegionManager regManager)
        {
            IsSummaryVisible = false;
            eventAggregator = evtAggregator;
            regionManager = regManager;

            Popup = new InteractionRequest<PopupWindow>();
            Back = new DelegateCommand(GoBack);
            Refresh = new DelegateCommand(this.RefreshBatchSummary);
            OpenReceiptBatch = new DelegateCommand(OpenReceiptBatchSummary);
            OpenReceiptMonth = new DelegateCommand(OpenReceiptMonthSummary);
            StatusForward = new DelegateCommand(MoveStatusForward);
            StatusBackward = new DelegateCommand(RevertStatusBackward);

            LockTableName = "ReceiptBatch";
        }

        public List<ReceiptBatchSummary> ReceiptBatchSummaries
        {
            get
            {
                return receiptBatchSummaries;
            }
            set
            {
                if (receiptBatchSummaries != value)
                {
                    receiptBatchSummaries = value;
                    IsSummaryVisible = true;
                    RaisePropertyChanged("ReceiptBatchSummaries");
                }
            }
        }

        public List<ReceiptBatchMonthSummary> ReceiptBatchMonthSummaries
        {
            get
            {
                return receiptBatchMonthSummaries;
            }
            set
            {
                if (receiptBatchMonthSummaries != value)
                {
                    receiptBatchMonthSummaries = value;
                    IsSummaryVisible = false;
                    RaisePropertyChanged("ReceiptBatchMonthSummaries");                    
                }
            }
        }

        public ReceiptBatchSummary SelectedReceiptBatch
        {
            get
            {
                return selectedReceiptBatch;
            }
            set
            {
                if (selectedReceiptBatch != value)
                {
                    selectedReceiptBatch = value;
                    RaisePropertyChanged("ReceiptBatchSummary");
                    RaisePropertyChanged("IsStatusForwardAllowed");
                    RaisePropertyChanged("IsStatusBackwardAllowed");
                }
            }
        }

        public ReceiptBatchMonthSummary SelectedReceiptMonth
        {
            get
            {
                return selectReceiptMonth;
            }
            set
            {
                if (selectReceiptMonth != value)
                {
                    selectReceiptMonth = value;
                    RaisePropertyChanged("SelectedReceiptMonth");
                }
            }
        }

        public bool IsSummaryVisible
        {
            get
            {
                return isSummaryVisisble;
            }
            set
            {
                if (isSummaryVisisble != value)
                {
                    isSummaryVisisble = value;
                    RaisePropertyChanged("IsSummaryVisible");
                    RaisePropertyChanged("IsMonthSummaryVisible");
                    RaisePropertyChanged("IsPostedSummary");
                }
            }
        }

        public bool IsMonthSummaryVisible
        {
            get
            {
                return !IsSummaryVisible;
            }            
        }

        public bool IsPostedSummary
        {
            get
            {
                return batchStatus == ReceiptBatchStatus.Posted && isSummaryVisisble;                
            }
        }

        public bool IsStatusForwardVisible
        {
            get
            {
                return IsSummaryVisible && string.IsNullOrEmpty(StatusForwardText) == false;
            }
            set
            {
            }
        }

        public bool IsStatusBackwardVisible
        {
            get
            {
                return IsSummaryVisible && string.IsNullOrEmpty(StatusBackwardText) == false;
            }
            set
            {
            }
        }

        public bool IsStatusForwardAllowed
        {
            get
            {
                return selectedReceiptBatch != null && selectedReceiptBatch.NumberOfEntries > 0;
            }
            set
            {
            }
        }

        public bool IsStatusBackwardAllowed
        {
            get
            {
                return selectedReceiptBatch != null;
            }
            set
            {
            }
        }

        public ReceiptBatchStatus BatchStatus
        {
            get
            {
                return batchStatus;
            }
            set
            {
                if (batchStatus != value)
                {
                    batchStatus = value;
                    RaisePropertyChanged("BatchStatus");
                }
            }
        }

        public string StatusForwardText
        {
            get
            {                
                if (batchStatus == ReceiptBatchStatus.Created)
                {
                    return "Move Status to " + ReceiptBatchStatus.Pending.ToString();
                }
                else if (batchStatus == ReceiptBatchStatus.Pending)
                {
                    return "Move Status to " + ReceiptBatchStatus.Posted.ToString();
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
                if (batchStatus == ReceiptBatchStatus.Pending)
                {
                    return "Revert Status to " + ReceiptBatchStatus.Created.ToString();
                }

                return string.Empty;
            }
            set
            {
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters["Status"] != null)
            {
                lastClicked = "Status";
                batchStatus = (ReceiptBatchStatus)Enum.Parse(typeof(ReceiptBatchStatus), navigationContext.Parameters["Status"].ToString());

                if (batchStatus == ReceiptBatchStatus.Posted)
                {
                    ReceiptBatchMonthSummaries = BatchTypeFunctions.GetReceiptBatchMonthSummary();
                }
                else
                {
                    ReceiptBatchSummaries = BatchTypeFunctions.GetReceiptBatchSummary(batchStatus);
                }
            }
            else if (navigationContext.Parameters["BatchMonth"] != null)
            {
                batchStatus = ReceiptBatchStatus.Posted;
                ReceiptBatchSummaries = BatchTypeFunctions.GetReceiptBatchSummary(navigationContext.Parameters["BatchMonth"].ToString());
                batchMonth = navigationContext.Parameters["BatchMonth"];
                Shared.AddMissingPostedMonthReceipts(batchMonth);
                lastClicked = "BatchMonth";
            }

            eventAggregator.GetEvent<NavigationChanged>().Publish(Shared.ReceiptNavigation.NavigatingToPath);
            RaisePropertyChanged("IsSummaryVisible");
            RaisePropertyChanged("IsMonthSummaryVisible");
            RaisePropertyChanged("IsPostedSummary");
            RaisePropertyChanged("StatusForwardText");
            RaisePropertyChanged("StatusBackwardText");
            RaisePropertyChanged("IsStatusForwardVisible");
            RaisePropertyChanged("IsStatusBackwardVisible");
            RaisePropertyChanged("IsStatusForwardAllowed");
            RaisePropertyChanged("IsStatusBackwardAllowed");
        }

        private void GoBack()
        {
            if (lastClicked == "Status")
            {
                Shared.SetReceiptNavigation(new NavigationItem { ReceiptText = Shared.ReceiptNavigation.Receipts.Children.First().ReceiptText });
                regionManager.RequestNavigate(Regions.ContentRegion, new Uri("/ReceiptsHome", UriKind.Relative));
                eventAggregator.GetEvent<NavigationChanged>().Publish(Shared.ReceiptNavigation.NavigatingToPath);
            }
            else
            {
                lastClicked = "Status";
                Shared.SetReceiptNavigation(new NavigationItem { BatchStatus = (int)batchStatus, ReceiptText = batchStatus.ToString() });
                ReceiptBatchMonthSummaries = BatchTypeFunctions.GetReceiptBatchMonthSummary();
                eventAggregator.GetEvent<NavigationChanged>().Publish(Shared.ReceiptNavigation.NavigatingToPath);
            }

            RaisePropertyChanged("IsStatusForwardVisible");
            RaisePropertyChanged("IsStatusBackwardVisible");
        }

        private void OpenReceiptBatchSummary()
        {
            UriQuery query = new UriQuery();

            if (selectedReceiptBatch != null)
            {
                eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Start);
                Shared.SetReceiptNavigation(new NavigationItem { BatchStatus = (int)batchStatus, ReceiptID = selectedReceiptBatch.BatchID, BatchMonth = selectedReceiptBatch.BatchMonth,
                        ReceiptText = selectedReceiptBatch.BatchID + "-" + selectedReceiptBatch.BatchType }, batchStatus == ReceiptBatchStatus.Posted);
           
                query.Add("BatchType", selectedReceiptBatch.BatchTypeID.ToString());
                query.Add("BatchID", selectedReceiptBatch.BatchID.ToString());
                regionManager.RequestNavigate(Regions.ContentRegion, new Uri("/ReceiptsSummary" + query.ToString(), UriKind.Relative));           
                eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Stop);
            }
        }

        private void OpenReceiptMonthSummary()
        {
            UriQuery query = new UriQuery();

            if (selectReceiptMonth != null)
            {
                eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Start);
                Shared.SetReceiptNavigation(new NavigationItem { BatchStatus = (int)batchStatus, BatchMonth = selectReceiptMonth.BatchMonth, ReceiptText = selectReceiptMonth.BatchMonth });
                
                ReceiptBatchSummaries = BatchTypeFunctions.GetReceiptBatchSummary(selectReceiptMonth.BatchMonth);
                batchMonth = selectReceiptMonth.BatchMonth;
                lastClicked = "BatchMonth";                
                eventAggregator.GetEvent<NavigationChanged>().Publish(Shared.ReceiptNavigation.NavigatingToPath);                
                eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Stop);
                Shared.AddMissingPostedMonthReceipts(selectReceiptMonth.BatchMonth);

                RaisePropertyChanged("StatusForwardText");
                RaisePropertyChanged("StatusBackwardText");
                RaisePropertyChanged("IsStatusForwardVisible");
                RaisePropertyChanged("IsStatusBackwardVisible");
                RaisePropertyChanged("IsStatusForwardAllowed");
                RaisePropertyChanged("IsStatusBackwardAllowed");
            }
        }

        private void MoveStatusForward()
        {
            PopupWindow popupWindow;
            int newStatus = (int)batchStatus;

            try
            {
                if (selectedReceiptBatch.BatchTypeID != (int)ReceiptBatchType.DirectDebit && selectedReceiptBatch.BatchTypeID != (int)ReceiptBatchType.CreditCard)
                {
                    LockUniqueIdentifier = selectedReceiptBatch.BatchID.ToString();
                    Lock();
                }

                if (IsLocked || selectedReceiptBatch.BatchTypeID == (int)ReceiptBatchType.DirectDebit || selectedReceiptBatch.BatchTypeID == (int)ReceiptBatchType.CreditCard)
                {
                    if (batchStatus == ReceiptBatchStatus.Created)
                    {
                        if (selectedReceiptBatch.BatchTypeID == (int)ReceiptBatchType.DirectDebit)
                        {
                            IsBusy = true;
                            popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "DDCC.DDCCStatusChange", true);
                            popupWindow.Parameters.Add(ReceiptBatchType.DirectDebit);
                            Popup.Raise(popupWindow);
                            ReceiptBatchSummaries = BatchTypeFunctions.GetReceiptBatchSummary(batchStatus);
                            IsBusy = false;
                            return;
                        }
                        else if (selectedReceiptBatch.BatchTypeID == (int)ReceiptBatchType.CreditCard)
                        {
                            IsBusy = true;
                            popupWindow = new PopupWindow(ViewAssemblies.ReceiptsViewAssembly, "DDCC.DDCCStatusChange", true);
                            popupWindow.Parameters.Add(ReceiptBatchType.CreditCard);
                            Popup.Raise(popupWindow);
                            ReceiptBatchSummaries = BatchTypeFunctions.GetReceiptBatchSummary(batchStatus);
                            IsBusy = false;
                            return;
                        }
                        else
                        {
                            if (selectedReceiptBatch.BatchTypeID == (int)ReceiptBatchType.Dishonour || selectedReceiptBatch.BatchTypeID == (int)ReceiptBatchType.Reversals)
                            {
                                if (BatchTypeFunctions.GetReceiptBatchSystemDefaults(selectedReceiptBatch.BatchTypeID).EnforceReasonCode.GetValueOrDefault()
                                    && DishonourReversalFunctions.IsReceiptExistsWithoutReasonCode(selectedReceiptBatch.BatchTypeID, selectedReceiptBatch.BatchID))
                                {
                                    ShowMessage(selectedReceiptBatch.BatchType + " Reason must be Entered for all Receipts before moving the Batch to Pending", selectedReceiptBatch.BatchType + " Reason - Validation");
                                    return;
                                }
                            }

                            newStatus = (int)ReceiptBatchStatus.Pending;
                            ReceiptBatchFunctions.UpdateStatus(selectedReceiptBatch.BatchID, newStatus, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId);

                            if (Shared.MoveNavigationItem(new NavigationItem { BatchStatus = (int)batchStatus, BatchMonth = selectedReceiptBatch.BatchMonth, ReceiptID = selectedReceiptBatch.BatchID }, newStatus, true))
                            {
                                GoBack();
                                UnLock();
                                return;
                            }

                            ReceiptBatchSummaries = BatchTypeFunctions.GetReceiptBatchSummary(batchStatus);
                        }
                    }
                    else if (batchStatus == ReceiptBatchStatus.Pending)
                    {
                        newStatus = (int)ReceiptBatchStatus.Posted;

                        ReceiptBatchFunctions.UpdateStatus(selectedReceiptBatch.BatchID, newStatus, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId);

                        if (Shared.MoveNavigationItem(new NavigationItem { BatchStatus = (int)batchStatus, BatchMonth = selectedReceiptBatch.BatchMonth, ReceiptID = selectedReceiptBatch.BatchID }, newStatus, true))
                        {
                            GoBack();
                            UnLock();
                            return;
                        }

                        ReceiptBatchSummaries = BatchTypeFunctions.GetReceiptBatchSummary(batchStatus);
                    }

                    UnLock();
                }
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

        private void RevertStatusBackward()
        {            
            ReceiptBatchStatus newStatus = batchStatus;

            try
            {
                LockUniqueIdentifier = selectedReceiptBatch.BatchID.ToString();
                Lock();

                if (IsLocked)
                {
                    if (batchStatus == ReceiptBatchStatus.Posted)
                    {
                        newStatus = ReceiptBatchStatus.Pending;
                    }
                    else if (batchStatus == ReceiptBatchStatus.Pending)
                    {
                        newStatus = ReceiptBatchStatus.Created;
                    }

                    ReceiptBatchFunctions.UpdateStatus(selectedReceiptBatch.BatchID, (int)newStatus, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId);
                    if (Shared.MoveNavigationItem(new NavigationItem { BatchStatus = (int)batchStatus, BatchMonth = selectedReceiptBatch.BatchMonth, ReceiptID = selectedReceiptBatch.BatchID }, (int)newStatus, true))
                    {
                        GoBack();
                        UnLock();
                        return;
                    }

                    if (batchStatus == ReceiptBatchStatus.Posted)
                    {
                        ReceiptBatchSummaries = BatchTypeFunctions.GetReceiptBatchSummary(selectedReceiptBatch.BatchMonth);
                    }
                    else
                    {
                        ReceiptBatchSummaries = BatchTypeFunctions.GetReceiptBatchSummary(batchStatus);
                    }

                    UnLock();
                }
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

        private void RefreshBatchSummary()
        {
            eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Start);

            if (IsSummaryVisible)
            {
                if (lastClicked == "Status")
                {
                    ReceiptBatchSummaries = BatchTypeFunctions.GetReceiptBatchSummary(batchStatus);
                }
                else
                {
                    ReceiptBatchSummaries = BatchTypeFunctions.GetReceiptBatchSummary(batchMonth);
                }

                ClearGridFilters(this, true);
            }
            else
            {
                ReceiptBatchMonthSummaries = BatchTypeFunctions.GetReceiptBatchMonthSummary();
                ClearGridFilters(this, false);
            }

            Shared.LoadReceiptNavigation();
            eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Stop);            
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
