// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The main view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.Views.Shell.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Threading;

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.WPF.ViewModels.Assets;
    using Insyston.Operations.WPF.ViewModels.Assets.AssetClasses;
    using Insyston.Operations.WPF.ViewModels.Collections;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;
    using Insyston.Operations.WPF.ViewModels.Funding;
    using Insyston.Operations.WPF.ViewModels.RegisteredAsset;
    using Insyston.Operations.WPF.ViewModels.Security;
    using Insyston.Operations.WPF.ViewModels.Shell;

    /// <summary>
    /// The main view model.
    /// </summary>
    public class MainViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        private DocumentTabViewModel _documentTabViewModel;

        /// <summary>
        /// The _ menu list view model.
        /// </summary>
        private MainMenuViewModel _menuListViewModel;

        /// <summary>
        /// The _system toolbar view model.
        /// </summary>
        private SystemToolbarViewModel _systemToolbarViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            this._documentTabViewModel = new DocumentTabViewModel();
            this._menuListViewModel = new MainMenuViewModel();
            this._systemToolbarViewModel = new SystemToolbarViewModel();
            foreach (var item in this.MenuListViewModel.ListButtons)
            {
                item.OnClickButton += this.OnNewMainContent;
            }
            this._documentTabViewModel.TabSelectionChanged += this.ChangeStyleMainMenuItemClick;
            this._systemToolbarViewModel.OnClickButton += this.OnNewMainContent;
            this._documentTabViewModel.TabItemAddClick += this.OnNewMainContent;

            this.OnNewMainContent("+");
        }

        /// <summary>
        /// Gets or sets the document tab view model.
        /// </summary>
        public DocumentTabViewModel DocumentTabViewModel
        {
            get
            {
                return this._documentTabViewModel;
            }
            set
            {
                if (value != null)
                {
                    this.SetField(ref this._documentTabViewModel, value, () => this.DocumentTabViewModel);
                }
            }
        }

        /// <summary>
        /// Gets or sets the menu list view model.
        /// </summary>
        public MainMenuViewModel MenuListViewModel
        {
            get
            {
                return this._menuListViewModel;
            }
            set
            {
                if (value != null)
                {
                    this.SetField(ref this._menuListViewModel, value, () => this.MenuListViewModel);
                }
            }
        }

        /// <summary>
        /// Gets or sets the system toolbar view model.
        /// </summary>
        public SystemToolbarViewModel SystemToolbarViewModel
        {
            get
            {
                return this._systemToolbarViewModel;
            }
            set
            {
                if (value != null)
                {
                    this.SetField(ref this._systemToolbarViewModel, value, () => this.SystemToolbarViewModel);
                }
            }
        }

        /// <summary>
        /// The change style main menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        private void ChangeStyleMainMenuItemClick(object sender)
        {
            // Change style for MainMenuButton when change tab.
            foreach (var item in this.MenuListViewModel.ListButtons)
            {
                if (item.Header.Equals(sender))
                {
                    item.SelectedStyle = (Style)Application.Current.FindResource("MenuStyleClick");
                }
                else
                {
                    item.SelectedStyle = (Style)Application.Current.FindResource("MenuStyle");
                }
            }
        }

        /// <summary>
        /// The loffof main window.
        /// </summary>
        public Action PowerOffMainWindowView;

        /// <summary>
        /// The on new main content.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        public async void OnNewMainContent(object sender)
        {
            CustomHyperlink configurationItem = sender as CustomHyperlink;

            if (configurationItem != null)
            {
                sender = configurationItem.Action;
            }

            string content;

            // Check if sender is Add tab button.
            if (sender.ToString().Equals("+"))
            {
                content = "Home";
            }
            else
            {
                content = sender.ToString();
            }

            // Check if sender is call from Configuration page.
            if (sender.ToString().Equals("ConfigurationMenu"))
            {
                content = "Configuration";
            }
            if (sender.ToString().Equals("Asset Register"))
            {
                content = "RegisteredAsset";
            }

            var newTab = new CustomTabItem();
            newTab.ItemvalidateTabContent = new List<ItemValidateContent>();
            switch (content)
            {
                case "Home":
                    var homePage = new MainWindowDetailsToggle();
                    var homepageViewModel = new MainWindowDetailsToggleViewModel("Home");

                    var homenContent = new ObservableCollection<object>();

                    var homeControl = new MainWindowDetails();

                    Binding homeBinding = new Binding("ScreenDetailViewModel");
                    homeBinding.Source = homepageViewModel;
                    homeControl.SetBinding(FrameworkElement.DataContextProperty, homeBinding);

                    homenContent.Add(homeControl);
                    homepageViewModel.ScreenDetailViewModel = new MainWindowDetailsViewModel(EnumScreen.Home);
                    homepageViewModel.MainContent = homenContent;

                    homePage.DataContext = homepageViewModel;

                    // Add content for new tab
                    newTab.Content = homePage;
                    break;

                case "Funding":
                    if (await Operations.Security.Authorisation.IsModuleInstalledAsync(Modules.Funding))
                    {
                        var fundingView = new MainWindowDetailsToggle();
                        var fundingViewModel = new MainWindowDetailsToggleViewModel("Funding");

                        var fundingContent = new ObservableCollection<object>();
                        var fundingControl = new MainWindowDetails();

                        Binding fundingBinding = new Binding("ScreenDetailViewModel");
                        fundingBinding.Source = fundingViewModel;
                        fundingControl.SetBinding(FrameworkElement.DataContextProperty, fundingBinding);

                        fundingContent.Add(fundingControl);

                        fundingViewModel.ScreenDetailViewModel =
                            new MainWindowDetailsViewModel(EnumScreen.FundingSummary);
                        (fundingViewModel.ScreenDetailViewModel as MainWindowDetailsViewModel).NavigatedToScreen =
                            NavigatedToScreen;
                        fundingViewModel.MainContent = fundingContent;
                        fundingView.DataContext = fundingViewModel;

                        // Add content for new tab
                        newTab.Content = fundingView;
                    }
                    else
                    {
                        var fundingView = new MainWindowDetailsToggle();
                        var fundingViewModel = new MainWindowDetailsToggleViewModel("Funding");

                        var fundingContent = new ObservableCollection<object>();
                        var fundingControl = new DescriptionModuleView();
                        Binding fundingBinding = new Binding("ScreenDetailViewModel");
                        fundingBinding.Source = fundingViewModel;
                        fundingControl.SetBinding(FrameworkElement.DataContextProperty, fundingBinding);

                        fundingContent.Add(fundingControl);

                        fundingViewModel.ScreenDetailViewModel =
                            new DescriptionModuleViewModel("Funding");
                        fundingViewModel.MainContent = fundingContent;
                        fundingView.DataContext = fundingViewModel;

                        // Add content for new tab
                        newTab.Content = fundingView;
                    }
                    break;

                case "Collection":
                    if (await Operations.Security.Authorisation.IsModuleInstalledAsync(Modules.Collections))
                    {
                        var collectionView = new MainWindowDetailsToggle();
                        var collectionViewModel = new MainWindowDetailsToggleViewModel("Collection");

                        var collectionContent = new ObservableCollection<object>();

                        var collectionControl = new MainWindowDetails();

                        Binding collectionBinding = new Binding("ScreenDetailViewModel");
                        collectionBinding.Source = collectionViewModel;
                        collectionControl.SetBinding(FrameworkElement.DataContextProperty, collectionBinding);

                        collectionContent.Add(collectionControl);
                        collectionViewModel.ScreenDetailViewModel =
                            new MainWindowDetailsViewModel(EnumScreen.CollectionAssignment);
                        (collectionViewModel.ScreenDetailViewModel as MainWindowDetailsViewModel).NavigatedToScreen =
                            NavigatedToScreen;
                        collectionViewModel.MainContent = collectionContent;

                        collectionView.DataContext = collectionViewModel;

                        // Add content for new tab
                        newTab.Content = collectionView;
                    }
                    else
                    {
                        var collectionView = new MainWindowDetailsToggle();
                        var collectionViewModel = new MainWindowDetailsToggleViewModel("Collection");

                        var collectionContent = new ObservableCollection<object>();

                        var collectionControl = new DescriptionModuleView();

                        Binding collectionBinding = new Binding("ScreenDetailViewModel");
                        collectionBinding.Source = collectionViewModel;
                        collectionControl.SetBinding(FrameworkElement.DataContextProperty, collectionBinding);

                        collectionContent.Add(collectionControl);
                        collectionViewModel.ScreenDetailViewModel =
                            new DescriptionModuleViewModel("Collection");

                        collectionViewModel.MainContent = collectionContent;

                        collectionView.DataContext = collectionViewModel;

                        // Add content for new tab
                        newTab.Content = collectionView;
                    }
                    break;
                case "Security":
                    var usersGroupsContent = new MainWindowDetailsToggle();
                    var usersGroupsViewModel = new MainWindowDetailsToggleViewModel("Security");
                    usersGroupsContent.DataContext = usersGroupsViewModel;
                    var content1 = new ObservableCollection<object>();

                    var tabControl = new MainWindowDetailsTabControl();

                    Binding securitybinding = new Binding("ScreenDetailViewModel");
                    securitybinding.Source = usersGroupsViewModel;
                    tabControl.SetBinding(FrameworkElement.DataContextProperty, securitybinding);

                    content1.Add(tabControl);
                    usersGroupsViewModel.ScreenDetailViewModel = new MainWindowDetailsTabControlViewModel();
                    (usersGroupsViewModel.ScreenDetailViewModel as MainWindowDetailsTabControlViewModel).NavigatedToScreen = NavigatedToScreen;
                    usersGroupsViewModel.MainContent = content1;

                    usersGroupsContent.DataContext = usersGroupsViewModel;

                    // Add content for new tab
                    newTab.Content = usersGroupsContent;

                    break;
                case "PowerOff":
                    bool canProceed = true;
                    foreach (var tabItem in this._documentTabViewModel.ListTabItems)
                    {
                        int isContent = this._documentTabViewModel.ValidateContentClose(tabItem);
                        if (isContent == 1)
                        {
                            break;
                        }
                        if (isContent == 0)
                        {
                            canProceed = false;
                            break;
                        }
                    }

                    if (canProceed)
                    {
                        foreach (var tab in _documentTabViewModel.ListTabItems)
                        {
                            if (tab.ItemvalidateTabContent != null)
                            {
                                foreach (var itemTabValidate in tab.ItemvalidateTabContent)
                                {
                                    if (itemTabValidate.DoUnLockAsync != null)
                                    {
                                        itemTabValidate.DoUnLockAsync();
                                    }
                                }
                            }
                        }
                        if (PowerOffMainWindowView != null)
                        {
                            this.PowerOffMainWindowView();
                        }
                    }
                    return;
                case "Configuration":

                    var configContent = new MainWindowDetailsToggle();
                    var configViewModel = new MainWindowDetailsToggleViewModel("Configuration");

                    var contentConfig = new ObservableCollection<object>();
                    var configuration = new MainWindowDetails();

                    Binding configurationBinding = new Binding("ScreenDetailViewModel");
                    configurationBinding.Source = configViewModel;
                    configuration.SetBinding(FrameworkElement.DataContextProperty, configurationBinding);

                    contentConfig.Add(configuration);
                    configViewModel.ScreenDetailViewModel = new MainWindowDetailsViewModel(EnumScreen.Configuration);
                    (configViewModel.ScreenDetailViewModel as MainWindowDetailsViewModel).NavigatedToScreen = NavigatedToScreen;
                    (configViewModel.ScreenDetailViewModel as MainWindowDetailsViewModel).ConfigurationItemClicked = this.OnNewMainContent;
                    configViewModel.MainContent = contentConfig;

                    configContent.DataContext = configViewModel;

                    // Add content for new tab
                    newTab.Content = configContent;
                    break;
                case "CollectionSettings":
                    if (await Operations.Security.Authorisation.IsModuleInstalledAsync(Modules.Collections))
                    {
                        var collectionSettingContent = new MainWindowDetailsToggle();
                        var collectionSettingViewModel = new MainWindowDetailsToggleViewModel("CollectionSettings");

                        var contentCollectionSetting = new ObservableCollection<object>();
                        var collectionSetting = new MainWindowDetails();

                        Binding collectionSettingBinding = new Binding("ScreenDetailViewModel");
                        collectionSettingBinding.Source = collectionSettingViewModel;
                        collectionSetting.SetBinding(FrameworkElement.DataContextProperty, collectionSettingBinding);

                        contentCollectionSetting.Add(collectionSetting);

                        collectionSettingViewModel.ScreenDetailViewModel =
                            new MainWindowDetailsViewModel(EnumScreen.CollectionSettings);
                        (collectionSettingViewModel.ScreenDetailViewModel as MainWindowDetailsViewModel)
                            .NavigatedToScreen = NavigatedToScreen;

                        collectionSettingViewModel.MainContent = contentCollectionSetting;
                        collectionSettingViewModel.BackToConfiguration = this.OnNewMainContent;

                        collectionSettingContent.DataContext = collectionSettingViewModel;

                        // Add content for new tab
                        newTab.Content = collectionSettingContent;
                    }
                    else
                    {
                        var collectionSettingContent = new MainWindowDetailsToggle();
                        var collectionSettingViewModel = new MainWindowDetailsToggleViewModel("CollectionSettings");

                        var contentCollectionSetting = new ObservableCollection<object>();

                        var collectionSetting = new DescriptionModuleView();

                        Binding collectionBinding = new Binding("ScreenDetailViewModel");
                        collectionBinding.Source = collectionSettingViewModel;
                        collectionSetting.SetBinding(FrameworkElement.DataContextProperty, collectionBinding);

                        contentCollectionSetting.Add(collectionSetting);
                        collectionSettingViewModel.ScreenDetailViewModel = new DescriptionModuleViewModel("Collection");

                        collectionSettingViewModel.MainContent = contentCollectionSetting;
                        collectionSettingViewModel.BackToConfiguration = this.OnNewMainContent;

                        collectionSettingContent.DataContext = collectionSettingViewModel;

                        // Add content for new tab
                        newTab.Content = collectionSettingContent;
                    }
                    break;
                case "SecuritySetting":
                    var securitySettingContent = new MainWindowDetailsToggle();
                    var securitySettingViewModel = new MainWindowDetailsToggleViewModel("SecuritySetting");

                    var contentSecuritySetting = new ObservableCollection<object>();
                    var securitySetting = new MainWindowDetails();

                    Binding securitySettingBinding = new Binding("ScreenDetailViewModel");
                    securitySettingBinding.Source = securitySettingViewModel;
                    securitySetting.SetBinding(FrameworkElement.DataContextProperty, securitySettingBinding);

                    contentSecuritySetting.Add(securitySetting);

                    securitySettingViewModel.ScreenDetailViewModel = new MainWindowDetailsViewModel(EnumScreen.SecuritySetting);
                    (securitySettingViewModel.ScreenDetailViewModel as MainWindowDetailsViewModel).NavigatedToScreen = NavigatedToScreen;

                    securitySettingViewModel.MainContent = contentSecuritySetting;
                    securitySettingViewModel.BackToConfiguration = this.OnNewMainContent;

                    securitySettingContent.DataContext = securitySettingViewModel;

                    // Add content for new tab
                    newTab.Content = securitySettingContent;
                    break;
                case "ColletionQueues":
                    if (await Operations.Security.Authorisation.IsModuleInstalledAsync(Modules.Collections))
                    {
                        var colletionQueuesContent = new MainWindowDetailsToggle();
                        var colletionQueuesViewModel = new MainWindowDetailsToggleViewModel("ColletionQueues");

                        var contentColletionQueues = new ObservableCollection<object>();
                        var colletionQueues = new MainWindowDetailsTabControlCollectionQueue();

                        Binding collectionQueuesBinding = new Binding("ScreenDetailViewModel");
                        collectionQueuesBinding.Source = colletionQueuesViewModel;
                        colletionQueues.SetBinding(FrameworkElement.DataContextProperty, collectionQueuesBinding);

                        contentColletionQueues.Add(colletionQueues);

                        colletionQueuesViewModel.ScreenDetailViewModel =
                            new MainWindowDetailsTabControlCollectionQueueViewModel();
                        (colletionQueuesViewModel.ScreenDetailViewModel as
                         MainWindowDetailsTabControlCollectionQueueViewModel).NavigatedToScreen = NavigatedToScreen;

                        colletionQueuesViewModel.MainContent = contentColletionQueues;
                        colletionQueuesViewModel.BackToConfiguration = this.OnNewMainContent;

                        colletionQueuesContent.DataContext = colletionQueuesViewModel;

                        // Add content for new tab
                        newTab.Content = colletionQueuesContent;
                    }
                    else
                    {
                        var colletionQueuesContent = new MainWindowDetailsToggle();
                        var colletionQueuesViewModel = new MainWindowDetailsToggleViewModel("ColletionQueues");

                        var contentColletionQueues = new ObservableCollection<object>();

                        var colletionQueues = new DescriptionModuleView();

                        Binding collectionBinding = new Binding("ScreenDetailViewModel");
                        collectionBinding.Source = colletionQueuesViewModel;
                        colletionQueues.SetBinding(FrameworkElement.DataContextProperty, collectionBinding);

                        contentColletionQueues.Add(colletionQueues);
                        colletionQueuesViewModel.ScreenDetailViewModel = new DescriptionModuleViewModel("Collection");

                        colletionQueuesViewModel.MainContent = contentColletionQueues;
                        colletionQueuesViewModel.BackToConfiguration = this.OnNewMainContent;

                        colletionQueuesContent.DataContext = colletionQueuesViewModel;

                        // Add content for new tab
                        newTab.Content = colletionQueuesContent;
                    }
                    break;
                case "AssetClasses":
                    var assetClassesContent = new MainWindowDetailsToggle();
                    var assetClassesViewModel = new MainWindowDetailsToggleViewModel("AssetClasses");

                    var assetClasses = new ObservableCollection<object>();
                    var mainAssetClasses = new MainAssetClassesView();

                    Binding assetClassesBinding = new Binding("ScreenDetailViewModel");
                    assetClassesBinding.Source = assetClassesViewModel;
                    mainAssetClasses.SetBinding(FrameworkElement.DataContextProperty, assetClassesBinding);

                    assetClasses.Add(mainAssetClasses);

                    assetClassesViewModel.ScreenDetailViewModel = new MainAssetClassesViewModel();
                    (assetClassesViewModel.ScreenDetailViewModel as MainAssetClassesViewModel).NavigatedToScreen = NavigatedToScreen;

                    assetClassesViewModel.MainContent = assetClasses;
                    assetClassesViewModel.BackToConfiguration = this.OnNewMainContent;

                    assetClassesContent.DataContext = assetClassesViewModel;

                    // Add content for new tab
                    newTab.Content = assetClassesContent;
                    break;
                case "AssetCollateralClasses":
                    if (await Operations.Security.Authorisation.IsModuleInstalledAsync(Modules.PPSR))
                    {
                        var assetCollateralContent = new MainWindowDetailsToggle();
                        var assetCollateralViewModel = new MainWindowDetailsToggleViewModel("AssetCollateralClasses");

                        var contentassetCollateral = new ObservableCollection<object>();
                        var assetCollateral = new MainWindowDetails();

                        Binding assetCollateralBinding = new Binding("ScreenDetailViewModel");
                        assetCollateralBinding.Source = assetCollateralViewModel;
                        assetCollateral.SetBinding(FrameworkElement.DataContextProperty, assetCollateralBinding);

                        contentassetCollateral.Add(assetCollateral);

                        assetCollateralViewModel.ScreenDetailViewModel =
                            new MainWindowDetailsViewModel(EnumScreen.AssetCollateralClasses);
                        (assetCollateralViewModel.ScreenDetailViewModel as MainWindowDetailsViewModel).NavigatedToScreen
                            = NavigatedToScreen;

                        assetCollateralViewModel.MainContent = contentassetCollateral;
                        assetCollateralViewModel.BackToConfiguration = this.OnNewMainContent;

                        assetCollateralContent.DataContext = assetCollateralViewModel;

                        // Add content for new tab
                        newTab.Content = assetCollateralContent;
                    }
                    else
                    {
                        var assetCollateralContent = new MainWindowDetailsToggle();
                        var assetCollateralViewModel = new MainWindowDetailsToggleViewModel("AssetCollateralClasses");

                        var contentassetCollateral = new ObservableCollection<object>();
                        var assetCollateral = new DescriptionModuleView();

                        Binding assetCollateralBinding = new Binding("ScreenDetailViewModel");
                        assetCollateralBinding.Source = assetCollateralViewModel;
                        assetCollateral.SetBinding(FrameworkElement.DataContextProperty, assetCollateralBinding);

                        contentassetCollateral.Add(assetCollateral);

                        assetCollateralViewModel.ScreenDetailViewModel =
                            new DescriptionModuleViewModel("AssetCollateralClasses");
                        assetCollateralViewModel.MainContent = contentassetCollateral;
                        assetCollateralViewModel.BackToConfiguration = this.OnNewMainContent;
                        assetCollateralContent.DataContext = assetCollateralViewModel;

                        // Add content for new tab
                        newTab.Content = assetCollateralContent;
                    }
                    break;
                case "AssetFeatures":
                    var assetFeaturesContent = new MainWindowDetailsToggle();
                    var assetFeaturesViewModel = new MainWindowDetailsToggleViewModel("AssetFeatures");

                    var contentAssetFeatures = new ObservableCollection<object>();
                    var assetFeatures = new MainWindowDetails();

                    Binding assetFeaturesBinding = new Binding("ScreenDetailViewModel");
                    assetFeaturesBinding.Source = assetFeaturesViewModel;
                    assetFeatures.SetBinding(FrameworkElement.DataContextProperty, assetFeaturesBinding);

                    contentAssetFeatures.Add(assetFeatures);

                    assetFeaturesViewModel.ScreenDetailViewModel = new MainWindowDetailsViewModel(EnumScreen.AssetFeatures);
                    (assetFeaturesViewModel.ScreenDetailViewModel as MainWindowDetailsViewModel).NavigatedToScreen = NavigatedToScreen;

                    assetFeaturesViewModel.MainContent = contentAssetFeatures;
                    assetFeaturesViewModel.BackToConfiguration = this.OnNewMainContent;

                    assetFeaturesContent.DataContext = assetFeaturesViewModel;

                    // Add content for new tab
                    newTab.Content = assetFeaturesContent;
                    break;
                case "AssetSetting":
                    var assetSettingContent = new MainWindowDetailsToggle();
                    var assetSettingViewModel = new MainWindowDetailsToggleViewModel("AssetSetting");

                    var contentAssetSetting = new ObservableCollection<object>();
                    var assetSetting = new MainWindowDetails();

                    Binding assetSettingBinding = new Binding("ScreenDetailViewModel");
                    assetSettingBinding.Source = assetSettingViewModel;
                    assetSetting.SetBinding(FrameworkElement.DataContextProperty, assetSettingBinding);

                    contentAssetSetting.Add(assetSetting);

                    assetSettingViewModel.ScreenDetailViewModel = new MainWindowDetailsViewModel(EnumScreen.AssetSettings);
                    (assetSettingViewModel.ScreenDetailViewModel as MainWindowDetailsViewModel).NavigatedToScreen = NavigatedToScreen;

                    assetSettingViewModel.MainContent = contentAssetSetting;
                    assetSettingViewModel.BackToConfiguration = this.OnNewMainContent;

                    assetSettingContent.DataContext = assetSettingViewModel;

                    // Add content for new tab
                    newTab.Content = assetSettingContent;
                    break;
                case "AssetRegister":

                        var assetRegisterContent = new MainWindowDetailsToggle();
                        var assetRegisterViewModel = new MainWindowDetailsToggleViewModel("AssetRegister");

                        var contentAssetRegister = new ObservableCollection<object>();
                        var assetRegister = new MainWindowDetails();

                        Binding assetRegisterBinding = new Binding("ScreenDetailViewModel");
                        assetRegisterBinding.Source = assetRegisterViewModel;
                        assetRegister.SetBinding(FrameworkElement.DataContextProperty, assetRegisterBinding);

                        contentAssetRegister.Add(assetRegister);

                        assetRegisterViewModel.ScreenDetailViewModel =
                            new MainWindowDetailsViewModel(EnumScreen.AssetRegisters);
                        (assetRegisterViewModel.ScreenDetailViewModel as MainWindowDetailsViewModel).NavigatedToScreen =
                            NavigatedToScreen;

                        assetRegisterViewModel.MainContent = contentAssetRegister;
                        assetRegisterViewModel.BackToConfiguration = this.OnNewMainContent;

                        assetRegisterContent.DataContext = assetRegisterViewModel;

                        // Add content for new tab
                        newTab.Content = assetRegisterContent;
                    
                    break;
                case "RegisteredAsset":
                    if (await Operations.Security.Authorisation.IsModuleInstalledAsync(Modules.AssetRegister))
                    {
                        var registeredAssetContent = new MainWindowDetailsToggle();
                        var registeredAssetViewModel = new MainWindowDetailsToggleViewModel("RegisteredAsset");
                        registeredAssetContent.DataContext = registeredAssetViewModel;
                        var contentRegisteredAsset = new ObservableCollection<object>();
                        var registeredAsset = new MainWindowDetails();
                        Binding registeredAssetBinding = new Binding("ScreenDetailViewModel");
                        registeredAssetBinding.Source = registeredAssetViewModel;

                        registeredAsset.SetBinding(FrameworkElement.DataContextProperty, registeredAssetBinding);

                        contentRegisteredAsset.Add(registeredAsset);
                        registeredAssetViewModel.ScreenDetailViewModel =
                            new MainWindowDetailsViewModel(EnumScreen.RegisteredAsset);
                        (registeredAssetViewModel.ScreenDetailViewModel as MainWindowDetailsViewModel).NavigatedToScreen = NavigatedToScreen;
                        registeredAssetViewModel.MainContent = contentRegisteredAsset;

                        registeredAssetContent.DataContext = registeredAssetViewModel;

                        // Add content for new tab
                        newTab.Content = registeredAssetContent;
                    }
                    else
                    {
                        var registeredAssetView = new MainWindowDetailsToggle();
                        var registeredAssetViewModel = new MainWindowDetailsToggleViewModel("RegisteredAsset");

                        var registeredAsssetContent = new ObservableCollection<object>();

                        var registeredAsssetControl = new DescriptionModuleView();

                        Binding registeredAssetBinding = new Binding("ScreenDetailViewModel");
                        registeredAssetBinding.Source = registeredAssetViewModel;
                        registeredAsssetControl.SetBinding(FrameworkElement.DataContextProperty, registeredAssetBinding);

                        registeredAsssetContent.Add(registeredAsssetControl);
                        registeredAssetViewModel.ScreenDetailViewModel =
                            new DescriptionModuleViewModel("RegisteredAsset");

                        registeredAssetViewModel.MainContent = registeredAsssetContent;

                        registeredAssetView.DataContext = registeredAssetViewModel;

                        // Add content for new tab
                        newTab.Content = registeredAssetView;
                    }
                    break;
            }

            // Sender is Add and Configuration will be create new tab.
            if (sender.ToString().Equals("+") || sender.ToString().Equals("Configuration"))
            {
                this._documentTabViewModel.AddTabItemWithContent(sender.ToString(), newTab);
            }
            else
            {
                // All another sender from MainMenu button will be change content of tab is selected.
                this._documentTabViewModel.ChangeContentSelectedTab(sender.ToString(), newTab);
            }
        }

        /// <summary>
        /// Set event check is content is editing or not.
        /// Set Dispose method for event Tab unloaded to free memory when closing the tab.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        private void NavigatedToScreen(object viewModel)
        {
            CustomTabItem tabItemNew = _documentTabViewModel.ListTabItems.Where(tab => tab.IsSelected).FirstOrDefault();

            if (tabItemNew != null)
            {
                var mainWindowDetailsToggle = tabItemNew.Content as MainWindowDetailsToggle;
                if (mainWindowDetailsToggle != null)
                {
                    var mainModel = mainWindowDetailsToggle.DataContext as MainWindowDetailsToggleViewModel;
                    tabItemNew.Unloaded += (sender, args) =>
                        {
                            if (mainModel != null)
                            {
                                Application.Current.Dispatcher.BeginInvoke(
                                    new Action(mainModel.Dispose),
                                    DispatcherPriority.Normal);
                            }
                        };
                }
            }

            var mainDetailViewModel = viewModel as MainWindowDetailsViewModel;

            if (mainDetailViewModel != null)
            {
                var collectionAssignmentViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as CollectionsAssignmentViewModel;
                if (collectionAssignmentViewModel != null)
                {                    
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = collectionAssignmentViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = collectionAssignmentViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                    /*
                    MainWindowDetailsToggle viewContent = tabItemNew.Content as MainWindowDetailsToggle;
                    if (viewContent != null)
                    {
                        MainWindowDetailsToggleViewModel viewModelContent = viewContent.DataContext as MainWindowDetailsToggleViewModel;
                        if (viewModelContent != null)
                        {
                            collectionAssignmentViewModel.SelectedItemChanged = viewModelContent.ToggleViewModel.ChangeSelectedItem;
                        }
                    }
                    */
                }

                var fundingViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as FundingSummaryViewModel;
                if (fundingViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = fundingViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = fundingViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var systemSettingViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as SystemSettingViewModel;
                if (systemSettingViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = systemSettingViewModel.CheckContentEditing;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var colletionSettingViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as CollectionsSettingViewModel;
                if (colletionSettingViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = colletionSettingViewModel.CheckContentEditing;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var userViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as UsersViewModel;
                if (userViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = userViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = userViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var groupViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as GroupsViewModel;
                if (groupViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = groupViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = groupViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var membershipViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as MembershipViewModel;
                if (membershipViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = membershipViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = membershipViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var colletionQueuesViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as CollectionsManagementViewModel;
                if (colletionQueuesViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = colletionQueuesViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = colletionQueuesViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var collectorViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as CollectorsViewModel;
                if (collectorViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = collectorViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = collectorViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var assetModelViewModel = mainDetailViewModel.ScreenDetailViewModel as AssetClassesModelViewModel;
                if (assetModelViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate=new ItemValidateContent();
                        tabItemValidate.CheckContentEdit = assetModelViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = assetModelViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var aasetClassCategoryViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as AssetClassesCategoryViewModel;
                if (aasetClassCategoryViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = aasetClassCategoryViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = aasetClassCategoryViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var aasetClassTypeViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as AssetClassesTypeViewModel;
                if (aasetClassTypeViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = aasetClassTypeViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = aasetClassTypeViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var assetFeaturesViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as AssetFeaturesViewModel;
                if (assetFeaturesViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = assetFeaturesViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = assetFeaturesViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var assetSettingViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as AssetSettingsViewModel;
                if (assetSettingViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = assetSettingViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = assetSettingViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var assetMakeViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as AssetClassesMakeViewModel;
                if (assetMakeViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = assetMakeViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = assetMakeViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var assetCollateralViewModel =
                    mainDetailViewModel.ScreenDetailViewModel as AssetCollateralClassesViewModel;
                if (assetCollateralViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = assetCollateralViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = assetCollateralViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }

                var assetRegistersViewModel =
                  mainDetailViewModel.ScreenDetailViewModel as AssetRegistersViewModel;
                if (assetRegistersViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = assetRegistersViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = assetRegistersViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
                }
                var registeredAssetViewModel =
                  mainDetailViewModel.ScreenDetailViewModel as RegisteredAssetViewModel;
                if (registeredAssetViewModel != null)
                {
                    if (tabItemNew != null)
                    {
                        ItemValidateContent tabItemValidate = new ItemValidateContent();

                        tabItemValidate.CheckContentEdit = registeredAssetViewModel.CheckContentEditing;
                        tabItemValidate.DoUnLockAsync = registeredAssetViewModel.UnlockItem;
                        tabItemNew.ItemvalidateTabContent.Add(tabItemValidate);
                    }
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

    }
}