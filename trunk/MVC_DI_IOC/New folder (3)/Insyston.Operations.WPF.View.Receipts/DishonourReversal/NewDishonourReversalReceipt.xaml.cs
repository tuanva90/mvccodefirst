using Insyston.Operations.Business.Receipts;
using Insyston.Operations.WPF.View.Common;
using Insyston.Operations.WPF.ViewModel.Receipts.DishonourReversal;
using System;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls.GridView;

namespace Insyston.Operations.WPF.View.Receipts.DishonourReversal
{    
    public partial class NewDishonourReversalReceipt : OperationsView
    {
        public NewDishonourReversalReceipt()
        {
            InitializeComponent();
        }

        public NewDishonourReversalReceipt(ReceiptBatchType batchType, int receiptBatchID, int batchStatus, int receiptID = 0)
        {
            InitializeComponent();
            NewDishonourReversalViewModel = new NewDishonourReversalViewModel(batchType, receiptBatchID, batchStatus, receiptID);
            this.Loaded += NewDishonourReversalReceipt_Loaded;
        }

        void NewDishonourReversalReceipt_Loaded(object sender, RoutedEventArgs e)
        {
            if (NewDishonourReversalViewModel.IsResetAllocation)
            {
                NewDishonourReversalViewModel.ResetAllocation.Execute();
            }
        }

        public NewDishonourReversalViewModel NewDishonourReversalViewModel
        {
            get { return this.DataContext as NewDishonourReversalViewModel; }
            set
            {
                this.DataContext = value;
                value.ClearGridFilter += ClearGridFilter;
            }
        }

        void ClearGridFilter()
        {
            while (GrdOpenItems.FilterDescriptors.Count > 0)
            {
                ((IColumnFilterDescriptor)GrdOpenItems.FilterDescriptors.FirstOrDefault()).Clear();
            }
        }
    }
}
