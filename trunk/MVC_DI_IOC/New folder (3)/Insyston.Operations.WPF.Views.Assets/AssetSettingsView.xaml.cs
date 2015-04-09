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

namespace Insyston.Operations.WPF.Views.Assets
{
    using Insyston.Operations.WPF.ViewModels.Assets;

    using Telerik.Windows.Data;

    /// <summary>
    /// Interaction logic for AssetSettingsView.xaml
    /// </summary>
    public partial class AssetSettingsView : UserControl
    {
        public AssetSettingsView()
        {
            InitializeComponent();
            this.Loaded += this.AssetSettings_Loaded;
        }

        private async void AssetSettings_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await ((AssetSettingsViewModel)this.DataContext).OnStepAsync(AssetSettingsViewModel.EnumSteps.Start);
        }
    }
}
