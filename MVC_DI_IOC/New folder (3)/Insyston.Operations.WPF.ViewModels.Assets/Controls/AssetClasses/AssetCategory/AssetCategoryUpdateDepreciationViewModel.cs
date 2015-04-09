// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetCategoryUpdateDepreciationViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   The asset category update depreciation view model.
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
    /// The asset category update depreciation view model.
    /// </summary>
    public class AssetCategoryUpdateDepreciationViewModel : ViewModelUseCaseBase
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
        /// The item book view model.
        /// </summary>
        private ItemChildViewModel _itemBookViewModel;

        /// <summary>
        /// The _item tax view model.
        /// </summary>
        private ItemChildViewModel _itemTaxViewModel;

        /// <summary>
        /// The _update depreciation view model.
        /// </summary>
        private ObservableCollection<ItemDepreciationDetailViewModel> _updateDepreciationViewModel;

        /// <summary>
        /// The is tax content changed.
        /// </summary>
        private bool IsTaxContentChanged = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetCategoryUpdateDepreciationViewModel"/> class.
        /// </summary>
        public AssetCategoryUpdateDepreciationViewModel()
        {
            this.Validator = new AssetClassesCategoryUpdateDepreciationViewModelValidation();
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
        /// Gets or sets the update depreciation view model.
        /// </summary>
        public ObservableCollection<ItemDepreciationDetailViewModel> UpdateDepreciationViewModel
        {
            get
            {
                return this._updateDepreciationViewModel;
            }

            set
            {
                this.SetField(ref this._updateDepreciationViewModel, value, () => this.UpdateDepreciationViewModel);
            }
        }
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
            this.UpdateDepreciationViewModel = new ObservableCollection<ItemDepreciationDetailViewModel>();
            var itemChild = new List<ItemChildType>
                                {
                                    new ItemChildType { TypeItem = "ItemDepnMethod" },
                                    new ItemChildType { TypeItem = "ItemSalvage" },
                                    new ItemChildType { TypeItem = "ItemEffectiveLife" },
                                };

            ItemDepreciationDetailViewModel bookDepn = new ItemDepreciationDetailViewModel
            {
                Header = "Book",
                ListItemChild = itemChild,
            };
            ItemDepreciationDetailViewModel taxDepn = new ItemDepreciationDetailViewModel
            {
                Header = "Tax",
                ListItemChild = itemChild,
            };

            this.UpdateDepreciationViewModel.Add(bookDepn);
            this.UpdateDepreciationViewModel.Add(taxDepn);
        }

        /// <summary>
        /// The get update depreciation data source.
        /// </summary>
        /// <param name="categoryName">
        /// The category name.
        /// </param>
        /// <param name="itemSelected">
        /// The item selected.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetUpdateDepreciationDataSource(string categoryName, AssetClassesCategoryRowItem itemSelected)
        {
            if (itemSelected != null)
            {
                this.CategoryName = categoryName;
                this.IsCategoryEnable = itemSelected.Enabled;

                List<AssetClassesCategoryItemDetail> listBookItemComboBox;
                listBookItemComboBox = await AssetClassesCategoryFunctions.GetListBookDepnMethod();
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
                    IsCheckBoxAllShowUp = true,
                    HasSalvage = true,
                    IsConTractTermActive = true,
                    ListItemDepnMethod = listBookItemComboBox,
                    SelectedItemCombobox = listBookItemComboBox.FirstOrDefault(x => x.ItemId == itemSelected.BookDepnMethodId) == null ? listBookItemComboBox.FirstOrDefault() : listBookItemComboBox.FirstOrDefault(x => x.ItemId == itemSelected.BookDepnMethodId),
                    SalvagePercent = itemSelected.SalvagePercent,
                    EffectiveMonth = itemSelected.BookDepnLife,
                    EffectiveYear = itemSelected.BookDepnLifeYear,
                    DepnRatePercent = itemSelected.BookDepnPercent,
                    IsAllDepnMethodChecked = false,
                    IsAllEffectiveLifeChecked = false,
                    IsAllSalvageChecked = false,
                };

                List<AssetClassesCategoryItemDetail> listTaxItemComboBox;
                listTaxItemComboBox = await AssetClassesCategoryFunctions.GetListTaxDepnMethod();
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
                    IsCheckBoxAllShowUp = true,
                    HasSalvage = false,
                    IsConTractTermActive = true,
                    ListItemDepnMethod = listTaxItemComboBox,
                    SelectedItemCombobox = listTaxItemComboBox.FirstOrDefault(),
                    SalvagePercent = itemSelected.SalvagePercent,
                    EffectiveMonth = itemSelected.TaxDepnLife,
                    EffectiveYear = itemSelected.TaxDepnLifeYear,
                    DepnRatePercent = itemSelected.TaxDepnPercent,
                    IsAllDepnMethodChecked = false,
                    IsAllEffectiveLifeChecked = false,
                    IsAllSalvageChecked = false,
                };

                if (this.UpdateDepreciationViewModel != null)
                {
                    var itemBookDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                    if (itemBookDepreciationDetailViewModel != null)
                    {
                        itemBookDepreciationDetailViewModel.ItemChildViewMdoel = this.ItemBookViewModel;
                    }

                    var itemTaxDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (itemTaxDepreciationDetailViewModel != null)
                    {
                        itemTaxDepreciationDetailViewModel.ItemChildViewMdoel = this.ItemTaxViewModel;
                    }
                }
            }

            this.PropertyChanged += this.AssetCategoryUpdateDepreciationViewModel_PropertyChanged;
        }

        /// <summary>
        /// The validate salvage percent empty.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateSalvagePercentEmpty()
        {
            ItemDepreciationDetailViewModel item = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));

            if (item != null && item.ItemChildViewMdoel.SalvagePercent == null)
            {
                var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                if (itemDepreciationDetailViewModel != null)
                {
                    itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("SalvagePercent", "Salvage is invalid.");
                }

                return false;
            }

            var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
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
            ItemDepreciationDetailViewModel item = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));

            if (item != null && item.ItemChildViewMdoel.SalvagePercent < 0)
            {
                var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                if (itemDepreciationDetailViewModel != null)
                {
                    itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("SalvagePercent", "Salvage is invalid.");
                }

                return false;
            }

            var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
            if (depreciationDetailViewModel != null)
            {
                depreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("SalvagePercent");
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
            ItemDepreciationDetailViewModel item = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));

            if (item != null && (item.ItemChildViewMdoel.IsDenpRateActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId == 117 && item.ItemChildViewMdoel.DepnRatePercent >= 50))
            {
                var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                if (itemDepreciationDetailViewModel != null)
                {
                    itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("DepnRatePercent", "Rate must be < 50% for Diminishing Value Method.");
                }

                return false;
            }

            var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
            if (depreciationDetailViewModel != null)
            {
                depreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("DepnRatePercent");
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
            ItemDepreciationDetailViewModel item = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));

            if (item != null && (item.ItemChildViewMdoel.IsDenpRateActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1 && item.ItemChildViewMdoel.DepnRatePercent == null))
            {
                var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                if (itemDepreciationDetailViewModel != null)
                {
                    itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("DepnRatePercent", "Book Rate is required.");
                }

                return false;
            }

            var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
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
            ItemDepreciationDetailViewModel item = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));

            if (item != null && (item.ItemChildViewMdoel.IsDenpRateActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1 && item.ItemChildViewMdoel.DepnRatePercent == null))
            {
                var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                if (itemDepreciationDetailViewModel != null)
                {
                    itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("DepnRatePercent", "Tax Rate is required.");
                }

                return false;
            }

            var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
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
            ItemDepreciationDetailViewModel item = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));

            if (item != null && (item.ItemChildViewMdoel.EffectiveMonth != null || item.ItemChildViewMdoel.EffectiveYear != null))
            {
                if (item.ItemChildViewMdoel.IsEffectiveLifeActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId == 117 && ((double)item.ItemChildViewMdoel.EffectiveYear + (double)item.ItemChildViewMdoel.EffectiveMonth / 12) <= 2)
                {
                    var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (itemDepreciationDetailViewModel != null)
                    {
                        itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError(
                            "EffectiveYear", 
                            "Effective Life must be > 2 years for Diminishing Value Method.");
                    }

                    var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (depreciationDetailViewModel != null)
                    {
                        depreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError(
                            "EffectiveMonth", 
                            "Effective Life must be > 2 years for Diminishing Value Method.");
                    }

                    return false;
                }

                if (item.ItemChildViewMdoel.IsEffectiveLifeActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1)
                {
                    if ((item.ItemChildViewMdoel.EffectiveMonth == 0 && item.ItemChildViewMdoel.EffectiveYear == 0)
                        || (item.ItemChildViewMdoel.EffectiveMonth == null && item.ItemChildViewMdoel.EffectiveYear == 0)
                        || (item.ItemChildViewMdoel.EffectiveMonth == 0 && item.ItemChildViewMdoel.EffectiveYear == null))
                    {
                        var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                        if (depreciationDetailViewModel != null)
                        {
                            depreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError(
                                "EffectiveYear", 
                                "Tax Effective Life must be greater than 0.");
                        }

                        var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                        if (itemDepreciationDetailViewModel != null)
                        {
                            itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError(
                                "EffectiveMonth", 
                                "Tax Effective Life must be greater than 0.");
                        }

                        return false;
                    }
                }

                if (!this.IsTaxContentChanged)
                {
                    var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (itemDepreciationDetailViewModel != null)
                    {
                        itemDepreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveYear");
                    }

                    var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (depreciationDetailViewModel != null)
                    {
                        depreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveMonth");
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

                var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                if (itemDepreciationDetailViewModel != null)
                {
                    itemDepreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveYear");
                }

                var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                if (depreciationDetailViewModel != null)
                {
                    depreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveMonth");
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
            ItemDepreciationDetailViewModel item = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));

            if (item != null && (item.ItemChildViewMdoel.EffectiveMonth != null || item.ItemChildViewMdoel.EffectiveYear != null))
            {
                if (item.ItemChildViewMdoel.IsEffectiveLifeActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1)
                {
                    if ((item.ItemChildViewMdoel.EffectiveMonth == 0 && item.ItemChildViewMdoel.EffectiveYear == 0)
                        || (item.ItemChildViewMdoel.EffectiveMonth == null && item.ItemChildViewMdoel.EffectiveYear == 0)
                        || (item.ItemChildViewMdoel.EffectiveMonth == 0 && item.ItemChildViewMdoel.EffectiveYear == null))
                    {
                        var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                        if (itemDepreciationDetailViewModel != null)
                        {
                            itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveYear", "Tax Effective Life must be greater than 0.");
                        }

                        var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                        if (depreciationDetailViewModel != null)
                        {
                            depreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveMonth", "Tax Effective Life must be greater than 0.");
                        }

                        return false;
                    }
                }
                else
                {
                    var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                    if (itemDepreciationDetailViewModel != null)
                    {
                        itemDepreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveYear");
                    }

                    var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
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
            ItemDepreciationDetailViewModel item = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));

            if (item != null && (item.ItemChildViewMdoel.EffectiveMonth != null || item.ItemChildViewMdoel.EffectiveYear != null))
            {
                if (item.ItemChildViewMdoel.IsEffectiveLifeActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1)
                {
                    if ((item.ItemChildViewMdoel.EffectiveMonth == 0 && item.ItemChildViewMdoel.EffectiveYear == 0)
                        || (item.ItemChildViewMdoel.EffectiveMonth == null && item.ItemChildViewMdoel.EffectiveYear == 0)
                        || (item.ItemChildViewMdoel.EffectiveMonth == 0 && item.ItemChildViewMdoel.EffectiveYear == null))
                    {
                        var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                        if (itemDepreciationDetailViewModel
                            != null)
                        {
                            itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveYear", "Book Effective Life must be greater than 0.");
                        }

                        var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                        if (depreciationDetailViewModel
                            != null)
                        {
                            depreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveMonth", "Book Effective Life must be greater than 0.");
                        }

                        return false;
                    }
                }
                else
                {
                    var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                    if (itemDepreciationDetailViewModel != null)
                    {
                        itemDepreciationDetailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveYear");
                    }

                    var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
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
            ItemDepreciationDetailViewModel item = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));

            if (item != null && (item.ItemChildViewMdoel.IsEffectiveLifeActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1 && item.ItemChildViewMdoel.EffectiveYear == null && item.ItemChildViewMdoel.EffectiveMonth == null))
            {
                var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                if (depreciationDetailViewModel != null)
                {
                    depreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveYear", "Book Effective Life is required.");
                }

                var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                if (itemDepreciationDetailViewModel != null)
                {
                    itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveMonth", "Book Effective Life is required.");
                }

                return false;
            }

            var detailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
            if (detailViewModel != null)
            {
                detailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveYear");
            }

            var firstOrDefault = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
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
            ItemDepreciationDetailViewModel item = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));

            if (item != null && (item.ItemChildViewMdoel.IsEffectiveLifeActive && item.ItemChildViewMdoel.SelectedItemCombobox.ItemId != -1 && item.ItemChildViewMdoel.EffectiveYear == null && item.ItemChildViewMdoel.EffectiveMonth == null))
            {
                var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                if (itemDepreciationDetailViewModel != null)
                {
                    itemDepreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveYear", "Tax Effective Life is required.");
                }

                var depreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                if (depreciationDetailViewModel != null)
                {
                    depreciationDetailViewModel.ItemChildViewMdoel.AddNotifyError("EffectiveMonth", "Tax Effective Life is required.");
                }

                return false;
            }

            var detailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
            if (detailViewModel != null)
            {
                detailViewModel.ItemChildViewMdoel.RemoveNotifyError("EffectiveYear");
            }

            var firstOrDefault = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
            if (firstOrDefault != null)
            {
                firstOrDefault.ItemChildViewMdoel.RemoveNotifyError("EffectiveMonth");
            }

            return true;
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// The get book items save all.
        /// </summary>
        /// <returns>
        /// The <see cref="AssetClassesCategoryUpdateDepreciationItemSave"/>.
        /// </returns>
        internal AssetClassesCategoryUpdateDepreciationItemSave GetBookItemsSaveAll()
        {
            var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
            if (itemDepreciationDetailViewModel != null)
            {
                ItemChildViewModel itemSaveAll = itemDepreciationDetailViewModel.ItemChildViewMdoel;

                return new AssetClassesCategoryUpdateDepreciationItemSave
                {
                    IsAllStopDepnChecked = itemSaveAll.IsAllStopDepnChecked,
                    IsAllDepnMethodChecked = itemSaveAll.IsAllDepnMethodChecked,
                    IsAllSalvageChecked = itemSaveAll.IsAllSalvageChecked,
                    IsAllEffectiveLifeChecked = itemSaveAll.IsAllEffectiveLifeChecked,
                };
            }

            return null;
        }

        /// <summary>
        /// The parse category item detail to save.
        /// </summary>
        /// <returns>
        /// The <see cref="AssetClassesCategoryRowItem"/>.
        /// </returns>
        internal AssetClassesCategoryRowItem ParseCategoryItemDetailToSave()
        {
            var result = new AssetClassesCategoryRowItem
                             {
                                 Category = this.CategoryName,
                                 Enabled = this.IsCategoryEnable
                             };

            if (this.UpdateDepreciationViewModel != null)
            {
                foreach (var item in this.UpdateDepreciationViewModel)
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

        /// <summary>
        /// The get tax items save all.
        /// </summary>
        /// <returns>
        /// The <see cref="AssetClassesCategoryUpdateDepreciationItemSave"/>.
        /// </returns>
        internal AssetClassesCategoryUpdateDepreciationItemSave GetTaxItemsSaveAll()
        {
            var itemDepreciationDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
            if (itemDepreciationDetailViewModel != null)
            {
                ItemChildViewModel itemSaveAll = itemDepreciationDetailViewModel.ItemChildViewMdoel;

                return new AssetClassesCategoryUpdateDepreciationItemSave
                {
                    IsAllStopDepnChecked = itemSaveAll.IsAllStopDepnChecked,
                    IsAllDepnMethodChecked = itemSaveAll.IsAllDepnMethodChecked,
                    IsAllSalvageChecked = itemSaveAll.IsAllSalvageChecked,
                    IsAllEffectiveLifeChecked = itemSaveAll.IsAllEffectiveLifeChecked,
                };
            }

            return null;
        }
        #endregion

        #region Protected Method
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

        #region Private Methods
        /// <summary>
        /// The asset category update depreciation view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetCategoryUpdateDepreciationViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ContentItemChanged != null && (e.PropertyName.IndexOf("ListErrorHyperlink", StringComparison.Ordinal) == -1)
                && (e.PropertyName.IndexOf("ValidationSummary", StringComparison.Ordinal) == -1))
            {
                if (this.IsCheckedOut)
                {
                    // Set value of Salvage in BookViewModel to TaxViewModel
                    if (e.PropertyName.IndexOf("ItemBookViewModel.SalvagePercent", StringComparison.Ordinal) != -1)
                    {
                        var taxDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
                        if (taxDetailViewModel != null)
                        {
                            var bookDetailViewModel = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Book"));
                            if (bookDetailViewModel != null)
                            {
                                taxDetailViewModel.ItemChildViewMdoel.SalvagePercent = bookDetailViewModel.ItemChildViewMdoel.SalvagePercent;
                            }
                        }
                    }

                    if (e.PropertyName.IndexOf("ItemTaxViewModel.EffectiveYear", StringComparison.Ordinal) != -1 || e.PropertyName.IndexOf("ItemTaxViewModel.EffectiveMonth", StringComparison.Ordinal) != -1)
                    {
                        ItemDepreciationDetailViewModel item = this.UpdateDepreciationViewModel.FirstOrDefault(x => x.Header.Equals("Tax"));
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
                                SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton2"),
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
                }

                this.ContentItemChanged(sender, e);
            }
        }
        #endregion
    }
}
