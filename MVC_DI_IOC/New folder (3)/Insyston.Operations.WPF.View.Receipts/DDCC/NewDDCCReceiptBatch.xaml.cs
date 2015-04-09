using Insyston.Operations.WPF.View.Common;
using System;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls.GridView;

namespace Insyston.Operations.WPF.View.Receipts.DDCC
{
    public partial class NewDDCCReceiptBatch : OperationsView
    {
        public NewDDCCReceiptBatch()
        {
            InitializeComponent();            
        }
        
        private void ClearGridFilter(object sender, RoutedEventArgs e)
        {
            while (grdBankInternalCompany.FilterDescriptors.Count > 0)
            {
                ((IColumnFilterDescriptor)grdBankInternalCompany.FilterDescriptors.FirstOrDefault()).Clear();
            }
        }       
    }
}
