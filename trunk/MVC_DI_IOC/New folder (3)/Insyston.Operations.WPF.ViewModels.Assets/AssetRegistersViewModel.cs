// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetRegistersViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetRegistersViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets
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

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Bussiness.Assets;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.Locking.Helpers;
    using Insyston.Operations.Model;
    using Insyston.Operations.Security;
    using Insyston.Operations.Security.Model;
    using Insyston.Operations.WPF.ViewModel.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetRegisters;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;

    using global::WPF.DataTable.Models;

    using WPFDynamic.ViewModels.Controls;

    /// <summary>
    /// The asset registers view model.
    /// </summary>
    public class AssetRegistersViewModel : ViewModelUseCaseBase
    {
        #region Variables

        /// <summary>
        /// The loading text.
        /// </summary>
        private const string LoadingText = "Please Wait Loading ...";

        /// <summary>
        /// The permission make detail.
        /// </summary>
        private Permission PermissionRegisterDetail;

        /// <summary>
        /// The _all asset register.
        /// </summary>
        private List<AssetRegisterRowItem> _allAssetRegister;

        /// <summary>
        /// The _selected register.
        /// </summary>
        private AssetRegisterRowItem _selectedRegister;

        /// <summary>
        /// The _report name.
        /// </summary>
        private string _reportName;

        /// <summary>
        /// The _current enum step.
        /// </summary>
        private Asset.EnumSteps _currentEnumStep;

        /// <summary>
        /// The _asset registers detail view model.
        /// </summary>
        private AssetRegistersDetailViewModel _assetRegistersDetailViewModel;

        /// <summary>
        /// The _dynamic asset register view model.
        /// </summary>
        private DynamicGridViewModel _dynamicAssetRegisterViewModel;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetRegistersViewModel"/> class.
        /// </summary>
        public AssetRegistersViewModel()
        {
            this.InstanceGUID = Guid.NewGuid();
            this.PropertyChanged += this.AssetRegisterViewModel_PropertyChanged;
            this.AssetRegistersDetailViewModel = new AssetRegistersDetailViewModel();
        }
        #endregion

        #region Event, Delegate

        /// <summary>
        /// The story board changed.
        /// </summary>
        /// <param name="storyBoard">
        /// The story board.
        /// </param>
        public delegate void StoryBoardChanged(string storyBoard);

        /// <summary>
        /// The on story board changed.
        /// </summary>
        public event StoryBoardChanged OnStoryBoardChanged;

        /// <summary>
        /// The step changed.
        /// </summary>
        public event Action<string> StepChanged;

        #endregion

        #region Properties

        public AssetRegisterRowItem DefaultAssetRegister { get; set; }

        /// <summary>
        /// Gets or sets the dynamic asset register view model.
        /// </summary>
        public DynamicGridViewModel DynamicAssetRegisterViewModel
        {
            get { return this._dynamicAssetRegisterViewModel; }
            set { this.SetField(ref this._dynamicAssetRegisterViewModel, value, () => this.DynamicAssetRegisterViewModel); }
        }

        /// <summary>
        /// Gets or sets the asset registers detail view model.
        /// </summary>
        public AssetRegistersDetailViewModel AssetRegistersDetailViewModel
        {
            get { return this._assetRegistersDetailViewModel; }
            set { this.SetField(ref this._assetRegistersDetailViewModel, value, () => this.AssetRegistersDetailViewModel); }
        }

        /// <summary>
        /// Gets or sets the all asset register.
        /// </summary>
        public List<AssetRegisterRowItem> AllAssetRegister
        {
            get { return this._allAssetRegister; }
            set { this.SetField(ref this._allAssetRegister, value, () => this.AllAssetRegister); }
        }

        /// <summary>
        /// Gets or sets the selected register.
        /// </summary>
        public AssetRegisterRowItem SelectedRegister
        {
            get
            {
                return this._selectedRegister;
            }

            set
            {
                if (!this.IsAdd)
                {
                    this.SetSelectedRegisterAsync(value);
                }
                else
                {
                    this.SetField(ref this._selectedRegister, value, () => this.SelectedRegister);
                }
            }
        }

        /// <summary>
        /// Gets or sets the report name.
        /// </summary>
        public string ReportName
        {
            get { return this._reportName; }
            set { this.SetField(ref this._reportName, value, () => this.ReportName); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is add.
        /// </summary>
        public bool IsAdd { get; set; }

        /// <summary>
        /// Gets or sets the list item locks.
        /// </summary>
        private Dictionary<string, ItemLock> ListItemLocks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether in edit summary grid.
        /// </summary>
        private bool InEditSummaryGrid { get; set; }

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
                    this._currentEnumStep = Asset.EnumSteps.Start;
                    this.ActiveViewModel = this;
                    this.IsAdd = false;
                    this.GetPermission();
                    await this.GetDataSourceForGrid();
                    this.RaiseSelectedItemChanged();
                    this.SetActionCommandsAsync();
                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.SelectRegister:
                    this._currentEnumStep = Asset.EnumSteps.SelectRegister;
                    this.StepChanged("MainContentState");
                    this.AssetRegistersDetailViewModel.ClearNotifyErrors();
                    this.ValidateNotError();
                    this.OnStoryBoardChanged(Asset.EnumSteps.DetailState.ToString());
                    await AssetRegistersDetailViewModel.GetDetailDataSource(this.SelectedRegister.ID);
                    this.SetBackgroundToNotEdit();
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetRegisters, Asset.EnumSteps.SelectRegister, this.SelectedRegister);
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.Add:
                    this.IsAdd = true;
                    this.SetNewRegister();
                    this.SetBackgroundToEdit();
                    this.StepChanged("MainContentState");
                    await AssetRegistersDetailViewModel.GetDataSourceForAddScreen();
                    this.AssetRegistersDetailViewModel.DynamicRegisterAssignViewModel.IsEnableHoverRow = true;
                    this.OnStoryBoardChanged(Asset.EnumSteps.DetailState.ToString());
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetRegisters, Asset.EnumSteps.Add);
                    await this.OnStepAsync(Asset.EnumSteps.EditRegisterDetail);
                    this.IsAdd = false;
                    this._currentEnumStep = Asset.EnumSteps.Add;
                    this.AssetRegistersDetailViewModel.IsChanged = false;
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.MainViewState:
                    this._currentEnumStep = Asset.EnumSteps.MainViewState;
                    this.DynamicAssetRegisterViewModel.IsEnableHoverRow = false;
                    this.DynamicAssetRegisterViewModel.SelectedRows = new List<object>();

                    this.StepChanged("MainViewState");
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.CancelAdd:
                    canProcess = await this.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this._currentEnumStep = Asset.EnumSteps.CancelAdd;
                        this.ValidateNotError();
                        this.AssetRegistersDetailViewModel.ClearNotifyErrors();
                        this.DynamicAssetRegisterViewModel.IsEnableHoverRow = false;
                        this.DynamicAssetRegisterViewModel.SelectedItem = null;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetRegisters, Asset.EnumSteps.Cancel);
                        this.OnCancelNewItem(EnumScreen.AssetRegisters);
                        this.StepChanged("MainViewState");
                        this.AssetRegistersDetailViewModel.IsCheckedOut = false;
                        this.AssetRegistersDetailViewModel.IsChanged = false;
                        this.SetActionCommandsAsync();
                        if (this.SelectedRegister.IsNewRegister)
                        {
                            this.AssetRegistersDetailViewModel.AssetRegisterLocations =
                                new ObservableCollection<AssetRegisterLocationRowItem>();
                        }
                    }

                    break;
                case Asset.EnumSteps.EditRegisterSummary:
                    this.SetBusyAction(LoadingText);
                    this._currentEnumStep = Asset.EnumSteps.EditRegisterSummary;
                    if (await this.LockAsync() == false)
                    {
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetRegisters, Asset.EnumSteps.ItemLocked);
                    }
                    else
                    {
                        
                        this.InEditSummaryGrid = true;
                        this.SetBackgroundToEdit();
                        this.IsCheckedOut = true;
                        var defaultReg = this.DynamicAssetRegisterViewModel.MembersTable.Rows.FirstOrDefault(a =>
                            {
                                var assetRegisterRowItem = a.RowObject as AssetRegisterRowItem;
                                return assetRegisterRowItem != null && assetRegisterRowItem.IsRadioSelected;
                            });
                        if (defaultReg != null)
                        {
                            this.DefaultAssetRegister = (AssetRegisterRowItem)defaultReg.RowObject;
                        }

                        this.DynamicAssetRegisterViewModel.IsEnableRadioButtonRow = true;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetRegisters, Asset.EnumSteps.Edit);
                        this.SetActionCommandsAsync();
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.CancelRegisterSummary:
                    this.SetBusyAction(LoadingText);
                    this._currentEnumStep = Asset.EnumSteps.CancelRegisterSummary;
                    canProcess = await this.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        await this.UnLockAsync();
                        this.DynamicAssetRegisterViewModel.MaxWidthGrid = 700;
                        this.DynamicAssetRegisterViewModel.IsEnableHoverRow = false;
                        this.DynamicAssetRegisterViewModel.IsShowGroupPanel = true;
                        this.DynamicAssetRegisterViewModel.IsEnableRadioButtonRow = false;

                        // make sure that only one radio button is checked belong to DefaultAssetRegister
                        this.DefaultAssetRegister.IsRadioSelected = false;
                        this.DynamicAssetRegisterViewModel.SelectedRadioRows = null;
                        this.DynamicAssetRegisterViewModel.SelectedRadioRows = this.DefaultAssetRegister;
                        this.DefaultAssetRegister.IsRadioSelected = true;
                        this.DynamicAssetRegisterViewModel.SelectedRadioItems = this.DefaultAssetRegister;
                        this.DynamicAssetRegisterViewModelOnSelectedRadioChange(this.DefaultAssetRegister, null);

                        // update default row
                        this.InEditSummaryGrid = false;
                        this.SetBackgroundToNotEdit();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetRegisters, Asset.EnumSteps.Cancel);
                        this.SetActionCommandsAsync();
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.SaveRegisterSummary:
                    this.SetBusyAction(LoadingText);
                    await this.UnLockAsync();
                    this._currentEnumStep = Asset.EnumSteps.SaveRegisterSummary;
                    var item = this.DynamicAssetRegisterViewModel.SelectedRadioItems as AssetRegisterRowItem;
                    await AssetRegisterFunction.SaveSumaryGridEdit(item);
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetRegisters, Asset.EnumSteps.SaveRegisterSummary);
                    this.DynamicAssetRegisterViewModel.MaxWidthGrid = 700;
                    this.DynamicAssetRegisterViewModel.IsEnableHoverRow = false;
                    this.DynamicAssetRegisterViewModel.IsShowGroupPanel = true;
                    this.DynamicAssetRegisterViewModel.IsEnableRadioButtonRow = false;

                    // make sure that only one radio button is checked belong to item
                    if (item != null)
                    {
                        item.IsRadioSelected = false;
                        this.DynamicAssetRegisterViewModel.SelectedRadioRows = null;
                        this.DynamicAssetRegisterViewModel.SelectedRadioRows = item;
                        item.IsRadioSelected = true;
                        this.DynamicAssetRegisterViewModelOnSelectedRadioChange(item, null);
                    }

                    this.InEditSummaryGrid = false;
                    this.SetActionCommandsAsync();
                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.EditRegisterDetail:
                    this._currentEnumStep = Asset.EnumSteps.EditRegisterDetail;
                    if (await this.LockAsync() == false)
                    {
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetRegisters, Asset.EnumSteps.ItemLocked);
                        this.SetBackgroundToNotEdit();
                        await this.OnStepAsync(Asset.EnumSteps.SelectRegister);
                    }
                    else
                    {
                        this.SetBackgroundToEdit();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetRegisters, Asset.EnumSteps.Edit);
                        this.AssetRegistersDetailViewModel.DynamicRegisterAssignViewModel.IsEnableHoverRow = true;
                        this.AssetRegistersDetailViewModel.DynamicRegisterLocationViewModel.IsEnableHoverRow = true;
                        this.AssetRegistersDetailViewModel.IsCheckedOut = true;
                        this.AssetRegistersDetailViewModel.DynamicRegisterLocationViewModel.ToolbarVisibilityChanged =
                        Visibility.Visible;
                        this.IsCheckedOut = false;
                        SetActionCommandsAsync();
                    }

                    break;
                case Asset.EnumSteps.CancelRegisterDetail:
                    this._currentEnumStep = Asset.EnumSteps.CancelRegisterDetail;
                    canProcess = await this.AssetRegistersDetailViewModel.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this.SetBusyAction(LoadingText);
                        if (!this.SelectedRegister.IsNewRegister)
                        {
                            await this.UnLockAsync();
                            await AssetRegistersDetailViewModel.GetDetailDataSource(this.SelectedRegister.ID);
                            this.AssetRegistersDetailViewModel.DynamicRegisterAssignViewModel.IsEnableHoverRow = false;
                            this.AssetRegistersDetailViewModel.IsCheckedOut = false;
                        }
                        else
                        {
                            this.AssetRegistersDetailViewModel.IsCheckedOut = false;
                            this.AssetRegistersDetailViewModel.IsChanged = false;
                        }

                        this.ValidateNotError();
                        this.AssetRegistersDetailViewModel.ClearNotifyErrors();
                        this.SetBackgroundToNotEdit();
                        this.AssetRegistersDetailViewModel.IsChanged = false;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetRegisters, Asset.EnumSteps.Cancel);
                        this.AssetRegistersDetailViewModel.DynamicRegisterLocationViewModel.ToolbarVisibilityChanged =
                        Visibility.Collapsed;
                        if (this.SelectedRegister.IsNewRegister)
                        {
                            this.AssetRegistersDetailViewModel.AssetRegisterLocations = new ObservableCollection<AssetRegisterLocationRowItem>();
                            this.OnCancelNewItem(EnumScreen.AssetRegisters);
                            await this.OnStepAsync(Asset.EnumSteps.MainViewState);
                        }

                        this.SetActionCommandsAsync();
                        this.ResetBusyAction();
                    }

                    break;
                case Asset.EnumSteps.SaveRegisterDetail:
                    this.SetBusyAction(LoadingText);
                    this._currentEnumStep = Asset.EnumSteps.SaveRegisterDetail;
                    this.AssetRegistersDetailViewModel.Validate();
                    if (this.AssetRegistersDetailViewModel.HasErrors == false)
                    {
                        this.ValidateNotError();
                        this.AssetRegistersDetailViewModel.ClearNotifyErrors();
                        AssetRegister selectedRegister = await this.SaveAllDataForDetailScreen();
                        this.AssetRegistersDetailViewModel.IsCheckedOut = false;
                        this.AssetRegistersDetailViewModel.IsChanged = false;
                        this.AssetRegistersDetailViewModel.DynamicRegisterLocationViewModel.ToolbarVisibilityChanged = Visibility.Collapsed;
                        if (!this.IsAdd)
                        {
                            await this.UnLockAsync();
                        }

                        this.SetBackgroundToNotEdit();
                        if (AssetRegisterFunction.GetCategory() == 7)
                        {
                            this.AllAssetRegister = await AssetRegisterFunction.GetDataOnGridInternalCompany();
                        }
                        else
                        {
                            if (AssetRegisterFunction.GetCategory() == 5)
                            {
                                this.AllAssetRegister = await AssetRegisterFunction.GetDataOnGridFinancier();
                            }
                        }

                        // Update selected 
                        this.SelectedRegister.ID = selectedRegister.ID;
                        this.SelectedRegister.RegisterName = selectedRegister.RegisterName;

                        // Update record on grid
                        this.UpdateDataRegisters();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetRegisters, Asset.EnumSteps.Save, selectedRegister.ID);
                        await AssetRegistersDetailViewModel.GetDetailDataSource(selectedRegister.ID);
                        this.AssetRegistersDetailViewModel.DynamicRegisterAssignViewModel.IsEnableHoverRow = false;
                        await this.OnStepAsync(Asset.EnumSteps.SelectRegister);
                        this.AssetRegistersDetailViewModel.IsEditMode = true;
                    }
                    else
                    {
                        this._currentEnumStep = Asset.EnumSteps.Error;
                        this.SetActionCommandsAsync();
                        this.ListErrorHyperlink = this.AssetRegistersDetailViewModel.ListErrorHyperlink;
                        this.OnErrorHyperlinkSelected();
                    }

                    this.SetActionCommandsAsync();
                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.Error:
                    this._currentEnumStep = Asset.EnumSteps.Error;
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
        /// The set selected register async.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task SetSelectedRegisterAsync(AssetRegisterRowItem value)
        {
            bool canProceed = true;
            if (this.AssetRegistersDetailViewModel.IsCheckedOut && this.AssetRegistersDetailViewModel.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Assets Register";
                confirm.DataContext = confirmViewModel;
                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
                else
                {
                    this.AssetRegistersDetailViewModel.IsChanged = false;
                }
            }

            if (canProceed)
            {
                this.ValidateNotError();
                this.AssetRegistersDetailViewModel.IsChanged = false;
                this.AssetRegistersDetailViewModel.IsCheckedOut = false;
                if (value != null)
                {
                    if (this.SelectedRegister != null)
                    {
                        await this.UnLockAsync();
                        this.IsAdd = false;
                    }
                }

                this.SetField(ref this._selectedRegister, value, () => this.SelectedRegister);
                if (value != null)
                {
                    if (this.StepChanged != null && this._currentEnumStep != Asset.EnumSteps.EditRegisterSummary)
                    {
                        await this.OnStepAsync(Asset.EnumSteps.SelectRegister);
                    }
                }

                this.Validate();
            }
        }

        /// <summary>
        /// The raise selected item changed.
        /// </summary>
        public void RaiseSelectedItemChanged()
        {
            if (this.DynamicAssetRegisterViewModel != null)
            {
                this.DynamicAssetRegisterViewModel.SelectedItemChanged = this.GridSelectedItemChanged;
            }
        }

        /// <summary>
        /// The grid selected item changed.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public async void GridSelectedItemChanged(object item)
        {
            bool isHover = this.DynamicAssetRegisterViewModel.IsEnableHoverRow;
            if (!isHover && item != null && !this.InEditSummaryGrid)
            {
                if (this.DynamicAssetRegisterViewModel.SelectedItem == null)
                {
                    return;
                }

                AssetRegisterRowItem selectedRegiser =
                    this.DynamicAssetRegisterViewModel.SelectedItem as AssetRegisterRowItem;
                if (selectedRegiser != null)
                {
                    this._selectedRegister =
                        this.AllAssetRegister.FirstOrDefault(
                            register => register.ID == selectedRegiser.ID);
                    if (this._selectedRegister != null)
                    {
                        this._selectedRegister.IsDefault = selectedRegiser.IsRadioSelected;
                        this._selectedRegister.IsRadioSelected = selectedRegiser.IsRadioSelected;
                    }
                }

                await this.OnStepAsync(Asset.EnumSteps.SelectRegister);
            }
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
            if (this.AssetRegistersDetailViewModel.IsChanged || this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Asset Register";
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
        /// The check content editing.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<bool> CheckContentEditing()
        {
            if ((this.AssetRegistersDetailViewModel.IsCheckedOut && this.AssetRegistersDetailViewModel.IsChanged)
                || (this.IsCheckedOut && this.IsChanged))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
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

        #endregion

        #region Protected Methods

        /// <summary>
        /// The set action commands async.
        /// </summary>
        protected override void SetActionCommandsAsync()
        {
            if (this.PermissionRegisterDetail.CanEdit)
            {
                switch (this._currentEnumStep)
                {
                    case Asset.EnumSteps.Start:
                    case Asset.EnumSteps.MainViewState:
                    case Asset.EnumSteps.CancelAdd:
                    case Asset.EnumSteps.CancelRegisterSummary:
                    case Asset.EnumSteps.SaveRegisterSummary:
                        ObservableCollection<ActionCommand> tempActionCommands = new ObservableCollection<ActionCommand>();
                        if (this.PermissionRegisterDetail.CanAdd)
                        {
                            tempActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.Add.ToString(), Command = new Add() });
                        }

                        tempActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.EditRegisterSummary.ToString(), Command = new Edit() });
                        this.ActionCommands = tempActionCommands;
                        break;
                    case Asset.EnumSteps.Add:
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                        new ActionCommand
                        {
                            Parameter = Asset.EnumSteps.SaveRegisterDetail.ToString(),
                            Command = new Save()
                        },
                        new ActionCommand { Parameter = Asset.EnumSteps.CancelAdd.ToString(), Command = new Cancel() }
                        };
                        break;
                    case Asset.EnumSteps.EditRegisterSummary:
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand
                        {
                            Parameter = Asset.EnumSteps.SaveRegisterSummary.ToString(),
                            Command = new Save()
                        },
                        new ActionCommand
                        {
                            Parameter = Asset.EnumSteps.CancelRegisterSummary.ToString(),
                            Command = new Cancel()
                        }
                    };
                        break;
                    case Asset.EnumSteps.SelectRegister:
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand
                        {
                            Parameter = Asset.EnumSteps.EditRegisterDetail.ToString(),
                            Command = new Edit()
                        }
                    };
                        break;
                    case Asset.EnumSteps.EditRegisterDetail:
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand
                        {
                            Parameter = Asset.EnumSteps.SaveRegisterDetail.ToString(),
                            Command = new Save()
                        },
                        new ActionCommand
                        {
                            Parameter = Asset.EnumSteps.CancelRegisterDetail.ToString(),
                            Command = new Cancel()
                        }
                    };
                        break;
                    case Asset.EnumSteps.CancelRegisterDetail:
                    case Asset.EnumSteps.SaveRegisterDetail:
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand
                        {
                            Parameter = Asset.EnumSteps.EditRegisterDetail.ToString(),
                            Command = new Edit()
                        }
                    };
                        break;
                    case Asset.EnumSteps.Error:
                        if (this.SelectedRegister.IsNewRegister)
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = Asset.EnumSteps.SaveRegisterDetail.ToString(), Command = new Save() },
                            new ActionCommand { Parameter = Asset.EnumSteps.CancelAdd.ToString(), Command = new Cancel() },
                            new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() },
                        };
                        }
                        else
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = Asset.EnumSteps.SaveRegisterDetail.ToString(), Command = new Save() },
                            new ActionCommand { Parameter = Asset.EnumSteps.CancelRegisterDetail.ToString(), Command = new Cancel() },
                            new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() },
                        };
                        }

                        break;
                }
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
            if (!this.IsAdd)
            {
                Dictionary<string, ItemLock> listItemLocks = new Dictionary<string, ItemLock>();
                this.ListItemLocks = new Dictionary<string, ItemLock>();
                bool result = true;
                int userId = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;

                if (this.SelectedRegister != null)
                {
                    listItemLocks.Add(
                        "AssetRegister",
                        new ItemLock
                            {
                                ListUniqueIdentifier = new List<string> { this.SelectedRegister.ID.ToString(CultureInfo.InvariantCulture) },
                                UserId = userId,
                                InstanceGUID = this.InstanceGUID
                            });
                    listItemLocks.Add(
                    "AssetRegisterFinancier",
                    new ItemLock
                        {
                            ListUniqueIdentifier = new List<string> { "-1" },
                            UserId = userId,
                            InstanceGUID = this.InstanceGUID
                        });

                    listItemLocks.Add(
                        "AssetRegisterInternalCompany",
                        new ItemLock
                            {
                                ListUniqueIdentifier = new List<string> { "-1" },
                                UserId = userId,
                                InstanceGUID = this.InstanceGUID
                            });
                    listItemLocks.Add(
                        "AssetRegisterLocation",
                        new ItemLock
                            {
                                ListUniqueIdentifier = new List<string> { "-1" },
                                UserId = userId,
                                InstanceGUID = this.InstanceGUID
                            });
                }

                if (!this.InEditSummaryGrid && this.SelectedRegister == null)
                {
                    List<string> listItemLock = new List<string>();

                    foreach (var item in this.AllAssetRegister)
                    {
                        listItemLock.Add(item.ID.ToString(CultureInfo.InvariantCulture));
                    }

                    listItemLocks.Add(
                        "AssetRegister",
                        new ItemLock
                            {
                                ListUniqueIdentifier = listItemLock,
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

                return result;
            }

            this.AssetRegistersDetailViewModel.IsCheckedOut = false;
            return true;
        }

        #endregion

        #region Private Method
        /// <summary>
        /// The set new register.
        /// </summary>
        private void SetNewRegister()
        {
            this.SelectedRegister = new AssetRegisterRowItem
            {
                RegisterName = string.Empty,
                InternalOnly = false,
                IsNewRegister = true,
                IsDefault = false,
                IsRadioSelected = false,
            };
        }

        /// <summary>
        /// The get permission.
        /// </summary>
        private void GetPermission()
        {
            this.PermissionRegisterDetail = Authorisation.GetPermission(Components.SystemManagementAssetRegister, Forms.RegisterDetail);
        }

        /// <summary>
        /// The get data source for grid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataSourceForGrid()
        {
            if (this.DynamicAssetRegisterViewModel == null)
            {
                this.DynamicAssetRegisterViewModel = new DynamicGridViewModel(typeof(AssetRegisterRowItem));
            }

            this.DynamicAssetRegisterViewModel.MaxWidthGrid = 700;
            this.DynamicAssetRegisterViewModel.IsEnableHoverRow = false;
            this.DynamicAssetRegisterViewModel.IsShowGroupPanel = true;
            this.DynamicAssetRegisterViewModel.IsEnableRadioButtonRow = false;
            if (AssetRegisterFunction.GetCategory() == 5 && await Authorisation.IsModuleInstalledAsync(Modules.GLModule))
            {
                this.AllAssetRegister = await AssetRegisterFunction.GetDataOnGridFinancier();
                this.ReportName = "Financier";
            }
            else
            {
                this.AllAssetRegister = await AssetRegisterFunction.GetDataOnGridInternalCompany();
                this.ReportName = "Internal Company";
            }

            this.DynamicAssetRegisterViewModel.GridColumns = new List<DynamicColumn>

                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "ID", Width = 25, Header = "ID", MinWidth = 30, HeaderTextAlignment = TextAlignment.Center, TextAlignment = TextAlignment.Center },
                                                                                             new DynamicColumn { ColumnName = "RegisterName", Width = 220, Header = "REGISTER NAME", MinWidth = 85, HeaderTextAlignment = TextAlignment.Left, TextAlignment = TextAlignment.Left },
                                                                                             new DynamicColumn { ColumnName = "ReportName", Width = 200, Header = this.ReportName.ToUpper(), MinWidth = 95, HeaderTextAlignment = TextAlignment.Left, TextAlignment = TextAlignment.Left },
                                                                                             new DynamicColumn { ColumnName = "InternalOnly", Width = 90, Header = "INTERNAL ONLY", MinWidth = 85, ColumnTemplate = ViewModels.Common.Enums.RadGridViewEnum.ColumnCheckedTemplate, HeaderTextAlignment = TextAlignment.Center, TextAlignment = TextAlignment.Center },
                                                                                             new DynamicColumn { ColumnName = "IsDefault", Width = 65, Header = "IS DEFAULT", MinWidth = 65, IsRadioSelectedColumn = true, HeaderTextAlignment = TextAlignment.Center, TextAlignment = TextAlignment.Center },
                                                                                         };

            this.DynamicAssetRegisterViewModel.FilteringGenerate = true;
            this.DynamicAssetRegisterViewModel.GridDataRows = this.AllAssetRegister.ToList<object>();
            this.DynamicAssetRegisterViewModel.LoadRadGridView();
            this.DynamicAssetRegisterViewModel.GroupedItemChanged = this.GroupedItemChanged;

            // change selected radio button, update source to make sure, only one radio button is checked in grid
            this.DynamicAssetRegisterViewModel.SelectedRadioChange -= this.DynamicAssetRegisterViewModelOnSelectedRadioChange;
            this.DynamicAssetRegisterViewModel.SelectedRadioChange += this.DynamicAssetRegisterViewModelOnSelectedRadioChange;
        }

        /// <summary>
        /// The dynamic asset register view model on selected radio change.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void DynamicAssetRegisterViewModelOnSelectedRadioChange(object sender, EventArgs eventArgs)
        {
            AssetRegisterRowItem item = (AssetRegisterRowItem)sender;
            foreach (var row in this.DynamicAssetRegisterViewModel.MembersTable.Rows)
            {
                var data = (AssetRegisterRowItem)row.RowObject;
                if (data.ID == item.ID)
                {
                    data.IsRadioSelected = item.IsRadioSelected;
                }
                else
                {
                    data.IsRadioSelected = false;
                }
            }
        }

        /// <summary>
        /// The update data feature types async.
        /// </summary>
        private void UpdateDataRegisters()
        {
            DataRow editItem = null;
            if (this.SelectedRegister.IsNewRegister)
            {
                this.SelectedRegister.IsNewRegister = false;
                this.DynamicAssetRegisterViewModel.InsertRow(0, this.SelectedRegister);

                // add record for filter
            }
            else
            {
                foreach (var m in this.DynamicAssetRegisterViewModel.MembersTable.Rows)
                {
                    if (this.SelectedRegister != null && this.SelectedRegister.ID.ToString(CultureInfo.InvariantCulture) == m["ID"].ToString())
                    {
                        editItem = m;
                        break;
                    }
                }

                if (editItem != null)
                {
                    int index = this.DynamicAssetRegisterViewModel.MembersTable.Rows.IndexOf(editItem);
                    this.DynamicAssetRegisterViewModel.UpdateRow(index, this.SelectedRegister);
                }
            }
        }

        /// <summary>
        /// The update default.
        /// </summary>
        /// <param name="defaultRegister">
        /// The default register.
        /// </param>
        private void UpdateDefault(AssetRegisterRowItem defaultRegister)
        {
            DataRow editItem = null;
            foreach (var m in this.DynamicAssetRegisterViewModel.MembersTable.Rows)
            {
                if (defaultRegister != null && defaultRegister.ID.ToString(CultureInfo.InvariantCulture) == m["ID"].ToString())
                {
                    editItem = m;
                    break;
                }
            }

            if (editItem != null)
            {
                int index = this.DynamicAssetRegisterViewModel.MembersTable.Rows.IndexOf(editItem);
                this.DynamicAssetRegisterViewModel.UpdateRow(index, defaultRegister);
            }
        }

        /// <summary>
        /// The grouped item changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GroupedItemChanged(object sender, object e)
        {
            if ((int)e == -1)
            {
                this.DynamicAssetRegisterViewModel.MaxWidthGrid = 10000;
            }
            else
            {
                this.DynamicAssetRegisterViewModel.MaxWidthGrid = (int)e + 1;
            }
        }

        /// <summary>
        /// The get selected row.
        /// </summary>
        /// <returns>
        /// The <see cref="Collection"/>.
        /// </returns>
        private ObservableCollection<AssetRelationRowItem> GetSelectedRow()
        {
            if (this.AssetRegistersDetailViewModel.DynamicRegisterAssignViewModel.SelectedItems != null)
            {
                var allItemsSelected =
                    new ObservableCollection<AssetRelationRowItem>(
                        this.AssetRegistersDetailViewModel.DynamicRegisterAssignViewModel.SelectedItems
                            .Cast<AssetRelationRowItem>());
                return allItemsSelected;
            }

            return null;
        }

        /// <summary>
        /// The save all data for detail screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<AssetRegister> SaveAllDataForDetailScreen()
        {
            ObservableCollection<AssetRelationRowItem> selectedReport = this.GetSelectedRow();
            if (selectedReport != null)
            {
                if (!this.SelectedRegister.IsNewRegister)
                {
                    this.SelectedRegister.RegisterName = AssetRegistersDetailViewModel.RegisterName;
                    this.SelectedRegister.InternalOnly = AssetRegistersDetailViewModel.IsInternalOnly;
                    if (AssetRegistersDetailViewModel.SelectDefault != null)
                    {
                        this.SelectedRegister.ReportId = AssetRegistersDetailViewModel.SelectDefault.NodeId;
                        this.SelectedRegister.ReportName = AssetRegistersDetailViewModel.SelectDefault.NodeName;
                    }

                    List<AssetRegisterLocationRowItem> newItems = new List<AssetRegisterLocationRowItem>();
                    List<AssetRegisterLocationRowItem> existItems = new List<AssetRegisterLocationRowItem>();
                    List<AssetRegisterLocationRowItem> removeItems = new List<AssetRegisterLocationRowItem>();

                    existItems.AddRange(this.AssetRegistersDetailViewModel.AssetRegisterLocations.Where(i => this.AssetRegistersDetailViewModel.OldRegisterLocations.Select(f => f.GuidId).Contains(i.GuidId)).ToList());

                    removeItems.AddRange(this.AssetRegistersDetailViewModel.OldRegisterLocations.Where(f => !existItems.Select(a => a.GuidId).Contains(f.GuidId)).ToList());

                    newItems.AddRange(
                    this.AssetRegistersDetailViewModel.AssetRegisterLocations.Where(
                        i =>
                        !existItems.Select(a => a.GuidId).Contains(i.GuidId) && !removeItems.Select(a => a.GuidId).Contains(i.GuidId))
                        .ToList());

                    AssetRegisterFunction.RemoveAssetLocationItems(removeItems);

                    AssetRegisterFunction.UpdateAssetLocationItems(existItems, this.AssetRegistersDetailViewModel.OldRegisterLocations);               

                    AssetRegisterFunction.UpdateAssetLocationItems(newItems, null);

                    await AssetRegisterFunction.SaveDetailAsync(this.SelectedRegister, selectedReport);
                }
                else
                {
                    this.SelectedRegister.RegisterName = AssetRegistersDetailViewModel.RegisterName;
                    this.SelectedRegister.InternalOnly = AssetRegistersDetailViewModel.IsInternalOnly;
                    if (this.AssetRegistersDetailViewModel.SelectDefault != null)
                    {
                        this.SelectedRegister.ReportId = this.AssetRegistersDetailViewModel.SelectDefault.NodeId;
                        this.SelectedRegister.ReportName = this.AssetRegistersDetailViewModel.SelectDefault.NodeName;
                    }
                    await AssetRegisterFunction.SaveNewDetailAsync(this.SelectedRegister, selectedReport, this.AssetRegistersDetailViewModel.AssetRegisterLocations);
                }
            }

            return AssetRegisterFunction.GetAssetRegisterByName(this.SelectedRegister.RegisterName);
        }

        /// <summary>
        /// The set background to edit.
        /// </summary>
        private void SetBackgroundToEdit()
        {
            this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
            if (this.AssetRegistersDetailViewModel != null)
            {
                this.AssetRegistersDetailViewModel.GridStyle =
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                this.AssetRegistersDetailViewModel.IsCheckedOut = true;
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
            if (this.AssetRegistersDetailViewModel != null)
            {
                this.AssetRegistersDetailViewModel.GridStyle =
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.AssetRegistersDetailViewModel.IsCheckedOut = false;
            }
        }

        /// <summary>
        /// The asset register view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetRegisterViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut
                && (e.PropertyName.IndexOf("DynamicAssetRegisterViewModel.IsCheckItemChanged", StringComparison.Ordinal) != -1))
            {
                this.IsChanged = true;
            }
        }

        #endregion
    }
}

