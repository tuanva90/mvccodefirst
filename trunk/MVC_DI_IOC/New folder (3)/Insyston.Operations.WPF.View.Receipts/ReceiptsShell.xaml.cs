using System;
using System.Linq;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using Telerik.Windows.Controls;
using Insyston.Operations.WPF.ViewModel.Events;
using System.Windows.Interop;
using Insyston.Operations.WPF.ViewModel.Receipts;

namespace Insyston.Operations.WPF.View.Receipts
{
    [Export(typeof(ReceiptsShell))]
    public partial class ReceiptsShell : UserControl
    {        
        public ReceiptsShell()
        {
            InitializeComponent();
            StyleManager.ApplicationTheme = new Windows8Theme();
        }    
        
        [Import]
        public ReceiptShellViewModel ReceiptShellViewModel
        {
            get { return this.DataContext as ReceiptShellViewModel; }
            set { this.DataContext = value; }           
        }       
    }
}
