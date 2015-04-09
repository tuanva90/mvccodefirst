// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetFeaturesAssignFeatureViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   The asset features assign feature view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetFeatures
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Bussiness.Assets;
    using Insyston.Operations.Bussiness.Assets.AssetClasses;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Controls.DragDrop;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The asset features assign feature view model.
    /// </summary>
    public class AssetFeaturesAssignFeatureViewModel : ViewModelUseCaseBase
    {
        #region Variables
        /// <summary>
        /// The type key.
        /// </summary>
        private const string TypeKey = "Type";

        /// <summary>
        /// The cat key.
        /// </summary>
        private const string CatKey = "Category";

        /// <summary>
        /// The _asset feature drag drop view model.
        /// </summary>
        private GroupAssetFeatureDragDropViewModel _assetFeatureDragDropViewModel;

        /// <summary>
        /// The _list select categories.
        /// </summary>
        private ListDragDropViewModel _listSelectCategories;

        /// <summary>
        /// The _list select types.
        /// </summary>
        private ListDragDropViewModel _listSelectTypes;
        
        #endregion

        #region Pubic Properties

        /// <summary>
        /// Gets or sets the list select categories.
        /// </summary>
        public ListDragDropViewModel ListSelectCategories
        {
            get
            {
                return this._listSelectCategories;
            }

            set
            {
                this.SetField(ref this._listSelectCategories, value, () => this.ListSelectCategories);
            }
        }

        /// <summary>
        /// Gets or sets the list select types.
        /// </summary>
        public ListDragDropViewModel ListSelectTypes
        {
            get
            {
                return this._listSelectTypes;
            }

            set
            {
                this.SetField(ref this._listSelectTypes, value, () => this.ListSelectTypes);
            }
        }

        /// <summary>
        /// Gets or sets the asset feature drag drop view model.
        /// </summary>
        public GroupAssetFeatureDragDropViewModel AssetFeatureDragDropViewModel
        {
            get
            {
                return this._assetFeatureDragDropViewModel;
            }

            set
            {
                this.SetField(ref this._assetFeatureDragDropViewModel, value, () => this.AssetFeatureDragDropViewModel);
            }
        }

        #endregion

        #region Private Method
        #endregion

        #region Public Method

        /// <summary>
        /// The populate assign feature drog drag.
        /// </summary>
        /// <param name="selectedFeatureTypes">
        /// The selected feature types.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task PopulateAssignFeatureDrogDrag(ObservableCollection<AssetFeatureTypeRowItem> selectedFeatureTypes)
        {
            // create Select one or more in drag drop
            var listDragDropCat = new ListDragDropViewModel(1, CatKey);
            var listDragDropType = new ListDragDropViewModel(2, TypeKey);

            string assetTypeKey = TypeKey + Guid.NewGuid().ToString();
            string assetCategoryKey = CatKey + Guid.NewGuid().ToString();
            listDragDropCat.Key = assetCategoryKey;
            listDragDropType.Key = assetTypeKey;
            listDragDropCat.ChangeVisibilityHeader = Visibility.Collapsed;
            listDragDropType.ChangeVisibilityHeader = Visibility.Collapsed;
            listDragDropCat.Items = new ObservableCollection<ItemDragDrop>();
            listDragDropType.Items = new ObservableCollection<ItemDragDrop>();

            var categoriesList = await AssetClassesCategoryFunctions.GetAssetCategoriesList();
            var typesList = await AssetClassesTypeFunctions.GetAssetTypesList();

            foreach (var category in categoriesList)
            {
                listDragDropCat.Items.Add(
                    new ItemDragDrop { ID = category.EquipCatId, Name = category.Description, Key = assetCategoryKey });
            }

            foreach (var type in typesList)
            {
                listDragDropType.Items.Add(
                    new ItemDragDrop { ID = type.EquipTypeId, Name = type.Description, Key = assetTypeKey });
            }

            this.ListSelectCategories = listDragDropCat;
            this.ListSelectCategories.IsConstantSource = true;

            this.ListSelectTypes = listDragDropType;
            this.ListSelectTypes.IsConstantSource = true;

            // create feature control
            var groupFeatureViewModel = new GroupAssetFeatureDragDropViewModel();
            groupFeatureViewModel.GroupAssetDragDropSource = new ObservableCollection<ItemAssetFeatureDragDropViewModel>();

            foreach (var selectedFeature in selectedFeatureTypes)
            {
                var itemGroupViewModel = new ItemAssetFeatureDragDropViewModel();

                itemGroupViewModel.HeaderName = selectedFeature.FeatureName;
                itemGroupViewModel.Id = selectedFeature.FeatureTypeId;

                itemGroupViewModel.AssetCategoryViewModel = new ListDragDropViewModel(1, CatKey);
                itemGroupViewModel.AssetTypeViewModel = new ListDragDropViewModel(2, TypeKey);

                itemGroupViewModel.AssetCategoryViewModel.ChangeVisibilityHeader = Visibility.Collapsed;
                itemGroupViewModel.AssetTypeViewModel.ChangeVisibilityHeader = Visibility.Collapsed;

                itemGroupViewModel.AssetCategoryViewModel.Key = assetCategoryKey;
                itemGroupViewModel.AssetTypeViewModel.Key = assetTypeKey;

                itemGroupViewModel.AssetCategoryViewModel.Items = new ObservableCollection<ItemDragDrop>();
                itemGroupViewModel.AssetTypeViewModel.Items = new ObservableCollection<ItemDragDrop>();

                var categoryFeaturesList = (await AssetFeatureFunction.GetAssetCategoriesFeatureAsync(selectedFeature.FeatureTypeId)).Where(asset => asset.IsSelected).ToList();
                var typeFeaturesList = (await AssetFeatureFunction.GetAssetTypesFeatureAsync(selectedFeature.FeatureTypeId)).Where(asset => asset.IsSelected).ToList();

                // load categoryFeature and typeFeature list for each feature
                foreach (var categoryFeature in categoryFeaturesList)
                {
                    itemGroupViewModel.AssetCategoryViewModel.Items.Add(new ItemDragDrop { ID = categoryFeature.EquipCatId, Name = categoryFeature.EquipCatName, Key = assetCategoryKey });
                }

                foreach (var typeFeature in typeFeaturesList)
                {
                    itemGroupViewModel.AssetTypeViewModel.Items.Add(new ItemDragDrop { ID = typeFeature.EquipTypeId, Name = typeFeature.EquipTypeName, Key = assetTypeKey });
                }

                groupFeatureViewModel.GroupAssetDragDropSource.Add(itemGroupViewModel);
            }

            this.AssetFeatureDragDropViewModel = groupFeatureViewModel;
            this.AssetFeatureDragDropViewModel.NotifyItemsChanged();
        }

        #endregion

        #region Protected Method
        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Unlock function
        /// </exception>
        protected override Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The<see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Lock function
        /// </exception>
        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
