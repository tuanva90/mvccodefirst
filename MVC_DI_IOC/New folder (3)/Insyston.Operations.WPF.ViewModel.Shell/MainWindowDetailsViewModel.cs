// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowDetailsViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The new module selected event handler 1.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Insyston.Operations.WPF.ViewModels.Assets.AssetClasses;

namespace Insyston.Operations.WPF.ViewModels.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Funding;

    /// <summary>
    /// The new module selected event handler 1.
    /// </summary>
    /// <param name="module">
    /// The module.
    /// </param>
    public delegate void NewModuleSelectedEventHandler1(object module);

    /// <summary>
    /// The mw details view model : manage dynamic hyperlink view model, action command, old content view model
    /// </summary>
    public class MainWindowDetailsViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// The navigated to screen.
        /// </summary>
        public Action<object> NavigatedToScreen;

        /// <summary>
        /// Gets or sets the key tab document.
        /// </summary>
        public string KeyTabDocument { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowDetailsViewModel"/> class.
        /// </summary>
        public MainWindowDetailsViewModel()
        {
            this._listUserControls = new ObservableCollection<UserControl>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowDetailsViewModel"/> class.
        /// </summary>
        /// <param name="screen">
        /// The screen.
        /// </param>
        public MainWindowDetailsViewModel(EnumScreen screen)
        {
            this._hyperLinkVm = new TabHyperlinkViewModel();
            this._listUserControls = new ObservableCollection<UserControl>();
            _changedVisibility = new Visibility();
            ChangedVisibilityHyperlink = Visibility.Collapsed;

            // set current screen (Users/Groups/Collection/ ...)
            Screen = screen;
        }

        #region event

        /// <summary>
        /// The on new module.
        /// </summary>
        public event NewModuleSelectedEventHandler1 OnNewModule;

        #endregion

        #region Properties

        /// <summary>
        /// The style hyperlink click.
        /// </summary>
        private const string StyleHyperlinkClick = "HyperLinkButtonStyle2";

        /// <summary>
        /// The style error hyperlink click.
        /// </summary>
        private const string StyleErrorHyperlinkClick = "HyperLinkButtonStyleErrorClick";

        private string _permissionText;

        public string PermissionText
        {
            get
            {
                return this._permissionText;
            }
            set
            {
                this.SetField(ref this._permissionText, value, () => this.PermissionText);
            }
        }

        /// <summary>
        /// The _hyper link vm.
        /// </summary>
        private TabHyperlinkViewModel _hyperLinkVm;

        /// <summary>
        /// Gets or sets the hyperlink vm.
        /// </summary>
        public TabHyperlinkViewModel HyperlinkVm
        {
            get
            {
                return this._hyperLinkVm;
            }
            set
            {
                this.SetField(ref this._hyperLinkVm, value, () => this.HyperlinkVm);
            }
        }

        /// <summary>
        /// The _ list user controls.
        /// </summary>
        private ObservableCollection<UserControl> _listUserControls;

        /// <summary>
        /// Gets or sets the list user controls.
        /// </summary>
        public ObservableCollection<UserControl> ListUserControls
        {
            get
            {
                return this._listUserControls;
            }
            set
            {
                this.SetField(ref this._listUserControls, value, () => ListUserControls);
            }
        }

        /// <summary>
        /// Gets or sets the screen.
        /// </summary>
        public EnumScreen Screen { get; set; }

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
                this.SetField(ref this._screenDetailViewModel, value, () => ScreenDetailViewModel);
            }
        }

        /// <summary>
        /// The _changed visibility.
        /// </summary>
        private Visibility _changedVisibility;

        /// <summary>
        /// The on hyperlink screen.
        /// </summary>
        public Action<object> OnHyperlinkScreen;

        /// <summary>
        /// Gets or sets the changed visibility hyperlink.
        /// </summary>
        public Visibility ChangedVisibilityHyperlink
        {
            get
            {
                return this._changedVisibility;
            }
            set
            {
                this.SetField(ref _changedVisibility, value, () => ChangedVisibilityHyperlink);
            }
        }

        /// <summary>
        /// The hidden screen.
        /// </summary>
        private List<EnumScreen> HiddenScreen = new List<EnumScreen> { EnumScreen.FundingSummary, EnumScreen.Home, EnumScreen.Configuration, EnumScreen.CollectionAssignment, EnumScreen.SecuritySetting, EnumScreen.AssetFeatures, EnumScreen.AssetCollateralClasses, EnumScreen.AssetRegisters, EnumScreen.RegisteredAsset};

        #endregion

        #region Private Method

        /// <summary>
        /// The set dynamic hyperlink.
        /// </summary>
        private void SetCustomHyperlink()
        {
            switch (Screen)
            {
                case EnumScreen.Users:
                    List<CustomHyperlink> userCustomHyperlinks = new List<CustomHyperlink>
                                                         {
                                                             new CustomHyperlink { HyperlinkHeader = "Detail", Action = HyperLinkAction.PersonalDetailsState, Screen = EnumScreen.Users, SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick) },
                                                             new CustomHyperlink { HyperlinkHeader = "Credentials", Action = HyperLinkAction.CredentialsState, Screen = EnumScreen.Users },
                                                             new CustomHyperlink { HyperlinkHeader = "Permissions", Action = HyperLinkAction.PermissionsState, Screen = EnumScreen.Users }
                                                         };
                    this.HyperlinkVm.CustomHyperlinks = userCustomHyperlinks;
                    break;
                case EnumScreen.Groups:
                    List<CustomHyperlink> groupCustomHyperlinks = new List<CustomHyperlink>
                                                         {
                                                             new CustomHyperlink { HyperlinkHeader = "Detail", Action = HyperLinkAction.DetailsState, Screen = EnumScreen.Groups, SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick) },
                                                             new CustomHyperlink { HyperlinkHeader = "Permissions", Action = HyperLinkAction.PermissionsState, Screen = EnumScreen.Groups }
                                                         };
                    this.HyperlinkVm.CustomHyperlinks = groupCustomHyperlinks;
                    break;
                case EnumScreen.CollectionAssignment:
                    List<CustomHyperlink> collectionAssignmentCustomHyperlinks = new List<CustomHyperlink>
                                                         {
                                                             new CustomHyperlink { HyperlinkHeader = "Detail", Action = HyperLinkAction.Details, Screen = EnumScreen.CollectionAssignment, SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick) },
                                                             new CustomHyperlink { HyperlinkHeader = "Activity", Action = HyperLinkAction.Activity, Screen = EnumScreen.CollectionAssignment },
                                                         };
                    this.HyperlinkVm.CustomHyperlinks = collectionAssignmentCustomHyperlinks;
                    break;
                case EnumScreen.Configuration:
                    List<CustomHyperlink> configurationCustomHyperlinks = new List<CustomHyperlink>
                                                         {
                                                             new CustomHyperlink { HyperlinkHeader = " " }
                                                         };
                    this.HyperlinkVm.CustomHyperlinks = configurationCustomHyperlinks;
                    break;
                case EnumScreen.ColletionQueues:
                    List<CustomHyperlink> collectionQueueCustomHyperlinks = new List<CustomHyperlink>
                                                         {
                                                             new CustomHyperlink { HyperlinkHeader = "Detail", Action = HyperLinkAction.None, SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick) }
                                                         };
                    this.HyperlinkVm.CustomHyperlinks = collectionQueueCustomHyperlinks;
                    break;
                case EnumScreen.CollectionSettings:
                    List<CustomHyperlink> collectionSettingCustomHyperlinks = new List<CustomHyperlink>
                                                         {
                                                             new CustomHyperlink { HyperlinkHeader = "Settings", Action = HyperLinkAction.None, SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick) }
                                                         };
                    this.HyperlinkVm.CustomHyperlinks = collectionSettingCustomHyperlinks;
                    this.ChangedVisibilityHyperlink = Visibility.Visible;
                    break;
                case EnumScreen.SecuritySetting:
                    List<CustomHyperlink> securitySettingCustomHyperlinks = new List<CustomHyperlink>
                                                         {
                                                             new CustomHyperlink { HyperlinkHeader = " " }
                                                         };
                    this.HyperlinkVm.CustomHyperlinks = securitySettingCustomHyperlinks;
                    break;
                case EnumScreen.FundingSummary:
                    List<CustomHyperlink> fundingSummaryCustomHyperlinks = new List<CustomHyperlink>
                                                         {
                                                             new CustomHyperlink { HyperlinkHeader = "Detail", Action = HyperLinkAction.SummaryState, Screen = EnumScreen.FundingSummary ,SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick)},
                                                             new CustomHyperlink { HyperlinkHeader = "Contracts", Action = HyperLinkAction.ResultState, Screen = EnumScreen.FundingContact },
                                                         };
                    this.HyperlinkVm.CustomHyperlinks = fundingSummaryCustomHyperlinks;
                    break;
                case EnumScreen.Home:
                    List<CustomHyperlink> homeCustomHyperlinks = new List<CustomHyperlink>
                                                         {
                                                             new CustomHyperlink { HyperlinkHeader = " ", Screen = EnumScreen.Home}
                                                         };
                    this.HyperlinkVm.CustomHyperlinks = homeCustomHyperlinks;
                    break;
                case EnumScreen.AssetClassesCategory:
                    var permissionCategoryDetail = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesCategory, Forms.AssetClassesCategoryDetail);
                    var permissionCategoryFeature = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesCategory, Forms.AssetClassesCategoryFeatures);
                    var permissionCategoryType = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesCategory, Forms.AssetClassesCategoryTypes);
                    
                    bool selectedTab = false;
                    List<CustomHyperlink> assetClassesCategoryCustomHyperlinks = new List<CustomHyperlink>();

                    if (permissionCategoryDetail.CanSee)
                    {
                        assetClassesCategoryCustomHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Detail", Action = HyperLinkAction.AssetClassesCategoryDetailState, Screen = EnumScreen.AssetClassesCategory ,SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick)});
                        selectedTab = true;
                    }
                    if (permissionCategoryFeature.CanSee)
                    {
                        if (selectedTab)
                        {
                            assetClassesCategoryCustomHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Features", Action = HyperLinkAction.AssetClassesCategoryFeaturesState, Screen = EnumScreen.AssetClassesCategory });
                        }
                        else
                        {
                            assetClassesCategoryCustomHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Features", Action = HyperLinkAction.AssetClassesCategoryFeaturesState, Screen = EnumScreen.AssetClassesCategory, SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick) });
                            selectedTab = true;
                        }
                    }
                    if (permissionCategoryType.CanSee)
                    {
                        if (selectedTab)
                        {
                            assetClassesCategoryCustomHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Asset Types", Action = HyperLinkAction.AssetClassesCategoryAssetTypesState, Screen = EnumScreen.AssetClassesCategory });
                        }
                        else
                        {
                            assetClassesCategoryCustomHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Asset Types", Action = HyperLinkAction.AssetClassesCategoryAssetTypesState, Screen = EnumScreen.AssetClassesCategory, SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick) });
                            selectedTab = true;
                        }
                    }
                    foreach (var hyper in assetClassesCategoryCustomHyperlinks)
                    {
                        if (hyper.SelectedStyle.Equals((Style)Application.Current.FindResource(StyleHyperlinkClick)))
                        {
                            this.HyperlinkVm.OldCustomHyperlink = hyper;
                            this.HyperlinkVm.CustomHyperlinhSelected = hyper;
                        }
                    }
                    this.HyperlinkVm.CustomHyperlinks = assetClassesCategoryCustomHyperlinks;
                    break;
                case EnumScreen.AssetClassesType:
                    var permissionTypeDetail = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesType, Forms.AssetClassesTypeDetail);
                    var permissionTypeFeature = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesType, Forms.AssetClassesTypeFeatures);
                    var permissionTypeMake = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesType, Forms.AssetClassesTypeMake);

                    bool selectedTabType = false;
                    List<CustomHyperlink> assetClassesTypeCustomHyperlinks = new List<CustomHyperlink>();

                    if (permissionTypeDetail.CanSee)
                    {
                        assetClassesTypeCustomHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Detail", Action = HyperLinkAction.AssetClassesTypeDetailState, Screen = EnumScreen.AssetClassesType, SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick) });
                        selectedTabType = true;
                    }
                    if (permissionTypeFeature.CanSee)
                    {
                        if (selectedTabType)
                        {
                            assetClassesTypeCustomHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Features", Action = HyperLinkAction.AssetClassesTypeFeaturesState, Screen = EnumScreen.AssetClassesType });
                        }
                        else
                        {
                            assetClassesTypeCustomHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Features", Action = HyperLinkAction.AssetClassesTypeFeaturesState, Screen = EnumScreen.AssetClassesType, SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick) });
                            selectedTabType = true;
                        }
                    }
                    if (permissionTypeMake.CanSee)
                    {
                        if (selectedTabType)
                        {
                            assetClassesTypeCustomHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Make", Action = HyperLinkAction.AssetClassesTypeMakeState, Screen = EnumScreen.AssetClassesType });
                        }
                        else
                        {
                            assetClassesTypeCustomHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Make", Action = HyperLinkAction.AssetClassesTypeMakeState, Screen = EnumScreen.AssetClassesType, SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick) });
                            selectedTabType = true;
                        }
                    }
                    foreach (var hyper in assetClassesTypeCustomHyperlinks)
                    {
                        if (hyper.SelectedStyle.Equals((Style)Application.Current.FindResource(StyleHyperlinkClick)))
                        {
                            this.HyperlinkVm.OldCustomHyperlink = hyper;
                            this.HyperlinkVm.CustomHyperlinhSelected = hyper;
                        }
                    }
                    this.HyperlinkVm.CustomHyperlinks = assetClassesTypeCustomHyperlinks;
                    break;
                case EnumScreen.AssetClassesMake:
                    List<CustomHyperlink> assetClassesMakeCustomHyperlinks = new List<CustomHyperlink>
                                                         {
                                                             new CustomHyperlink { HyperlinkHeader = "Detail", Action = HyperLinkAction.None, SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick)}
                                                         };
                    this.HyperlinkVm.CustomHyperlinks = assetClassesMakeCustomHyperlinks;
                    break;
                case EnumScreen.AssetClassesModel:
                     List<CustomHyperlink> assetClassesModelCustomHyperlinks = new List<CustomHyperlink>
                                                         {
                                                             new CustomHyperlink { HyperlinkHeader = "Detail", SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick)}
                                                         };
                    this.HyperlinkVm.CustomHyperlinks = assetClassesModelCustomHyperlinks;
                    break;
                case EnumScreen.AssetCollateralClasses:
                    var permissionCollateralDetail = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetCollateralClasses, Forms.CollateralClassesDetail);
                    var permissionCollateralType = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetCollateralClasses, Forms.CollateralClassesType);

                    bool selectedTabCollateral = false;
                    List<CustomHyperlink> assetCollateralCustomHyperlinks = new List<CustomHyperlink>();

                    if (permissionCollateralDetail.CanSee)
                    {
                        assetCollateralCustomHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Detail", Action = HyperLinkAction.DetailsState, Screen = EnumScreen.AssetFeatures, SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick) });
                        selectedTabCollateral = true;
                    }
                    if (permissionCollateralType.CanSee)
                    {
                        if (selectedTabCollateral)
                        {
                            assetCollateralCustomHyperlinks.Add(
                                new CustomHyperlink
                                    {
                                        HyperlinkHeader = "Type",
                                        Action = HyperLinkAction.AssignedToState,
                                        Screen = EnumScreen.AssetFeatures,                                        
                                    });
                        }
                        else
                        {
                            assetCollateralCustomHyperlinks.Add(
                                new CustomHyperlink
                                {
                                    HyperlinkHeader = "Type",
                                    Action = HyperLinkAction.AssignedToState,
                                    Screen = EnumScreen.AssetFeatures,
                                    SelectedStyle =
                                            (Style)Application.Current.FindResource(StyleHyperlinkClick)
                                });
                        }
                    }
                    this.HyperlinkVm.CustomHyperlinks = assetCollateralCustomHyperlinks;
                    break;
                case EnumScreen.AssetFeatures:
                    var permissionFeatureDetail = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetFeatures, Forms.FeaturesDetail);
                    var permissionFeatureAssignTo = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetFeatures, Forms.FeaturesAssignTo);

                    bool selectedTabFeature = false;
                    List<CustomHyperlink> assetFeaturesCustomHyperlinks = new List<CustomHyperlink>();

                    if (permissionFeatureDetail.CanSee)
                    {
                        assetFeaturesCustomHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Detail", Action = HyperLinkAction.DetailsState, Screen = EnumScreen.AssetFeatures ,SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick) });
                        selectedTabFeature = true;
                    }
                    if (permissionFeatureAssignTo.CanSee)
                    {
                        if (selectedTabFeature)
                        {
                            assetFeaturesCustomHyperlinks.Add(
                                new CustomHyperlink
                                    {
                                        HyperlinkHeader = "Assigned To",
                                        Action = HyperLinkAction.AssignedToState,
                                        Screen = EnumScreen.AssetFeatures,                                        
                                    });
                        }
                        else
                        {
                            assetFeaturesCustomHyperlinks.Add(
                                new CustomHyperlink
                                {
                                    HyperlinkHeader = "Assigned To",
                                    Action = HyperLinkAction.AssignedToState,
                                    Screen = EnumScreen.AssetFeatures,
                                    SelectedStyle =
                                            (Style)Application.Current.FindResource(StyleHyperlinkClick)
                                });
                        }
                    }
                    this.HyperlinkVm.CustomHyperlinks = assetFeaturesCustomHyperlinks;
                    break;
                case EnumScreen.AssetSettings:
                    List<CustomHyperlink> assetSettingsCustomHyperlinks = new List<CustomHyperlink>
                                                         {
                                                             new CustomHyperlink { HyperlinkHeader = "Settings", Action = HyperLinkAction.None, SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick) }
                                                       };
                    this.HyperlinkVm.CustomHyperlinks = assetSettingsCustomHyperlinks;
                    this.ChangedVisibilityHyperlink = Visibility.Visible;
                    break;
                case EnumScreen.AssetRegisters:
                    List<CustomHyperlink> assetRegisterCustomerHyperLinks = new List<CustomHyperlink>
                    {
                        new CustomHyperlink
                        {
                            //HyperlinkHeader = "Detail",
                            //Action = HyperLinkAction.DetailsState,
                            //Screen = EnumScreen.AssetRegisters,
                            //SelectedStyle = (Style) Application.Current.FindResource(StyleHyperlinkClick)
                        }
                    };
                    this.HyperlinkVm.CustomHyperlinks = assetRegisterCustomerHyperLinks;
                    this.ChangedVisibilityHyperlink = Visibility.Visible;
                    break;
                case EnumScreen.RegisteredAsset:
                    List<CustomHyperlink> registeredAssetCustomerHyperLinks = new List<CustomHyperlink>
                    {
                        new CustomHyperlink
                        {
                            HyperlinkHeader = "Detail",
                            Action = HyperLinkAction.DetailsState,
                            Screen = EnumScreen.RegisteredAsset,
                            SelectedStyle = (Style) Application.Current.FindResource(StyleHyperlinkClick)
                        },
                        new CustomHyperlink
                        {
                            HyperlinkHeader = "Depreciation",
                            Action = HyperLinkAction.DepreciationState,
                            Screen = EnumScreen.RegisteredAsset,
                        },
                        new CustomHyperlink
                        {
                            HyperlinkHeader = "Disposal",
                            Action = HyperLinkAction.DisposalState,
                            Screen = EnumScreen.RegisteredAsset,
                        },
                        new CustomHyperlink
                        {
                            HyperlinkHeader = "History",
                            Action = HyperLinkAction.HistoryState,
                            Screen = EnumScreen.RegisteredAsset,
                        }
                    };
                    this.HyperlinkVm.CustomHyperlinks = registeredAssetCustomerHyperLinks;
                    //this.ChangedVisibilityHyperlink = Visibility.Hidden;
                    break;
            }

            // Hyperlinks on some Screen should hidden
            if (HiddenScreen.Contains(Screen))
            {
                this.ChangedVisibilityHyperlink = Visibility.Hidden;
            }
        }
        #endregion

        #region Public Method

        /// <summary>
        /// The hyper link item click.
        /// </summary>
        public void HyperLinkItemClick()
        {
            SetCustomHyperlink();
            foreach (var item in this.HyperlinkVm.CustomHyperlinks)
            {
                item.ItemClicked = ItemCommandClick;
            }
        }

        public Action<object> ConfigurationItemClicked;

        /// <summary>
        /// The hyper link item click.
        /// </summary>
        public void ConfigItemClick()
        {
            var viewModel = this.ScreenDetailViewModel as ConfigurationViewModel;
            if (viewModel != null)
            {
                foreach (var item in viewModel.GroupSetting)
                {
                    foreach (var link in item.Hyperlinks)
                    {
                        link.ItemClicked = this.ConfigurationItemClicked;
                    }
                }
            }
        }

        /// <summary>
        /// The item comman click.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public async void ItemCommandClick(object item)
        {
            CustomHyperlink cus = item as CustomHyperlink;

            if (cus != null)
            {
                this.HyperlinkVm.OldCustomHyperlink = this.HyperlinkVm.CustomHyperlinhSelected;
                // change style Hyperlink when error
                if (this.ScreenDetailViewModel.ListErrorHyperlink.Count > 0)
                {
                    foreach (var hyper in this.HyperlinkVm.CustomHyperlinks)
                    {
                        if (hyper.Action.Equals(cus.Action))
                        {
                            if (hyper.SelectedStyle.Equals((Style)Application.Current.FindResource("HyperLinkButtonStyle")))
                            {
                                hyper.SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick);
                            }
                            else if (hyper.SelectedStyle.Equals((Style)Application.Current.FindResource("HyperLinkButtonStyleError")))
                            {
                                hyper.SelectedStyle = (Style)Application.Current.FindResource(StyleErrorHyperlinkClick);
                            }
                            this.HyperlinkVm.CustomHyperlinhSelected = hyper;
                        }
                        else
                        {
                            if (hyper.SelectedStyle.Equals((Style)Application.Current.FindResource(StyleErrorHyperlinkClick)))
                            {
                                hyper.SelectedStyle = (Style)Application.Current.FindResource("HyperLinkButtonStyleError");
                            }
                            else if (hyper.SelectedStyle.Equals((Style)Application.Current.FindResource(StyleHyperlinkClick)))
                            {
                                hyper.SelectedStyle = (Style)Application.Current.FindResource("HyperLinkButtonStyle");
                            }
                        }
                    }
                }
                else
                {
                    // change style selected Hyperlink
                    foreach (var hyper in this.HyperlinkVm.CustomHyperlinks)
                    {
                        if (hyper.Action.Equals(cus.Action))
                        {
                            hyper.SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick);
                            this.HyperlinkVm.CustomHyperlinhSelected = hyper;
                        }
                        else
                        {
                            hyper.SelectedStyle = (Style)Application.Current.FindResource("HyperLinkButtonStyle");
                        }
                    }
                }
            }
            else
            {
                this.HyperlinkVm.CustomHyperlinhSelected = this.HyperlinkVm.OldCustomHyperlink;
                // change style Hyperlink when error
                if (this.ScreenDetailViewModel.ListErrorHyperlink.Count > 0)
                {
                    foreach (var hyper in this.HyperlinkVm.CustomHyperlinks)
                    {
                        if (hyper.Action.Equals(this.HyperlinkVm.OldCustomHyperlink.Action))
                        {
                            if (hyper.SelectedStyle.Equals((Style)Application.Current.FindResource("HyperLinkButtonStyle")))
                            {
                                hyper.SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick);
                            }
                            else if (hyper.SelectedStyle.Equals((Style)Application.Current.FindResource("HyperLinkButtonStyleError")))
                            {
                                hyper.SelectedStyle = (Style)Application.Current.FindResource(StyleErrorHyperlinkClick);
                            }
                        }
                        else
                        {
                            if (hyper.SelectedStyle.Equals((Style)Application.Current.FindResource(StyleErrorHyperlinkClick)))
                            {
                                hyper.SelectedStyle = (Style)Application.Current.FindResource("HyperLinkButtonStyleError");
                            }
                            else if (hyper.SelectedStyle.Equals((Style)Application.Current.FindResource(StyleHyperlinkClick)))
                            {
                                hyper.SelectedStyle = (Style)Application.Current.FindResource("HyperLinkButtonStyle");
                            }
                        }
                    }
                }
                else
                {
                    // change style selected Hyperlink
                    foreach (var hyper in this.HyperlinkVm.CustomHyperlinks)
                    {
                        if (hyper.Action.Equals(this.HyperlinkVm.OldCustomHyperlink.Action))
                        {
                            hyper.SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick);
                        }
                        else
                        {
                            hyper.SelectedStyle = (Style)Application.Current.FindResource("HyperLinkButtonStyle");
                        }
                    }
                }
            }
            
            if (ScreenDetailViewModel != null)
            {

                if (this.OnHyperlinkScreen != null)
                {
                    this.OnHyperlinkScreen(item);
                }

                if (Screen == EnumScreen.FundingSummary || Screen == EnumScreen.FundingContact)
                {
                    var fundingDetailsViewModel = this.ScreenDetailViewModel as FundingSummaryViewModel;
                    if (fundingDetailsViewModel != null)
                    {
                        if (cus != null)
                        {
                            await fundingDetailsViewModel.FundingDetails.SetViewDetailLostFocus();
                            await fundingDetailsViewModel.FundingDetails.OnStepAsync(cus.Action.ToString());
                        }
                    }
                }
                else if (cus != null)
                {
                    await this.ScreenDetailViewModel.OnStepAsync(cus.Action.ToString());
                }
            }
        }

        /// <summary>
        /// The load module with Dynamic Hyperlink.
        /// </summary>
        public void LoadModule()
        {
            switch (Screen)
            {
                case EnumScreen.Home:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.Home);
                    }
                    break;
                case EnumScreen.Users:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.Users);
                    }
                    break;
                case EnumScreen.Groups:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.Groups);
                    }
                    break;
                case EnumScreen.Configuration:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.Configuration);
                    }
                    break;
                case EnumScreen.CollectionSettings:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.CollectionSettings);
                    }
                    break;
                case EnumScreen.ColletionQueues:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.ColletionQueues);
                    }
                    break;
                case EnumScreen.SecuritySetting:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.SecuritySetting);
                    }
                    break;
                case EnumScreen.CollectionAssignment:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.CollectionAssignment);
                    }
                    break;
                case EnumScreen.Membership:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.Membership);
                    }
                    break;
                case EnumScreen.FundingSummary:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.FundingSummary);
                    }
                    break;
                case EnumScreen.FundingContact:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.FundingContact);
                    }
                    break;
                case EnumScreen.Collectors:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.Collectors);
                    }
                    break;
                case EnumScreen.AssetClassesCategory:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.AssetClassesCategory);
                    }
                    break;
                case EnumScreen.AssetClassesType:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.AssetClassesType);
                    }
                    break;
                case EnumScreen.AssetClassesMake:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.AssetClassesMake);
                    }
                    break;
                case EnumScreen.AssetClassesModel:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.AssetClassesModel);
                    }
                    break;
                case EnumScreen.AssetCollateralClasses:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.AssetCollateralClasses);
                    }
                    break;
                case EnumScreen.AssetFeatures:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.AssetFeatures);
                    }
                    break;
                case EnumScreen.AssetSettings:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.AssetSettings);
                    }
                    break;
                case EnumScreen.AssetRegisters:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.AssetRegisters);
                    }
                    break;
                case EnumScreen.RegisteredAsset:
                    if (this.OnNewModule != null)
                    {
                        this.OnNewModule(EnumScreen.RegisteredAsset);
                    }
                    break;
            }
        }

        /// <summary>
        /// The on raise step changed.
        /// </summary>
        public void OnRaiseStepChanged()
        {
            var viewModel = this.ScreenDetailViewModel;
            if (viewModel != null)
            {
                viewModel.RaiseStepChanged = this.ProcessingStepsOnChild_DetailsVm;
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
                viewModel.CancelNewItem = this.CollapseHyperlink;
            }
        }

        /// <summary>
        /// The on error hyperlink select.
        /// </summary>
        public void OnErrorHyperlinkSelect()
        {
            var viewModel = this.ScreenDetailViewModel;
            if (viewModel != null)
            {
                viewModel.ErrorHyperlinkSelected = this.ErrorHyperLinkClick;
            }
        }

        /// <summary>
        /// The on validate not error.
        /// </summary>
        public void OnValidateNotError()
        {
            var viewModel = this.ScreenDetailViewModel;
            if (viewModel != null)
            {
                viewModel.NotErrorHyperlink = this.ChangeStyleToDefault;
            }
        }

        /// <summary>
        /// The show custom hyper link.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="_params">
        /// The _params.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        public void ProcessingStepsOnChild_DetailsVm(EnumScreen e, object _params, object item)
        {
            EnumSteps currentStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), _params.ToString());

            // Change status hyperlink when selected item on grid summary
            switch (e)
            {
                case EnumScreen.Membership:
                    break;
                case EnumScreen.Collectors:
                    ChangedVisibilityHyperlink = Visibility.Collapsed;
                    break;
                case EnumScreen.AssetClassesCategory:
                    if (currentStep == EnumSteps.SaveAssignFeature || currentStep == EnumSteps.SaveAssignTypes
                        || currentStep == EnumSteps.SaveUpdateDepreciation || currentStep == EnumSteps.CancelAssignFeature
                        || currentStep == EnumSteps.CancelAssignTypes || currentStep == EnumSteps.CancelUpdateDepreciation)
                    {
                        ChangedVisibilityHyperlink = Visibility.Hidden;
                    }
                    else
                    {
                        ChangedVisibilityHyperlink = Visibility.Visible;
                    }
                    break;
                case EnumScreen.AssetClassesType:
                    if (currentStep == EnumSteps.SaveAssignFeature || currentStep == EnumSteps.SaveAssignMake
                        || currentStep == EnumSteps.SaveUpdateDepreciation || currentStep == EnumSteps.CancelAssignFeature
                        || currentStep == EnumSteps.CancelAssignMake || currentStep == EnumSteps.CancelUpdateDepreciation)
                    {
                        ChangedVisibilityHyperlink = Visibility.Hidden;
                    }
                    else
                    {
                        ChangedVisibilityHyperlink = Visibility.Visible;
                    }
                    break;
                case EnumScreen.AssetClassesMake:
                    if (currentStep == EnumSteps.SaveAssignModel || currentStep == EnumSteps.CancelAssignModel)
                    {
                        ChangedVisibilityHyperlink = Visibility.Hidden;
                    }
                    else
                    {
                        ChangedVisibilityHyperlink = Visibility.Visible;
                    }
                    break;
                case EnumScreen.AssetRegisters:
                    if (currentStep == EnumSteps.SelectRegister || currentStep == EnumSteps.Add)
                    {
                        ChangedVisibilityHyperlink = Visibility.Hidden;
                    }
                    break;
                default:
                    if (currentStep != EnumSteps.ItemLocked)
                    {
                        ChangedVisibilityHyperlink = Visibility.Visible;
                    }
                    break;
            }

            // change style for hyperlink when change step
            switch (currentStep)
            {
                case EnumSteps.ResultState:
                case EnumSteps.Copy:
                case EnumSteps.Add:
                case EnumSteps.CreateNew:
                    if (HyperlinkVm != null)
                    {
                        this.HyperlinkVm.CustomHyperlinks.First().SelectedStyle =
                            (Style)Application.Current.FindResource(StyleHyperlinkClick);
                        for (int i = 1; i < this.HyperlinkVm.CustomHyperlinks.Count(); i++)
                        {
                            this.HyperlinkVm.CustomHyperlinks[i].SelectedStyle = (Style)Application.Current.FindResource("HyperLinkButtonStyle");
                        }
                    }
                    break;
                case EnumSteps.SelectOldTabHyperlink:
                    this.ItemCommandClick(null);
                    break;
                case EnumSteps.AssetClassesCategoryAssignFeaturesState:
                    ChangedVisibilityHyperlink = Visibility.Hidden;
                    break;
                case EnumSteps.AssetClassesCategoryAssignTypesState:
                    ChangedVisibilityHyperlink = Visibility.Hidden;
                    break;
                case EnumSteps.AssetClassesCategoryUpdateDepreciationState:
                    ChangedVisibilityHyperlink = Visibility.Hidden;
                    break;
                case EnumSteps.AssetClassesTypeAssignFeaturesState:
                    ChangedVisibilityHyperlink = Visibility.Hidden;
                    break;
                case EnumSteps.AssetClassesTypeAssignMakeState:
                    ChangedVisibilityHyperlink = Visibility.Hidden;
                    break;
                case EnumSteps.AssetClassesTypeUpdateDepreciationState:
                    ChangedVisibilityHyperlink = Visibility.Hidden;
                    break;
                case EnumSteps.AssignModel:
                    ChangedVisibilityHyperlink = Visibility.Hidden;
                    break;
            }

            // Raise action RaiseActionsWhenChangeStep again 
            this.RaiseActionsWhenChangeStep(e, _params, item);
        }

        /// <summary>
        /// The collapse hyperlink.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public void CollapseHyperlink(EnumScreen e)
        {
            // collapsed hyperlink 
            ChangedVisibilityHyperlink = Visibility.Collapsed;
            this.CancelNewItem(e);
        }

        /// <summary>
        /// The error hyper link click.
        /// </summary>
        public void ErrorHyperLinkClick()
        {
            foreach (var item in this.ScreenDetailViewModel.ListErrorHyperlink)
            {
                // Set action click for hyperlinkError
                item.ItemClicked = ItemCommandClick;
            }
            this.ChangeStyleToDefault();
            this.ChangeStyleToError();
        }

        /// <summary>
        /// The change style to error.
        /// </summary>
        public void ChangeStyleToError()
        {
            // Change style for hyperlinkTab when error
            foreach (var hyper in this.HyperlinkVm.CustomHyperlinks)
            {
                bool styleChanged = false;
                foreach (var item in this.ScreenDetailViewModel.ListErrorHyperlink)
                {
                    if (hyper.Action.Equals(item.Action) || (item.Action.Equals(HyperLinkAction.None) && this.HyperlinkVm.CustomHyperlinks.Count < 2))
                    {
                        styleChanged = true;
                        if (hyper.SelectedStyle.Equals((Style)Application.Current.FindResource(StyleHyperlinkClick)))
                        {
                            hyper.SelectedStyle = (Style)Application.Current.FindResource(StyleErrorHyperlinkClick);
                        }
                        else
                        {
                            if (!hyper.SelectedStyle.Equals((Style)Application.Current.FindResource(StyleErrorHyperlinkClick)))
                            {
                                if (!hyper.SelectedStyle.Equals((Style)Application.Current.FindResource(StyleHyperlinkClick)))
                                {
                                    hyper.SelectedStyle = (Style)Application.Current.FindResource("HyperLinkButtonStyleError");
                                }

                            }
                        }
                    }
                    else if (!styleChanged)
                    {
                        if (hyper.SelectedStyle.Equals((Style)Application.Current.FindResource(StyleErrorHyperlinkClick)))
                        {
                            hyper.SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick);
                        }
                        else
                        {
                            if (!hyper.SelectedStyle.Equals((Style)Application.Current.FindResource("HyperLinkButtonStyleError")))
                            {
                                if (!hyper.SelectedStyle.Equals((Style)Application.Current.FindResource(StyleHyperlinkClick)))
                                {
                                    hyper.SelectedStyle = (Style)Application.Current.FindResource("HyperLinkButtonStyle");
                                }
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// The change style to default.
        /// </summary>
        public void ChangeStyleToDefault()
        {
            // Change style for hyperlinkTab to default when validate not error
            if (this.HyperlinkVm != null)
            {
                foreach (var hyper in this.HyperlinkVm.CustomHyperlinks)
                {
                    if (hyper.SelectedStyle.Equals((Style)Application.Current.FindResource(StyleHyperlinkClick)) || hyper.SelectedStyle.Equals((Style)Application.Current.FindResource(StyleErrorHyperlinkClick)))
                    {
                        hyper.SelectedStyle = (Style)Application.Current.FindResource(StyleHyperlinkClick);
                    }
                    else
                    {
                        hyper.SelectedStyle = (Style)Application.Current.FindResource("HyperLinkButtonStyle");
                    }
                }
            }
        }
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
            Dispatcher.CurrentDispatcher.BeginInvoke(new System.Action(() =>
            {
                if (this._hyperLinkVm != null)
                {
                    this._hyperLinkVm.Dispose();
                    this._hyperLinkVm = null;
                }
                if (this._listUserControls != null)
                {
                    this._listUserControls.Clear();
                    this._listUserControls = null;
                }
                if (this._screenDetailViewModel != null)
                {
                    this._screenDetailViewModel.Dispose();
                    this._screenDetailViewModel = null;
                }

                base.Dispose();
            }));
        }
    }

}
