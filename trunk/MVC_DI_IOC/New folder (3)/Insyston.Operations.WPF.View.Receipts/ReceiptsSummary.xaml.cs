using System;
using System.Linq;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using Insyston.Operations.WPF.View.Receipts;
using Telerik.Windows.Controls.GridView;
using Insyston.Operations.WPF.ViewModel.Receipts;

namespace Insyston.Operations.WPF.View.Receipts
{
    [Export(typeof(ReceiptsSummary))]
    public partial class ReceiptsSummary : UserControl
    {
        public ReceiptsSummary()
        {
            InitializeComponent();            
        }

        [Import]
        public ReceiptsSummaryViewModel ReceiptsSummaryViewModel
        {
            get { return this.DataContext as ReceiptsSummaryViewModel; }
            set 
            { 
                this.DataContext = value;
                value.ClearGridFilter += ClearGridFilter;
            }
        }

        void ClearGridFilter()
        {
            while (GrdReceipts.FilterDescriptors.Count > 0)
            {
                ((IColumnFilterDescriptor)GrdReceipts.FilterDescriptors.FirstOrDefault()).Clear();
            }
        }
    }
}
