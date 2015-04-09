using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
using Insyston.Operations.Business.OpenItems;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Insyston.Operations.WPF.ViewModel.Events;
using Microsoft.Practices.Prism;
using Insyston.Operations.Model;
using model = Insyston.Operations.Model;
using Insyston.Operations.Business.Receipts.Model;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.WPF.View.Receipts;
using Insyston.Operations.Business.Common.Enums;

namespace Insyston.Operations.WPF.ViewModel.Receipts
{
    [Export(typeof(ReceiptsHomeViewModel))]
    public class ReceiptsHomeViewModel : OldViewModelBase, INavigationAware
    {
        private IEnumerable<ReceiptBatchStatusSummary> receiptBatchStatuses;
        private ReceiptBatchStatusSummary selectedBatchStatus;

        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public DelegateCommand Refresh { get; private set; }
        public DelegateCommand OpenReceiptBatch { get; private set; }

        public delegate void ClearGridEventHandler(object sender, bool isSummary);
        public event ClearGridEventHandler ClearGridFilters;

        [ImportingConstructor]
        public ReceiptsHomeViewModel(IEventAggregator evtAggregator, IRegionManager regManager)
        {
            eventAggregator = evtAggregator;
            regionManager = regManager;
                 
            Refresh = new DelegateCommand(this.RefreshBatchSummary);
            OpenReceiptBatch = new DelegateCommand(OpenReceiptBatchSummary);            
            Shared.SetReceiptNavigation(Shared.ReceiptNavigation.Receipts.Children.First().Children.First());
            OnNavigatedTo(null);            
        }

        public IEnumerable<ReceiptBatchStatusSummary> ReceiptBatchStatuses
        {
            get
            {
                return receiptBatchStatuses;
            }
            set
            {
                if (receiptBatchStatuses != value)
                {
                    receiptBatchStatuses = value;
                    RaisePropertyChanged("ReceiptBatchStatuses");
                }
            }
        }

        public ReceiptBatchStatusSummary SelectedBatchStatus
        {
            get
            {
                return selectedBatchStatus;
            }
            set
            {
                if (selectedBatchStatus != value)
                {
                    selectedBatchStatus = value;
                    RaisePropertyChanged("SelectedBatchStatus");
                }
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ReceiptBatchStatuses = BatchTypeFunctions.GetReceiptBatchStatusSummary();
            eventAggregator.GetEvent<NavigationChanged>().Publish(Shared.ReceiptNavigation.NavigatingToPath);            
        }
        
        private void OpenReceiptBatchSummary()
        {
            UriQuery query = new UriQuery();

            if (selectedBatchStatus != null)
            {
                eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Start);
                Shared.SetReceiptNavigation(new NavigationItem { BatchStatus = selectedBatchStatus.BatchStatusID, 
                        ReceiptText = selectedBatchStatus.BatchStatus });

                query.Add("Status", selectedBatchStatus.BatchStatus);
                regionManager.RequestNavigate(Regions.ContentRegion, new Uri("/ReceiptsBatchSummary" + query.ToString(), UriKind.Relative));           
                eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Stop);
            }
        }      

        private void RefreshBatchSummary()
        {
            eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Start);
            ReceiptBatchStatuses = BatchTypeFunctions.GetReceiptBatchStatusSummary();
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
