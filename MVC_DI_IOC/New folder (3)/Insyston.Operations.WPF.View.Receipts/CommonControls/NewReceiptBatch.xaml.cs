using System;
using System.Linq;
using System.ComponentModel.Composition;
using Insyston.Operations.WPF.View.Common;
using Insyston.Operations.WPF.ViewModel.Receipts.CommonControls;

namespace Insyston.Operations.WPF.View.Receipts.CommonControls
{ 
    [Export(typeof(NewReceiptBatch))]
    public partial class NewReceiptBatch : OperationsView
    {
        public NewReceiptBatch()
        {
            InitializeComponent();
            NewReceiptBatchViewModel = new NewReceiptBatchViewModel();
        }
        
        public NewReceiptBatch(int batchType)
        {
            InitializeComponent();
            NewReceiptBatchViewModel = new NewReceiptBatchViewModel(batchType);
        }
        
        public NewReceiptBatchViewModel NewReceiptBatchViewModel
        {
            get { return this.DataContext as NewReceiptBatchViewModel; }
            set { this.DataContext = value; }
        }       
    }
}
