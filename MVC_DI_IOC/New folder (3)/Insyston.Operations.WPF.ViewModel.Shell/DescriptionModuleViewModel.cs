// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DescriptionModuleViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the DescriptionModuleViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Shell
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The description module view model.
    /// </summary>
    public class DescriptionModuleViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptionModuleViewModel"/> class.
        /// </summary>
        /// <param name="moduleName">
        /// The module name.
        /// </param>
        public DescriptionModuleViewModel(string moduleName)
        {
            switch (moduleName)
            {
                case "Funding":
                    this._customDescriptionViewModel = new CustomDescription
                                                      {
                                                          WidthBox = 670,
                                                          HeightBox = 400,
                                                          WidthImage = 100,
                                                          Header = "Funding",
                                                          SourceImage = @"pack://application:,,,/Insyston.Operations.WPF.Views.Common;component\Images\Funding.png",
                                                          ContentModule = "Batch allocate contracts for Tranche level securitisation funding. Filter contracts by batches using flexible contract level parameters.",
                                                          BorderThicknessValue = 0,
                                                          HeaderStyle = (Style)Application.Current.FindResource("Header2"),
                                                          ContentModuleStyle = (Style)Application.Current.FindResource("ContentModule2"),
                                                          BulletTextList = new ObservableCollection<string> { "Tranche creation", "Bulk update of contract Funding records in Operations Classic", "Reconciliation reporting" },
                                                          ContentDetails = "Our Funding module provides the core functionality required for asset finance securitisation: ",
                                                      };
                    break;
                case "AssetCollateralClasses":
                    this._customDescriptionViewModel = new CustomDescription
                                                      {
                                                          WidthBox = 670,
                                                          HeightBox = 400,
                                                          WidthImage = 100,
                                                          Header = "PPSR",
                                                          SourceImage = @"pack://application:,,,/Insyston.Operations.WPF.Views.Common;component\Images\PPSR.png",
                                                          ContentModule = "Establish automation rules for the Registrtion of interest and automated Discharge at Termination, via a direct interface to the Government’s PPSR.",
                                                          BorderThicknessValue = 0,
                                                          HeaderStyle = (Style)Application.Current.FindResource("Header2"),
                                                          ContentModuleStyle = (Style)Application.Current.FindResource("ContentModule2"),
                                                          BulletTextList = new ObservableCollection<string> 
                                                          { 
                                                              "Ability for user to associate relevant PPSR data to equipment types", 
                                                              "Generate registration at Quote or Contract level and as option during Settlement", 
                                                              "Manualy discharge via user interface or automated discharge as part of Natural Teminations" 
                                                          },
                                                          ContentDetails = "Our PPSR interface module allows you to register and discharge your interests directly for our solution: ",
                                                      };
                    break;
                 case "Collection":
                    this._customDescriptionViewModel = new CustomDescription
                                                      {
                                                          WidthBox = 670,
                                                          HeightBox = 400,
                                                          WidthImage = 100,
                                                          Header = "Collection",
                                                          SourceImage = @"pack://application:,,,/Insyston.Operations.WPF.Views.Common;component\Images\Collections.png",
                                                          ContentModule = "Manage the allocation of collection related queues and tasks. Record collection activity and associated notes, which synchronise with Insyston Operations Classic.",
                                                          BorderThicknessValue = 0,
                                                          HeaderStyle = (Style)Application.Current.FindResource("Header2"),
                                                          ContentModuleStyle = (Style)Application.Current.FindResource("ContentModule2"),
                                                          BulletTextList = new ObservableCollection<string> 
                                                          { 
                                                              "Permissions based allocation of collection queues", 
                                                              "Record activity against each collection entry", 
                                                              "Synchronise your activity notes with Insyston Operations Classic providing the business with up-to-date information." 
                                                          },
                                                          ContentDetails = "Provide your Collection team with the appropriate tools to efficiently maintain their activities: ",
                                                      };
                    break;
                case "RegisteredAsset":
                    this._customDescriptionViewModel = new CustomDescription
                    {
                        WidthBox = 670,
                        HeightBox = 400,
                        WidthImage = 100,
                        Header = "Asset Register",
                        SourceImage = @"pack://application:,,,/Insyston.Operations.WPF.Views.Common;component\Images\AssetRegister.png",
                        ContentModule = "Track assets that you own or have an interest in, including identifiers, depreciation and acquisition. Assets are available for selection on quotes.",
                        BorderThicknessValue = 0,
                        HeaderStyle = (Style)Application.Current.FindResource("Header2"),
                        ContentModuleStyle = (Style)Application.Current.FindResource("ContentModule2"),
                        BulletTextList = new ObservableCollection<string> 
                                                          { 
                                                              "Permissions based allocation of collection queues", 
                                                              "Record activity against each collection entry", 
                                                              "Synchronise your activity notes with Insyston Operations Classic providing the business with up-to-date information." 
                                                          },
                        ContentDetails = "Provide your Collection team with the appropriate tools to efficiently maintain their activities: ",
                    };
                    break;
            }
        }

        /// <summary>
        /// The _custom description view model.
        /// </summary>
        private CustomDescription _customDescriptionViewModel;

        /// <summary>
        /// Gets or sets the custom description view model.
        /// </summary>
        public CustomDescription CustomDescriptionViewModel
        {
            get
            {
                return _customDescriptionViewModel;
            }

            set
            {
                this.SetField(ref _customDescriptionViewModel, value, () => CustomDescriptionViewModel);
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
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task<bool> LockAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
