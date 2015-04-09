using Insyston.Operations.Business.Receipts;
using Insyston.Operations.WPF.View.Common;
using Insyston.Operations.WPF.ViewModel.Receipts.DishonourReversal;
using System;
using System.Linq;
using Telerik.Windows.Controls.GridView;

namespace Insyston.Operations.WPF.View.Receipts.DishonourReversal
{    
    public partial class ReversalReceiptReallocation : OperationsView
    {
        public ReversalReceiptReallocation()
        {
            InitializeComponent();            
        }

        public ReversalReceiptReallocation(NewDishonourReversalViewModel viewModel, int reallocationReceiptID)
        {
            InitializeComponent();
            NewReversalReAllocationViewModel = new NewReversalReAllocationViewModel(viewModel, reallocationReceiptID);
        }

        public NewReversalReAllocationViewModel NewReversalReAllocationViewModel
        {
            get { return this.DataContext as NewReversalReAllocationViewModel; }
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
