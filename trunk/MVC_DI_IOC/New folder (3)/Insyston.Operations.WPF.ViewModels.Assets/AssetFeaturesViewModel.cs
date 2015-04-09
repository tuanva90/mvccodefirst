// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetFeaturesViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The dynamic grid view model.
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
    using Insyston.Operations.Bussiness.Assets;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.Locking.Helpers;
    using Insyston.Operations.Logging;
    using Insyston.Operations.Model;
    using Insyston.Operations.Security;
    using Insyston.Operations.Security.Model;
    using Insyston.Operations.WPF.ViewModel.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetFeatures;
    using Insyston.Operations.WPF.ViewModels.Assets.Validation;
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
    /// The story board changed.
    /// </summary>
    /// <param name="storyBoard">
    /// The story board.
    /// </param>
    public delegate void StoryBoardChanged(string storyBoard);
    
    /// <summary>
    /// The asset features view model.
    /// </summary>
    public class AssetFeaturesViewModel : ViewModelUseCaseBase
    {
        #region Variables
        /// <summary>
        /// The loading text.
        /// </summary>
        private const string LoadingText = "Please Wait Loading ...";

                /// <summary>
        /// The _current enum step.
        /// </summary>
        private Asset.EnumSteps _currentEnumStep;

                /// <summary>
        /// The _dynamic feature type view model.
        /// </summary>
        private DynamicGridViewModel _dynamicFeatureTypeViewModel;

        /// <summary>
        /// The _asset features detail view model.
        /// </summary>
        private AssetFeaturesDetailViewModel _assetFeaturesDetailViewModel;

        /// <summary>
        /// The _asset features assigned to view model.
        /// </summary>
        private AssetFeaturesAssignedToViewModel _assetFeaturesAssignedToViewModel;

        /// <summary>
        /// The _asset features assign feature view model.
        /// </summary>
        private AssetFeaturesAssignFeatureViewModel _assetFeaturesAssignFeatureViewModel;

        /// <summary>
        /// The _all feature types.
        /// </summary>
        private ObservableCollection<FeatureType> _allFeatureTypes;

        /// <summary>
        /// The _selected feature type.
        /// </summary>
        private FeatureType _selectedFeatureType;

        /// <summary>
        /// The _permission feature detail.
        /// </summary>
        private Permission _permissionFeatureDetail;

        /// <summary>
        /// The _permission feature assign to.
        /// </summary>
        private Permission _permissionFeatureAssignTo;

        /// <summary>
        /// Gets or sets the list item locks.
        /// </summary>
        private Dictionary<string, ItemLock> ListItemLocks { get; set; }

        #endregion

        #region Event


        /// <summary>
        /// The on story board changed.
        /// </summary>
        public event StoryBoardChanged OnStoryBoardChanged;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetFeaturesViewModel"/> class.
        /// </summary>
        public AssetFeaturesViewModel()
        {
            this.InstanceGUID = Guid.NewGuid();
            this.Validator = new AssetFeaturesViewModelValidation();
            this.AssetFeaturesDetailViewModel = new AssetFeaturesDetailViewModel();
            this.AssetFeaturesAssignedToViewModel = new AssetFeaturesAssignedToViewModel();
            this.AssetFeaturesAssignFeatureViewModel = new AssetFeaturesAssignFeatureViewModel();
            this.AssetFeaturesDetailViewModel.PropertyChanged += this.AssetFeaturesDetailViewModelOnPropertyChanged;
            this.AssetFeaturesAssignedToViewModel.PropertyChanged += this.AssetFeaturesAssignedToViewModelOnPropertyChanged;
            this.AssetFeaturesAssignFeatureViewModel.PropertyChanged += this.AssetFeaturesAssignedFeatureViewModelOnPropertyChanged;
        }


        #endregion

        #region Properties

        /// <summary>
        /// Gets the current step.
        /// </summary>
        public Asset.EnumSteps CurrentStep { get; private set; }

        /// <summary>
        /// Gets or sets the current tab.
        /// </summary>
        public Asset.EnumSteps CurrentTab { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is add.
        /// </summary>
        public bool IsAdd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is bulk update.
        /// </summary>
        public bool IsBulkUpdate { get; set; }

        /// <summary>
        /// Gets or sets the asset features detail view model.
        /// </summary>
        public AssetFeaturesDetailViewModel AssetFeaturesDetailViewModel
        {
            get
            {
                return this._assetFeaturesDetailViewModel;
            }

            set
            {
                this.SetField(ref this._assetFeaturesDetailViewModel, value, () => this.AssetFeaturesDetailViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the asset features assigned to view model.
        /// </summary>
        public AssetFeaturesAssignedToViewModel AssetFeaturesAssignedToViewModel
        {
            get
            {
                return this._assetFeaturesAssignedToViewModel;
            }

            set
            {
                this.SetField(ref this._assetFeaturesAssignedToViewModel, value, () => this.AssetFeaturesAssignedToViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the asset features assign feature view model.
        /// </summary>
        public AssetFeaturesAssignFeatureViewModel AssetFeaturesAssignFeatureViewModel
        {
            get
            {
                return this._assetFeaturesAssignFeatureViewModel;
            }

            set
            {
                this.SetField(ref this._assetFeaturesAssignFeatureViewModel, value, () => this.AssetFeaturesAssignFeatureViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic feature type view model.
        /// </summary>
        public DynamicGridViewModel DynamicFeatureTypeViewModel
        {
            get
            {
                return this._dynamicFeatureTypeViewModel;
            }

            set
            {
                this.SetField(ref this._dynamicFeatureTypeViewModel, value, () => this.DynamicFeatureTypeViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the all feature types.
        /// </summary>
        public ObservableCollection<FeatureType> AllFeatureTypes
        {
            get
            {
                return this._allFeatureTypes;
            }

            set
            {
                this.SetField(ref this._allFeatureTypes, value, () => this.AllFeatureTypes);
            }
        }

        /// <summary>
        /// Gets or sets the selected feature type.
        /// </summary>
        public FeatureType SelectedFeatureType
        {
            get
            {
                return this._selectedFeatureType;
            }

            set
            {
                if (!this.IsAdd)
                {
                    this.SetSelectedFeatureType(value);
                }
                else
                {
                    this.SetField(ref this._selectedFeatureType, value, () => this.SelectedFeatureType);
                }
            }
        }

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
                    this.ActiveViewModel = this;
                    this.IsAdd = false;
                    this.GetPermission();
                    await this.PopulateAllFeatureTypesForViewAsync();
                    this.RaiseSelectedItemChanged();
                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.SelectedFeatureType:
                    this.SetBusyAction(LoadingText);
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(Asset.EnumSteps.GridContentState.ToString());
                    }

                    this.SetGridStype(false);
                    this.SetValueForAssetFeaturesDetail(this.SelectedFeatureType);
                    await this.SetValueForAssetFeaturesAssignedTo(this.SelectedFeatureType);
                    if (this.IsCheckedOut && this.AssetFeaturesDetailViewModel.Enabled)
                    {
                        this.AssetFeaturesAssignedToViewModel.DynamicAssetCategoriesViewModel.IsEnableHoverRow = true;
                        this.AssetFeaturesAssignedToViewModel.DynamicAssetTypesViewModel.IsEnableHoverRow = true;
                    }

                    this.SetActionCommandsAsync();
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetFeatures, EnumSteps.SelectedFeatureType, this.SelectedFeatureType);
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
                    var items = new ObservableCollection<AssetFeatureTypeRowItem>(this.DynamicFeatureTypeViewModel.SelectedItems.Cast<AssetFeatureTypeRowItem>());
                    if (items.Count > 0)
                    {
                        bool result;
                        List<int> list = new List<int>(items.Select(x => x.FeatureTypeId));
                        result = await this.LockBulkUpdateAssetFeaturesAsync(list);
                        if (!result)
                        {
                            // Raise event to visible FormMenu if record selected is locked.
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetFeatures, EnumSteps.ItemLocked);
                            return;
                        }

                        this.IsCheckedOut = true;
                        this.SetIsCheckedOut();
                        this.SetGridStype(true);
                        var selectedFeatureTypes =
                            new ObservableCollection<AssetFeatureTypeRowItem>(
                                this.DynamicFeatureTypeViewModel.SelectedItems.Cast<AssetFeatureTypeRowItem>());
                        await
                            this.AssetFeaturesAssignFeatureViewModel.PopulateAssignFeatureDrogDrag(selectedFeatureTypes);
                        if (this.OnStoryBoardChanged != null)
                        {
                            this.OnStoryBoardChanged(this._currentEnumStep.ToString());
                        }

                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetFeatures, EnumSteps.EditBulkUpdate);
                        this.SetActionCommandsAsync();
                    }
                    else
                    {
                        this.ShowMessageAsync("Select one or more record to Assign Features.", "Confirm - Asset Feature");
                        this.CurrentStep = Asset.EnumSteps.None;
                    }
                    
                    break;
                case Asset.EnumSteps.BulkUpdate:
                    this.DynamicFeatureTypeViewModel.IsEnableHoverRow = true;
                    this.IsBulkUpdate = true;
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.Add:
                    this.IsAdd = true;
                    this.SetNewFeatureType();
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(Asset.EnumSteps.GridContentState.ToString());
                    }

                    this.OnStepChanged(Asset.EnumSteps.DetailsState.ToString());
                    this.CurrentTab = Asset.EnumSteps.DetailsState;
                    this.SetValueForAssetFeaturesDetail(this.SelectedFeatureType);
                    await this.SetValueForAssetFeaturesAssignedTo(this.SelectedFeatureType);
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetFeatures, EnumSteps.Add);
                    await this.OnStepAsync(Asset.EnumSteps.Edit);
                    break;
                case Asset.EnumSteps.Edit:
                    if (this.SelectedFeatureType != null && !this.SelectedFeatureType.IsNewFeatureType)
                    {
                        this.IsAdd = false;
                        bool checkLock =
                            await
                            this.LockBulkUpdateAssetFeaturesAsync(
                                new List<int> { this.SelectedFeatureType.FeatureTypeId },
                                true);
                        if (!checkLock)
                        {
                            // Raise event to visible FormMenu if record selected is locked.
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetFeatures, EnumSteps.ItemLocked);

                            // Change background if record selected is locked when editing.
                            this.SetGridStype(false);
                            this.CurrentStep = Asset.EnumSteps.None;
                            return;
                        }
                    }

                    this.IsCheckedOut = true;
                    this.SetIsCheckedOut();
                    this.SetGridStype(true);
                    if (this.AssetFeaturesAssignedToViewModel.IsCheckedOut)
                    {
                        this.AssetFeaturesAssignedToViewModel.DynamicAssetCategoriesViewModel.IsEnableHoverRow = true;
                        this.AssetFeaturesAssignedToViewModel.DynamicAssetTypesViewModel.IsEnableHoverRow = true;
                    }

                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetFeatures, EnumSteps.Edit);
                    break;
                case Asset.EnumSteps.EditBulkUpdate:
                    var item2S = new ObservableCollection<AssetFeatureTypeRowItem>(this.DynamicFeatureTypeViewModel.SelectedItems.Cast<AssetFeatureTypeRowItem>());
                    if (item2S.Count != 0)
                    {
                        this.IsBulkUpdate = true;
                        bool result;
                        List<int> list = new List<int>(item2S.Select(x => x.FeatureTypeId));
                        result = await this.LockBulkUpdateAssetFeaturesAsync(list);
                        if (!result)
                        {
                            // Raise event to visible FormMenu if record selected is locked.
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetFeatures, EnumSteps.ItemLocked);
                            return;
                        }
                    }

                    this.IsCheckedOut = true;
                    this.SetIsCheckedOut();
                    this.SetGridStype(true);
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetFeatures, EnumSteps.EditBulkUpdate);
                    break;
                case Asset.EnumSteps.SaveBulkUpdate:
                    try
                    {
                        await this.SaveFeatureBulkUpdateAsync();
                        await this.UnLockBulkUpdateAssetFeaturesAsync();
                        this.IsBulkUpdate = false;
                        this.IsCheckedOut = false;
                        this.SetIsCheckedOut();
                        this.SetGridStype(false);
                        this.IsChanged = false;
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetFeatures, EnumSteps.SaveAssignFeature);
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
                        this.AssetFeaturesDetailViewModel.Validate();
                        if (this.AssetFeaturesDetailViewModel.HasErrors == false)
                        {
                            this.IsAdd = false;
                            if (this.SelectedFeatureType != null && !this.SelectedFeatureType.IsNewFeatureType)
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
                            this.SelectedFeatureType = await this.SaveFeatureAsync();
                            this.UpdateDataFeatureTypesAsync();
                            this.SelectedFeatureType.IsNewFeatureType = false;

                            this.ValidateNotError();
                            this.AllFeatureTypes = new ObservableCollection<FeatureType>(await AssetFeatureFunction.GetAllFeatureTypesAsync());
                            await this.OnStepAsync(Asset.EnumSteps.SelectedFeatureType);
                            this.RaiseActionsWhenChangeStep(EnumScreen.AssetFeatures, EnumSteps.Save, this.SelectedFeatureType.FeatureTypeId);
                        }
                        else
                        {
                            this.CurrentStep = Asset.EnumSteps.Error;
                            this.SetActionCommandsAsync();
                            this.ListErrorHyperlink = this.AssetFeaturesDetailViewModel.ListErrorHyperlink;
                            this.OnErrorHyperlinkSelected();
                        }
                    }
                    catch (Exception exc)
                    {
                        ExceptionLogger.WriteLog(exc);
                        this.ShowErrorMessage("Error encountered while Saving Asset Feature Details.", "Asset Features");
                    }

                    break;
                case Asset.EnumSteps.SaveAndAdd:
                    await this.OnStepAsync(Asset.EnumSteps.Save);
                    if (this.AssetFeaturesDetailViewModel.HasErrors == false)
                    {
                        await this.OnStepAsync(Asset.EnumSteps.Add);
                    }

                    break;
                case Asset.EnumSteps.Delete:
                    this.AssetFeaturesDetailViewModel.ClearNotifyErrors();
                    this.ValidateNotError();
                    this.Validate();
                    if (this.HasErrors == false)
                    {
                        bool canProceed = false;
                        ConfirmationWindowView confirm = new ConfirmationWindowView();
                        ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                        confirmViewModel.Content = "Select OK to confirm deletion?";
                        confirmViewModel.Title = "Delete - Asset Feature";
                        confirm.DataContext = confirmViewModel;

                        confirm.ShowDialog();
                        if (confirm.DialogResult == true)
                        {
                            canProceed = true;
                        }

                        if (canProceed)
                        {
                            if (this.SelectedFeatureType != null && !this.SelectedFeatureType.IsNewFeatureType)
                            {
                                await
                                    AssetFeatureFunction.DeleteSelectedFeatureTypeAsync(
                                        this.SelectedFeatureType.FeatureTypeId);
                                await this.UnLockBulkUpdateAssetFeaturesAsync();
                                this.AllFeatureTypes =
                                   new ObservableCollection<FeatureType>(
                                       await AssetFeatureFunction.GetAllFeatureTypesAsync());
                                this.RaiseActionsWhenChangeStep(
                                    EnumScreen.AssetFeatures,
                                    EnumSteps.Delete,
                                    this.SelectedFeatureType.FeatureTypeId);
                                await this.UnLockBulkUpdateAssetFeaturesAsync();
                                this.UpdateDataFeatureTypesAsync(true);
                                this.IsCheckedOut = false;
                                this.SetIsCheckedOut();
                                this.IsChanged = false;
                                this.OnCancelNewItem(EnumScreen.AssetFeatures);
                                if (this.OnStoryBoardChanged != null)
                                {
                                    this.OnStoryBoardChanged(Asset.EnumSteps.GridSummaryState.ToString());
                                    this.CurrentStep = Asset.EnumSteps.GridSummaryState;
                                }
                            }
                        }
                        else
                        {
                            this.CurrentStep = Asset.EnumSteps.None;
                        }
                    }
                    else
                    {
                        this.CurrentStep = Asset.EnumSteps.Error;
                        this.SetActionCommandsAsync();

                        if (!_permissionFeatureAssignTo.CanSee)
                        {
                            foreach (var itemError in this.ListErrorHyperlink)
                            {
                                itemError.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton2");
                                itemError.Action = HyperLinkAction.DetailsState;
                            }
                        }
                        
                        this.OnErrorHyperlinkSelected();
                    }

                    break;
                case Asset.EnumSteps.Cancel:
                    canProcess = await this.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this.AssetFeaturesDetailViewModel.ClearNotifyErrors();
                        this.ValidateNotError();

                        // Just do UnLockAsync if not in mode Add.
                        if (this.SelectedFeatureType != null && !this.SelectedFeatureType.IsNewFeatureType)
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
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetFeatures, EnumSteps.Cancel);
                        if (this.SelectedFeatureType == null
                            || (this.SelectedFeatureType != null && this.SelectedFeatureType.IsNewFeatureType))
                        {
                            this.OnCancelNewItem(EnumScreen.AssetFeatures);
                            if (this.OnStoryBoardChanged != null)
                            {
                                this.OnStoryBoardChanged(Asset.EnumSteps.GridSummaryState.ToString());
                                this.CurrentStep = Asset.EnumSteps.GridSummaryState;
                            }
                        }
                        else
                        {
                            this.AssetFeaturesAssignedToViewModel.DynamicAssetCategoriesViewModel.IsEnableHoverRow =
                                false;
                            this.AssetFeaturesAssignedToViewModel.DynamicAssetTypesViewModel.IsEnableHoverRow = false;
                            this.SetValueForAssetFeaturesDetail(this.SelectedFeatureType);
                            await this.SetValueForAssetFeaturesAssignedTo(this.SelectedFeatureType);
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
                        // await this.UnLockAsync();
                        this.IsChanged = false;
                        var selectedFeatureTypesCancel = new ObservableCollection<AssetFeatureTypeRowItem>(this.DynamicFeatureTypeViewModel.SelectedItems.Cast<AssetFeatureTypeRowItem>());
                        if (selectedFeatureTypesCancel.Count() != 0)
                        {
                            await this.UnLockBulkUpdateAssetFeaturesAsync();
                        }
                        else
                        {
                            this.IsCheckedOut = false;
                            this.SetIsCheckedOut();
                            this.IsChanged = false;
                        }

                        await this.AssetFeaturesAssignFeatureViewModel.PopulateAssignFeatureDrogDrag(selectedFeatureTypesCancel);
                        this.IsCheckedOut = false;
                        this.IsBulkUpdate = false;
                        this.SetIsCheckedOut();
                        this.SetGridStype(false);
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetFeatures, EnumSteps.CancelAssignFeature);
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
                        this.DynamicFeatureTypeViewModel.SelectedItem = null;
                        this.DynamicFeatureTypeViewModel.IsEnableHoverRow = false;
                        this.DynamicFeatureTypeViewModel.SelectedRows = new List<object>();

                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetFeatures, EnumSteps.CancelBulkUpdate);
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
        /// The dispose.
        /// </summary>
        public override void Dispose()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    this.CurrentStep = Asset.EnumSteps.Dispose;
                    this.IsChanged = false;
                    this.IsCheckedOut = false;
                    // ReSharper disable once CSharpWarnings::CS4014
                    this.UnLockBulkUpdateAssetFeaturesAsync();
                base.Dispose();
            }));
        }

        /// <summary>
        /// The unlock item.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task UnlockItem()
        {
            this.CurrentStep = Asset.EnumSteps.Cancel;
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
            if (this.DynamicFeatureTypeViewModel != null)
            {
                this.DynamicFeatureTypeViewModel.SelectedItemChanged = this.GridSelectedItemChanged;
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
            bool isHover = this.DynamicFeatureTypeViewModel.IsEnableHoverRow;
            if (!isHover && item != null)
            {
                if (this.DynamicFeatureTypeViewModel.SelectedItem == null)
                {
                    return;
                }

                AssetFeatureTypeRowItem selectedFeatureType =
                    this.DynamicFeatureTypeViewModel.SelectedItem as AssetFeatureTypeRowItem;
                this._selectedFeatureType =
                    this.AllFeatureTypes.FirstOrDefault(
                        feature =>
                        selectedFeatureType != null && feature.FeatureTypeId == selectedFeatureType.FeatureTypeId);
                await this.OnStepAsync(EnumSteps.SelectedFeatureType);
            }
        }

        /// <summary>
        /// The check if un saved changes.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        // ReSharper disable once CSharpWarnings::CS1998
        public async Task<bool> CheckIfUnSavedChanges()
        {
            bool canProceed = true;
            if (this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Asset Features";
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

        #region Protected

        /// <summary>
        /// The set action commands async.
        /// </summary>
        protected override void SetActionCommandsAsync()
        {
            if (!this.IsCheckedOut)
            {
                this.IsError = false;
            }

            if (this.CurrentStep == Asset.EnumSteps.Start || this.CurrentStep == Asset.EnumSteps.GridSummaryState || this.CurrentStep == Asset.EnumSteps.CancelBulkUpdate)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>();

                if (this._permissionFeatureDetail.CanEdit && this._permissionFeatureDetail.CanAdd)
                {
                    this.ActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.Add.ToString(), Command = new Add() });
                }

                if (this._permissionFeatureAssignTo.CanEdit)
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
                            new ActionCommand { Parameter = Asset.EnumSteps.AssignFeatureState.ToString(), Command = new AssignFeature() },
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
                            if (this._permissionFeatureDetail.CanEdit)
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                                };
                            }
                        }

                        if (this.IsCheckedOut)
                        {
                            if (this._permissionFeatureDetail.CanEdit)
                            {
                                if (this.IsAdd)
                                {
                                    if (this._permissionFeatureDetail.CanAdd)
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
                                else
                                {
                                    if (this._permissionFeatureDetail.CanDelete)
                                    {
                                        this.ActionCommands = new ObservableCollection<ActionCommand>
                                            {
                                                new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                                new ActionCommand { Parameter = Asset.EnumSteps.Delete.ToString(), Command = new Delete() },
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
                            }
                            else
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>();
                            }
                        }

                        if (this.SelectedFeatureType == null || (this.SelectedFeatureType != null && !this.SelectedFeatureType.IsNewFeatureType))
                        {
                            if (this.CurrentStep == Asset.EnumSteps.Save || this.CurrentStep == Asset.EnumSteps.Cancel || this.CurrentStep == Asset.EnumSteps.Delete)
                            {
                                if (this._permissionFeatureDetail.CanEdit)
                                {
                                    this.ActionCommands = new ObservableCollection<ActionCommand>
                                    {
                                        new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                                    };
                                }
                            }
                        }

                        if (this.CurrentStep == Asset.EnumSteps.Error)
                        {
                            this.IsError = true;
                            if (this.IsAdd)
                            {
                                if (this._permissionFeatureDetail.CanAdd)
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
                                if (this._permissionFeatureDetail.CanDelete)
                                {
                                    this.ActionCommands = new ObservableCollection<ActionCommand>
                                    {
                                        new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                        new ActionCommand { Parameter = Asset.EnumSteps.Delete.ToString(), Command = new Delete() },
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
                        }

                        break;
                    case Asset.EnumSteps.AssignedToState:

                        if (this.IsCheckedOut == false)
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>();
                            if (this._permissionFeatureAssignTo.CanEdit)
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                                };
                            }
                        }

                        if (this.IsCheckedOut)
                        {
                            if (this._permissionFeatureAssignTo.CanEdit)
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

                        if (this.SelectedFeatureType != null && !this.SelectedFeatureType.IsNewFeatureType)
                        {
                            if (this.CurrentStep == Asset.EnumSteps.Save || this.CurrentStep == Asset.EnumSteps.Cancel || this.CurrentStep == Asset.EnumSteps.Delete)
                            {
                                if (this._permissionFeatureAssignTo.CanEdit)
                                {
                                    this.ActionCommands = new ObservableCollection<ActionCommand>
                                    {
                                        new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                                    };
                                }
                            }
                        }

                        if (this.CurrentStep == Asset.EnumSteps.Error)
                        {
                            if (this._permissionFeatureAssignTo.CanEdit)
                            {
                                this.IsError = true;
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                                    new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() },
                                };
                            }
                        }

                        break;
                }
            }

            if (this.CurrentStep != Asset.EnumSteps.Error && this.ActionCommands != null
                && this.ActionCommands.Count(a => a.Parameter == Asset.EnumSteps.Error.ToString()) == 0 && this.IsError)
            {
                if (this.CurrentTab == Asset.EnumSteps.DetailsState && this._permissionFeatureDetail.CanEdit)
                {
                    this.ActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() });
                }

                if (this.CurrentTab == Asset.EnumSteps.AssignedToState && this._permissionFeatureAssignTo.CanEdit)
                {
                    this.ActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() });
                }
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
                    "FeatureType",
                    new ItemLock
                        {
                            ListUniqueIdentifier = new List<string> { value.FirstOrDefault().ToString(CultureInfo.InvariantCulture) },
                    UserId = userId,
                    InstanceGUID = this.InstanceGUID
                });

                // Get CatFeatureId to lock table EquipCatFeature
                List<int> listItemCatFeatureExist;
                listItemCatFeatureExist = await AssetFeatureFunction.GetIdCatFeatureItem(value.FirstOrDefault());
                if (listItemCatFeatureExist != null && listItemCatFeatureExist.Count > 0)
                {
                    listItemLocks.Add(
                        "EquipCatFeature",
                        new ItemLock
                            {
                                ListUniqueIdentifier = listItemCatFeatureExist.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)),
                        UserId = userId,
                        InstanceGUID = this.InstanceGUID
                    });
                }

                // Get CatFeatureId to lock table EquipTypeFeature
                List<int> listItemTypeFeatureExist;
                listItemTypeFeatureExist = await AssetFeatureFunction.GetIdTypeFeatureItem(value.FirstOrDefault());
                if (listItemTypeFeatureExist != null && listItemTypeFeatureExist.Count > 0)
                {
                    listItemLocks.Add(
                        "EquipTypeFeature",
                        new ItemLock
                            {
                                ListUniqueIdentifier = listItemTypeFeatureExist.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)),
                        UserId = userId,
                        InstanceGUID = this.InstanceGUID
                    });
                }
            }
            else
            {
                if (value != null)
                {
                    listItemLocks.Add("EquipType", new ItemLock { ListUniqueIdentifier = value.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)), UserId = userId, InstanceGUID = this.InstanceGUID });
                }

                // Get CatFeatureId to lock table EquipCatFeature
                List<int> listItemCatFeatureExist = new List<int>();

                // Get CatFeatureId to lock table EquipTypeFeature
                List<int> listItemTypeFeatureExist = new List<int>();

                if (value != null)
                {
                    foreach (var recordIdLock in value)
                    {
                        listItemCatFeatureExist.AddRange(await AssetFeatureFunction.GetIdCatFeatureItem(recordIdLock));
                        listItemTypeFeatureExist.AddRange(await AssetFeatureFunction.GetIdTypeFeatureItem(recordIdLock));
                    }
                }

                if (listItemCatFeatureExist.Count > 0)
                {
                    listItemLocks.Add(
                        "EquipCatFeature",
                        new ItemLock
                            {
                                ListUniqueIdentifier = listItemCatFeatureExist.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)),
                        UserId = userId,
                        InstanceGUID = this.InstanceGUID
                    });
                }

                if (listItemTypeFeatureExist.Count > 0)
                {
                    listItemLocks.Add(
                        "EquipTypeFeature",
                        new ItemLock
                            {
                                ListUniqueIdentifier = listItemTypeFeatureExist.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)),
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
        /// The populate all feature types for view async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task PopulateAllFeatureTypesForViewAsync()
        {
            this.DynamicFeatureTypeViewModel = new DynamicGridViewModel(typeof(AssetFeatureTypeRowItem));
            this.DynamicFeatureTypeViewModel.MaxWidthGrid = 650;
            this.DynamicFeatureTypeViewModel.IsEnableHoverRow = false;
            this.DynamicFeatureTypeViewModel.IsSelectAllRow = false;
            this.DynamicFeatureTypeViewModel.IsShowGroupPanel = true;
            this.AllFeatureTypes = new ObservableCollection<FeatureType>(await AssetFeatureFunction.GetAllFeatureTypesAsync());

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

            this.DynamicFeatureTypeViewModel.GridColumns = new List<DynamicColumn>
                                                                                             {
                                                                                                 new DynamicColumn { ColumnName = "FeatureTypeID", Header = "ID", MinWidth = 30, TextAlignment = TextAlignment.Center, HeaderTextAlignment = TextAlignment.Center, Width = 50 },
                                                                                                 new DynamicColumn { ColumnName = "FeatureName", Header = "NAME", MinWidth = 50, IsSelectedColumn = true, Width = 300, HeaderTextAlignment = TextAlignment.Left },
                                                                                                 new DynamicColumn { ColumnName = "RequiredLength",  Header = "REQUIRED LENGTH", MinWidth = 85, TextAlignment = TextAlignment.Center, HeaderTextAlignment = TextAlignment.Center, Width = 200 },
                                                                                                 new DynamicColumn { ColumnName = "Enabled",  Header = "ENABLED", MinWidth = 80, ColumnTemplate = RadGridViewEnum.ColumnCheckedTemplate, FilteringTemplate = RadGridViewEnum.FilteringDataList, FilteringDataSource = sourceEnable, HeaderTextAlignment = TextAlignment.Center, Width = 98 }
                                                                                             };
            this.DynamicFeatureTypeViewModel.FilteringGenerate = true;
            this.AllFeatureTypes = new ObservableCollection<FeatureType>(await AssetFeatureFunction.GetAllFeatureTypesAsync());
            var allFeatureTypes = from f in this.AllFeatureTypes
                                  select new AssetFeatureTypeRowItem
                                  {
                                      FeatureTypeId = f.FeatureTypeId,
                                      FeatureName = f.FeatureName,
                                      RequiredLength = f.RequiredLength == -1 ? "N/A" : f.RequiredLength.ToString(CultureInfo.InvariantCulture),
                                      Enabled = f.Enabled,
                                      IsMouseHover = f.Enabled
                                  };
            this.DynamicFeatureTypeViewModel.GridDataRows = allFeatureTypes.ToList<object>();
            this.DynamicFeatureTypeViewModel.LoadRadGridView();
            this.DynamicFeatureTypeViewModel.GroupedItemChanged = this.GroupedChanged;
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
        private void GroupedChanged(object sender, object e)
        {
            if ((int)e == -1)
            {
                this.DynamicFeatureTypeViewModel.MaxWidthGrid = 10000;
            }
            else
            {
                this.DynamicFeatureTypeViewModel.MaxWidthGrid = (int)e + 1;
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

            if (this.AssetFeaturesDetailViewModel != null)
            {
                if (this._permissionFeatureDetail.CanEdit)
                {
                    this.AssetFeaturesDetailViewModel.GridStyle = this.GridStyle;
                }
                else
                {
                    this.AssetFeaturesDetailViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                }
            }

            if (this.AssetFeaturesAssignedToViewModel != null)
            {
                if (this._permissionFeatureAssignTo.CanEdit)
                {
                    this.AssetFeaturesAssignedToViewModel.GridStyle = this.GridStyle;
                }
                else
                {
                    this.AssetFeaturesAssignedToViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                }
            }

            if (this.AssetFeaturesAssignFeatureViewModel != null)
            {
                this.AssetFeaturesAssignFeatureViewModel.GridStyle = this.GridStyle;
            }
        }

        /// <summary>
        /// The set is checked out.
        /// </summary>
        private void SetIsCheckedOut()
        {
            if (this.AssetFeaturesDetailViewModel != null)
            {
                if (this._permissionFeatureDetail.CanEdit)
                {
                    this.AssetFeaturesDetailViewModel.IsCheckedOut = this.IsCheckedOut;
                }
            }

            if (this.AssetFeaturesAssignedToViewModel != null)
            {
                if (this._permissionFeatureAssignTo.CanEdit)
                {
                    this.AssetFeaturesAssignedToViewModel.IsCheckedOut = this.IsCheckedOut;
                }
            }

            if (this.AssetFeaturesAssignFeatureViewModel != null)
            {
                this.AssetFeaturesAssignFeatureViewModel.IsCheckedOut = this.IsCheckedOut;
            }
        }

        /// <summary>
        /// The get permission.
        /// </summary>
        private void GetPermission()
        {
            this._permissionFeatureDetail = Authorisation.GetPermission(Components.SystemManagementAssetFeatures, Forms.FeaturesDetail);
            this._permissionFeatureAssignTo = Authorisation.GetPermission(Components.SystemManagementAssetFeatures, Forms.FeaturesAssignTo);
        }

        /// <summary>
        /// The set selected feature type.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private async void SetSelectedFeatureType(FeatureType value)
        {
            bool canProceed = true;

            if (this.IsCheckedOut && this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Asset Features";
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
                if (this.AssetFeaturesDetailViewModel != null)
                {
                    this.AssetFeaturesDetailViewModel.ClearNotifyErrors();
                }

                this.IsChanged = false;
                this.IsCheckedOut = false;
                this.SetIsCheckedOut();

                // Just do UnLockAsync if not in mode Add.
                if (value != null && !value.IsNewFeatureType)
                {
                    if (this.SelectedFeatureType != null)
                    {
                        await this.UnLockBulkUpdateAssetFeaturesAsync();
                        this.IsAdd = false;
                    }
                }

                this.SetField(ref this._selectedFeatureType, value, () => this.SelectedFeatureType);
                if (value != null)
                {
                    await this.OnStepAsync(Asset.EnumSteps.SelectedFeatureType);
                }
            }
            else
            {
                this.CurrentStep = Asset.EnumSteps.None;
            }
        }

        /// <summary>
        /// The set value for asset features assigned to.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SetValueForAssetFeaturesAssignedTo(FeatureType value)
        {
            this.AssetFeaturesAssignedToViewModel.FeatureName = value.FeatureName;
            if (!value.IsNewFeatureType)
            {
                await this.AssetFeaturesAssignedToViewModel.PopulateEquipCatFeature(value);
                await this.AssetFeaturesAssignedToViewModel.PopulateEquipTypeFeature(value);
            }
            else
            {
                await
                    Task.WhenAll(
                        this.AssetFeaturesAssignedToViewModel.PopulateEquipCatFeature(value),
                        this.AssetFeaturesAssignedToViewModel.PopulateEquipTypeFeature(value));
            }
        }

        /// <summary>
        /// The set value for asset features detail.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private void SetValueForAssetFeaturesDetail(FeatureType value)
        {
            this.AssetFeaturesDetailViewModel.FeatureTypeId = value.FeatureTypeId;
            this.AssetFeaturesDetailViewModel.FeatureName = value.FeatureName;
            this.AssetFeaturesDetailViewModel.Enabled = value.Enabled;
            this.AssetFeaturesDetailViewModel.RequiredLengthString = value.RequiredLength == -1 ? string.Empty : value.RequiredLength.ToString(CultureInfo.InvariantCulture);
            if (this.AssetFeaturesDetailViewModel.RequiredLengthString != string.Empty)
            {
                if (int.Parse(this.AssetFeaturesDetailViewModel.RequiredLengthString) > -1)
                {
                    this.AssetFeaturesDetailViewModel.IsChecked = true;
                }
            }
            else
            {
                this.AssetFeaturesDetailViewModel.IsChecked = false;
            }

            this.AssetFeaturesDetailViewModel.IsNewFeatureType = value.IsNewFeatureType;
        }

        /// <summary>
        /// The set new feature type.
        /// </summary>
        private void SetNewFeatureType()
        {
            this.SelectedFeatureType = new FeatureType
                                           {
                                               IsNewFeatureType = true,
                                               Enabled = true,
                                               RequiredLength = -1
                                           };
            this.AssetFeaturesDetailViewModel.IsChecked = false;
        }

        /// <summary>
        /// The save feature async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<FeatureType> SaveFeatureAsync()
        {
            ObservableCollection<AssetTypesFeatureRowItem> typeFeatureTypes;
            ObservableCollection<AssetCategoriesFeatureRowItem> categoryFeatureTypes;
            var selectedTypes = this.AssetFeaturesAssignedToViewModel.DynamicAssetTypesViewModel.SelectedItems;
            var selectedCategories = this.AssetFeaturesAssignedToViewModel.DynamicAssetCategoriesViewModel.SelectedItems;
            typeFeatureTypes = selectedTypes == null ? new ObservableCollection<AssetTypesFeatureRowItem>() : new ObservableCollection<AssetTypesFeatureRowItem>(selectedTypes.Cast<AssetTypesFeatureRowItem>());
            if (selectedCategories == null)
            {
                categoryFeatureTypes = new ObservableCollection<AssetCategoriesFeatureRowItem>();
            }
            else
            {
                categoryFeatureTypes = new ObservableCollection<AssetCategoriesFeatureRowItem>(this.AssetFeaturesAssignedToViewModel.DynamicAssetCategoriesViewModel.SelectedItems.Cast<AssetCategoriesFeatureRowItem>());
            }

            return await AssetFeatureFunction.SaveSelectedFeatureTypeAsync(this.AssetFeaturesDetailViewModel.GetDataAssetFeaturesDetail(), categoryFeatureTypes, typeFeatureTypes, this.AssetFeaturesDetailViewModel.Enabled);
        }

        /// <summary>
        /// The save feature bulk update async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SaveFeatureBulkUpdateAsync()
        {
            var groupAssetFeatures = this.AssetFeaturesAssignFeatureViewModel.AssetFeatureDragDropViewModel.GroupAssetDragDropSource;
            if (groupAssetFeatures != null)
            {
                foreach (var feature in groupAssetFeatures)
                {
                    ObservableCollection<AssetTypesFeatureRowItem> typeFeatureTypes = new ObservableCollection<AssetTypesFeatureRowItem>();
                    ObservableCollection<AssetCategoriesFeatureRowItem> categoryFeatureTypes = new ObservableCollection<AssetCategoriesFeatureRowItem>();

                    ObservableCollection<ItemDragDrop> selectedTypes =
                        new ObservableCollection<ItemDragDrop>(feature.AssetTypeViewModel.Items);
                    foreach (var selectedType in selectedTypes)
                    {
                        typeFeatureTypes.Add(new AssetTypesFeatureRowItem
                                                 {
                                                     EquipTypeId = selectedType.ID,
                                                     EquipTypeName = selectedType.Name
                                                 });
                    }

                    ObservableCollection<ItemDragDrop> selectedCategories =
                        new ObservableCollection<ItemDragDrop>(feature.AssetCategoryViewModel.Items);

                    foreach (var selectedCat in selectedCategories)
                    {
                        categoryFeatureTypes.Add(new AssetCategoriesFeatureRowItem
                                                 {
                                                     EquipCatId = selectedCat.ID,
                                                     EquipCatName = selectedCat.Name
                                                 });
                    }

                    await
                        AssetFeatureFunction.SaveCategoriesTypesFeatureBulkUpdateTypeAsync(
                            feature.Id,
                            categoryFeatureTypes,
                            typeFeatureTypes);
                }
            }
        }

        /// <summary>
        /// The update data feature types async.
        /// </summary>
        /// <param name="isDelete">
        /// The is Delete.
        /// </param>
        private void UpdateDataFeatureTypesAsync(bool isDelete = false)
        {
            DataRow editItem = null;
            if (this.SelectedFeatureType.IsNewFeatureType)
            {
                this.SelectedFeatureType.IsNewFeatureType = false;
                AssetFeatureTypeRowItem featureType = new AssetFeatureTypeRowItem
                                                          {
                                                              FeatureTypeId = this.SelectedFeatureType.FeatureTypeId,
                                                              FeatureName = this.SelectedFeatureType.FeatureName,
                                                              RequiredLength = this.SelectedFeatureType.RequiredLength == -1 ? "N/A" : this.SelectedFeatureType.RequiredLength.ToString(CultureInfo.InvariantCulture),
                                                              Enabled = this.SelectedFeatureType.Enabled,
                                                              IsMouseHover = this.SelectedFeatureType.Enabled,
                                                          };
                this.DynamicFeatureTypeViewModel.InsertRow(0, featureType);

                // add record for filter
            }
            else
            {
                foreach (var m in this.DynamicFeatureTypeViewModel.MembersTable.Rows)
                {
                    if (this.SelectedFeatureType != null && this.SelectedFeatureType.FeatureTypeId.ToString(CultureInfo.InvariantCulture) == m["FeatureTypeId"].ToString())
                    {
                        editItem = m;
                        break;
                    }
                }

                if (editItem != null)
                {
                    int index = this.DynamicFeatureTypeViewModel.MembersTable.Rows.IndexOf(editItem);
                    AssetFeatureTypeRowItem featureType = new AssetFeatureTypeRowItem
                                                          {
                                                              FeatureTypeId = this.SelectedFeatureType.FeatureTypeId,
                                                              FeatureName = this.SelectedFeatureType.FeatureName,
                                                              RequiredLength = this.SelectedFeatureType.RequiredLength == -1 ? "N/A" : this.SelectedFeatureType.RequiredLength.ToString(CultureInfo.InvariantCulture),
                                                              Enabled = this.SelectedFeatureType.Enabled,
                                                              IsMouseHover = this.SelectedFeatureType.Enabled,
                                                          };
                    if (isDelete)
                    {
                        this.DynamicFeatureTypeViewModel.MembersTable.Rows.RemoveAt(index);
                    }
                    else
                    {
                        this.DynamicFeatureTypeViewModel.UpdateRow(index, featureType);
                    }
                }

                // update filter
            }
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
        /// The asset features detail view model on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetFeaturesDetailViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut
            && ((e.PropertyName.IndexOf("FeatureName", StringComparison.Ordinal) != -1 || e.PropertyName.IndexOf("Enabled", StringComparison.Ordinal) != -1)
                || e.PropertyName.IndexOf("RequiredLength", StringComparison.Ordinal) != -1 || e.PropertyName.Equals("IsChecked")))
            {
                this.IsChanged = true;
                if (e.PropertyName.IndexOf("Enabled", StringComparison.Ordinal) != -1)
                {
                    FeatureType typeToReset = new FeatureType
                                                  {
                                                      FeatureTypeId = this.SelectedFeatureType.FeatureTypeId,
                                                      Enabled = this.SelectedFeatureType.Enabled,
                                                  };
                    this.AssetFeaturesAssignedToViewModel.ResetGridWhenChangeEnable(typeToReset, this.AssetFeaturesDetailViewModel.Enabled);
                }
            }
        }

        /// <summary>
        /// The asset features assigned to view model on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetFeaturesAssignedToViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut && (e.PropertyName.IndexOf("IsCheckItemChanged", StringComparison.Ordinal) != -1))
            {
                this.IsChanged = true;
            }
        }

        /// <summary>
        /// The asset features assigned feature view model on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetFeaturesAssignedFeatureViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut && (e.PropertyName.IndexOf("Items", StringComparison.Ordinal) != -1))
            {
                this.IsChanged = true;
            }
        }

        #endregion
    }
}