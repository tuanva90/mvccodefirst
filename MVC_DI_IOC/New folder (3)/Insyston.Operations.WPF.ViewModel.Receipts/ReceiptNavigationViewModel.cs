using System;
using System.Linq;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using System.Collections.Generic;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Insyston.Operations.WPF.ViewModel.Events;
using System.Collections.ObjectModel;
using Insyston.Operations.Model;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.WPF.View.Receipts;
using Insyston.Operations.Business.Common.Enums;
using System.Windows.Threading;

namespace Insyston.Operations.WPF.ViewModel.Receipts
{
    [Export(typeof(ReceiptNavigationViewModel))]
    public class ReceiptNavigationViewModel : NotificationObject
    {                
        public DelegateCommand<NavigationItem> Navigate { get; private set; }
        public DelegateCommand<NavigationItem> NavigateSummary { get; private set; }
        public DelegateCommand Refresh { get; private set; }
        public DelegateCommand<string> OpenPopup { get; private set; }        

        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public ObservableCollection<NavigationItem> ReceiptNavigation
        {
            get
            {
                return new ObservableCollection<NavigationItem> { Shared.ReceiptNavigation.Receipts };
            }
            set
            {
                RaisePropertyChanged("ReceiptsNavigation");
            }
        }

        [ImportingConstructor]
        public ReceiptNavigationViewModel(IRegionManager regManager, IEventAggregator evtAggregator)
        {
            regionManager = regManager;
            eventAggregator = evtAggregator;                    

            Navigate = new DelegateCommand<NavigationItem>(this.NavigateToUri);
            NavigateSummary = new DelegateCommand<NavigationItem>(this.NavigateToSummary);            
            Refresh = new DelegateCommand(this.RefreshNavigation);            
            eventAggregator.GetEvent<ReceiptBatchAdded>().Subscribe(AddReceiptBatch);            
        }

        public List<NavigationItem> GetReceiptPostedList(string monthYear)
        {            
            return BatchTypeFunctions.GetReceiptPostedList(monthYear);
        }      

        public void RefreshNavigation()
        {
            eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Start);
            Shared.LoadReceiptNavigation();
            regionManager.RequestNavigate(Regions.ContentRegion, "/ReceiptsHome");
            eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Stop);
        }
        
        public void NavigateToUri(NavigationItem item)
        {
            UriQuery query = new UriQuery();
            eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Start);

            Shared.SetReceiptNavigation(item);
            if (item.ReceiptID > 0)
            {
                query.Add("BatchType", item.BatchTypeID.ToString());
                query.Add("BatchID", item.ReceiptID.ToString());
                regionManager.RequestNavigate(Regions.ContentRegion, new Uri("/ReceiptsSummary" + query.ToString(), UriKind.Relative));
                
            }
            else
            {
                query.Add("BatchMonth", item.ReceiptText);                
                regionManager.RequestNavigate(Regions.ContentRegion, new Uri("/ReceiptsBatchSummary" + query.ToString(), UriKind.Relative));            
            }
            
            eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Stop);
        }
    
        public void NavigateToSummary(NavigationItem navItem)
        {
            eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Start);
            
            UriQuery query = new UriQuery();
            
            if(navItem.BatchStatus == 0)
            {
                Dispatcher.CurrentDispatcher.Invoke(new Action(() => { regionManager.RequestNavigate(Regions.ContentRegion, "/ReceiptsHome"); }));
            }
            else
            {
                query.Add("Status", navItem.BatchStatus.ToString());
                Shared.SetReceiptNavigation(navItem); 
                Dispatcher.CurrentDispatcher.Invoke(new Action(() => { regionManager.RequestNavigate(Regions.ContentRegion, new Uri("/ReceiptsBatchSummary" + query.ToString(), UriKind.Relative)); }));                              
            }

            eventAggregator.GetEvent<ProgressChanged>().Publish(ProgressStatus.Stop);         
        }

        private void AddReceiptBatch(NavigationItem newItem)
        {
            Shared.AddLoadedItem(newItem);
            NavigateToUri(newItem);
        }
    }    
}
