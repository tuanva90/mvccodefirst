// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowDetailsTabControlViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The mw details view model 2.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Shell
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Security;

    /// <summary>
    /// The mw details view model 2: manage MainWindowDetailsViewModel and TabControl for Users and Groups
    /// </summary>
    public class MainWindowDetailsTabControlViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// The navigated to screen.
        /// </summary>
        public Action<object> NavigatedToScreen;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowDetailsTabControlViewModel"/> class.
        /// </summary>
        public MainWindowDetailsTabControlViewModel()
        {
            this._usersMainWindowDetailsVm = new MainWindowDetailsViewModel(EnumScreen.Users);
            this._groupsMainWindowDetailsVm = new MainWindowDetailsViewModel(EnumScreen.Groups);
            this._membershipMainWindowDetailsVm = new MainWindowDetailsViewModel(EnumScreen.Membership);
            _changedVisibility = new Visibility();
            _changedVisibility = Visibility.Visible;
            
        }

        #region Properties

        /// <summary>
        /// The _users main window details vm.
        /// </summary>
        private MainWindowDetailsViewModel _usersMainWindowDetailsVm;

        /// <summary>
        /// Gets or sets the users main window details vm.
        /// </summary>
        public MainWindowDetailsViewModel UsersMainWindowDetailsVm
        {
            get
            {
                return this._usersMainWindowDetailsVm;
            }
            set
            {
                this.SetField(ref this._usersMainWindowDetailsVm, value, () => this.UsersMainWindowDetailsVm);
            }
        }

        /// <summary>
        /// The _groups main window details vm.
        /// </summary>
        private MainWindowDetailsViewModel _groupsMainWindowDetailsVm;

        /// <summary>
        /// Gets or sets the groups main window details vm.
        /// </summary>
        public MainWindowDetailsViewModel GroupsMainWindowDetailsVm
        {
            get
            {
                return this._groupsMainWindowDetailsVm;
            }
            set
            {
                this.SetField(ref this._groupsMainWindowDetailsVm, value, () => this.GroupsMainWindowDetailsVm);
            }
        }

        /// <summary>
        /// The _membership main window details vm.
        /// </summary>
        private MainWindowDetailsViewModel _membershipMainWindowDetailsVm;

        /// <summary>
        /// Gets or sets the membership main window details vm.
        /// </summary>
        public MainWindowDetailsViewModel MembershipMainWindowDetailsVm
        {
            get
            {
                return this._membershipMainWindowDetailsVm;
            }
            set
            {
                this.SetField(ref this._membershipMainWindowDetailsVm, value, () => this.MembershipMainWindowDetailsVm);
            }
        }

        /// <summary>
        /// The _changed visibility.
        /// </summary>
        private Visibility _changedVisibility;

        /// <summary>
        /// Gets or sets the changed visibility.
        /// </summary>
        public Visibility ChangedVisibility
        {
            get
            {
                return this._changedVisibility;
            }
            set
            {
                this.SetField(ref _changedVisibility, value, () => ChangedVisibility);
            }
        }

        /// <summary>
        /// The _selected tab.
        /// </summary>
        private int _selectedTab;

        /// <summary>
        /// Gets or sets the selected tab.
        /// </summary>
        public int SelectedTab
        {
            get
            {
                return this._selectedTab;
            }
            set
            {
                // check validate when set selected tab
                this.SetSelectedTabWithValidateAsync(value);
            }
        }

        #endregion

        /// <summary>
        /// The set selected tab with validate async.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SetSelectedTabWithValidateAsync(int value)
        {
            var membershipViewModel = this.MembershipMainWindowDetailsVm.ScreenDetailViewModel as MembershipViewModel;

            if (membershipViewModel != null && membershipViewModel.IsCheckedOut)
            {
                //membershipViewModel.IsCheckedOut = false;
                await membershipViewModel.Edit.OnStepAsync(EditMembershipViewModel.EnumSteps.Cancel);

                if (!membershipViewModel.IsChanged)
                {
                    this.SetField(ref _selectedTab, value, () => SelectedTab);
                }
            }
            else
            {
                this.SetField(ref _selectedTab, value, () => SelectedTab);
            }
        }

        /// <summary>
        /// The select user.
        /// </summary>
        public void OnRaiseStepChanged()
        {
            var viewModel = this.UsersMainWindowDetailsVm;
            if (viewModel != null)
            {
                viewModel.RaiseStepChanged = this.ProcessingStepsOnChild_TabControlVm;
            }
            var viewModel1 = this.GroupsMainWindowDetailsVm;
            if (viewModel1 != null)
            {
                viewModel1.RaiseStepChanged = this.ProcessingStepsOnChild_TabControlVm;
            }
            var viewModel2 = this.MembershipMainWindowDetailsVm;
            if (viewModel2 != null)
            {
                viewModel2.RaiseStepChanged = this.ProcessingStepsOnChild_TabControlVm;
            }

        }

        /// <summary>
        /// When cancel add new item or cancel copy new item, back to grid summary.
        /// </summary>
        public void OnCancelNewItem()
        {
            var viewModel = this.UsersMainWindowDetailsVm;
            if (viewModel != null)
            {
                viewModel.CancelNewItem = this.VisibleTab;
            }
            var viewModel1 = this.GroupsMainWindowDetailsVm;
            if (viewModel1 != null)
            {
                viewModel1.CancelNewItem = this.VisibleTab;
            }
        }

        /// <summary>
        /// The collapse tab.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="_params">
        /// The _params.
        /// </param>
        /// <param name="item">
        /// The id.
        /// </param>
        public async void ProcessingStepsOnChild_TabControlVm(EnumScreen e, object _params, object item)
        {
            // handle behavior for screens when select item
            switch (e)
            {
                case EnumScreen.Membership:
                    ChangedVisibility = Visibility.Visible;
                    break;
                default:
                    ChangedVisibility = Visibility.Collapsed;
                    break;
            }

            // handle behavior for step on content
            EnumSteps currentStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), _params.ToString());
            switch (currentStep)
            {
                case EnumSteps.Save:
                    if (e == EnumScreen.Users || e == EnumScreen.Groups)
                    {
                        var membershipViewModel =
                            this.MembershipMainWindowDetailsVm.ScreenDetailViewModel as MembershipViewModel;
                        if (membershipViewModel != null)
                        {
                            await membershipViewModel.OnStepAsync(MembershipViewModel.EnumSteps.Start);
                        }
                    }
                    else
                    {
                        var userViewModel = this.UsersMainWindowDetailsVm.ScreenDetailViewModel as UsersViewModel;
                        if (userViewModel != null)
                        {
                            await userViewModel.OnStepAsync(UsersViewModel.EnumSteps.Start);
                        }
                        return;
                    }
                    break;

            }

            // Raise action RaiseActionsWhenChangeStep again
            this.RaiseActionsWhenChangeStep(e, _params, item);
        }

        /// <summary>
        /// The visible tab.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public void VisibleTab(EnumScreen e)
        {
            ChangedVisibility = Visibility.Visible;

            // Raise action CancelNewItem
            this.CancelNewItem(e);
        }

        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public override void Dispose()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new System.Action(() =>
            {
                if (this._usersMainWindowDetailsVm != null)
                {
                    this._usersMainWindowDetailsVm.Dispose();
                    this._usersMainWindowDetailsVm = null;
                }
                if (this._groupsMainWindowDetailsVm != null)
                {
                    this._groupsMainWindowDetailsVm.Dispose();
                    this._groupsMainWindowDetailsVm = null;
                }
                if (this._membershipMainWindowDetailsVm != null)
                {
                    this._membershipMainWindowDetailsVm.Dispose();
                    this._membershipMainWindowDetailsVm = null;
                }

                base.Dispose();
            }));
        }
    }
}
