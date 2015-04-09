// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetClassesMakeViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetClassesMakeViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.AssetClasses
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.TextFormatting;
    using System.Windows.Threading;

    using global::WPF.DataTable.Models;

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Bussiness.Assets.AssetClasses;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.Locking.Helpers;
    using Insyston.Operations.Model;
    using Insyston.Operations.Security;
    using Insyston.Operations.Security.Model;
    using Insyston.Operations.WPF.ViewModel.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.Asset_Make;
    using Insyston.Operations.WPF.ViewModels.Assets.Validation;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Enums;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;
    using Insyston.Operations.WPF.ViewModels.Common.Model.Filtering;

    using WPFDynamic.ViewModels.Controls;

    /// <summary>
    /// The asset classes make view model.
    /// </summary>
    public class AssetClassesMakeViewModel : ViewModelUseCaseBase
    {
        #region Private property

        /// <summary>
        /// The loading text.
        /// </summary>
        private const string LoadingText = "Please Wait Loading ...";

        /// <summary>
        /// The current step.
        /// </summary>
        private Asset.EnumSteps _currentEnumStep;

        /// <summary>
        /// The _dynamic asset class make view model.
        /// </summary>
        private DynamicGridViewModel _dynamicAssetClassMakeViewModel;

        /// <summary>
        /// The _asset make detail view model.
        /// </summary>
        private AssetMakeDetailViewModel _assetMakeDetailViewModel;

        /// <summary>
        /// The _asset make assign feature view model.
        /// </summary>
        private AssetMakeAssignModelViewModel _assetMakeAssignModelViewModel;

        /// <summary>
        /// The _all asset make.
        /// </summary>
        private List<AssetClassesMakeRowItem> _allAssetMake;

        /// <summary>
        /// The _selected make.
        /// </summary>
        private AssetClassesMakeRowItem _selectedMake;

        /// <summary>
        /// The _is steps bulk update.
        /// </summary>
        private bool _isStepsBulkUpdate;

        /// <summary>
        /// The permission make detail.
        /// </summary>
        private Permission PermissionMakeDetail;

        /// <summary>
        /// The _source filter make.
        /// </summary>
        private List<FilteringDataItem> _sourceFilterMake;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetClassesMakeViewModel"/> class.
        /// </summary>
        public AssetClassesMakeViewModel()
        {
            this.InstanceGUID = Guid.NewGuid();
            this.AssetMakeDetailViewModel = new AssetMakeDetailViewModel();
            this.AssetMakeDetailViewModel.PropertyChanged += this.AssetClassesMakeDetailViewModel_PropertyChanged;
            this.AssetMakeAssignModelViewModel = new AssetMakeAssignModelViewModel();
            this.AssetMakeAssignModelViewModel.PropertyChanged += this.AssetClassesMakeDetailViewModel_PropertyChanged;
            this.Validator = new AsssetClassesMakeViewModelValidation();
        }

        #endregion

        #region Deleray
        /// <summary>
        /// The story board changed.
        /// </summary>
        /// <param name="storyBoard">
        /// The story board.
        /// </param>
        public delegate void StoryBoardChanged(string storyBoard);

        #endregion

        #region Event
        /// <summary>
        /// The step changed.
        /// </summary>
        public new event Action<string> StepChanged;

        /// <summary>
        /// The on story board changed.
        /// </summary>
        public event StoryBoardChanged OnStoryBoardChanged;
        #endregion
     
        #region Public Property

        /// <summary>
        /// Gets or sets the list item locks.
        /// </summary>
        public Dictionary<string, ItemLock> ListItemLocks { get; set; }

        /// <summary>
        /// Gets or sets the dynamic asset class make view model.
        /// </summary>
        public DynamicGridViewModel DynamicAssetClassMakeViewModel
        {
            get
            {
                return this._dynamicAssetClassMakeViewModel;
            }

            set
            {
                this.SetField(ref this._dynamicAssetClassMakeViewModel, value, () => this.DynamicAssetClassMakeViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the asset make detail view model.
        /// </summary>
        public AssetMakeDetailViewModel AssetMakeDetailViewModel
        {
            get
            {
                return this._assetMakeDetailViewModel;
            }

            set
            {
                this.SetField(ref this._assetMakeDetailViewModel, value, () => AssetMakeDetailViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the asset make assign model view model.
        /// </summary>
        public AssetMakeAssignModelViewModel AssetMakeAssignModelViewModel
        {
            get
            {
                return this._assetMakeAssignModelViewModel;
            }

            set
            {
                this.SetField(ref this._assetMakeAssignModelViewModel, value, () => AssetMakeAssignModelViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the all asset make.
        /// </summary>
        public List<AssetClassesMakeRowItem> AllAssetMake
        {
            get
            {
                return this._allAssetMake;
            }

            set
            {
                this.SetField(ref this._allAssetMake, value, () => this.AllAssetMake);
            }
        }

        /// <summary>
        /// Gets or sets the selected make.
        /// </summary>
        public AssetClassesMakeRowItem SelectedMake
        {
            get
            {
                return this._selectedMake;
            }

            set
            {
                this.SetField(ref this._selectedMake, value, () => this.SelectedMake);
            }
        }

        /// <summary>
        /// Gets or sets the source filter make.
        /// </summary>
        public List<FilteringDataItem> SourceFilterMake
        {
            get
            {
                return this._sourceFilterMake;
            }

            set
            {
                this.SetField(ref this._sourceFilterMake, value, () => this.SourceFilterMake);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is step bulk update.
        /// </summary>
        public bool IsStepBulkUpdate
        {
            get
            {
                return this._isStepsBulkUpdate;
            }

            set
            {
                this.SetField(ref this._isStepsBulkUpdate, value, () => this.IsStepBulkUpdate);
            }
        }

        #endregion

        #region Public Method
        /// <summary>
        /// The on step async.
        /// </summary>
        /// <param name="stepName">
        /// The step name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override async Task OnStepAsync(object stepName)
        {
            bool canProcess;
            this._currentEnumStep = (Asset.EnumSteps)Enum.Parse(typeof(Asset.EnumSteps), stepName.ToString());
            switch (this._currentEnumStep)
            {
                case Asset.EnumSteps.Start:
                    this.SetBusyAction(LoadingText);
                    this.GetPermission();
                    this._currentEnumStep = Asset.EnumSteps.Start;
                    this.ActiveViewModel = this;
                    await this.GetDataSourceForGrid();
                    this.SetActionCommandsAsync();
                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.MainViewState:
                    this._currentEnumStep = Asset.EnumSteps.MainViewState;
                    this.DynamicAssetClassMakeViewModel.IsEnableHoverRow = false;
                    this.DynamicAssetClassMakeViewModel.SelectedRows = new List<object>();
                    this.IsStepBulkUpdate = false;
                    this.StepChanged("MainViewState");
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.SelectedMake:
                    this.SetBusyAction(LoadingText);
                    if (this.ListItemLocks != null && this.ListItemLocks.Count > 0)
                    {
                        await this.UnLockAsync();
                    }

                    this.ClearNotifyErrors();
                    this.ValidateNotError();
                    this.SetBackgroundToNotEdit();
                    this._currentEnumStep = Asset.EnumSteps.SelectedMake;
                    this.StepChanged("MainContentState");
                    this.OnStoryBoardChanged(Asset.EnumSteps.DetailState.ToString());
                    this.GetDataSelectedMakeForDetailScreen();
                    await this.LoadDataForDetailScreen();
                    this.RaiseActionsWhenChangeStep(
                        EnumScreen.AssetClassesMake, 
                        Asset.EnumSteps.SelectedMake, 
                        this.SelectedMake);
                    this.SetActionCommandsAsync();
                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.Edit:
                    if (await this.LockAsync())
                    {
                        this.SetBackgroundToEdit();
                        this.AssetMakeDetailViewModel.DynamicAssignAssetTypeViewModel.IsEnableHoverRow = true;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesMake, Asset.EnumSteps.Edit);
                        this._currentEnumStep = Asset.EnumSteps.Edit;

                        this.IsCheckedOut = true;
                        this.SetActionCommandsAsync();
                    }

                    break;
                case Asset.EnumSteps.Add:
                    this.SetNewMakeClass();
                    this.SetBackgroundToEdit();
                    await this.AssetMakeDetailViewModel.GetDataSourceForAddScreen();
                    this.StepChanged("MainContentState");
                    this.OnStoryBoardChanged(Asset.EnumSteps.DetailState.ToString());
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesMake, Asset.EnumSteps.Add);
                    await this.OnStepAsync(Asset.EnumSteps.Edit);
                    this._currentEnumStep = Asset.EnumSteps.Add;
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.CancelAdd:
                    canProcess = await this.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this._currentEnumStep = Asset.EnumSteps.CancelAdd;
                        this.ValidateNotError();
                        this.ClearNotifyErrors();
                        this.DynamicAssetClassMakeViewModel.IsEnableHoverRow = false;
                        this.DynamicAssetClassMakeViewModel.SelectedItem = null;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesMake, Asset.EnumSteps.Cancel);
                        this.OnCancelNewItem(EnumScreen.AssetClassesMake);
                        this.StepChanged("MainViewState");
                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                        this.SetActionCommandsAsync();
                    }

                    break;
                case Asset.EnumSteps.Cancel:
                    canProcess = await this.CheckIfUnSavedChanges();

                    if (canProcess)
                    {
                        this.SetBusyAction(LoadingText);
                        if (this.SelectedMake != null && !this.SelectedMake.IsNewMakeClass)
                        {
                            await this.UnLockAsync();
                        }

                        this.SetBusyAction(LoadingText);
                        this.GetDataSelectedMakeForDetailScreen();
                        if (this.AssetMakeDetailViewModel != null)
                        {
                            await this.AssetMakeDetailViewModel.GetDataSourceForDetailScreen(this.SelectedMake);
                        }

                        this._currentEnumStep = Asset.EnumSteps.Cancel;
                        var assetMakeDetailViewModel = this.AssetMakeDetailViewModel;
                        if (assetMakeDetailViewModel != null)
                        {
                            assetMakeDetailViewModel.DynamicAssignAssetTypeViewModel.IsEnableHoverRow = false;
                            this.SetBackgroundToNotEdit();
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesMake, Asset.EnumSteps.Cancel);
                            this.GetDataSelectedMakeForDetailScreen();
                            this.StepChanged("MainContentState");
                            this.OnStoryBoardChanged(Asset.EnumSteps.DetailState.ToString());
                            this.ValidateNotError();
                            this.ClearNotifyErrors();
                            assetMakeDetailViewModel.IsCheckedOut = false;
                            assetMakeDetailViewModel.IsChanged = false;
                        }

                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                        this.ResetBusyAction();
                    }
                    else
                    {
                        this._currentEnumStep = Asset.EnumSteps.Edit;
                        this.Validate();
                        if (this.HasErrors)
                        {
                            this._currentEnumStep = Asset.EnumSteps.Error;
                        }
                    }

                    this.ResetBusyAction();
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.Save:
                    this.Validate();
                    if (this.HasErrors == false)
                    {
                        this.SetBusyAction(LoadingText);
                        this.ValidateNotError();
                        this.ClearNotifyErrors();
                        string previousMake = this.SelectedMake.Description;
                        EquipMake selectMake = await this.SaveAllDataForDetailScreen();
                        this.SelectedMake.EquipMakeId = selectMake.EquipMakeId;
                        this.SelectedMake.IsMouseHover = true;
                        this.AllAssetMake = await AssetClassesMakeFunctions.GetDataOnGrid();
                        if (!this.SelectedMake.IsNewMakeClass)
                        {
                            await this.UnLockAsync();
                            this.UpdateSourceForGrid(previousMake);
                        }
                        else
                        {
                            this._selectedMake.Enabled = selectMake.Enabled;
                            this.UpdateSourceForGrid();
                            this.IsCheckedOut = false;
                            this.AssetMakeDetailViewModel.IsCheckedOut = false;
                            this.IsChanged = false;
                        }

                        this.AssetMakeDetailViewModel.IsModeEdit = true;
                        this.SelectedMake.IsNewMakeClass = false;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesMake, Asset.EnumSteps.Save, selectMake.EquipMakeId);
                        await this.OnStepAsync(Asset.EnumSteps.SelectedMake.ToString());
                        this.ResetBusyAction();
                    }
                    else
                    {
                        this._currentEnumStep = Asset.EnumSteps.Error;
                        this.OnErrorHyperlinkSelected();
                    }

                    this.SetActionCommandsAsync();
                    break;

                case Asset.EnumSteps.SaveAndAdd:
                    this.Validate();
                    if (!this.HasErrors)
                    {
                        this._currentEnumStep = Asset.EnumSteps.SaveAndAdd;
                        await this.OnStepAsync(Asset.EnumSteps.Save.ToString());
                        await this.OnStepAsync(Asset.EnumSteps.Add.ToString());
                    }
                    else
                    {
                        this._currentEnumStep = Asset.EnumSteps.Error;
                        this.OnErrorHyperlinkSelected();
                    }

                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.BulkUpdate:
                    this._currentEnumStep = Asset.EnumSteps.BulkUpdate;
                    this.DynamicAssetClassMakeViewModel.IsEnableHoverRow = true;
                    this.IsStepBulkUpdate = true;
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.AssignModel:
                    if (this.DynamicAssetClassMakeViewModel.SelectedItems.Count() > 0)
                    {
                        if (await this.LockAsync())
                        {
                            this.SetBusyAction(LoadingText);
                            if (this.AssetMakeAssignModelViewModel != null)
                            {
                                await this.GetListModelItemsForAssignModelScreen();
                            }

                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesMake, Asset.EnumSteps.AssignModel);
                            this.SetBackgroundToEdit();
                            this.StepChanged("MainContentState");
                            this.OnStoryBoardChanged(Asset.EnumSteps.BulkState.ToString());
                            this.IsCheckedOut = true;
                            this.IsChanged = false;
                            this.AssetMakeAssignModelViewModel.IsCheckedOut = true;
                            this.SetActionCommandsAsync();
                            this.ResetBusyAction();
                        }
                    }
                    else
                    {
                        this.ShowMessageAsync("Select one or more record to Assign Model.", "Confirm - Asset Classes Make");
                    }

                    break;
                case Asset.EnumSteps.CancelBulkUpdate:
                    canProcess = await this.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this.SetBusyAction(LoadingText);
                        this.DynamicAssetClassMakeViewModel.IsEnableHoverRow = false;
                        this.DynamicAssetClassMakeViewModel.SelectedRows = new List<object>();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesMake, Asset.EnumSteps.Cancel);
                        this.OnCancelNewItem(EnumScreen.AssetClassesMake);
                        this._currentEnumStep = Asset.EnumSteps.CancelBulkUpdate;
                        this.IsStepBulkUpdate = false;
                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                        this.ResetBusyAction();
                    }
                    else
                    {
                        this._currentEnumStep = Asset.EnumSteps.BulkUpdate;
                    }

                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.EditBulkUpdate:
                    this.DynamicAssetClassMakeViewModel.IsEnableHoverRow = true;
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesMake, Asset.EnumSteps.Edit);
                    this._currentEnumStep = Asset.EnumSteps.EditBulkUpdate;
                    this.IsCheckedOut = true;
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.EditAssignModel:
                    if (await this.LockAsync())
                    {
                        this.SetBusyAction(LoadingText);
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesMake, Asset.EnumSteps.AssignModel);
                        this.SetBackgroundToEdit();
                        this._currentEnumStep = Asset.EnumSteps.EditAssignModel;
                        this.AssetMakeAssignModelViewModel.IsCheckedOut = true;
                        this.IsCheckedOut = true;
                        this.SetActionCommandsAsync();
                        this.ResetBusyAction();
                    }

                    break;
                case Asset.EnumSteps.CancelAssignModel:
                    canProcess = await this.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this.SetBusyAction(LoadingText);
                        await this.UnLockAsync();
                        await this.GetListModelItemsForAssignModelScreen();
                        this.DynamicAssetClassMakeViewModel.IsEnableHoverRow = false;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesMake, Asset.EnumSteps.CancelAssignModel);
                        this.SetBackgroundToNotEdit();
                        this._currentEnumStep = Asset.EnumSteps.CancelAssignModel;
                        this.AssetMakeAssignModelViewModel.IsCheckedOut = false;
                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                        this.ResetBusyAction();
                    }
                    else
                    {
                        this._currentEnumStep = Asset.EnumSteps.EditAssignModel;
                    }

                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.SaveAssignModel:
                    this.SetBusyAction(LoadingText);
                    await this.UnLockAsync();
                    await this.SaveAllDataForAssignModelScreen();
                    this._currentEnumStep = Asset.EnumSteps.SaveAssignModel;
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesMake, Asset.EnumSteps.SaveAssignModel);
                    this.SetBackgroundToNotEdit();
                    this.IsCheckedOut = false;
                    this.AssetMakeAssignModelViewModel.IsCheckedOut = false;
                    this.IsChanged = false;
                    this.SetActionCommandsAsync();
                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.Error:
                    // Show NotificationWindow when click Error button.
                    NotificationErrorView errorPopup = new NotificationErrorView();
                    NotificationErrorViewModel errorPopupViewModel = new NotificationErrorViewModel();
                    errorPopupViewModel.listCustomHyperlink = this.ListErrorHyperlink;
                    errorPopup.DataContext = errorPopupViewModel;
                    errorPopup.Style = (Style)Application.Current.FindResource("RadWindowStyleNew");
                    errorPopup.ShowDialog();
                    this.SetActionCommandsAsync();
                    break;
            }  
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public override void Dispose()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(async () =>
            {
                this._currentEnumStep = Asset.EnumSteps.Dispose;
                this.IsChanged = false;
                this.IsCheckedOut = false;
                await this.UnLockAsync();

                base.Dispose();
            }));
        }

        /// <summary>
        /// The get list model items for assign model screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetListModelItemsForAssignModelScreen()
        {
            if (this.DynamicAssetClassMakeViewModel.SelectedItems != null)
            {
                var allItemsSelected = new ObservableCollection<AssetClassesMakeRowItem>(this.DynamicAssetClassMakeViewModel.SelectedItems.Cast<AssetClassesMakeRowItem>());
                await this.AssetMakeAssignModelViewModel.GetListModelItems(allItemsSelected);
            }
        }

        /// <summary>
        /// The unlock item.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task UnlockItem()
        {
            return this.UnLockAsync();
        }

        /// <summary>
        /// The check content is editing.
        /// Calling from DocumentTab
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<bool> CheckContentEditing()
        {
            if (this.ActiveViewModel.IsCheckedOut && this.IsChanged)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        /// <summary>
        /// The check if un saved changes.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> CheckIfUnSavedChanges()
        {
            bool canProceed = true;
            if (this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Asset Make";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }

            return canProceed;
        }

        /// <summary>
        /// The grouped changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void GroupedChanged(object sender, object e)
        {
            if ((int)e == -1)
            {
                this.DynamicAssetClassMakeViewModel.MaxWidthGrid = 10000;
            }
            else
            {
                this.DynamicAssetClassMakeViewModel.MaxWidthGrid = (int)e + 1;
            }
        }

        #endregion

        #region Protected Method
        /// <summary>
        /// The set action commands async.
        /// </summary>
        protected override void SetActionCommandsAsync()
        {
            switch (this._currentEnumStep)
            {
                case Asset.EnumSteps.Start:
                case Asset.EnumSteps.MainViewState:
                case Asset.EnumSteps.CancelBulkUpdate:
                case Asset.EnumSteps.CancelAdd:
                    ObservableCollection<ActionCommand> tempActionCommands = new ObservableCollection<ActionCommand>();
                    if (this.PermissionMakeDetail.CanAdd && this.PermissionMakeDetail.CanEdit)
                    {
                        tempActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.Add.ToString(), Command = new Add() });
                    }

                    if (this.PermissionMakeDetail.CanEdit)
                    {
                        tempActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.BulkUpdate.ToString(), Command = new BulkUpdate() });
                    }

                    this.ActionCommands = tempActionCommands;
                    break;
                case Asset.EnumSteps.SelectedMake:
                case Asset.EnumSteps.Cancel:
                case Asset.EnumSteps.Save:
                    if (this.PermissionMakeDetail.CanEdit)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                            {
                                new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                            };
                    }

                    break;
                case Asset.EnumSteps.Edit:
                    if (this.PermissionMakeDetail.CanEdit)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                            {
                                new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() }
                            };
                    }

                    break;
                case Asset.EnumSteps.Add:
                    if (this.PermissionMakeDetail.CanEdit)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                            {
                                new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                new ActionCommand { Parameter = Asset.EnumSteps.SaveAndAdd.ToString(), Command = new SaveAndAdd() },
                                new ActionCommand { Parameter = Asset.EnumSteps.CancelAdd.ToString(), Command = new Cancel() }
                            };
                    }

                    break;
                case Asset.EnumSteps.BulkUpdate:
                    if (this.PermissionMakeDetail.CanEdit)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                            {
                                new ActionCommand { Parameter = Asset.EnumSteps.AssignModel.ToString(), Command = new AssignModel() },
                                new ActionCommand { Parameter = Asset.EnumSteps.CancelBulkUpdate.ToString(), Command = new Cancel() }
                            };
                    }

                    break;
                case Asset.EnumSteps.CancelAssignModel:
                case Asset.EnumSteps.SaveAssignModel:
                    if (this.PermissionMakeDetail.CanEdit)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                            {
                               new ActionCommand { Parameter = Asset.EnumSteps.EditAssignModel.ToString(), Command = new Edit() },
                            };
                    }

                    break;
                case Asset.EnumSteps.AssignModel:
                case Asset.EnumSteps.EditAssignModel:
                    if (this.PermissionMakeDetail.CanEdit)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                            {
                                new ActionCommand { Parameter = Asset.EnumSteps.SaveAssignModel.ToString(), Command = new Save() },
                                new ActionCommand { Parameter = Asset.EnumSteps.CancelAssignModel.ToString(), Command = new Cancel() }
                            };
                    }

                    break;
                case Asset.EnumSteps.Error:
                    if (this.SelectedMake.IsNewMakeClass)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                            new ActionCommand { Parameter = Asset.EnumSteps.SaveAndAdd.ToString(), Command = new SaveAndAdd() },
                            new ActionCommand { Parameter = Asset.EnumSteps.CancelAdd.ToString(), Command = new Cancel() },
                            new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() },
                        };
                    }
                    else
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                            new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                            new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() },
                        };
                    }

                    break;
                case Asset.EnumSteps.None:
                    break;
            }
        }

        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task UnLockAsync()
        {
            if (this.ListItemLocks != null)
            {
                foreach (var itemLock in this.ListItemLocks)
                {
                    await this.UnLockListItemsAsync(itemLock.Key, itemLock.Value);
                }

                this.ListItemLocks = new Dictionary<string, ItemLock>();
            }
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task<bool> LockAsync()
        {
            Dictionary<string, ItemLock> listItemLocks = new Dictionary<string, ItemLock>();
            this.ListItemLocks = new Dictionary<string, ItemLock>();
            bool result;
            int userId = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;

            if (this._currentEnumStep == Asset.EnumSteps.Edit && this.SelectedMake.EquipMakeId != 0)
            {
                listItemLocks.Add(
                    "EquipMake",
                    new ItemLock
                    {
                        ListUniqueIdentifier = new List<string> { this.SelectedMake.EquipMakeId.ToString() },
                        UserId = userId,
                        InstanceGUID = this.InstanceGUID
                    });
                listItemLocks.Add(
                    "xrefAssetTypeMake",
                    new ItemLock
                    {
                        ListUniqueIdentifier = new List<string> { "-1" },
                        UserId = userId,
                        InstanceGUID = this.InstanceGUID
                    });
            }
            else if (this._currentEnumStep == Asset.EnumSteps.AssignModel)
            {
                ObservableCollection<AssetClassesMakeRowItem> allItemsSelected = new ObservableCollection<AssetClassesMakeRowItem>(this.DynamicAssetClassMakeViewModel.SelectedItems.Cast<AssetClassesMakeRowItem>());

                List<int> listRecordLock = allItemsSelected.Select(x => x.EquipMakeId).ToList();

                listItemLocks.Add("EquipMake", new ItemLock { ListUniqueIdentifier = listRecordLock.ConvertAll(x => x.ToString()), UserId = userId, InstanceGUID = this.InstanceGUID });

                listItemLocks.Add(
                    "xrefAssetMakeModel",
                    new ItemLock
                    {
                        ListUniqueIdentifier = new List<string> { "-1" },
                        UserId = userId,
                        InstanceGUID = this.InstanceGUID
                    });
            }

            this.ListItemLocks = listItemLocks;
            foreach (var itemLock in listItemLocks)
            {
                result = await this.CheckLockedAsync(itemLock.Key, itemLock.Value);
                if (result == false)
                {
                    return false;
                }
            }

            foreach (var itemLock in listItemLocks)
            {
                result = await this.LockListItemsAsync(itemLock.Key, itemLock.Value);
            }

            this.IsCheckedOut = false;
            this.IsChanged = false;
            return true;
        }
        #endregion

        #region Private Method
        /// <summary>
        /// The get permission.
        /// </summary>
        private void GetPermission()
        {
            this.PermissionMakeDetail = Authorisation.GetPermission(Components.SystemManagementAssetClassesMake, Forms.AssetClassesMakeDetail);
        }

        /// <summary>
        /// The set new make class.
        /// </summary>
        private void SetNewMakeClass()
        {
            this.SelectedMake = new AssetClassesMakeRowItem 
            {
                IsNewMakeClass = true,
                Enabled = true,
            };
        } 

        /// <summary>
        /// The get data source for grid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataSourceForGrid()
        {
            List<FilteringDataItem> sourceEnable;
            if (this.DynamicAssetClassMakeViewModel == null)
            {
                this.DynamicAssetClassMakeViewModel = new DynamicGridViewModel(typeof(AssetClassesMakeRowItem));
            }

            this.DynamicAssetClassMakeViewModel.MaxWidthGrid = 400;
            this.DynamicAssetClassMakeViewModel.IsEnableHoverRow = false;
            this.DynamicAssetClassMakeViewModel.IsShowGroupPanel = true;
            this.AllAssetMake = await AssetClassesMakeFunctions.GetDataOnGrid();

            List<string> makes = this.AllAssetMake.Select(a => a.Description).Distinct().ToList();
            this.SourceFilterMake = (from f in makes
                           select new FilteringDataItem
                           {
                               Text = f,
                               IsSelected = true
                           }).Distinct().ToList();

            // Get data for Enable filter
            sourceEnable = new List<FilteringDataItem>
                                                       {
                                                           new FilteringDataItem
                                                               {
                                                                   Text = "True",
                                                                   IsSelected = true
                                                               },
                                                           new FilteringDataItem
                                                               {
                                                                   Text = "False",
                                                                   IsSelected = true
                                                               },
                                                       };
            this.DynamicAssetClassMakeViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "Description", Header = "MAKE", IsSelectedColumn = true, Width = 300, MinWidth = 70, FilteringTemplate = RadGridViewEnum.FilteringDataList, FilteringDataSource = this.SourceFilterMake },
                                                                                             new DynamicColumn { ColumnName = "Enabled",  Header = "ENABLED", FilteringTemplate = RadGridViewEnum.FilteringDataList, FilteringDataSource = sourceEnable, ColumnTemplate = RadGridViewEnum.ColumnCheckedTemplate, HeaderTextAlignment = TextAlignment.Center, Width = 86, MinWidth = 75},
                                                                                         };
            this.DynamicAssetClassMakeViewModel.FilteringGenerate = true;
            this.DynamicAssetClassMakeViewModel.GridDataRows = this.AllAssetMake.ToList<object>();
            this.DynamicAssetClassMakeViewModel.LoadRadGridView();
            this.DynamicAssetClassMakeViewModel.SelectedItemChanged = this.SelectedItemChanged;
            this.DynamicAssetClassMakeViewModel.GroupedItemChanged = this.GroupedChanged;
        }

        /// <summary>
        /// The update source for filter.
        /// </summary>
        /// <param name="make">
        /// The make.
        /// </param>
        private void UpdateSourceForGrid(string make = null)
        {
            DataRow editItem = null;
            if (this.SelectedMake.IsNewMakeClass)
            {
                this.SelectedMake.IsNewMakeClass = false;
                this.DynamicAssetClassMakeViewModel.InsertRow(0, this.SelectedMake);

                // add record for filter
                this.AddRecordFilter();
            }
            else
            {
                foreach (var m in this.DynamicAssetClassMakeViewModel.MembersTable.Rows)
                {
                    if (this.SelectedMake != null && this.SelectedMake.EquipMakeId.ToString(CultureInfo.InvariantCulture) == m["EquipMakeId"].ToString())
                    {
                        editItem = m;
                        break;
                    }
                }

                if (editItem != null)
                {
                    int index = this.DynamicAssetClassMakeViewModel.MembersTable.Rows.IndexOf(editItem);
                    this.DynamicAssetClassMakeViewModel.UpdateRow(index, this.SelectedMake);
                }

                // update filter
                this.UpdateRecordFilter(make);
            }
        }

        /// <summary>
        /// The update record filter.
        /// </summary>
        /// <param name="make">
        /// The make.
        /// </param>
        private void UpdateRecordFilter(string make)
        {
            if (!this.SourceFilterMake.Select(a => a.Text).Contains(this.AssetMakeDetailViewModel.AssetMakeName))
            {
                FilteringDataItem item = this.SourceFilterMake.FirstOrDefault(a => a.Text == make);
                int count = this.AllAssetMake.Count(a => a.Description == make);

                // if more than 1 similar item, don't remove
                if (item != null && count == 0)
                {
                    this.SourceFilterMake.Remove(item);
                }

                // add new item for filter
                this.SourceFilterMake.Add(new FilteringDataItem
                                                  {
                                                      Text = this.AssetMakeDetailViewModel.AssetMakeName,
                                                  });

                this.SourceFilterMake = this.SourceFilterMake.OrderBy(a => a.Text).ToList();
                this.DynamicAssetClassMakeViewModel.UpdateSourceForFilter(this.SourceFilterMake, 0, this.AssetMakeDetailViewModel.AssetMakeName);
            }
        }

        /// <summary>
        /// The add record filter.
        /// </summary>
        private void AddRecordFilter()
        {
            if (!this.SourceFilterMake.Select(a => a.Text).Contains(this.AssetMakeDetailViewModel.AssetMakeName))
            {
                // add new item for filter
                this.SourceFilterMake.Add(new FilteringDataItem
                                                  {
                                                      Text = this.AssetMakeDetailViewModel.AssetMakeName,
                                                  });
                this.SourceFilterMake = this.SourceFilterMake.OrderBy(a => a.Text).ToList();
                this.DynamicAssetClassMakeViewModel.AddSourceForFilter(this.SourceFilterMake, 0, this.AssetMakeDetailViewModel.AssetMakeName);
            }
        }

        /// <summary>
        /// The load data for detail screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task LoadDataForDetailScreen()
        {
            if (this.AssetMakeDetailViewModel != null)
            {
                await this.AssetMakeDetailViewModel.GetDataSourceForDetailScreen(this.SelectedMake);
            }
        }

        /// <summary>
        /// The asset classes make detail view model_ property changed.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetClassesMakeDetailViewModel_PropertyChanged(object obj, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut)
            {
                this.IsChanged = true;
                /*if (e.PropertyName.IndexOf("IsMakeEnable", StringComparison.Ordinal) != -1)
                  {
                      this.AssetMakeDetailViewModel.ResetGridWhenChangeEnable(this.SelectedMake, this.AssetMakeDetailViewModel.IsMakeEnable);
                  }*/
            }
        }

        /// <summary>
        /// The get data selected make for detail screen.
        /// </summary>
        private void GetDataSelectedMakeForDetailScreen()
        {
            if (this.DynamicAssetClassMakeViewModel.SelectedItem != null)
            {
                this.SelectedMake = this.DynamicAssetClassMakeViewModel.SelectedItem as AssetClassesMakeRowItem;
            }
        }
       
        /// <summary>
        /// The selected item changed.
        /// </summary>
        /// <param name="value">
        /// The o.
        /// </param>
        private async void SelectedItemChanged(object value)
        {
            if (this.DynamicAssetClassMakeViewModel.IsEnableHoverRow != true)
            {
                if (!this.IsStepBulkUpdate)
                {
                    if (value != null)
                    {
                        await this.OnStepAsync(Asset.EnumSteps.SelectedMake);
                    }
                }
            }
        }

        /// <summary>
        /// The save all data for assign model screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SaveAllDataForAssignModelScreen()
        {
            var allItemsSelected = new ObservableCollection<AssetClassesMakeRowItem>(this.DynamicAssetClassMakeViewModel.SelectedItems.Cast<AssetClassesMakeRowItem>());
            AssetClassesMakeFunctions.SaveAllForAssignModelScreen(
                allItemsSelected,
                this.AssetMakeAssignModelViewModel.ListItemsDragDrop.GroupDragDropSource);
        }

        /// <summary>
        /// The save all data for detail screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<EquipMake> SaveAllDataForDetailScreen()
        {
            var allItemsSelected = new ObservableCollection<AssetTypeMakeRowItem>();
            if (this.AssetMakeDetailViewModel.DynamicAssignAssetTypeViewModel.SelectedItems != null)
            {
                allItemsSelected =
                    new ObservableCollection<AssetTypeMakeRowItem>(
                        this.AssetMakeDetailViewModel.DynamicAssignAssetTypeViewModel.SelectedItems
                            .Cast<AssetTypeMakeRowItem>());
            }

            if (!this.SelectedMake.IsNewMakeClass)
            {
                this.SelectedMake.Description = this.AssetMakeDetailViewModel.AssetMakeName;
                this.SelectedMake.Enabled = this.AssetMakeDetailViewModel.IsMakeEnable;
                this.SelectedMake.IsAutoAssignToAnyNewType = this.AssetMakeDetailViewModel.IsAutoAssignToAnyNewType;
                await AssetClassesMakeFunctions.SaveUpdateMakeAsync(this.SelectedMake, allItemsSelected);
            }
            else
            {
                this.SelectedMake.Description = this.AssetMakeDetailViewModel.AssetMakeName;
                this.SelectedMake.Enabled = this.AssetMakeDetailViewModel.IsMakeEnable;
                this.SelectedMake.IsAutoAssignToAnyNewType = this.AssetMakeDetailViewModel.IsAutoAssignToAnyNewType;
                await AssetClassesMakeFunctions.SaveAddMakeAsync(this.SelectedMake, allItemsSelected);
            }

            return AssetClassesMakeFunctions.GetAssetMakeByName(this.SelectedMake.Description);
        }

        /// <summary>
        /// The set background to edit.
        /// </summary>
        private void SetBackgroundToEdit()
        {
            this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
            if (this.AssetMakeDetailViewModel != null)
            {
                this.AssetMakeDetailViewModel.GridStyle =
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                this.AssetMakeDetailViewModel.IsCheckedOut = true;
            }

            if (this.AssetMakeAssignModelViewModel != null)
            {
                this.AssetMakeAssignModelViewModel.GridStyle =
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                this.AssetMakeAssignModelViewModel.IsCheckedOut = true;
            }
        }

        /// <summary>
        /// The set background to not edit.
        /// </summary>
        private void SetBackgroundToNotEdit()
        {
            this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
            this.IsCheckedOut = false;
            this.IsChanged = false;
            if (this.AssetMakeDetailViewModel != null)
            {
                this.AssetMakeDetailViewModel.GridStyle =
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.AssetMakeDetailViewModel.IsCheckedOut = false;
            }

            if (this.AssetMakeAssignModelViewModel != null)
            {
                this.AssetMakeAssignModelViewModel.GridStyle =
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.AssetMakeAssignModelViewModel.IsCheckedOut = false;
            }
        }
        #endregion

        #region Other

        #endregion
    }
}
