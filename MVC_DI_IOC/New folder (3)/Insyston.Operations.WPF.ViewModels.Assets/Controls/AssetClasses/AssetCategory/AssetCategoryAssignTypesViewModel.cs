// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetCategoryAssignTypesViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   The asset category assign types view model.
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
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Controls.DragDrop;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The asset category assign types view model.
    /// </summary>
    public class AssetCategoryAssignTypesViewModel : ViewModelUseCaseBase
    {
        #region Private Properties
        /// <summary>
        /// The type key.
        /// </summary>
        private const string TypeKey = "Type";

        /// <summary>
        /// The list all asset types items.
        /// </summary>
        private ListDragDropViewModel _listAllAssetTypesItems;

        /// <summary>
        /// The list items drag drop.
        /// </summary>
        private GroupDragDropViewModel _listItemsDragDrop;
        #endregion

        #region Public Properties
        /// <summary>
        /// The content item changed.
        /// </summary>
        public Action<object, PropertyChangedEventArgs> ContentItemChanged;

        /// <summary>
        /// Gets or sets the list all asset types items.
        /// </summary>
        public ListDragDropViewModel ListAllAssetTypesItems
        {
            get
            {
                return this._listAllAssetTypesItems;
            }

            set
            {
                this.SetField(ref this._listAllAssetTypesItems, value, () => this.ListAllAssetTypesItems);
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
        /// The get list types items.
        /// </summary>
        /// <param name="allItemsSelected">
        /// The all items selected.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetListTypesItems(ObservableCollection<AssetClassesCategoryRowItem> allItemsSelected)
        {
            List<AssetClassesCategoryItemDetail> listFeaturesItem;
            listFeaturesItem = await AssetClassesCategoryFunctions.GetAllAssetTypesItems();

            this.ListAllAssetTypesItems = new ListDragDropViewModel(1, TypeKey);

            this.ListAllAssetTypesItems.Key = TypeKey;
            this.ListAllAssetTypesItems.ChangeVisibilityHeader = Visibility.Collapsed;
            this.ListAllAssetTypesItems.Items = new ObservableCollection<ItemDragDrop>();
            this.ListAllAssetTypesItems.IsConstantSource = true;
            foreach (var item in listFeaturesItem)
            {
                this.ListAllAssetTypesItems.Items.Add(
                    new ItemDragDrop { ID = item.ItemId, Name = item.Text, Key = TypeKey });
            }

            this.ListAllAssetTypesItems.Items = new ObservableCollection<ItemDragDrop>(this.ListAllAssetTypesItems.Items.OrderBy(a => a.Name).ToList());

            this.ListItemsDragDrop = new GroupDragDropViewModel();
            ObservableCollection<ListDragDropViewModel> listItemsDragDropViewModel = new ObservableCollection<ListDragDropViewModel>();

            foreach (var itemSelected in allItemsSelected)
            {
                var featuresItem = new ListDragDropViewModel(itemSelected.EquipCategoryId, itemSelected.Category);
                featuresItem.Key = TypeKey;
                featuresItem.ChangeVisibilityHeader = Visibility.Collapsed;
                featuresItem.Items = new ObservableCollection<ItemDragDrop>();

                var listItemSelected = await AssetClassesCategoryFunctions.GetListAssetTypesItemsSelected(itemSelected.EquipCategoryId);

                if (listItemSelected.Count > 0)
                {
                    foreach (var itemId in listItemSelected)
                    {
                        var item = listFeaturesItem.FirstOrDefault(x => x.ItemId == itemId);
                        if (item != null)
                        {
                            featuresItem.Items.Add(new ItemDragDrop { ID = item.ItemId, Name = item.Text, Key = TypeKey });
                        }
                    }
                }

                listItemsDragDropViewModel.Add(featuresItem);
            }

            this.ListItemsDragDrop.GroupDragDropSource = new ObservableCollection<ListDragDropViewModel>(listItemsDragDropViewModel);
            this.ListItemsDragDrop.GroupDragDropSource = listItemsDragDropViewModel;
            this.ListItemsDragDrop.NotifyItemsChanged();
            this.ListItemsDragDrop.PropertyChanged += this.AssetCategoryAssignTypesViewModel_PropertyChanged;
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

        #region Private Methods
        /// <summary>
        /// The asset category assign types view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetCategoryAssignTypesViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ContentItemChanged != null)
            {
                this.ContentItemChanged(sender, e);
            }
        }
        #endregion
    }
}
