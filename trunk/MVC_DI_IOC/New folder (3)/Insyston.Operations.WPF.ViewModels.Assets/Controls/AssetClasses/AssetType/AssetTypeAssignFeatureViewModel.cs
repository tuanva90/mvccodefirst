// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetTypeAssignFeatureViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   The asset type assign feature view model.
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

    using Insyston.Operations.Bussiness.Assets.AssetClasses;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Controls.DragDrop;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The asset type assign feature view model.
    /// </summary>
    public class AssetTypeAssignFeatureViewModel : ViewModelUseCaseBase
    {
        #region Private Properties
        /// <summary>
        /// The feature key.
        /// </summary>
        private const string FeatureKey = "Feature";

        /// <summary>
        /// The _list all features items.
        /// </summary>
        private ListDragDropViewModel _listAllFeaturesItems;

        /// <summary>
        /// The _list items drag drop.
        /// </summary>
        private GroupDragDropViewModel _listItemsDragDrop;
        #endregion

        #region Public Properties
        /// <summary>
        /// The content item changed.
        /// </summary>
        public Action<object, PropertyChangedEventArgs> ContentItemChanged;

        /// <summary>
        /// Gets or sets the list all features items.
        /// </summary>
        public ListDragDropViewModel ListAllFeaturesItems
        {
            get
            {
                return this._listAllFeaturesItems;
            }

            set
            {
                this.SetField(ref this._listAllFeaturesItems, value, () => this.ListAllFeaturesItems);
            }
        }

        /// <summary>
        /// Gets or sets the list items drag drop.
        /// </summary>
        public GroupDragDropViewModel ListItemsDragDrop
        {
            get
            {
                return this._listItemsDragDrop;
            }

            set
            {
                this.SetField(ref this._listItemsDragDrop, value, () => this.ListItemsDragDrop);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// The get list feature items.
        /// </summary>
        /// <param name="allItemsSelected">
        /// The all items selected.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetListFeatureItems(ObservableCollection<AssetClassesTypeRowItem> allItemsSelected)
        {
            List<AssetClassesCategoryItemDetail> listFeaturesItem = await AssetClassesTypeFunctions.GetAllFeaturesItems();

            this.ListAllFeaturesItems = new ListDragDropViewModel(1, FeatureKey)
                                            {
                                                Key = FeatureKey,
                                                ChangeVisibilityHeader = Visibility.Collapsed,
                                                Items = new ObservableCollection<ItemDragDrop>(),
                                                IsConstantSource = true
                                            };

            foreach (var item in listFeaturesItem)
            {
                this.ListAllFeaturesItems.Items.Add(
                    new ItemDragDrop { ID = item.ItemId, Name = item.Text, Key = FeatureKey });
            }

            this.ListAllFeaturesItems.Items = new ObservableCollection<ItemDragDrop>(this.ListAllFeaturesItems.Items.OrderBy(a => a.Name));

            this.ListItemsDragDrop = new GroupDragDropViewModel();
            ObservableCollection<ListDragDropViewModel> listItemsDragDropViewModel = new ObservableCollection<ListDragDropViewModel>();

            foreach (var itemSelected in allItemsSelected)
            {
                var featuresItem = new ListDragDropViewModel(itemSelected.EquipTypeId, itemSelected.TypeDescription);
                featuresItem.Key = FeatureKey;
                featuresItem.ChangeVisibilityHeader = Visibility.Collapsed;
                featuresItem.Items = new ObservableCollection<ItemDragDrop>();

                var listItemSelected = await AssetClassesTypeFunctions.GetListFeaturesItemsSelected(itemSelected.EquipTypeId);

                if (listItemSelected.Count > 0)
                {
                    foreach (var itemId in listItemSelected)
                    {
                        var item = listFeaturesItem.FirstOrDefault(x => x.ItemId == itemId);
                        if (item != null)
                        {
                            featuresItem.Items.Add(new ItemDragDrop { ID = item.ItemId, Name = item.Text, Key = FeatureKey });
                        }
                    }
                }

                listItemsDragDropViewModel.Add(featuresItem);
            }

            this.ListItemsDragDrop.GroupDragDropSource = new ObservableCollection<ListDragDropViewModel>(listItemsDragDropViewModel);
            this.ListItemsDragDrop.GroupDragDropSource = listItemsDragDropViewModel;
            this.ListItemsDragDrop.NotifyItemsChanged();
            this.ListItemsDragDrop.PropertyChanged += this.AssetCategoryAssignFeatureViewModel_PropertyChanged;
        }

        /// <summary>
        /// The asset category assign feature view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void AssetCategoryAssignFeatureViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ContentItemChanged != null)
            {
                this.ContentItemChanged(sender, e);
            }
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
