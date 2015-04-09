// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetCategoryDetailViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetCategoryDetailViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.AssetCategory
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Bussiness.Assets.AssetClasses;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.WPF.ViewModels.Assets.Validation.AssetClassesCategoryControlValidate;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The asset category detail view model.
    /// </summary>
    public class AssetCategoryDetailViewModel : ViewModelUseCaseBase
    {
        #region Private Properties
        /// <summary>
        /// The category name.
        /// </summary>
        private string _categoryName;

        /// <summary>
        /// The _is category enable.
        /// </summary>
        private bool _isCategoryEnable;

        /// <summary>
        /// The detail tab view model.
        /// </summary>
        private ObservableCollection<ItemDepreciationDetailViewModel> _detailTabViewModel;

        /// <summary>
        /// The item book view model.
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
        /// Initializes a new instance of the <see cref="AssetCategoryDetailViewModel"/> class.
        /// </summary>
        public AssetCategoryDetailViewModel()
        {
            this.Validator = new CategoryDetailValidation();
            this.PropertyChanged += this.AssetCategoryDetailViewModel_PropertyChanged;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The content item changed.
        /// </summary>
        public Action<object, PropertyChangedEventArgs> ContentItemChanged;

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        public string CategoryName
        {
            get
            {
                return this._categoryName;
            }

            set
            {
                this.SetField(ref this._categoryName, value, () => this.CategoryName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is category enable.
        /// </summary>
        public bool IsCategoryEnable
        {
            get
            {
                return this._isCategoryEnable;
            }

            set
            {
                this.SetField(ref this._isCategoryEnable, value, () => this.IsCategoryEnable);
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
        /// Populate data for screen Detail.
        /// </summary>
        /// <param name="itemSelectedId">
        /// The item selected id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetDetailDataSource(int itemSelectedId)
        {
            // Get Id selected for validate name
            this.IdSelectedValidateName = itemSelectedId;

            AssetClassesCategoryRowItem itemSelected;
            if (itemSelectedId != 0)
            {
                itemSelected = await AssetClassesCategoryFunctions.GetDataDetailItemSelected(itemSelectedId);
            }
            else
            {
                itemSelected = await AssetClassesCategoryFunctions.GetDefaultDataForDetail();
            }

            if (itemSelected != null)
            {
                this.CategoryName = itemSelected.Category;
                this.IsCategoryEnable = itemSelected.Enabled;

                List<AssetClassesCategoryItemDetail> listBookItemComboBox = await AssetClassesCategoryFunctions.GetListBookDepnMethod();
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
                    ListItemDepnMethod = listBookItemComboBox,
                    SelectedItemCombobox = listBookItemComboBox.FirstOrDefault(x => x.ItemId == itemSelected.BookDepnMethodId) != null ? listBookItemComboBox.FirstOrDefault(x => x.ItemId == itemSelected.BookDepnMethodId) : listBookItemComboBox.FirstOrDefault(x => x.ItemId == -1),
                    SalvagePercent = itemSelected.SalvagePercent,
                    EffectiveMonth = itemSelected.BookDepnLife,
                    EffectiveYear = itemSelected.BookDepnLifeYear,
                    DepnRatePercent = itemSelected.BookDepnPercent,
                };
                if (itemSelected.BookDepnEffectiveLifeOption == 753)
                {
                    this.ItemBookViewModel.IsConTractTermActive = true;
                }
                else if (itemSelected.BookDepnEffectiveLifeOption == 754)
                {
                    this.ItemBookViewModel.IsDenpRateActive = true;
                }
                else if (itemSelected.BookDepnEffectiveLifeOption == 755)
                {
                    this.ItemBookViewModel.IsEffectiveLifeActive = true;
                }

                List<AssetClassesCategoryItemDetail> listTaxItemComboBox = await AssetClassesCategoryFunctions.GetListTaxDepnMethod();
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
                    ListItemDepnMethod = listTaxItemComboBox,
                    SelectedItemCombobox = listTaxItemComboBox.FirstOrDefault(x => x.ItemId == itemSelected.TaxDepnMethodId) != null ? listTaxItemComboBox.FirstOrDefault(x => x.ItemId == itemSelected.TaxDepnMethodId) : listTaxItemComboBox.FirstOrDefault(x => x.ItemId == -1),
                    SalvagePercent = itemSelected.SalvagePercent,
                    EffectiveMonth = itemSelected.TaxDepnLife,
                    EffectiveYear = itemSelected.TaxDepnLifeYear,
                    DepnRatePercent = itemSelected.TaxDepnPercent,
                };
                if (itemSelected.TaxDepnEffectiveLifeOption == 753)
                {
                    this.ItemTaxViewModel.IsConTractTermActive = true;
                }
                else if (itemSelected.TaxDepnEffectiveLifeOption == 754)
                {
                    this.ItemTaxViewModel.IsDenpRateActive = true;
                }
                else if (itemSelected.TaxDepnEffectiveLifeOption == 755)
                {
                    this.ItemTaxViewModel.IsEffectiveLifeActive = true;
                }

                if (this.DetailTabViewModel != null)
                {
                    var itemBookDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                    if (itemBookDepreciationDetailViewModel != null)
                    {
                        itemBookDepreciationDetailViewModel.ItemChildViewMdoel = this.ItemBookViewModel;
                    }

                    var itemTaxDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (itemTaxDepreciationDetailViewModel != null)
                    {
                        itemTaxDepreciationDetailViewModel.ItemChildViewMdoel = this.ItemTaxViewModel;
                    }
                }
            }
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

            if (item != null && item.ItemChildViewMdoel.SalvagePercent == null)
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

            if (item != null && item.ItemChildViewMdoel.SalvagePercent < 0)
            {
                var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                if (depreciationDetailViewModel != null)
                {
                    depreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("SalvagePercent", "Salvage is invalid.");
                }

                return false;
            }

            var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
            if (itemDepreciationDetailViewModel != null)
            {
                itemDepreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("SalvagePercent");
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

            if (item != null && item.ItemChildViewMdoel.DepnRatePercent != null)
            {
                if (item.ItemChildViewMdoel.IsDenpRateActive
                    && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId == 117
                    && item.ItemChildViewMdoel.DepnRatePercent >= 50)
                {
                    var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (itemDepreciationDetailViewModel != null)
                    {
                        itemDepreciationDetailViewModel
                            .ItemChildViewMdoel.AddNotifyError("DepnRatePercent", "Rate must be < 50% for Diminishing Value Method.");
                    }

                    return false;
                }

                var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                if (depreciationDetailViewModel != null)
                {
                    depreciationDetailViewModel
                        .ItemChildViewMdoel.RemoveNotifyError("DepnRatePercent");
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

            if (item != null && (item.ItemChildViewMdoel.IsDenpRateActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1 && (item.ItemChildViewMdoel.DepnRatePercent == null || double.IsInfinity(item.ItemChildViewMdoel.DepnRatePercent.Value))))
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

            if (item != null && (item.ItemChildViewMdoel.IsDenpRateActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1 && item.ItemChildViewMdoel.DepnRatePercent == null))
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
                if (item.ItemChildViewMdoel.EffectiveMonth != null && (item.ItemChildViewMdoel.EffectiveYear != null && (item.ItemChildViewMdoel.IsEffectiveLifeActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId == 117 && ((double)item.ItemChildViewMdoel.EffectiveYear + (double)item.ItemChildViewMdoel.EffectiveMonth / 12) <= 2)))
                {
                    var itemDepreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (itemDepreciationDetailViewModel != null)
                    {
                        itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveYear", "Tax Effective Life must be > 2 years for Diminishing Value Method.");
                    }

                    var depreciationDetailViewModel = this.DetailTabViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (depreciationDetailViewModel != null)
                    {
                        depreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveMonth", "Tax Effective Life must be > 2 years for Diminishing Value Method.");
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

            if (item != null && (item.ItemChildViewMdoel.IsEffectiveLifeActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1 && item.ItemChildViewMdoel.EffectiveYear == null && item.ItemChildViewMdoel.EffectiveMonth == null))
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

            if (item != null && (item.ItemChildViewMdoel.IsEffectiveLifeActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1 && item.ItemChildViewMdoel.EffectiveYear == null && item.ItemChildViewMdoel.EffectiveMonth == null))
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
                                                                Action = HyperLinkAction.AssetClassesCategoryDetailState,
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
        #endregion

        #region Internal Methods
        /// <summary>
        /// The parse category item detail to save.
        /// </summary>
        /// <param name="itemSelectedId">
        /// The item selected id.
        /// </param>
        /// <returns>
        /// The <see cref="AssetClassesCategoryRowItem"/>.
        /// </returns>
        internal AssetClassesCategoryRowItem ParseCategoryItemDetailToSave(int itemSelectedId)
        {
            AssetClassesCategoryRowItem result = new AssetClassesCategoryRowItem();
            result.EquipCategoryId = itemSelectedId;
            result.Category = this.CategoryName;
            result.Enabled = this.IsCategoryEnable;
            if (this.DetailTabViewModel != null)
            {
                foreach (var item in this.DetailTabViewModel)
                {
                    if (item.Header.Equals("Book"))
                    {
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
                            result.BookDepnEffectiveLifeOption = 753;
                        }
                        else if (item.ItemChildViewMdoel.IsDenpRateActive)
                        {
                            result.BookDepnEffectiveLifeOption = 754;
                        }
                        else if (item.ItemChildViewMdoel.IsEffectiveLifeActive)
                        {
                            result.BookDepnEffectiveLifeOption = 755;
                        }
                    }

                    if (item.Header.Equals("Tax"))
                    {
                        result.TaxDepnMethodId = item.ItemChildViewMdoel.SelectedItemCombobox.ItemId;
                        result.TaxDepnLife = item.ItemChildViewMdoel.EffectiveMonth;
                        result.TaxDepnLifeYear = item.ItemChildViewMdoel.EffectiveYear;
                        result.TaxDepnPercent = item.ItemChildViewMdoel.DepnRatePercent;

                        if (item.ItemChildViewMdoel.IsConTractTermActive)
                        {
                            result.TaxDepnEffectiveLifeOption = 753;
                        }
                        else if (item.ItemChildViewMdoel.IsDenpRateActive)
                        {
                            result.TaxDepnEffectiveLifeOption = 754;
                        }
                        else if (item.ItemChildViewMdoel.IsEffectiveLifeActive)
                        {
                            result.TaxDepnEffectiveLifeOption = 755;
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
