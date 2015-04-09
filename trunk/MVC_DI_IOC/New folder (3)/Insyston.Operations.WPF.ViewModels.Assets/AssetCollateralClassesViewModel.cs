// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetCollateralClassesViewModel.cs" company="Insyston">
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
    using System.Windows.Threading;

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Bussiness.Assets.AssetCollateralClasses;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.Locking.Helpers;
    using Insyston.Operations.Logging;
    using Insyston.Operations.Model;
    using Insyston.Operations.Security;
    using Insyston.Operations.Security.Model;
    using Insyston.Operations.WPF.ViewModel.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetCollateralClasses;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Enums;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;
    using Insyston.Operations.WPF.ViewModels.Common.Model.Filtering;

    using global::WPF.DataTable.Models;

    using WPFDynamic.ViewModels.Controls;

    /// <summary>
    /// The asset collateral classes view model.
    /// </summary>
    public class AssetCollateralClassesViewModel : ViewModelUseCaseBase
    {
        #region Variables

        /// <summary>
        /// The _dynamic collateral view model.
        /// </summary>
        private DynamicGridViewModel _dynamicCollateralViewModel;

        /// <summary>
        /// The _asset collateral assign type view model.
        /// </summary>
        private AssetCollateralAssignTypeViewModel _assetCollateralAssignTypeViewModel;

        /// <summary>
        /// The _asset collateral detail view model.
        /// </summary>
        private AssetCollateralDetailViewModel _assetCollateralDetailViewModel;

        /// <summary>
        /// The _asset collateral type view model.
        /// </summary>
        private AssetCollateralTypeViewModel _assetCollateralTypeViewModel ;

        /// <summary>
        /// The loading text.
        /// </summary>
        private const string LoadingText = "Please Wait Loading ...";

        /// <summary>
        /// Gets or sets the list item locks.
        /// </summary>
        public Dictionary<string, ItemLock> ListItemLocks { get; set; }

        /// <summary>
        /// The _current enum step.
        /// </summary>
        private Asset.EnumSteps _currentEnumStep;

        /// <summary>
        /// The _selected collateral.
        /// </summary>
        private PPSRCollateralClass _selectedCollateral;

        /// <summary>
        /// The on story board changed.
        /// </summary>
        public event StoryBoardChanged OnStoryBoardChanged;

        /// <summary>
        /// The _all collateral classes.
        /// </summary>
        public ObservableCollection<PPSRCollateralClass> _allCollateralClasses;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetCollateralClassesViewModel"/> class.
        /// </summary>
        public AssetCollateralClassesViewModel()
        {
            this.InstanceGUID = Guid.NewGuid();
            this._assetCollateralAssignTypeViewModel = new AssetCollateralAssignTypeViewModel();
            this._assetCollateralDetailViewModel = new AssetCollateralDetailViewModel();
            this._assetCollateralTypeViewModel = new AssetCollateralTypeViewModel();
            this.AssetCollateralTypeViewModel.PropertyChanged += this.AssetCollateralTypeViewModelOnPropertyChanged;
            this.AssetCollateralAssignTypeViewModel.SelectedItemChanged =
                this.AssetCollateralAssignTypeViewModelOnPropertyChanged;        
        }

        #endregion

        #region Properties

        /// <summary>
        /// The _permission collateral detail.
        /// </summary>
        Permission _permissionCollateralDetail;

        /// <summary>
        /// The _permission collateral type.
        /// </summary>
        Permission _permissionCollateralType;

        /// <summary>
        /// Gets the current step.
        /// </summary>
        public Asset.EnumSteps CurrentStep { get; private set; }

        /// <summary>
        /// Gets or sets the current tab.
        /// </summary>
        public Asset.EnumSteps CurrentTab { get; set; }

        /// <summary>
        /// Gets or sets the dynamic collateral view model.
        /// </summary>
        public DynamicGridViewModel DynamicCollateralViewModel
        {
            get
            {
                return this._dynamicCollateralViewModel;
            }

            set
            {
                this.SetField(ref this._dynamicCollateralViewModel, value, () => this.DynamicCollateralViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the asset collateral assign type view model.
        /// </summary>
        public AssetCollateralAssignTypeViewModel AssetCollateralAssignTypeViewModel
        {
            get
            {
                return this._assetCollateralAssignTypeViewModel;
            }

            set
            {
                this.SetField(ref this._assetCollateralAssignTypeViewModel, value, () => this.AssetCollateralAssignTypeViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the asset collateral detail view model.
        /// </summary>
        public AssetCollateralDetailViewModel AssetCollateralDetailViewModel
        {
            get
            {
                return this._assetCollateralDetailViewModel;
            }

            set
            {
                this.SetField(ref this._assetCollateralDetailViewModel, value, () => this.AssetCollateralDetailViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the asset collateral type view model.
        /// </summary>
        public AssetCollateralTypeViewModel AssetCollateralTypeViewModel
        {
            get
            {
                return this._assetCollateralTypeViewModel;
            }

            set
            {
                this.SetField(ref this._assetCollateralTypeViewModel, value, () => this.AssetCollateralTypeViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the selected collateral.
        /// </summary>
        public PPSRCollateralClass SelectedCollateral
        {
            get
            {
                return this._selectedCollateral;
            }

            set
            {
                this.SetSelectedCollateralClasses(value);
            } 
        }

        /// <summary>
        /// Gets or sets the all collateral classes.
        /// </summary>
        public ObservableCollection<PPSRCollateralClass> AllCollateralClasses
        {
            get
            {
                return this._allCollateralClasses;
            }

            set
            {
                this.SetField(ref this._allCollateralClasses, value, () => this.AllCollateralClasses);
            }
        }

        // Check in bulk update mode

        /// <summary>
        /// Gets or sets a value indicating whether is bulk update.
        /// </summary>
        public bool IsBulkUpdate { get; set; }

        #endregion

        #region Override Method

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
            if (this._currentEnumStep != Asset.EnumSteps.DetailsState && this._currentEnumStep != Asset.EnumSteps.AssignedToState)
            {
                this.CurrentStep = this._currentEnumStep;
            }

            switch (this._currentEnumStep)
            {
                case Asset.EnumSteps.Start:
                    this.SetBusyAction(LoadingText);
                    this.GetPermssion();
                    this.ActiveViewModel = this;
                    await this.PopulateAllCollateralsForViewAsync();
                    this.RaiseSelectedItemChanged();
                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.SelectedCollateral:
                    this.SetBusyAction(LoadingText);
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(Asset.EnumSteps.GridContentState.ToString());
                    }

                    this.SetGridStype(false);
                    this.SetValueForAssetCollateralDetail(this.SelectedCollateral);

                    await this.GetDataSourceForAssetTypesScreen();
                    await this.AssetCollateralDetailViewModel.PopulateCollateralClasses(this.SelectedCollateral);
                    if (this.AssetCollateralDetailViewModel.ListCollateralDetail != null)
                    {
                        this.NotifySelectComboboxChanged();
                    }

                    this.SetActionCommandsAsync();
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetCollateralClasses, EnumSteps.SelectedCollateral, this.SelectedCollateral);
                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.GridContentState:
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                    }

                    break;
                case Asset.EnumSteps.DetailsState:
                    this.OnStepChanged(Asset.EnumSteps.DetailsState.ToString());
                    this.CurrentTab = Asset.EnumSteps.DetailsState;
                    break;
                case Asset.EnumSteps.AssignedToState:
                    this.OnStepChanged(Asset.EnumSteps.AssignedToState.ToString());
                    this.CurrentTab = Asset.EnumSteps.AssignedToState;
                    break;
                case Asset.EnumSteps.GridSummaryState:
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                    }

                    break;
                case Asset.EnumSteps.AssignFeatureState:
                    this.AssetCollateralAssignTypeViewModel.SetBusyAction(LoadingText);
                    var items = new ObservableCollection<AssetCollateralRowItem>(this.DynamicCollateralViewModel.SelectedItems.Cast<AssetCollateralRowItem>());
                    if (items.Count != 0)
                    {
                        bool result;
                        List<int> list = new List<int>(items.Select(x => x.CollateralClassID));
                        result = await this.LockBulkUpdateAssetFeaturesAsync(list);
                        if (!result)
                        {
                            // Raise event to visible FormMenu if record selected is locked.
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetCollateralClasses, EnumSteps.ItemLocked);

                            // Change background if record selected is locked when editing.
                            this.SetGridStype(false);

                            return;
                        }

                        this.IsCheckedOut = true;
                        this.SetIsCheckedOut();
                        this.SetGridStype(true);
                        var selectedCollaterals = new ObservableCollection<AssetCollateralRowItem>(this.DynamicCollateralViewModel.SelectedItems.Cast<AssetCollateralRowItem>());
                        await this.AssetCollateralAssignTypeViewModel.GetListCollateralItems(selectedCollaterals);
                        this.AssetCollateralAssignTypeViewModel.SelectedItemChanged +=
                            this.AssetClassesTypeDetailViewModel_PropertyChanged;
                        if (this.OnStoryBoardChanged != null)
                        {
                            this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                        }

                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetCollateralClasses, EnumSteps.EditBulkUpdate);
                        this.SetActionCommandsAsync();
                    }
                    else
                    {
                        this.ShowMessageAsync("Select one or more record to Assign Features.", "Confirm - Asset Feature");
                        this.CurrentStep = Asset.EnumSteps.None;
                    }

                    this.AssetCollateralAssignTypeViewModel.ResetBusyAction();
                    break;
                case Asset.EnumSteps.BulkUpdate:
                    this.DynamicCollateralViewModel.IsEnableHoverRow = true;
                    this.IsBulkUpdate = true;
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.Edit:
                    if (this.SelectedCollateral != null && await this.LockBulkUpdateAssetFeaturesAsync(new List<int> { this.SelectedCollateral.CollateralClassID }, true) == false)
                    {
                        // Raise event to visible FormMenu if record selected is locked.
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetCollateralClasses, EnumSteps.ItemLocked);

                        // Change background if record selected is locked when editing.
                        this.SetGridStype(false);

                        return;
                    }

                    this.IsCheckedOut = true;
                    this.SetIsCheckedOut();
                    this.SetGridStype(true);
                    if (this.AssetCollateralTypeViewModel.IsCheckedOut)
                    {
                        this.AssetCollateralTypeViewModel.DynamicAssignAssetTypeViewModel.IsEnableHoverRow = true;
                    }

                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetCollateralClasses, EnumSteps.Edit);
                    break;
                case Asset.EnumSteps.EditBulkUpdate:
                    items = new ObservableCollection<AssetCollateralRowItem>(this.DynamicCollateralViewModel.SelectedItems.Cast<AssetCollateralRowItem>());
                    if (items.Count != 0)
                    {
                        this.IsBulkUpdate = true;
                        bool result;
                        List<int> list = new List<int>(items.Select(x => x.CollateralClassID));
                        result = await this.LockBulkUpdateAssetFeaturesAsync(list);
                        if (!result)
                        {
                            // Raise event to visible FormMenu if record selected is locked.
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetCollateralClasses, EnumSteps.ItemLocked);

                            // Change background if record selected is locked when editing.
                            this.SetGridStype(false);

                            return;
                        }
                    }

                    this.IsCheckedOut = true;
                    this.SetIsCheckedOut();
                    this.SetGridStype(true);
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetCollateralClasses, EnumSteps.EditBulkUpdate);
                    break;
                case Asset.EnumSteps.SaveBulkUpdate:
                    try
                    {
                        this.AssetCollateralAssignTypeViewModel.SetBusyAction(LoadingText);
                        await this.SaveCollateralBulkUpdateAsync();
                        await this.UnLockBulkUpdateAssetFeaturesAsync();
                        this.IsBulkUpdate = false;
                        this.IsCheckedOut = false;
                        this.SetIsCheckedOut();
                        this.IsChanged = false;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetCollateralClasses, EnumSteps.SaveAssignTypes);
                        this.SetGridStype(false);
                        this.AssetCollateralAssignTypeViewModel.ResetBusyAction();
                    }
                    catch (Exception exc)
                    {
                        ExceptionLogger.WriteLog(exc);
                        this.ShowErrorMessage("Error encountered while Saving Asset Feature Bulk Update.", "Asset Features");
                    }

                    break;
                case Asset.EnumSteps.Save:
                    try
                    {
                        this.Validate();
                        if (this.HasErrors == false)
                        {
                            if (!this.SelectedCollateral.IsNewCollateral)
                            {
                                await this.UnLockBulkUpdateAssetFeaturesAsync();
                            }
                            else
                            {
                                this.IsCheckedOut = false;
                                this.SetIsCheckedOut();
                                this.IsChanged = false;
                            }

                            this.IsCheckedOut = false;
                            this.SetIsCheckedOut();
                            this.IsChanged = false;
                            await this.SaveCollateralClassesAsync();

                            this.ValidateNotError();

                            await this.OnStepAsync(Asset.EnumSteps.SelectedCollateral);
                            
                            // this.AllCollateralClasses = new ObservableCollection<PPSRCollateralClass>(await AssetCollateralClassesFunction.GetAllPPSRCollateralClassesAsync());
                            this.UpdateDataCollateralsAsync();
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetCollateralClasses, EnumSteps.Save, this.SelectedCollateral.CollateralClassID);
                        }
                        else
                        {
                            this._currentEnumStep = Asset.EnumSteps.Error;
                            this.SetActionCommandsAsync();
                            this.OnErrorHyperlinkSelected();
                        }
                    }
                    catch (Exception exc)
                    {
                        ExceptionLogger.WriteLog(exc);
                        this.ShowErrorMessage("Error encountered while Saving Asset Feature Details.", "Asset Features");
                    }

                    break;
                case Asset.EnumSteps.Cancel:
                    canProcess = await this.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this.AssetCollateralDetailViewModel.ClearNotifyErrors();
                        this.ValidateNotError();

                        // Just do UnLockAsync if not in mode Add.
                        if (this.SelectedCollateral != null && !this.SelectedCollateral.IsNewCollateral)
                        {
                            await this.UnLockBulkUpdateAssetFeaturesAsync();
                        }
                        else
                        {
                            this.IsCheckedOut = false;
                            this.SetIsCheckedOut();
                            this.IsChanged = false;
                        }

                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                        this.SetIsCheckedOut();
                        this.SetGridStype(false);
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetCollateralClasses, EnumSteps.Cancel);
                        if (this.SelectedCollateral == null || (this.SelectedCollateral != null && this.SelectedCollateral.IsNewCollateral))
                        {
                            this.OnCancelNewItem(EnumScreen.AssetCollateralClasses);
                            if (this.OnStoryBoardChanged != null)
                            {
                                this.OnStoryBoardChanged(Asset.EnumSteps.GridSummaryState.ToString());
                                this._currentEnumStep = Asset.EnumSteps.GridSummaryState;
                            }
                        }
                        else
                        {
                            await this.OnStepAsync(EnumSteps.SelectedCollateral);
                        }
                    }
                    else
                    {
                        this.CurrentStep = Asset.EnumSteps.None;
                    }

                    break;
                case Asset.EnumSteps.CancelAssignFeature:
                    canProcess = await this.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        await this.UnLockBulkUpdateAssetFeaturesAsync();
                        this.IsChanged = false;
                        var selectedCollateralsCancel = new ObservableCollection<AssetCollateralRowItem>(this.DynamicCollateralViewModel.SelectedItems.Cast<AssetCollateralRowItem>());

                        this.IsCheckedOut = false;
                        this.SetIsCheckedOut();
                        this.IsChanged = false;
                        this.IsBulkUpdate = false;
                        await this.AssetCollateralAssignTypeViewModel.GetListCollateralItems(selectedCollateralsCancel);
                        this.IsCheckedOut = false;
                        this.SetIsCheckedOut();
                        this.SetGridStype(false);
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetCollateralClasses, EnumSteps.CancelAssignFeature);
                    }
                    else
                    {
                        this.CurrentStep = Asset.EnumSteps.None;
                    }

                    break;
                case Asset.EnumSteps.CancelBulkUpdate:
                    canProcess = await this.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this.IsBulkUpdate = false;
                        this.DynamicCollateralViewModel.SelectedItem = null;
                        this.DynamicCollateralViewModel.IsEnableHoverRow = false;
                        this.DynamicCollateralViewModel.SelectedRows = new List<object>();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetCollateralClasses, EnumSteps.CancelBulkUpdate);
                    }

                    break;
                case Asset.EnumSteps.Error:
                    // Show NotificationWindow when click Error button.
                    NotificationErrorView errorPopup = new NotificationErrorView();
                    NotificationErrorViewModel errorPopupViewModel = new NotificationErrorViewModel();
                    errorPopupViewModel.listCustomHyperlink = this.ListErrorHyperlink;
                    errorPopup.DataContext = errorPopupViewModel;

                    errorPopup.Style = (Style)Application.Current.FindResource("RadWindowStyleNew");

                    errorPopup.ShowDialog();
                    break;
            }

            this.SetActionCommandsAsync();
        }

        #endregion

        #region Public Method
        /// <summary>
        /// The unlock item.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task UnlockItem()
        {
            return this.UnLockBulkUpdateAssetFeaturesAsync();
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
            if (this.IsCheckedOut && this.IsChanged)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        /// <summary>
        /// The raise selected item changed.
        /// </summary>
        public void RaiseSelectedItemChanged()
        {
            if (this.DynamicCollateralViewModel != null)
            {
                this.DynamicCollateralViewModel.SelectedItemChanged = this.GridSelectedItemChanged;
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
            bool isHover = this.DynamicCollateralViewModel.IsEnableHoverRow;
            if (!isHover && item != null)
            {
                if (this.DynamicCollateralViewModel.SelectedItem == null)
                {
                    return;
                }

                AssetCollateralRowItem selectedCollateral =
                    this.DynamicCollateralViewModel.SelectedItem as AssetCollateralRowItem;
                this.SelectedCollateral = this.AllCollateralClasses.FirstOrDefault(collateral =>
                        selectedCollateral != null && collateral.CollateralClassID == selectedCollateral.CollateralClassID);

                if (this._selectedCollateral != null)
                {
                    this._selectedCollateral.MotorVehicleRego =
                     await AssetCollateralClassesFunction.GetListMotorVehicleRego(this._selectedCollateral.CollateralClassID);
                    this._selectedCollateral.Aircraft =
                            await AssetCollateralClassesFunction.GetListAircraft(this._selectedCollateral.CollateralClassID);
                    if (this._selectedCollateral.MotorVehicleRego == null)
                    {
                        this._selectedCollateral.MotorVehicleRego = new PPSRCollateralClassMotorVehicle();
                    }

                    if (this._selectedCollateral.Aircraft == null)
                    {
                        this._selectedCollateral.Aircraft = new PPSRCollateralClassAircraft();
                    }
                }

                await this.OnStepAsync(Asset.EnumSteps.SelectedCollateral);
            }
        }

        /// <summary>
        /// The check if un saved changes.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
#pragma warning disable 1998
        public async Task<bool> CheckIfUnSavedChanges()
#pragma warning restore 1998
        {
            bool canProceed = true;
            if (this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Asset Collateral";
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
        /// The dispose.
        /// </summary>
        public override void Dispose()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                this.CurrentStep = Asset.EnumSteps.Dispose;
                this.IsChanged = false;
                this.IsCheckedOut = false;
                if (this.ListItemLocks != null && this.ListItemLocks.Count > 0)
                {
                    // ReSharper disable once CSharpWarnings::CS4014
                    this.UnLockBulkUpdateAssetFeaturesAsync();
                }

                base.Dispose();
            }));
        }
        #endregion

        #region Protect Method

        /// <summary>
        /// The set action commands async.
        /// </summary>
        protected override void SetActionCommandsAsync()
        {
            if (this.CurrentStep == Asset.EnumSteps.Start || this.CurrentStep == Asset.EnumSteps.GridSummaryState || this.CurrentStep == Asset.EnumSteps.CancelBulkUpdate)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>();

                if (this._permissionCollateralType.CanEdit)
                {
                    this.ActionCommands.Add(
                        new ActionCommand
                        {
                            Parameter = Asset.EnumSteps.BulkUpdate.ToString(),
                            Command = new BulkUpdate()
                        });
                }
            }
            else if (this.CurrentStep == Asset.EnumSteps.CancelAssignFeature)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                {
                    new ActionCommand { Parameter = Asset.EnumSteps.EditBulkUpdate.ToString(), Command = new Edit() },
                };
            }
            else if (this.CurrentStep == Asset.EnumSteps.BulkUpdate)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = Asset.EnumSteps.AssignFeatureState.ToString(), Command = new AssignType() },
                            new ActionCommand { Parameter = Asset.EnumSteps.CancelBulkUpdate.ToString(), Command = new Cancel() },
                        };
            }
            else if (this.CurrentStep == Asset.EnumSteps.AssignFeatureState || this.CurrentStep == Asset.EnumSteps.EditBulkUpdate)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = Asset.EnumSteps.SaveBulkUpdate.ToString(), Command = new Save() },
                            new ActionCommand { Parameter = Asset.EnumSteps.CancelAssignFeature.ToString(), Command = new Cancel() },
                        };
            }
            else if (this.CurrentStep == Asset.EnumSteps.SaveBulkUpdate)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = Asset.EnumSteps.EditBulkUpdate.ToString(), Command = new Edit() },
                        };
            }
            else if (!this.IsBulkUpdate)
            {
                switch (this.CurrentTab)
                {
                    case Asset.EnumSteps.DetailsState:
                        if (!this.IsCheckedOut)
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>();
                            if (this._permissionCollateralDetail.CanEdit)
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                    {
                                        new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                                    };
                            }
                        }

                        if (this.IsCheckedOut)
                        {
                            if (this._permissionCollateralDetail.CanEdit)
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                        {
                                            new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                            new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                                        };
                            }
                            else
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>();
                            }
                        }

                        if (this.SelectedCollateral == null || (this.SelectedCollateral != null && !this.SelectedCollateral.IsNewCollateral))
                        {
                            if (this.CurrentStep == Asset.EnumSteps.Save || this.CurrentStep == Asset.EnumSteps.Cancel || this.CurrentStep == Asset.EnumSteps.Delete)
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                                };
                            }
                        }

                        if (this.CurrentStep == Asset.EnumSteps.Error)
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() },
                                };
                        }

                        break;
                    case Asset.EnumSteps.AssignedToState:
                        if (!this.IsCheckedOut)
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>();
                            if (this._permissionCollateralType.CanEdit)
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                    {
                                        new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                                    };
                            }
                        }

                        if (this.IsCheckedOut)
                        {
                            if (this._permissionCollateralType.CanEdit)
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                        new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                                };
                            }
                            else
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>();
                            }
                        }

                        if (this.SelectedCollateral != null && !this.SelectedCollateral.IsNewCollateral)
                        {
                            if (this.CurrentStep == Asset.EnumSteps.Save || this.CurrentStep == Asset.EnumSteps.Cancel || this.CurrentStep == Asset.EnumSteps.Delete)
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                                };
                            }
                        }

                        if (this.CurrentStep == Asset.EnumSteps.Error)
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() },
                                };
                        }

                        break;
                }
            }

            if (this.CurrentStep != Asset.EnumSteps.Error && this.ActionCommands != null
                && this.ActionCommands.Count(a => a.Parameter == Asset.EnumSteps.Error.ToString()) != 0)
            {
                this.ActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() });
            }
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
        /// The lock bulk update asset features async.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="isNotBulkUpdate">
        /// The is not bulk update.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected async Task<bool> LockBulkUpdateAssetFeaturesAsync(List<int> value, bool isNotBulkUpdate = false)
        {
            Dictionary<string, ItemLock> listItemLocks = new Dictionary<string, ItemLock>();
            this.ListItemLocks = new Dictionary<string, ItemLock>();
            bool result = true;
            int userId = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;

            if (isNotBulkUpdate && value != null)
            {
                listItemLocks.Add(
                    "PPSRCollateralClass",
                    new ItemLock
                        {
                            ListUniqueIdentifier = new List<string> { value.FirstOrDefault().ToString(CultureInfo.InvariantCulture) },
                    UserId = userId,
                    InstanceGUID = this.InstanceGUID
                });
                listItemLocks.Add(
                    "PPSRCollateralClassAircraft",
                    new ItemLock
                        {
                            ListUniqueIdentifier = new List<string> { value.FirstOrDefault().ToString(CultureInfo.InvariantCulture) },
                    UserId = userId,
                    InstanceGUID = this.InstanceGUID
                });
                listItemLocks.Add(
                    "PPSRCollateralClassMotorVehicle",
                    new ItemLock
                        {
                            ListUniqueIdentifier = new List<string> { value.FirstOrDefault().ToString(CultureInfo.InvariantCulture) },
                    UserId = userId,
                    InstanceGUID = this.InstanceGUID
                });
            }
            else
            {
                if (value != null)
                {
                    listItemLocks.Add("PPSRCollateralClass", new ItemLock { ListUniqueIdentifier = value.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)), UserId = userId, InstanceGUID = this.InstanceGUID });
                }

                // Get CatFeatureId to lock table EquipCatFeature
                List<int> listItemTypeCollateralExist = new List<int>();

                if (value != null)
                {
                    foreach (var recordIdLock in value)
                    {
                        listItemTypeCollateralExist.AddRange(await AssetCollateralClassesFunction.GetListTypesItemsSelected(recordIdLock));
                    }
                }

                if (listItemTypeCollateralExist.Count > 0)
                {
                    listItemLocks.Add(
                        "EquipType",
                        new ItemLock
                            {
                                ListUniqueIdentifier = listItemTypeCollateralExist.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)),
                        UserId = userId,
                        InstanceGUID = this.InstanceGUID
                    });
                }
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
            return result;
        }

        #endregion

        #region Private Method
        /// <summary>
        /// The set selected collateral classes.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private async void SetSelectedCollateralClasses(PPSRCollateralClass value)
        {
            bool canProceed = true;

            if (this.IsCheckedOut && this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Asset Collateral";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
                else
                {
                    this.IsChanged = false;
                }
            }

            if (canProceed)
            {
                // Raise event to change style for hyperlink when select another record.
                this.ValidateNotError();
                this.IsChanged = false;
                this.IsCheckedOut = false;
                this.SetIsCheckedOut();

                // Just do UnLockAsync if not in mode Add.
                if (value != null)
                {
                    if (this.SelectedCollateral != null)
                    {
                        await this.UnLockBulkUpdateAssetFeaturesAsync();
                    }
                }

                this.SetField(ref this._selectedCollateral, value, () => this.SelectedCollateral);
                if (value != null)
                {
                    this.SetValueForMappedTo();
                    await this.OnStepAsync(Asset.EnumSteps.SelectedCollateral);
                }
            }
        }

        /// <summary>
        /// The populate all collaterals for view async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task PopulateAllCollateralsForViewAsync()
        {
            List<FilteringDataItem> sourceRegoEndDate;
            List<FilteringDataItem> sourceMappedTo;
            this.DynamicCollateralViewModel = new DynamicGridViewModel(typeof(AssetCollateralRowItem));
            this.DynamicCollateralViewModel.IsEnableHoverRow = false;
            this.DynamicCollateralViewModel.IsSelectAllRow = false;
            this.DynamicCollateralViewModel.IsShowGroupPanel = true;
            this.AllCollateralClasses = new ObservableCollection<PPSRCollateralClass>(AssetCollateralClassesFunction.GetAllPPSRCollateralClassesAsync());

            // get data filter for Rego and date
            List<SystemConstant> regs = AssetCollateralClassesFunction.GetListSystemConstant(116);
            sourceRegoEndDate = (from f in regs
                                 select new FilteringDataItem
                                 {
                                     Text = f.SystemDescription,
                                     IsSelected = true
                                 }).Distinct().ToList();
            sourceRegoEndDate.Add(new FilteringDataItem { Text = "N/A", IsSelected = true });
            sourceRegoEndDate = sourceRegoEndDate.OrderBy(a => a.Text).ToList();

            // get data filter for Mapped To
            List<SystemConstant> mappedTos = AssetCollateralClassesFunction.GetListSystemConstant(117);
            sourceMappedTo = (from f in mappedTos
                                  select new FilteringDataItem
                                  {
                                      Text = f.SystemDescription,
                                      IsSelected = true
                                  }).Distinct().ToList();
            sourceMappedTo.Add(new FilteringDataItem { Text = "N/A", IsSelected = true });
            sourceMappedTo = sourceMappedTo.OrderBy(a => a.Text).ToList();

            // get data filter for Append To Date

            // sourceAppendToDate = sourceMappedTo;
            this.DynamicCollateralViewModel.GridColumns = new List<DynamicColumn>
                                                                                             {
                                                                                                 new DynamicColumn { ColumnName = "Description", MinWidth = 90, Header = "COLLATERAL CLASS", IsSelectedColumn = true, Width = 350 },
                                                                                                 new DynamicColumn { ColumnName = "RegoEndDate", MinWidth = 80, Header = "REG END DATE", FilteringTemplate = RadGridViewEnum.FilteringDataList, FilteringDataSource = sourceRegoEndDate, Width = 250 },
                                                                                                 new DynamicColumn { ColumnName = "AppendToDesc", MinWidth = 80, Header = "APPENT TO DESC", FilteringTemplate = RadGridViewEnum.FilteringDataList, FilteringDataSource = sourceMappedTo, Width = 250 },
                                                                                                 new DynamicColumn { ColumnName = "MappedTo", MinWidth = 75, Header = "MAPPED TO", FilteringTemplate = RadGridViewEnum.FilteringDataList, FilteringDataSource = sourceMappedTo, Width = 300 },
                                                                                                 new DynamicColumn { ColumnName = "MappedField", MinWidth = 75, Header = "MAPPED FIELD" }
                                                                                             };
            this.DynamicCollateralViewModel.FilteringGenerate = true;

            // Load Grid
            this.DynamicCollateralViewModel.GridDataRows = this.AllCollateralClasses.ToList<object>();
            this.DynamicCollateralViewModel.LoadRadGridView();
        }

        /// <summary>
        /// The update data collaterals async.
        /// </summary>
        private void UpdateDataCollateralsAsync()
        {
            DataRow editItem = null;
            if (this.SelectedCollateral.IsNewCollateral)
            {
                this.SelectedCollateral.IsNewCollateral = false;
                this.DynamicCollateralViewModel.InsertRow(0, this.SelectedCollateral);
            }
            else
            {
                foreach (var m in this.DynamicCollateralViewModel.MembersTable.Rows)
                {
                    if (this.SelectedCollateral != null && this.SelectedCollateral.CollateralClassID.ToString(CultureInfo.InvariantCulture) == m["CollateralClassID"].ToString())
                    {
                        editItem = m;
                        break;
                    }
                }

                if (editItem != null)
                {
                    int index = this.DynamicCollateralViewModel.MembersTable.Rows.IndexOf(editItem);
                    this.DynamicCollateralViewModel.UpdateRow(index, this.SelectedCollateral);
                }
            }
        }

        /// <summary>
        /// The set grid stype.
        /// </summary>
        /// <param name="isEditMode">
        /// The is edit mode.
        /// </param>
        private void SetGridStype(bool isEditMode)
        {
            if (isEditMode)
            {
                this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
            }
            else
            {
                this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
            }

            if (this.AssetCollateralDetailViewModel != null)
            {
                if (this._permissionCollateralDetail.CanEdit)
                {
                    this.AssetCollateralDetailViewModel.GridStyle = this.GridStyle;
                }
                else
                {
                    this.AssetCollateralDetailViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                }
            }

            if (this.AssetCollateralTypeViewModel != null)
            {
                if (this._permissionCollateralType.CanEdit)
                {
                    this.AssetCollateralTypeViewModel.GridStyle = this.GridStyle;
                }
                else
                {
                    this.AssetCollateralTypeViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                }
            }

            if (this.AssetCollateralAssignTypeViewModel != null)
            {
                this.AssetCollateralAssignTypeViewModel.GridStyle = this.GridStyle;
            }
        }

        /// <summary>
        /// The set is checked out.
        /// </summary>
        private void SetIsCheckedOut()
        {
            if (this.AssetCollateralDetailViewModel != null)
            {
                if (this._permissionCollateralDetail.CanEdit)
                {
                    this.AssetCollateralDetailViewModel.IsCheckedOut = this.IsCheckedOut;
                }
            }

            if (this.AssetCollateralTypeViewModel != null)
            {
                if (this._permissionCollateralType.CanEdit)
                {
                    this.AssetCollateralTypeViewModel.IsCheckedOut = this.IsCheckedOut;
                }
            }

            if (this.AssetCollateralAssignTypeViewModel != null)
            {
                this.AssetCollateralAssignTypeViewModel.IsCheckedOut = this.IsCheckedOut;
            }
        }

        /// <summary>
        /// The set value for asset collateral detail.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private void SetValueForAssetCollateralDetail(PPSRCollateralClass value)
        {
            this.AssetCollateralDetailViewModel.CollateralName = value.Description;
        }

        /// <summary>
        /// The get data source for asset types screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataSourceForAssetTypesScreen()
        {
            if (this.SelectedCollateral != null)
            {
                AssetCollateralRowItem collateralRow = new AssetCollateralRowItem
                {
                    Description = this.SelectedCollateral.Description,
                    CollateralClassID = this.SelectedCollateral.CollateralClassID
                };
                await this.AssetCollateralTypeViewModel.GetDataSourceForDetailScreen(collateralRow);
                this.AssetCollateralTypeViewModel.DetailContentChanged = this.AssetClassesTypeDetailViewModel_PropertyChanged;
            }
        }

        /// <summary>
        /// The asset classes type detail view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        private void AssetClassesTypeDetailViewModel_PropertyChanged(object sender)
        {
            if (this.IsCheckedOut)
            {
                this.IsChanged = true;
            }
        }

        /// <summary>
        /// The notify select combobox changed.
        /// </summary>
        private void NotifySelectComboboxChanged()
        {
            foreach (var source in this.AssetCollateralDetailViewModel.ListCollateralDetail)
            {
                source.PropertyChanged += this.AssetCollateralDetailViewModelOnPropertyChanged;
                if (source.ListMultiItem != null)
                {
                    foreach (var list in source.ListMultiItem)
                    {
                        list.PropertyChanged += this.AssetCollateralDetailViewModelOnPropertyChanged;
                    }
                }
            }
        }

        /// <summary>
        /// The asset collateral detail view model on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetCollateralDetailViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut && e.PropertyName.StartsWith("SelectComboBox"))
            {
                this.IsChanged = true;
            }
        }

        /// <summary>
        /// The set value for mapped to.
        /// </summary>
        private async void SetValueForMappedTo()
        {
            this.SelectedCollateral.MotorVehicleRego =
                     await AssetCollateralClassesFunction.GetListMotorVehicleRego(this.SelectedCollateral.CollateralClassID);
            this.SelectedCollateral.Aircraft =
                    await AssetCollateralClassesFunction.GetListAircraft(this.SelectedCollateral.CollateralClassID);
            if (this.SelectedCollateral.MotorVehicleRego == null)
            {
                this.SelectedCollateral.MotorVehicleRego = new PPSRCollateralClassMotorVehicle();
            }

            if (this.SelectedCollateral.Aircraft == null)
            {
                this.SelectedCollateral.Aircraft = new PPSRCollateralClassAircraft();
            }
        }

        /// <summary>
        /// The save collateral classes async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SaveCollateralClassesAsync()
        {
            // Save detail
            ObservableCollection<ItemCollateralClassViewModel> listField = new ObservableCollection<ItemCollateralClassViewModel>();
            foreach (var listParent in this.AssetCollateralDetailViewModel.ListCollateralDetail)
            {
                if (listParent.IsShowUp)
                {
                    listField.Add(listParent);
                    if (listParent.ListMultiItem != null)
                    {
                        foreach (var listChild in listParent.ListMultiItem)
                        {
                            if (listChild.IsShowUp)
                            {
                                listField.Add(listChild);
                            }
                        }
                    }
                }
            }

            foreach (var field in listField)
            { 
                switch (field.CollateralFieldID)
                {
                    case Asset.CollateralFieldID.RegistrationEndDate:
                        this.SelectedCollateral.RegoEndDateID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        this.SelectedCollateral.RegoEndDate = AssetCollateralClassesFunction.GetSystemConstantName(this.SelectedCollateral.RegoEndDateID);
                        break;
                    case Asset.CollateralFieldID.SerialNumberType:
                        this.SelectedCollateral.SerialNumberTypeID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                        
                    // 1
                    case Asset.CollateralFieldID.MappedTo:
                        this.SelectedCollateral.MappedToID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        this.SelectedCollateral.MappedTo = AssetCollateralClassesFunction.GetSerialNumberRequired(this.SelectedCollateral.CollateralClassID) != 0 ? AssetCollateralClassesFunction.GetSystemConstantName(this.SelectedCollateral.MappedToID) : "N/A";
                        break;
                    case Asset.CollateralFieldID.MappedField:
                        this.SelectedCollateral.MappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        this.SelectedCollateral.MappedField = AssetCollateralClassesFunction.GetSystemConstantName(this.SelectedCollateral.MappedFieldID);
                        break;
                    case Asset.CollateralFieldID.QuoteMappedField:
                        this.SelectedCollateral.QuoteMappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.ContractMappedField:
                        this.SelectedCollateral.MappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;

                    // 2
                    case Asset.CollateralFieldID.AppendedTo:
                        this.SelectedCollateral.MappedToID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        this.SelectedCollateral.AppendToDesc = AssetCollateralClassesFunction.GetSerialNumberRequired(this.SelectedCollateral.CollateralClassID) == 0 ? AssetCollateralClassesFunction.GetSystemConstantName(this.SelectedCollateral.MappedToID) : "N/A";
                        break;
                    case Asset.CollateralFieldID.AppendedToMappedField:
                        this.SelectedCollateral.MappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.AppendedToContractMappedField:
                        this.SelectedCollateral.MappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.AppendedToQuoteMappedField:
                        this.SelectedCollateral.QuoteMappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    
                    // 3
                    case Asset.CollateralFieldID.MotorVehicleRegoMappedTo:
                        this.SelectedCollateral.MotorVehicleRego.RegoMappedTo = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.MotorVehicleRegoMappedField:
                        this.SelectedCollateral.MotorVehicleRego.RegoMappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.MotorVehicleRegoContractMappedField:
                        this.SelectedCollateral.MotorVehicleRego.RegoMappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.MotorVehicleRegoQuoteMappedField:
                        this.SelectedCollateral.MotorVehicleRego.RegoQuoteMappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;

                    // 4
                    case Asset.CollateralFieldID.AircraftNationalityMappedTo:
                        this.SelectedCollateral.Aircraft.NationalityMappedTo = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.AircraftNationalityMappedField:
                        this.SelectedCollateral.Aircraft.NationalityMappedToFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.AircraftNationalityContractMappedField:
                        this.SelectedCollateral.Aircraft.NationalityMappedToFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.AircraftNationalityQuoteMappedField:
                        this.SelectedCollateral.Aircraft.NationalityQuoteMappedToFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    
                    // 5
                    case Asset.CollateralFieldID.AircraftNationalityCodeRegoMarkMappedTo:
                        this.SelectedCollateral.Aircraft.NationalityCodeAndRegoMarkMappedTo = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.AircraftNationalityCodeRegoMarkMappedField:
                        this.SelectedCollateral.Aircraft.NationalityCodeAndRegoMarkMappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.AircraftNationalityCodeRegoMarkContractMappedField:
                        this.SelectedCollateral.Aircraft.NationalityCodeAndRegoMarkMappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.AircraftNationalityCodeRegoMarkQuoteMappedField:
                        this.SelectedCollateral.Aircraft.NationalityCodeAndRegoMarkQuoteMappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;

                    // 6
                    case Asset.CollateralFieldID.AircraftManufacturerModelMappedTo:
                        this.SelectedCollateral.Aircraft.ManufacturerModelMappedTo = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.AircraftManufacturerModelMappedField:
                        this.SelectedCollateral.Aircraft.ManufacturerModelMappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.AircraftManufacturerModelContractMappedField:
                        this.SelectedCollateral.Aircraft.ManufacturerModelMappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.AircraftManufacturerModelQuoteMappedField:
                        this.SelectedCollateral.Aircraft.ManufacturerModelQuoteMappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;

                    // 7
                    case Asset.CollateralFieldID.AircraftManufacturerNameMappedTo:
                        this.SelectedCollateral.Aircraft.ManufacturerNameMappedTo = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.AircraftManufacturerNameMappedField:
                        this.SelectedCollateral.Aircraft.ManufacturerNameMappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.AircraftManufacturerNameContractMappedField:
                        this.SelectedCollateral.Aircraft.ManufacturerNameMappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;
                    case Asset.CollateralFieldID.AircraftManufacturerNameQuoteMappedField:
                        this.SelectedCollateral.Aircraft.ManufacturerNameQuoteMappedFieldID = field.SelectComboBox != null && field.SelectComboBox.ItemId != -1 ? field.SelectComboBox.ItemId : null;
                        break;               
                }
            }

            await AssetCollateralClassesFunction.SaveCollateralClassesAsync(this.SelectedCollateral);

            // Save type
            if (this.AssetCollateralTypeViewModel.DynamicAssignAssetTypeViewModel.SelectedItems != null)
            {
                var allItemsSelected =
                    new ObservableCollection<AssetClassesTypeRowItem>(
                        this.AssetCollateralTypeViewModel.DynamicAssignAssetTypeViewModel.SelectedItems
                            .Cast<AssetClassesTypeRowItem>());
                await
                    AssetCollateralClassesFunction.SaveAssignTypesScreen(
                        this.SelectedCollateral.CollateralClassID,
                        allItemsSelected);
            }
        }

        /// <summary>
        /// The get permssion.
        /// </summary>
        private void GetPermssion()
        {
            this._permissionCollateralDetail = Authorisation.GetPermission(Components.SystemManagementAssetCollateralClasses, Forms.CollateralClassesDetail);
            this._permissionCollateralType = Authorisation.GetPermission(Components.SystemManagementAssetCollateralClasses, Forms.CollateralClassesType);
        }

        /// <summary>
        /// The save collateral bulk update async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SaveCollateralBulkUpdateAsync()
        {
            var allItemsSelected = new ObservableCollection<AssetCollateralRowItem>(this.DynamicCollateralViewModel.SelectedItems.Cast<AssetCollateralRowItem>());
            await AssetCollateralClassesFunction.SaveAllForAssignTypesScreen(allItemsSelected, this.AssetCollateralAssignTypeViewModel.ListItemsDragDrop.GroupDragDropSource);
        }

        #endregion

        #region Public Methods for Events

        #endregion

        #region Other

        /// <summary>
        /// The un lock bulk update asset features async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task UnLockBulkUpdateAssetFeaturesAsync()
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
        /// The asset collateral assign type view model on property changed.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void AssetCollateralAssignTypeViewModelOnPropertyChanged(object obj)
        {
            if (this.IsCheckedOut)
            {
                this.IsChanged = true;
            }
        }

        /// <summary>
        /// The asset collateral type view model on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetCollateralTypeViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut && (e.PropertyName.IndexOf("IsCheckItemChanged", StringComparison.Ordinal) != -1))
            {
                this.IsChanged = true;
            }
        }

        #endregion
    } 
}
