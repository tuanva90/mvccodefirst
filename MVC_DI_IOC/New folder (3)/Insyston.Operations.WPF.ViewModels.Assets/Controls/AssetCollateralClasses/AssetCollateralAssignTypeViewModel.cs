// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetCollateralAssignTypeViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   The asset collateral assign type view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetCollateralClasses
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Bussiness.Assets.AssetCollateralClasses;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Controls.DragDrop;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The asset collateral assign type view model.
    /// </summary>
    public class AssetCollateralAssignTypeViewModel : ViewModelUseCaseBase
    {
        #region Private Properties
        /// <summary>
        /// The type key.
        /// </summary>
        private const string TypeKey = "Type";

        /// <summary>
        /// The _list all types items.
        /// </summary>
        private ListDragDropViewModel _listAllTypesItems;

        /// <summary>
        /// The _list items drag drop.
        /// </summary>
        private GroupDragDropViewModel _listItemsDragDrop;
        #endregion

        #region Public Properties
        /// <summary>
        /// The selected item changed.
        /// </summary>
        public Action<object> SelectedItemChanged;

        /// <summary>
        /// Gets or sets the list all types items.
        /// </summary>
        public ListDragDropViewModel ListAllTypesItems
        {
            get
            {
                return this._listAllTypesItems;
            }

            set
            {
                this.SetField(ref this._listAllTypesItems, value, () => this.ListAllTypesItems);
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

        #region Public Method
        /// <summary>
        /// The get list collateral items.
        /// </summary>
        /// <param name="allItemsSelected">
        /// The all items selected.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetListCollateralItems(ObservableCollection<AssetCollateralRowItem> allItemsSelected)
        {
            List<AssetClassesTypeItemDetail> listTypesItem;
            listTypesItem = await AssetCollateralClassesFunction.GetAllTypesItems();
            this.ListAllTypesItems = new ListDragDropViewModel(1, TypeKey);
            this.ListAllTypesItems.Key = TypeKey;
            this.ListAllTypesItems.ChangeVisibilityHeader = Visibility.Collapsed;
            this.ListAllTypesItems.Items = new ObservableCollection<ItemDragDrop>();
            this.ListAllTypesItems.IsConstantSource = true;

            foreach (var item in listTypesItem)
            {
                this.ListAllTypesItems.Items.Add(
                    new ItemDragDrop { ID = item.ItemId, Name = item.Text, Key = TypeKey });
            }

            this.ListItemsDragDrop = new GroupDragDropViewModel();
            ObservableCollection<ListDragDropViewModel> listItemsDragDropViewModel = new ObservableCollection<ListDragDropViewModel>();

            foreach (var itemSelected in allItemsSelected)
            {
                var typesItem = new ListDragDropViewModel(itemSelected.CollateralClassID, itemSelected.Description);
                typesItem.Key = TypeKey;
                typesItem.ChangeVisibilityHeader = Visibility.Collapsed;
                typesItem.Items = new ObservableCollection<ItemDragDrop>();

                var listItemSelected = await AssetCollateralClassesFunction.GetListTypesItemsSelected(itemSelected.CollateralClassID);

                if (listItemSelected.Count > 0)
                {
                    foreach (var itemId in listItemSelected)
                    {
                        var item = listTypesItem.FirstOrDefault(x => x.ItemId == itemId);
                        if (item != null)
                        {
                            typesItem.Items.Add(new ItemDragDrop { ID = item.ItemId, Name = item.Text, Key = TypeKey });
                        }
                    }
                }

                for (int i = 0; i < this.ListAllTypesItems.Items.Count; i++)
                {
                    if (listItemSelected.Contains(this.ListAllTypesItems.Items[i].ID))
                    {
                        this.ListAllTypesItems.Items[i].IsNoneDropItem = true;
                    }
                }

                for (int i = 0; i < this.ListAllTypesItems.Items.Count; i++)
                {
                    if (listItemSelected.Contains(this.ListAllTypesItems.Items[i].ID))
                    {
                        this.ListAllTypesItems.Items[i].IsNoneDropItem = true;
                    }
                }

                listItemsDragDropViewModel.Add(typesItem);
            }
            
            this.ListItemsDragDrop.GroupDragDropSource = new ObservableCollection<ListDragDropViewModel>(listItemsDragDropViewModel);
            this.ListItemsDragDrop.GroupDragDropSource = listItemsDragDropViewModel;
            foreach (var source in this.ListItemsDragDrop.GroupDragDropSource)
            {
                source.Items.CollectionChanged += this.ItemsAdd_CollectionChanged;
            }

            this.ListItemsDragDrop.NotifyItemsChanged();
            this.ListItemsDragDrop.PropertyChanged += this.AssetCollateralAssignTypeViewModel_PropertyChanged;
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
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Lock function
        /// </exception>
        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Method
        /// <summary>
        /// The asset collateral assign type view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetCollateralAssignTypeViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.SelectedItemChanged != null)
            {
                this.SelectedItemChanged(sender);
            }
        }

        /// <summary>
        /// The items add_ collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ItemsAdd_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<ItemDragDrop> source = new ObservableCollection<ItemDragDrop>();
            foreach (var itemSource in this.ListItemsDragDrop.GroupDragDropSource)
            {
                for (int i = 0; i < itemSource.Items.Count; i++)
                {
                    source.Add(itemSource.Items[i]);
                }
            }

            for (int i = 0; i < this.ListAllTypesItems.Items.Count; i++)
            {
                this.ListAllTypesItems.Items[i].IsNoneDropItem = false;
            }

            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < this.ListAllTypesItems.Items.Count; j++)
                {
                    if (this.ListAllTypesItems.Items[j].ID.Equals(source[i].ID))
                    {
                        this.ListAllTypesItems.Items[j].IsNoneDropItem = true;
                    }
                }
            }
        }
        #endregion
    }
}
