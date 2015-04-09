using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Insyston.Operations.WPF.ViewModels.Security;

namespace Insyston.Operations.WPF.Views.Security
{
    /// <summary>
    /// Interaction logic for SystemSettings.xaml
    /// </summary>
    public partial class SystemSettingView : UserControl
    {
        public SystemSettingView()
        {
            this.InitializeComponent();
            //this.DataContext = new SystemSettingViewModel();
    
            this.Loaded += this.SystemSettings_Loaded;
        }

        private async void SystemSettings_Loaded(object sender, RoutedEventArgs e)
        {
            await ((SystemSettingViewModel)this.DataContext).OnStepAsync(SystemSettingViewModel.EnumSteps.Start);
        }
    }
}
