using Insyston.Operations.Business.Receipts.Model;
using Insyston.Operations.WPF.View.Common;
using Insyston.Operations.WPF.ViewModel.Receipts.DishonourReversal;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Windows.Controls.GridView;

namespace Insyston.Operations.WPF.View.Receipts.DishonourReversal
{    
    public partial class MultipleReversalReAllocation : OperationsView
    {
        public MultipleReversalReAllocation()
        {
            InitializeComponent();
        }

        public MultipleReversalReAllocation(int batchID, int reason, List<DishonourReversalReceiptSummary> reversalReceipts)
        {
            InitializeComponent();
            MultipleReversalReAllocationViewModel = new MultipleReversalReAllocationViewModel(batchID, reason, reversalReceipts);
        }

        MultipleReversalReAllocationViewModel MultipleReversalReAllocationViewModel
        {
            get
            {
                return this.DataContext as MultipleReversalReAllocationViewModel;
            }
            set
            {
                this.DataContext = value;
                value.ClearGridFilter += ClearGridFilter;
            }
        }

        void ClearGridFilter()
        {
            while (GrdDishonourReversalReceipts.FilterDescriptors.Count > 0)
            {
                ((IColumnFilterDescriptor)GrdDishonourReversalReceipts.FilterDescriptors.FirstOrDefault()).Clear();
            }
        }
    }
}
