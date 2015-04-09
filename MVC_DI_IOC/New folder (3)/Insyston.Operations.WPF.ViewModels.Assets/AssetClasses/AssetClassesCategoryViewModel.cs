// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetClassesCategoryViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetClassesCategoryViewModel type.
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
    using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.AssetCategory;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Enums;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;
    using Insyston.Operations.WPF.ViewModels.Common.Model.Filtering;

    using WPFDynamic.ViewModels.Controls;

    /// <summary>
    /// The asset classes category view model.
    /// </summary>
    public class AssetClassesCategoryViewModel : ViewModelUseCaseBase
    {
        #region Private Properties
        /// <summary>
        /// The permission category detail.
        /// </summary>
        public Permission PermissionCategoryDetail;

        /// <summary>
        /// The permission category feature.
        /// </summary>
        public Permission PermissionCategoryFeature;

        /// <summary>
        /// The permission category type.
        /// </summary>
        public Permission PermissionCategoryType;

        /// <summary>
        /// The loading text.
        /// </summary>
        private const string LoadingText = "Please Wait Loading ...";

        /// <summary>
        /// The current enumerable step.
        /// </summary>
        private Asset.EnumSteps _currentEnumStep;

        /// <summary>
        /// The _selected category item.
        /// </summary>
        private AssetClassesCategoryRowItem _selectedCategoryItem;

        /// <summary>
        /// The _list data grid item.
        /// </summary>
        private List<AssetClassesCategoryRowItem> _listDataGridItem;

        /// <summary>
        /// The _dynamic main grid view model.
        /// </summary>
        private DynamicGridViewModel _dynamicMainGridViewModel;

        /// <summary>
        /// The _category detail view model.
        /// </summary>
        private AssetCategoryDetailViewModel _categoryDetailViewModel;

        /// <summary>
        /// The _category features view model.
        /// </summary>
        private AssetCategoryFeaturesViewModel _categoryFeaturesViewModel;

        /// <summary>
        /// The _category asset types view model.
        /// </summary>
        private AssetCategoryAssetTypesViewModel _categoryAssetTypesViewModel;

        /// <summary>
        /// The _category update depreciation view model.
        /// </summary>
        private AssetCategoryUpdateDepreciationViewModel _categoryUpdateDepreciationViewModel;

        /// <summary>
        /// The _category assign feature view model.
        /// </summary>
        private AssetCategoryAssignFeatureViewModel _categoryAssignFeatureViewModel;

        /// <summary>
        /// The _category assign types view model.
        /// </summary>
        private AssetCategoryAssignTypesViewModel _categoryAssignTypesViewModel;

        /// <summary>
        /// The _source filter category.
        /// </summary>
        private List<FilteringDataItem> _sourceFilterCategory;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetClassesCategoryViewModel"/> class.
        /// </summary>
        public AssetClassesCategoryViewModel()
        {
            this.InstanceGUID = Guid.NewGuid();

            this.DynamicMainGridViewModel = new DynamicGridViewModel(typeof(AssetClassesCategoryRowItem))
            {
                IsEnableHoverRow = false,
                IsShowGroupPanel = true
            };

            this.CategoryDetailViewModel = new AssetCategoryDetailViewModel();
            this.CategoryFeaturesViewModel = new AssetCategoryFeaturesViewModel();
            this.CategoryAssetTypesViewModel = new AssetCategoryAssetTypesViewModel();
            this.CategoryUpdateDepreciationViewModel = new AssetCategoryUpdateDepreciationViewModel();
            this.CategoryAssignFeatureViewModel = new AssetCategoryAssignFeatureViewModel();
            this.CategoryAssignTypesViewModel = new AssetCategoryAssignTypesViewModel();
            this.ListItemLocks = new Dictionary<string, ItemLock>();
        }
        #endregion

        #region Public Properties
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
        /// Gets or sets a value indicating whether is need to load.
        /// </summary>
        public bool IsNeedToLoad { get; set; }

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

        /// <summary>
        /// Gets or sets the current state.
        /// </summary>
        public Asset.EnumSteps CurrentState { get; set; }

        /// <summary>
        /// Gets or sets the current tab.
        /// </summary>
        public Asset.EnumSteps CurrentTab { get; set; }

        /// <summary>
        /// Gets or sets the list item locks.
        /// </summary>
        public Dictionary<string, ItemLock> ListItemLocks { get; set; }

        /// <summary>
        /// Gets or sets the source filter category.
        /// </summary>
        public List<FilteringDataItem> SourceFilterCategory 
        {
            get
            {
                return this._sourceFilterCategory;
            }

            set
            {
                this.SetField(ref this._sourceFilterCategory, value, () => this.SourceFilterCategory);
            }
        } 

        /// <summary>
        /// Gets or sets the selected category item.
        /// </summary>
        public AssetClassesCategoryRowItem SelectedCategoryItem
        {
            get
            {
                return this._selectedCategoryItem;
            }

            set
            {
                this.SetSelectedCategoryItem(value);
            }
        }

        /// <summary>
        /// Gets or sets the list data grid item.
        /// </summary>
        public List<AssetClassesCategoryRowItem> ListDataGridItem
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
        /// Gets or sets the category detail view model.
        /// </summary>
        public AssetCategoryDetailViewModel CategoryDetailViewModel
        {
            get
            {
                return this._categoryDetailViewModel;
            }

            set
            {
                this.SetField(ref this._categoryDetailViewModel, value, () => this.CategoryDetailViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the category features view model.
        /// </summary>
        public AssetCategoryFeaturesViewModel CategoryFeaturesViewModel
        {
            get
            {
                return this._categoryFeaturesViewModel;
            }

            set
            {
                this.SetField(ref this._categoryFeaturesViewModel, value, () => this.CategoryFeaturesViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the category asset types view model.
        /// </summary>
        public AssetCategoryAssetTypesViewModel CategoryAssetTypesViewModel
        {
            get
            {
                return this._categoryAssetTypesViewModel;
            }

            set
            {
                this.SetField(ref this._categoryAssetTypesViewModel, value, () => this.CategoryAssetTypesViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the category update depreciation view model.
        /// </summary>
        public AssetCategoryUpdateDepreciationViewModel CategoryUpdateDepreciationViewModel
        {
            get
            {
                return this._categoryUpdateDepreciationViewModel;
            }

            set
            {
                this.SetField(ref this._categoryUpdateDepreciationViewModel, value, () => this.CategoryUpdateDepreciationViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the category assign feature view model.
        /// </summary>
        public AssetCategoryAssignFeatureViewModel CategoryAssignFeatureViewModel
        {
            get
            {
                return this._categoryAssignFeatureViewModel;
            }

            set
            {
                this.SetField(ref this._categoryAssignFeatureViewModel, value, () => this.CategoryAssignFeatureViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the category assign types view model.
        /// </summary>
        public AssetCategoryAssignTypesViewModel CategoryAssignTypesViewModel
        {
            get
            {
                return this._categoryAssignTypesViewModel;
            }

            set
            {
                this.SetField(ref this._categoryAssignTypesViewModel, value, () => this.CategoryAssignTypesViewModel);
            }
        }

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
                case Asset.EnumSteps.SelectedAssetClassesCategoryItem:
                    this.SetBusyAction(LoadingText);
                    this.CurrentState = Asset.EnumSteps.SelectedAssetClassesCategoryItem;
                    await this.GetDataSourceForDetailScreen();
                    if (!this.OnLoaded)
                    {
                        this.OnStepChanged("MainContentState");
                        this.OnLoaded = true;
                    }

                    await this.GetDataSourceForFeaturesScreen();
                    await this.GetDataSourceForAssetTypesScreen();
                    this.ResetBusyAction();
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.SelectedAssetClassesCategoryItem, this.DynamicMainGridViewModel.SelectedItem);
                    break;
                case Asset.EnumSteps.AssetClassesCategoryDetailState:
                    this.CurrentState = Asset.EnumSteps.AssetClassesCategoryDetailState;
                    this.CurrentTab = Asset.EnumSteps.AssetClassesCategoryDetailState;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                    }

                    break;
                case Asset.EnumSteps.AssetClassesCategoryFeaturesState:
                    this.CurrentState = Asset.EnumSteps.AssetClassesCategoryFeaturesState;
                    this.CurrentTab = Asset.EnumSteps.AssetClassesCategoryFeaturesState;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                    }

                    break;
                case Asset.EnumSteps.AssetClassesCategoryAssetTypesState:
                    bool canProcess = true;
                    if (this.IsCheckedOut)
                    {
                        canProcess = await this.LockAssetClassesCategoryAsync();
                    }

                    if (canProcess)
                    {
                        if (this.ListItemLocks.Count > 0)
                        {
                            this.IsCheckedOut = true;
                        }

                        this.CurrentState = Asset.EnumSteps.AssetClassesCategoryAssetTypesState;
                        this.CurrentTab = Asset.EnumSteps.AssetClassesCategoryAssetTypesState;
                        if (this.OnStoryBoardChanged != null)
                        {
                            this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                        }
                    }
                    else
                    {
                        this.IsCheckedOut = true;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, EnumSteps.SelectOldTabHyperlink);
                    }

                    break;
                case Asset.EnumSteps.AssetClassesCategoryAssignFeaturesState:
                    this.SetBusyAction(LoadingText);
                    if (this.DynamicMainGridViewModel.SelectedItems.Count() > 0)
                    {
                        if (await this.LockAssetClassesCategoryAsync())
                        {
                            if (this.OnStoryBoardChanged != null)
                            {
                                this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                            }

                            this.OnStepChanged("BulkUpdateState");
                            this.SetBackgroundToEdit();
                            this.CurrentState = Asset.EnumSteps.AssetClassesCategoryAssignFeaturesState;
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.AssetClassesCategoryAssignFeaturesState);
                            await this.GetListFeatureItemsForAssignFeaturesScreen();
                            this.IsCheckedOut = true;
                        }
                    }
                    else
                    {
                        this.ShowMessageAsync("Select one or more record to Assign Features.", "Confirm - Asset Classes Category");
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.AssetClassesCategoryAssignTypesState:
                    this.SetBusyAction(LoadingText);
                    if (this.DynamicMainGridViewModel.SelectedItems.Count() > 0)
                    {
                        if (await this.LockAssetClassesCategoryAsync())
                        {
                            if (this.OnStoryBoardChanged != null)
                            {
                                this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                            }

                            this.OnStepChanged("BulkUpdateState");
                            this.SetBackgroundToEdit();
                            this.CurrentState = Asset.EnumSteps.AssetClassesCategoryAssignTypesState;
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.AssetClassesCategoryAssignTypesState);
                            await this.GetListTypesItemsForAssignTypesScreen();
                            this.IsCheckedOut = true;
                        }
                    }
                    else
                    {
                        this.ShowMessageAsync("Select one or more record to Assign Types.", "Confirm - Asset Classes Category");
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.AssetClassesCategoryUpdateDepreciationState:
                    this.SetBusyAction(LoadingText);
                    if (this.DynamicMainGridViewModel.SelectedItems.Count() > 0)
                    {
                        if (await this.LockAssetClassesCategoryAsync())
                        {
                            if (this.OnStoryBoardChanged != null)
                            {
                                this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                            }

                            this.OnStepChanged("BulkUpdateState");
                            this.SetBackgroundToEdit();
                            this.CurrentState = Asset.EnumSteps.AssetClassesCategoryUpdateDepreciationState;
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.AssetClassesCategoryUpdateDepreciationState);
                            await this.GetDataSourceForUpdateDepreciationScreen();
                            this.IsCheckedOut = true;
                        }
                    }
                    else
                    {
                        this.ShowMessageAsync("Select one or more record to Update Depreciation.", "Confirm - Asset Classes Category");
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
                        this.OnStoryBoardChanged("AssetClassesCategoryDetailState");
                    }

                    this.OnStepChanged("MainContentState");
                    this.CurrentState = Asset.EnumSteps.Add;
                    this.SetBackgroundToEdit();
                    await this.SetSelectedCategoryItem(new AssetClassesCategoryRowItem { EquipCategoryId = 0, Enabled = true });
                    this.IsNewItem = true;
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, EnumSteps.Add);
                    await this.OnStepAsync(Asset.EnumSteps.Edit);
                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.Edit:
                    this.SetBusyAction(LoadingText);
                    if (await this.LockAssetClassesCategoryAsync())
                    {
                        this.SetBackgroundToEdit();

                        this.CurrentState = Asset.EnumSteps.Edit;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.Edit);
                        if (this.CategoryFeaturesViewModel.FeaturesTabViewModel != null)
                        {
                            if (this.PermissionCategoryFeature.CanEdit)
                            {
                                this.CategoryFeaturesViewModel.FeaturesTabViewModel.IsEnableHoverRow = true;
                            }
                        }

                        if (this.CategoryAssetTypesViewModel.AssetTypesTabViewModel != null)
                        {
                            if (this.PermissionCategoryType.CanEdit)
                            {
                                this.CategoryAssetTypesViewModel.AssetTypesTabViewModel.IsEnableHoverRow = true;
                            }
                        }

                        this.IsCheckedOut = true;
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.EditAssignFeature:
                    if (await this.LockAssetClassesCategoryAsync())
                    {
                        this.SetBackgroundToEdit();
                        this.CurrentState = Asset.EnumSteps.EditAssignFeature;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.AssetClassesCategoryAssignFeaturesState);
                        this.IsCheckedOut = true;
                    }

                    break;
                case Asset.EnumSteps.EditAssignTypes:
                    if (this.DynamicMainGridViewModel.SelectedItems != null)
                    {
                        if (await this.LockAssetClassesCategoryAsync())
                        {
                            this.SetBackgroundToEdit();
                            this.CurrentState = Asset.EnumSteps.EditAssignTypes;
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.AssetClassesCategoryAssignTypesState);
                            this.IsCheckedOut = true;
                        }
                    }

                    break;
                case Asset.EnumSteps.EditUpdateDepreciation:
                    if (this.DynamicMainGridViewModel.SelectedItems != null)
                    {
                        if (await this.LockAssetClassesCategoryAsync())
                        {
                            this.SetBackgroundToEdit();
                            this.CurrentState = Asset.EnumSteps.EditUpdateDepreciation;
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.AssetClassesCategoryUpdateDepreciationState);
                            this.IsCheckedOut = true;
                        }
                    }

                    break;
                case Asset.EnumSteps.Save:
                    this.CategoryDetailViewModel.Validate();
                    if (this.CategoryDetailViewModel.HasErrors == false)
                    {
                        if (!this.IsNewItem)
                        {
                            await this.UnLockAssetClassesCategoryAsync();
                        }

                        this.ListErrorHyperlink.Clear();
                        this.CategoryDetailViewModel.ListErrorHyperlink.Clear();
                        this.InError = false;
                        this.ValidateNotError();
                        this.SetBusyAction(LoadingText);
                        this.CurrentState = Asset.EnumSteps.Save;

                        this.CategoryDetailViewModel.ClearNotifyErrors();
                        this.SetBackgroundToNotEdit();
                        
                        string previousCate = this._selectedCategoryItem.Category;
                        EquipCategory catItemSaved = await this.SaveAllDataForDetailScreen();
                        await this.SetSelectedNewItem(catItemSaved);
                        if (this.IsNewItem)
                        {
                            this.UpdateSourceForGrid();
                            this.IsNewItem = false;
                            this.InModeAdd = false;
                            await this.CategoryFeaturesViewModel.GetFeatureDataSource(catItemSaved.EquipCatId, catItemSaved.Enabled);
                            await this.CategoryAssetTypesViewModel.GetAssetTypesDataSource(catItemSaved.EquipCatId, catItemSaved.Enabled);
                        }
                        else
                        {
                            this.UpdateSourceForGrid(previousCate);
                            await this.CategoryFeaturesViewModel.GetFeatureDataSource(catItemSaved.EquipCatId, catItemSaved.Enabled);
                            await this.CategoryAssetTypesViewModel.GetAssetTypesDataSource(catItemSaved.EquipCatId, catItemSaved.Enabled);
                        }

                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.Save, catItemSaved);
                        this.CategoryFeaturesViewModel.FeaturesTabViewModel.IsEnableHoverRow = false;
                        this.CategoryAssetTypesViewModel.AssetTypesTabViewModel.IsEnableHoverRow = false;
                    }
                    else
                    {
                        this.InError = true;
                        this.CurrentState = Asset.EnumSteps.Error;
                        this.ListErrorHyperlink = this.CategoryDetailViewModel.ListErrorHyperlink;
                        foreach (var itemError in this.ListErrorHyperlink)
                        {
                            itemError.Action = HyperLinkAction.AssetClassesCategoryDetailState;
                            itemError.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
                        }

                        this.OnErrorHyperlinkSelected();
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.SaveAndAdd:
                    this.CategoryDetailViewModel.Validate();
                    if (this.CategoryDetailViewModel.HasErrors == false)
                    {
                        await this.OnStepAsync(Asset.EnumSteps.Save);
                        await this.OnStepAsync(Asset.EnumSteps.Add);
                    }
                    else
                    {
                        this.InError = true;
                        this.CurrentState = Asset.EnumSteps.Error;
                        this.ListErrorHyperlink = this.CategoryDetailViewModel.ListErrorHyperlink;
                        foreach (var itemError in this.ListErrorHyperlink)
                        {
                            itemError.Action = HyperLinkAction.AssetClassesCategoryDetailState;
                            itemError.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
                        }

                        this.OnErrorHyperlinkSelected();
                    }

                    break;
                case Asset.EnumSteps.SaveAssignFeature:
                    this.CurrentState = Asset.EnumSteps.SaveAssignFeature;
                    await this.UnLockAssetClassesCategoryAsync();
                    this.SetBackgroundToNotEdit();
                    await this.SaveAllDataForAssignFeatureScreen();
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.SaveAssignFeature);
                    break;
                case Asset.EnumSteps.SaveAssignTypes:
                    this.CurrentState = Asset.EnumSteps.SaveAssignTypes;
                    await this.UnLockAssetClassesCategoryAsync();
                    this.SetBackgroundToNotEdit();
                    await this.SaveAllDataForAssignTypesScreen();
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.SaveAssignTypes);
                    break;
                case Asset.EnumSteps.SaveUpdateDepreciation:
                    this.CategoryUpdateDepreciationViewModel.Validate();
                    if (this.CategoryUpdateDepreciationViewModel.HasErrors == false)
                    {
                        this.InError = false;
                        this.ListErrorHyperlink.Clear();
                        this.CategoryUpdateDepreciationViewModel.ListErrorHyperlink.Clear();
                        this.SetBusyAction(LoadingText);
                        this.CurrentState = Asset.EnumSteps.SaveUpdateDepreciation;
                        await this.UnLockAssetClassesCategoryAsync();
                        this.CategoryUpdateDepreciationViewModel.ClearNotifyErrors();
                        this.SetBackgroundToNotEdit();
                        await this.SaveAllDataForUpdateDepreciationScreen();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.SaveUpdateDepreciation);
                    }
                    else
                    {
                        this.InError = true;
                        this.CurrentState = Asset.EnumSteps.ErrorUpdateDepreciation;
                        this.ListErrorHyperlink = this.CategoryUpdateDepreciationViewModel.ListErrorHyperlink;
                        this.OnErrorHyperlinkSelected();
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.Cancel:
                    if (await this.CheckIfUnSavedChanges())
                    {
                        this.InError = false;
                        this.ListErrorHyperlink.Clear();
                        this.CategoryDetailViewModel.ListErrorHyperlink.Clear();
                        this.ValidateNotError();
                        this.SetBusyAction();
                        this.CurrentState = Asset.EnumSteps.Cancel;
                        this.CategoryDetailViewModel.ClearNotifyErrors();
                        await this.UnLockAssetClassesCategoryAsync();
                        this.SetBackgroundToNotEdit();

                        if (!this.IsCheckedOut)
                        {
                            if (this.CategoryFeaturesViewModel.FeaturesTabViewModel != null)
                            {
                                this.CategoryFeaturesViewModel.FeaturesTabViewModel.IsEnableHoverRow = false;
                            }

                            if (this.CategoryAssetTypesViewModel.AssetTypesTabViewModel != null)
                            {
                                this.CategoryAssetTypesViewModel.AssetTypesTabViewModel.IsEnableHoverRow = false;
                            }
                        }

                        if (this.IsNewItem)
                        {
                            this.IsNewItem = false;
                            this.OnCancelNewItem(EnumScreen.AssetClassesCategory);
                            await this.OnStepAsync(Asset.EnumSteps.MainViewState);
                            this.InModeAdd = false;
                        }
                        else
                        {
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.Cancel);
                            await this.OnStepAsync(Asset.EnumSteps.SelectedAssetClassesCategoryItem);
                        }

                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.CancelAssignFeature:
                    if (await this.CheckIfUnSavedChanges())
                    {
                        await this.UnLockAssetClassesCategoryAsync();
                        await this.GetListFeatureItemsForAssignFeaturesScreen();
                        this.CurrentState = Asset.EnumSteps.CancelAssignFeature;
                        this.SetBackgroundToNotEdit();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.CancelAssignFeature);
                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                    }

                    break;
                case Asset.EnumSteps.CancelAssignTypes:
                    if (await this.CheckIfUnSavedChanges())
                    {
                        await this.UnLockAssetClassesCategoryAsync();
                        await this.GetListTypesItemsForAssignTypesScreen();
                        this.CurrentState = Asset.EnumSteps.CancelAssignTypes;
                        this.SetBackgroundToNotEdit();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.CancelAssignTypes);
                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                    }

                    break;
                case Asset.EnumSteps.CancelUpdateDepreciation:
                    if (await this.CheckIfUnSavedChanges())
                    {
                        this.InError = false;
                        this.ListErrorHyperlink.Clear();
                        this.CategoryUpdateDepreciationViewModel.ListErrorHyperlink.Clear();
                        this.SetBusyAction(LoadingText);
                        await this.UnLockAssetClassesCategoryAsync();
                        await this.GetDataSourceForUpdateDepreciationScreen();
                        this.CurrentState = Asset.EnumSteps.CancelUpdateDepreciation;
                        this.SetBackgroundToNotEdit();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.CancelUpdateDepreciation);
                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                    }

                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.CancelBulkUpdate:
                    if (await this.CheckIfUnSavedChanges())
                    {
                        this.CurrentState = Asset.EnumSteps.CancelBulkUpdate;
                        this.InModeBulkUpdate = true;
                        this.DynamicMainGridViewModel.SelectedRows = new List<object>();
                        this.DynamicMainGridViewModel.IsEnableHoverRow = false;
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
        /// The get list types items for assign types screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetListTypesItemsForAssignTypesScreen()
        {
            if (this.DynamicMainGridViewModel.SelectedItems != null)
            {
                var allItemsSelected = new ObservableCollection<AssetClassesCategoryRowItem>(this.DynamicMainGridViewModel.SelectedItems.Cast<AssetClassesCategoryRowItem>());
                await this.CategoryAssignTypesViewModel.GetListTypesItems(allItemsSelected);
                this.CategoryAssignTypesViewModel.ContentItemChanged += this.AssetClassesCategoryDetailViewModel_PropertyChanged;
            }
        }

        /// <summary>
        /// The load data for main grid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task LoadData()
        {
            await this.GetDataSourceForGrid();
        }

        /// <summary>
        /// The disable hover grid.
        /// </summary>
        public void DisableHover()
        {
            this.DynamicMainGridViewModel.IsEnableHoverRow = false;
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
                var allItemsSelected = new ObservableCollection<AssetClassesCategoryRowItem>(this.DynamicMainGridViewModel.SelectedItems.Cast<AssetClassesCategoryRowItem>());
                await this.CategoryAssignFeatureViewModel.GetListFeatureItems(allItemsSelected);
                this.CategoryAssignFeatureViewModel.ContentItemChanged += this.AssetClassesCategoryDetailViewModel_PropertyChanged;
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
            if (!this.InModeBulkUpdate)
            {
                this._currentEnumStep = Asset.EnumSteps.Dispose;
            }

            return this.UnLockAssetClassesCategoryAsync();
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
                confirmViewModel.Title = "Confirm Save - Asset Classes Category";
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
                this._currentEnumStep = Asset.EnumSteps.Dispose;
                this.IsChanged = false;
                this.IsCheckedOut = false;
                this.UnLockAssetClassesCategoryAsync();

                base.Dispose();
            }));
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
                if (this.PermissionCategoryDetail.CanAdd && this.PermissionCategoryDetail.CanEdit)
                {
                    tempActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.Add.ToString(), Command = new Add() });
                }

                if (this.PermissionCategoryDetail.CanEdit || this.PermissionCategoryFeature.CanEdit || this.PermissionCategoryType.CanEdit)
                {
                    tempActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.BulkUpdate.ToString(), Command = new BulkUpdate() });
                }

                this.ActionCommands = tempActionCommands;
            }
            else if (this.CurrentState == Asset.EnumSteps.SelectedAssetClassesCategoryItem)
            {
                if (this.PermissionCategoryDetail.CanEdit && this.CurrentTab == Asset.EnumSteps.AssetClassesCategoryDetailState)
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                    };
                }
                else if (this.PermissionCategoryFeature.CanEdit && this.CurrentTab == Asset.EnumSteps.AssetClassesCategoryFeaturesState)
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                    };
                }
                else if (this.PermissionCategoryType.CanEdit && this.CurrentTab == Asset.EnumSteps.AssetClassesCategoryAssetTypesState)
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
            else if (this.CurrentState == Asset.EnumSteps.AssetClassesCategoryDetailState)
            {
                if (this.PermissionCategoryDetail.CanEdit)
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
                        if (this.PermissionCategoryDetail.CanAdd && this.InModeAdd)
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
            else if (this.CurrentState == Asset.EnumSteps.AssetClassesCategoryFeaturesState)
            {
                if (this.PermissionCategoryFeature.CanEdit)
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
            else if (this.CurrentState == Asset.EnumSteps.AssetClassesCategoryAssetTypesState)
            {
                if (this.PermissionCategoryType.CanEdit)
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

                if (this.PermissionCategoryFeature.CanEdit)
                {
                    tempActionCommand.Add(new ActionCommand { Parameter = Asset.EnumSteps.AssetClassesCategoryAssignFeaturesState.ToString(), Command = new AssignFeature() });
                }

                if (this.PermissionCategoryDetail.CanEdit)
                {
                    tempActionCommand.Add(new ActionCommand { Parameter = Asset.EnumSteps.AssetClassesCategoryUpdateDepreciationState.ToString(), Command = new UpdateDepreciation() });
                }

                if (this.PermissionCategoryType.CanEdit)
                {
                    tempActionCommand.Add(new ActionCommand { Parameter = Asset.EnumSteps.AssetClassesCategoryAssignTypesState.ToString(), Command = new AssignType() });
                }

                tempActionCommand.Add(new ActionCommand { Parameter = Asset.EnumSteps.CancelBulkUpdate.ToString(), Command = new Cancel() });

                this.ActionCommands = new ObservableCollection<ActionCommand>();
                this.ActionCommands = tempActionCommand;
            }
            else if (this.CurrentState == Asset.EnumSteps.AssetClassesCategoryAssignFeaturesState)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.SaveAssignFeature.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = Asset.EnumSteps.CancelAssignFeature.ToString(), Command = new Cancel() },
                    };
            }
            else if (this.CurrentState == Asset.EnumSteps.AssetClassesCategoryAssignTypesState)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.SaveAssignTypes.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = Asset.EnumSteps.CancelAssignTypes.ToString(), Command = new Cancel() },
                    };
            }
            else if (this.CurrentState == Asset.EnumSteps.AssetClassesCategoryUpdateDepreciationState)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.SaveUpdateDepreciation.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = Asset.EnumSteps.CancelUpdateDepreciation.ToString(), Command = new Cancel() },
                    };
            }
            else if (this.CurrentState == Asset.EnumSteps.Edit)
            {
                if (this.PermissionCategoryDetail.CanAdd && this.InModeAdd)
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
            else if (this.CurrentState == Asset.EnumSteps.EditAssignTypes)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.SaveAssignTypes.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = Asset.EnumSteps.CancelAssignTypes.ToString(), Command = new Cancel() },
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
            else if (this.CurrentState == Asset.EnumSteps.SaveAssignTypes || this.CurrentState == Asset.EnumSteps.CancelAssignTypes)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.EditAssignTypes.ToString(), Command = new Edit() },
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
                    if (this.CurrentTab == Asset.EnumSteps.AssetClassesCategoryDetailState)
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
        /// The un lock asset classes category async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected async Task UnLockAssetClassesCategoryAsync()
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
        /// The lock asset classes category async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected async Task<bool> LockAssetClassesCategoryAsync()
        {
            if ((this._currentEnumStep == Asset.EnumSteps.Edit && this.SelectedCategoryItem.EquipCategoryId != 0)
                || this._currentEnumStep == Asset.EnumSteps.AssetClassesCategoryAssetTypesState || this.InModeBulkUpdate)
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
                                "EquipCategory",
                                new ItemLock
                                {
                                    ListUniqueIdentifier =
                                        new List<string>
                                                {
                                                    this.SelectedCategoryItem.EquipCategoryId
                                                        .ToString()
                                                },
                                    UserId = userId,
                                    InstanceGUID = this.InstanceGUID
                                });

                            // Get CatFeatureId to lock table EquipCatFeature
                            List<int> listItemCatFeatureExist =
                                await
                                AssetClassesCategoryFunctions.GetIdCatFeatureItem(
                                    this.SelectedCategoryItem.EquipCategoryId);
                            if (listItemCatFeatureExist != null && listItemCatFeatureExist.Count > 0)
                            {
                                listItemLocks.Add(
                                    "EquipCatFeature",
                                    new ItemLock
                                    {
                                        ListUniqueIdentifier =
                                            listItemCatFeatureExist.ConvertAll(x => x.ToString()),
                                        UserId = userId,
                                        InstanceGUID = this.InstanceGUID
                                    });
                            }
                        }
                        if (this._currentEnumStep == Asset.EnumSteps.AssetClassesCategoryAssetTypesState
                            || this.CurrentTab == Asset.EnumSteps.AssetClassesCategoryAssetTypesState)
                        {
                            if (this.ListItemLocks != null
                                && !this.ListItemLocks.Any(x => x.Key.Equals("xrefAssetCategoryType")))
                            {
                                listItemLocks.Add(
                                    "xrefAssetCategoryType",
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
                        ObservableCollection<AssetClassesCategoryRowItem> allItemsSelected =
                            new ObservableCollection<AssetClassesCategoryRowItem>(
                                this.DynamicMainGridViewModel.SelectedItems.Cast<AssetClassesCategoryRowItem>());

                        List<int> listRecordLock = allItemsSelected.Select(x => x.EquipCategoryId).ToList();

                        listItemLocks.Add(
                            "EquipCategory",
                            new ItemLock
                            {
                                ListUniqueIdentifier = listRecordLock.ConvertAll(x => x.ToString()),
                                UserId = userId,
                                InstanceGUID = this.InstanceGUID
                            });

                        if (this._currentEnumStep == Asset.EnumSteps.EditAssignFeature
                            || this._currentEnumStep == Asset.EnumSteps.AssetClassesCategoryAssignFeaturesState)
                        {
                            // Get CatFeatureId to lock table EquipCatFeature
                            var listItemCatFeatureExist = new List<int>();

                            foreach (var recordIdLock in listRecordLock)
                            {
                                listItemCatFeatureExist.AddRange(
                                    await AssetClassesCategoryFunctions.GetIdCatFeatureItem(recordIdLock));
                            }

                            if (listItemCatFeatureExist.Count > 0)
                            {
                                listItemLocks.Add(
                                    "EquipCatFeature",
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
                                 || this._currentEnumStep == Asset.EnumSteps.AssetClassesCategoryAssignTypesState)
                        {
                            listItemLocks.Add(
                                "xrefAssetCategoryType",
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
                        this._selectedCategoryItem = value as AssetClassesCategoryRowItem;
                        await this.OnStepAsync("SelectedAssetClassesCategoryItem");
                    }
                }
            }
        }

        /// <summary>
        /// The set selected category item.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SetSelectedCategoryItem(AssetClassesCategoryRowItem value)
        {
            this.SetField(ref this._selectedCategoryItem, value, () => this.SelectedCategoryItem);
            if (value != null)
            {
                if (this.IsCheckedOut)
                {
                    this.SetBusyAction(LoadingText);
                    this.IsCheckedOut = false;
                    this.InModeAdd = false;
                    this.InError = false;
                    this.ValidateNotError();
                    this.CategoryDetailViewModel.ClearNotifyErrors();
                    this.SetBackgroundToNotEdit();
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesCategory, Asset.EnumSteps.Cancel);
                    this.CurrentState = Asset.EnumSteps.SelectedAssetClassesCategoryItem;
                    await this.UnLockAssetClassesCategoryAsync();
                    this.SetActionCommandsAsync();
                }

                // Load data for detail screen
                await this.GetDataSourceForDetailScreen();
                await this.GetDataSourceForFeaturesScreen();
                await this.GetDataSourceForAssetTypesScreen();
                this.ResetBusyAction();
            }

            this.IsChanged = false;
        }

        /// <summary>
        /// The get permission.
        /// Get permission for Detail, Feature, Type screen
        /// </summary>
        private void GetPermission()
        {
            this.PermissionCategoryDetail = Authorisation.GetPermission(Components.SystemManagementAssetClassesCategory, Forms.AssetClassesCategoryDetail);
            this.PermissionCategoryFeature = Authorisation.GetPermission(Components.SystemManagementAssetClassesCategory, Forms.AssetClassesCategoryFeatures);
            this.PermissionCategoryType = Authorisation.GetPermission(Components.SystemManagementAssetClassesCategory, Forms.AssetClassesCategoryTypes);
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
        private async Task SetSelectedNewItem(EquipCategory itemNew)
        {
            this._selectedCategoryItem = await AssetClassesCategoryFunctions.GetDataDetailItemSelected(itemNew.EquipCatId);

            var itemExist = this.ListDataGridItem.FirstOrDefault(x => x.EquipCategoryId == itemNew.EquipCatId);

            if (itemExist != null)
            {
                this.ListDataGridItem.Remove(itemExist);
                this.ListDataGridItem.Add(this._selectedCategoryItem);
            }
            else
            {
                this.ListDataGridItem.Add(this._selectedCategoryItem);
            }

            this.ListDataGridItem = this.ListDataGridItem.OrderBy(x => x.Category).ToList();
        }

        /// <summary>
        /// The set background to edit.
        /// </summary>
        private void SetBackgroundToEdit()
        {
            this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
            if (this.CategoryDetailViewModel != null)
            {
                if (this.PermissionCategoryDetail.CanEdit)
                {
                    this.CategoryDetailViewModel.GridStyle =
                        this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                    this.CategoryDetailViewModel.IsCheckedOut = true;
                }
            }

            if (this.CategoryFeaturesViewModel != null)
            {
                if (this.PermissionCategoryFeature.CanEdit)
                {
                    this.CategoryFeaturesViewModel.GridStyle =
                        this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                    this.CategoryFeaturesViewModel.IsCheckedOut = true;
                }
            }

            if (this.CategoryAssetTypesViewModel != null)
            {
                if (this.PermissionCategoryType.CanEdit)
                {
                    this.CategoryAssetTypesViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                    this.CategoryAssetTypesViewModel.IsCheckedOut = true;
                }
            }

            if (this.CategoryAssignFeatureViewModel != null)
            {
                this.CategoryAssignFeatureViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                this.CategoryAssignFeatureViewModel.IsCheckedOut = true;
            }

            if (this.CategoryAssignTypesViewModel != null)
            {
                this.CategoryAssignTypesViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                this.CategoryAssignTypesViewModel.IsCheckedOut = true;
            }

            if (this.CategoryUpdateDepreciationViewModel != null)
            {
                this.CategoryUpdateDepreciationViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                this.CategoryUpdateDepreciationViewModel.IsCheckedOut = true;
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
            if (this.CategoryDetailViewModel != null)
            {
                this.CategoryDetailViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.CategoryDetailViewModel.IsCheckedOut = false;
            }

            if (this.CategoryFeaturesViewModel != null)
            {
                this.CategoryFeaturesViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.CategoryFeaturesViewModel.IsCheckedOut = false;
            }

            if (this.CategoryAssetTypesViewModel != null)
            {
                this.CategoryAssetTypesViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.CategoryAssetTypesViewModel.IsCheckedOut = false;
            }

            if (this.CategoryAssignFeatureViewModel != null)
            {
                this.CategoryAssignFeatureViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.CategoryAssignFeatureViewModel.IsCheckedOut = false;
            }

            if (this.CategoryAssignTypesViewModel != null)
            {
                this.CategoryAssignTypesViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.CategoryAssignTypesViewModel.IsCheckedOut = false;
            }

            if (this.CategoryUpdateDepreciationViewModel != null)
            {
                this.CategoryUpdateDepreciationViewModel.GridStyle = this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.CategoryUpdateDepreciationViewModel.IsCheckedOut = false;
            }
        }

        /// <summary>
        /// The get data source for main grid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataSourceForGrid()
        {
            if (this.ListDataGridItem == null || this.InModeBulkUpdate || this.IsNeedToLoad)
            {
                List<AssetClassesCategoryRowItem> data = await AssetClassesCategoryFunctions.GetDataOnGrid();
                this.ListDataGridItem = data;
            }

            List<string> cates = this.ListDataGridItem.Select(a => a.Category).Distinct().ToList();

            // Get data for Category filter
            this.SourceFilterCategory = (from f in cates
                                                      select
                                                          new FilteringDataItem
                                                              {
                                                                  Text = f,
                                                                  IsSelected = true
                                                              }).Distinct().ToList();

            // Get data for Book Depn filter
            List<string> bookDes =
                (await AssetClassesCategoryFunctions.GetListBookDepnMethod()).Select(a => a.Text).ToList();
            List<FilteringDataItem> sourceBook =
                (from f in bookDes select new FilteringDataItem { Text = f, IsSelected = true }).Distinct().ToList();

            sourceBook.Add(new FilteringDataItem { Text = "<None>", IsSelected = true });
            sourceBook = sourceBook.OrderBy(a => a.Text).ToList();

            // Get data for Tax Depn filter
            List<string> taxDes =
                (await AssetClassesCategoryFunctions.GetListTaxDepnMethod()).Select(a => a.Text).ToList();
            List<FilteringDataItem> sourceTax =
                (from f in taxDes select new FilteringDataItem { Text = f, IsSelected = true }).Distinct().ToList();

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
                this.DynamicMainGridViewModel = new DynamicGridViewModel(typeof(AssetClassesCategoryRowItem))
                                                    {
                                                        IsEnableHoverRow = false,
                                                        IsShowGroupPanel = true
                                                    };
            }

            this.DynamicMainGridViewModel.MaxWidthGrid = 800;
            this.DynamicMainGridViewModel.GridColumns = new List<DynamicColumn>
                                                            {
                                                                new DynamicColumn
                                                                    {
                                                                        ColumnName = "Category",
                                                                        Header = "CATEGORY",
                                                                        IsSelectedColumn = true,
                                                                        TextAlignment = TextAlignment.Left,
                                                                        Width = 430,
                                                                        MinWidth = 100,
                                                                        FilteringTemplate = RadGridViewEnum.FilteringDataList,
                                                                        FilteringDataSource = this.SourceFilterCategory
                                                                    },
                                                                new DynamicColumn
                                                                    {
                                                                        ColumnName = "BookDepnMethod",
                                                                        Header = "BOOK DEPN METHOD",
                                                                        MinWidth = 95,
                                                                        TextAlignment = TextAlignment.Left,
                                                                        Width = 100,
                                                                        FilteringTemplate = RadGridViewEnum.FilteringDataList,
                                                                        FilteringDataSource = sourceBook
                                                                    },
                                                                new DynamicColumn
                                                                    {
                                                                        ColumnName = "SalvagePercentText",
                                                                        Header = "SALVAGE (%)",
                                                                        MinWidth = 75,
                                                                        Width = 75,
                                                                        TextAlignment = TextAlignment.Center,
                                                                        HeaderTextAlignment = TextAlignment.Center
                                                                    },
                                                                new DynamicColumn
                                                                    {
                                                                        ColumnName = "BookDepnLife",
                                                                        Header = "BOOK DEPN LIFE (MONTHS)",
                                                                        MinWidth = 95,
                                                                        Width = 100,
                                                                        TextAlignment = TextAlignment.Center,
                                                                        HeaderTextAlignment = TextAlignment.Center
                                                                    },
                                                                new DynamicColumn
                                                                    {
                                                                        ColumnName = "BookDepnPercentText",
                                                                        Header = "BOOK DEPN (%)",
                                                                        MinWidth = 75,
                                                                        Width = 80,
                                                                        TextAlignment = TextAlignment.Center,
                                                                        HeaderTextAlignment = TextAlignment.Center
                                                                    },
                                                                new DynamicColumn
                                                                    {
                                                                        ColumnName = "TaxDepnMethod",
                                                                        Header = "TAX DEPN METHOD",
                                                                        TextAlignment = TextAlignment.Left,
                                                                        Width = 150,
                                                                        MinWidth = 95,
                                                                        FilteringTemplate = RadGridViewEnum.FilteringDataList,
                                                                        FilteringDataSource = sourceTax
                                                                    },
                                                                new DynamicColumn
                                                                    {
                                                                        ColumnName = "TaxDepnLife",
                                                                        Header = "TAX DEPN LIFE (MONTHS)",
                                                                        MinWidth = 95,
                                                                        Width = 100,
                                                                        TextAlignment = TextAlignment.Center,
                                                                        HeaderTextAlignment = TextAlignment.Center
                                                                    },
                                                                new DynamicColumn
                                                                    {
                                                                        ColumnName = "TaxDepnPercentText",
                                                                        Header = "TAX DEPN (%)",
                                                                        MinWidth = 70,
                                                                        Width = 75,
                                                                        TextAlignment = TextAlignment.Center,
                                                                        HeaderTextAlignment = TextAlignment.Center
                                                                    },
                                                                new DynamicColumn
                                                                    {
                                                                        ColumnName = "Enabled",
                                                                        Header = "ENABLED",
                                                                        MinWidth = 80,
                                                                        Width = 80,
                                                                        TextAlignment = TextAlignment.Center,
                                                                        HeaderTextAlignment = TextAlignment.Center,
                                                                        FilteringTemplate = RadGridViewEnum.FilteringDataList,
                                                                        FilteringDataSource = sourceEnable,
                                                                        ColumnTemplate = RadGridViewEnum.ColumnCheckedTemplate
                                                                    },
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
        /// <param name="category">
        /// The category.
        /// </param>
        private void UpdateSourceForGrid(string category = null)
        {
            // update data for grid
            DataRow editItem = null;
            if (this.IsNewItem)
            {
                this.DynamicMainGridViewModel.InsertRow(0, this.SelectedCategoryItem);
            }
            else
            {
                foreach (var m in this.DynamicMainGridViewModel.MembersTable.Rows)
                {
                    if (this.SelectedCategoryItem != null && this.SelectedCategoryItem.EquipCategoryId.ToString() == m["EquipCategoryId"].ToString())
                    {
                        editItem = m;
                        break;
                    }
                }

                if (editItem != null)
                {
                    int index = this.DynamicMainGridViewModel.MembersTable.Rows.IndexOf(editItem);
                    this.DynamicMainGridViewModel.UpdateRow(index, this.SelectedCategoryItem);
                }
            }
            
            // update column Category 
            if (this.IsNewItem)
            {
                this.AddRecordFilter();
            }
            else
            {
                this.UpdateRecordFilter(category);
            }
        }

        /// <summary>
        /// The update record filter.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        private void UpdateRecordFilter(string category)
        {
            if (!this.SourceFilterCategory.Select(a => a.Text).Contains(this.CategoryDetailViewModel.CategoryName))
            {
                FilteringDataItem item = this.SourceFilterCategory.FirstOrDefault(a => a.Text == category);
                int count = this.ListDataGridItem.Count(a => a.Category == category);

                // if more than 1 similar item, don't remove
                if (item != null && count == 0)
                {
                    this.SourceFilterCategory.Remove(item);
                }

                // add new item for filter
                this.SourceFilterCategory.Add(new FilteringDataItem
                                                  {
                                                      Text = this.CategoryDetailViewModel.CategoryName,
                                                  });

                this.SourceFilterCategory = this.SourceFilterCategory.OrderBy(a => a.Text).ToList();
                this.DynamicMainGridViewModel.UpdateSourceForFilter(this.SourceFilterCategory, 0, this.CategoryDetailViewModel.CategoryName);
            }
        }

        /// <summary>
        /// The add record filter.
        /// </summary>
        private void AddRecordFilter()
        {
            if (!this.SourceFilterCategory.Select(a => a.Text).Contains(this.CategoryDetailViewModel.CategoryName))
            {
                // add new item for filter
                this.SourceFilterCategory.Add(new FilteringDataItem
                                                  {
                                                      Text = this.CategoryDetailViewModel.CategoryName,
                                                  });
                this.SourceFilterCategory = this.SourceFilterCategory.OrderBy(a => a.Text).ToList();
                this.DynamicMainGridViewModel.AddSourceForFilter(this.SourceFilterCategory, 0, this.CategoryDetailViewModel.CategoryName);
            }
        }

        /// <summary>
        /// The update multi source for filter.
        /// </summary>
        /// <param name="selectedCatItems">
        /// The selected cat items.
        /// </param>
        private void UpdateMultiSourceForFilter(ObservableCollection<AssetClassesCategoryRowItem> selectedCatItems)
        {
            foreach (var selectedCatItem in selectedCatItems)
            {
                DataRow editItem = null;

                foreach (var m in this.DynamicMainGridViewModel.MembersTable.Rows)
                {
                    if (selectedCatItem != null && selectedCatItem.EquipCategoryId.ToString(CultureInfo.InvariantCulture) == m["EquipCategoryId"].ToString())
                    {
                        editItem = m;
                        break;
                    }
                }

                if (editItem != null)
                {
                    int index = this.DynamicMainGridViewModel.MembersTable.Rows.IndexOf(editItem);
                    this.DynamicMainGridViewModel.UpdateRow(index, selectedCatItem);
                }
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
            await this.CategoryDetailViewModel.GenerateUserControlForDetailScreen();

            await this.CategoryFeaturesViewModel.GenerateFeatureControl();

            await this.CategoryAssetTypesViewModel.GenerateAssetTypesControl();

            await this.CategoryUpdateDepreciationViewModel.GenerateUserControlForDetailScreen();
        }

        /// <summary>
        /// The get data source for detail screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataSourceForDetailScreen()
        {
            await this.CategoryDetailViewModel.GetDetailDataSource(this.SelectedCategoryItem.EquipCategoryId);
            this.CategoryDetailViewModel.ContentItemChanged = this.AssetClassesCategoryDetailViewModel_PropertyChanged;
        }

        /// <summary>
        /// The get data source for features screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataSourceForFeaturesScreen()
        {
            if (this.SelectedCategoryItem != null)
            {
                await this.CategoryFeaturesViewModel.GetFeatureDataSource(this.SelectedCategoryItem.EquipCategoryId, this.SelectedCategoryItem.Enabled);
                this.CategoryFeaturesViewModel.DetailContentChanged = this.AssetClassesCategoryDetailViewModel_PropertyChanged;
            }
        }

        /// <summary>
        /// The get data source for asset types screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataSourceForAssetTypesScreen()
        {
            if (this.SelectedCategoryItem != null)
            {
                await this.CategoryAssetTypesViewModel.GetAssetTypesDataSource(this.SelectedCategoryItem.EquipCategoryId, this.SelectedCategoryItem.Enabled);
                this.CategoryAssetTypesViewModel.DetailContentChanged = this.AssetClassesCategoryDetailViewModel_PropertyChanged;
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
                // Get category name for UpdateDepreciation screen
                string updateDepreciationName = default(string);
                var allItemsSelected = new ObservableCollection<AssetClassesCategoryRowItem>(this.DynamicMainGridViewModel.SelectedItems.Cast<AssetClassesCategoryRowItem>());
                foreach (var item in allItemsSelected)
                {
                    updateDepreciationName += item.Category + ", ";
                }

                await this.CategoryUpdateDepreciationViewModel.GenerateUserControlForDetailScreen();
                await this.CategoryUpdateDepreciationViewModel.GetUpdateDepreciationDataSource(updateDepreciationName, await AssetClassesCategoryFunctions.GetDefaultDataForDetail());
                this.CategoryUpdateDepreciationViewModel.ContentItemChanged = this.AssetClassesCategoryDetailViewModel_PropertyChanged;
            }
        }

        /// <summary>
        /// The save all data for detail screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<EquipCategory> SaveAllDataForDetailScreen()
        {
            ObservableCollection<AssetClassesCategoryItemDetail> featureItemsSelected;
            ObservableCollection<AssetClassesCategoryItemDetail> typesItemsSelected;
            if (this.CategoryFeaturesViewModel.FeaturesTabViewModel.SelectedItems != null)
            {
                featureItemsSelected = new ObservableCollection<AssetClassesCategoryItemDetail>(this.CategoryFeaturesViewModel.FeaturesTabViewModel.SelectedItems.Cast<AssetClassesCategoryItemDetail>());
            }
            else
            {
                featureItemsSelected = null;
            }

            if (this.CategoryAssetTypesViewModel.AssetTypesTabViewModel.SelectedItems != null)
            {
                typesItemsSelected = new ObservableCollection<AssetClassesCategoryItemDetail>(this.CategoryAssetTypesViewModel.AssetTypesTabViewModel.SelectedItems.Cast<AssetClassesCategoryItemDetail>());
            }
            else
            {
                typesItemsSelected = null;
            }

            AssetClassesCategoryRowItem dataItemDetail = this.CategoryDetailViewModel.ParseCategoryItemDetailToSave(this.SelectedCategoryItem.EquipCategoryId);

            if (!dataItemDetail.Enabled)
            {
                if (this.CategoryAssetTypesViewModel != null)
                {
                    this.CategoryAssetTypesViewModel.SetGridUnCheckedAll();
                }

                if (this.CategoryFeaturesViewModel != null)
                {
                    this.CategoryFeaturesViewModel.SetGridUnCheckedAll();
                }
            }

            return await AssetClassesCategoryFunctions.SaveAllForDetailScreen(dataItemDetail, featureItemsSelected, typesItemsSelected);    
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
                new ObservableCollection<AssetClassesCategoryRowItem>(
                    this.DynamicMainGridViewModel.SelectedItems.Cast<AssetClassesCategoryRowItem>());
            List<EquipCategory> result = await AssetClassesCategoryFunctions.SaveAllForUpdateDepreciationScreen(
                    this.CategoryUpdateDepreciationViewModel.GetBookItemsSaveAll(),
                    this.CategoryUpdateDepreciationViewModel.GetTaxItemsSaveAll(),
                    this.CategoryUpdateDepreciationViewModel.ParseCategoryItemDetailToSave(),
                    allItemsSelected);

            if (result != null)
            {
                foreach (var equipCat in result)
                {
                    AssetClassesCategoryRowItem item =
                        allItemsSelected.FirstOrDefault(a => a.EquipCategoryId == equipCat.EquipCatId);
                    if (item != null)
                    {
                        // book
                        item.BookDepnEffectiveLifeOption = equipCat.BookDepnEffectiveLifeOption;
                        item.BookDepnLifeYear = equipCat.BookDepnEffectiveLifeYear;
                        List<AssetClassesCategoryItemDetail> listBookDepn = await AssetClassesCategoryFunctions.GetListBookDepnMethod();
                        var assetClassesTypeItemDetailBook = listBookDepn.FirstOrDefault(a => a.ItemId == equipCat.BookDepnMethodID);
                        if (assetClassesTypeItemDetailBook != null)
                        {
                            item.BookDepnMethod = assetClassesTypeItemDetailBook.Text;
                        }
                        else
                        {
                            if (equipCat.BookDepnMethodID == -1)
                            {
                                item.BookDepnMethod = "<None>";
                            }
                        }

                        item.BookDepnMethodId = equipCat.BookDepnMethodID;

                        // tax
                        item.TaxDepnEffectiveLifeOption = equipCat.TaxDepnEffectiveLifeOption;
                        item.TaxDepnLifeYear = equipCat.TaxDepnEffectiveLifeYear;
                        List<AssetClassesCategoryItemDetail> listTaxDepn = await AssetClassesCategoryFunctions.GetListTaxDepnMethod();
                        var assetClassesTypeItemDetailTax = listTaxDepn.FirstOrDefault(a => a.ItemId == equipCat.TaxDepnMethodID);
                        if (assetClassesTypeItemDetailTax != null)
                        {
                            item.TaxDepnMethod = assetClassesTypeItemDetailTax.Text;
                        }
                        else
                        {
                            if (equipCat.TaxDepnMethodID == -1)
                            {
                                item.TaxDepnMethod = "<None>";
                            }
                        }

                        item.TaxDepnMethodId = equipCat.TaxDepnMethodID;
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
            var allItemsSelected = new ObservableCollection<AssetClassesCategoryRowItem>(this.DynamicMainGridViewModel.SelectedItems.Cast<AssetClassesCategoryRowItem>());
            await AssetClassesCategoryFunctions.SaveAllForAssignFeatureScreen(allItemsSelected, this.CategoryAssignFeatureViewModel.ListItemsDragDrop.GroupDragDropSource);
        }

        /// <summary>
        /// The save all data for assign types screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SaveAllDataForAssignTypesScreen()
        {
            var allItemsSelected = new ObservableCollection<AssetClassesCategoryRowItem>(this.DynamicMainGridViewModel.SelectedItems.Cast<AssetClassesCategoryRowItem>());
            await AssetClassesCategoryFunctions.SaveAllForAssignTypesScreen(allItemsSelected, this.CategoryAssignTypesViewModel.ListItemsDragDrop.GroupDragDropSource);
        }

        /// <summary>
        /// The asset classes category detail view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetClassesCategoryDetailViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut)
            {
                this.IsChanged = true;

                // IsCategoryEnable change Types and Features can not select
                /*if (e.PropertyName.IndexOf("IsCategoryEnable", StringComparison.Ordinal) != -1)
                {
                    this.CategoryFeaturesViewModel.ResetGridWhenChangeEnable(
                        this.SelectedCategoryItem.EquipCategoryId,
                        this.CategoryDetailViewModel.IsCategoryEnable);
                    this.CategoryAssetTypesViewModel.ResetGridWhenChangeEnable(
                        this.SelectedCategoryItem.EquipCategoryId,
                        this.CategoryDetailViewModel.IsCategoryEnable);
                }*/

                if (e.PropertyName.IndexOf("ItemTaxViewModel.EffectiveYear", StringComparison.Ordinal) != -1 || e.PropertyName.IndexOf("ItemTaxViewModel.EffectiveMonth", StringComparison.Ordinal) != -1)
                {
                    if (!this.InModeBulkUpdate)
                    {
                        if (this.ListErrorHyperlink.Count > 0)
                        {
                            if (this.CategoryDetailViewModel.ListErrorHyperlink.Count > 0)
                            {
                                if (this.ListErrorHyperlink.Exists(x => x.HyperlinkHeader.Contains("Effective Life must be > 2 years for Diminishing Value Method")))
                                {
                                    return;
                                }

                                this.ListErrorHyperlink.Add(this.CategoryDetailViewModel.ListErrorHyperlink.FirstOrDefault());

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

                                var itemDepreciationDetailViewModel = this.CategoryDetailViewModel.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
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
                            if (this.CategoryDetailViewModel.ListErrorHyperlink.Count > 0)
                            {
                                this.ListErrorHyperlink = this.CategoryDetailViewModel.ListErrorHyperlink;
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
                            if (this.CategoryUpdateDepreciationViewModel.ListErrorHyperlink.Count > 0)
                            {
                                if (this.ListErrorHyperlink.Exists(x => x.HyperlinkHeader.Contains("Effective Life must be > 2 years for Diminishing Value Method")))
                                {
                                    return;
                                }

                                this.ListErrorHyperlink.Add(this.CategoryUpdateDepreciationViewModel.ListErrorHyperlink.FirstOrDefault());

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
                                        this.CurrentState = Asset.EnumSteps.AssetClassesCategoryUpdateDepreciationState;
                                        this.SetActionCommandsAsync();
                                        this.ValidateNotError();
                                    }
                                }

                                var itemDepreciationDetailViewModel = this.CategoryUpdateDepreciationViewModel.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                                if (itemDepreciationDetailViewModel != null && itemDepreciationDetailViewModel.ItemChildViewMdoel.SelectedItemCombobox.ItemId == 117)
                                {
                                    CustomHyperlink itemError2 = this.ListErrorHyperlink.FirstOrDefault(x => x.HyperlinkHeader.Contains("Tax Effective Life is required"));
                                    this.ListErrorHyperlink.Remove(itemError2);

                                    if (this.ListErrorHyperlink.Count == 0)
                                    {
                                        this.InError = false;
                                        this.CurrentState = Asset.EnumSteps.AssetClassesCategoryUpdateDepreciationState;
                                        this.SetActionCommandsAsync();
                                        this.ValidateNotError();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (this.CategoryUpdateDepreciationViewModel.ListErrorHyperlink.Count > 0)
                            {
                                this.ListErrorHyperlink = this.CategoryUpdateDepreciationViewModel.ListErrorHyperlink;
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
