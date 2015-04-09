namespace Insyston.Operations.WPF.Views.Shell
{
    using System;
    using System.Windows;

    using Caliburn.Micro;

    using Insyston.Operations.Security;
    using Insyston.Operations.WPF.Views.Shell.ViewModel;

    using Telerik.Windows.Controls;

    /// <summary>
    /// The main window.
    /// </summary>
    public partial class MainWindow : RadWindow
    {
        //MainViewModel ViewModelMain = new MainViewModel();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
            //this.DataContext = ViewModelMain;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as MainViewModel;
            if (viewModel != null)
            {
                viewModel.PowerOffMainWindowView += this.ActionPowerOff;
            }
        }

        /// <summary>
        /// The action power off.
        /// </summary>
        private async void ActionPowerOff()
        {
            await Authentication.LogoffAsync();
            this.Close();
        }
    }
}