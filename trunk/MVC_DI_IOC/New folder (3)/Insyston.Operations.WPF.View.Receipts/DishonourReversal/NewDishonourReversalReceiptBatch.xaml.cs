using Insyston.Operations.WPF.View.Common;
using System;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls.GridView;

namespace Insyston.Operations.WPF.View.Receipts.DishonourReversal
{
    public partial class NewDishonourReversalReceiptBatch : OperationsView
    {
        public NewDishonourReversalReceiptBatch()
        {
            InitializeComponent();            
        }
        
        private void ClearGridFilter(object sender, RoutedEventArgs e)
        {
            while (grdReceipts.FilterDescriptors.Count > 0)
            {
                ((IColumnFilterDescriptor)grdReceipts.FilterDescriptors.FirstOrDefault()).Clear();
            }
        }       
    }
}
