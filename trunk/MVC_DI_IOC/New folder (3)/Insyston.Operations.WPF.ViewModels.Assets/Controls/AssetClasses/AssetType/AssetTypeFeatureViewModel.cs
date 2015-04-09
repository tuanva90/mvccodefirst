// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetTypeFeatureViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetTypeFeatureViewModel type.
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
    /// The asset type feature view model.
    /// </summary>
    public class AssetTypeFeatureViewModel : ViewModelUseCaseBase
    {
        #region Private Properties
        /// <summary>
        /// The features tab view model.
        /// </summary>
        private DynamicGridViewModel _featuresTabViewModel;

        /// <summary>
        /// The _list features item.
        /// </summary>
        private List<AssetClassesCategoryItemDetail> _listFeaturesItem;

        /// <summary>
        /// The _is enable select all.
        /// </summary>
        private bool _isEnableSelectAll;

        /// <summary>
        /// The _is selected all features.
        /// </summary>
        private bool _isSelectedAllFeatures;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetTypeFeatureViewModel"/> class.
        /// </summary>
        public AssetTypeFeatureViewModel()
        {
            this.IsEnableSelectAll = true;
            this.PropertyChanged += this.OnPropertyChanged;

            this.FeaturesTabViewModel = new DynamicGridViewModel(typeof(AssetClassesCategoryItemDetail));
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
        public bool IsChangeCheckedFeature { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is change item checked type.
        /// </summary>
        public bool IsChangeItemCheckedFeature { get; set; }

        /// <summary>
        /// Gets or sets the features tab view model.
        /// </summary>
        public DynamicGridViewModel FeaturesTabViewModel
        {
            get
            {
                return this._featuresTabViewModel;
            }

            set
            {
                this.SetField(ref this._featuresTabViewModel, value, () => this.FeaturesTabViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the list features item.
        /// </summary>
        public List<AssetClassesCategoryItemDetail> ListFeaturesItem
        {
            get
            {
                return this._listFeaturesItem;
            }

            set
            {
                this.SetField(ref this._listFeaturesItem, value, () => this.ListFeaturesItem);
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
        public bool IsSelectedAllFeatures
        {
            get
            {
                return this._isSelectedAllFeatures;
            }

            set
            {
                this.IsChangeCheckedFeature = true;
                if (!this.IsChangeItemCheckedFeature)
                {
                    this.FeaturesTabViewModel.IsSelectAllRow = value;
                }

                this.SetField(ref this._isSelectedAllFeatures, value, () => this.IsSelectedAllFeatures);
                this.IsChangeCheckedFeature = false;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// The set grid un checked all.
        /// </summary>
        public void SetGridUnCheckedAll()
        {
            if (this.ListFeaturesItem != null)
            {
                foreach (var item in this.ListFeaturesItem)
                {
                    item.IsSelected = false;
                }
            }

            this.FeaturesTabViewModel = new DynamicGridViewModel(typeof(AssetClassesCategoryItemDetail));
            this.FeaturesTabViewModel.IsEnableHoverRow = false;
            this.FeaturesTabViewModel.IsSelectAllRow = false;
            this.FeaturesTabViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "Text", Header = "FEATURE NAME", IsSelectedColumn = true, Width = 200, MinWidth = 90 },
                                                                                         };
            this.ListFeaturesItem = (from item in this.ListFeaturesItem
                                     orderby item.IsSelected descending, item.Text
                                     select item).ToList();
            this.FeaturesTabViewModel.GridDataRows = this.ListFeaturesItem.ToList<object>();
            this.FeaturesTabViewModel.LoadRadGridView();

            if (this.FeaturesTabViewModel.SelectedItems != null && this.FeaturesTabViewModel.MembersTable.Rows != null &&
                this.FeaturesTabViewModel.SelectedItems.Count() == this.FeaturesTabViewModel.MembersTable.Rows.Count())
            {
                this._isSelectedAllFeatures = true;
                this.OnPropertyChanged(() => this.IsSelectedAllFeatures);
            }
            else
            {
                this._isSelectedAllFeatures = false;
                this.OnPropertyChanged(() => this.IsSelectedAllFeatures);
            }
        }

        /// <summary>
        /// The get feature data source.
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
        public async Task GetFeatureDataSource(int equipTypeId, bool isEnable)
        {
            this.ListFeaturesItem = await AssetClassesTypeFunctions.GetListFeaturesItems();

            if (equipTypeId != 0)
            {
                List<int> listItemFeatureSelected = await AssetClassesTypeFunctions.GetItemsFeaturesSelected(equipTypeId);

                if (listItemFeatureSelected != null && listItemFeatureSelected.Count > 0)
                {
                    foreach (var item in this.ListFeaturesItem)
                    {
                        if (listItemFeatureSelected.Any(x => x == item.ItemId))
                        {
                            item.IsSelected = true;
                        }
                        else
                        {
                            item.IsSelected = false;
                        }
                    }
                }
                else if ((listItemFeatureSelected != null && listItemFeatureSelected.Count < 1) || listItemFeatureSelected == null)
                {
                    foreach (var item in this.ListFeaturesItem)
                    {
                        item.IsSelected = false;
                    }
                }
            }
            else
            {
                foreach (var item in this.ListFeaturesItem)
                {
                    item.IsSelected = false;
                }
            }

            this.ListFeaturesItem = (from item in this.ListFeaturesItem
                                     orderby item.IsSelected descending, item.Text
                                     select item).ToList();

            /*this.IsEnableSelectAll = isEnable;
            if (!isEnable)
            {
                foreach (var item in this.ListFeaturesItem)
                {
                    item.IsMouseHover = item.IsSelected;
                }
            }
            else
            {
                foreach (var item in this.ListFeaturesItem)
                {
                    item.IsMouseHover = true;
                }
            }*/

            foreach (var item in this.ListFeaturesItem)
                {
                    item.IsMouseHover = true;
                }

            this.FeaturesTabViewModel = new DynamicGridViewModel(typeof(AssetClassesCategoryItemDetail));
            this.FeaturesTabViewModel.IsEnableHoverRow = false;
            this.FeaturesTabViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "Text", Header = "FEATURE NAME", IsSelectedColumn = true, Width = 200, MinWidth = 90},
                                                                                         };
            this.FeaturesTabViewModel.GridDataRows = this.ListFeaturesItem.ToList<object>();
            this.FeaturesTabViewModel.LoadRadGridView();

            if (this.FeaturesTabViewModel.SelectedItems != null && this.FeaturesTabViewModel.MembersTable.Rows != null &&
                this.FeaturesTabViewModel.SelectedItems.Count() == this.FeaturesTabViewModel.MembersTable.Rows.Count())
            {
                this._isSelectedAllFeatures = true;
                this.OnPropertyChanged(() => this.IsSelectedAllFeatures);
            }
            else
            {
                this._isSelectedAllFeatures = false;
                this.OnPropertyChanged(() => this.IsSelectedAllFeatures);
            }
        }

        /// <summary>
        /// The reset grid when change enable.
        /// </summary>
        /// <param name="equipTypeId">
        /// The equip Type Id.
        /// </param>
        /// <param name="isEnable">
        /// The is enable.
        /// </param>
        public async void ResetGridWhenChangeEnable(int equipTypeId, bool isEnable)
        {
            await this.GetFeatureDataSource(equipTypeId, isEnable);
            if (this.IsCheckedOut)
            {
                this.FeaturesTabViewModel.IsEnableHoverRow = true;
            }
        }

        /// <summary>
        /// The generate feature control.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GenerateFeatureControl()
        {
            // Get all item feature for grid Feature
            this.ListFeaturesItem = new List<AssetClassesCategoryItemDetail>();
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
        /// The selected item_ on properties changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SelectedItem_OnPropertiesChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.FeaturesTabViewModel != null)
            {
                if (this.FeaturesTabViewModel.IsEnableHoverRow)
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
                && e.PropertyName.IndexOf("FeaturesTabViewModel.SelectedItem", StringComparison.Ordinal) == -1)
            {
                this.SelectedItem_OnPropertiesChanged(sender, e);
            }

            if (this.IsCheckedOut && (e.PropertyName.IndexOf("FeaturesTabViewModel.IsCheckItemChanged", StringComparison.Ordinal) != -1) && !this.IsChangeCheckedFeature)
            {
                this.IsChangeItemCheckedFeature = true;
                if (this.FeaturesTabViewModel.SelectedItems.Count() != this.FeaturesTabViewModel.MembersTable.Rows.Count())
                {
                    this.IsSelectedAllFeatures = false;
                }
                else
                {
                    this.IsSelectedAllFeatures = true;
                }

                this.IsChangeItemCheckedFeature = false;
            }
        }

        #endregion
    }
}
