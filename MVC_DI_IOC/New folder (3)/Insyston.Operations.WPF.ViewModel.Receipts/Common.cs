using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Insyston.Operations.Model;
using System.Collections.ObjectModel;
using Insyston.Operations.Business.OpenItems;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Insyston.Operations.View.OpenItems.Events;
using System.Reflection;

namespace Insyston.Operations.View.OpenItems.Receipts
{
    public static class Common
    {
        public const int PrideOpsSystemID = 100;
        public const string PrideToken = "AbcvuOIA5o+FQhQbF8IMLMak9NleHGDgawb5V5lseBNhCH2FCm6/i8lCaU9aCGB6KQgAhrPTeo4=";

        private static NavigationItem receiptLoaded;

        public static ReceiptNavigation ReceiptNavigation;        
        public static string UserName;
        public static string PrideBaseUri;
        public static int PrideSystemID;
        public static int CreditAssessmentPeriodInsertedIndex;

        public static LXMSystemReceiptDefault CashChequeReceiptDefaults;        

        #region User ID 

        public static int UserID
        {
            get
            {
                if (Application.Current.Properties["UserID"] == null)
                {
                    return 0;
                }

                return (int)Application.Current.Properties["UserID"];
            }
            set
            {
                Application.Current.Properties["UserID"] = value;
                UserName = SecurityFunctions.GetUserName(value);
            }
        }

        #endregion

        #region Receipt Navigation 

        public static void LoadReceiptNavigation()
        {
            NavigationItem currentItem = null;
            string path;

            if (ReceiptNavigation != null)
            {
                currentItem = ReceiptNavigation.CurrentItem;
                path = ReceiptNavigation.Path;
            }
            else
            {
                path = string.Empty;
            }
            ReceiptNavigation = new ReceiptNavigation();
            ReceiptNavigation.Receipts = new NavigationItem { ReceiptText = "Receipts", Image = "../../../Images/OpenItems/Receipts.ico" };

            ReceiptNavigation.Receipts.Children = new ObservableCollection<NavigationItem>();
            ReceiptNavigation.Receipts.Children.Add(new NavigationItem { ReceiptText = ReceiptBatchStatus.Created.ToString(), BatchStatus = (int)ReceiptBatchStatus.Created, Image = "../../../Images/OpenItems/Loading.ico" });
            receiptLoaded = ReceiptNavigation.Receipts.Children.FirstOrDefault();

            ReceiptNavigation.Receipts.Children.Add(new NavigationItem { ReceiptText = ReceiptBatchStatus.Pending.ToString(), BatchStatus = (int)ReceiptBatchStatus.Pending, Image = "../../../Images/OpenItems/Pending.ico" });
            ReceiptNavigation.Receipts.Children.Add(new NavigationItem { ReceiptText = ReceiptBatchStatus.Posted.ToString(), BatchStatus = (int)ReceiptBatchStatus.Posted, Image = "../../../Images/OpenItems/Posted.ico" });

            BatchTypeFunctions.GetReceiptNavigationList(ReceiptNavigation.Receipts);

            if (currentItem != null)
            {
                ReceiptNavigation.CurrentItem = currentItem;
                ReceiptNavigation.Path = path;
            }
        }

        public static void AddMissingPostedMonthReceipts(string batchMonth)
        {
            NavigationItem postedMonth;

            postedMonth = ReceiptNavigation.Receipts.Children.Where(item => item.BatchStatus == (int)ReceiptBatchStatus.Posted).FirstOrDefault().Children.Where(month => month.BatchMonth == batchMonth).FirstOrDefault();
            
            if (postedMonth != null && (postedMonth.Children == null || postedMonth.Children.Count == 0))
            {
                postedMonth.Children = new ObservableCollection<NavigationItem>(BatchTypeFunctions.GetReceiptPostedList(batchMonth));
            }

            //if (postedMonth != null)
            //{
            //    //ReceiptNavigation.CurrentItem.Children = postedMonth.Children;
            //}
        }

        public static bool MoveNavigationItem(NavigationItem navItem, int batchStatus, bool isSummary = false)
        {
            NavigationItem parentItem, destinationItem, navigationItem;
            bool result = false;
            IRegionManager regionManager;

            try
            {
                destinationItem = ReceiptNavigation.Receipts.Children.Where(item => item.BatchStatus == batchStatus).FirstOrDefault();

                if (navItem.BatchStatus == (int)ReceiptBatchStatus.Posted)
                {
                    parentItem = ReceiptNavigation.Receipts.Children.Where(item => item.BatchStatus == navItem.BatchStatus).FirstOrDefault().Children.Where(item => item.BatchMonth == navItem.BatchMonth).FirstOrDefault();
                }
                else
                {
                    parentItem = ReceiptNavigation.Receipts.Children.Where(item => item.BatchStatus == navItem.BatchStatus).FirstOrDefault();

                    if (batchStatus == (int)ReceiptBatchStatus.Posted)
                    {
                        if (destinationItem.Children.Where(item => item.BatchMonth == navItem.BatchMonth).Count() == 0)
                        {
                            destinationItem.Children.Insert(GetInsertMonthIndex(destinationItem.Children, navItem.BatchMonth),
                                    new NavigationItem { BatchMonth = navItem.BatchMonth, BatchStatus = (int)ReceiptBatchStatus.Posted, ReceiptText = navItem.BatchMonth, Image = "../../../Images/OpenItems/Month.ico" });

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

                if (parentItem.Children != null)
                {
                    navigationItem = parentItem.Children.Where(item => item.ReceiptID == navItem.ReceiptID).FirstOrDefault();
                    parentItem.Children.Remove(navigationItem);

                    if (navItem.BatchStatus == (int)ReceiptBatchStatus.Posted && parentItem.Children.Count == 0)
                    {
                        ReceiptNavigation.Receipts.Children.Where(item => item.BatchStatus == (int)ReceiptBatchStatus.Posted).FirstOrDefault().Children.Remove(parentItem);
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
                    ReceiptNavigation.NavigatingToPath = string.Format(@"{0}\{1}\{2}", Enum.Parse(typeof(ReceiptBatchStatus), navItem.BatchStatus.ToString()), navItem.BatchMonth, navItem.ReceiptText);

                    if (checkAndLoadPostedList)
                    {
                        postedMonth = ReceiptNavigation.Receipts.Children.Where(item => item.BatchStatus == (int)ReceiptBatchStatus.Posted).FirstOrDefault().Children.Where(month => month.BatchMonth == navItem.BatchMonth).FirstOrDefault();

                        if (postedMonth == null)
                        {
                            ReceiptNavigation.Receipts.Children.Where(item => item.BatchStatus == (int)ReceiptBatchStatus.Posted).FirstOrDefault().Children.Insert(
                                GetInsertMonthIndex(ReceiptNavigation.Receipts.Children.Where(item => item.BatchStatus == (int)ReceiptBatchStatus.Posted).FirstOrDefault().Children, navItem.BatchMonth),
                                new NavigationItem { BatchMonth = navItem.BatchMonth, BatchStatus = (int)ReceiptBatchStatus.Posted, ReceiptText = navItem.BatchMonth, Image = "../../../Images/OpenItems/Month.ico" });
                        }

                        if (postedMonth.Children == null || postedMonth.Children.Count == 0)
                        {
                            postedMonth.Children = new ObservableCollection<NavigationItem>(BatchTypeFunctions.GetReceiptPostedList(navItem.BatchMonth));
                        }
                    }
                }
                else
                {
                    ReceiptNavigation.NavigatingToPath =  string.Format(@"{0}\{1}", Enum.Parse(typeof(ReceiptBatchStatus), navItem.BatchStatus.ToString()), navItem.ReceiptText);
                }
            }
            else
            {
                if (navItem.ReceiptText == navItem.BatchMonth)
                {
                    ReceiptNavigation.NavigatingToPath = string.Format(@"{0}\{1}", Enum.Parse(typeof(ReceiptBatchStatus), navItem.BatchStatus.ToString()), navItem.ReceiptText);                   
                }
                else
                {
                    ReceiptNavigation.NavigatingToPath = navItem.ReceiptText;
                }
            }            
        }
     
        public static void AddLoadedItem(NavigationItem navItem)
        {
            IRegionManager regionManager;
            UriQuery query = new UriQuery();

            if (receiptLoaded.Children == null)
            {
                receiptLoaded.Children = new ObservableCollection<NavigationItem>();
            }

            receiptLoaded.Children.Insert(0, navItem);

            Common.SetReceiptNavigation(navItem);


            regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            query.Add("BatchType", navItem.BatchTypeID.ToString());
            query.Add("BatchID", navItem.ReceiptID.ToString());
            regionManager.RequestNavigate(Regions.ContentRegion, new Uri("/ReceiptsSummary" + query.ToString(), UriKind.Relative));
            Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<NavigationChanged>().Publish(Common.ReceiptNavigation.NavigatingToPath);
        }

        #endregion

        #region Select All 

        public static void TextBox_SelectAll(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;

            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }

        #endregion   

        #region Copy 

        public static T Copy<T>(this T source)
            where T : new()
        {
            Type type = source.GetType();
            T temp;

            temp = new T();

            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(temp, property.GetValue(source, new object[] { }), new object[] { });
                }
            }

            return temp;
        }

        public static void Copy<T>(T source, T target)
        {
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(target, property.GetValue(source, new object[] { }), new object[] { });
                }
            }
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
