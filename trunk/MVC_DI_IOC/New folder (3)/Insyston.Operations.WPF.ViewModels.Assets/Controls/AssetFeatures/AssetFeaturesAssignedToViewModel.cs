// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetFeaturesAssignedToViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetRegistersViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetFeatures
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    using Insyston.Operations.Bussiness.Assets;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;

    using WPFDynamic.ViewModels.Controls;

    /// <summary>
    /// The asset features assigned to view model.
    /// </summary>
    public class AssetFeaturesAssignedToViewModel : ViewModelUseCaseBase
    {
        #region Variables

        /// <summary>
        /// The _dynamic asset categories view model.
        /// </summary>
        private DynamicGridViewModel _dynamicAssetCategoriesViewModel;

        /// <summary>
        /// The _dynamic asset types view model.
        /// </summary>
        private DynamicGridViewModel _dynamicAssetTypesViewModel;

        /// <summary>
        /// The _feature name.
        /// </summary>
        private string _featureName;

        /// <summary>
        /// The _is selected all types.
        /// </summary>
        private bool _isSelectedAllTypes;

        /// <summary>
        /// The _is selected all categories.
        /// </summary>
        private bool _isSelectedAllCategories;

        /// <summary>
        /// The _is enable select all.
        /// </summary>
        private bool _isEnableSelectAll;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetFeaturesAssignedToViewModel"/> class.
        /// </summary>
        public AssetFeaturesAssignedToViewModel()
        {
            this.IsUnCheckedCatsAll = true;
            this.IsUnCheckedTypesAll = true;
            this.IsEnableSelectAll = true;

            this.DynamicAssetTypesViewModel = new DynamicGridViewModel(typeof(AssetTypesFeatureRowItem));
            this.DynamicAssetCategoriesViewModel = new DynamicGridViewModel(typeof(AssetCategoriesFeatureRowItem));

            this.PropertyChanged += this.AssetFeaturesAssignedToViewModel_PropertyChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether is change checked type.
        /// </summary>
        public bool IsChangeCheckedType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is change item checked type.
        /// </summary>
        public bool IsChangeItemCheckedType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is change checked category.
        /// </summary>
        public bool IsChangeCheckedCategory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is change item checked category.
        /// </summary>
        public bool IsChangeItemCheckedCategory { get; set; }

        /// <summary>
        /// Gets or sets the num categories.
        /// </summary>
        public int NumCategories { get; set; }

        /// <summary>
        /// Gets or sets the num types.
        /// </summary>
        public int NumTypes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is un checked types all.
        /// </summary>
        public bool IsUnCheckedTypesAll { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is un checked cats all.
        /// </summary>
        public bool IsUnCheckedCatsAll { get; set; }

        /// <summary>
        /// Gets or sets the dynamic asset categories view model.
        /// </summary>
        public DynamicGridViewModel DynamicAssetCategoriesViewModel
        {
            get
            {
                return this._dynamicAssetCategoriesViewModel;
            }

            set
            {
                this.SetField(ref this._dynamicAssetCategoriesViewModel, value, () => this.DynamicAssetCategoriesViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic asset types view model.
        /// </summary>
        public DynamicGridViewModel DynamicAssetTypesViewModel
        {
            get
            {
                return this._dynamicAssetTypesViewModel;
            }

            set
            {
                this.SetField(ref this._dynamicAssetTypesViewModel, value, () => this.DynamicAssetTypesViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the feature name.
        /// </summary>
        public string FeatureName
        {
            get
            {
                return this._featureName;
            }

            set
            {
                this.SetField(ref this._featureName, value, () => this.FeatureName);
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
        /// Gets or sets a value indicating whether is selected all types.
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
                    this.DynamicAssetTypesViewModel.IsSelectAllRow = value;
                }
                
                this.SetField(ref this._isSelectedAllTypes, value, () => this.IsSelectedAllTypes);
                this.IsChangeCheckedType = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is selected all categories.
        /// </summary>
        public bool IsSelectedAllCategories
        {
            get
            {
                return this._isSelectedAllCategories;
            }

            set
            {
                this.IsChangeCheckedCategory = true;
                if (!this.IsChangeItemCheckedCategory)
                {
                    this.DynamicAssetCategoriesViewModel.IsSelectAllRow = value;
                }

                this.SetField(ref this._isSelectedAllCategories, value, () => this.IsSelectedAllCategories);
                this.IsChangeCheckedCategory = false;
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// The populate equip cat feature.
        /// </summary>
        /// <param name="SelectedFeatureType">
        /// The selected feature type.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task PopulateEquipCatFeature(FeatureType SelectedFeatureType)
        {
            if (SelectedFeatureType != null)
            {
                this.DynamicAssetCategoriesViewModel = new DynamicGridViewModel(typeof(AssetCategoriesFeatureRowItem));
                this.DynamicAssetCategoriesViewModel.IsSelectAllRow = false;
                this.DynamicAssetCategoriesViewModel.GridColumns = new List<DynamicColumn>
                                                                           {
                                                                               new DynamicColumn
                                                                                   {
                                                                                       ColumnName
                                                                                           =
                                                                                           "EquipCatName",
                                                                                       Header
                                                                                           =
                                                                                           "ASSET CATEGORIES",
                                                                                       IsSelectedColumn
                                                                                           =
                                                                                           true, Width = 200, MinWidth = 95
                                                                                   }
                                                                           };
                var assetCategoriesFeature = new ObservableCollection<AssetCategoriesFeatureRowItem>(await 
                    AssetFeatureFunction.GetAssetCategoriesFeatureAsync(SelectedFeatureType.FeatureTypeId));
                assetCategoriesFeature = new ObservableCollection<AssetCategoriesFeatureRowItem>((from item in assetCategoriesFeature
                        orderby item.IsSelected descending, item.EquipCatName
                        select item).ToList());

                this.IsEnableSelectAll = SelectedFeatureType.Enabled;
                if (!SelectedFeatureType.Enabled)
                {
                    foreach (var item in assetCategoriesFeature)
                    {
                        item.IsMouseHover = item.IsSelected;
                    }
                }
                else
                {
                    foreach (var item in assetCategoriesFeature)
                    {
                        item.IsMouseHover = true;
                    }
                }

                this.NumCategories = assetCategoriesFeature.Count;
                this.DynamicAssetCategoriesViewModel.GridDataRows = assetCategoriesFeature.ToList<object>();
                this.DynamicAssetCategoriesViewModel.LoadRadGridView();
                this.DynamicAssetCategoriesViewModel.IsEnableHoverRow = false;
                if (this.DynamicAssetCategoriesViewModel.SelectedItems != null && this.DynamicAssetCategoriesViewModel.MembersTable.Rows != null
                    && this.DynamicAssetCategoriesViewModel.SelectedItems.Count() == this.DynamicAssetCategoriesViewModel.MembersTable.Rows.Count())
                {
                    this._isSelectedAllCategories = true;
                    this.OnPropertyChanged(() => this.IsSelectedAllCategories);
                }
                else
                {
                    this._isSelectedAllCategories = false;
                    this.OnPropertyChanged(() => this.IsSelectedAllCategories);
                }
            }
        }

        /// <summary>
        /// The reset grid when change enable.
        /// </summary>
        /// <param name="SelectedFeatureType">
        /// The selected feature type.
        /// </param>
        /// <param name="isEnable">
        /// The is enable.
        /// </param>
        public async void ResetGridWhenChangeEnable(FeatureType SelectedFeatureType, bool isEnable)
        {
            SelectedFeatureType.Enabled = isEnable;
            await this.PopulateEquipCatFeature(SelectedFeatureType);
            await this.PopulateEquipTypeFeature(SelectedFeatureType);
            if (this.IsCheckedOut)
            {
                this.DynamicAssetTypesViewModel.IsEnableHoverRow = true;
                this.DynamicAssetCategoriesViewModel.IsEnableHoverRow = true;
            }
        }

        /// <summary>
        /// The populate equip type feature.
        /// </summary>
        /// <param name="SelectedFeatureType">
        /// The selected feature type.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task PopulateEquipTypeFeature(FeatureType SelectedFeatureType)
        {
            if (SelectedFeatureType != null)
            {
                this.DynamicAssetTypesViewModel = new DynamicGridViewModel(typeof(AssetTypesFeatureRowItem));
                this.DynamicAssetTypesViewModel.IsSelectAllRow = false;
                this.DynamicAssetTypesViewModel.GridColumns = new List<DynamicColumn>
                                                                      {
                                                                          new DynamicColumn
                                                                              {
                                                                                  ColumnName
                                                                                      =
                                                                                      "EquipTypeName",
                                                                                  Header
                                                                                      =
                                                                                      "ASSET TYPES",
                                                                                  IsSelectedColumn
                                                                                      =
                                                                                      true, Width = 200, MinWidth = 70
                                                                              }
                                                                      };
                var assetTypesFeature = new ObservableCollection<AssetTypesFeatureRowItem>(await AssetFeatureFunction.GetAssetTypesFeatureAsync(SelectedFeatureType.FeatureTypeId));
                assetTypesFeature = new ObservableCollection<AssetTypesFeatureRowItem>((from item in assetTypesFeature
                                                                                                  orderby item.IsSelected descending, item.EquipTypeName
                                                                                                  select item).ToList());
                if (!SelectedFeatureType.Enabled)
                {
                    foreach (var item in assetTypesFeature)
                    {
                        item.IsMouseHover = item.IsSelected;
                    }
                }
                else
                {
                    foreach (var item in assetTypesFeature)
                    {
                        item.IsMouseHover = true;
                    }
                }

                this.NumTypes = assetTypesFeature.Count;
                this.DynamicAssetTypesViewModel.GridDataRows = assetTypesFeature.ToList<object>();
                this.DynamicAssetTypesViewModel.LoadRadGridView();
                this.DynamicAssetTypesViewModel.IsEnableHoverRow = false;
                if (this.DynamicAssetTypesViewModel.SelectedItems != null && this.DynamicAssetTypesViewModel.MembersTable.Rows != null
                    && this.DynamicAssetTypesViewModel.SelectedItems.Count() == this.DynamicAssetTypesViewModel.MembersTable.Rows.Count())
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
        }
        #endregion

        #region Other

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

        #region Private Method

        /// <summary>
        /// The asset features assigned to view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetFeaturesAssignedToViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut && (e.PropertyName.IndexOf("DynamicAssetCategoriesViewModel.IsCheckItemChanged", StringComparison.Ordinal) != -1) && !this.IsChangeCheckedCategory)
            {
                this.IsChangeItemCheckedCategory = true;
                if (this.DynamicAssetCategoriesViewModel.SelectedItems.Count() != this.DynamicAssetCategoriesViewModel.MembersTable.Rows.Count())
                {
                    this.IsSelectedAllCategories = false;
                }
                else
                {
                    this.IsSelectedAllCategories = true;
                }

                this.IsChangeItemCheckedCategory = false;
            }

            if (this.IsCheckedOut && (e.PropertyName.IndexOf("DynamicAssetTypesViewModel.IsCheckItemChanged", StringComparison.Ordinal) != -1) && !this.IsChangeCheckedType)
            {
                this.IsChangeItemCheckedType = true;
                if (this.DynamicAssetTypesViewModel.SelectedItems.Count() != this.DynamicAssetTypesViewModel.MembersTable.Rows.Count())
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
