using System;
using System.Linq;
using Telerik.Windows.Controls.GridView;
using Insyston.Operations.WPF.View.Common;
using Insyston.Operations.Business.Receipts;
using Insyston.Operations.WPF.ViewModel.Receipts.DDCC.ViewModel;

namespace Insyston.Operations.WPF.View.Receipts.DDCC
{
    public partial class DDCCStatusChange : OperationsView
    {
        public DDCCStatusChange()
        {
        }

        public DDCCStatusChange(ReceiptBatchType batchType)
        {
            InitializeComponent();
            DDCCStatusChangeViewModel = new DDCCStatusChangeViewModel(batchType);
        }

        public DDCCStatusChangeViewModel DDCCStatusChangeViewModel
        {
            get
            {
                return this.DataContext as DDCCStatusChangeViewModel;
            }
            set
            {
                this.DataContext = value;
                value.ClearGridFilter += ClearGridFilter;
            }
        }

        void ClearGridFilter()
        {
            while (GrdBatchSummary.FilterDescriptors.Count > 0)
            {
                ((IColumnFilterDescriptor)GrdBatchSummary.FilterDescriptors.FirstOrDefault()).Clear();
            }
        }
    }
}
