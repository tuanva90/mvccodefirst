// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetTypeAssignMakeViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetTypeAssignMakeViewModel type.
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
    /// The asset type assign make view model.
    /// </summary>
    public class AssetTypeAssignMakeViewModel : ViewModelUseCaseBase
    {
        #region Private Properties
        /// <summary>
        /// The feature key.
        /// </summary>
        private const string FeatureKey = "Make";

        /// <summary>
        /// The _list all asset types items.
        /// </summary>
        private ListDragDropViewModel _listAllAssetTypesItems;

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
        /// The get list make items.
        /// </summary>
        /// <param name="allItemsSelected">
        /// The all items selected.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetListMakeItems(ObservableCollection<AssetClassesTypeRowItem> allItemsSelected)
        {
            List<AssetClassesCategoryItemDetail> listMakeItem = await AssetClassesTypeFunctions.GetListMakeItems();

            this.ListAllAssetTypesItems = new ListDragDropViewModel(1, FeatureKey);

            this.ListAllAssetTypesItems.Key = FeatureKey;
            this.ListAllAssetTypesItems.ChangeVisibilityHeader = Visibility.Collapsed;
            this.ListAllAssetTypesItems.Items = new ObservableCollection<ItemDragDrop>();
            this.ListAllAssetTypesItems.IsConstantSource = true;
            foreach (var item in listMakeItem)
            {
                this.ListAllAssetTypesItems.Items.Add(
                    new ItemDragDrop { ID = item.ItemId, Name = item.Text, Key = FeatureKey });
            }

            this.ListAllAssetTypesItems.Items = new ObservableCollection<ItemDragDrop>(this.ListAllAssetTypesItems.Items.OrderBy(a => a.Name));

            this.ListItemsDragDrop = new GroupDragDropViewModel();
            ObservableCollection<ListDragDropViewModel> listItemsDragDropViewModel = new ObservableCollection<ListDragDropViewModel>();

            foreach (var itemSelected in allItemsSelected)
            {
                var featuresItem = new ListDragDropViewModel(itemSelected.EquipTypeId, itemSelected.TypeDescription);
                featuresItem.Key = FeatureKey;
                featuresItem.ChangeVisibilityHeader = Visibility.Collapsed;
                featuresItem.Items = new ObservableCollection<ItemDragDrop>();

                var listItemSelected = await AssetClassesTypeFunctions.GetListMakeItemsSelected(itemSelected.EquipTypeId);

                if (listItemSelected.Count > 0)
                {
                    foreach (var itemId in listItemSelected)
                    {
                        var item = listMakeItem.FirstOrDefault(x => x.ItemId == itemId);
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
            this.ListItemsDragDrop.PropertyChanged += this.AssetCategoryAssignTypesViewModel_PropertyChanged;
        }

        /// <summary>
        /// The asset category assign types view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void AssetCategoryAssignTypesViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ContentItemChanged != null)
            {
                this.ContentItemChanged(sender, e);
            }
        }
        #endregion

        #region Protected 
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
