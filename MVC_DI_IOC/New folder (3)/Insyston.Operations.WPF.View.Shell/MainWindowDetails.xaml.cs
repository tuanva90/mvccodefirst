// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowDetails.xaml.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for MainWindowDetails.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.Views.Shell
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using FirstFloor.ModernUI.Windows.Navigation;

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Security.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Security;
    using Insyston.Operations.WPF.ViewModels.Shell;

    /// <summary>
    /// Interaction logic for MainWindowDetails.xaml
    /// </summary>
    public partial class MainWindowDetails
    {
        /// <summary>
        /// The URL content.
        /// </summary>
        private string UrlContent { get; set; }

        /// <summary>
        /// The _view model.
        /// </summary>
        private MainWindowDetailsViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowDetails"/> class.
        /// </summary>
        public MainWindowDetails()
        {
            InitializeComponent();
            this.Loaded += MainWindowDetails_Loaded;
        }

        /// <summary>
        /// The main window details_ loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void MainWindowDetails_Loaded(object sender, RoutedEventArgs e)
        {
            this._viewModel = this.DataContext as MainWindowDetailsViewModel;
            if (this._viewModel != null)
            {
                this._viewModel.OnNewModule -= this.MainWindow_OnNewModule;
                this._viewModel.OnNewModule += this.MainWindow_OnNewModule;
                this._viewModel.LoadModule();              
            }
        }

        /// <summary>
        /// The main window_ on new module.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        private void MainWindow_OnNewModule(object module)
        {
            EnumScreen currentModule = (EnumScreen)module;

            // Check for content don't load again.
            if (FrameContent.Source != null)
            {
                return;
            }
            this._viewModel = this.DataContext as MainWindowDetailsViewModel;

            if (this._viewModel != null)
            {
                // navigate to the module specified.
                switch (currentModule)
                {
                    case EnumScreen.Home:
                        UrlContent = "/Insyston.Operations.WPF.Views.Shell;component/HomePageView.xaml";
                        break;
                    case EnumScreen.Users:
                        Permission permissionUser = Operations.Security.Authorisation.GetPermission(Components.Security, Forms.Users);

                        if (permissionUser.CanSee)
                        {
                            UrlContent = "/Insyston.Operations.WPF.Views.Security;component/UsersView.xaml";
                        }
                        else
                        {
                            UrlContent = "None";
                            this._viewModel.PermissionText = "Security Permissions are required to access this area";
                        }
                        break;
                    case EnumScreen.Groups:
                        Permission permissionGroup = Operations.Security.Authorisation.GetPermission(Components.Security, Forms.Groups);

                        if (permissionGroup.CanSee)
                        {
                            UrlContent = "/Insyston.Operations.WPF.Views.Security;component/GroupsView.xaml";
                        }
                        else
                        {
                            UrlContent = "None";
                            this._viewModel.PermissionText = "Security Permissions are required to access this area";
                        }
                        break;
                    case EnumScreen.Configuration:
                        UrlContent = "/Insyston.Operations.WPF.Views.Shell;component/ConfigurationView.xaml";
                        break;
                    case EnumScreen.CollectionSettings:
                        UrlContent = "/Insyston.Operations.WPF.Views.Collections;component/CollectionsSettingView.xaml";
                        break;
                    case EnumScreen.CollectionAssignment:
                        Permission permissionAssignment = Operations.Security.Authorisation.GetPermission(Components.Collections, Forms.CollectionsQueueAssignment);
                        if (permissionAssignment.CanSee)
                        {
                            UrlContent = "/Insyston.Operations.WPF.Views.Collections;component/CollectionsAssignmentView.xaml";
                        }
                        else
                        {
                            UrlContent = "None";
                            this._viewModel.PermissionText = "Security Permissions are required to access this area";
                        }
                        break;
                    case EnumScreen.SecuritySetting:
                        UrlContent = "/Insyston.Operations.WPF.Views.Security;component/SystemSettingView.xaml";
                        break;
                    case EnumScreen.ColletionQueues:
                        UrlContent = "/Insyston.Operations.WPF.Views.Collections;component/CollectionsManagementView.xaml";
                        break;
                    case EnumScreen.Membership:
                        Permission permissionMembership = Operations.Security.Authorisation.GetPermission(Components.Security, Forms.Membership);

                        if (permissionMembership.CanSee)
                        {
                            UrlContent = "/Insyston.Operations.WPF.Views.Security;component/MembershipView.xaml";
                        }
                        else
                        {
                            UrlContent = "None";
                            this._viewModel.PermissionText = "Security Permissions are required to access this area";
                        }
                        break;
                    case EnumScreen.FundingSummary:
                        Permission permissionFunding = Operations.Security.Authorisation.GetPermission(Components.Funding, Forms.Funding);
                        if (permissionFunding.CanSee)
                        {
                            UrlContent = "/Insyston.Operations.WPF.Views.Funding;component/FundingSummaryView.xaml";
                        }
                        else
                        {
                            UrlContent = "None";
                            this._viewModel.PermissionText = "Security Permissions are required to access this area";
                        }
                        break;
                    case EnumScreen.Collectors:
                        UrlContent = "/Insyston.Operations.WPF.Views.Collections;component/CollectorsView.xaml";
                        break;
                    case EnumScreen.AssetClassesCategory:
                        UrlContent = "/Insyston.Operations.WPF.Views.Assets;component/AssetClasses/AssetClassesCategoryView.xaml";
                        break;
                    case EnumScreen.AssetClassesType:
                        UrlContent = "/Insyston.Operations.WPF.Views.Assets;component/AssetClasses/AssetClassesTypeView.xaml";
                        break;
                    case EnumScreen.AssetClassesMake:
                        UrlContent = "/Insyston.Operations.WPF.Views.Assets;component/AssetClasses/AssetClassesMakeView.xaml";
                        break;
                    case EnumScreen.AssetClassesModel:
                        UrlContent = "/Insyston.Operations.WPF.Views.Assets;component/AssetClasses/AssetClassesModelView.xaml";
                        break;
                    case EnumScreen.AssetCollateralClasses:
                        UrlContent = "/Insyston.Operations.WPF.Views.Assets;component/AssetCollateralClassesView.xaml";
                        break;
                    case EnumScreen.AssetFeatures:
                        UrlContent = "/Insyston.Operations.WPF.Views.Assets;component/AssetFeaturesView.xaml";
                        break;
                    case EnumScreen.AssetSettings:
                        UrlContent = "/Insyston.Operations.WPF.Views.Assets;component/AssetSettingsView.xaml";
                        break;
                    case EnumScreen.AssetRegisters:
                        UrlContent = "/Insyston.Operations.WPF.Views.Assets;component/AssetRegistersView.xaml";
                        break;
                    case EnumScreen.RegisteredAsset:
                        Permission permissionRegisteredAsset =
                            Operations.Security.Authorisation.GetPermission(
                                Components.SystemManagementRegisterdAsset,
                                Forms.RegisteredAssetDetail);
                        if (permissionRegisteredAsset.CanSee)
                        {
                            UrlContent =
                                "/Insyston.Operations.WPF.Views.RegisteredAsset;component/RegisteredAssetView.xaml";
                        }
                        else
                        {
                            UrlContent = "None";
                            this._viewModel.PermissionText = "Security Permissions are required to access this area";
                        }
                        break;
                }
                FrameContent.Source = new Uri(UrlContent, UriKind.Relative);
            }
        }

        private void FrameContent_OnNavigated(object sender, NavigationEventArgs e)
        {
            FrameContent.Visibility = System.Windows.Visibility.Visible;

            this._viewModel = this.DataContext as MainWindowDetailsViewModel;
            var windowDetailsViewModel = this._viewModel;
            if (windowDetailsViewModel != null)
            {
                UserControl content = this.FrameContent.Content as UserControl;
                if (content != null)
                {
                    var mainWindowDetailsViewModel = windowDetailsViewModel;
                    {
                        mainWindowDetailsViewModel.ScreenDetailViewModel = content.DataContext as ViewModelUseCaseBase;
                        Permission permissionUser;
                        var s = content.ToString();
                        if (s != null && s.IndexOf("UsersView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(Components.Security, Forms.Users);
                            if (!permissionUser.CanSee)
                            {
                                this.FrameContent.Visibility = System.Windows.Visibility.Collapsed;
                                this.PermissionContent.Visibility = Visibility.Visible;
                            }
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                if (permissionUser != null)
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                                }
                                else
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                                }
                            }
                        }
                        if (s != null && s.IndexOf("GroupsView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(
                                Components.Security,
                                Forms.Groups);
                            if (!permissionUser.CanSee)
                            {
                                this.FrameContent.Visibility = System.Windows.Visibility.Collapsed;
                                this.PermissionContent.Visibility = Visibility.Visible;
                            }
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                if (permissionUser != null)
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                                }
                                else
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                                }
                            }
                        }
                        if (s != null && s.IndexOf("ConfigurationView", System.StringComparison.Ordinal) != -1)
                        {
                            Permission permissionSecuritySetting = Operations.Security.Authorisation.GetPermission(Components.SystemManagementSecuritySettings, Forms.SecuritySettings);
                            Permission permissionQueuesSetting = Operations.Security.Authorisation.GetPermission(Components.SystemManagementCollectionSettings, Forms.CollectionSettings);
                            Permission permissionQueuesManagerment = Operations.Security.Authorisation.GetPermission(Components.SystemManagementCollectionQueues, Forms.QueueDetail);
                            Permission permissionAssetSetting = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetSettings, Forms.AssetSetting);
                        
                            // Check Permission for Asset Classes module
                            int resultAssetClassPermission = 0;

                            // Check permission for Asset Classes Category
                            Permission assetClassesCategoryDetail = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesCategory, Forms.AssetClassesCategoryDetail);
                            Permission assetClassesCategoryFeature = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesCategory, Forms.AssetClassesCategoryFeatures);
                            Permission assetClassesCategoryType = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesCategory, Forms.AssetClassesCategoryTypes);
                            if (!assetClassesCategoryDetail.CanSee && !assetClassesCategoryFeature.CanSee && !assetClassesCategoryType.CanSee)
                            {
                                resultAssetClassPermission += 1;
                            }

                            // Check permission for Asset Classes Type
                            Permission assetClassesTypeDetail = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesType, Forms.AssetClassesTypeDetail);
                            Permission assetClassesTypeFeature = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesType, Forms.AssetClassesTypeFeatures);
                            Permission assetClassesTypeMake = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesType, Forms.AssetClassesTypeMake);
                            if (!assetClassesTypeDetail.CanSee && !assetClassesTypeFeature.CanSee && !assetClassesTypeMake.CanSee)
                            {
                                resultAssetClassPermission += 1;
                            }

                            // Check permission for Asset Classes Make
                            Permission assetClassesMake = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesMake, Forms.AssetClassesMakeDetail);
                            if (!assetClassesMake.CanSee)
                            {
                                resultAssetClassPermission += 1;
                            }

                            // Check permission for Asset Classes Model
                            Permission assetClassesModel = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesModel, Forms.AssetClassesModelDetail);
                            if (!assetClassesModel.CanSee)
                            {
                                resultAssetClassPermission += 1;
                            }

                            // Check permission for Asset Collateral
                            bool resultAssetCollateralPermission = false;
                            Permission permissionCollateralDetail = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetCollateralClasses, Forms.CollateralClassesDetail);
                            Permission permissionCollateralType = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetCollateralClasses, Forms.CollateralClassesType);
                            if (permissionCollateralDetail.CanSee || permissionCollateralType.CanSee)
                            {
                                resultAssetCollateralPermission = true;
                            }

                            // Check permission for Asset Feature 
                            bool resultAssetFeaturePermission = false;
                            Permission permissionAssetFeatureDetail = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetFeatures, Forms.FeaturesDetail);
                            Permission permissionAssetFeatureAssignTo = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetFeatures, Forms.FeaturesAssignTo);
                            if (permissionAssetFeatureDetail.CanSee || permissionAssetFeatureAssignTo.CanSee)
                            {
                                resultAssetFeaturePermission = true;
                            }

                            // Check permission for Asset Register
                            Permission permissionRegister = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetRegister, Forms.RegisterDetail);

                            var configurationViewModel =
                                mainWindowDetailsViewModel.ScreenDetailViewModel as ConfigurationViewModel;
                            if (configurationViewModel != null)
                            {
                                configurationViewModel.GetHyperlinkWithPermission(permissionQueuesSetting, permissionQueuesManagerment, permissionSecuritySetting, resultAssetClassPermission == 4 ? false : true, resultAssetCollateralPermission, resultAssetFeaturePermission, permissionRegister, permissionAssetSetting);
                            }
                        }
                        if (s != null && s.IndexOf("CollectionsSettingView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(
                                Components.SystemManagementCollectionSettings, Forms.CollectionSettings);
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                if (permissionUser != null)
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                                }
                                else
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                                }
                            }
                        }
                        if (s != null && s.IndexOf("SystemSettingView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(
                                Components.SystemManagementSecuritySettings, Forms.SecuritySettings);
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                if (permissionUser != null)
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                                }
                                else
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                                }
                            }
                        }
                        if (s != null && s.IndexOf("CollectionsManagementView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(
                                Components.SystemManagementCollectionQueues, Forms.QueueDetail);
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                if (permissionUser != null)
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                                }
                                else
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                                }
                            }
                        }
                        if (s != null && s.IndexOf("CollectionsAssignmentView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(
                                Components.Collections,
                                Forms.CollectionsQueueAssignment);
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                if (permissionUser != null)
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                                }
                                else
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                                }
                            }
                        }

                        if (s != null && s.IndexOf("FundingSummaryView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(
                                Components.Funding,
                                Forms.Funding);
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                if (permissionUser != null)
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                                }
                                else
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                                }
                            }
                        }

                        if (s != null && s.IndexOf("MembershipView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(
                                Components.Security,
                                Forms.Membership);
                            if (!permissionUser.CanSee)
                            {
                                this.FrameContent.Visibility = System.Windows.Visibility.Collapsed;
                                this.PermissionContent.Visibility = Visibility.Visible;
                            }
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                if (permissionUser != null)
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                                }
                                else
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                                }
                            }
                        }

                        if (s != null && s.IndexOf("CollectorsView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(Components.SystemManagementCollectionQueues, Forms.Collectors);
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                if (permissionUser != null)
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                                }
                                else
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                                }
                            }
                        }

                        if (s != null && s.IndexOf("AssetClassesCategoryView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesCategory, Forms.AssetClassesCategoryDetail);
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                if (permissionUser != null)
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                                }
                                else
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                                }
                            }
                        }

                        if (s != null && s.IndexOf("AssetClassesTypeView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesType, Forms.AssetClassesTypeDetail);
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                if (permissionUser != null)
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                                }
                                else
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                                }
                            }
                        }

                        if (s != null && s.IndexOf("AssetClassesMakeView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesMake, Forms.AssetClassesMakeDetail);
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                if (permissionUser != null)
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                                }
                                else
                                {
                                    mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                                }
                            }
                        }

                        if (s != null && s.IndexOf("AssetClassesModelView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesModel, Forms.AssetClassesModelDetail);
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                            }
                            else
                            {
                                mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                            }
                        }

                        if (s != null && s.IndexOf("AssetFeaturesView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetFeatures, Forms.FeaturesDetail);
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                            }
                            else
                            {
                                mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                            }
                        }

                        if (s != null && s.IndexOf("AssetCollateralClassesView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetCollateralClasses, Forms.CollateralClassesDetail);
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                            }
                            else
                            {
                                mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                            }
                        }

                        if (s != null && s.IndexOf("AssetSettingsView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetSettings, Forms.AssetSetting);
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                            }
                        }

                        if (s != null && s.IndexOf("AssetRegistersView", System.StringComparison.Ordinal) != -1)
                        {
                            permissionUser = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetRegister, Forms.RegisterDetail);
                            if (mainWindowDetailsViewModel.ScreenDetailViewModel != null)
                            {
                                mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = permissionUser.CanEdit;
                            }
                            else
                            {
                                mainWindowDetailsViewModel.ScreenDetailViewModel.CanEdit = false;
                            }
                        }

                        this.ActionMenuItemsControl.DataContext = content.DataContext;
                    }
                    windowDetailsViewModel.HyperLinkItemClick();
                    windowDetailsViewModel.OnRaiseStepChanged();
                    windowDetailsViewModel.OnErrorHyperlinkSelect();
                    windowDetailsViewModel.OnValidateNotError();
                    windowDetailsViewModel.ConfigItemClick();
                    windowDetailsViewModel.OnCancelNewItem();

                    if (windowDetailsViewModel.NavigatedToScreen != null)
                    {
                        windowDetailsViewModel.NavigatedToScreen(windowDetailsViewModel);
                    }
                }                
            }
        }

        private void FrameContent_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            FrameContent.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
