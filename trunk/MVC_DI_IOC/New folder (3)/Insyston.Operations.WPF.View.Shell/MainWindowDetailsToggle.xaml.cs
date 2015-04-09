// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MWDetailsToggle.xaml.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for MWDetailsToggle.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.Views.Shell
{
    using System.Windows;

    using Insyston.Operations.WPF.ViewModels.Shell;

    /// <summary>
    /// Interaction logic for MWDetailsToggle.xaml
    /// </summary>
    public partial class MainWindowDetailsToggle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowDetailsToggle"/> class.
        /// </summary>
        public MainWindowDetailsToggle()
        {
            InitializeComponent();
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
            var viewModel = this.DataContext as MainWindowDetailsToggleViewModel;
            if (viewModel != null)
            {
                viewModel.SetSelectedTab();
                viewModel.OnRaiseStepChanged();
                viewModel.OnCancelNewItem();
                viewModel.ClickHyperlink();
            }
        }
    }
}
