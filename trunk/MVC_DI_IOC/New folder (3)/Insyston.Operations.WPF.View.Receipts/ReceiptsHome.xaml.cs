using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel.Composition;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using Insyston.Operations.WPF.ViewModel.Receipts;

namespace Insyston.Operations.WPF.View.Receipts
{
    [Export(typeof(ReceiptsHome))]
    public partial class ReceiptsHome : UserControl
    {
        public ReceiptsHome()
        {
            InitializeComponent();
        }       

        [Import]
        public ReceiptsHomeViewModel ReceiptsHomeViewModel
        {
            get { return this.DataContext as ReceiptsHomeViewModel; }
            set
            {
                this.DataContext = value;
                value.ClearGridFilters += ClearGridFilters;                
            }
        }

        void ClearGridFilters(object sender, bool isSummary)
        {
            ((IColumnFilterDescriptor)GrdBatchStatusSummary.FilterDescriptors.FirstOrDefault()).Clear();
        }        
    }
}
