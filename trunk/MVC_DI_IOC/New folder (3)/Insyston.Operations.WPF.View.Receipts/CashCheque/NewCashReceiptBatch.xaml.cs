using System;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls.GridView;
using Insyston.Operations.WPF.View.Common;

namespace Insyston.Operations.WPF.View.Receipts.CashCheque
{
    public partial class NewCashReceiptBatch : OperationsView
    {
        public NewCashReceiptBatch()
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
