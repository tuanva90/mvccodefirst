using Insyston.Operations.Business.Receipts;
using Insyston.Operations.WPF.View.Common;
using Insyston.Operations.WPF.ViewModel.Receipts.DishonourReversal;
using System;
using System.Linq;

namespace Insyston.Operations.WPF.View.Receipts.DishonourReversal
{
    public partial class DishonourReversalReceiptExcelImport : OperationsView
    {
        public DishonourReversalReceiptExcelImport()
        {
            InitializeComponent();
        }

        public DishonourReversalReceiptExcelImport(ReceiptBatchType batchType, int receiptBatchID) 
        {
            InitializeComponent();
            DishonourReversalExcelImportViewModel = new DishonourReversalExcelImportViewModel(batchType, receiptBatchID);
        }

        public DishonourReversalExcelImportViewModel DishonourReversalExcelImportViewModel 
        {
            get
            {
                return this.DataContext as DishonourReversalExcelImportViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }       
    }
}
