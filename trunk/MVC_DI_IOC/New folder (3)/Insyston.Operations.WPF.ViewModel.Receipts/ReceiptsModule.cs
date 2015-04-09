using System;
using System.Linq;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Insyston.Operations.Base.Controls;

namespace Insyston.Operations.View.OpenItems.Receipts
{
    [ModuleExport(typeof(ReceiptsModule))]
    public class ReceiptsModule : IModule
    {
        [Import]
        public IRegionManager RegionManager;

        public void Initialize()
        {            
            RegionManager.RegisterViewWithRegion(Regions.ContentRegion, typeof(ReceiptsHome));
            RegionManager.RegisterViewWithRegion(Regions.ContentRegion, typeof(ReceiptsBatchSummary));
            RegionManager.RegisterViewWithRegion(Regions.ContentRegion, typeof(ReceiptsSummary));            

            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotFocusEvent, new RoutedEventHandler(Common.TextBox_SelectAll));
            EventManager.RegisterClassHandler(typeof(NumericTextBox), UIElement.GotFocusEvent, new RoutedEventHandler(Common.TextBox_SelectAll));
        }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
    }
}
                                                                                                                           