using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using Insyston.Operations.Security.Enums;
using Insyston.Operations.WPF.ViewModels.Shell;

namespace Insyston.Operations.WPF.Views.Shell
{
    using Insyston.Operations.WPF.Views.Shell.ViewModel;
    using System.Windows;
    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            this.InitializeComponent();

            //Set start up location for Login Window is in Center of Main Screen
            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            this.Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2;

            LoginViewModel viewModel = new LoginViewModel();
            viewModel.OnAuthenticationStatusChanged += this.viewModel_OnAuthenticationStatusChanged;
            this.DataContext = viewModel;
            this.Loaded += async (object sender, RoutedEventArgs e) =>
            {
                await ((LoginViewModel)this.DataContext).OnStepAsync(LoginViewModel.EnumStep.Start);
            };
        }

        private void viewModel_OnAuthenticationStatusChanged(AuthenticationResult status)
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated == true && status == AuthenticationResult.OK)
            {
                Storyboard fadeOut = this.Resources["FadeOut"] as Storyboard;

                fadeOut.Completed += (object sender, EventArgs e) =>
                {
                    MainWindow main = new MainWindow();
                    MainViewModel ViewModelMain = new MainViewModel();
                    main.DataContext = ViewModelMain;
                    Application.Current.MainWindow = main.ParentOfType<Window>();
                    main.Show();
                    this.Close();
                };
                fadeOut.Begin();
            }
        }

        private void TwitterButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://twitter.com/Insyston");
        }

        private void LinkedInButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.linkedin.com/company/Insyston");
        }
    }
}
