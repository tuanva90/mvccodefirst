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

namespace Insyston.Operations.WPF.Views.Shell
{
    using Insyston.Operations.WPF.ViewModels.Shell;

    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePageView : UserControl
    {
        public HomePageView()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var viewModel = this.DataContext as HomePageViewModel;
            if (viewModel != null)
            {
                viewModel.RaiseActionsWhenChangeStep(EnumScreen.Home, EnumSteps.Home);
            }
        }

    }
}
