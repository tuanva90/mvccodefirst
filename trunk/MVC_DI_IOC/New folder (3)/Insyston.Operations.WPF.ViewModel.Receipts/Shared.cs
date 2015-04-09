using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Insyston.Operations.Model;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Insyston.Operations.WPF.ViewModel.Events;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Business.Receipts.Model;
using Insyston.Operations.Business.Receipts;

namespace Insyston.Operations.WPF.View.Receipts
{
    public static class Shared
    {
        private static NavigationItem _ReceiptLoaded;
        
        public static ReceiptNavigation ReceiptNavigation;        
        public static LXMSystemReceiptDefault CashChequeReceiptDefaults;               

        #region Receipt Navigation 

        public static void LoadReceiptNavigation()
        {
            NavigationItem currentItem = null;
            NavigationItem receipts;
            string path = string.Empty;

            if (ReceiptNavigation == null)
            {
                receipts = new NavigationItem { ReceiptText = "Receipts", Image = "Images/Receipts.ico" };
                
                ReceiptNavigation = new ReceiptNavigation();

                ReceiptNavigation.Receipts = new NavigationItem { ReceiptText = "Insyston Operations Launchpad" };
                ReceiptNavigation.Receipts.Children = new ObservableCollection<NavigationItem>();
                ReceiptNavigation.Receipts.Children.Add(new NavigationItem { ReceiptText = "Receipts" });
                ReceiptNavigation.Receipts.Children.First().Children = new ObservableCollection<NavigationItem>();
                ReceiptNavigation.Receipts.Children.First().Children.Add(receipts);

                receipts.Children = new ObservableCollection<NavigationItem>();
                receipts.Children.Add(new NavigationItem { ReceiptText = ReceiptBatchStatus.Created.ToString(), BatchStatus = (int)ReceiptBatchStatus.Created, Image = "Images/Loading.ico" });
                _ReceiptLoaded = receipts.Children.FirstOrDefault();
                receipts.Children.Add(new NavigationItem { ReceiptText = ReceiptBatchStatus.Pending.ToString(), BatchStatus = (int)ReceiptBatchStatus.Pending, Image = "Images/Pending.ico" });
                receipts.Children.Add(new NavigationItem { ReceiptText = ReceiptBatchStatus.Posted.ToString(), BatchStatus = (int)ReceiptBatchStatus.Posted, Image = "Images/Posted.ico" });                
            }
            else
            {
                receipts = ReceiptNavigation.Receipts.Children.First().Children.First();
            }

            BatchTypeFunctions.GetReceiptNavigationList(receipts);
        }

        public static void AddMissingPostedMonthReceipts(string batchMonth)
        {
            NavigationItem postedMonth;

            postedMonth = ReceiptNavigation.Receipts.Children.First().Children.First().Children.Where(item => item.BatchStatus == (int)ReceiptBatchStatus.Posted).FirstOrDefault().Children.Where(month => month.BatchMonth == batchMonth).FirstOrDefault();
            
            if (postedMonth != null && (postedMonth.Children == null || postedMonth.Children.Count == 0))
            {
                postedMonth.Children = new ObservableCollection<NavigationItem>(BatchTypeFunctions.GetReceiptPostedList(batchMonth));
            }            
        }

        public static bool MoveNavigationItem(NavigationItem navItem, int batchStatus, bool isSummary = false)
        {
            NavigationItem parentItem, destinationItem, navigationItem;
            bool result = false;
            IRegionManager regionManager;

            try
            {
                destinationItem = ReceiptNavigation.Receipts.Children.First().Children.First().Children.Where(item => item.BatchStatus == batchStatus).FirstOrDefault();

                if (navItem.BatchStatus == (int)ReceiptBatchStatus.Posted)
                {
                    parentItem = ReceiptNavigation.Receipts.Children.First().Children.First().Children.Where(item => item.BatchStatus == navItem.BatchStatus).FirstOrDefault().Children.Where(item => item.BatchMonth == navItem.BatchMonth).FirstOrDefault();
                }
                else
                {
                    parentItem = ReceiptNavigation.Receipts.Children.First().Children.First().Children.Where(item => item.BatchStatus == navItem.BatchStatus).FirstOrDefault();

                    if (batchStatus == (int)ReceiptBatchStatus.Posted)
                    {
                        if (destinationItem.Children.Where(item => item.BatchMonth == navItem.BatchMonth).Count() == 0)
                        {
                            destinationItem.Children.Insert(GetInsertMonthIndex(destinationItem.Children, navItem.BatchMonth),
                                    new NavigationItem { BatchMonth = navItem.BatchMonth, BatchStatus = (int)ReceiptBatchStatus.Posted, ReceiptText = navItem.BatchMonth, Image = "../Images/Month.ico" });

                            destinationItem = destinationItem.Children.Where(item => item.BatchMonth == navItem.BatchMonth).FirstOrDefault();
                            destinationItem.Children = new ObservableCollection<NavigationItem>();
                        }
                        else
                        {
                            destinationItem = destinationItem.Children.Where(item => item.BatchMonth == navItem.BatchMonth).FirstOrDefault();
                        }
                    }
                }

                if (isSummary && ReceiptNavigation.CurrentItem != null && ReceiptNavigation.CurrentItem.ReceiptID != 0)
                {
                    ReceiptNavigation.CurrentItem = null;
                }

                if (parentItem != null && parentItem.Children != null)
                {
                    navigationItem = parentItem.Children.Where(item => item.ReceiptID == navItem.ReceiptID).FirstOrDefault();
                    parentItem.Children.Remove(navigationItem);

                    if (navItem.BatchStatus == (int)ReceiptBatchStatus.Posted && parentItem.Children.Count == 0)
                    {
                        ReceiptNavigation.Receipts.Children.First().Children.First().Children.Where(item => item.BatchStatus == (int)ReceiptBatchStatus.Posted).FirstOrDefault().Children.Remove(parentItem);
                    }

                    if (parentItem.Children.Count == 0)
                    {
                        result = true;
                    }

                    if (navigationItem != null && destinationItem.Children != null)
                    {
                        navigationItem.BatchStatus = batchStatus;
                        navigationItem.BatchMonth = destinationItem.BatchMonth;
                        destinationItem.Children.Insert(GetInsertIndex(destinationItem.Children, navigationItem.ReceiptID), navigationItem);                        
                    }
                }

                return result;
            }
            catch (Exception ex)
            {                
                LoadReceiptNavigation();
                ReceiptNavigation.CurrentItem = ReceiptNavigation.Receipts;
                regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
                regionManager.RequestNavigate(Regions.ContentRegion, new Uri("/ReceiptsHome", UriKind.Relative));
                return false;
            }
        }

        public static void SetReceiptNavigation(NavigationItem navItem, bool checkAndLoadPostedList = false)
        {
            NavigationItem postedMonth;
            ReceiptNavigation.CurrentItem = navItem;

            if (navItem.ReceiptID > 0)
            {
                if (navItem.BatchStatus == (int)ReceiptBatchStatus.Posted)
                {
                    ReceiptNavigation.NavigatingToPath = string.Format(@"Receipts\Receipts\{0}\{1}\{2}", Enum.Parse(typeof(ReceiptBatchStatus), navItem.BatchStatus.ToString()), navItem.BatchMonth, navItem.ReceiptText);

                    if (checkAndLoadPostedList)
                    {
                        postedMonth = ReceiptNavigation.Receipts.Children.First().Children.First().Children.Where(item => item.BatchStatus == (int)ReceiptBatchStatus.Posted).FirstOrDefault().Children.Where(month => month.BatchMonth == navItem.BatchMonth).FirstOrDefault();

                        if (postedMonth == null)
                        {
                            ReceiptNavigation.Receipts.Children.First().Children.First().Children.Where(item => item.BatchStatus == (int)ReceiptBatchStatus.Posted).FirstOrDefault().Children.Insert(
                                GetInsertMonthIndex(ReceiptNavigation.Receipts.Children.First().Children.First().Children.Where(item => item.BatchStatus == (int)ReceiptBatchStatus.Posted).FirstOrDefault().Children, navItem.BatchMonth),
                                new NavigationItem { BatchMonth = navItem.BatchMonth, BatchStatus = (int)ReceiptBatchStatus.Posted, ReceiptText = navItem.BatchMonth, Image = "../Images/Month.ico" });
                        }

                        if (postedMonth.Children == null || postedMonth.Children.Count == 0)
                        {
                            postedMonth.Children = new ObservableCollection<NavigationItem>(BatchTypeFunctions.GetReceiptPostedList(navItem.BatchMonth));
                        }
                    }
                }
                else
                {
                    ReceiptNavigation.NavigatingToPath = string.Format(@"Receipts\Receipts\{0}\{1}", Enum.Parse(typeof(ReceiptBatchStatus), navItem.BatchStatus.ToString()), navItem.ReceiptText);
                }
            }
            else if (navItem.ReceiptText.ToLower() != "insyston operations launchpad")
            {
                if (navItem.ReceiptText == navItem.BatchMonth)
                {
                    ReceiptNavigation.NavigatingToPath = string.Format(@"Receipts\Receipts\{0}\{1}", Enum.Parse(typeof(ReceiptBatchStatus), navItem.BatchStatus.ToString()), navItem.ReceiptText);                   
                }                
                else if(navItem.ReceiptText == "Receipts")
                {
                    ReceiptNavigation.NavigatingToPath = @"Receipts\Receipts";
                }
                else 
                {
                    ReceiptNavigation.NavigatingToPath = @"Receipts\Receipts\" + navItem.ReceiptText;
                }
            }            
        }
     
        public static void AddLoadedItem(NavigationItem navItem)
        {
            IRegionManager regionManager;
            UriQuery query = new UriQuery();

            if (_ReceiptLoaded.Children == null)
            {
                _ReceiptLoaded.Children = new ObservableCollection<NavigationItem>();
            }

            _ReceiptLoaded.Children.Insert(0, navItem);

            Shared.SetReceiptNavigation(navItem);


            regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            query.Add("BatchType", navItem.BatchTypeID.ToString());
            query.Add("BatchID", navItem.ReceiptID.ToString());
            regionManager.RequestNavigate(Regions.ContentRegion, new Uri("/ReceiptsSummary" + query.ToString(), UriKind.Relative));
            Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<NavigationChanged>().Publish(Shared.ReceiptNavigation.NavigatingToPath);
        }

        #endregion           

        #region Private Functions

        private static int GetInsertIndex(ObservableCollection<NavigationItem> navItems, int receiptID)
        {
            for (int index = 0; index < navItems.Count; index++)
            {
                if (navItems[index].ReceiptID < receiptID)
                {
                    return index;
                }
            }

            return navItems.Count;
        }

        private static int GetInsertMonthIndex(ObservableCollection<NavigationItem> navItems, string batchMonth)
        {
            for (int index = 0; index < navItems.Count; index++)
            {
                if (Convert.ToDateTime("01 " + navItems[index].BatchMonth) < Convert.ToDateTime("01 " + batchMonth))
                {
                    return index;
                }
            }

            return navItems.Count;
        }

        #endregion
    }
}
