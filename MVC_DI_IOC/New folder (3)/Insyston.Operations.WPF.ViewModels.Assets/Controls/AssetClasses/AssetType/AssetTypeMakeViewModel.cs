// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetTypeMakeViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   The asset type make view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.AssetType
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    using Insyston.Operations.Bussiness.Assets.AssetClasses;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;

    using WPFDynamic.ViewModels.Controls;

    /// <summary>
    /// The asset type make view model.
    /// </summary>
    public class AssetTypeMakeViewModel : ViewModelUseCaseBase
    {
        #region Private Properties
        /// <summary>
        /// The _asset types tab view model.
        /// </summary>
        private DynamicGridViewModel _assetMakeTabViewModel;

        /// <summary>
        /// The _list make item.
        /// </summary>
        private List<AssetClassesCategoryItemDetail> _listMakeItem;

        /// <summary>
        /// The _is enable select all.
        /// </summary>
        private bool _isEnableSelectAll;

        /// <summary>
        /// The _is selected all features.
        /// </summary>
        private bool _isSelectedAllMakes;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetTypeMakeViewModel"/> class.
        /// </summary>
        public AssetTypeMakeViewModel()
        {
            this.IsEnableSelectAll = true;
            this.PropertyChanged += this.OnPropertyChanged;

            this.AssetMakeTabViewModel = new DynamicGridViewModel(typeof(AssetClassesCategoryItemDetail));
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The detail content changed.
        /// </summary>
        public Action<object, PropertyChangedEventArgs> DetailContentChanged;

        /// <summary>
        /// Gets or sets a value indicating whether is change checked make.
        /// </summary>
        public bool IsChangeCheckedMake { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is change item checked make.
        /// </summary>
        public bool IsChangeItemCheckedMake { get; set; }

        /// <summary>
        /// Gets or sets the asset types tab view model.
        /// </summary>
        public DynamicGridViewModel AssetMakeTabViewModel
        {
            get
            {
                return this._assetMakeTabViewModel;
            }

            set
            {
                this.SetField(ref this._assetMakeTabViewModel, value, () => this.AssetMakeTabViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the list make item.
        /// </summary>
        public List<AssetClassesCategoryItemDetail> ListMakeItem
        {
            get
            {
                return this._listMakeItem;
            }

            set
            {
                this.SetField(ref this._listMakeItem, value, () => this._listMakeItem);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is enable select all.
        /// </summary>
        public bool IsEnableSelectAll
        {
            get
            {
                return this._isEnableSelectAll;
            }

            set
            {
                this.SetField(ref this._isEnableSelectAll, value, () => this.IsEnableSelectAll);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is selected all features.
        /// </summary>
        public bool IsSelectedAllMakes
        {
            get
            {
                return this._isSelectedAllMakes;
            }

            set
            {
                this.IsChangeCheckedMake = true;
                if (!this.IsChangeItemCheckedMake)
                {
                    this.AssetMakeTabViewModel.IsSelectAllRow = value;
                }

                this.SetField(ref this._isSelectedAllMakes, value, () => this.IsSelectedAllMakes);
                this.IsChangeCheckedMake = false;
            }
        }
        #endregion 

        #region Public Methods

        /// <summary>
        /// The set grid un checked all.
        /// </summary>
        public void SetGridUnCheckedAll()
        {
            if (this.ListMakeItem != null)
            {
                foreach (var item in this.ListMakeItem)
                {
                    item.IsSelected = false;
                }
            }

            this.AssetMakeTabViewModel = new DynamicGridViewModel(typeof(AssetClassesCategoryItemDetail));
            this.AssetMakeTabViewModel.IsEnableHoverRow = false;
            this.AssetMakeTabViewModel.IsSelectAllRow = false;
            this.AssetMakeTabViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "Text", Header = "ASSET MAKES", IsSelectedColumn = true, Width = 200, MinWidth = 90 },
                                                                                         };
            this.ListMakeItem = (from item in this.ListMakeItem
                                     orderby item.IsSelected descending, item.Text
                                     select item).ToList();
            this.AssetMakeTabViewModel.GridDataRows = this.ListMakeItem.ToList<object>();
            this.AssetMakeTabViewModel.LoadRadGridView();

            if (this.AssetMakeTabViewModel.SelectedItems != null && this.AssetMakeTabViewModel.MembersTable.Rows != null &&
                this.AssetMakeTabViewModel.SelectedItems.Count() == this.AssetMakeTabViewModel.MembersTable.Rows.Count())
            {
                this._isSelectedAllMakes = true;
                this.OnPropertyChanged(() => this.IsSelectedAllMakes);
            }
            else
            {
                this._isSelectedAllMakes = false;
                this.OnPropertyChanged(() => this.IsSelectedAllMakes);
            }
        }

        /// <summary>
        /// The get make data source.
        /// </summary>
        /// <param name="equipTypeId">
        /// The equip type id.
        /// </param>
        /// <param name="isEnable">
        /// The is Enable.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetMakeDataSource(int equipTypeId, bool isEnable)
        {
            if (equipTypeId != 0)
            {
                // Get all item make for Make screen
                this.ListMakeItem = await AssetClassesTypeFunctions.GetListMakeItems();
                List<int> listItemMakeSelected = await AssetClassesTypeFunctions.GetItemsMakeSelected(equipTypeId);

                if (listItemMakeSelected != null && listItemMakeSelected.Count > 0)
                {
                    foreach (var item in this.ListMakeItem)
                    {
                        if (listItemMakeSelected.Any(x => x == item.ItemId))
                        {
                            item.IsSelected = true;
                        }
                        else
                        {
                            item.IsSelected = false;
                        }
                    }
                }
                else if ((listItemMakeSelected != null && listItemMakeSelected.Count < 1) || listItemMakeSelected == null)
                {
                    foreach (var item in this.ListMakeItem)
                    {
                        item.IsSelected = false;
                    }
                }
            }
            else
            {
                // Get all item make for Make screen
                this.ListMakeItem = await AssetClassesTypeFunctions.GetListMakeItemsForAddState();
            }

            /*this.IsEnableSelectAll = isEnable;
            if (!isEnable)
            {
                foreach (var item in this.ListMakeItem)
                {
                    item.IsMouseHover = item.IsSelected;
                }
            }
            else
            {
                foreach (var item in this.ListMakeItem)
                {
                    item.IsMouseHover = true;
                }
            }*/

            foreach (var item in this.ListMakeItem)
                {
                    item.IsMouseHover = true;
                }

            this.ListMakeItem = (from item in this.ListMakeItem
                                     orderby item.IsSelected descending, item.Text
                                     select item).ToList();

            this.AssetMakeTabViewModel = new DynamicGridViewModel(typeof(AssetClassesCategoryItemDetail));
            this.AssetMakeTabViewModel.IsEnableHoverRow = false;
            this.AssetMakeTabViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "Text", Header = "ASSET MAKES", IsSelectedColumn = true, Width = 200, MinWidth = 90},
                                                                                         };
            this.AssetMakeTabViewModel.GridDataRows = this.ListMakeItem.ToList<object>();
            this.AssetMakeTabViewModel.LoadRadGridView();

            if (this.AssetMakeTabViewModel.SelectedItems != null && this.AssetMakeTabViewModel.MembersTable.Rows != null &&
                this.AssetMakeTabViewModel.SelectedItems.Count() == this.AssetMakeTabViewModel.MembersTable.Rows.Count())
            {
                this._isSelectedAllMakes = true;
                this.OnPropertyChanged(() => this.IsSelectedAllMakes);
            }
            else
            {
                this._isSelectedAllMakes = false;
                this.OnPropertyChanged(() => this.IsSelectedAllMakes);
            }
        }

        /// <summary>
        /// The reset grid when change enable.
        /// </summary>
        /// <param name="equipTyped">
        /// The equip Typed.
        /// </param>
        /// <param name="isEnable">
        /// The is enable.
        /// </param>
        public async void ResetGridWhenChangeEnable(int equipTyped, bool isEnable)
        {
            await this.GetMakeDataSource(equipTyped, isEnable);
            if (this.IsCheckedOut)
            {
                this.AssetMakeTabViewModel.IsEnableHoverRow = true;
            }
        }

        /// <summary>
        /// The generate asset types control.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GenerateAssetTypesControl()
        {
            this.ListMakeItem = new List<AssetClassesCategoryItemDetail>();
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
            // TODO: Implement this method
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
            // TODO: Implement this method
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut
                && e.PropertyName.IndexOf("AssetMakeTabViewModel.SelectedItem", StringComparison.Ordinal) == -1)
            {
                this.AssetCategoryAssetTypesViewModel_PropertyChanged(sender, e);
            }

            if (this.IsCheckedOut && (e.PropertyName.IndexOf("AssetMakeTabViewModel.IsCheckItemChanged", StringComparison.Ordinal) != -1) && !this.IsChangeCheckedMake)
            {
                this.IsChangeItemCheckedMake = true;
                if (this.AssetMakeTabViewModel.SelectedItems.Count() != this.AssetMakeTabViewModel.MembersTable.Rows.Count())
                {
                    this.IsSelectedAllMakes = false;
                }
                else
                {
                    this.IsSelectedAllMakes = true;
                }

                this.IsChangeItemCheckedMake = false;
            }
        }

        /// <summary>
        /// The asset category asset types view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetCategoryAssetTypesViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.AssetMakeTabViewModel != null)
            {
                if (this.AssetMakeTabViewModel.IsEnableHoverRow)
                {
                    if (this.DetailContentChanged != null)
                    {
                        this.DetailContentChanged(sender, e);
                    }
                }
            }
        }
        #endregion
    }
}
