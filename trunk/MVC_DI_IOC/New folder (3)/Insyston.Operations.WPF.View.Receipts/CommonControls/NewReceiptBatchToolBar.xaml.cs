using Insyston.Operations.WPF.ViewModel.Receipts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Insyston.Operations.WPF.View.Receipts.CommonControls
{    
    public partial class NewReceiptBatchToolBar : UserControl
    {
        public NewReceiptBatchToolBar()
        {
            InitializeComponent();
            NewReceiptBatchToolBarViewModel = new NewReceiptBatchToolBarViewModel();
        }

        [Import]
        public NewReceiptBatchToolBarViewModel NewReceiptBatchToolBarViewModel
        {
            get
            {
                return this.DataContext as NewReceiptBatchToolBarViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}
