// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MWDetails2.xaml.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for MWDetails2.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.Views.Shell
{
    using System.Windows;

    using Insyston.Operations.WPF.ViewModel.Shell;

    /// <summary>
    /// Interaction logic for MWDetails2.xaml
    /// </summary>
    public partial class MainWindowDetailsTabControl
    {
        /// <summary>
        /// The view model.
        /// </summary>
        private MainWindowDetailsTabControlViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowDetailsTabControl"/> class.
        /// </summary>
        public MainWindowDetailsTabControl()
        {
            InitializeComponent();
            this.DataContext = this._viewModel;
            this.Loaded += OnLoaded;
        }

        /// <summary>
        /// The on loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="routedEventArgs">
        /// The routed event args.
        /// </param>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this._viewModel = this.DataContext as MainWindowDetailsTabControlViewModel;
            if (this._viewModel != null)
            {
                this._viewModel.SelectUser();
            }
        }
    }
}
