// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetTypeDetailViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetTypeDetailViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.AssetType
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Bussiness.Assets.AssetClasses;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.WPF.ViewModels.Assets.Validation.AssetClassesTypeValidate;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The asset type detail view model.
    /// </summary>
    public class AssetTypeDetailViewModel : ViewModelUseCaseBase
    {
        #region Private Properties
        /// <summary>
        /// The _type name.
        /// </summary>
        private string _typeName;

        /// <summary>
        /// The _is type enable.
        /// </summary>
        private bool _isTypeEnable;

        /// <summary>
        /// The _is collateral enable.
        /// </summary>
        private bool _isCollateralVisible;

        /// <summary>
        /// The _list collateral class items.
        /// </summary>
        private List<AssetClassesCategoryItemDetail> _listCollateralClassItems;

        /// <summary>
        /// The _selected collateral class item.
        /// </summary>
        private AssetClassesCategoryItemDetail _selectedCollateralClassItem;

        /// <summary>
        /// The _detail tab view model.
        /// </summary>
        private ObservableCollection<ItemDepreciationDetailViewModel> _detailTabViewModel;

        /// <summary>
        /// The _item book view model.
        /// </summary>
        private ItemChildViewModel _itemBookViewModel;

        /// <summary>
        /// The _item tax view model.
        /// </summary>
        private ItemChildViewModel _itemTaxViewModel;

        /// <summary>
        /// The is tax content changed.
        /// </summary>
        private bool IsTaxContentChanged = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetTypeDetailViewModel"/> class.
        /// </summary>
        public AssetTypeDetailViewModel()
        {
            this._isCollateralVisible = false;
            this.Validator = new TypeDetailValidation();
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// The content item changed.
        /// </summary>
        public Action<object, PropertyChangedEventArgs> ContentItemChanged;

        /// <summary>
        /// Gets or sets the old collateral class id.
        /// </summary>
        public int OldCollateralClassId { get; set; }

        /// <summary>
        /// Gets or sets the type name.
        /// </summary>
        public string TypeName
        {
            get
            {
                return this._typeName;
            }

            set
            {
                this.SetField(ref this._typeName, value, () => this.TypeName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is type enable.
        /// </summary>
        public bool IsTypeEnable
        {
            get
            {
                return this._isTypeEnable;
            }

            set
            {
                this.SetField(ref this._isTypeEnable, value, () => this.IsTypeEnable);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is collateral enable.
        /// </summary>
        public bool IsCollateralVisible
        {
            get
            {
                return this._isCollateralVisible;
            }

            set
            {
                this.SetField(ref this._isCollateralVisible, value, () => this.IsCollateralVisible);
            }
        }

        /// <summary>
        /// Gets or sets the list collateral class items.
        /// </summary>
        public List<AssetClassesCategoryItemDetail> ListCollateralClassItems
        {
            get
            {
                return this._listCollateralClassItems;
            }

            set
            {
                this.SetField(ref this._listCollateralClassItems, value, () => this.ListCollateralClassItems);
            }
        }

        /// <summary>
        /// Gets or sets the selected collateral class item.
        /// </summary>
        public AssetClassesCategoryItemDetail SelectedCollateralClassItem
        {
            get
            {
                return this._selectedCollateralClassItem;
            }

            set
            {
                this.SetField(ref this._selectedCollateralClassItem, value, () => this.SelectedCollateralClassItem);
            }
        }

        /// <summary>
        /// Gets or sets the detail tab view model.
        /// </summary>
        public ObservableCollection<ItemDepreciationDetailViewModel> DetailTabViewModel
        {
            get
            {
                return this._detailTabViewModel;
            }

            set
            {
                this.SetField(ref this._detailTabViewModel, value, () => this.DetailTabViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the item book view model.
        /// </summary>
        public ItemChildViewModel ItemBookViewModel
        {
            get
            {
                return this._itemBookViewModel;
            }

            set
            {
                this.SetField(ref this._itemBookViewModel, value, () => this.ItemBookViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the item tax view model.
        /// </summary>
        public ItemChildViewModel ItemTaxViewModel
        {
            get
            {
                return this._itemTaxViewModel;
            }

            set
            {
                this.SetField(ref this._itemTaxViewModel, value, () => this.ItemTaxViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the id selected validate name.
        /// Id selected for validate name
        /// </summary>
        public int IdSelectedValidateName { get; set; }

        #endregion

        #region Public Methods
        /// <summary>
        /// The generate user control for detail screen.
        /// Load user control for detail screen
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GenerateUserControlForDetailScreen()
        {
            this.DetailTabViewModel = new ObservableCollection<ItemDepreciationDetailViewModel>();
            var itemChild = new List<ItemChildType>
                                {
                                    new ItemChildType { TypeItem = "ItemUseDefault" },
                                    new ItemChildType { TypeItem = "ItemDepnMethod" },
                                    new ItemChildType { TypeItem = "ItemSalvage" },
                                    new ItemChildType { TypeItem = "ItemEffectiveLife" },
                                };

            var bookDepn = new ItemDepreciationDetailViewModel
            {
                Header = "Book",
                ListItemChild = itemChild,
            };
            var taxDepn = new ItemDepreciationDetailViewModel
            {
                Header = "Tax",
                ListItemChild = itemChild,
            };

            this.DetailTabViewModel.Add(bookDepn);
            this.DetailTabViewModel.Add(taxDepn);
        }

        /// <summary>
        /// The get detail data source.
        /// </summary>
        /// <param name="itemSelectedId">
        /// The item selected id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetDetailDataSource(int itemSelectedId)
        {
            // Check PPSR key for Collateral Class
            if (await Operations.Security.Authorisation.IsModuleInstalledAsync(Modules.PPSR))
            {
                this.IsCollateralVisible = true;
            }
            else
            {
                this.IsCollateralVisible = false;
            }

            // Get Id selected for validate name
            this.IdSelectedValidateName = itemSelectedId;

            // Get list Collateral Classes
            this.ListCollateralClassItems = await AssetClassesTypeFunctions.GetCollateralClassItems();
            this.ListCollateralClassItems.Insert(
                0,
                new AssetClassesCategoryItemDetail
                    {
                        Text = "<None>",
                        ItemId = -1,
                    });

            AssetClassesTypeRowItem itemSelected;
            if (itemSelectedId != 0)
            {
                itemSelected = await AssetClassesTypeFunctions.GetDataDetailItemSelected(itemSelectedId);
            }
            else
            {
                itemSelected = await AssetClassesTypeFunctions.GetDefaultDataForDetail();
            }

            if (itemSelected != null)
            {
                this.TypeName = itemSelected.TypeDescription;
                this.IsTypeEnable = itemSelected.Enabled;

                this.SelectedCollateralClassItem = this.ListCollateralClassItems.Any(x => x.ItemId == itemSelected.CollateralClassId) ?
                    this.ListCollateralClassItems.FirstOrDefault(x => x.ItemId == itemSelected.CollateralClassId) : this.ListCollateralClassItems.FirstOrDefault(x => x.ItemId == -1);
                var assetClassesCategoryItemDetail = this.SelectedCollateralClassItem;
                if (assetClassesCategoryItemDetail != null)
                {
                    this.OldCollateralClassId = assetClassesCategoryItemDetail.ItemId;
                }

                List<AssetClassesCategoryItemDetail> listBookItemComboBox =
                    await AssetClassesTypeFunctions.GetListBookDepnMethod();
                listBookItemComboBox.Insert(
                    0,
                    new AssetClassesCategoryItemDetail
                        {
                            Text = "<None>",
                            ItemId = -1,
                        });
                this.ItemBookViewModel = new ItemChildViewModel
                {
                    Header = "Book",
                    HasSalvage = true,
                    IsUseCategoryDefault = itemSelected.BookDepnUseCategoryDefaults,
                    ListItemDepnMethod = listBookItemComboBox,
                    SelectedItemCombobox = listBookItemComboBox.FirstOrDefault(x => x.ItemId == itemSelected.BookDepnMethodId) != null ? listBookItemComboBox.FirstOrDefault(x => x.ItemId == itemSelected.BookDepnMethodId) : listBookItemComboBox.FirstOrDefault(x => x.ItemId == -1),
                    SalvagePercent = itemSelected.SalvagePercent,
                    EffectiveMonth = itemSelected.BookDepnLife,
                    EffectiveYear = itemSelected.BookDepnLifeYear,
                    DepnRatePercent = itemSelected.BookDepnPercent,
                };
                if (itemSelected.BookDepnEffectiveLifeOption == 1)
                {
                    this.ItemBookViewModel.IsConTractTermActive = true;
                }
                else if (itemSelected.BookDepnEffectiveLifeOption == 2)
                {
                    this.ItemBookViewModel.IsDenpRateActive = true;
                }
                else if (itemSelected.BookDepnEffectiveLifeOption == 3)
                {
                    this.ItemBookViewModel.IsEffectiveLifeActive = true;
                }

                List<AssetClassesCategoryItemDetail> listTaxItemComboBox =
                    await AssetClassesTypeFunctions.GetListTaxDepnMethod();
                listTaxItemComboBox.Insert(
                    0,
                    new AssetClassesCategoryItemDetail
                        {
                            Text = "<None>",
                            ItemId = -1,
                        });
                this.ItemTaxViewModel = new ItemChildViewModel
                {
                    Header = "Tax",
                    HasSalvage = false,
                    IsUseCategoryDefault = itemSelected.TaxDepnUseCategoryDefaults,
                    ListItemDepnMethod = listTaxItemComboBox,
                    SelectedItemCombobox = listTaxItemComboBox.FirstOrDefault(x => x.ItemId == itemSelected.TaxDepnMethodId) != null ? listTaxItemComboBox.FirstOrDefault(x => x.ItemId == itemSelected.TaxDepnMethodId) : listTaxItemComboBox.FirstOrDefault(x => x.ItemId == -1),
                    SalvagePercent = itemSelected.SalvagePercent,
                    EffectiveMonth = itemSelected.TaxDepnLife,
                    EffectiveYear = itemSelected.TaxDepnLifeYear,
                    DepnRatePercent = itemSelected.TaxDepnPercent,
                };
                if (itemSelected.TaxDepnEffectiveLifeOption == 1)
                {
                    this.ItemTaxViewModel.IsConTractTermActive = true;
                }
                else if (itemSelected.TaxDepnEffectiveLifeOption == 2)
                {
                    this.ItemTaxViewModel.IsDenpRateActive = true;
                }
                else if (itemSelected.TaxDepnEffectiveLifeOption == 3)
                {
                    this.ItemTaxViewModel.IsEffectiveLifeActive = true;
                }

                if (this.DetailTabViewModel != null)
                {
                    var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                    if (itemDepreciationDetailViewModel != null)
                    {
                        itemDepreciationDetailViewModel.ItemChildViewMdoel = this.ItemBookViewModel;
                    }

                    var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (depreciationDetailViewModel != null)
                    {
                        depreciationDetailViewModel.ItemChildViewMdoel = this.ItemTaxViewModel;
                    }
                }
            }

            this.PropertyChanged += this.AssetCategoryDetailViewModel_PropertyChanged;
        }

        /// <summary>
        /// The validate salvage percent empty.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateSalvagePercentEmpty()
        {
            ItemDepreciationDetailViewModel item = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));

            if (item != null && (item.ItemChildViewMdoel.SalvagePercent == null && item.ItemChildViewMdoel.IsUseCategoryDefault == false))
            {
                var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                if (itemDepreciationDetailViewModel != null)
                {
                    itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("SalvagePercent", "Salvage is required.");
                }

                return false;
            }

            var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
            if (depreciationDetailViewModel != null)
            {
                depreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("SalvagePercent");
            }

            return true;
        }

        /// <summary>
        /// The validate salvage percent.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateSalvagePercent()
        {
            ItemDepreciationDetailViewModel item = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));

            if (item != null && (item.ItemChildViewMdoel.SalvagePercent != null && item.ItemChildViewMdoel.IsUseCategoryDefault == false))
            {
                if (item.ItemChildViewMdoel.SalvagePercent < 0)
                {
                    var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                    if (itemDepreciationDetailViewModel != null)
                    {
                        itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("SalvagePercent", "Salvage is invalid.");
                    }

                    return false;
                }

                var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                if (depreciationDetailViewModel != null)
                {
                    depreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("SalvagePercent");
                }
            }

            return true;
        }

        /// <summary>
        /// The validate rate percent.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateRatePercent()
        {
            ItemDepreciationDetailViewModel item = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));

            if (item != null && (item.ItemChildViewMdoel.DepnRatePercent != null && item.ItemChildViewMdoel.IsUseCategoryDefault == false))
            {
                if (item.ItemChildViewMdoel.IsDenpRateActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId == 117 && item.ItemChildViewMdoel.DepnRatePercent >= 50)
                {
                    var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (itemDepreciationDetailViewModel != null)
                    {
                        itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("DepnRatePercent", "Rate must be < 50% for Diminishing Value Method.");
                    }

                    return false;
                }

                var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                if (depreciationDetailViewModel != null)
                {
                    depreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("DepnRatePercent");
                }
            }

            return true;
        }

        /// <summary>
        /// The validate book rate percent empty.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateBookRatePercentEmpty()
        {
            ItemDepreciationDetailViewModel item = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));

            if (item != null && (item.ItemChildViewMdoel.IsUseCategoryDefault == false && item.ItemChildViewMdoel.IsDenpRateActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1 && item.ItemChildViewMdoel.DepnRatePercent == null))
            {
                var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                if (itemDepreciationDetailViewModel != null)
                {
                    itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("DepnRatePercent", "Book Rate is required.");
                }

                return false;
            }

            var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
            if (depreciationDetailViewModel != null)
            {
                depreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("DepnRatePercent");
            }

            return true;
        }

        /// <summary>
        /// The validate tax rate percent empty.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateTaxRatePercentEmpty()
        {
            ItemDepreciationDetailViewModel item = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));

            if (item != null && (item.ItemChildViewMdoel.IsDenpRateActive && item.ItemChildViewMdoel.IsUseCategoryDefault == false && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1 && item.ItemChildViewMdoel.DepnRatePercent == null))
            {
                var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                if (itemDepreciationDetailViewModel != null)
                {
                    itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("DepnRatePercent", "Tax Rate is required.");
                }

                return false;
            }

            var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
            if (depreciationDetailViewModel != null)
            {
                depreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("DepnRatePercent");
            }

            return true;
        }

        /// <summary>
        /// The validate effective life.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateTaxEffectiveLife()
        {
            ItemDepreciationDetailViewModel item = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));

            if (item != null && (item.ItemChildViewMdoel.EffectiveMonth != null || item.ItemChildViewMdoel.EffectiveYear != null))
            {
                if (item.ItemChildViewMdoel.IsEffectiveLifeActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId == 117 && ((double)item.ItemChildViewMdoel.EffectiveYear + (double)item.ItemChildViewMdoel.EffectiveMonth / 12) <= 2)
                {
                    var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (itemDepreciationDetailViewModel != null)
                    {
                        itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveYear", "Effective Life must be > 2 years for Diminishing Value Method.");
                    }

                    var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (depreciationDetailViewModel != null)
                    {
                        depreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveMonth", "Effective Life must be > 2 years for Diminishing Value Method.");
                    }

                    return false;
                }

                if ((item.ItemChildViewMdoel.EffectiveMonth == 0 && item.ItemChildViewMdoel.EffectiveYear == 0)
                    || (item.ItemChildViewMdoel.EffectiveMonth == null && item.ItemChildViewMdoel.EffectiveYear == 0)
                    || (item.ItemChildViewMdoel.EffectiveMonth == 0 && item.ItemChildViewMdoel.EffectiveYear == null))
                {
                    var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (itemDepreciationDetailViewModel != null)
                    {
                        itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveYear", "Tax Effective Life must be greater than 0.");
                    }

                    var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (depreciationDetailViewModel != null)
                    {
                        depreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveMonth", "Tax Effective Life must be greater than 0.");
                    }

                    return false;
                }

                if (!this.IsTaxContentChanged)
                {
                    var detailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (detailViewModel != null)
                    {
                        detailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveYear");
                    }

                    var firstOrDefault = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (firstOrDefault != null)
                    {
                        firstOrDefault.ItemChildViewMdoel.RemoveNotifyError("EffectiveMonth");
                    }
                }
            }
            else
            {
                if (item != null
                    && (item.ItemChildViewMdoel.IsEffectiveLifeActive
                        && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1
                        && item.ItemChildViewMdoel.EffectiveYear == null
                        && item.ItemChildViewMdoel.EffectiveMonth == null))
                {
                    return true;
                }

                var detailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                if (detailViewModel != null)
                {
                    detailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveYear");
                }

                var firstOrDefault = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                if (firstOrDefault != null)
                {
                    firstOrDefault.ItemChildViewMdoel.RemoveNotifyError("EffectiveMonth");
                }
            }

            return true;
        }

        /// <summary>
        /// The validate tax effective life valid.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateTaxEffectiveLifeValid()
        {
            ItemDepreciationDetailViewModel item = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));

            if (item != null && (item.ItemChildViewMdoel.EffectiveMonth != null || item.ItemChildViewMdoel.EffectiveYear != null))
            {
                if (item.ItemChildViewMdoel.IsEffectiveLifeActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1)
                {
                    if ((item.ItemChildViewMdoel.EffectiveMonth == 0 && item.ItemChildViewMdoel.EffectiveYear == 0)
                        || (item.ItemChildViewMdoel.EffectiveMonth == null && item.ItemChildViewMdoel.EffectiveYear == 0)
                        || (item.ItemChildViewMdoel.EffectiveMonth == 0 && item.ItemChildViewMdoel.EffectiveYear == null))
                    {
                        var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                        if (itemDepreciationDetailViewModel != null)
                        {
                            itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveYear", "Tax Effective Life must be greater than 0.");
                        }

                        var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                        if (depreciationDetailViewModel != null)
                        {
                            depreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveMonth", "Tax Effective Life must be greater than 0.");
                        }

                        return false;
                    }
                }
                else
                {
                    var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (itemDepreciationDetailViewModel != null)
                    {
                        itemDepreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveYear");
                    }

                    var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (depreciationDetailViewModel != null)
                    {
                        depreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveMonth");
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The validate book effective life valid.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateBookEffectiveLifeValid()
        {
            ItemDepreciationDetailViewModel item = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));

            if (item != null && (item.ItemChildViewMdoel.EffectiveMonth != null || item.ItemChildViewMdoel.EffectiveYear != null))
            {
                if (item.ItemChildViewMdoel.IsEffectiveLifeActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1)
                {
                    if ((item.ItemChildViewMdoel.EffectiveMonth == 0 && item.ItemChildViewMdoel.EffectiveYear == 0)
                        || (item.ItemChildViewMdoel.EffectiveMonth == null && item.ItemChildViewMdoel.EffectiveYear == 0)
                        || (item.ItemChildViewMdoel.EffectiveMonth == 0 && item.ItemChildViewMdoel.EffectiveYear == null))
                    {
                        var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                        if (itemDepreciationDetailViewModel != null)
                        {
                            itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveYear", "Book Effective Life must be greater than 0.");
                        }

                        var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                        if (depreciationDetailViewModel != null)
                        {
                            depreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveMonth", "Book Effective Life must be greater than 0.");
                        }

                        return false;
                    }
                }
                else
                {
                    var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                    if (itemDepreciationDetailViewModel != null)
                    {
                        itemDepreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveYear");
                    }

                    var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                    if (depreciationDetailViewModel != null)
                    {
                        depreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveMonth");
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The validate book effective life empty.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateBookEffectiveLifeEmpty()
        {
            ItemDepreciationDetailViewModel item = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));

            if (item != null && (item.ItemChildViewMdoel.IsEffectiveLifeActive && item.ItemChildViewMdoel.IsUseCategoryDefault == false && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1 && item.ItemChildViewMdoel.EffectiveYear == null && item.ItemChildViewMdoel.EffectiveMonth == null))
            {
                var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                if (itemDepreciationDetailViewModel != null)
                {
                    itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveYear", "Book Effective Life is required.");
                }

                var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                if (depreciationDetailViewModel != null)
                {
                    depreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveMonth", "Book Effective Life is required.");
                }

                return false;
            }

            var detailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
            if (detailViewModel != null)
            {
                detailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveYear");
            }

            var firstOrDefault = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
            if (firstOrDefault != null)
            {
                firstOrDefault.ItemChildViewMdoel.RemoveNotifyError("EffectiveMonth");
            }

            return true;
        }

        /// <summary>
        /// The validate tax effective life empty.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateTaxEffectiveLifeEmpty()
        {
            ItemDepreciationDetailViewModel item = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));

            if (item != null && (item.ItemChildViewMdoel.IsEffectiveLifeActive && item.ItemChildViewMdoel.IsUseCategoryDefault == false && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1 && item.ItemChildViewMdoel.EffectiveYear == null && item.ItemChildViewMdoel.EffectiveMonth == null))
            {
                var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                if (itemDepreciationDetailViewModel != null)
                {
                    itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveYear", "Tax Effective Life is required.");
                }

                var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                if (depreciationDetailViewModel != null)
                {
                    depreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveMonth", "Tax Effective Life is required.");
                }

                return false;
            }

            var detailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
            if (detailViewModel != null)
            {
                detailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveYear");
            }

            var firstOrDefault = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
            if (firstOrDefault != null)
            {
                firstOrDefault.ItemChildViewMdoel.RemoveNotifyError("EffectiveMonth");
            }

            return true;
        }

        /// <summary>
        /// The asset category detail view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void AssetCategoryDetailViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ContentItemChanged != null && (e.PropertyName.IndexOf("ListErrorHyperlink", StringComparison.Ordinal) == -1)
                && (e.PropertyName.IndexOf("ValidationSummary", StringComparison.Ordinal) == -1))
            {
                if (this.IsCheckedOut)
                {
                    // Set value of Salvage in BookViewModel to TaxViewModel
                    if (e.PropertyName.IndexOf("ItemBookViewModel.SalvagePercent", StringComparison.Ordinal) != -1)
                    {
                        var taxDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                        if (taxDetailViewModel != null)
                        {
                            var bookDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                            if (bookDetailViewModel != null)
                            {
                                taxDetailViewModel.ItemChildViewMdoel.SalvagePercent = bookDetailViewModel.ItemChildViewMdoel.SalvagePercent;
                            }
                        }
                    }
                    else if (e.PropertyName.IndexOf("ItemBookViewModel.IsUseCategoryDefault", StringComparison.Ordinal) != -1)
                    {
                        this.SetUseCategoryDefault("Book");
                    }
                    else if (e.PropertyName.IndexOf("ItemTaxViewModel.IsUseCategoryDefault", StringComparison.Ordinal) != -1)
                    {
                        this.SetUseCategoryDefault("Tax");
                    }

                    if (e.PropertyName.IndexOf("ItemTaxViewModel.EffectiveYear", StringComparison.Ordinal) != -1 || e.PropertyName.IndexOf("ItemTaxViewModel.EffectiveMonth", StringComparison.Ordinal) != -1)
                    {
                        ItemDepreciationDetailViewModel item = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                        if (item != null && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != 117)
                        {
                            this.IsTaxContentChanged = true;
                        }

                        if ((item != null && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != 118 && !item.ItemChildViewMdoel.IsDenpRateActive) && !this.ValidateTaxEffectiveLife())
                        {
                            this.ListErrorHyperlink = new List<CustomHyperlink>();
                            this.ListErrorHyperlink.Add(new CustomHyperlink
                                                            {
                                                                Action = HyperLinkAction.AssetClassesTypeDetailState,
                                                                SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton"),
                                                                HyperlinkHeader = "Effective Life must be > 2 years for Diminishing Value Method.",
                                                            });
                        }
                        else
                        {
                            if (this.ListErrorHyperlink.Count > 0)
                            {
                                this.ListErrorHyperlink = new List<CustomHyperlink>();
                            }
                        }

                        this.IsTaxContentChanged = false;
                    }

                    this.ContentItemChanged(sender, e);
                }
            }
        }

        /// <summary>
        /// The set use category default.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        public async void SetUseCategoryDefault(string header)
        {
            if (header.Equals("Book"))
            {
                ItemDepreciationDetailViewModel item = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                if (item != null)
                {
                    if (item.ItemChildViewMdoel.IsUseCategoryDefault)
                    {
                        AssetClassesCategoryRowItem itemDefault = await AssetClassesCategoryFunctions.GetDefaultDataForDetail();
                        item.ItemChildViewMdoel.SetValueDefaultForAll(
                            itemDefault.BookDepnMethodId,
                            itemDefault.SalvagePercent,
                            itemDefault.BookDepnLife,
                            itemDefault.BookDepnLifeYear,
                            itemDefault.BookDepnPercent);
                    }
                }
            }
            else if (header.Equals("Tax"))
            {
                ItemDepreciationDetailViewModel item = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                if (item != null)
                {
                    if (item.ItemChildViewMdoel.IsUseCategoryDefault)
                    {
                        AssetClassesCategoryRowItem itemDefault = await AssetClassesCategoryFunctions.GetDefaultDataForDetail();
                        item.ItemChildViewMdoel.SetValueDefaultForAll(
                            itemDefault.TaxDepnMethodId,
                            itemDefault.SalvagePercent,
                            itemDefault.TaxDepnLife,
                            itemDefault.TaxDepnLifeYear,
                            itemDefault.TaxDepnPercent);
                    }
                }
            }
        }

        #endregion

        #region Internal Methods
        /// <summary>
        /// The parse type item detail to save.
        /// </summary>
        /// <param name="itemSelectedId">
        /// The item selected id.
        /// </param>
        /// <returns>
        /// The <see cref="AssetClassesTypeRowItem"/>.
        /// </returns>
        internal AssetClassesTypeRowItem ParseTypeItemDetailToSave(int itemSelectedId)
        {
            var result = new AssetClassesTypeRowItem
                             {
                                 EquipTypeId = itemSelectedId,
                                 TypeDescription = this.TypeName,
                                 CollateralClassId = this.SelectedCollateralClassItem.ItemId,
                                 OldCollateralClassId = this.OldCollateralClassId,
                                 Enabled = this.IsTypeEnable
                             };
            if (this.DetailTabViewModel != null)
            {
                foreach (var item in this.DetailTabViewModel)
                {
                    if (item.Header.Equals("Book"))
                    {
                        result.BookDepnUseCategoryDefaults = item.ItemChildViewMdoel.IsUseCategoryDefault;
                        result.BookDepnMethodId = item.ItemChildViewMdoel.SelectedItemCombobox.ItemId;
                        if (item.ItemChildViewMdoel.SalvagePercent != null)
                        {
                            result.SalvagePercent = item.ItemChildViewMdoel.SalvagePercent.Value;
                        }

                        result.BookDepnLife = item.ItemChildViewMdoel.EffectiveMonth;
                        result.BookDepnLifeYear = item.ItemChildViewMdoel.EffectiveYear;
                        result.BookDepnPercent = item.ItemChildViewMdoel.DepnRatePercent;

                        if (item.ItemChildViewMdoel.IsConTractTermActive)
                        {
                            result.BookDepnEffectiveLifeOption = 1;
                        }
                        else if (item.ItemChildViewMdoel.IsDenpRateActive)
                        {
                            result.BookDepnEffectiveLifeOption = 2;
                        }
                        else if (item.ItemChildViewMdoel.IsEffectiveLifeActive)
                        {
                            result.BookDepnEffectiveLifeOption = 3;
                        }
                    }

                    if (item.Header.Equals("Tax"))
                    {
                        result.TaxDepnUseCategoryDefaults = item.ItemChildViewMdoel.IsUseCategoryDefault;
                        result.TaxDepnMethodId = item.ItemChildViewMdoel.SelectedItemCombobox.ItemId;
                        result.TaxDepnLife = item.ItemChildViewMdoel.EffectiveMonth;
                        result.TaxDepnLifeYear = item.ItemChildViewMdoel.EffectiveYear;
                        result.TaxDepnPercent = item.ItemChildViewMdoel.DepnRatePercent;

                        if (item.ItemChildViewMdoel.IsConTractTermActive)
                        {
                            result.TaxDepnEffectiveLifeOption = 1;
                        }
                        else if (item.ItemChildViewMdoel.IsDenpRateActive)
                        {
                            result.TaxDepnEffectiveLifeOption = 2;
                        }
                        else if (item.ItemChildViewMdoel.IsEffectiveLifeActive)
                        {
                            result.TaxDepnEffectiveLifeOption = 3;
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region Protected Methods
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
        #endregion
    }
}
