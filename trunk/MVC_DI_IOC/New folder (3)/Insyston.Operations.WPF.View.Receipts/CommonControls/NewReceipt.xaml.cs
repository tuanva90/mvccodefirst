using Microsoft.Practices.Prism.Events;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls.GridView;
using Insyston.Operations.WPF.View.Common;
using Insyston.Operations.WPF.ViewModel.Receipts.CommonControls;
using Insyston.Operations.Business.Receipts;

namespace Insyston.Operations.WPF.View.Receipts.CommonControls
{
    [Export(typeof(NewReceipt))]
    public partial class NewReceipt : OperationsView
    {
        public NewReceipt()
        {
            InitializeComponent();
            this.Unloaded +=NewReceipt_Unloaded;
            NewReceiptViewModel = new NewReceiptViewModel();
        }        

        public NewReceipt(ReceiptBatchType batchType, int receiptBatchID, int batchStatus, int receiptID = 0)
        {
            IEventAggregator eventtAggregator;

            InitializeComponent();
            NewReceiptViewModel = new NewReceiptViewModel(batchType, receiptBatchID, batchStatus, receiptID);
            NewReceiptViewModel.GridViewTotalsChanged += CaculateAggregates;
            this.Unloaded += NewReceipt_Unloaded;            
        }

        public NewReceiptViewModel NewReceiptViewModel
        {
            get { return this.DataContext as NewReceiptViewModel; }
            set 
            { 
                this.DataContext = value;
                value.ClearGridFilter += ClearGridFilter;
            }
        }

        void NewReceipt_Unloaded(object sender, RoutedEventArgs e)
        {           
            if (this.DataContext != null)
            {
                ((NewReceiptViewModel)this.DataContext).GridViewTotalsChanged -= CaculateAggregates;
            }
        }

        void ClearGridFilter()
        {
            while (GrdOpenItems.FilterDescriptors.Count > 0)
            {
                ((IColumnFilterDescriptor)GrdOpenItems.FilterDescriptors.FirstOrDefault()).Clear();
            }
        }

        private void CaculateAggregates()
        {
            GrdOpenItems.CalculateAggregates();
        }
    }
}
