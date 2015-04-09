using Insyston.Operations.WPF.View.Common;
using Insyston.Operations.WPF.ViewModel.Receipts.CommonControls;
using System;
using System.Linq;

namespace Insyston.Operations.WPF.View.Receipts.CommonControls
{      
    public partial class AddOtherCharge : OperationsView
    {
        public AddOtherCharge()
        {
            InitializeComponent();
        }

        public AddOtherCharge(int contractID)
        {
            InitializeComponent();
            AddOtherChargeViewModel = new AddOtherChargeViewModel(contractID);
        }
              
        public AddOtherChargeViewModel AddOtherChargeViewModel
        {
            get { return this.DataContext as AddOtherChargeViewModel; }
            set { this.DataContext = value; }
        }
    }
}
