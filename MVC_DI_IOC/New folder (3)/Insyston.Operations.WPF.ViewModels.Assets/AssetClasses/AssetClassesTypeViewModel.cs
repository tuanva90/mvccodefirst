// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetClassesTypeViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetClassesTypeViewModel type.
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

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Bussiness.Assets.AssetClasses;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.Locking.Helpers;
    using Insyston.Operations.Model;
    using Insyston.Operations.Security;
    using Insyston.Operations.Security.Model;
    using Insyston.Operations.WPF.ViewModel.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.AssetType;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Enums;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;
    using Insyston.Operations.WPF.ViewModels.Common.Model.Filtering;

    using global::WPF.DataTable.Models;

    using WPFDynamic.ViewModels.Controls;

    /// <summary>
    /// The asset classes type view model.
    /// </summary>
    public class AssetClassesTypeViewModel : ViewModelUseCaseBase
    {
        #region Private Properties
        /// <summary>
        /// The permission type detail.
        /// </summary>
        public Permission PermissionTypeDetail;

        /// <summary>
        /// The permission type feature.
        /// </summary>
        public Permission PermissionTypeFeature;

        /// <summary>
        /// The permission type make.
        /// </summary>
        public Permission PermissionTypeMake;

        /// <summary>
        /// The loading text.
        /// </summary>
        private const string LoadingText = "Please Wait Loading ...";

        /// <summary>
        /// The _selected type item.
        /// </summary>
        private AssetClassesTypeRowItem _selectedTypeItem;

        /// <summary>
        /// The _current enumerable step.
        /// </summary>
        private Asset.EnumSteps _currentEnumStep;

        /// <summary>
        /// The _list data grid item.
        /// </summary>
        private List<AssetClassesTypeRowItem> _listDataGridItem;

        /// <summary>
        /// The _dynamic main grid view model.
        /// </summary>
        private DynamicGridViewModel _dynamicMainGridViewModel;

        /// <summary>
        /// The _type detail view model.
        /// </summary>
        private AssetTypeDetailViewModel _typeDetailViewModel;

        /// <summary>
        /// The _type features view model.
        /// </summary>
        private AssetTypeFeatureViewModel _typeFeaturesViewModel;

        /// <summary>
        /// The _type update depreciation view model.
        /// </summary>
        private AssetTypeUpdateDepreciationViewModel _typeUpdateDepreciationViewModel;

        /// <summary>
        /// The _type make view model.
        /// </summary>
        private AssetTypeMakeViewModel _typeMakeViewModel;

        /// <summary>
        /// The _type assign feature view model.
        /// </summary>
        private AssetTypeAssignFeatureViewModel _typeAssignFeatureViewModel;

        /// <summary>
        /// The _type assign make view model.
        /// </summary>
        private AssetTypeAssignMakeViewModel _typeAssignMakeViewModel;

        /// <summary>
        /// The _source filter type.
        /// </summary>
        private List<FilteringDataItem> _sourceFilterType;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetClassesTypeViewModel"/> class.
        /// </summary>
        public AssetClassesTypeViewModel()
        {
            this.IsNeedToLoad = false;
            this.InstanceGUID = Guid.NewGuid();
            this.TypeDetailViewModel = new AssetTypeDetailViewModel();
            this.TypeFeaturesViewModel = new AssetTypeFeatureViewModel();
            this.TypeMakeViewModel = new AssetTypeMakeViewModel();
            this.TypeUpdateDepreciationViewModel = new AssetTypeUpdateDepreciationViewModel();
            this.TypeAssignFeatureViewModel = new AssetTypeAssignFeatureViewModel();
            this.TypeAssignMakeViewModel = new AssetTypeAssignMakeViewModel();
            this.ListItemLocks = new Dictionary<string, ItemLock>();

            this.DynamicMainGridViewModel = new DynamicGridViewModel(typeof(AssetClassesTypeRowItem));
            this.DynamicMainGridViewModel.IsEnableHoverRow = false;
            this.DynamicMainGridViewModel.IsShowGroupPanel = true;
        }

        #endregion

        #region Event, Action
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
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the current state.
        /// </summary>
        public Asset.EnumSteps CurrentState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is need to load.
        /// </summary>
        public bool IsNeedToLoad { get; set; }

        /// <summary>
        /// Gets or sets the current tab.
        /// </summary>
        public Asset.EnumSteps CurrentTab { get; set; }

        /// <summary>
        /// Gets or sets the list item locks.
        /// </summary>
        public Dictionary<string, ItemLock> ListItemLocks { get; set; }

        /// <summary>
        /// Gets or sets the source filter type.
        /// </summary>
        public List<FilteringDataItem> SourceFilterType 
        {
            get
            {
                return this._sourceFilterType;
            }

            set
            {
                this.SetField(ref this._sourceFilterType, value, () => this.SourceFilterType);
            }
        }

        /// <summary>
        /// Gets or sets the selected type item.
        /// </summary>
        public AssetClassesTypeRowItem SelectedTypeItem
        {
            get
            {
                return this._selectedTypeItem;
            }

            set
            {
                this.SetSelectedTypeItem(value);
            }
        }

        /// <summary>
        /// Gets or sets the list data grid item.
        /// </summary>
        public List<AssetClassesTypeRowItem> ListDataGridItem
        {
            get
            {
                return this._listDataGridItem;
            }

            set
            {
                this.SetField(ref this._listDataGridItem, value, () => this.ListDataGridItem);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic main grid view model.
        /// </summary>
        public DynamicGridViewModel DynamicMainGridViewModel
        {
            get
            {
                return this._dynamicMainGridViewModel;
            }

            set
            {
                this.SetField(ref this._dynamicMainGridViewModel, value, () => this.DynamicMainGridViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the type detail view model.
        /// </summary>
        public AssetTypeDetailViewModel TypeDetailViewModel
        {
            get
            {
                return this._typeDetailViewModel;
            }

            set
            {
                this.SetField(ref this._typeDetailViewModel, value, () => this.TypeDetailViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the type features view model.
        /// </summary>
        public AssetTypeFeatureViewModel TypeFeaturesViewModel
        {
            get
            {
                return this._typeFeaturesViewModel;
            }

            set
            {
                this.SetField(ref this._typeFeaturesViewModel, value, () => this.TypeFeaturesViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the type make view model.
        /// </summary>
        public AssetTypeMakeViewModel TypeMakeViewModel
        {
            get
            {
                return this._typeMakeViewModel;
            }

            set
            {
                this.SetField(ref this._typeMakeViewModel, value, () => this.TypeMakeViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the type update depreciation view model.
        /// </summary>
        public AssetTypeUpdateDepreciationViewModel TypeUpdateDepreciationViewModel
        {
            get
            {
                return this._typeUpdateDepreciationViewModel;
            }

            set
            {
                this.SetField(ref this._typeUpdateDepreciationViewModel, value, () => this.TypeUpdateDepreciationViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the type assign feature view model.
        /// </summary>
        public AssetTypeAssignFeatureViewModel TypeAssignFeatureViewModel
        {
            get
            {
                return this._typeAssignFeatureViewModel;
            }

            set
            {
                this.SetField(ref this._typeAssignFeatureViewModel, value, () => this.TypeAssignFeatureViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the type assign make view model.
        /// </summary>
        public AssetTypeAssignMakeViewModel TypeAssignMakeViewModel
        {
            get
            {
                return this._typeAssignMakeViewModel;
            }

            set
            {
                this.SetField(ref this._typeAssignMakeViewModel, value, () => this.TypeAssignMakeViewModel);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether on loaded.
        /// </summary>
        public bool OnLoaded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether in mode bulk update.
        /// </summary>
        public bool InModeBulkUpdate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether in mode add.
        /// </summary>
        public bool InModeAdd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether in error.
        /// </summary>
        public bool InError { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is new item.
        /// </summary>
        public bool IsNewItem { get; set; }

        #endregion

        #region Public Methods
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
            this._currentEnumStep = (Asset.EnumSteps)Enum.Parse(typeof(Asset.EnumSteps), stepName.ToString());
            switch (this._currentEnumStep)
            {
                case Asset.EnumSteps.Start:
                    this.SetBusyAction(LoadingText);
                    this.GetPermission();
                    this.CurrentState = Asset.EnumSteps.Start;
                    this.OnStepChanged("MainViewState");
                    this.ActiveViewModel = this;
                    await this.LoadData();
                    await this.GenerateDataForAllControl();
                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.MainViewState:
                    this.CurrentState = Asset.EnumSteps.MainViewState;
                    this.OnStepChanged("MainViewState");
                    break;
                case Asset.EnumSteps.SelectedAssetClassesTypeItem:
                    this.SetBusyAction(LoadingText);
                    this.CurrentState = Asset.EnumSteps.SelectedAssetClassesTypeItem;
                    await this.GetDataSourceForDetailScreen();
                    if (!this.OnLoaded)
                    {
                        this.OnStepChanged("MainContentState");
                        this.OnLoaded = true;
                    }

                    await this.GetDataSourceForFeaturesScreen();
                    await this.GetDataSourceForMakeScreen();
                    this.ResetBusyAction();
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.SelectedAssetClassesTypeItem, this.DynamicMainGridViewModel.SelectedItem);
                    break;
                case Asset.EnumSteps.AssetClassesTypeDetailState:
                    this.CurrentState = Asset.EnumSteps.AssetClassesTypeDetailState;
                    this.CurrentTab = Asset.EnumSteps.AssetClassesTypeDetailState;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                    }

                    break;
                case Asset.EnumSteps.AssetClassesTypeFeaturesState:
                    this.CurrentState = Asset.EnumSteps.AssetClassesTypeFeaturesState;
                    this.CurrentTab = Asset.EnumSteps.AssetClassesTypeFeaturesState;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                    }

                    break;
                case Asset.EnumSteps.AssetClassesTypeMakeState:
                    bool canProcess = true;
                    if (this.IsCheckedOut)
                    {
                        canProcess = await this.LockAssetClassesTypeAsync();
                    }

                    if (canProcess)
                    {
                        if (this.ListItemLocks.Count > 0)
                        {
                            this.IsCheckedOut = true;
                        }

                        this.CurrentState = Asset.EnumSteps.AssetClassesTypeMakeState;
                        this.CurrentTab = Asset.EnumSteps.AssetClassesTypeMakeState;
                        if (this.OnStoryBoardChanged != null)
                        {
                            this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                        }
                    }
                    else
                    {
                        this.IsCheckedOut = true;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, EnumSteps.SelectOldTabHyperlink);
                    }

                    break;
                case Asset.EnumSteps.AssetClassesTypeAssignFeaturesState:
                    this.SetBusyAction(LoadingText);
                    if (this.DynamicMainGridViewModel.SelectedItems.Count() > 0)
                    {
                        if (await this.LockAssetClassesTypeAsync())
                        {
                            if (this.OnStoryBoardChanged != null)
                            {
                                this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                            }

                            this.OnStepChanged("BulkUpdateState");
                            this.SetBackgroundToEdit();
                            this.CurrentState = Asset.EnumSteps.AssetClassesTypeAssignFeaturesState;
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.AssetClassesTypeAssignFeaturesState);
                            await this.GetListFeatureItemsForAssignFeaturesScreen();
                            this.IsCheckedOut = true;
                        }
                    }
                    else
                    {
                        this.ShowMessageAsync("Select one or more record to Assign Features.", "Confirm - Asset Classes Type");
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.AssetClassesTypeAssignMakeState:
                    this.SetBusyAction(LoadingText);
                    if (this.DynamicMainGridViewModel.SelectedItems.Count() > 0)
                    {
                        if (await this.LockAssetClassesTypeAsync())
                        {
                            if (this.OnStoryBoardChanged != null)
                            {
                                this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                            }

                            this.OnStepChanged("BulkUpdateState");
                            this.SetBackgroundToEdit();
                            this.CurrentState = Asset.EnumSteps.AssetClassesTypeAssignMakeState;
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.AssetClassesTypeAssignMakeState);
                            await this.GetListTypesItemsForAssignMakeScreen();
                            this.IsCheckedOut = true;
                        }
                    }
                    else
                    {
                        this.ShowMessageAsync("Select one or more record to Assign Make.", "Confirm - Asset Classes Type");
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.AssetClassesTypeUpdateDepreciationState:
                    this.SetBusyAction(LoadingText);
                    if (this.DynamicMainGridViewModel.SelectedItems.Count() > 0)
                    {
                        if (await this.LockAssetClassesTypeAsync())
                        {
                            if (this.OnStoryBoardChanged != null)
                            {
                                this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                            }

                            this.OnStepChanged("BulkUpdateState");
                            this.SetBackgroundToEdit();
                            this.CurrentState = Asset.EnumSteps.AssetClassesTypeUpdateDepreciationState;
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.AssetClassesTypeUpdateDepreciationState);
                            await this.GetDataSourceForUpdateDepreciationScreen();
                            this.IsCheckedOut = true;
                        }
                    }
                    else
                    {
                        this.ShowMessageAsync("Select one or more record to Update Depreciation.", "Confirm - Asset Classes Type");
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.BulkUpdate:
                    this.CurrentState = Asset.EnumSteps.BulkUpdate;
                    this.InModeBulkUpdate = true;
                    this.DynamicMainGridViewModel.IsEnableHoverRow = true;
                    break;
                case Asset.EnumSteps.Add:
                    this.InModeAdd = true;
                    this.SetBusyAction(LoadingText);

                    // Change story board to screen detail
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged("AssetClassesTypeDetailState");
                    }

                    this.OnStepChanged("MainContentState");
                    this.CurrentState = Asset.EnumSteps.Add;
                    this.SetBackgroundToEdit();
                    await this.SetSelectedTypeItem(new AssetClassesTypeRowItem { EquipTypeId = 0, Enabled = true });
                    this.IsNewItem = true;
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, EnumSteps.Add);
                    await this.OnStepAsync(Asset.EnumSteps.Edit);
                    this.ResetBusyAction();
                    this.IsChanged = false;
                    break;
                case Asset.EnumSteps.Edit:
                    this.SetBusyAction(LoadingText);
                    if (await this.LockAssetClassesTypeAsync())
                    {
                        this.SetBackgroundToEdit();

                        this.CurrentState = Asset.EnumSteps.Edit;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.Edit);
                        if (this.TypeFeaturesViewModel.FeaturesTabViewModel != null)
                        {
                            if (this.PermissionTypeFeature.CanEdit)
                            {
                                this.TypeFeaturesViewModel.FeaturesTabViewModel.IsEnableHoverRow = true;
                            }
                        }

                        if (this.TypeMakeViewModel.AssetMakeTabViewModel != null)
                        {
                            if (this.PermissionTypeMake.CanEdit)
                            {
                                this.TypeMakeViewModel.AssetMakeTabViewModel.IsEnableHoverRow = true;
                            }
                        }

                        this.IsCheckedOut = true;
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.EditAssignFeature:
                    if (await this.LockAssetClassesTypeAsync())
                    {
                        this.SetBackgroundToEdit();
                        this.CurrentState = Asset.EnumSteps.EditAssignFeature;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.AssetClassesTypeAssignFeaturesState);
                        this.IsCheckedOut = true;
                    }

                    break;
                case Asset.EnumSteps.EditAssignMake:
                    if (this.DynamicMainGridViewModel.SelectedItems != null)
                    {
                        if (await this.LockAssetClassesTypeAsync())
                        {
                            this.SetBackgroundToEdit();
                            this.CurrentState = Asset.EnumSteps.EditAssignMake;
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.AssetClassesTypeAssignMakeState);
                            this.IsCheckedOut = true;
                        }
                    }

                    break;
                case Asset.EnumSteps.EditUpdateDepreciation:
                    if (this.DynamicMainGridViewModel.SelectedItems != null)
                    {
                        if (await this.LockAssetClassesTypeAsync())
                        {
                            this.SetBackgroundToEdit();
                            this.CurrentState = Asset.EnumSteps.EditUpdateDepreciation;
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.AssetClassesTypeUpdateDepreciationState);
                            this.IsCheckedOut = true;
                        }
                    }

                    break;
                case Asset.EnumSteps.Save:
                    this.TypeDetailViewModel.Validate();
                    if (this.TypeDetailViewModel.HasErrors == false)
                    {
                        if (await this.CheckCollateralClassId())
                        {
                            if (!this.IsNewItem)
                            {
                                await this.UnLockAssetClassesTypeAsync();
                            }

                            this.ListErrorHyperlink.Clear();
                            this.TypeDetailViewModel.ListErrorHyperlink.Clear();
                            this.InError = false;
                            this.ValidateNotError();
                            this.SetBusyAction(LoadingText);
                            this.CurrentState = Asset.EnumSteps.Save;
                            this.TypeDetailViewModel.ClearNotifyErrors();
                            this.SetBackgroundToNotEdit();
                            string previousType = this._selectedTypeItem.TypeDescription;
                            EquipType itemTypeSaved = await this.SaveAllDataForDetailScreen();
                            this.TypeDetailViewModel.OldCollateralClassId = itemTypeSaved.CollateralClassId;
                            await this.SetSelectedNewItem(itemTypeSaved);
                            if (this.IsNewItem)
                            {
                                this.UpdateSourceForGrid();
                                this.InModeAdd = false;
                                this.IsNewItem = false;
                                await this.TypeFeaturesViewModel.GetFeatureDataSource(itemTypeSaved.EquipTypeId, itemTypeSaved.Enabled);
                                await this.TypeMakeViewModel.GetMakeDataSource(itemTypeSaved.EquipTypeId, itemTypeSaved.Enabled);
                            }
                            else
                            {
                                this.UpdateSourceForGrid(previousType);
                                await this.TypeFeaturesViewModel.GetFeatureDataSource(itemTypeSaved.EquipTypeId, itemTypeSaved.Enabled);
                                await this.TypeMakeViewModel.GetMakeDataSource(itemTypeSaved.EquipTypeId, itemTypeSaved.Enabled);
                            }

                            this.RaiseActionsWhenChangeStep(
                                EnumScreen.AssetClassesType,
                                Asset.EnumSteps.Save,
                                itemTypeSaved);
                            this.TypeFeaturesViewModel.FeaturesTabViewModel.IsEnableHoverRow = false;
                            this.TypeMakeViewModel.AssetMakeTabViewModel.IsEnableHoverRow = false;
                        }
                    }
                    else
                    {
                        this.InError = true;
                        this.CurrentState = Asset.EnumSteps.Error;
                        this.ListErrorHyperlink = this.TypeDetailViewModel.ListErrorHyperlink;
                        foreach (var itemError in this.ListErrorHyperlink)
                        {
                            itemError.Action = HyperLinkAction.AssetClassesTypeDetailState;
                            itemError.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
                        }

                        this.OnErrorHyperlinkSelected();
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.SaveAndAdd:
                    this.TypeDetailViewModel.Validate();
                    if (this.TypeDetailViewModel.HasErrors == false)
                    {
                        await this.OnStepAsync(Asset.EnumSteps.Save);
                        await this.OnStepAsync(Asset.EnumSteps.Add);
                    }
                    else
                    {
                        this.InError = true;
                        this.CurrentState = Asset.EnumSteps.Error;
                        this.ListErrorHyperlink = this.TypeDetailViewModel.ListErrorHyperlink;
                        foreach (var itemError in this.ListErrorHyperlink)
                        {
                            itemError.Action = HyperLinkAction.AssetClassesTypeDetailState;
                            itemError.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
                        }

                        this.OnErrorHyperlinkSelected();
                    }

                    break;
                case Asset.EnumSteps.SaveAssignFeature:
                    this.CurrentState = Asset.EnumSteps.SaveAssignFeature;
                    await this.UnLockAssetClassesTypeAsync();
                    this.SetBackgroundToNotEdit();
                    await this.SaveAllDataForAssignFeatureScreen();
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.SaveAssignFeature);
                    break;
                case Asset.EnumSteps.SaveAssignMake:
                    this.CurrentState = Asset.EnumSteps.SaveAssignMake;
                    await this.UnLockAssetClassesTypeAsync();
                    this.SetBackgroundToNotEdit();
                    await this.SaveAllDataForAssignMakeScreen();
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.SaveAssignMake);
                    break;
                case Asset.EnumSteps.SaveUpdateDepreciation:
                    this.TypeUpdateDepreciationViewModel.Validate();
                    if (this.TypeUpdateDepreciationViewModel.HasErrors == false)
                    {
                        this.InError = false;
                        this.ListErrorHyperlink.Clear();
                        this.TypeUpdateDepreciationViewModel.ListErrorHyperlink.Clear();
                        this.SetBusyAction(LoadingText);
                        this.CurrentState = Asset.EnumSteps.SaveUpdateDepreciation;
                        await this.UnLockAssetClassesTypeAsync();
                        this.TypeUpdateDepreciationViewModel.ClearNotifyErrors();
                        this.SetBackgroundToNotEdit();
                        await this.SaveAllDataForUpdateDepreciationScreen();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.SaveUpdateDepreciation);
                    }
                    else
                    {
                        this.InError = true;
                        this.CurrentState = Asset.EnumSteps.ErrorUpdateDepreciation;
                        this.ListErrorHyperlink = this.TypeUpdateDepreciationViewModel.ListErrorHyperlink;
                        this.OnErrorHyperlinkSelected();
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.Cancel:
                    if (await this.CheckIfUnSavedChanges())
                    {
                        this.InError = false;
                        this.ListErrorHyperlink.Clear();
                        this.TypeDetailViewModel.ListErrorHyperlink.Clear();
                        this.SetBusyAction(LoadingText);
                        this.ValidateNotError();
                        this.CurrentState = Asset.EnumSteps.Cancel;
                        this.TypeDetailViewModel.ClearNotifyErrors();
                        await this.UnLockAssetClassesTypeAsync();
                        this.SetBackgroundToNotEdit();

                        if (!this.IsCheckedOut)
                        {
                            if (this.TypeFeaturesViewModel.FeaturesTabViewModel != null)
                            {
                                this.TypeFeaturesViewModel.FeaturesTabViewModel.IsEnableHoverRow = false;
                            }

                            if (this.TypeMakeViewModel.AssetMakeTabViewModel != null)
                            {
                                this.TypeMakeViewModel.AssetMakeTabViewModel.IsEnableHoverRow = false;
                            }
                        }

                        if (this.IsNewItem)
                        {
                            this.IsNewItem = false;
                            this.OnCancelNewItem(EnumScreen.AssetClassesType);
                            await this.OnStepAsync(Asset.EnumSteps.MainViewState);
                            this.InModeAdd = false;
                        }
                        else
                        {
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.Cancel);
                            await this.OnStepAsync(Asset.EnumSteps.SelectedAssetClassesTypeItem);
                        }

                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.CancelAssignFeature:
                    if (await this.CheckIfUnSavedChanges())
                    {
                        await this.UnLockAssetClassesTypeAsync();
                        await this.GetListFeatureItemsForAssignFeaturesScreen();
                        this.CurrentState = Asset.EnumSteps.CancelAssignFeature;
                        this.SetBackgroundToNotEdit();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.CancelAssignFeature);
                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                    }

                    break;
                case Asset.EnumSteps.CancelAssignMake:
                    if (await this.CheckIfUnSavedChanges())
                    {
                        await this.UnLockAssetClassesTypeAsync();
                        await this.GetListTypesItemsForAssignMakeScreen();
                        this.CurrentState = Asset.EnumSteps.CancelAssignMake;
                        this.SetBackgroundToNotEdit();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.CancelAssignMake);
                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                    }

                    break;
                case Asset.EnumSteps.CancelUpdateDepreciation:
                    if (await this.CheckIfUnSavedChanges())
                    {
                        this.InError = false;
                        this.ListErrorHyperlink.Clear();
                        this.TypeUpdateDepreciationViewModel.ListErrorHyperlink.Clear();
                        this.SetBusyAction(LoadingText);
                        await this.UnLockAssetClassesTypeAsync();
                        await this.GetDataSourceForUpdateDepreciationScreen();
                        this.CurrentState = Asset.EnumSteps.CancelUpdateDepreciation;
                        this.SetBackgroundToNotEdit();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.CancelUpdateDepreciation);
                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.CancelBulkUpdate:
                    if (await this.CheckIfUnSavedChanges())
                    {
                        this.CurrentState = Asset.EnumSteps.CancelBulkUpdate;
                        this.DynamicMainGridViewModel.IsEnableHoverRow = false;
                        this.InModeBulkUpdate = true;
                        this.DynamicMainGridViewModel.SelectedRows = new List<object>();
                        this.InModeBulkUpdate = false;
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

        /// <summary>
        /// The get list feature items for assign features screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetListFeatureItemsForAssignFeaturesScreen()
        {
            if (this.DynamicMainGridViewModel.SelectedItems != null)
            {
                var allItemsSelected = new ObservableCollection<AssetClassesTypeRowItem>(this.DynamicMainGridViewModel.SelectedItems.Cast<AssetClassesTypeRowItem>());
                await this.TypeAssignFeatureViewModel.GetListFeatureItems(allItemsSelected);
                this.TypeAssignFeatureViewModel.ContentItemChanged += this.AssetClassesTypeDetailViewModel_PropertyChanged;
            }
        }

        /// <summary>
        /// The get list types items for assign make screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetListTypesItemsForAssignMakeScreen()
        {
            if (this.DynamicMainGridViewModel.SelectedItems != null)
            {
                var allItemsSelected = new ObservableCollection<AssetClassesTypeRowItem>(this.DynamicMainGridViewModel.SelectedItems.Cast<AssetClassesTypeRowItem>());
                await this.TypeAssignMakeViewModel.GetListMakeItems(allItemsSelected);
                this.TypeAssignMakeViewModel.ContentItemChanged += this.AssetClassesTypeDetailViewModel_PropertyChanged;
            }
        }

        /// <summary>
        /// The load data.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task LoadData()
        {
            await this.GetDataSourceForGrid();
        }

        /// <summary>
        /// The disable hover.
        /// </summary>
        public void DisableHover()
        {
            this.DynamicMainGridViewModel.IsEnableHoverRow = false;
        }

        /// <summary>
        /// The unlock item.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task UnlockItem()
        {
            if (!this.InModeBulkUpdate)
            {
                this._currentEnumStep = Asset.EnumSteps.Dispose;
            }

            return this.UnLockAssetClassesTypeAsync();
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
                confirmViewModel.Title = "Confirm Save - Asset Classes Type";
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
        /// The check collateral class id.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> CheckCollateralClassId()
        {
            bool canProceed = true;
            if (this.TypeDetailViewModel.OldCollateralClassId != this.TypeDetailViewModel.SelectedCollateralClassItem.ItemId && this.SelectedTypeItem.EquipTypeId != 0)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changing collateral class on an equipment type will invalidate the existing PPSR registration data.  Do you wish to update all unregistered PPSR data and continue?";
                confirmViewModel.Title = "Confirm - Asset Classes Type";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }

            return canProceed;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// The set action commands async.
        /// </summary>
        protected override void SetActionCommandsAsync()
        {
            if (this.CurrentState == Asset.EnumSteps.Start || this.CurrentState == Asset.EnumSteps.MainViewState || this.CurrentState == Asset.EnumSteps.CancelBulkUpdate)
            {
                ObservableCollection<ActionCommand> tempActionCommands = new ObservableCollection<ActionCommand>();
                if (this.PermissionTypeDetail.CanAdd && this.PermissionTypeDetail.CanEdit)
                {
                    tempActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.Add.ToString(), Command = new Add() });
                }

                if (this.PermissionTypeDetail.CanEdit || this.PermissionTypeFeature.CanEdit || this.PermissionTypeMake.CanEdit)
                {
                    tempActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.BulkUpdate.ToString(), Command = new BulkUpdate() });
                }

                this.ActionCommands = tempActionCommands;
            }
            else if (this.CurrentState == Asset.EnumSteps.SelectedAssetClassesTypeItem)
            {
                if (this.PermissionTypeDetail.CanEdit && this.CurrentTab == Asset.EnumSteps.AssetClassesTypeDetailState)
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                    };
                }
                else if (this.PermissionTypeFeature.CanEdit && this.CurrentTab == Asset.EnumSteps.AssetClassesTypeFeaturesState)
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                    };
                }
                else if (this.PermissionTypeMake.CanEdit && this.CurrentTab == Asset.EnumSteps.AssetClassesTypeMakeState)
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                    };
                }
                else
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>();
                }
            }
            else if (this.CurrentState == Asset.EnumSteps.AssetClassesTypeDetailState)
            {
                if (this.PermissionTypeDetail.CanEdit)
                {
                    if (!this.IsCheckedOut)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                        };
                    }
                    else
                    {
                        if (this.PermissionTypeDetail.CanAdd && this.InModeAdd)
                        {
                            if (this.InError)
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.SaveAndAdd.ToString(), Command = new SaveAndAdd() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() },
                                };
                            }
                            else
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.SaveAndAdd.ToString(), Command = new SaveAndAdd() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                                };
                            }
                        }
                        else
                        {
                            if (this.InError)
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() },
                                };
                            }
                            else
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                                };
                            }
                        }
                    }
                }
                else
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>();
                }
            }
            else if (this.CurrentState == Asset.EnumSteps.AssetClassesTypeFeaturesState)
            {
                if (this.PermissionTypeFeature.CanEdit)
                {
                    if (!this.IsCheckedOut)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                        };
                    }
                    else
                    {
                        if (this.InError)
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() },
                                };
                        }
                        else
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                                };
                        }
                    }
                }
                else
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>();
                }
            }
            else if (this.CurrentState == Asset.EnumSteps.AssetClassesTypeMakeState)
            {
                if (this.PermissionTypeMake.CanEdit)
                {
                    if (!this.IsCheckedOut)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                        };
                    }
                    else
                    {
                        if (this.InError)
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() },
                                };
                        }
                        else
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                                };
                        }
                    }
                }
                else
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>();
                }
            }
            else if (this.CurrentState == Asset.EnumSteps.BulkUpdate)
            {
                ObservableCollection<ActionCommand> tempActionCommand = new ObservableCollection<ActionCommand>();

                if (this.PermissionTypeFeature.CanEdit)
                {
                    tempActionCommand.Add(new ActionCommand { Parameter = Asset.EnumSteps.AssetClassesTypeAssignFeaturesState.ToString(), Command = new AssignFeature() });
                }

                if (this.PermissionTypeDetail.CanEdit)
                {
                    tempActionCommand.Add(new ActionCommand { Parameter = Asset.EnumSteps.AssetClassesTypeUpdateDepreciationState.ToString(), Command = new UpdateDepreciation() });
                }

                if (this.PermissionTypeMake.CanEdit)
                {
                    tempActionCommand.Add(new ActionCommand { Parameter = Asset.EnumSteps.AssetClassesTypeAssignMakeState.ToString(), Command = new AssignMake() });
                }

                tempActionCommand.Add(new ActionCommand { Parameter = Asset.EnumSteps.CancelBulkUpdate.ToString(), Command = new Cancel() });

                this.ActionCommands = new ObservableCollection<ActionCommand>();
                this.ActionCommands = tempActionCommand;
            }
            else if (this.CurrentState == Asset.EnumSteps.AssetClassesTypeAssignFeaturesState)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.SaveAssignFeature.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = Asset.EnumSteps.CancelAssignFeature.ToString(), Command = new Cancel() },
                    };
            }
            else if (this.CurrentState == Asset.EnumSteps.AssetClassesTypeAssignMakeState)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.SaveAssignMake.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = Asset.EnumSteps.CancelAssignMake.ToString(), Command = new Cancel() },
                    };
            }
            else if (this.CurrentState == Asset.EnumSteps.AssetClassesTypeUpdateDepreciationState)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.SaveUpdateDepreciation.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = Asset.EnumSteps.CancelUpdateDepreciation.ToString(), Command = new Cancel() },
                    };
            }
            else if (this.CurrentState == Asset.EnumSteps.Edit)
            {
                if (this.PermissionTypeDetail.CanEdit && this.InModeAdd)
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = Asset.EnumSteps.SaveAndAdd.ToString(), Command = new SaveAndAdd() },
                        new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                    };
                }
                else
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                    };
                }
            }
            else if (this.CurrentState == Asset.EnumSteps.EditAssignFeature)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.SaveAssignFeature.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = Asset.EnumSteps.CancelAssignFeature.ToString(), Command = new Cancel() },
                    };
            }
            else if (this.CurrentState == Asset.EnumSteps.EditAssignMake)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.SaveAssignMake.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = Asset.EnumSteps.CancelAssignMake.ToString(), Command = new Cancel() },
                    };
            }
            else if (this.CurrentState == Asset.EnumSteps.EditUpdateDepreciation)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.SaveUpdateDepreciation.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = Asset.EnumSteps.CancelUpdateDepreciation.ToString(), Command = new Cancel() },
                    };
            }
            else if (this.CurrentState == Asset.EnumSteps.Save)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                    };
            }
            else if (this.CurrentState == Asset.EnumSteps.SaveAssignFeature || this.CurrentState == Asset.EnumSteps.CancelAssignFeature)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.EditAssignFeature.ToString(), Command = new Edit() },
                    };
            }
            else if (this.CurrentState == Asset.EnumSteps.SaveAssignMake || this.CurrentState == Asset.EnumSteps.CancelAssignMake)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.EditAssignMake.ToString(), Command = new Edit() },
                    };
            }
            else if (this.CurrentState == Asset.EnumSteps.SaveUpdateDepreciation || this.CurrentState == Asset.EnumSteps.CancelUpdateDepreciation)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.EditUpdateDepreciation.ToString(), Command = new Edit() },
                    };
            }
            else if (this.CurrentState == Asset.EnumSteps.Cancel)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                    };
            }
            else if (this.CurrentState == Asset.EnumSteps.Error)
            {
                if (this.InModeAdd)
                {
                    if (this.CurrentTab == Asset.EnumSteps.AssetClassesTypeDetailState)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                            new ActionCommand { Parameter = Asset.EnumSteps.SaveAndAdd.ToString(), Command = new SaveAndAdd() },
                            new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
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
            }
            else if (this.CurrentState == Asset.EnumSteps.ErrorUpdateDepreciation)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.SaveUpdateDepreciation.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = Asset.EnumSteps.CancelUpdateDepreciation.ToString(), Command = new Cancel() },
                        new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() },
                    };
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
        /// The un lock asset classes type async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected async Task UnLockAssetClassesTypeAsync()
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
        /// The lock asset classes type async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected async Task<bool> LockAssetClassesTypeAsync()
        {
            if ((this._currentEnumStep == Asset.EnumSteps.Edit && this.SelectedTypeItem.EquipTypeId != 0)
                || this._currentEnumStep == Asset.EnumSteps.AssetClassesTypeMakeState || this.InModeBulkUpdate)
            {
                if (!this.InModeAdd)
                {
                    Dictionary<string, ItemLock> listItemLocks = new Dictionary<string, ItemLock>();

                    bool result = true;
                    int userId = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;

                    if (!this.InModeBulkUpdate)
                    {
                        if (this.ListItemLocks == null || this.ListItemLocks.Count == 0)
                        {
                            listItemLocks.Add(
                                "EquipType",
                                new ItemLock
                                {
                                    ListUniqueIdentifier =
                                        new List<string> { this.SelectedTypeItem.EquipTypeId.ToString() },
                                    UserId = userId,
                                    InstanceGUID = this.InstanceGUID
                                });

                            // Get CatFeatureId to lock table EquipTypeFeature
                            List<int> listItemTypeFeatureExist =
                                await AssetClassesTypeFunctions.GetIdTypeFeatureItem(this.SelectedTypeItem.EquipTypeId);
                            if (listItemTypeFeatureExist != null && listItemTypeFeatureExist.Count > 0)
                            {
                                listItemLocks.Add(
                                    "EquipTypeFeature",
                                    new ItemLock
                                    {
                                        ListUniqueIdentifier =
                                            listItemTypeFeatureExist.ConvertAll(x => x.ToString()),
                                        UserId = userId,
                                        InstanceGUID = this.InstanceGUID
                                    });
                            }
                        }

                        if (this._currentEnumStep == Asset.EnumSteps.AssetClassesTypeMakeState
                            || this.CurrentTab == Asset.EnumSteps.AssetClassesTypeMakeState)
                        {
                            if (this.ListItemLocks != null
                                && !this.ListItemLocks.Any(x => x.Key.Equals("xrefAssetTypeMake")))
                            {
                                listItemLocks.Add(
                                    "xrefAssetTypeMake",
                                    new ItemLock
                                    {
                                        ListUniqueIdentifier = new List<string> { "-1" },
                                        UserId = userId,
                                        InstanceGUID = this.InstanceGUID
                                    });
                            }
                        }
                    }
                    else
                    {
                        ObservableCollection<AssetClassesTypeRowItem> allItemsSelected =
                            new ObservableCollection<AssetClassesTypeRowItem>(
                                this.DynamicMainGridViewModel.SelectedItems.Cast<AssetClassesTypeRowItem>());

                        List<int> listRecordLock = allItemsSelected.Select(x => x.EquipTypeId).ToList();

                        listItemLocks.Add(
                            "EquipType",
                            new ItemLock
                            {
                                ListUniqueIdentifier = listRecordLock.ConvertAll(x => x.ToString()),
                                UserId = userId,
                                InstanceGUID = this.InstanceGUID
                            });

                        if (this._currentEnumStep == Asset.EnumSteps.EditAssignFeature
                            || this._currentEnumStep == Asset.EnumSteps.AssetClassesTypeAssignFeaturesState)
                        {
                            // Get CatFeatureId to lock table EquipCatFeature
                            List<int> listItemCatFeatureExist = new List<int>();

                            foreach (var recordIdLock in listRecordLock)
                            {
                                listItemCatFeatureExist.AddRange(
                                    await AssetClassesTypeFunctions.GetIdTypeFeatureItem(recordIdLock));
                            }

                            if (listItemCatFeatureExist.Count > 0)
                            {
                                listItemLocks.Add(
                                    "EquipTypeFeature",
                                    new ItemLock
                                    {
                                        ListUniqueIdentifier =
                                            listItemCatFeatureExist.ConvertAll(x => x.ToString()),
                                        UserId = userId,
                                        InstanceGUID = this.InstanceGUID
                                    });
                            }
                        }
                        else if (this._currentEnumStep == Asset.EnumSteps.EditAssignTypes
                                 || this._currentEnumStep == Asset.EnumSteps.AssetClassesTypeAssignMakeState)
                        {
                            listItemLocks.Add(
                                "xrefAssetTypeMake",
                                new ItemLock
                                {
                                    ListUniqueIdentifier = new List<string> { "-1" },
                                    UserId = userId,
                                    InstanceGUID = this.InstanceGUID
                                });
                        }
                    }

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

                    if (this.ListItemLocks == null || this.ListItemLocks.Count == 0)
                    {
                        this.ListItemLocks = listItemLocks;
                    }
                    else
                    {
                        if (listItemLocks.Count > 0)
                        {
                            this.ListItemLocks.Add(
                                listItemLocks.FirstOrDefault().Key,
                                listItemLocks.FirstOrDefault().Value);
                        }
                    }

                    this.IsCheckedOut = false;
                    return result;
                }
            }

            return true;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// The get permission.
        /// </summary>
        private void GetPermission()
        {
            this.PermissionTypeDetail = Authorisation.GetPermission(Components.SystemManagementAssetClassesType, Forms.AssetClassesTypeDetail);
            this.PermissionTypeFeature = Authorisation.GetPermission(Components.SystemManagementAssetClassesType, Forms.AssetClassesTypeFeatures);
            this.PermissionTypeMake = Authorisation.GetPermission(Components.SystemManagementAssetClassesType, Forms.AssetClassesTypeMake);
        }

        /// <summary>
        /// The main grid selected item changed.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private async void MainGridSelectedItemChanged(object value)
        {
            if (!this.DynamicMainGridViewModel.IsEnableHoverRow)
            {
                if (!this.InModeBulkUpdate)
                {
                    if (value != null)
                    {
                        this._selectedTypeItem = value as AssetClassesTypeRowItem;
                        await this.OnStepAsync("SelectedAssetClassesTypeItem");
                    }
                }
            }
        }

        /// <summary>
        /// The set selected type item.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SetSelectedTypeItem(AssetClassesTypeRowItem value)
        {
            this.SetField(ref this._selectedTypeItem, value, () => this.SelectedTypeItem);
            if (value != null)
            {
                if (this.IsCheckedOut)
                {
                    this.SetBusyAction(LoadingText);
                    this.IsCheckedOut = false;
                    this.InModeAdd = false;
                    this.InError = false;
                    this.ValidateNotError();
                    this.TypeDetailViewModel.ClearNotifyErrors();
                    this.SetBackgroundToNotEdit();
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesType, Asset.EnumSteps.Cancel);
                    this.CurrentState = Asset.EnumSteps.SelectedAssetClassesTypeItem;
                    await this.UnLockAssetClassesTypeAsync();
                    this.SetActionCommandsAsync();
                }

                // Load data for detail screen
                await this.GetDataSourceForDetailScreen();
                await this.GetDataSourceForFeaturesScreen();
                await this.GetDataSourceForMakeScreen();
                this.ResetBusyAction();
            }

            this.IsChanged = false;
        }

        /// <summary>
        /// The set selected new item.
        /// </summary>
        /// <param name="itemNew">
        /// The item new.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SetSelectedNewItem(EquipType itemNew)
        {
            this._selectedTypeItem = await AssetClassesTypeFunctions.GetDataDetailItemSelected(itemNew.EquipTypeId);

            var itemExist = this.ListDataGridItem.FirstOrDefault(x => x.EquipTypeId == itemNew.EquipTypeId);

            if (itemExist != null)
            {
                this.ListDataGridItem.Remove(itemExist);
                this.ListDataGridItem.Add(this._selectedTypeItem);
            }
            else
            {
                this.ListDataGridItem.Add(this._selectedTypeItem);
            }

            this.ListDataGridItem = this.ListDataGridItem.OrderBy(x => x.TypeDescription).ToList();
        }

        /// <summary>
        /// The set background to edit.
        /// </summary>
        private void SetBackgroundToEdit()
        {
            this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
            if (this.TypeDetailViewModel != null)
            {
                if (this.PermissionTypeDetail.CanEdit)
                {
                    this.TypeDetailViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                    this.TypeDetailViewModel.IsCheckedOut = true;
                }
            }

            if (this.TypeFeaturesViewModel != null)
            {
                if (this.PermissionTypeFeature.CanEdit)
                {
                    this.TypeFeaturesViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                    this.TypeFeaturesViewModel.IsCheckedOut = true;
                }
            }

            if (this.TypeMakeViewModel != null)
            {
                if (this.PermissionTypeMake.CanEdit)
                {
                    this.TypeMakeViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                    this.TypeMakeViewModel.IsCheckedOut = true;
                }
            }

            if (this.TypeAssignFeatureViewModel != null)
            {
                this.TypeAssignFeatureViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                this.TypeAssignFeatureViewModel.IsCheckedOut = true;
            }

            if (this.TypeAssignMakeViewModel != null)
            {
                this.TypeAssignMakeViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                this.TypeAssignMakeViewModel.IsCheckedOut = true;
            }

            if (this.TypeUpdateDepreciationViewModel != null)
            {
                this.TypeUpdateDepreciationViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                this.TypeUpdateDepreciationViewModel.IsCheckedOut = true;
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
            if (this.TypeDetailViewModel != null)
            {
                this.TypeDetailViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.TypeDetailViewModel.IsCheckedOut = false;
            }

            if (this.TypeFeaturesViewModel != null)
            {
                this.TypeFeaturesViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.TypeFeaturesViewModel.IsCheckedOut = false;
            }

            if (this.TypeMakeViewModel != null)
            {
                this.TypeMakeViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.TypeMakeViewModel.IsCheckedOut = false;
            }

            if (this.TypeAssignFeatureViewModel != null)
            {
                this.TypeAssignFeatureViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.TypeAssignFeatureViewModel.IsCheckedOut = false;
            }

            if (this.TypeAssignMakeViewModel != null)
            {
                this.TypeAssignMakeViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.TypeAssignMakeViewModel.IsCheckedOut = false;
            }

            if (this.TypeUpdateDepreciationViewModel != null)
            {
                this.TypeUpdateDepreciationViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.TypeUpdateDepreciationViewModel.IsCheckedOut = false;
            }
        }

        /// <summary>
        /// The get data source for grid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataSourceForGrid()
        {
            if (this.ListDataGridItem == null || this.InModeBulkUpdate || this.IsNeedToLoad)
            {
                List<AssetClassesTypeRowItem> data = await AssetClassesTypeFunctions.GetDataOnGrid();
                this.ListDataGridItem = data;
            }

            List<string> types = this.ListDataGridItem.Select(a => a.TypeDescription).Distinct().ToList();

            this.SourceFilterType = (from f in types 
                                     select new FilteringDataItem
                                                         {
                                                             Text = f,
                                                             IsSelected = true
                                                         }).Distinct().ToList();

            List<string> collateralDes = (await AssetClassesTypeFunctions.GetCollateralClassItems()).Select(a => a.Text).ToList();
            List<FilteringDataItem> sourceCollateral = (from f in collateralDes
                                                        select new FilteringDataItem
                                                                   {
                                                                       Text = f,
                                                                       IsSelected = true
                                                                   }).Distinct().ToList();

            sourceCollateral.Add(new FilteringDataItem { Text = "<None>", IsSelected = true });
            sourceCollateral = sourceCollateral.OrderBy(a => a.Text).ToList();

            List<string> bookDes = (await AssetClassesTypeFunctions.GetListBookDepnMethod()).Select(a => a.Text).ToList();
            List<FilteringDataItem> sourceBook = (from f in bookDes
                                                  select new FilteringDataItem
                                                             {
                                                                 Text = f,
                                                                 IsSelected = true
                                                             }).Distinct().ToList();

            sourceBook.Add(new FilteringDataItem { Text = "<None>", IsSelected = true });
            sourceBook = sourceBook.OrderBy(a => a.Text).ToList();

            List<string> taxDes = (await AssetClassesTypeFunctions.GetListTaxDepnMethod()).Select(a => a.Text).ToList();
            List<FilteringDataItem> sourceTax = (from f in taxDes
                                                 select new FilteringDataItem
                                                            {
                                                                Text = f,
                                                                IsSelected = true
                                                            }).Distinct().ToList();

            sourceTax.Add(new FilteringDataItem { Text = "<None>", IsSelected = true });
            sourceTax = sourceTax.OrderBy(a => a.Text).ToList();

            // Get data for Enable filter
            List<FilteringDataItem> sourceEnable = new List<FilteringDataItem>
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

            if (this.DynamicMainGridViewModel == null)
            {
                this.DynamicMainGridViewModel = new DynamicGridViewModel(typeof(AssetClassesTypeRowItem));
                this.DynamicMainGridViewModel.IsEnableHoverRow = false;
                this.DynamicMainGridViewModel.IsShowGroupPanel = true;
            }

            this.DynamicMainGridViewModel.MaxWidthGrid = 800;
            this.DynamicMainGridViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "TypeDescription", Header = "ASSET TYPE", IsSelectedColumn = true, Width = 250, MinWidth = 70, TextAlignment = TextAlignment.Left, FilteringTemplate = RadGridViewEnum.FilteringDataList, FilteringDataSource = this.SourceFilterType },
                                                                                             new DynamicColumn { ColumnName = "CollateralClassText", Header = "COLLATERAL CLASS", MinWidth = 100, Width = 180, TextAlignment = TextAlignment.Left, FilteringTemplate = RadGridViewEnum.FilteringDataList, FilteringDataSource = sourceCollateral },
                                                                                             new DynamicColumn { ColumnName = "BookDepnMethod",  Header = "BOOK DEPN METHOD", MinWidth = 95, Width = 100, TextAlignment = TextAlignment.Left, FilteringTemplate = RadGridViewEnum.FilteringDataList, FilteringDataSource = sourceBook },
                                                                                             new DynamicColumn { ColumnName = "SalvagePercentText",  Header = "SALVAGE (%)", MinWidth = 75, Width = 75, TextAlignment = TextAlignment.Center, HeaderTextAlignment = TextAlignment.Center },
                                                                                             new DynamicColumn { ColumnName = "BookDepnLife", Header = "BOOK DEPN LIFE (MONTHS)", MinWidth = 95, Width = 100, TextAlignment = TextAlignment.Center, HeaderTextAlignment = TextAlignment.Center },
                                                                                             new DynamicColumn { ColumnName = "BookDepnPercentText", Header = "BOOK DEPN (%)", MinWidth = 70, Width = 80, TextAlignment = TextAlignment.Center, HeaderTextAlignment = TextAlignment.Center },
                                                                                             new DynamicColumn { ColumnName = "TaxDepnMethod", Header = "TAX DEPN METHOD", MinWidth = 85, Width = 150, TextAlignment = TextAlignment.Left, FilteringTemplate = RadGridViewEnum.FilteringDataList, FilteringDataSource = sourceTax },
                                                                                             new DynamicColumn { ColumnName = "TaxDepnLife", Header = "TAX DEPN LIFE (MONTHS)", MinWidth = 95, Width = 100, TextAlignment = TextAlignment.Center, HeaderTextAlignment = TextAlignment.Center },
                                                                                             new DynamicColumn { ColumnName = "TaxDepnPercentText", Header = "TAX DEPN (%)", MinWidth = 70, Width = 75, TextAlignment = TextAlignment.Center, HeaderTextAlignment = TextAlignment.Center },
                                                                                             new DynamicColumn { ColumnName = "Enabled", Header = "ENABLED", MinWidth = 80, Width = 80, TextAlignment = TextAlignment.Center, HeaderTextAlignment = TextAlignment.Center, FilteringTemplate = RadGridViewEnum.FilteringDataList, FilteringDataSource = sourceEnable, ColumnTemplate = RadGridViewEnum.ColumnCheckedTemplate },
                                                                                         };
            this.DynamicMainGridViewModel.FilteringGenerate = true;
            this.DynamicMainGridViewModel.GridDataRows = this.ListDataGridItem.ToList<object>();
            this.DynamicMainGridViewModel.LoadRadGridView();
            this.DynamicMainGridViewModel.SelectedItemChanged += this.MainGridSelectedItemChanged;
            this.DynamicMainGridViewModel.GroupedItemChanged = this.GroupedChanged;
            this.IsNeedToLoad = false;
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
                this.DynamicMainGridViewModel.MaxWidthGrid = 10000;
            }
            else
            {
                this.DynamicMainGridViewModel.MaxWidthGrid = (int)e + 1;
            }
        }

        /// <summary>
        /// The update source for filter.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        private void UpdateSourceForGrid(string type = null)
        {
            DataRow editItem = null;
            if (this.IsNewItem)
            {
                this.DynamicMainGridViewModel.InsertRow(0, this.SelectedTypeItem);
            }
            else
            {
                foreach (var m in this.DynamicMainGridViewModel.MembersTable.Rows)
                {
                    if (this.SelectedTypeItem != null && this.SelectedTypeItem.EquipTypeId.ToString(CultureInfo.InvariantCulture) == m["EquipTypeId"].ToString())
                    {
                        editItem = m;
                        break;
                    }
                }

                if (editItem != null)
                {
                    int index = this.DynamicMainGridViewModel.MembersTable.Rows.IndexOf(editItem);
                    this.DynamicMainGridViewModel.UpdateRow(index, this.SelectedTypeItem);
                }
            }

            // update column Type 
            if (this.IsNewItem)
            {
                this.AddRecordFilter();
            }
            else
            {
                this.UpdateRecordFilter(type);
            }
        }

        /// <summary>
        /// The update multi source for filter.
        /// </summary>
        /// <param name="selectedTypeItems">
        /// The selected Type Items.
        /// </param>
        private void UpdateMultiSourceForFilter(ObservableCollection<AssetClassesTypeRowItem> selectedTypeItems)
        {
            foreach (var selectedTypeItem in selectedTypeItems)
            {
                DataRow editItem = null;

                foreach (var m in this.DynamicMainGridViewModel.MembersTable.Rows)
                {
                    if (selectedTypeItem != null && selectedTypeItem.EquipTypeId.ToString(CultureInfo.InvariantCulture) == m["EquipTypeId"].ToString())
                    {
                        editItem = m;
                        break;
                    }
                }

                if (editItem != null)
                {
                    int index = this.DynamicMainGridViewModel.MembersTable.Rows.IndexOf(editItem);
                    this.DynamicMainGridViewModel.UpdateRow(index, selectedTypeItem);
                }
            }
        }

        /// <summary>
        /// The update record filter.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        private void UpdateRecordFilter(string type)
        {
            if (!this.SourceFilterType.Select(a => a.Text).Contains(this.TypeDetailViewModel.TypeName))
            {
                FilteringDataItem item = this.SourceFilterType.FirstOrDefault(a => a.Text == type);
                int count = this.ListDataGridItem.Count(a => a.TypeDescription == type);

                // if more than 1 similar item, don't remove
                if (item != null && count == 0)
                {
                    this.SourceFilterType.Remove(item);
                }

                // add new item for filter
                this.SourceFilterType.Add(new FilteringDataItem
                                                  {
                                                      Text = this.TypeDetailViewModel.TypeName,
                                                  });

                this.SourceFilterType = this.SourceFilterType.OrderBy(a => a.Text).ToList();
                this.DynamicMainGridViewModel.UpdateSourceForFilter(this.SourceFilterType, 0, this.TypeDetailViewModel.TypeName);
            }
        }

        /// <summary>
        /// The add record filter.
        /// </summary>
        private void AddRecordFilter()
        {
            if (!this.SourceFilterType.Select(a => a.Text).Contains(this.TypeDetailViewModel.TypeName))
            {
                // add new item for filter
                this.SourceFilterType.Add(new FilteringDataItem
                                                  {
                                                      Text = this.TypeDetailViewModel.TypeName,
                                                  });
                this.SourceFilterType = this.SourceFilterType.OrderBy(a => a.Text).ToList();
                this.DynamicMainGridViewModel.AddSourceForFilter(this.SourceFilterType, 0, this.TypeDetailViewModel.TypeName);
            }
        }

        /// <summary>
        /// The generate data for all control.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GenerateDataForAllControl()
        {
            await this.TypeDetailViewModel.GenerateUserControlForDetailScreen();

            await this.TypeFeaturesViewModel.GenerateFeatureControl();

            await this.TypeMakeViewModel.GenerateAssetTypesControl();

            await this.TypeUpdateDepreciationViewModel.GenerateUserControlForDetailScreen();
        }

        /// <summary>
        /// The get data source for detail screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataSourceForDetailScreen()
        {
            await this.TypeDetailViewModel.GetDetailDataSource(this.SelectedTypeItem.EquipTypeId);
            this.TypeDetailViewModel.ContentItemChanged = this.AssetClassesTypeDetailViewModel_PropertyChanged;
        }

        /// <summary>
        /// The get data source for features screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataSourceForFeaturesScreen()
        {
            if (this.SelectedTypeItem != null)
            {
                await this.TypeFeaturesViewModel.GetFeatureDataSource(this.SelectedTypeItem.EquipTypeId, this.SelectedTypeItem.Enabled);
                this.TypeFeaturesViewModel.DetailContentChanged = this.AssetClassesTypeDetailViewModel_PropertyChanged;
            }
        }

        /// <summary>
        /// The get data source for make screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataSourceForMakeScreen()
        {
            if (this.SelectedTypeItem != null)
            {
                await this.TypeMakeViewModel.GetMakeDataSource(this.SelectedTypeItem.EquipTypeId, this.SelectedTypeItem.Enabled);
                this.TypeMakeViewModel.DetailContentChanged = this.AssetClassesTypeDetailViewModel_PropertyChanged;
            }
        }

        /// <summary>
        /// The get data source for update depreciation screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataSourceForUpdateDepreciationScreen()
        {
            if (this.DynamicMainGridViewModel.SelectedItems != null)
            {
                // Get Type name for UpdateDepreciation screen
                string updateDepreciationName = default(string);
                var allItemsSelected = new ObservableCollection<AssetClassesTypeRowItem>(this.DynamicMainGridViewModel.SelectedItems.Cast<AssetClassesTypeRowItem>());
                foreach (var item in allItemsSelected)
                {
                    var assetClassesTypeRowItem = allItemsSelected.FirstOrDefault();
                    var classesTypeRowItem = allItemsSelected.LastOrDefault();
                    if (classesTypeRowItem != null && (assetClassesTypeRowItem != null && (item.EquipTypeId == assetClassesTypeRowItem.EquipTypeId || item.EquipTypeId == classesTypeRowItem.EquipTypeId)))
                    {
                        updateDepreciationName = updateDepreciationName + item.TypeDescription;
                    }
                    else
                    {
                        updateDepreciationName = updateDepreciationName + ", " + item.TypeDescription;
                    }
                }

                await this.TypeUpdateDepreciationViewModel.GenerateUserControlForDetailScreen();
                await this.TypeUpdateDepreciationViewModel.GetUpdateDepreciationDataSource(updateDepreciationName, await AssetClassesTypeFunctions.GetDefaultDataForDetail());
                this.TypeUpdateDepreciationViewModel.ContentItemChanged = this.AssetClassesTypeDetailViewModel_PropertyChanged;
            }
        }

        /// <summary>
        /// The save all data for detail screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<EquipType> SaveAllDataForDetailScreen()
        {
            ObservableCollection<AssetClassesCategoryItemDetail> featureItemsSelected;
            ObservableCollection<AssetClassesCategoryItemDetail> makeItemsSelected;
            if (this.TypeFeaturesViewModel.FeaturesTabViewModel.SelectedItems != null)
            {
                featureItemsSelected = new ObservableCollection<AssetClassesCategoryItemDetail>(this.TypeFeaturesViewModel.FeaturesTabViewModel.SelectedItems.Cast<AssetClassesCategoryItemDetail>());
            }
            else
            {
                featureItemsSelected = null;
            }

            if (this.TypeMakeViewModel.AssetMakeTabViewModel.SelectedItems != null)
            {
                makeItemsSelected = new ObservableCollection<AssetClassesCategoryItemDetail>(this.TypeMakeViewModel.AssetMakeTabViewModel.SelectedItems.Cast<AssetClassesCategoryItemDetail>());
            }
            else
            {
                makeItemsSelected = null;
            }

            AssetClassesTypeRowItem dataItemDetail = this.TypeDetailViewModel.ParseTypeItemDetailToSave(this.SelectedTypeItem.EquipTypeId);

            if (!dataItemDetail.Enabled)
            {
                if (this.TypeFeaturesViewModel != null)
                {
                    this.TypeFeaturesViewModel.SetGridUnCheckedAll();
                }

                if (this.TypeMakeViewModel != null)
                {
                    this.TypeMakeViewModel.SetGridUnCheckedAll();
                }
            }

            return await AssetClassesTypeFunctions.SaveAllForDetailScreen(dataItemDetail, featureItemsSelected, makeItemsSelected);
        }

        /// <summary>
        /// The save all data for update depreciation screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SaveAllDataForUpdateDepreciationScreen()
        {
            var allItemsSelected =
                new ObservableCollection<AssetClassesTypeRowItem>(
                    this.DynamicMainGridViewModel.SelectedItems.Cast<AssetClassesTypeRowItem>());
            List<EquipType> result = await AssetClassesTypeFunctions.SaveAllForUpdateDepreciationScreen(
                    this.TypeUpdateDepreciationViewModel.GetBookItemsSaveAll(),
                    this.TypeUpdateDepreciationViewModel.GetTaxItemsSaveAll(),
                    this.TypeUpdateDepreciationViewModel.ParseTypeItemDetailToSave(),
                    allItemsSelected);

            if (result != null)
            {
                foreach (var equipType in result)
                {
                    AssetClassesTypeRowItem item =
                        allItemsSelected.FirstOrDefault(a => a.EquipTypeId == equipType.EquipTypeId);
                    if (item != null)
                    {
                        // book
                        item.BookDepnEffectiveLifeOption = equipType.BookDepnEffectiveLifeOption;
                        item.BookDepnLifeYear = equipType.BookDepnEffectiveLifeYear;
                        List<AssetClassesCategoryItemDetail> listBookDepn = await AssetClassesTypeFunctions.GetListBookDepnMethod();
                        var assetClassesTypeItemDetailBook = listBookDepn.FirstOrDefault(a => a.ItemId == equipType.BookDepnMethodID);
                        if (assetClassesTypeItemDetailBook != null)
                        {
                            item.BookDepnMethod = assetClassesTypeItemDetailBook.Text;
                        }
                        else
                        {
                            if (equipType.BookDepnMethodID == -1)
                            {
                                item.BookDepnMethod = "<None>";
                            }
                        }

                        item.BookDepnMethodId = equipType.BookDepnMethodID;
                        item.BookDepnUseCategoryDefaults = equipType.BookDepnUseCategoryDefaults;

                        // tax
                        item.TaxDepnEffectiveLifeOption = equipType.TaxDepnEffectiveLifeOption;
                        item.TaxDepnLifeYear = equipType.TaxDepnEffectiveLifeYear;
                        List<AssetClassesCategoryItemDetail> listTaxDepn = await AssetClassesTypeFunctions.GetListTaxDepnMethod();
                        var assetClassesTypeItemDetailTax = listTaxDepn.FirstOrDefault(a => a.ItemId == equipType.TaxDepnMethodID);
                        if (assetClassesTypeItemDetailTax != null)
                        {
                            item.TaxDepnMethod = assetClassesTypeItemDetailTax.Text;
                        }
                        else
                        {
                            if (equipType.TaxDepnMethodID == -1)
                            {
                                item.TaxDepnMethod = "<None>";
                            }
                        }

                        item.TaxDepnMethodId = equipType.TaxDepnMethodID;
                        item.TaxDepnUseCategoryDefaults = equipType.TaxDepnUseCategoryDefaults;
                    }
                }
            }

            this.UpdateMultiSourceForFilter(allItemsSelected);
        }

        /// <summary>
        /// The save all data for assign feature screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SaveAllDataForAssignFeatureScreen()
        {
            var allItemsSelected = new ObservableCollection<AssetClassesTypeRowItem>(this.DynamicMainGridViewModel.SelectedItems.Cast<AssetClassesTypeRowItem>());
            await AssetClassesTypeFunctions.SaveAllForAssignFeatureScreen(allItemsSelected, this.TypeAssignFeatureViewModel.ListItemsDragDrop.GroupDragDropSource);
        }

        /// <summary>
        /// The save all data for assign make screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SaveAllDataForAssignMakeScreen()
        {
            var allItemsSelected = new ObservableCollection<AssetClassesTypeRowItem>(this.DynamicMainGridViewModel.SelectedItems.Cast<AssetClassesTypeRowItem>());
            await AssetClassesTypeFunctions.SaveAllForAssignMakeScreen(allItemsSelected, this.TypeAssignMakeViewModel.ListItemsDragDrop.GroupDragDropSource);
        }

        /// <summary>
        /// The asset classes type detail view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetClassesTypeDetailViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut)
            {
                this.IsChanged = true;

                // IsTypeEnable change Makes and Features can not select
                /*
                if (e.PropertyName.IndexOf("IsTypeEnable", StringComparison.Ordinal) != -1)
                {
                    this.TypeFeaturesViewModel.ResetGridWhenChangeEnable(
                        this.SelectedTypeItem.EquipTypeId,
                        this.TypeDetailViewModel.IsTypeEnable);
                    this.TypeMakeViewModel.ResetGridWhenChangeEnable(
                        this.SelectedTypeItem.EquipTypeId,
                        this.TypeDetailViewModel.IsTypeEnable);
                }*/

                if (e.PropertyName.IndexOf("ItemTaxViewModel.EffectiveYear", StringComparison.Ordinal) != -1 || e.PropertyName.IndexOf("ItemTaxViewModel.EffectiveMonth", StringComparison.Ordinal) != -1)
                {
                    if (!this.InModeBulkUpdate)
                    {
                        if (this.ListErrorHyperlink.Count > 0)
                        {
                            if (this.TypeDetailViewModel.ListErrorHyperlink.Count > 0)
                            {
                                if (this.ListErrorHyperlink.Exists(x => x.HyperlinkHeader.Contains("Effective Life must be > 2 years for Diminishing Value Method")))
                                {
                                    return;
                                }

                                this.ListErrorHyperlink.Add(this.TypeDetailViewModel.ListErrorHyperlink.FirstOrDefault());

                                CustomHyperlink itemError = this.ListErrorHyperlink.FirstOrDefault(x => x.HyperlinkHeader.Contains("Tax Effective Life is required"));

                                if (itemError != null)
                                {
                                    this.ListErrorHyperlink.Remove(itemError);
                                }

                                this.OnErrorHyperlinkSelected();
                            }
                            else
                            {
                                CustomHyperlink itemError = this.ListErrorHyperlink.FirstOrDefault(x => x.HyperlinkHeader.Contains("Effective Life must be > 2 years for Diminishing Value Method"));
                                if (itemError != null)
                                {
                                    this.ListErrorHyperlink.Remove(itemError);

                                    if (this.ListErrorHyperlink.Count == 0)
                                    {
                                        this.InError = false;
                                        this.CurrentState = Asset.EnumSteps.Edit;
                                        this.SetActionCommandsAsync();
                                        this.ValidateNotError();
                                    }
                                }

                                var itemDepreciationDetailViewModel = this.TypeDetailViewModel.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                                if (itemDepreciationDetailViewModel != null && itemDepreciationDetailViewModel.ItemChildViewMdoel.SelectedItemCombobox.ItemId == 117)
                                {
                                    CustomHyperlink itemError2 = this.ListErrorHyperlink.FirstOrDefault(x => x.HyperlinkHeader.Contains("Tax Effective Life is required"));
                                    this.ListErrorHyperlink.Remove(itemError2);

                                    if (this.ListErrorHyperlink.Count == 0)
                                    {
                                        this.InError = false;
                                        this.CurrentState = Asset.EnumSteps.Edit;
                                        this.SetActionCommandsAsync();
                                        this.ValidateNotError();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (this.TypeDetailViewModel.ListErrorHyperlink.Count > 0)
                            {
                                this.ListErrorHyperlink = this.TypeDetailViewModel.ListErrorHyperlink;
                                this.InError = true;
                                this.CurrentState = Asset.EnumSteps.Error;
                                this.SetActionCommandsAsync();
                                this.OnErrorHyperlinkSelected();
                            }
                        }
                    }
                    else
                    {
                        if (this.ListErrorHyperlink.Count > 0)
                        {
                            if (this.TypeUpdateDepreciationViewModel.ListErrorHyperlink.Count > 0)
                            {
                                if (this.ListErrorHyperlink.Exists(x => x.HyperlinkHeader.Contains("Effective Life must be > 2 years for Diminishing Value Method")))
                                {
                                    return;
                                }

                                this.ListErrorHyperlink.Add(this.TypeUpdateDepreciationViewModel.ListErrorHyperlink.FirstOrDefault());

                                CustomHyperlink itemError = this.ListErrorHyperlink.FirstOrDefault(x => x.HyperlinkHeader.Contains("Tax Effective Life is required"));

                                if (itemError != null)
                                {
                                    this.ListErrorHyperlink.Remove(itemError);
                                }

                                this.OnErrorHyperlinkSelected();
                            }
                            else
                            {
                                CustomHyperlink itemError = this.ListErrorHyperlink.FirstOrDefault(x => x.HyperlinkHeader.Contains("Effective Life must be > 2 years for Diminishing Value Method"));
                                if (itemError != null)
                                {
                                    this.ListErrorHyperlink.Remove(itemError);

                                    if (this.ListErrorHyperlink.Count == 0)
                                    {
                                        this.InError = false;
                                        this.CurrentState = Asset.EnumSteps.AssetClassesTypeUpdateDepreciationState;
                                        this.SetActionCommandsAsync();
                                        this.ValidateNotError();
                                    }
                                }

                                var itemDepreciationDetailViewModel = this.TypeUpdateDepreciationViewModel.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                                if (itemDepreciationDetailViewModel != null && itemDepreciationDetailViewModel.ItemChildViewMdoel.SelectedItemCombobox.ItemId == 117)
                                {
                                    CustomHyperlink itemError2 = this.ListErrorHyperlink.FirstOrDefault(x => x.HyperlinkHeader.Contains("Tax Effective Life is required"));
                                    this.ListErrorHyperlink.Remove(itemError2);

                                    if (this.ListErrorHyperlink.Count == 0)
                                    {
                                        this.InError = false;
                                        this.CurrentState = Asset.EnumSteps.AssetClassesTypeUpdateDepreciationState;
                                        this.SetActionCommandsAsync();
                                        this.ValidateNotError();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (this.TypeUpdateDepreciationViewModel.ListErrorHyperlink.Count > 0)
                            {
                                this.ListErrorHyperlink = this.TypeUpdateDepreciationViewModel.ListErrorHyperlink;
                                this.InError = true;
                                this.CurrentState = Asset.EnumSteps.ErrorUpdateDepreciation;
                                this.SetActionCommandsAsync();
                                this.OnErrorHyperlinkSelected();
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
