using System;
using System.Linq;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using Telerik.Windows.Controls.GridView;
using Insyston.Operations.WPF.ViewModel.Receipts;

namespace Insyston.Operations.WPF.View.Receipts
{
    [Export(typeof(ReceiptsBatchSummary))]
    public partial class ReceiptsBatchSummary : UserControl
    {
        public ReceiptsBatchSummary()
        {
            InitializeComponent();            
        }

        [Import]
        public ReceiptsBatchSummaryViewModel ReceiptsBatchSummaryViewModel
        {
            get { return this.DataContext as ReceiptsBatchSummaryViewModel; }
            set 
            { 
                this.DataContext = value;
                value.ClearGridFilters += ClearGridFilters;
            }
        }

        void ClearGridFilters(object sender, bool isSummary)
        {
            if (isSummary)
            {
                while (GrdSummary.FilterDescriptors.Count > 0)
                {
                    ((IColumnFilterDescriptor)GrdSummary.FilterDescriptors.FirstOrDefault()).Clear();
                }
            }
            else
            {
                while (GrdBatchMonthSummary.FilterDescriptors.Count > 0)
                {
                    ((IColumnFilterDescriptor)GrdBatchMonthSummary.FilterDescriptors.FirstOrDefault()).Clear();
                }
            }
        }     
    }
}
