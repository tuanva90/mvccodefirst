// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowDetailsTabControl.xaml.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for MWDetails2.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.Views.Shell
{
    using System.Windows;

    using Insyston.Operations.WPF.ViewModels.Shell;

    using Telerik.Windows.Controls;

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
                this._viewModel.OnRaiseStepChanged();
                this._viewModel.OnCancelNewItem();
                this._viewModel.SelectedTab = 0;

                this._viewModel.UsersMainWindowDetailsVm.NavigatedToScreen = this._viewModel.NavigatedToScreen;
                this._viewModel.GroupsMainWindowDetailsVm.NavigatedToScreen = this._viewModel.NavigatedToScreen;
                this._viewModel.MembershipMainWindowDetailsVm.NavigatedToScreen = this._viewModel.NavigatedToScreen;
                //if (this._viewModel.NavigatedToScreen != null)
                //{
                //    if (this._viewModel.UsersMainWindowDetailsVm != null)
                //    {
                //        this._viewModel.NavigatedToScreen();
                //    }
                //    if (this._viewModel.GroupsMainWindowDetailsVm != null)
                //    {
                //        this._viewModel.NavigatedToScreen(this._viewModel.GroupsMainWindowDetailsVm);
                //    }
                //    if (this._viewModel.MembershipMainWindowDetailsVm != null)
                //    {
                //        this._viewModel.NavigatedToScreen(this._viewModel.MembershipMainWindowDetailsVm);
                //    }
                //}
            }
        }

        /// <summary>
        /// The rad tab control base_ on selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RadTabControlBase_OnSelectionChanged(object sender, RadSelectionChangedEventArgs e)
        {
            this._viewModel = this.DataContext as MainWindowDetailsTabControlViewModel;
            if (this._viewModel != null)
            {
                var tabItem = SecurityTabControl[this._viewModel.SelectedTab];

                // try keep current selectedTab when edit mode and cancel dialog.
                if (null != tabItem)
                {
                    tabItem.IsSelected = true;
                }
            }
        }
    }
}
