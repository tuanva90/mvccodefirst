using Insyston.Operations.WPF.View.Common;
using Insyston.Operations.WPF.ViewModel.Receipts.DishonourReversal;
using System;
using System.Linq;

namespace Insyston.Operations.WPF.View.Receipts.DishonourReversal
{
    public partial class AddReceipts : OperationsView
    {
        public AddReceipts()
        {
            InitializeComponent();
        }

        public AddReceipts(int batchType, int batchID)
        {
            InitializeComponent();
            AddReceiptsViewModel = new AddReceiptsViewModel(batchType, batchID);
        }

        public AddReceiptsViewModel AddReceiptsViewModel
        {
            get { return this.DataContext as AddReceiptsViewModel; }
            set { this.DataContext = value; }
        }       
    }
}
