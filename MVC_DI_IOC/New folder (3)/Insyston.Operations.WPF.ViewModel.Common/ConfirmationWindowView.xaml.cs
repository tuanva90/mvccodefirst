using System;
using System.Collections.Generic;
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

namespace Insyston.Operations.WPF.ViewModels.Common
{
    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for ComfirmationWindowView.xaml
    /// </summary>
    public partial class ConfirmationWindowView : RadWindow
    {
        public ConfirmationWindowView()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            this.DialogResult = false;
        }
    }
}
