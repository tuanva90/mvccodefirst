// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowDetailsTabControlCollectionQueueViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
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

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.WPF.ViewModels.Collections;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The mw details view model 2: manage MainWindowDetailsViewModel and TabControl for List and Collectors
    /// </summary>
    public class MainWindowDetailsTabControlCollectionQueueViewModel : ViewModelUseCaseBase
    {
        public Action<object> NavigatedToScreen;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowDetailsTabControlCollectionQueueViewModel"/> class. 
        /// Initializes a new instance of the <see cref="MainWindowDetailsTabControlViewModel"/> class.
        /// </summary>
        public MainWindowDetailsTabControlCollectionQueueViewModel()
        {
            this._listMainWindowDetailsVm = new MainWindowDetailsViewModel(EnumScreen.ColletionQueues);
            this._collectorsMainWindowDetailsVm = new MainWindowDetailsViewModel(EnumScreen.Collectors);
            _changedVisibility = new Visibility();
            _changedVisibility = Visibility.Visible;
        }

        #region Properties

        /// <summary>
        /// The _users main window details vm.
        /// </summary>
        private MainWindowDetailsViewModel _listMainWindowDetailsVm;

        /// <summary>
        /// Gets or sets the users main window details vm.
        /// </summary>
        public MainWindowDetailsViewModel ListMainWindowDetailsVm
        {
            get
            {
                return this._listMainWindowDetailsVm;
            }
            set
            {
                this.SetField(ref this._listMainWindowDetailsVm, value, () => this.ListMainWindowDetailsVm);
            }
        }

        /// <summary>
        /// The _groups main window details vm.
        /// </summary>
        private MainWindowDetailsViewModel _collectorsMainWindowDetailsVm;

        /// <summary>
        /// Gets or sets the groups main window details vm.
        /// </summary>
        public MainWindowDetailsViewModel CollectorsMainWindowDetailsVm
        {
            get
            {
                return this._collectorsMainWindowDetailsVm;
            }
            set
            {
                this.SetField(ref this._collectorsMainWindowDetailsVm, value, () => this.CollectorsMainWindowDetailsVm);
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
        /// The _collectors changed visibility.
        /// </summary>
        private Visibility _collectorsChangedVisibility;

        /// <summary>
        /// Gets or sets the collectors changed visibility.
        /// </summary>
        public Visibility CollectorsChangedVisibility
        {
            get
            {
                return this._collectorsChangedVisibility;
            }
            set
            {
                this.SetField(ref _collectorsChangedVisibility, value, () => CollectorsChangedVisibility);
            }
        }
        
        /// <summary>
        /// The _selected tab.
        /// </summary>
        private int _selectedTabListCollectors;

        /// <summary>
        /// Gets or sets the selected tab.
        /// </summary>
        public int SelectedTab_ListCollectors
        {
            get
            {
                return this._selectedTabListCollectors;
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
        private async void SetSelectedTabWithValidateAsync(int value)
        {
            var collectorsViewModel = this.CollectorsMainWindowDetailsVm.ScreenDetailViewModel as CollectorsViewModel;

            if (collectorsViewModel != null && collectorsViewModel.IsCheckedOut)
            {
                //collectorsViewModel.IsCheckedOut = false;
                await collectorsViewModel.Edit.OnStepAsync(EditCollectorsViewModel.EnumSteps.Cancel);

                if (!collectorsViewModel.IsChanged)
                {
                    this.SetField(ref this._selectedTabListCollectors, value, () => SelectedTab_ListCollectors);
                }
            }
            else
            {
                this.SetField(ref this._selectedTabListCollectors, value, () => SelectedTab_ListCollectors);
            }
        }

        /// <summary>
        /// The check collectors permission.
        /// </summary>
        public void CheckCollectorsPermission()
        {
            var permissionCollectors = Operations.Security.Authorisation.GetPermission(Components.SystemManagementCollectionQueues, Forms.Collectors);
            if (permissionCollectors.CanSee)
            {
                CollectorsChangedVisibility = Visibility.Visible;
            }
            else
            {
                CollectorsChangedVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// The select user.
        /// </summary>
        public void OnRaiseStepChanged()
        {
            var viewModel = this.ListMainWindowDetailsVm;
            if (viewModel != null)
            {
                viewModel.RaiseStepChanged = this.ProcessingStepsOnChild_TabControlCollectionQueueVm;
            }
            var viewModel1 = this.CollectorsMainWindowDetailsVm;
            if (viewModel1 != null)
            {
                viewModel1.RaiseStepChanged = this.ProcessingStepsOnChild_TabControlCollectionQueueVm;
            }
        }

        /// <summary>
        /// The on cancel new item.
        /// </summary>
        public void OnCancelNewItem()
        {
            var viewModel = this.ListMainWindowDetailsVm;
            if (viewModel != null)
            {
                viewModel.CancelNewItem = this.VisibleTab;
            }
            var viewModel1 = this.CollectorsMainWindowDetailsVm;
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
        public async void ProcessingStepsOnChild_TabControlCollectionQueueVm(EnumScreen e, object _params, object item)
        {
            EnumSteps currentStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), _params.ToString());

            // handle behavior for screens when select item
            switch (e)
            {
                case EnumScreen.Collectors:
                    CheckCollectorsPermission();
                    break;
                default:
                    if (currentStep != EnumSteps.ItemLocked)
                    {
                        ChangedVisibility = Visibility.Collapsed;
                        CollectorsChangedVisibility = Visibility.Collapsed;
                    }
                    break;
            }

            // handle behavior for step on content
            switch (currentStep)
            {
                case EnumSteps.Save:
                    if (e == EnumScreen.ColletionQueues)
                    {
                        var collectorsViewModel =
                            this.CollectorsMainWindowDetailsVm.ScreenDetailViewModel as CollectorsViewModel;
                        if (collectorsViewModel != null)
                        {
                            await collectorsViewModel.OnStepAsync(CollectorsViewModel.EnumSteps.Start);
                        }
                    }
                    else
                    {
                        var collectionQueueViewModel =
                            this.ListMainWindowDetailsVm.ScreenDetailViewModel as CollectionsManagementViewModel;
                        if (collectionQueueViewModel != null)
                        {
                            await collectionQueueViewModel.OnStepAsync(CollectionsManagementViewModel.EnumSteps.Start);
                        }
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
            this.CheckCollectorsPermission();
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
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                if (this._listMainWindowDetailsVm != null)
                {
                    this._listMainWindowDetailsVm.Dispose();
                    this._listMainWindowDetailsVm = null;
                }
                if (this.CollectorsMainWindowDetailsVm != null)
                {
                    this.CollectorsMainWindowDetailsVm.Dispose();
                    this.CollectorsMainWindowDetailsVm = null;
                }

                base.Dispose();
            }));
        }
    }
}
