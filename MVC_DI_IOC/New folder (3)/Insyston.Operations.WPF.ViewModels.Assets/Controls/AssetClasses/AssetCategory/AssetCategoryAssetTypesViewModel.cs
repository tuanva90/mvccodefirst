// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetCategoryAssetTypesViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetCategoryAssetTypesViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.AssetCategory
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
    /// The asset category asset types view model.
    /// </summary>
    public class AssetCategoryAssetTypesViewModel : ViewModelUseCaseBase
    {
        #region Private Properties
        /// <summary>
        /// The list asset types item.
        /// </summary>
        private List<AssetClassesCategoryItemDetail> _listAssetTypesItem;

        /// <summary>
        /// The asset types tab view model.
        /// </summary>
        private DynamicGridViewModel _assetTypesTabViewModel;

        /// <summary>
        /// The _is enable select all.
        /// </summary>
        private bool _isEnableSelectAll;

        /// <summary>
        /// The _is selected all types.
        /// </summary>
        private bool _isSelectedAllTypes;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetCategoryAssetTypesViewModel"/> class.
        /// </summary>
        public AssetCategoryAssetTypesViewModel()
        {
            this.IsEnableSelectAll = true;
            this.PropertyChanged += this.OnPropertyChanged;

            this.AssetTypesTabViewModel = new DynamicGridViewModel(typeof(AssetClassesCategoryItemDetail));
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The detail content changed.
        /// </summary>
        public Action<object, PropertyChangedEventArgs> DetailContentChanged;

        /// <summary>
        /// Gets or sets a value indicating whether is change checked type.
        /// </summary>
        public bool IsChangeCheckedType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is change item checked type.
        /// </summary>
        public bool IsChangeItemCheckedType { get; set; }

        /// <summary>
        /// Gets or sets the asset types tab view model.
        /// </summary>
        public DynamicGridViewModel AssetTypesTabViewModel
        {
            get
            {
                return this._assetTypesTabViewModel;
            }

            set
            {
                this.SetField(ref this._assetTypesTabViewModel, value, () => this.AssetTypesTabViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the list asset types item.
        /// </summary>
        public List<AssetClassesCategoryItemDetail> ListAssetTypesItem
        {
            get
            {
                return this._listAssetTypesItem;
            }

            set
            {
                this.SetField(ref this._listAssetTypesItem, value, () => this.ListAssetTypesItem);
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
        public bool IsSelectedAllTypes
        {
            get
            {
                return this._isSelectedAllTypes;
            }

            set
            {
                this.IsChangeCheckedType = true;
                if (!this.IsChangeItemCheckedType)
                {
                    this.AssetTypesTabViewModel.IsSelectAllRow = value;
                }

                this.SetField(ref this._isSelectedAllTypes, value, () => this.IsSelectedAllTypes);
                this.IsChangeCheckedType = false;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// The set grid un checked all.
        /// </summary>
        public void SetGridUnCheckedAll()
        {
            if (this.ListAssetTypesItem != null)
            {
                foreach (var item in this.ListAssetTypesItem)
                {
                    item.IsSelected = false;
                }
            }

            this.AssetTypesTabViewModel = new DynamicGridViewModel(typeof(AssetClassesCategoryItemDetail));
            this.AssetTypesTabViewModel.IsEnableHoverRow = false;
            this.AssetTypesTabViewModel.IsSelectAllRow = false;
            this.AssetTypesTabViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "Text", Header = "ASSET TYPES", IsSelectedColumn = true, Width = 200, MinWidth = 80 },
                                                                                         };
            this.ListAssetTypesItem = (from item in this.ListAssetTypesItem
                                       orderby item.IsSelected descending, item.Text
                                       select item).ToList();
            this.AssetTypesTabViewModel.GridDataRows = this.ListAssetTypesItem.ToList<object>();
            this.AssetTypesTabViewModel.LoadRadGridView();

            if (this.AssetTypesTabViewModel.SelectedItems != null && this.AssetTypesTabViewModel.MembersTable.Rows != null &&
                this.AssetTypesTabViewModel.SelectedItems.Count() == this.AssetTypesTabViewModel.MembersTable.Rows.Count())
            {
                this._isSelectedAllTypes = true;
                this.OnPropertyChanged(() => this.IsSelectedAllTypes);
            }
            else
            {
                this._isSelectedAllTypes = false;
                this.OnPropertyChanged(() => this.IsSelectedAllTypes);
            }
        }

        /// <summary>
        /// The get asset types data source.
        /// </summary>
        /// <param name="equipCatId">
        /// The equip cat id.
        /// </param>
        /// <param name="isEnable">
        /// The is Enable.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetAssetTypesDataSource(int equipCatId, bool isEnable)
        {
            // Get all item asset types for grid Feature
            this.ListAssetTypesItem = await AssetClassesCategoryFunctions.GetListTypesItems();
            if (equipCatId != 0)
            {
                List<int> listItemTypeSelected = await AssetClassesCategoryFunctions.GetItemsTypesSelected(equipCatId);

                if (listItemTypeSelected != null && listItemTypeSelected.Count > 0)
                {
                    foreach (var item in this.ListAssetTypesItem)
                    {
                        if (listItemTypeSelected.Any(x => x == item.ItemId))
                        {
                            item.IsSelected = true;
                        }
                        else
                        {
                            item.IsSelected = false;
                        }
                    }
                }
                else if ((listItemTypeSelected != null && listItemTypeSelected.Count < 1) || listItemTypeSelected == null)
                {
                    foreach (var item in this.ListAssetTypesItem)
                    {
                        item.IsSelected = false;
                    }
                }
            }
            else
            {
                foreach (var item in this.ListAssetTypesItem)
                {
                    item.IsSelected = false;
                }
            }

            this.ListAssetTypesItem = (from item in this.ListAssetTypesItem
                                       orderby item.IsSelected descending, item.Text
                                       select item).ToList();

            /*this.IsEnableSelectAll = isEnable;
            if (!isEnable)
            {
                foreach (var item in this.ListAssetTypesItem)
                {
                    item.IsMouseHover = item.IsSelected;
                }
            }
            else
            {
                foreach (var item in this.ListAssetTypesItem)
                {
                    item.IsMouseHover = true;
                }
            }*/

            foreach (var item in this.ListAssetTypesItem)
                {
                    item.IsMouseHover = true;
                }

            this.AssetTypesTabViewModel = new DynamicGridViewModel(typeof(AssetClassesCategoryItemDetail));
            this.AssetTypesTabViewModel.IsEnableHoverRow = false;
            this.AssetTypesTabViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "Text", Header = "ASSET TYPES", IsSelectedColumn = true, Width = 200, MinWidth = 80 },
                                                                                         };
            this.AssetTypesTabViewModel.GridDataRows = this.ListAssetTypesItem.ToList<object>();
            this.AssetTypesTabViewModel.LoadRadGridView();

            if (this.AssetTypesTabViewModel.SelectedItems != null && this.AssetTypesTabViewModel.MembersTable.Rows != null &&
                this.AssetTypesTabViewModel.SelectedItems.Count() == this.AssetTypesTabViewModel.MembersTable.Rows.Count())
            {
                this._isSelectedAllTypes = true;
                this.OnPropertyChanged(() => this.IsSelectedAllTypes);
            }
            else
            {
                this._isSelectedAllTypes = false;
                this.OnPropertyChanged(() => this.IsSelectedAllTypes);
            }
        }

        /// <summary>
        /// The reset grid when change enable.
        /// </summary>
        /// <param name="equipCatId">
        /// The equip Cat Id.
        /// </param>
        /// <param name="isEnable">
        /// The is enable.
        /// </param>
        public async void ResetGridWhenChangeEnable(int equipCatId, bool isEnable)
        {
            await this.GetAssetTypesDataSource(equipCatId, isEnable);
            if (this.IsCheckedOut)
            {
                this.AssetTypesTabViewModel.IsEnableHoverRow = true;
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
            this.ListAssetTypesItem = new List<AssetClassesCategoryItemDetail>();
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
            //// TODO: Implement this method
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
            //// TODO: Implement this method
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// The asset category asset types view model property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetCategoryAssetTypesViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.AssetTypesTabViewModel != null)
            {
                if (this.AssetTypesTabViewModel.IsEnableHoverRow)
                {
                    if (this.DetailContentChanged != null)
                    {
                        this.DetailContentChanged(sender, e);
                    }
                }
            }
        }

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
                && e.PropertyName.IndexOf("AssetTypesTabViewModel.SelectedItem", StringComparison.Ordinal) == -1)
            {
                this.AssetCategoryAssetTypesViewModel_PropertyChanged(sender, e);
            }

            if (this.IsCheckedOut && (e.PropertyName.IndexOf("AssetTypesTabViewModel.IsCheckItemChanged", StringComparison.Ordinal) != -1) && !this.IsChangeCheckedType)
            {
                this.IsChangeItemCheckedType = true;
                if (this.AssetTypesTabViewModel.SelectedItems.Count() != this.AssetTypesTabViewModel.MembersTable.Rows.Count())
                {
                    this.IsSelectedAllTypes = false;
                }
                else
                {
                    this.IsSelectedAllTypes = true;
                }

                this.IsChangeItemCheckedType = false;
            }
        }

        #endregion
    }
}
