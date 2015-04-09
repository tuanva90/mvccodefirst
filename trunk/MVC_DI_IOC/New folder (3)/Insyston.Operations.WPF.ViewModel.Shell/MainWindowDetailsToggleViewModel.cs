// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowDetailsToggleViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Insyston.Operations.Bussiness.Assets;

namespace Insyston.Operations.WPF.ViewModels.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Insyston.Operations.Business.Collections.Model;
    using Insyston.Operations.Business.Funding.Model;
    using Insyston.Operations.Business.Security.Model;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.Bussiness.RegisteredAsset.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Assets;
    using Insyston.Operations.WPF.ViewModels.Collections;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Enums;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Funding;
    using Insyston.Operations.WPF.ViewModels.RegisteredAsset;
    using Insyston.Operations.WPF.ViewModels.Security;
    using global::WPF.DataTable.Models;
    using WPFDynamic.ViewModels.Controls;
    using Insyston.Operations.WPF.ViewModels.Assets.AssetClasses;
    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;

    /// <summary>
    /// The mw details toggle view model: manage toggle View Model, MainWindowDetailTabControlViewModel and FormBarMenuViewModel
    /// </summary>
    public class MainWindowDetailsToggleViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowDetailsToggleViewModel"/> class.
        /// </summary>
        /// <param name="formBar">
        /// The form Bar.
        /// </param>
        public MainWindowDetailsToggleViewModel(string formBar)
        {
            _toggleViewModel = new ToggleViewModel();
            this._formBarMenuViewModel = new FormBarMenuViewModel();

            // change FormBar content when change screen
            switch (formBar)
            {
                case "Security":
                    FormBarMenuViewModel.FormBarContent = "Users";      
                    break;
                case "Collection":
                    FormBarMenuViewModel.FormBarContent = "Assignments";      
                    break;
                case "Configuration":
                    FormBarMenuViewModel.FormBarContent = "Configuration Menu";
                    break;
                case "CollectionSettings":
                    FormBarMenuViewModel.FormBarContent = "Collection Settings";
                    FormBarMenuViewModel.FormMenuContent = "Menu";
                    FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    break;
                case "SecuritySetting":
                    FormBarMenuViewModel.FormBarContent = "Security Settings";
                    FormBarMenuViewModel.FormMenuContent = "Menu";
                    FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    break;
                case "ColletionQueues":
                    FormBarMenuViewModel.FormBarContent = "Collection Queues";
                    FormBarMenuViewModel.FormMenuContent = "Menu";
                    FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    break;
                case "Funding":
                    FormBarMenuViewModel.FormBarContent = "Tranches";
                    break;
                case "Home":
                    FormBarMenuViewModel.FormBarContent = "Home";
                    break;
                case "AssetClasses":
                    FormBarMenuViewModel.FormBarContent = "Asset Classes";
                    FormBarMenuViewModel.FormMenuContent = "Menu";
                    break;
                case "AssetCollateralClasses":
                    FormBarMenuViewModel.FormBarContent = "Asset Collateral Classes";
                    FormBarMenuViewModel.FormMenuContent = "Menu";
                    FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    break;
                case "AssetFeatures":
                    FormBarMenuViewModel.FormBarContent = "Asset Features";
                    FormBarMenuViewModel.FormMenuContent = "Menu";
                    FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    break;
                case "AssetSetting":
                    FormBarMenuViewModel.FormBarContent = "Asset Settings";
                    FormBarMenuViewModel.FormMenuContent = "Menu";
                    FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    break;
                case "AssetRegister":
                    FormBarMenuViewModel.FormBarContent = "Asset Registers";
                    FormBarMenuViewModel.FormMenuContent = "Menu";
                    FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    break;
                case "RegisteredAsset":
                    FormBarMenuViewModel.FormBarContent = "Assets";
                    break;
            }
            this._formBarCurrent = string.Empty;
            this._formGroupBarCurrent = string.Empty;

            _changedVisibility = new Visibility();
            ChangedVisibility = Visibility.Collapsed;
            this._allUsers = null;
        }

        #region Properties
        /// <summary>
        /// The all users.
        /// </summary>
        private ObservableCollection<UserDetails> _allUsers;

        /// <summary>
        /// The _all funding summary.
        /// </summary>
        private List<FundingSummary> _allFundingSummary;

        /// <summary>
        /// The _groups.
        /// </summary>
        private ObservableCollection<GroupDetails> _groups;

        /// <summary>
        /// The _all queue management details.
        /// </summary>
        private ObservableCollection<QueueDetailsModel> _allQueueManagementDetails;

        /// <summary>
        /// Gets or sets the screen.
        /// </summary>
        public EnumScreen Screen { get; set; }

        /// <summary>
        /// The form bar current.
        /// </summary>
        private string _formBarCurrent;

        /// <summary>
        /// The _form group bar current.
        /// </summary>
        private string _formGroupBarCurrent;

        /// <summary>
        /// The _screen detail view model.
        /// </summary>
        private ViewModelUseCaseBase _screenDetailViewModel;

        /// <summary>
        /// Gets or sets the screen detail view model.
        /// </summary>
        public ViewModelUseCaseBase ScreenDetailViewModel
        {
            get
            {
                return this._screenDetailViewModel;
            }
            set
            {
                this.SetField(ref this._screenDetailViewModel, value, () => this.ScreenDetailViewModel);
            }
        }

        // private object SelectedItem { get; set; }

        /// <summary>
        /// The _main content.
        /// </summary>
        private ObservableCollection<object> _mainContent;

        /// <summary>
        /// Gets or sets the main content.
        /// </summary>
        public ObservableCollection<object> MainContent
        {
            get
            {
                return this._mainContent;
            }
            set
            {
                this.SetField(ref this._mainContent, value, () => this.MainContent);
            }
        }

        /// <summary>
        /// The _ form bar menu view model.
        /// </summary>
        private FormBarMenuViewModel _formBarMenuViewModel;

        /// <summary>
        /// Gets or sets the form bar menu view model.
        /// </summary>
        public FormBarMenuViewModel FormBarMenuViewModel
        {
            get
            {
                return this._formBarMenuViewModel;
            }
            set
            {
                this.SetField(ref this._formBarMenuViewModel, value, () => FormBarMenuViewModel);
            }
        }

        /// <summary>
        /// The _toggle view model.
        /// </summary>
        private ToggleViewModel _toggleViewModel;

        /// <summary>
        /// Gets or sets the toggle view model.
        /// </summary>
        public ToggleViewModel ToggleViewModel
        {
            get
            {
                return this._toggleViewModel;
            }
            set
            {
                this.SetField(ref _toggleViewModel, value, () => ToggleViewModel);
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

        #endregion

        //#region Private Method

        /// <summary>
        /// The tab control view model_ on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TabControlViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.IndexOf("SelectedTab", StringComparison.Ordinal) != -1)
            {
                var mainWindowDetailsTabControlViewModel = this.ScreenDetailViewModel as MainWindowDetailsTabControlViewModel;
                if (mainWindowDetailsTabControlViewModel != null)
                {
                    int tabItem = mainWindowDetailsTabControlViewModel.SelectedTab;
                    switch (tabItem)
                    {
                        case 0:
                            this.FormBarMenuViewModel.FormBarContent = "Users";
                            break;
                        case 1:
                            this.FormBarMenuViewModel.FormBarContent = "Groups";
                            break;
                        case 2:
                            this.FormBarMenuViewModel.FormBarContent = "Membership";
                            break;
                    }
                }

            }
            if (e.PropertyName.IndexOf("SelectedTab_ListCollectors", StringComparison.Ordinal) != -1)
            {
                var mainWindowDetailsTabControlViewModel = this.ScreenDetailViewModel as MainWindowDetailsTabControlCollectionQueueViewModel;
                if (mainWindowDetailsTabControlViewModel != null)
                {
                    int tabItem = mainWindowDetailsTabControlViewModel.SelectedTab_ListCollectors;
                    switch (tabItem)
                    {
                        case 0:
                            this.FormBarMenuViewModel.FormBarContent = "Collection Queues";
                            break;
                        case 1:
                            this.FormBarMenuViewModel.FormBarContent = "Queue Collectors";
                            break;
                    }
                }

            }

            if (e.PropertyName.IndexOf("SelectedTab_AssetClass", StringComparison.Ordinal) != -1)
            {
                var mainWindowDetailsAssetClassesViewModel = this.ScreenDetailViewModel as MainAssetClassesViewModel;
                if (mainWindowDetailsAssetClassesViewModel != null)
                {
                    int tabItem = mainWindowDetailsAssetClassesViewModel.SelectedTab_AssetClass;
                    switch (tabItem)
                    {
                        case 0:
                            this.FormBarMenuViewModel.FormBarContent = "Asset Classes Category";
                            break;
                        case 1:
                            this.FormBarMenuViewModel.FormBarContent = "Asset Classes Types";
                            break;
                        case 2:
                            this.FormBarMenuViewModel.FormBarContent = "Asset Classes Make";
                            break;
                        case 3:
                            this.FormBarMenuViewModel.FormBarContent = "Asset Classes Model";
                            break;
                    }
                }

            }
        }

        /// <summary>
        /// The validate popup.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ValidatePopup()
        {
            ConfirmationWindowView confirm = new ConfirmationWindowView();
            ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
            confirmViewModel.Content = "Form has not been saved. Click OK to proceed without saving changes!";
            confirmViewModel.Title = "Confirm Close - Document Tab";
            confirm.DataContext = confirmViewModel;

            confirm.ShowDialog();
            if (confirm.DialogResult == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The back to grid when click on form menu.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        private void BacktoGrid(object o)
        {
            FormBarMenuViewModel screen = o as FormBarMenuViewModel;
            if (screen != null)
            {
                switch (screen.FormMenuContent)
                {
                    #region Users
                    case "Users":
                        var mainWindowDetailsTabControlViewModel = this.ScreenDetailViewModel as MainWindowDetailsTabControlViewModel;
                        if (mainWindowDetailsTabControlViewModel != null)
                        {
                            bool canProceed = true;

                            var usersViewModel = mainWindowDetailsTabControlViewModel.UsersMainWindowDetailsVm.ScreenDetailViewModel as UsersViewModel;
                            if (usersViewModel != null)
                            {
                                // validate for mode edit
                                if (usersViewModel.IsCheckedOut && usersViewModel.IsChanged)
                                {
                                    canProceed = this.ValidatePopup();
                                }

                                if (canProceed)
                                {
                                    FormBarMenuViewModel.FormBarContent = "Users";
                                    mainWindowDetailsTabControlViewModel.ChangedVisibility = Visibility.Visible;
                                    mainWindowDetailsTabControlViewModel.UsersMainWindowDetailsVm
                                        .ChangedVisibilityHyperlink = Visibility.Collapsed;
                                    usersViewModel.GetResizeGrid();
                                    this.ChangedVisibility = Visibility.Collapsed;
                                    FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                                }
                            }
                        }
                        break;
                    #endregion

                    #region Groups
                    case "Groups":
                        var mainWindowDetailsTabControlViewModel1 = this.ScreenDetailViewModel as MainWindowDetailsTabControlViewModel;
                        if (mainWindowDetailsTabControlViewModel1 != null)
                        {
                            bool canProceed = true;

                            var groupsViewModel = mainWindowDetailsTabControlViewModel1.GroupsMainWindowDetailsVm.ScreenDetailViewModel as
                                GroupsViewModel;
                            if (groupsViewModel != null)
                            {
                                // validate for mode edit
                                if (groupsViewModel.IsCheckedOut && groupsViewModel.IsChanged)
                                {
                                    canProceed = this.ValidatePopup();
                                }

                                if (canProceed)
                                {
                                    FormBarMenuViewModel.FormBarContent = "Groups";
                                    mainWindowDetailsTabControlViewModel1.ChangedVisibility = Visibility.Visible;
                                    mainWindowDetailsTabControlViewModel1.GroupsMainWindowDetailsVm
                                        .ChangedVisibilityHyperlink = Visibility.Collapsed;
                                    groupsViewModel.GetResizeGrid();
                                    this.ChangedVisibility = Visibility.Collapsed;
                                    FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                                }
                            }
                        }
                        break;
                    #endregion

                    #region Assignments
                    case "Assignments":
                        var mainWindowDetailsViewModel = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                        if (mainWindowDetailsViewModel != null)
                        {
                            bool canProceed = true;

                            var assignmentsViewModel =
                                mainWindowDetailsViewModel.ScreenDetailViewModel as CollectionsAssignmentViewModel;
                            if (assignmentsViewModel != null)
                            {
                                // validate for mode edit
                                assignmentsViewModel.IsChangeContent = true;
                                if (assignmentsViewModel.IsCheckedOut)
                                {
                                    canProceed = this.ValidatePopup();
                                }

                                if (canProceed)
                                {
                                    FormBarMenuViewModel.FormBarContent = "Assignments";
                                    mainWindowDetailsViewModel.ChangedVisibilityHyperlink = Visibility.Hidden;

                                    var collectionsAssignmentViewModel = mainWindowDetailsViewModel.ScreenDetailViewModel as CollectionsAssignmentViewModel;
                                    collectionsAssignmentViewModel.GetResizeGrid();
                                    this.ChangedVisibility = Visibility.Collapsed;
                                    FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                                }
                            }
                        }
                        break;
                    #endregion

                    #region Menu
                    case "Menu":
                        FormBarMenuViewModel.FormBarContent = "Configuration";
                        this.DoBackToConfiguration("ConfigurationMenu");
                        break;
                    #endregion

                    #region Tranches
                    case "Tranches":
                        FormBarMenuViewModel.FormBarContent = "Tranches";
                        var mainWindowDetailsViewModelFundingSummary = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                        if (mainWindowDetailsViewModelFundingSummary != null)
                        {
                            mainWindowDetailsViewModelFundingSummary.ChangedVisibilityHyperlink = Visibility.Hidden;
                            var fundingSummaryViewModel = mainWindowDetailsViewModelFundingSummary.ScreenDetailViewModel as FundingSummaryViewModel;
                            if (fundingSummaryViewModel != null)
                                fundingSummaryViewModel.GetResizeGrid();
                        }
                        this.ChangedVisibility = Visibility.Collapsed;
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                        break;
                    #endregion

                    #region Queues
                    case "Queues":
                        var mainWindowDetailsTabControlCollectionQueueViewModel = this.ScreenDetailViewModel as MainWindowDetailsTabControlCollectionQueueViewModel;
                        if (mainWindowDetailsTabControlCollectionQueueViewModel != null)
                        {
                            bool canProceed = true;

                            var listViewModel = mainWindowDetailsTabControlCollectionQueueViewModel.ListMainWindowDetailsVm.ScreenDetailViewModel as CollectionsManagementViewModel;
                            if (listViewModel != null)
                            {
                                // validate for mode edit
                                if (listViewModel.IsCheckedOut && listViewModel.IsChanged)
                                {
                                    canProceed = this.ValidatePopup();
                                }

                                if (canProceed)
                                {
                                    FormBarMenuViewModel.FormBarContent = "Collection Queues";
                                    mainWindowDetailsTabControlCollectionQueueViewModel.ChangedVisibility = Visibility.Visible;
                                    mainWindowDetailsTabControlCollectionQueueViewModel.CheckCollectorsPermission();
                                    mainWindowDetailsTabControlCollectionQueueViewModel.ListMainWindowDetailsVm
                                        .ChangedVisibilityHyperlink = Visibility.Collapsed;
                                    listViewModel.GetResizeGrid();
                                    this.ChangedVisibility = Visibility.Collapsed;
                                    FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                                    FormBarMenuViewModel.FormMenuContent = "Menu";
                                }
                            }
                        }
                        break;
                    #endregion

                    #region Asset Features
                    case "Asset Features":
                        var model = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                        if (model != null)
                        {
                            bool canProceed = true;

                            var assetFeaturesViewModel =
                                model.ScreenDetailViewModel as AssetFeaturesViewModel;
                            if (assetFeaturesViewModel != null)
                            {
                                // validate for mode edit
                                if (assetFeaturesViewModel.IsCheckedOut && assetFeaturesViewModel.IsChanged)
                                {
                                    canProceed = this.ValidatePopup();
                                }

                                if (canProceed)
                                {
                                    FormBarMenuViewModel.FormMenuContent = "Menu";

                                    // hide tab hyperlink
                                    model.ChangedVisibilityHyperlink = Visibility.Hidden;
                                    assetFeaturesViewModel.DynamicFeatureTypeViewModel.IsEnableHoverRow = false;
                                    assetFeaturesViewModel.OnStepAsync(Asset.EnumSteps.GridSummaryState);
                                    assetFeaturesViewModel.SelectedFeatureType = null;
                                    assetFeaturesViewModel.DynamicFeatureTypeViewModel.SelectedItem = null;
                                    assetFeaturesViewModel.DynamicFeatureTypeViewModel.SelectedRows = new List<object>();                                  
                                    // collapse toggle
                                    this.ChangedVisibility = Visibility.Collapsed;
                                }
                            }
                        }
                        break;
                    #endregion

                    #region Asset Classes Category
                    case "Asset Category":
                        var modelAssetClass = this.ScreenDetailViewModel as MainAssetClassesViewModel;
                        if (modelAssetClass != null)
                        {
                            bool canProceed = true;

                            var assetClassesCategoryViewModel =
                                modelAssetClass.AssetCategoryViewModel.ScreenDetailViewModel as AssetClassesCategoryViewModel;
                            if (assetClassesCategoryViewModel != null)
                            {
                                // validate for mode edit
                                if (assetClassesCategoryViewModel.IsCheckedOut && assetClassesCategoryViewModel.IsChanged)
                                {
                                    canProceed = this.ValidatePopup();
                                }

                                if (canProceed)
                                {
                                    FormBarMenuViewModel.FormMenuContent = "Menu";
                                    FormBarMenuViewModel.FormBarContent = "Asset Classes";
                                    // hide tab hyperlink
                                    modelAssetClass.ChangedVisibility = Visibility.Visible;
                                    modelAssetClass.AssetCategoryViewModel.ChangedVisibilityHyperlink = Visibility.Collapsed;
                                    assetClassesCategoryViewModel.OnLoaded = false;
                                    assetClassesCategoryViewModel.InModeAdd = false;
                                    if (assetClassesCategoryViewModel.InModeBulkUpdate)
                                    {
                                        assetClassesCategoryViewModel.DynamicMainGridViewModel.IsEnableHoverRow = false;
                                        assetClassesCategoryViewModel.InModeBulkUpdate = true;
                                        assetClassesCategoryViewModel.DynamicMainGridViewModel.SelectedRows = new List<object>();
                                        assetClassesCategoryViewModel.InModeBulkUpdate = false;
                                    }
                                    assetClassesCategoryViewModel.OnStepAsync(Asset.EnumSteps.MainViewState);
                                    assetClassesCategoryViewModel.DynamicMainGridViewModel.SelectedItem = null;
                                    // collapse toggle
                                    this.ChangedVisibility = Visibility.Collapsed;
                                }
                            }
                        }
                        break;
                    #endregion

                    #region Asset Classes Type
                    case "Asset Type":
                        var modelAssetClassType = this.ScreenDetailViewModel as MainAssetClassesViewModel;
                        if (modelAssetClassType != null)
                        {
                            bool canProceed = true;

                            var assetClassesTypeViewModel =
                                modelAssetClassType.AssetClassesTypeViewModel.ScreenDetailViewModel as AssetClassesTypeViewModel;
                            if (assetClassesTypeViewModel != null)
                            {
                                // validate for mode edit
                                if (assetClassesTypeViewModel.IsCheckedOut && assetClassesTypeViewModel.IsChanged)
                                {
                                    canProceed = this.ValidatePopup();
                                }

                                if (canProceed)
                                {
                                    FormBarMenuViewModel.FormMenuContent = "Menu";
                                    FormBarMenuViewModel.FormBarContent = "Asset Classes";

                                    // hide tab hyperlink
                                    modelAssetClassType.ChangedVisibility = Visibility.Visible;
                                    modelAssetClassType.AssetClassesTypeViewModel.ChangedVisibilityHyperlink = Visibility.Collapsed;
                                    assetClassesTypeViewModel.OnLoaded = false;
                                    assetClassesTypeViewModel.InModeAdd = false;
                                    if (assetClassesTypeViewModel.InModeBulkUpdate)
                                    {
                                        assetClassesTypeViewModel.InModeBulkUpdate = true;
                                        assetClassesTypeViewModel.DynamicMainGridViewModel.SelectedRows = new List<object>();
                                        assetClassesTypeViewModel.DynamicMainGridViewModel.IsEnableHoverRow = false;
                                        assetClassesTypeViewModel.InModeBulkUpdate = false;
                                    }
                                    assetClassesTypeViewModel.OnStepAsync(Asset.EnumSteps.MainViewState);
                                    assetClassesTypeViewModel.DynamicMainGridViewModel.SelectedItem = null;
                                    // collapse toggle
                                    this.ChangedVisibility = Visibility.Collapsed;
                                }
                            }
                        }
                        break;
                    #endregion

                    #region Asset Classes Model
                    case "Asset Model":
                        var mainModel = this.ScreenDetailViewModel as MainAssetClassesViewModel;
                        if (mainModel != null)
                        {
                            bool canProceed = true;
                            var assetModelViewModel = mainModel.AssetClassesModelViewModel.ScreenDetailViewModel as AssetClassesModelViewModel;
                            if (assetModelViewModel != null)
                            {
                                if (canProceed)
                                {
                                    FormBarMenuViewModel.FormMenuContent = "Menu";
                                    mainModel.ChangedVisibility = Visibility.Visible;
                                    mainModel.AssetClassesModelViewModel.ChangedVisibilityHyperlink = Visibility.Collapsed;
                                    assetModelViewModel.OnStepAsync(Asset.EnumSteps.GridSummaryState);
                                    assetModelViewModel.DynamicAssetClassModelViewModel.SelectedItem = null;
                                    this.ChangedVisibility = Visibility.Collapsed;
                                }
                            }
                        }
                        break;
                    #endregion

                    #region Asset Classes Make
                    case "Asset Make":
                        var mainmodel = this.ScreenDetailViewModel as MainAssetClassesViewModel;
                        if (mainmodel != null)
                        {
                            bool canProceed = true;

                            var assetMakeViewModel =
                                mainmodel.AssetClassesMakeViewModel.ScreenDetailViewModel as AssetClassesMakeViewModel;
                            if (assetMakeViewModel != null)
                            {
                                // validate for mode edit
                                if (assetMakeViewModel.IsCheckedOut && assetMakeViewModel.IsChanged)
                                {
                                    canProceed = this.ValidatePopup();
                                }

                                if (canProceed)
                                {
                                    FormBarMenuViewModel.FormMenuContent = "Menu";
                                    FormBarMenuViewModel.FormBarContent = "Asset Classes";

                                    // hide tab hyperlink
                                    mainmodel.AssetClassesMakeViewModel.ChangedVisibilityHyperlink = Visibility.Collapsed;
                                    mainmodel.ChangedVisibility = Visibility.Visible;

                                    assetMakeViewModel.OnStepAsync(Asset.EnumSteps.MainViewState);
                                    assetMakeViewModel.DynamicAssetClassMakeViewModel.SelectedItem = null;
                                    // collapse toggle
                                    this.ChangedVisibility = Visibility.Collapsed;
                                }
                            }
                        }
                        break;
                    #endregion

                    #region Asset Collateral Classes
                    case "Asset Collateral Classes":
                        var viewmodel = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                        if (viewmodel != null)
                        {
                            bool canProceed = true;

                            var assetCollateralsViewModel =
                                viewmodel.ScreenDetailViewModel as AssetCollateralClassesViewModel;
                            if (assetCollateralsViewModel != null)
                            {
                                // validate for mode edit
                                if (assetCollateralsViewModel.IsCheckedOut && assetCollateralsViewModel.IsChanged)
                                {
                                    canProceed = this.ValidatePopup();
                                }

                                if (canProceed)
                                {
                                    FormBarMenuViewModel.FormMenuContent = "Menu";

                                    // hide tab hyperlink
                                    viewmodel.ChangedVisibilityHyperlink = Visibility.Hidden;
                                    assetCollateralsViewModel.DynamicCollateralViewModel.IsEnableHoverRow = false;
                                    assetCollateralsViewModel.OnStepAsync(Asset.EnumSteps.GridSummaryState);
                                    assetCollateralsViewModel.SelectedCollateral = null;
                                    assetCollateralsViewModel.DynamicCollateralViewModel.SelectedItem = null;
                                    assetCollateralsViewModel.DynamicCollateralViewModel.SelectedRows = new List<object>();   

                                    // collapse toggle
                                    this.ChangedVisibility = Visibility.Collapsed;
                                }
                            }
                        }
                        break;
                    #endregion

                    #region Asset Registers
                    case "Asset Register":
                        var modelRegister = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                        if (modelRegister != null)
                        {
                            bool canProceed = true;

                            var assetRegistersViewModel =
                                modelRegister.ScreenDetailViewModel as AssetRegistersViewModel;
                            if (assetRegistersViewModel != null)
                            {
                                // validate for mode edit
                                if (assetRegistersViewModel.IsCheckedOut && assetRegistersViewModel.IsChanged)
                                {
                                    canProceed = this.ValidatePopup();
                                }

                                if (canProceed)
                                {
                                    FormBarMenuViewModel.FormMenuContent = "Menu";
                                    FormBarMenuViewModel.FormBarContent = "Asset Registers";
                                    // hide tab hyperlink
                                    modelRegister.ChangedVisibilityHyperlink = Visibility.Hidden;
                                    assetRegistersViewModel.DynamicAssetRegisterViewModel.IsEnableHoverRow = false;
                                    assetRegistersViewModel.OnStepAsync(Asset.EnumSteps.MainViewState);
                                    assetRegistersViewModel.SelectedRegister = null;
                                    assetRegistersViewModel.DynamicAssetRegisterViewModel.SelectedItem = null;
                                    assetRegistersViewModel.DynamicAssetRegisterViewModel.SelectedRows = new List<object>();
                                    // collapse toggle
                                    this.ChangedVisibility = Visibility.Collapsed;

                                }
                            }
                        }
                        break;
                    #endregion

                    #region Registered Asset
                    case "Assets":
                        var modelRegisteredAsset = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                        if (modelRegisteredAsset != null)
                        {
                            bool canProceed = true;

                            var assetRegistersViewModel =
                                modelRegisteredAsset.ScreenDetailViewModel as RegisteredAssetViewModel;
                            if (assetRegistersViewModel != null)
                            {
                                // validate for mode edit
                                if (assetRegistersViewModel.IsCheckedOut && assetRegistersViewModel.IsChanged)
                                {
                                    canProceed = this.ValidatePopup();
                                }

                                if (canProceed)
                                {

                                    FormBarMenuViewModel.FormBarContent = "Assets";
                                    // hide tab hyperlink
                                    modelRegisteredAsset.ChangedVisibilityHyperlink = Visibility.Hidden;
                                    assetRegistersViewModel.DynamicRegisteredAssetViewModel.IsEnableHoverRow = false;
                                    assetRegistersViewModel.OnStepAsync(Asset.EnumSteps.MainViewState);
                                    assetRegistersViewModel.SelectedRegistererdAsset = null;
                                    assetRegistersViewModel.DynamicRegisteredAssetViewModel.SelectedItem = null;
                                    assetRegistersViewModel.DynamicRegisteredAssetViewModel.SelectedRows =
                                        new List<object>();
                                    // collapse toggle
                                    this.ChangedVisibility = Visibility.Collapsed;
                                    FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;

                                }
                            }
                        }
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// The back to configuration.
        /// </summary>
        public Action<object> BackToConfiguration;

        /// <summary>
        /// The do back to configuration.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        public void DoBackToConfiguration(object sender)
        {
            if (BackToConfiguration != null)
            {
                BackToConfiguration(sender);
            }
        }

        /// <summary>
        /// The right hand grid item changed when select.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        private void ToggleViewModel_SelectedItemChanged(object sender)
        {
            // After we set 'ToggleViewModel.GridDynamicViewModel = null' the selected Item of Grid inside a Toggle View will be set to null, so The Action OnSelectedItemChange of Toggle ViewModel will be called.
            // We need check Null for 'ToggleViewModel.GridDynamicViewModel' before we do an action.
            if (this.ToggleViewModel == null || this.ToggleViewModel.GridDynamicViewModel == null)
            {
                return;
            }

            switch (Screen)
            {
                case EnumScreen.Users:
                    var mainWindowDetailsTabControlViewModel =
                        this.ScreenDetailViewModel as MainWindowDetailsTabControlViewModel;
                    UserMapping user = this.ToggleViewModel.GridDynamicViewModel.SelectedItem as UserMapping;
                    if (mainWindowDetailsTabControlViewModel != null)
                    {
                        var usersViewModel =
                            mainWindowDetailsTabControlViewModel.UsersMainWindowDetailsVm.ScreenDetailViewModel as
                            UsersViewModel;                        
                        if (usersViewModel != null)
                        {
                            UserDetails selectedUser = usersViewModel.AllUsers.FirstOrDefault(x => user != null && x.UserEntityId == user.UserEntityId);
                            usersViewModel.SelectedUser = selectedUser;
                        }
                    }
                    break;
                case EnumScreen.Groups:
                    var mainWindowDetailsTabControlViewModel1 =
                        this.ScreenDetailViewModel as MainWindowDetailsTabControlViewModel;
                    GroupMapping group = this.ToggleViewModel.GridDynamicViewModel.SelectedItem as GroupMapping;
                    
                    if (mainWindowDetailsTabControlViewModel1 != null)
                    {
                        var groupsViewModel =
                            mainWindowDetailsTabControlViewModel1.GroupsMainWindowDetailsVm.ScreenDetailViewModel as
                            GroupsViewModel;
                        if (groupsViewModel != null)
                        {
                            GroupDetails selectedGroup = groupsViewModel.Groups.FirstOrDefault(x => group != null && x.UserEntityId == group.UserEntityId);
                            groupsViewModel.SelectedGroup = selectedGroup;
                        }
                    }
                    break;
                case EnumScreen.CollectionAssignment:
                    var mainWindowDetailsViewModel = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    CollectionAssignmentMapping collectionAssignment = this.ToggleViewModel.GridDynamicViewModel.SelectedItem as CollectionAssignmentMapping;
                    if (mainWindowDetailsViewModel != null)
                    {
                        var collectionAssignmentViewModel = mainWindowDetailsViewModel.ScreenDetailViewModel as CollectionsAssignmentViewModel;
                        if (collectionAssignmentViewModel != null)
                        {
                            CollectionAssignmentModel collectionAssignmentSelected = collectionAssignmentViewModel.AllCollectionAssignmentDetails.FirstOrDefault(d => collectionAssignment != null && (d.QueueID == collectionAssignment.QueueID && d.ContractId == collectionAssignment.ContractId));
                            collectionAssignmentViewModel.IsChangeContent = false;
                            collectionAssignmentViewModel.SelectedQueue = collectionAssignmentSelected;
                        }
                    }
                    break;

                case EnumScreen.FundingSummary:
                    var mainWindowDetailsViewModelFundingSummary = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    FundingSummaryMaping fundingSummaryMaping = this.ToggleViewModel.GridDynamicViewModel.SelectedItem as FundingSummaryMaping;
                    if (mainWindowDetailsViewModelFundingSummary != null)
                    {
                        var fundingViewModel = mainWindowDetailsViewModelFundingSummary.ScreenDetailViewModel as FundingSummaryViewModel;
                        if (fundingViewModel != null)
                        {
                            FundingSummary fundingSummarySelected = fundingViewModel.TrancheSummary.FirstOrDefault(x => fundingSummaryMaping != null && x.TrancheId == fundingSummaryMaping.TrancheId);
                            fundingViewModel.SelectedTranche = fundingSummarySelected;
                        }
                    }
                    break;
                case EnumScreen.ColletionQueues:
                    var mainWindowDetailsTabControlCollectionQueueViewModel =
                        this.ScreenDetailViewModel as MainWindowDetailsTabControlCollectionQueueViewModel;
                    CollectionsManagementMapping collectionQueue = this.ToggleViewModel.GridDynamicViewModel.SelectedItem as CollectionsManagementMapping;
                    if (mainWindowDetailsTabControlCollectionQueueViewModel != null)
                    {
                        var collectionsManagementViewModel =
                            mainWindowDetailsTabControlCollectionQueueViewModel.ListMainWindowDetailsVm.ScreenDetailViewModel as
                            CollectionsManagementViewModel;
                        if (collectionsManagementViewModel != null)
                        {
                            QueueDetailsModel collectionQueueSelected = collectionsManagementViewModel.AllQueueManagementDetails.FirstOrDefault(x => collectionQueue != null && x.CollectionQueue.ID == collectionQueue.ID);

                            collectionsManagementViewModel.SelectedQueue = collectionQueueSelected;
                        }
                    }
                    break;
                case EnumScreen.AssetFeatures:
                    var model = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    var featureType = this.ToggleViewModel.GridDynamicViewModel.SelectedItem as AssetFeatureTypeRowItem;
                    if (model != null)
                    {
                        var featureTypeViewModel = model.ScreenDetailViewModel as AssetFeaturesViewModel;
                        if (featureTypeViewModel != null)
                        {
                            FeatureType selectedFeatureType = featureTypeViewModel.AllFeatureTypes.FirstOrDefault(x => featureType != null && x.FeatureTypeId == featureType.FeatureTypeId);
                            if (selectedFeatureType != null)
                            {
                                featureTypeViewModel.IsAdd = false;
                                featureTypeViewModel.SelectedFeatureType = selectedFeatureType;
                            }
                        }
                    }
                    break;
                case EnumScreen.AssetClassesCategory:
                    var modelAssetClasses = this.ScreenDetailViewModel as MainAssetClassesViewModel;
                    var itemSelected = this.ToggleViewModel.GridDynamicViewModel.SelectedItem as AssetClassesCategoryRowItem;
                    if (modelAssetClasses != null)
                    {
                        if (itemSelected != null)
                        {
                            var assetClassesCategoryViewModel = modelAssetClasses.AssetCategoryViewModel.ScreenDetailViewModel as AssetClassesCategoryViewModel;
                            if (assetClassesCategoryViewModel != null)
                            {
                                if (assetClassesCategoryViewModel.CheckIfUnSavedChanges().Result)
                                {
                                    assetClassesCategoryViewModel.SelectedCategoryItem = assetClassesCategoryViewModel.ListDataGridItem.FirstOrDefault(x => x.EquipCategoryId == itemSelected.EquipCategoryId);
                                }
                            }
                        }
                    }
                    break;
                case EnumScreen.AssetClassesType:
                    var modelAssetClassesType = this.ScreenDetailViewModel as MainAssetClassesViewModel;
                    var itemTypeSelected = this.ToggleViewModel.GridDynamicViewModel.SelectedItem as AssetClassesTypeRowItem;
                    if (modelAssetClassesType != null)
                    {
                        if (itemTypeSelected != null)
                        {
                            var assetClassesTypeViewModel = modelAssetClassesType.AssetClassesTypeViewModel.ScreenDetailViewModel as AssetClassesTypeViewModel;
                            if (assetClassesTypeViewModel != null)
                            {
                                if (assetClassesTypeViewModel.CheckIfUnSavedChanges().Result)
                                {
                                    assetClassesTypeViewModel.SelectedTypeItem = assetClassesTypeViewModel.ListDataGridItem.FirstOrDefault(x => x.EquipTypeId == itemTypeSelected.EquipTypeId);
                                }
                            }
                        }
                    }
                    break;
                case EnumScreen.AssetClassesModel:
                    var mainAssetModel = this.ScreenDetailViewModel as MainAssetClassesViewModel;
                    var assetModel = this.ToggleViewModel.GridDynamicViewModel.SelectedItem as AssetClassesModelRowItem;
                    if (mainAssetModel != null)
                    {
                        var assetModelViewModel =
                            mainAssetModel.AssetClassesModelViewModel.ScreenDetailViewModel as
                                AssetClassesModelViewModel;
                        if (assetModelViewModel != null)
                        {
                            AssetClassesModelRowItem selectedEquipModel =
                                assetModelViewModel.AllAssetModel.FirstOrDefault(
                                    x => assetModel != null && x.EquipModelId == assetModel.EquipModelId);
                            if (selectedEquipModel != null)
                            {
                                assetModelViewModel.SelectedModel = new EquipModel
                                {
                                    EquipModelId = selectedEquipModel.EquipModelId,
                                    Description = selectedEquipModel.Description,
                                    Enabled = selectedEquipModel.Enabled
                                };
                            }
                        }
                    }
                    break;
                case EnumScreen.AssetClassesMake:
                    var modelMake = this.ScreenDetailViewModel as MainAssetClassesViewModel;
                    var makeType = this.ToggleViewModel.GridDynamicViewModel.SelectedItem as AssetClassesMakeRowItem;
                    if (modelMake != null)
                    {
                        var makeTypeViewModel = modelMake.AssetClassesMakeViewModel.ScreenDetailViewModel as AssetClassesMakeViewModel;
                        if (makeTypeViewModel != null)
                        {
                            if (makeTypeViewModel.CheckIfUnSavedChanges().Result)
                            {
                                AssetClassesMakeRowItem selectedMakeType =
                                    makeTypeViewModel.AllAssetMake.FirstOrDefault(
                                        x => makeType != null && x.EquipMakeId == makeType.EquipMakeId);
                                makeTypeViewModel.DynamicAssetClassMakeViewModel.SelectedItem = selectedMakeType;
                            }
                        }
                    }
                    break;
                case EnumScreen.AssetCollateralClasses:
                    var modelColl = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    var selectedCollateral = this.ToggleViewModel.GridDynamicViewModel.SelectedItem as AssetCollateralRowItem;
                    if (modelColl != null)
                    {
                        var collateralViewModel = modelColl.ScreenDetailViewModel as AssetCollateralClassesViewModel;
                        if (collateralViewModel != null)
                        {
                            collateralViewModel.SelectedCollateral = collateralViewModel.AllCollateralClasses.FirstOrDefault(x => selectedCollateral != null && x.CollateralClassID == selectedCollateral.CollateralClassID);
                        }
                    }
                    break;
                case EnumScreen.AssetRegisters:
                    var modelRegister = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    var register = this.ToggleViewModel.GridDynamicViewModel.SelectedItem as AssetRegisterRowItem;
                    if (modelRegister != null)
                    {
                        var registerViewModel = modelRegister.ScreenDetailViewModel as AssetRegistersViewModel;
                        if (registerViewModel != null)
                        {
                            AssetRegisterRowItem selectedRegister = registerViewModel.AllAssetRegister.FirstOrDefault(x => register != null && x.ID == register.ID);
                            if (selectedRegister != null)
                            {
                                registerViewModel.SelectedRegister = selectedRegister;
                            }
                        }
                    }
                    break;
                case EnumScreen.RegisteredAsset:
                    var modelRegisteredAsset = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    var registeredAsset = this.ToggleViewModel.GridDynamicViewModel.SelectedItem as RegisteredAssetRowItem;
                    if (modelRegisteredAsset != null)
                    {
                        var registerViewModel = modelRegisteredAsset.ScreenDetailViewModel as RegisteredAssetViewModel;
                        if (registerViewModel != null)
                        {
                            RegisteredAssetRowItem selectedRegister = registerViewModel.AllRegisteredAsset.FirstOrDefault(x => registeredAsset != null && x.Id == registeredAsset.Id);
                            if (selectedRegister != null)
                            {
                                registerViewModel.SelectedRegistererdAsset = selectedRegister;
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// update right hand grid when add or edit a record.
        /// </summary>
        /// <param name="currentStep">
        /// The current Step.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        private void ContentViewModel_OnPropertyChanged(EnumSteps currentStep, object item)
        {
            switch (Screen)
            {
                #region User
                case EnumScreen.Users:
                    var mainWindowDetailsTabControlViewModelUsers =
                        this.ScreenDetailViewModel as MainWindowDetailsTabControlViewModel;
                    if (mainWindowDetailsTabControlViewModelUsers != null)
                    {
                        var usersViewModel =
                            mainWindowDetailsTabControlViewModelUsers.UsersMainWindowDetailsVm.ScreenDetailViewModel as
                            UsersViewModel;

                        if (usersViewModel != null && usersViewModel.AllUsers != null)
                        {
                            UserMapping userMapping = new UserMapping();

                            var user = usersViewModel.AllUsers.FirstOrDefault(x => x == item as UserDetails);
                                
                            if (user != null)
                            {
                                userMapping.UserEntityId = user.UserEntityId;
                                userMapping.LoginName = user.UserCredentials.LoginName;
                                userMapping.IsEnabled = user.UserCredentials.IsEnabled;
                                DataRow editItem = null;
                                foreach (var row in ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    var userDetails = item as UserDetails;
                                    if (userDetails != null && row["UserEntityId"].ToString() == userDetails.UserEntityId.ToString(CultureInfo.InvariantCulture))
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }

                                if (editItem != null)
                                {
                                    ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = true;
                                    int index = ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                    ToggleViewModel.GridDynamicViewModel.InsertRow(index, userMapping);
                                }
                                else
                                {
                                    ToggleViewModel.GridDynamicViewModel.AddRow(userMapping);
                                }
                                ToggleViewModel.SetSelectedItem(user);
                            }
                        }
                    }
                    break;
                #endregion

                #region Group
                case EnumScreen.Groups:
                    var mainWindowDetailsTabControlViewModelGroups =
                        this.ScreenDetailViewModel as MainWindowDetailsTabControlViewModel;
                    if (mainWindowDetailsTabControlViewModelGroups != null)
                    {
                        var groupsViewModel =
                            mainWindowDetailsTabControlViewModelGroups.GroupsMainWindowDetailsVm.ScreenDetailViewModel
                            as GroupsViewModel;

                        if (groupsViewModel != null && groupsViewModel.Groups != null)
                        {
                            GroupMapping groupMapping = new GroupMapping();

                            var group = groupsViewModel.Groups.FirstOrDefault(x => x == item as GroupDetails);
                            if (group != null)
                            {
                                groupMapping.UserEntityId = group.UserEntityId;
                                groupMapping.GroupName = group.LXMGroup.GroupName;
                                DataRow editItem = null;
                                foreach (var row in ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    var groupDetails = item as GroupDetails;
                                    if (groupDetails != null && row["UserEntityId"].ToString() == groupDetails.UserEntityId.ToString(CultureInfo.InvariantCulture))
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }

                                if (editItem != null)
                                {
                                    ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = true;
                                    int index = ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                    ToggleViewModel.GridDynamicViewModel.InsertRow(index, groupMapping);
                                }
                                else
                                {
                                    ToggleViewModel.GridDynamicViewModel.AddRow(groupMapping);
                                }
                                ToggleViewModel.SetSelectedItem(group);
                            }
                        }
                    }
                    break;
                #endregion

                #region ColletionQueues
                case EnumScreen.ColletionQueues:
                    var collectionQueueViewModel = this.ScreenDetailViewModel as MainWindowDetailsTabControlCollectionQueueViewModel;
                    QueueDetailsModel queueSource = item as QueueDetailsModel;
                    if (collectionQueueViewModel != null)
                    {
                        var collectionManagementViewModel =
                            collectionQueueViewModel.ListMainWindowDetailsVm.ScreenDetailViewModel as
                            CollectionsManagementViewModel;

                        if (collectionManagementViewModel != null
                            && collectionManagementViewModel.AllQueueManagementDetails != null)
                        {
                            CollectionsManagementMapping collectionsManagementMapping =
                                new CollectionsManagementMapping();
                            
                            var queue =
                                collectionManagementViewModel.AllQueueManagementDetails.FirstOrDefault(d => queueSource != null && d.QueueDetailId == queueSource.QueueDetailId);
                            if (queue != null)
                            {
                                collectionsManagementMapping.Enabled = queue.CollectionQueue.Enabled;
                                collectionsManagementMapping.ID = queue.CollectionQueue.ID;
                                collectionsManagementMapping.QueueName = queue.CollectionQueue.QueueName;
                                collectionsManagementMapping.AssignmentOrder = queue.CollectionQueue.AssignmentOrder;
                                DataRow editItem = null;
                                foreach (var row in ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    var queueDetailsModel = item as QueueDetailsModel;
                                    if (queueDetailsModel != null && row["ID"].ToString() == queueDetailsModel.QueueDetailId.ToString(CultureInfo.InvariantCulture))
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }

                                if (editItem != null)
                                {
                                    ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = true;
                                    int index = ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                    ToggleViewModel.GridDynamicViewModel.InsertRow(index, collectionsManagementMapping);
                                }
                                else
                                {
                                    ToggleViewModel.GridDynamicViewModel.AddRow(collectionsManagementMapping);
                                }
                                ToggleViewModel.SetSelectedItem(queue);
                            }
                        }
                    }
                    break;
                #endregion

                #region CollectionAssignment
                case EnumScreen.CollectionAssignment:
                    var collectionAssignmentViewModel =
                        this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    if (collectionAssignmentViewModel != null)
                    {
                        var viewModel = collectionAssignmentViewModel.ScreenDetailViewModel as CollectionsAssignmentViewModel;
                        CollectionAssignmentModel itemSource = item as CollectionAssignmentModel;
                        if (viewModel != null && itemSource != null)
                        {
                            CollectionAssignmentMapping collectionAssignmentMapping =
                                new CollectionAssignmentMapping();
                            
                            var collectionAssignment =
                                viewModel.FilteredItems.FirstOrDefault(d => d.QueueID == itemSource.QueueID && d.ContractId == itemSource.ContractId);

                            if (collectionAssignment != null)
                            {
                                collectionAssignmentMapping.QueueID = collectionAssignment.QueueID;
                                collectionAssignmentMapping.FollowUpDate = collectionAssignment.FollowUpDate;
                                collectionAssignmentMapping.ContractId = collectionAssignment.ContractId;
                                DataRow editItem = null;
                                foreach (var row in this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    var collectionAssignmentModel = item as CollectionAssignmentModel;
                                    if (row["QueueID"].ToString() == collectionAssignmentModel.QueueID.ToString(CultureInfo.InvariantCulture) && row["ContractId"].ToString() == collectionAssignmentModel.ContractId.ToString(CultureInfo.InvariantCulture))
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }

                                if (editItem != null)
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = true;
                                    int index = this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                    this.ToggleViewModel.GridDynamicViewModel.InsertRow(index, collectionAssignmentMapping);
                                }
                                else
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.AddRow(collectionAssignmentMapping);
                                }
                                this.ToggleViewModel.SetSelectedItem(collectionAssignment);
                            }
                            
                        }
                    }
                    break;
                #endregion

                #region FundingSummary
                case EnumScreen.FundingSummary:
                    var fundingViewModel =
                        this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    if (fundingViewModel != null)
                    {
                        var fundingSummaryViewModel =
                            fundingViewModel.ScreenDetailViewModel as
                            FundingSummaryViewModel;
                        
                        FundingSummaryMaping fundingSummaryMapping =
                                new FundingSummaryMaping();
                        if (fundingSummaryViewModel != null)
                        {
                            var fundingSummary =
                                fundingSummaryViewModel.TrancheSummary.FirstOrDefault(x => x.TrancheId == (int)item);
                            
                            if (fundingSummary != null)
                            {
                                fundingSummaryMapping.TrancheId = fundingSummary.TrancheId;
                                fundingSummaryMapping.FunderName = fundingSummary.FunderName;
                                DataRow editItem = null;
                                foreach (var row in this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    if (row["TrancheId"].ToString() == item.ToString())
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }

                                if (editItem != null)
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = true;
                                    int index = this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);                                    
                                    this.ToggleViewModel.GridDynamicViewModel.InsertRow(
                                            index,
                                            fundingSummaryMapping);
                                }
                                else
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.AddRow(fundingSummaryMapping);
                                }
                                this.ToggleViewModel.SetSelectedItem(fundingSummary);
                            }
                            else
                            {
                                if (currentStep == EnumSteps.Delete)
                                {
                                    DataRow editItem = null;
                                    foreach (var row in this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                    {
                                        if (row["TrancheId"].ToString() == item.ToString())
                                        {
                                            editItem = row;
                                            break;
                                        }
                                    }
                                    if (editItem != null)
                                    {
                                        this.ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = false;
                                        int index = this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                        this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                    }
                                    this.ToggleViewModel.SetSelectedItem(null);
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region Asset Class Category
                case EnumScreen.AssetClassesCategory:
                    var mainWindowDetailsTabControlViewModelCategory =
                        this.ScreenDetailViewModel as MainAssetClassesViewModel;
                    if (mainWindowDetailsTabControlViewModelCategory != null)
                    {
                        var categoryViewModel =
                            mainWindowDetailsTabControlViewModelCategory.AssetCategoryViewModel.ScreenDetailViewModel as
                            AssetClassesCategoryViewModel;

                        if (categoryViewModel != null && categoryViewModel.ListDataGridItem != null)
                        {
                            AssetClassesCategoryRowItem categoryMapping = new AssetClassesCategoryRowItem();

                            var category = categoryViewModel.ListDataGridItem.FirstOrDefault(x => x.EquipCategoryId == (item as EquipCategory).EquipCatId);

                            if (category != null)
                            {
                                categoryMapping.EquipCategoryId = (item as EquipCategory).EquipCatId;
                                categoryMapping.Category = (item as EquipCategory).Description;
                                categoryMapping.Enabled = (item as EquipCategory).Enabled;
                                DataRow editItem = null;
                                foreach (var row in ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    if (row["EquipCategoryId"].ToString() == (item as EquipCategory).EquipCatId.ToString())
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }

                                if (editItem != null)
                                {
                                    ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = true;
                                    int index = ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                    ToggleViewModel.GridDynamicViewModel.InsertRow(index, categoryMapping);
                                }
                                else
                                {
                                    ToggleViewModel.GridDynamicViewModel.AddRow(categoryMapping);
                                }
                                ToggleViewModel.SetSelectedItem(item);
                            }
                        }
                    }
                    break;
                #endregion

                #region Asset Class Type
                case EnumScreen.AssetClassesType:
                    var mainWindowDetailsTabControlViewModelType =
                        this.ScreenDetailViewModel as MainAssetClassesViewModel;
                    if (mainWindowDetailsTabControlViewModelType != null)
                    {
                        var typeViewModel =
                            mainWindowDetailsTabControlViewModelType.AssetClassesTypeViewModel.ScreenDetailViewModel as AssetClassesTypeViewModel;

                        if (typeViewModel != null && typeViewModel.ListDataGridItem != null)
                        {
                            AssetClassesTypeRowItem typeMapping = new AssetClassesTypeRowItem();

                            var type = typeViewModel.ListDataGridItem.FirstOrDefault(x => x.EquipTypeId == (item as EquipType).EquipTypeId);

                            if (type != null)
                            {
                                typeMapping.EquipTypeId = (item as EquipType).EquipTypeId;
                                typeMapping.TypeDescription = (item as EquipType).Description;
                                typeMapping.Enabled = (item as EquipType).Enabled;
                                DataRow editItem = null;
                                foreach (var row in ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    if (row["EquipTypeId"].ToString() == (item as EquipType).EquipTypeId.ToString())
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }

                                if (editItem != null)
                                {
                                    ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = true;
                                    int index = ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                    ToggleViewModel.GridDynamicViewModel.InsertRow(index, typeMapping);
                                }
                                else
                                {
                                    ToggleViewModel.GridDynamicViewModel.AddRow(typeMapping);
                                }
                                ToggleViewModel.SetSelectedItem(item);
                            }
                        }
                    }
                    break;
                #endregion

                #region AssetClassesModel
                case EnumScreen.AssetClassesModel:
                    var modelViewModel = this.ScreenDetailViewModel as MainAssetClassesViewModel;
                    if (modelViewModel != null)
                    {
                        var modelDetailViewModel = modelViewModel.AssetClassesModelViewModel.ScreenDetailViewModel
                            as AssetClassesModelViewModel;
                        AssetClassesModelRowItem assetModelMapping=new AssetClassesModelRowItem();
                        if (modelDetailViewModel != null)
                        {
                            var assetDetail =
                                modelDetailViewModel.AllAssetModel.FirstOrDefault(x=> x.EquipModelId == ((EquipModel)item).EquipModelId);

                            if (assetDetail != null)
                            {
                                assetModelMapping.EquipModelId = assetDetail.EquipModelId;
                                assetModelMapping.Description = assetDetail.Description;
                                assetModelMapping.Enabled = assetDetail.Enabled;
                                DataRow editItem = null;
                                foreach (var row in this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    if (row["EquipModelId"].ToString() == ((EquipModel)item).EquipModelId.ToString())
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }

                                if (editItem != null)
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = true;
                                    int index = this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                    this.ToggleViewModel.GridDynamicViewModel.InsertRow(
                                            index,
                                            assetModelMapping);
                                }
                                else
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.AddRow(assetModelMapping);
                                }
                                this.ToggleViewModel.SetSelectedItem(item);
                            }
                        }
                    }
                    break;
                #endregion

                #region AssetFeatures
                case EnumScreen.AssetFeatures:
                    var featureViewModel =
                        this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    if (featureViewModel != null)
                    {
                        var assetFeatureViewModel =
                            featureViewModel.ScreenDetailViewModel as
                            AssetFeaturesViewModel;

                        AssetFeatureTypeRowItem featureTypeMapping =
                                new AssetFeatureTypeRowItem();
                        if (assetFeatureViewModel != null)
                        {
                            var feature =
                                assetFeatureViewModel.AllFeatureTypes.FirstOrDefault(x => x.FeatureTypeId == (int)item);

                            if (feature != null)
                            {
                                featureTypeMapping.FeatureTypeId = feature.FeatureTypeId;
                                featureTypeMapping.FeatureName = feature.FeatureName;
                                featureTypeMapping.Enabled = feature.Enabled;
                                featureTypeMapping.RequiredLength = feature.RequiredLength == -1 ? "N/A" : feature.RequiredLength.ToString();
                                DataRow editItem = null;
                                foreach (var row in this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    if (row["FeatureTypeId"].ToString() == item.ToString())
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }

                                if (editItem != null)
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = true;
                                    int index = this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                    this.ToggleViewModel.GridDynamicViewModel.InsertRow(
                                            index,
                                            featureTypeMapping);
                                }
                                else
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.AddRow(featureTypeMapping);
                                }
                                this.ToggleViewModel.SetSelectedItem(feature);
                            }
                            else
                            {
                                if (currentStep == EnumSteps.Delete)
                                {
                                    DataRow editItem = null;
                                    foreach (var row in this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                    {
                                        if (row["FeatureTypeId"].ToString() == item.ToString())
                                        {
                                            editItem = row;
                                            break;
                                        }
                                    }
                                    if (editItem != null)
                                    {
                                        this.ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = false;
                                        int index = this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                        this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                    }
                                    this.ToggleViewModel.SetSelectedItem(null);
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region AssetClassesMake
                case EnumScreen.AssetClassesMake:
                    var makeViewModel =
                        this.ScreenDetailViewModel as MainAssetClassesViewModel;
                    if (makeViewModel != null)
                    {
                        var assetClassesMakeViewModel = makeViewModel.AssetClassesMakeViewModel.ScreenDetailViewModel as AssetClassesMakeViewModel;

                        AssetClassesMakeRowItem makeTypeMapping = new AssetClassesMakeRowItem();
                        if (assetClassesMakeViewModel != null)
                        {
                            var make =
                                assetClassesMakeViewModel.AllAssetMake.FirstOrDefault(x => x.EquipMakeId == (int)item);

                            if (make != null)
                            {
                                makeTypeMapping.EquipMakeId = make.EquipMakeId;
                                makeTypeMapping.Description = make.Description;
                                makeTypeMapping.Enabled = make.Enabled;
                                DataRow editItem = null;
                                foreach (var row in this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    if (row["EquipMakeId"].ToString() == item.ToString())
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }

                                if (editItem != null)
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = true;
                                    int index =
                                        this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                    this.ToggleViewModel.GridDynamicViewModel.InsertRow(index, makeTypeMapping);
                                }
                                else
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.AddRow(makeTypeMapping);
                                }
                                this.ToggleViewModel.SetSelectedItem(make);
                            }
                        }
                    }
                    break;
                #endregion

                #region Asset Collateral Classes
                case EnumScreen.AssetCollateralClasses:
                    var collateralViewModel =
                        this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    if (collateralViewModel != null)
                    {
                        var assetCollateralViewModel =
                            collateralViewModel.ScreenDetailViewModel as
                            AssetCollateralClassesViewModel;

                        AssetCollateralRowItem collateralMapping =
                                new AssetCollateralRowItem();
                        if (assetCollateralViewModel != null)
                        {
                            var collateral =
                                assetCollateralViewModel.AllCollateralClasses.FirstOrDefault(x => x.CollateralClassID == (int)item);

                            if (collateral != null)
                            {
                                collateralMapping.Description = collateral.Description;
                                collateralMapping.CollateralClassID = collateral.CollateralClassID;
                                DataRow editItem = null;
                                foreach (var row in this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    if (row["CollateralClassID"].ToString() == item.ToString())
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }

                                if (editItem != null)
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = true;
                                    int index = this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                    this.ToggleViewModel.GridDynamicViewModel.InsertRow(
                                            index,
                                            collateralMapping);
                                }
                                else
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.AddRow(collateralMapping);
                                }
                                this.ToggleViewModel.SetSelectedItem(collateral);
                            }
                        }
                    }
                    break;
                #endregion

                #region AssetRegister
                case EnumScreen.AssetRegisters:
                    var registerViewModel =
                        this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    if (registerViewModel != null)
                    {
                        var assetRegisterViewModel =
                            registerViewModel.ScreenDetailViewModel as AssetRegistersViewModel;

                        AssetRegisterRowItem registerMapping = new AssetRegisterRowItem();
                        if (assetRegisterViewModel != null)
                        {
                            var register = assetRegisterViewModel.AllAssetRegister.FirstOrDefault(x => x.ID == (int)item);

                            if (register != null)
                            {
                                registerMapping.ID = register.ID;
                                registerMapping.RegisterName = register.RegisterName;
                                registerMapping.InternalOnly = register.InternalOnly;

                                DataRow editItem = null;
                                foreach (var row in this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    if (row["ID"].ToString() == item.ToString())
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }

                                if (editItem != null)
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = true;
                                    int index = this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                    this.ToggleViewModel.GridDynamicViewModel.InsertRow(index, registerMapping);
                                }
                                else
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.AddRow(registerMapping);
                                }
                                this.ToggleViewModel.SetSelectedItem(register);
                            }
                            else
                            {
                                DataRow editItem = null;
                                foreach (var row in this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    if (row["ID"].ToString() == item.ToString())
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }
                                if (editItem != null)
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = false;
                                    int index = this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                }
                                this.ToggleViewModel.SetSelectedItem(null);
                            }
                        }
                    }
                    break;
                #endregion

                #region RegisteredAsset
                case EnumScreen.RegisteredAsset:
                    var registeredAssetViewModel =
                        this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    if (registeredAssetViewModel != null)
                    {
                        var assetRegisterViewModel =
                            registeredAssetViewModel.ScreenDetailViewModel as RegisteredAssetViewModel;

                        RegisteredAssetRowItem registerMapping = new RegisteredAssetRowItem();
                        if (assetRegisterViewModel != null)
                        {
                            var register = assetRegisterViewModel.AllRegisteredAsset.FirstOrDefault(x => x.Id == (int)item);

                            if (register != null)
                            {
                                registerMapping.Id = register.Id;
                                registerMapping.AssetRegisterId = register.AssetRegisterId;
                                registerMapping.AssetState = register.AssetState;

                                DataRow editItem = null;
                                foreach (var row in this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    if (row["Id"].ToString() == item.ToString())
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }

                                if (editItem != null)
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = true;
                                    int index = this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                    this.ToggleViewModel.GridDynamicViewModel.InsertRow(index, registerMapping);
                                }
                                else
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.AddRow(registerMapping);
                                }
                                this.ToggleViewModel.SetSelectedItem(register);
                            }
                            else
                            {
                                DataRow editItem = null;
                                foreach (var row in this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows)
                                {
                                    if (row["Id"].ToString() == item.ToString())
                                    {
                                        editItem = row;
                                        break;
                                    }
                                }
                                if (editItem != null)
                                {
                                    this.ToggleViewModel.GridDynamicViewModel.IsSetSelectedItem = false;
                                    int index = this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.IndexOf(editItem);
                                    this.ToggleViewModel.GridDynamicViewModel.MembersTable.Rows.RemoveAt(index);
                                }
                                this.ToggleViewModel.SetSelectedItem(null);
                            }
                        }
                    }
                    break;
                #endregion
            }
        }
        #region Public Method

        /// <summary>
        /// The set selected tab.
        /// </summary>
        public void SetSelectedTab()
        {
            var mainWindowDetailsTabControlViewModel = this.ScreenDetailViewModel as MainWindowDetailsTabControlViewModel;
            if (mainWindowDetailsTabControlViewModel != null)
            {
                mainWindowDetailsTabControlViewModel.PropertyChanged -= this.TabControlViewModel_OnPropertyChanged;
                mainWindowDetailsTabControlViewModel.PropertyChanged += this.TabControlViewModel_OnPropertyChanged;
            }
            var mainWindowDetailsTabControlCollectionQueueViewModel = this.ScreenDetailViewModel as MainWindowDetailsTabControlCollectionQueueViewModel;
            if (mainWindowDetailsTabControlCollectionQueueViewModel != null)
            {
                mainWindowDetailsTabControlCollectionQueueViewModel.PropertyChanged -= this.TabControlViewModel_OnPropertyChanged;
                mainWindowDetailsTabControlCollectionQueueViewModel.PropertyChanged += this.TabControlViewModel_OnPropertyChanged;
            }
        }

        /// <summary>
        /// The load action link.
        /// </summary>
        public void LoadActionLink()
        {
            MainWindowDetailsTabControlViewModel viewModel1 = ScreenDetailViewModel as MainWindowDetailsTabControlViewModel;
            MainWindowDetailsViewModel viewModel2 = ScreenDetailViewModel as MainWindowDetailsViewModel;
            MainWindowDetailsTabControlCollectionQueueViewModel viewModel3 = ScreenDetailViewModel as MainWindowDetailsTabControlCollectionQueueViewModel;

            if (viewModel1 != null)
            {
                viewModel1.UsersMainWindowDetailsVm.OnHyperlinkScreen = this.ChangeHyperlinkName;
                viewModel1.GroupsMainWindowDetailsVm.OnHyperlinkScreen = this.ChangeHyperlinkName;
            }
            if (viewModel2 != null)
            {
                viewModel2.OnHyperlinkScreen = this.ChangeHyperlinkName;
            }
            if (viewModel3 != null)
            {
                viewModel3.ListMainWindowDetailsVm.OnHyperlinkScreen = this.ChangeHyperlinkName;
            }
        }

        /// <summary>
        /// The select user.
        /// </summary>
        public void OnRaiseStepChanged()
        {
            LoadActionLink();
            var viewModel = this.ScreenDetailViewModel;
            if (viewModel != null)
            {
                viewModel.RaiseStepChanged = this.ProcessingStepsOnChild;
            }
        }

        /// <summary>
        /// When cancel add new item or cancel copy new item, back to grid summary
        /// </summary>
        public void OnCancelNewItem()
        {
            var viewModel = this.ScreenDetailViewModel;
            if (viewModel != null)
            {
                viewModel.CancelNewItem = CollapseToggle;
            }

        }

        /// <summary>
        /// behavior for collapse right hand grid.
        /// </summary>
        /// <param name="screen">
        /// The screen.
        /// </param>
        private void CollapseToggle(EnumScreen screen)
        {
            this.ChangedVisibility = Visibility.Collapsed;
            this.FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
            switch (screen)
            {
                case EnumScreen.Users:
                    this.FormBarMenuViewModel.FormBarContent = "Users";
                    break;
                case EnumScreen.Groups:
                    this.FormBarMenuViewModel.FormBarContent = "Groups";
                    break;
                case EnumScreen.ColletionQueues:
                    this.FormBarMenuViewModel.FormBarContent = "Collection Queues";
                    this.FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    this.FormBarMenuViewModel.FormMenuContent = "Menu";
                    break;
                case EnumScreen.FundingSummary:
                    var mainWindowDetailsViewModelFundingSummary = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    if (mainWindowDetailsViewModelFundingSummary != null)
                    {
                        mainWindowDetailsViewModelFundingSummary.ChangedVisibilityHyperlink = Visibility.Hidden;
                    }
                    break;
                case EnumScreen.AssetClassesModel:
                    this.FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    this.FormBarMenuViewModel.FormMenuContent = "Menu";
                    break;
                case EnumScreen.AssetFeatures:
                    this.FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    this.FormBarMenuViewModel.FormMenuContent = "Menu";
                    var viewmodel = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    if (viewmodel != null)
                    {
                        viewmodel.ChangedVisibilityHyperlink = Visibility.Hidden;
                    }
                    break;

                case EnumScreen.AssetClassesCategory:
                    this.FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    this.FormBarMenuViewModel.FormMenuContent = "Menu";
                    break;

                case EnumScreen.AssetClassesType:
                    this.FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    this.FormBarMenuViewModel.FormMenuContent = "Menu";
                    break;

                case EnumScreen.AssetClassesMake:
                    this.FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    this.FormBarMenuViewModel.FormMenuContent = "Menu";
                    break;

                case EnumScreen.AssetCollateralClasses:
                    this.FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    this.FormBarMenuViewModel.FormMenuContent = "Menu";
                    viewmodel = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    if (viewmodel != null)
                    {
                        viewmodel.ChangedVisibilityHyperlink = Visibility.Hidden;
                    }
                    break;
                case EnumScreen.AssetRegisters:
                    this.FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    this.FormBarMenuViewModel.FormMenuContent = "Menu";
                    this.FormBarMenuViewModel.FormBarContent = "Asset Registers";
                    viewmodel = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    if (viewmodel != null)
                    {
                        viewmodel.ChangedVisibilityHyperlink = Visibility.Hidden;
                    }
                    break;
                case EnumScreen.RegisteredAsset:
                    viewmodel = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                    if (viewmodel != null)
                    {
                        viewmodel.ChangedVisibilityHyperlink = Visibility.Hidden;
                    }
                    break;
            }

        }

        /// <summary>
        /// The change hyperlink name on form bar.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        private void ChangeHyperlinkName(object o)
        {
            CustomHyperlink customHyperlink = o as CustomHyperlink;
            string onScreen;
            if (customHyperlink != null)
            {
                onScreen = this.HyperLinkName(customHyperlink.Action);
                switch (customHyperlink.Screen)
                {
                    case EnumScreen.Users:
                        this.FormBarMenuViewModel.FormBarContent = "User " + onScreen;
                        this._formBarCurrent = "User " + onScreen;
                        break;
                    case EnumScreen.Groups:
                        this.FormBarMenuViewModel.FormBarContent = "Group " + onScreen;
                        this._formGroupBarCurrent = "Group " + onScreen;
                        break;
                    case EnumScreen.Configuration:
                        this.FormBarMenuViewModel.FormBarContent = customHyperlink.HyperlinkHeader;
                        this._formBarCurrent = "Configuration";
                        break;
                    case EnumScreen.FundingSummary:
                        this.FormBarMenuViewModel.FormBarContent = "Funding Detail";
                        break;
                    case EnumScreen.FundingContact:
                        this.FormBarMenuViewModel.FormBarContent = "Funding Contracts";
                        break;
                    case EnumScreen.CollectionAssignment:
                        this.FormBarMenuViewModel.FormBarContent = "Assignment " + onScreen;
                        this._formBarCurrent = "Assignment " + onScreen;
                        break;
                    case EnumScreen.ColletionQueues:
                        this.FormBarMenuViewModel.FormBarContent = "Collection Queue " + onScreen;
                        this._formBarCurrent = "Collection Queue " + onScreen;
                        break;

                }
            }
        }

        /// <summary>
        /// Declare hyper link name.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string HyperLinkName(object o)
        {
            var screen = o is HyperLinkAction ? (HyperLinkAction)o : HyperLinkAction.CredentialsState;
            string onScreen = string.Empty;
            switch (screen)
            {
                case HyperLinkAction.CredentialsState:
                    onScreen = "Credentials";
                    break;
                case HyperLinkAction.DetailsState:
                    onScreen = "Detail";
                    break;
                case HyperLinkAction.GroupsState:
                    onScreen = "Groups";
                    break;
                case HyperLinkAction.PermissionsState:
                    onScreen = "Permissions";
                    break;
                case HyperLinkAction.PersonalDetailsState:
                    onScreen = "Detail";
                    break;
                case HyperLinkAction.SummaryState:
                    onScreen = "Summary";
                    break;
                case HyperLinkAction.UsersState:
                    onScreen = "Members";
                    break;
                case HyperLinkAction.Details:
                    onScreen = "Detail";
                    break;
                case HyperLinkAction.Activity:
                    onScreen = "Activity";
                    break;
            }
            return onScreen;
        }

        /// <summary>
        /// handle click form menu.
        /// </summary>
        public void ClickHyperlink()
        {
            var viewModel = this.FormBarMenuViewModel;
            if (viewModel != null)
            {
                viewModel.ItemClicked = BacktoGrid;
            }
        }

        /// <summary>
        /// Process actions or steps were raised on content
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
        public void ProcessingStepsOnChild(EnumScreen e, object _params, object item)
        {
            Screen = e;

            // handle behavior for step on content
            EnumSteps currentStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), _params.ToString());
            object firstSelectedItem = null;
            if (FormBarMenuViewModel != null)
            {
                switch (currentStep)
                {
                    case EnumSteps.Edit:
                    case EnumSteps.Transfer:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                        return;
                    case EnumSteps.EditBulkUpdate:
                        switch (Screen)
                        {
                            case EnumScreen.AssetCollateralClasses:
                            case EnumScreen.AssetFeatures:
                                var assetFeatureVm = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                                if (assetFeatureVm != null)
                                {
                                    assetFeatureVm.ChangedVisibilityHyperlink = Visibility.Hidden;
                                }
                                FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                                return;
                        }
                        break;                       
                    case EnumSteps.Cancel:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        return;
                    case EnumSteps.CancelBulkUpdate:
                        switch (Screen)
                        {
                            case EnumScreen.AssetCollateralClasses:
                            case EnumScreen.AssetFeatures:
                                var assetFeatureVm = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                                if (assetFeatureVm != null)
                                {
                                    assetFeatureVm.ChangedVisibilityHyperlink = Visibility.Hidden;
                                }
                                FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                                return;
                        }
                        break;
                    case EnumSteps.CancelAssignFeature:
                        switch (Screen)
                        {
                            case EnumScreen.AssetCollateralClasses:
                                FormBarMenuViewModel.FormMenuContent = "Asset Collateral Classes";
                                var assetFeatureVm = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                                if (assetFeatureVm != null)
                                {
                                    assetFeatureVm.ChangedVisibilityHyperlink = Visibility.Hidden;
                                }
                                FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                                return;
                            case EnumScreen.AssetFeatures:
                                FormBarMenuViewModel.FormMenuContent = "Asset Features";
                                assetFeatureVm = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                                if (assetFeatureVm != null)
                                {
                                    assetFeatureVm.ChangedVisibilityHyperlink = Visibility.Hidden;
                                }
                                FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                                return;
                            case EnumScreen.AssetClassesCategory:
                                FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                                return;
                            case EnumScreen.AssetClassesType:
                                FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                                return;
                        }
                        break;
                    case EnumSteps.CancelAssignMake:
                        switch (Screen)
                        {
                            case EnumScreen.AssetClassesType:
                                FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                                return;
                        }
                        break;
                    case EnumSteps.ItemLocked:
                        if (item != null)
                        {
                            FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                            switch (Screen)
                            {
                                case EnumScreen.CollectionAssignment:
                                    CollectionAssignmentModel selectedQueue = item as CollectionAssignmentModel;
                                    this.ToggleViewModel.SetSelectedItem(selectedQueue);
                                    break;
                            }
                        }
                        return;
                    case EnumSteps.Add:
                        var toggleViewModel = this.ToggleViewModel.GridDynamicViewModel;
                        if (toggleViewModel != null && toggleViewModel.SelectedItem != null)
                        {
                            toggleViewModel.SelectedItem = null;
                        }
                        if (Screen == EnumScreen.AssetClassesMake)
                        {
                            FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                        }
                        break;
                    case EnumSteps.Delete:
                        if (item != null)
                        {
                            switch (Screen)
                            {
                                case EnumScreen.AssetFeatures:
                                    this.ContentViewModel_OnPropertyChanged(currentStep, item);
                                    FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                                    return;
                            }
                        }
                        var mainWindowDetailsViewModelFundingSummary = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                        if (mainWindowDetailsViewModelFundingSummary != null)
                        {
                            mainWindowDetailsViewModelFundingSummary.ChangedVisibilityHyperlink = Visibility.Hidden;
                        }
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                        ContentViewModel_OnPropertyChanged(currentStep, item);
                        return;
                    case EnumSteps.Save:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        ContentViewModel_OnPropertyChanged(currentStep, item);
                        return;
                    //Asset classes Category step change for mode bulk update
                    case EnumSteps.AssetClassesCategoryAssignFeaturesState:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                        FormBarMenuViewModel.FormBarContent = "Assign Features";
                        FormBarMenuViewModel.FormMenuContent = "Asset Category";
                        return;
                    case EnumSteps.AssetClassesCategoryAssignTypesState:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                        FormBarMenuViewModel.FormBarContent = "Assign Types";
                        FormBarMenuViewModel.FormMenuContent = "Asset Category";
                        return;
                    case EnumSteps.AssetClassesCategoryUpdateDepreciationState:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                        FormBarMenuViewModel.FormBarContent = "Update Depreciation";
                        FormBarMenuViewModel.FormMenuContent = "Asset Category";
                        return;
                    //Asset classes Type step change for mode bulk update
                    case EnumSteps.AssetClassesTypeAssignFeaturesState:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                        FormBarMenuViewModel.FormBarContent = "Assign Features";
                        FormBarMenuViewModel.FormMenuContent = "Asset Type";
                        return;
                    case EnumSteps.AssetClassesTypeAssignMakeState:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                        FormBarMenuViewModel.FormBarContent = "Assign Make";
                        FormBarMenuViewModel.FormMenuContent = "Asset Type";
                        return;
                    case EnumSteps.AssetClassesTypeUpdateDepreciationState:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                        FormBarMenuViewModel.FormBarContent = "Update Depreciation";
                        FormBarMenuViewModel.FormMenuContent = "Asset Type";
                        return;
                    case EnumSteps.SaveAssignFeature:
                        switch (Screen)
                        {
                            case EnumScreen.AssetCollateralClasses:
                            case EnumScreen.AssetFeatures:
                                var assetFeatureVm = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                                if (assetFeatureVm != null)
                                {
                                    assetFeatureVm.ChangedVisibilityHyperlink = Visibility.Hidden;
                                }
                                FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                                return;
                            case EnumScreen.AssetClassesCategory:
                                FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                                return;
                            case EnumScreen.AssetClassesType:
                                FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                                return;
                        }
                        break;
                    case EnumSteps.SaveAssignTypes:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        switch (Screen)
                        {
                            case EnumScreen.AssetCollateralClasses:
                                var collateralVm = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                                if (collateralVm != null)
                                {
                                    collateralVm.ChangedVisibilityHyperlink = Visibility.Hidden;
                                }
                                return;
                        }
                        return;
                    case EnumSteps.SaveAssignMake:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        return;
                    case EnumSteps.SaveUpdateDepreciation:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        return;
                    case EnumSteps.CancelAssignTypes:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        return;
                    case EnumSteps.CancelUpdateDepreciation:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        return;
                    case EnumSteps.BulkUpdate:
                        switch (Screen)
                        {
                            case EnumScreen.AssetCollateralClasses:
                                var collateralVm = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                                if (collateralVm != null)
                                {
                                    collateralVm.ChangedVisibilityHyperlink = Visibility.Hidden;
                                }
                                return;
                            case EnumScreen.AssetFeatures:
                                var assetFeatureVm = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                                if (assetFeatureVm != null)
                                {
                                    assetFeatureVm.ChangedVisibilityHyperlink = Visibility.Hidden;
                                }
                                return;
                        }
                        break;
                    case EnumSteps.SelectOldTabHyperlink:
                        return;
                    //case EnumSteps.AssignModel:
                    //    if (Screen == EnumScreen.AssetClassesMake)
                    //    {
                    //        var assetMakeVm = this.ScreenDetailViewModel as MainAssetClassesViewModel;
                    //        if (assetMakeVm != null)
                    //        {
                    //            assetMakeVm.ChangedVisibility = Visibility.Collapsed;
                    //        }
                    //        return;
                    //    }
                    //    break;
                    case EnumSteps.SaveAssignModel:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        return;
                    case EnumSteps.AssignModel:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Collapsed;
                        FormBarMenuViewModel.FormBarContent = "Asset Make";
                        FormBarMenuViewModel.FormMenuContent = "Asset Make";
                        return;
                    case EnumSteps.CancelAssignModel:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        return;
                    case EnumSteps.SaveRegisterSummary:
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        FormBarMenuViewModel.FormBarContent = "Asset Registers";
                        FormBarMenuViewModel.FormMenuContent = "Menu";
                        return;
                    //case EnumSteps.SelectRegisteredAsset:
                    //    FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                    //    FormBarMenuViewModel.FormBarContent = "Assets Detail";
                    //    FormBarMenuViewModel.FormMenuContent = "Assets";
                    //    return;
                }

                // handle behavior for screens when select item
                switch (Screen)
                {
                    #region users
                    case EnumScreen.Users:
                        var mainWindowDetailsTabControlViewModel = this.ScreenDetailViewModel as MainWindowDetailsTabControlViewModel;
                        if (mainWindowDetailsTabControlViewModel != null)
                        {
                            var usersViewModel = mainWindowDetailsTabControlViewModel.UsersMainWindowDetailsVm.ScreenDetailViewModel as UsersViewModel;
                            if (usersViewModel != null)
                            {
                                if (usersViewModel.AllUsers != null)
                                {
                                    // load data for right hand grid
                                    if (this.ToggleViewModel.GridDynamicViewModel == null || this.ToggleViewModel.Screen != Screen)
                                    {
                                        var userCredentials = usersViewModel.AllUsers.Select(x => x.UserCredentials).ToList();
                                        var userMappings = from d in userCredentials
                                                           select new UserMapping
                                                                           {
                                                                               UserEntityId = d.UserEntityId,
                                                                               LoginName = d.LoginName,
                                                                               IsEnabled = d.IsEnabled,
                                                                           };

                                        // create column and data for dynamic grid
                                        this.ToggleViewModel.GridDynamicViewModel = null;
                                        this.ToggleViewModel.GridDynamicViewModel = new DynamicGridViewModel(typeof(UserMapping));
                                        this.ToggleViewModel.GridDynamicViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "LoginName", Header = "Login Name" },
                                                                                             new DynamicColumn { ColumnName = "IsEnabled",  Header = "Enabled" },
                                                                                         };
                                        this.ToggleViewModel.GridDynamicViewModel.GridDataRows = userMappings.ToList<object>();
                                        this.ToggleViewModel.GridDynamicViewModel.LoadRadGridView();
                                        this.ToggleViewModel.Header = "User List";
                                        this.ToggleViewModel.Screen = Screen;
                                    }
                                    firstSelectedItem = usersViewModel.SelectedUser;
                                }
                            }
                        }

                        // change behavior form bar menu
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        FormBarMenuViewModel.FormMenuContent = "Users";
                        if (this._formBarCurrent != string.Empty)
                        {
                            FormBarMenuViewModel.FormBarContent = this._formBarCurrent;
                        }
                        else
                        {
                            if (FormBarMenuViewModel.FormBarContent == "Users")
                            {
                                FormBarMenuViewModel.FormBarContent = "User Detail";
                            }
                        }

                        this._formBarCurrent = FormBarMenuViewModel.FormBarContent;

                        // Visible right hand grid
                        ChangedVisibility = Visibility.Visible;
                        break;
                    #endregion

                    #region groups
                    case EnumScreen.Groups:
                        var mainWindowDetailsTabControlViewModel1 = this.ScreenDetailViewModel as MainWindowDetailsTabControlViewModel;
                        if (mainWindowDetailsTabControlViewModel1 != null)
                        {
                            var groupsViewModel = mainWindowDetailsTabControlViewModel1.GroupsMainWindowDetailsVm.ScreenDetailViewModel as GroupsViewModel;
                            if (groupsViewModel != null)
                            {
                                if (groupsViewModel.Groups != null)
                                {
                                    // load data for right hand grid
                                    if (this.ToggleViewModel.GridDynamicViewModel == null || this.ToggleViewModel.Screen != Screen)
                                    {
                                        var lxmGroup = groupsViewModel.Groups.Select(x => x.LXMGroup).ToList();

                                        var groupMappings = from d in lxmGroup
                                                            select new GroupMapping
                                                            {
                                                                UserEntityId = d.UserEntityId,
                                                                GroupName = d.GroupName,
                                                            };

                                        // create column and data for dynamic grid
                                        this.ToggleViewModel.GridDynamicViewModel = null;
                                        this.ToggleViewModel.GridDynamicViewModel = new DynamicGridViewModel(typeof(GroupMapping));
                                        this.ToggleViewModel.GridDynamicViewModel.GridColumns = new List<DynamicColumn>
                                                                                                    {
                                                                                                        new DynamicColumn
                                                                                                            {
                                                                                                                ColumnName
                                                                                                                    =
                                                                                                                    "GroupName",
                                                                                                                Header
                                                                                                                    =
                                                                                                                    "Group Name"
                                                                                                            }
                                                                                                    };
                                        this.ToggleViewModel.GridDynamicViewModel.GridDataRows = groupMappings.ToList<object>();
                                        this.ToggleViewModel.GridDynamicViewModel.LoadRadGridView();
                                        this.ToggleViewModel.Header = "Group List";
                                        this.ToggleViewModel.Screen = Screen;
                                    }
                                    firstSelectedItem = groupsViewModel.SelectedGroup;
                                }

                            }
                        }

                        // change behavior form bar menu
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        FormBarMenuViewModel.FormMenuContent = "Groups";
                        if (this._formGroupBarCurrent != string.Empty)
                        {
                            FormBarMenuViewModel.FormBarContent = this._formGroupBarCurrent;
                        }
                        else
                        {
                            if (FormBarMenuViewModel.FormBarContent == "Groups")
                            {
                                FormBarMenuViewModel.FormBarContent = "Group Detail";
                            }
                            else
                            {
                                FormBarMenuViewModel.FormBarContent = "Groups";
                            }
                        }

                        this._formGroupBarCurrent = FormBarMenuViewModel.FormBarContent;

                        // Visible right hand grid
                        ChangedVisibility = Visibility.Visible;
                        break;
                    #endregion

                    #region collectionAssignment
                    case EnumScreen.CollectionAssignment:
                        var mainWindowDetailsViewModel = this.ScreenDetailViewModel as MainWindowDetailsViewModel;

                        if (mainWindowDetailsViewModel != null)
                        {
                            var collectionAssignmentViewModel = mainWindowDetailsViewModel.ScreenDetailViewModel as CollectionsAssignmentViewModel;
                            if (collectionAssignmentViewModel != null)
                            {
                                // load data for right hand grid
                                var collectionAssignmentMappings = from d in collectionAssignmentViewModel.FilteredItems
                                                                   select new CollectionAssignmentMapping
                                                                   {
                                                                       QueueID = d.QueueID,
                                                                       FollowUpDate = d.FollowUpDate,
                                                                       ContractId = d.ContractId
                                                                   };

                                // create column and data for dynamic grid
                                this.ToggleViewModel.GridDynamicViewModel = null;
                                this.ToggleViewModel.GridDynamicViewModel = new DynamicGridViewModel(typeof(CollectionAssignmentMapping));
                                this.ToggleViewModel.GridDynamicViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "QueueID", Header = "Q#" },
                                                                                             new DynamicColumn { ColumnName = "FollowUpDate",  Header = "Follow-Up", DataFormatString = "{0:g}" },
                                                                                             new DynamicColumn { ColumnName = "ContractId",  Header = "Contract" },
                                                                                         };
                                this.ToggleViewModel.GridDynamicViewModel.GridDataRows = collectionAssignmentMappings.ToList<object>();
                                this.ToggleViewModel.GridDynamicViewModel.LoadRadGridView();
                                this.ToggleViewModel.Header = "Assignment List";
                                this.ToggleViewModel.Screen = Screen;
                                collectionAssignmentViewModel.SelectedItemChanged = this.ToggleViewModel.SetSelectedItem;
                                firstSelectedItem = collectionAssignmentViewModel.SelectedQueue;
                            }
                        }

                        // change behavior form bar menu
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        FormBarMenuViewModel.FormMenuContent = "Assignments";

                        if (this._formBarCurrent != string.Empty)
                        {
                            FormBarMenuViewModel.FormBarContent = this._formBarCurrent;
                        }
                        else
                        {
                            if (FormBarMenuViewModel.FormBarContent == "Assignments")
                            {
                                FormBarMenuViewModel.FormBarContent = "Assignment Detail";
                            }
                        }
                        this._formBarCurrent = FormBarMenuViewModel.FormBarContent;

                        // Visible right hand grid
                        ChangedVisibility = Visibility.Visible;
                        break;
                    #endregion

                    #region collectionQueues
                    case EnumScreen.ColletionQueues:
                        this.FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        this.FormBarMenuViewModel.FormMenuContent = "Menu";
                        var viewModel = this.ScreenDetailViewModel as MainWindowDetailsTabControlCollectionQueueViewModel;

                        if (viewModel != null)
                        {
                            var list = viewModel.ListMainWindowDetailsVm.ScreenDetailViewModel as CollectionsManagementViewModel;
                            if (list != null)
                            {
                                if (list.AllQueueManagementDetails != null)
                                {
                                    // load data for right hand grid
                                    if (this.ToggleViewModel.GridDynamicViewModel == null || this.ToggleViewModel.Screen != Screen)
                                    {
                                        var collectionQueue =
                                            list.AllQueueManagementDetails.Select(x => x.CollectionQueue)
                                                .OrderBy(c => c.AssignmentOrder)
                                                .ToList();

                                        var collectionsManagementMappings = from d in collectionQueue
                                                                            select new CollectionsManagementMapping
                                                                           {
                                                                               Enabled = d.Enabled,
                                                                               ID = d.ID,
                                                                               QueueName = d.QueueName,
                                                                               AssignmentOrder = d.AssignmentOrder
                                                                           };

                                        // create column and data for dynamic grid
                                        this.ToggleViewModel.GridDynamicViewModel = null;
                                        this.ToggleViewModel.GridDynamicViewModel = new DynamicGridViewModel(typeof(CollectionsManagementMapping));
                                        this.ToggleViewModel.GridDynamicViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "Enabled", Header = "Enabled" },
                                                                                             new DynamicColumn { ColumnName = "AssignmentOrder", Header = "Order#" },
                                                                                             new DynamicColumn { ColumnName = "QueueName", Header = "Queue Name" },                                                                                            
                                                                                         };
                                        this.ToggleViewModel.GridDynamicViewModel.GridDataRows = collectionsManagementMappings.ToList<object>();
                                        this.ToggleViewModel.GridDynamicViewModel.LoadRadGridView();
                                        this.ToggleViewModel.Header = "Queue List";
                                        this.ToggleViewModel.Screen = Screen;
                                    }
                                    firstSelectedItem = list.SelectedQueue;
                                }
                            }
                        }

                        // change behavior form bar menu
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        FormBarMenuViewModel.FormMenuContent = "Queues";
                        if (this._formBarCurrent != string.Empty)
                        {
                            FormBarMenuViewModel.FormBarContent = this._formBarCurrent;
                        }
                        else
                        {
                            if (FormBarMenuViewModel.FormBarContent == "Collection Queues")
                            {
                                FormBarMenuViewModel.FormBarContent = "Collection Queue Detail";
                            }
                        }
                        this._formBarCurrent = FormBarMenuViewModel.FormBarContent;

                        // Visible right hand grid
                        ChangedVisibility = Visibility.Visible;
                        break;
                    #endregion

                    #region fundingSummary
                    case EnumScreen.FundingSummary:
                        var mainWindowDetailsViewModelFundingSummary = this.ScreenDetailViewModel as MainWindowDetailsViewModel;

                        if (mainWindowDetailsViewModelFundingSummary != null)
                        {
                            var fundingSummaryViewModel = mainWindowDetailsViewModelFundingSummary.ScreenDetailViewModel as FundingSummaryViewModel;
                            if (fundingSummaryViewModel != null)
                            {
                                if (fundingSummaryViewModel.TrancheSummary != null)
                                {
                                    // load data for right hand grid
                                    if (this.ToggleViewModel.GridDynamicViewModel == null || this.ToggleViewModel.Screen != Screen)
                                    {
                                        var fundingSummaryMappings = from d in fundingSummaryViewModel.TrancheSummary
                                                                     select new FundingSummaryMaping
                                                                            {
                                                                                TrancheId = d.TrancheId,
                                                                                FunderName = d.FunderName,
                                                                            };

                                        // create column and data for dynamic grid
                                        this.ToggleViewModel.GridDynamicViewModel = null;
                                        this.ToggleViewModel.GridDynamicViewModel = new DynamicGridViewModel(typeof(FundingSummaryMaping));
                                        this.ToggleViewModel.GridDynamicViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "FunderName",  Header = "Funder Name" },
                                                                                             new DynamicColumn { ColumnName = "TrancheId", Header = "Tranche ID" },
                                                                                         };
                                        this.ToggleViewModel.GridDynamicViewModel.GridDataRows = fundingSummaryMappings.ToList<object>();
                                        this.ToggleViewModel.GridDynamicViewModel.LoadRadGridView();
                                        this.ToggleViewModel.Header = "Tranche List";
                                        this.ToggleViewModel.Screen = Screen;

                                    }
                                    firstSelectedItem = fundingSummaryViewModel.SelectedTranche;
                                }
                            }
                        }

                        // change behavior form bar menu
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        FormBarMenuViewModel.FormMenuContent = "Tranches";
                        FormBarMenuViewModel.FormBarContent = "Funding Detail";
                        this._formBarCurrent = FormBarMenuViewModel.FormBarContent;

                        // Visible right hand grid
                        ChangedVisibility = Visibility.Visible;
                        break;
                    #endregion

                    #region asset classes category
                    case EnumScreen.AssetClassesCategory:
                        var mainDetailModel = this.ScreenDetailViewModel as MainAssetClassesViewModel;

                        if (mainDetailModel != null)
                        {
                            var assetClassesCategoryViewModel = mainDetailModel.AssetCategoryViewModel.ScreenDetailViewModel as AssetClassesCategoryViewModel;
                            if (assetClassesCategoryViewModel != null)
                            {
                                // load data for right hand grid
                                if (this.ToggleViewModel.GridDynamicViewModel == null
                                    || this.ToggleViewModel.Screen != Screen)
                                {
                                    // create column and data for dynamic grid
                                    this.ToggleViewModel.GridDynamicViewModel = null;
                                    this.ToggleViewModel.GridDynamicViewModel =
                                        new DynamicGridViewModel(typeof(AssetClassesCategoryRowItem));
                                    this.ToggleViewModel.GridDynamicViewModel.GridColumns = new List<DynamicColumn> 
                                                                                                { 
                                                                                                    new DynamicColumn { ColumnName="Category",Header="CATEGORY NAME", MinWidth = 90},
                                                                                                    new DynamicColumn { ColumnName="Enabled",Header="ENABLED", MinWidth = 80, ColumnTemplate = RadGridViewEnum.ColumnCheckedTemplate },
                                                                                                };
                                    this.ToggleViewModel.GridDynamicViewModel.GridDataRows =
                                        assetClassesCategoryViewModel.ListDataGridItem.ToList<object>();
                                    this.ToggleViewModel.GridDynamicViewModel.LoadRadGridView();
                                    this.ToggleViewModel.Header = "Asset Category List";
                                    this.ToggleViewModel.Screen = Screen;
                                }
                                firstSelectedItem = new EquipCategory
                                {
                                    EquipCatId = assetClassesCategoryViewModel.SelectedCategoryItem.EquipCategoryId
                                };
                            }
                        }

                        // change behavior form bar menu
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        FormBarMenuViewModel.FormMenuContent = "Asset Category";

                        if (this._formBarCurrent != string.Empty)
                        {
                            FormBarMenuViewModel.FormBarContent = this._formBarCurrent;
                        }
                        else
                        {
                            if (FormBarMenuViewModel.FormBarContent == "Asset Features")
                            {
                                FormBarMenuViewModel.FormBarContent = "Asset Features";
                            }
                        }
                        this._formBarCurrent = FormBarMenuViewModel.FormBarContent;

                        // Visible right hand grid
                        ChangedVisibility = Visibility.Visible;
                        break;
                    #endregion

                    #region asset classes type
                    case EnumScreen.AssetClassesType:
                        var mainDetailModelType = this.ScreenDetailViewModel as MainAssetClassesViewModel;

                        if (mainDetailModelType != null)
                        {
                            var assetClassesTypeViewModel = mainDetailModelType.AssetClassesTypeViewModel.ScreenDetailViewModel as AssetClassesTypeViewModel;
                            if (assetClassesTypeViewModel != null)
                            {
                                // load data for right hand grid
                                if (this.ToggleViewModel.GridDynamicViewModel == null
                                    || this.ToggleViewModel.Screen != Screen)
                                {
                                    // create column and data for dynamic grid
                                    this.ToggleViewModel.GridDynamicViewModel = null;
                                    this.ToggleViewModel.GridDynamicViewModel =
                                        new DynamicGridViewModel(typeof(AssetClassesTypeRowItem));
                                    this.ToggleViewModel.GridDynamicViewModel.GridColumns = new List<DynamicColumn> 
                                                                                                { 
                                                                                                    new DynamicColumn { ColumnName="TypeDescription",Header="TYPE", MinWidth = 65 },
                                                                                                    new DynamicColumn { ColumnName="Enabled",Header="ENABLED", MinWidth= 80, ColumnTemplate = RadGridViewEnum.ColumnCheckedTemplate },
                                                                                                };
                                    this.ToggleViewModel.GridDynamicViewModel.GridDataRows = assetClassesTypeViewModel.ListDataGridItem.ToList<object>();
                                    this.ToggleViewModel.GridDynamicViewModel.LoadRadGridView();
                                    this.ToggleViewModel.Header = "Asset Type List";
                                    this.ToggleViewModel.Screen = Screen;
                                }
                                firstSelectedItem = new EquipType
                                {
                                    EquipTypeId = assetClassesTypeViewModel.SelectedTypeItem.EquipTypeId
                                };
                            }
                        }

                        // change behavior form bar menu
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        FormBarMenuViewModel.FormMenuContent = "Asset Type";

                        if (this._formBarCurrent != string.Empty)
                        {
                            FormBarMenuViewModel.FormBarContent = this._formBarCurrent;
                        }
                        else
                        {
                            if (FormBarMenuViewModel.FormBarContent == "Asset Type")
                            {
                                FormBarMenuViewModel.FormBarContent = "Asset Type";
                            }
                        }
                        this._formBarCurrent = FormBarMenuViewModel.FormBarContent;

                        // Visible right hand grid
                        ChangedVisibility = Visibility.Visible;
                        break;
                    #endregion

                    #region asset feature
                    case EnumScreen.AssetFeatures:
                        var model = this.ScreenDetailViewModel as MainWindowDetailsViewModel;

                        if (model != null)
                        {
                            var assetFeaturesViewModel = model.ScreenDetailViewModel as AssetFeaturesViewModel;
                            if (assetFeaturesViewModel != null)
                            {
                                // load data for right hand grid
                                if (this.ToggleViewModel.GridDynamicViewModel == null
                                    || this.ToggleViewModel.Screen != Screen)
                                {
                                    var assetFeaturesMappings = from d in assetFeaturesViewModel.AllFeatureTypes
                                                                select
                                                                    new AssetFeatureTypeRowItem
                                                                        {
                                                                            FeatureTypeId = d.FeatureTypeId,
                                                                            FeatureName = d.FeatureName,
                                                                            Enabled = d.Enabled,
                                                                        };

                                    // create column and data for dynamic grid
                                    this.ToggleViewModel.GridDynamicViewModel = null;
                                    this.ToggleViewModel.GridDynamicViewModel =
                                        new DynamicGridViewModel(typeof(AssetFeatureTypeRowItem));
                                    this.ToggleViewModel.GridDynamicViewModel.GridColumns = new List<DynamicColumn> 
                                                                                                { 
                                                                                                    new DynamicColumn { ColumnName = "FeatureName", Header = "FEATURE NAME", MinWidth= 80 },
                                                                                                    new DynamicColumn { ColumnName = "Enabled", Header ="ENABLED", MinWidth = 80, ColumnTemplate = RadGridViewEnum.ColumnCheckedTemplate },
                                                                                                };
                                    this.ToggleViewModel.GridDynamicViewModel.GridDataRows =
                                        assetFeaturesMappings.ToList<object>();
                                    this.ToggleViewModel.GridDynamicViewModel.LoadRadGridView();
                                    this.ToggleViewModel.Header = "Feature List";
                                    this.ToggleViewModel.Screen = Screen;
                                }
                                firstSelectedItem = assetFeaturesViewModel.SelectedFeatureType;
                            }
                        }

                        // change behavior form bar menu
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        FormBarMenuViewModel.FormMenuContent = "Asset Features";

                        if (this._formBarCurrent != string.Empty)
                        {
                            FormBarMenuViewModel.FormBarContent = this._formBarCurrent;
                        }
                        else
                        {
                            if (FormBarMenuViewModel.FormBarContent == "Asset Features")
                            {
                                FormBarMenuViewModel.FormBarContent = "Asset Features";
                            }
                        }
                        this._formBarCurrent = FormBarMenuViewModel.FormBarContent;

                        // Visible right hand grid
                        ChangedVisibility = Visibility.Visible;
                        break;
                    #endregion

                    #region Asset Classes Model
                    case EnumScreen.AssetClassesModel:
                        var mainWindowDetailsModel = this.ScreenDetailViewModel as MainAssetClassesViewModel;
                        if (mainWindowDetailsModel != null)
                        {
                            var assetClassesModelViewModel =
                                mainWindowDetailsModel.AssetClassesModelViewModel.ScreenDetailViewModel as
                                    AssetClassesModelViewModel;
                            if (assetClassesModelViewModel != null)
                            {
                                if ((this.ToggleViewModel.GridDynamicViewModel == null) ||
                                    (this.ToggleViewModel.Screen != Screen))
                                {
                                    var assetModelMappings = from d in assetClassesModelViewModel.AllAssetModel
                                                             select new AssetClassesModelRowItem
                                                             {
                                                                 EquipModelId = d.EquipModelId,
                                                                 Description = d.Description,
                                                                 Enabled = d.Enabled
                                                             };
                                    this.ToggleViewModel.GridDynamicViewModel = null;
                                    this.ToggleViewModel.GridDynamicViewModel = new DynamicGridViewModel(typeof(AssetClassesModelRowItem));
                                    this.ToggleViewModel.GridDynamicViewModel.GridColumns = new List<DynamicColumn>
                                    {
                                        new DynamicColumn
                                        {
                                            ColumnName = "Description", Header = "MODEL", MinWidth = 70
                                        },
                                        new DynamicColumn
                                        {
                                            ColumnName = "Enabled", Header = "ENABLED", MinWidth = 80, ColumnTemplate = RadGridViewEnum.ColumnCheckedTemplate
                                        }
                                    };
                                    this.ToggleViewModel.GridDynamicViewModel.GridDataRows =
                                        assetClassesModelViewModel.AllAssetModel.ToList<object>();
                                    this.ToggleViewModel.GridDynamicViewModel.LoadRadGridView();
                                    this.ToggleViewModel.Header = "Model List";
                                    this.ToggleViewModel.Screen = Screen;
                                }
                                //firstSelectedItem = assetClassesModelViewModel.SelectedModel;
                                firstSelectedItem = new EquipModel()
                                {
                                    EquipModelId = assetClassesModelViewModel.SelectedModel.EquipModelId,
                                };
                            }
                        }

                        // change behavior form bar menu
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        FormBarMenuViewModel.FormMenuContent = "Asset Model";

                        if (this._formBarCurrent != string.Empty)
                        {
                            FormBarMenuViewModel.FormBarContent = this._formBarCurrent;
                        }
                        else
                        {
                            if (FormBarMenuViewModel.FormBarContent == "Asset Model")
                            {
                                FormBarMenuViewModel.FormBarContent = "Asset Model";
                            }
                        }
                        this._formBarCurrent = FormBarMenuViewModel.FormBarContent;

                        // Visible right hand grid
                        ChangedVisibility = Visibility.Visible;

                        break;
                    #endregion

                    #region asset classes make type

                    case EnumScreen.AssetClassesMake:
                        var mainDetailModelAsset = this.ScreenDetailViewModel as MainAssetClassesViewModel;

                        if (mainDetailModelAsset != null)
                        {
                            var assetClassesMakeViewModel = mainDetailModelAsset.AssetClassesMakeViewModel.ScreenDetailViewModel as AssetClassesMakeViewModel;
                            if (assetClassesMakeViewModel != null)
                            {
                                // load data for right hand grid
                                if (this.ToggleViewModel.GridDynamicViewModel == null
                                    || this.ToggleViewModel.Screen != Screen)
                                {
                                    //var assetClassesCategoryMappings = from itemCategory in (assetClassesCategoryViewModel.CategoryDetailViewModel.DynamicAssetClassCategoryViewModel.GridDataRows)
                                    //                            select
                                    //                                new AssetClassesCategoryRowItem
                                    //                                {
                                    //                                    Category = (itemCategory as AssetClassesCategoryRowItem).Category,
                                    //                                    Enabled = (itemCategory as AssetClassesCategoryRowItem).Enabled,
                                    //                                };

                                    // create column and data for dynamic grid
                                    this.ToggleViewModel.GridDynamicViewModel = null;
                                    this.ToggleViewModel.GridDynamicViewModel =
                                        new DynamicGridViewModel(typeof(AssetClassesMakeRowItem));
                                    this.ToggleViewModel.GridDynamicViewModel.GridColumns = new List<DynamicColumn> 
                                                                                                { 
                                                                                                     new DynamicColumn { ColumnName = "Description", Header = "MAKE", MinWidth = 70 },
                                                                                                     new DynamicColumn { ColumnName = "Enabled",  Header = "ENABLED", MinWidth = 80, ColumnTemplate = RadGridViewEnum.ColumnCheckedTemplate },
                                                                                                };
                                    this.ToggleViewModel.GridDynamicViewModel.GridDataRows =
                                        assetClassesMakeViewModel.AllAssetMake.ToList<object>();
                                    this.ToggleViewModel.GridDynamicViewModel.LoadRadGridView();
                                    this.ToggleViewModel.Header = "Make List";
                                    this.ToggleViewModel.Screen = Screen;
                                }
                                //firstSelectedItem = assetClassesMakeViewModel.DynamicAssetClassMakeViewModel.SelectedItem;
                                firstSelectedItem = new EquipMake()
                                {
                                    EquipMakeId = assetClassesMakeViewModel.SelectedMake.EquipMakeId
                                };
                            }
                        }

                        // change behavior form bar menu
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        FormBarMenuViewModel.FormMenuContent = "Asset Make";

                        if (this._formBarCurrent != string.Empty)
                        {
                            FormBarMenuViewModel.FormBarContent = this._formBarCurrent;
                        }
                        else
                        {
                            if (FormBarMenuViewModel.FormBarContent == "Asset Make")
                            {
                                FormBarMenuViewModel.FormBarContent = "Asset Make";
                            }
                        }
                        this._formBarCurrent = FormBarMenuViewModel.FormBarContent;

                        // Visible right hand grid
                        if (currentStep == EnumSteps.AssignModel)
                        {
                            ChangedVisibility = Visibility.Collapsed;
                        }
                        else
                        {
                            ChangedVisibility = Visibility.Visible;
                        }
                        break;

                    #endregion

                    #region asset collateral classes
                    case EnumScreen.AssetCollateralClasses:
                        var viewmodel = this.ScreenDetailViewModel as MainWindowDetailsViewModel;

                        if (viewmodel != null)
                        {
                            var assetCollateralViewModel = viewmodel.ScreenDetailViewModel as AssetCollateralClassesViewModel;
                            if (assetCollateralViewModel != null)
                            {
                                // load data for right hand grid
                                if (this.ToggleViewModel.GridDynamicViewModel == null
                                    || this.ToggleViewModel.Screen != Screen)
                                {
                                    var assetFeaturesMappings = assetCollateralViewModel.AllCollateralClasses;

                                    // create column and data for dynamic grid
                                    this.ToggleViewModel.GridDynamicViewModel = null;
                                    this.ToggleViewModel.GridDynamicViewModel =
                                        new DynamicGridViewModel(typeof(AssetCollateralRowItem));
                                    this.ToggleViewModel.GridDynamicViewModel.GridColumns = new List<DynamicColumn>
                                                                                                {
                                                                                                    new DynamicColumn
                                                                                                        {
                                                                                                            ColumnName = "Description", Header = "COLLATERAL CLASS", MinWidth= 95
                                                                                                        },
                                                                                                };
                                    this.ToggleViewModel.GridDynamicViewModel.GridDataRows =
                                        assetFeaturesMappings.ToList<object>();
                                    this.ToggleViewModel.GridDynamicViewModel.LoadRadGridView();
                                    this.ToggleViewModel.Header = "Collateral Class List";
                                    this.ToggleViewModel.Screen = Screen;
                                }
                                firstSelectedItem = assetCollateralViewModel.SelectedCollateral;
                            }
                        }

                        // change behavior form bar menu
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        FormBarMenuViewModel.FormMenuContent = "Asset Collateral Classes";

                        if (this._formBarCurrent != string.Empty)
                        {
                            FormBarMenuViewModel.FormBarContent = this._formBarCurrent;
                        }
                        else
                        {
                            if (FormBarMenuViewModel.FormBarContent == "Asset Collateral Classes")
                            {
                                FormBarMenuViewModel.FormBarContent = "Asset Collateral Classes";
                            }
                        }
                        this._formBarCurrent = FormBarMenuViewModel.FormBarContent;

                        // Visible right hand grid
                        ChangedVisibility = Visibility.Visible;
                        break;
                    #endregion

                    #region registers
                    case EnumScreen.AssetRegisters:
                        var mainWindowDetailsViewModelRegister = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                        if (mainWindowDetailsViewModelRegister != null)
                        {
                            var registersViewModel = mainWindowDetailsViewModelRegister.ScreenDetailViewModel as AssetRegistersViewModel;
                            if (registersViewModel != null)
                            {
                                    // load data for right hand grid
                                    if (this.ToggleViewModel.GridDynamicViewModel == null || this.ToggleViewModel.Screen != Screen)
                                    {
                                        var assetRegistersMappings = from d in registersViewModel.AllAssetRegister
                                                                     select
                                                                         new AssetRegisterRowItem
                                                                         {
                                                                             ID = d.ID,
                                                                             RegisterName = d.RegisterName,
                                                                             ReportName = d.ReportName,
                                                                             InternalOnly = d.InternalOnly
                                                                         };

                                        // create column and data for dynamic grid
                                        this.ToggleViewModel.GridDynamicViewModel = null;
                                        this.ToggleViewModel.GridDynamicViewModel = new DynamicGridViewModel(typeof(AssetRegisterRowItem));
                                        this.ToggleViewModel.GridDynamicViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "RegisterName", Header = "REGISTER NAME", MinWidth = 90 },
                                                                                             new DynamicColumn { ColumnName = "InternalOnly",  Header = "INTERNAL ONLY", MinWidth = 90, ColumnTemplate = RadGridViewEnum.ColumnCheckedTemplate },
                                                                                         };
                                        this.ToggleViewModel.GridDynamicViewModel.GridDataRows = assetRegistersMappings.ToList<object>();
                                        this.ToggleViewModel.GridDynamicViewModel.LoadRadGridView();
                                        this.ToggleViewModel.Header = "Asset Register";
                                        this.ToggleViewModel.Screen = Screen;
                                    }
                                    firstSelectedItem = new AssetRegister()
                                    {
                                        ID = registersViewModel.SelectedRegister.ID
                                    };
                                    //firstSelectedItem = registersViewModel.SelectedRegister;
                            }
                        }

                        // change behavior form bar menu
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        FormBarMenuViewModel.FormMenuContent = "Asset Register";
                        if (this._formBarCurrent != string.Empty)
                        {
                            FormBarMenuViewModel.FormBarContent = this._formBarCurrent;
                        }
                        else
                        {
                            if (FormBarMenuViewModel.FormBarContent == "Asset Registers")
                            {
                                FormBarMenuViewModel.FormBarContent = "Asset Registers Detail";
                            }
                        }

                        this._formBarCurrent = FormBarMenuViewModel.FormBarContent;

                        // Visible right hand grid
                        ChangedVisibility = Visibility.Visible;
                        break;
                    #endregion

                    #region registeredAsset
                    case EnumScreen.RegisteredAsset:
                        var mainWindowDetailsViewModelRegisteredAsset = this.ScreenDetailViewModel as MainWindowDetailsViewModel;
                        if (mainWindowDetailsViewModelRegisteredAsset != null)
                        {
                            var registersViewModel = mainWindowDetailsViewModelRegisteredAsset.ScreenDetailViewModel as RegisteredAssetViewModel;
                            if (registersViewModel != null)
                            {
                                // load data for right hand grid
                                if (this.ToggleViewModel.GridDynamicViewModel == null || this.ToggleViewModel.Screen != Screen)
                                {
                                    var assetRegistersMappings = from d in registersViewModel.AllRegisteredAsset
                                                                 select
                                                                     new RegisteredAssetRowItem()
                                                                     {
                                                                         Id = d.Id,
                                                                         AssetRegisterId = d.AssetRegisterId,
                                                                         AssetState = d.AssetState,
                                                                     };

                                    // create column and data for dynamic grid
                                    this.ToggleViewModel.GridDynamicViewModel = null;
                                    this.ToggleViewModel.GridDynamicViewModel = new DynamicGridViewModel(typeof(RegisteredAssetRowItem));
                                    this.ToggleViewModel.GridDynamicViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "Id", Header = "Asset ID" },
                                                                                             new DynamicColumn { ColumnName = "AssetRegisterId",  Header = "Reg#" },
                                                                                             new DynamicColumn { ColumnName = "AssetState", Header = "Asset State"},
                                                                                         };
                                    this.ToggleViewModel.GridDynamicViewModel.GridDataRows = assetRegistersMappings.ToList<object>();
                                    this.ToggleViewModel.GridDynamicViewModel.LoadRadGridView();
                                    this.ToggleViewModel.Header = "Assets List";
                                    this.ToggleViewModel.Screen = Screen;
                                }
                                firstSelectedItem = new RegisteredAsset
                                {
                                    ID = registersViewModel.SelectedRegistererdAsset.Id
                                };
                                //firstSelectedItem = registersViewModel.SelectedRegister;
                            }
                        }

                        // change behavior form bar menu
                        FormBarMenuViewModel.ChangedVisibility = Visibility.Visible;
                        FormBarMenuViewModel.FormMenuContent = "Assets";
                        if (this._formBarCurrent != string.Empty)
                        {
                            FormBarMenuViewModel.FormBarContent = this._formBarCurrent;
                        }
                        else
                        {
                            if (FormBarMenuViewModel.FormBarContent == "Assets")
                            {
                                FormBarMenuViewModel.FormBarContent = "Asset Detail";
                            }
                        }

                        this._formBarCurrent = FormBarMenuViewModel.FormBarContent;

                        // Visible right hand grid
                        ChangedVisibility = Visibility.Visible;
                        break;
                    #endregion
                }
            }

            // To call some Actions or Events of Toggle ViewModel 
            if (ToggleViewModel != null)
            {
                ToggleViewModel.OnSelectedItemChange = ToggleViewModel_SelectedItemChanged;
                ToggleViewModel.RaiseSelectedItemChanged();
                if (firstSelectedItem != null)
                {
                    ToggleViewModel.SetSelectedItem(firstSelectedItem);
                }
            }
        }

        // Set selected item in toggle
        /*
                private void SetSelectedItemForToggle(EnumScreen screen, Nullable<int> id)
                {
                    switch (screen)
                    {
                        case EnumScreen.Users:
                             SelectedItem = ToggleViewModel.GridCustomers.GridDataRows.Cast<UserMapping>()
                                    .FirstOrDefault(x => x.UserEntityId == id);
                            ToggleViewModel.GridCustomers.SelectedItem = SelectedItem;
                            break;
                        case EnumScreen.Groups:
                            SelectedItem = ToggleViewModel.GridCustomers.GridDataRows.Cast<GroupMapping>()
                                   .FirstOrDefault(x => x.UserEntityId == id);
                            ToggleViewModel.GridCustomers.SelectedItem = SelectedItem;
                            break;
                    }
                }
        */

        #endregion

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
                if (this._allUsers != null)
                {
                    this._allUsers.Clear();
                    this._allUsers = null;
                }
                if (this._allFundingSummary != null)
                {
                    this._allFundingSummary.Clear();
                    this._allFundingSummary = null;
                }
                if (this._groups != null)
                {
                    this._groups.Clear();
                    this._groups = null;
                }
                if (this._allQueueManagementDetails != null)
                {
                    this._allQueueManagementDetails.Clear();
                    this._allQueueManagementDetails = null;
                }
                if (this._screenDetailViewModel != null)
                {
                    this._screenDetailViewModel.Dispose();
                    this._screenDetailViewModel = null;
                }
                if (this._mainContent != null)
                {
                    this._mainContent.Clear();
                    this._mainContent = null;
                }
                if (this._formBarMenuViewModel != null)
                {
                    this._formBarMenuViewModel.Dispose();
                    this._formBarMenuViewModel = null;
                }
                if (this._toggleViewModel != null)
                {
                    this._toggleViewModel.Dispose();
                    this._toggleViewModel = null;
                }

                base.Dispose();
            }));
        }
    }
}
