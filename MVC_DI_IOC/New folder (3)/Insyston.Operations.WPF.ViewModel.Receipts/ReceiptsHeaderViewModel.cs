using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Business.Receipts.Model;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.View.Receipts;
using Insyston.Operations.WPF.ViewModel.Common.Events;
using Insyston.Operations.WPF.ViewModel.Events;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Linq;
using view = Insyston.Operations.WPF.View;

namespace Insyston.Operations.WPF.ViewModel.Receipts
{
    public class ReceiptsHeaderViewModel : NotificationObject
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        
        public ReceiptsHeaderViewModel()
        {
            regionManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IRegionManager>();
            eventAggregator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAggregator.GetEvent<NavigationChanged>().Subscribe(OnNavigationChanged);            
        }

        public ReceiptNavigation ReceiptNavigation
        {
            get
            {
                return Shared.ReceiptNavigation;
            }
            set
            {
                if (Shared.ReceiptNavigation != value)
                {
                    Shared.ReceiptNavigation = value;
                    RaisePropertyChanged("ReceiptNavigation");
                }
            }
        }

        public NavigationItem CurrentItem
        {            
            get
            {
                return Shared.ReceiptNavigation.CurrentItem;
            }
            set
            {
                if (value != null)
                {
                    Shared.ReceiptNavigation.CurrentItem = value;

                    if (string.IsNullOrEmpty(Shared.ReceiptNavigation.NavigatingToPath))
                    {
                        NavigateTo();
                    }
                    else
                    {
                        RaisePropertyChanged("CurrentItem");
                    }
                }
            }
        }

        public string Path
        {
            get
            {
                return ReceiptNavigation.Path.ToString();
            }
            set
            {
                if (ReceiptNavigation.Path.ToString() != value)
                {
                    ReceiptNavigation.Path = value;
                }

                RaisePropertyChanged("Path");
            }
        }

        private void OnNavigationChanged(string value)
        {
            Path = value;
        }        

        private void NavigateTo()
        {
            UriQuery query = new UriQuery();
            string uri;

            eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Start);            

            if (Shared.ReceiptNavigation.CurrentItem.ReceiptText.ToLower() == "insyston operations launchpad" || (Shared.ReceiptNavigation.CurrentItem.ReceiptText.ToLower() == "receipts" && Shared.ReceiptNavigation.CurrentItem.Image == null))
            {
                eventAggregator.GetEvent<NavigateToLaunchpad>().Publish(true);
                return;
            }

            Shared.SetReceiptNavigation(CurrentItem);

            if (Shared.ReceiptNavigation.CurrentItem.ReceiptText.ToLower() == "receipts")
            {
                regionManager.RequestNavigate(Regions.ContentRegion, "/ReceiptsHome");
            }
            else
            {
                if (Shared.ReceiptNavigation.CurrentItem.ReceiptID > 0)
                {
                    query.Add("BatchType", Shared.ReceiptNavigation.CurrentItem.BatchTypeID.ToString());
                    query.Add("BatchID", Shared.ReceiptNavigation.CurrentItem.ReceiptID.ToString());
                    uri = "/ReceiptsSummary";
                }
                else if (string.IsNullOrEmpty(Shared.ReceiptNavigation.CurrentItem.BatchMonth) == false)
                {
                    query.Add("BatchMonth", Shared.ReceiptNavigation.CurrentItem.BatchMonth);
                    uri = "/ReceiptsBatchSummary";
                }
                else
                {
                    query.Add("Status", Shared.ReceiptNavigation.CurrentItem.BatchStatus.ToString());
                    uri = "/ReceiptsBatchSummary";
                }

                regionManager.RequestNavigate(Regions.ContentRegion, new Uri(uri + query.ToString(), UriKind.Relative));
            }            

            eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Stop);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }
    }    
}
