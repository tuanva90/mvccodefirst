using Insyston.Operations.WPF.ViewModel.Receipts;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Insyston.Operations.WPF.View.Receipts
{
    public partial class ReceiptsHeader : UserControl
    {
        public ReceiptsHeader()
        {            
            InitializeComponent();
            ReceiptsHeaderViewModel = new ReceiptsHeaderViewModel();      
        }
        
        public ReceiptsHeaderViewModel ReceiptsHeaderViewModel
        {
            get { return this.DataContext as ReceiptsHeaderViewModel; }
            set { this.DataContext = value; }
        }

        private void breadNavigation_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {           
            Shared.ReceiptNavigation.ClearNavigatingPath();
        }     
    }
}
