// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetMakeAssignModelViewModel.cs" company="Insyston">
// Insyston 
// </copyright>
// <summary>
//   The asset make assign model view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.Asset_Make
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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
    /// The asset make assign model view model.
    /// </summary>
    public class AssetMakeAssignModelViewModel : ViewModelUseCaseBase
    {
        #region Variables

        /// <summary>
        /// The model key.
        /// </summary>
        private const string ModelKey = "Model";

        /// <summary>
        /// The list all model items.
        /// </summary>
        private ListDragDropViewModel _listAllModelItems;

        /// <summary>
        /// The list items drag drop.
        /// </summary>
        private GroupDragDropViewModel _listItemsDragDrop;

        #endregion

        #region Property

        /// <summary>
        /// Gets or sets the list all model items.
        /// </summary>
        public ListDragDropViewModel ListAllModelItems
        {
            get
            {
                return this._listAllModelItems;
            }

            set
            {
                this.SetField(ref this._listAllModelItems, value, () => this.ListAllModelItems);
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

        #region Constructor
        #endregion

        #region Public Method
        /// <summary>
        /// The get list model items.
        /// </summary>
        /// <param name="allItemsSelected">
        /// The all items selected.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetListModelItems(ObservableCollection<AssetClassesMakeRowItem> allItemsSelected)
        {
            List<AssetClassesMakeItemDetail> listModelItem = await AssetClassesMakeFunctions.GetAllModelItems();

            this.ListAllModelItems = new ListDragDropViewModel(1, ModelKey);

            this.ListAllModelItems.Key = ModelKey;
            this.ListAllModelItems.ChangeVisibilityHeader = Visibility.Collapsed;
            this.ListAllModelItems.Items = new ObservableCollection<ItemDragDrop>();
            this.ListAllModelItems.IsConstantSource = true;

            foreach (var item in listModelItem)
            {
                this.ListAllModelItems.Items.Add(
                    new ItemDragDrop { ID = item.ItemId, Name = item.Text, Key = ModelKey });
            }

            this.ListAllModelItems.Items = new ObservableCollection<ItemDragDrop>(this.ListAllModelItems.Items.OrderBy(a => a.Name));

            this.ListItemsDragDrop = new GroupDragDropViewModel();
            ObservableCollection<ListDragDropViewModel> listItemsDragDropViewModel = new ObservableCollection<ListDragDropViewModel>();
            foreach (var itemSelected in allItemsSelected)
            {
                var modelItem = new ListDragDropViewModel(itemSelected.EquipMakeId, itemSelected.Description);
                modelItem.Key = ModelKey;
                modelItem.ChangeVisibilityHeader = Visibility.Collapsed;
                modelItem.Items = new ObservableCollection<ItemDragDrop>();

                var listItemSelected = AssetClassesMakeFunctions.GetListModelItemsSelected(itemSelected.EquipMakeId);

                if (listItemSelected.Count > 0)
                {
                    foreach (var itemId in listItemSelected)
                    {
                        var item = listModelItem.FirstOrDefault(x => x.ItemId == itemId);
                        if (item != null)
                        {
                            modelItem.Items.Add(new ItemDragDrop { ID = item.ItemId, Name = item.Text, Key = ModelKey });
                        }
                    }
                }

                listItemsDragDropViewModel.Add(modelItem);
            }

            this.ListItemsDragDrop.GroupDragDropSource = new ObservableCollection<ListDragDropViewModel>(listItemsDragDropViewModel);
            this.ListItemsDragDrop.GroupDragDropSource = listItemsDragDropViewModel;
            this.ListItemsDragDrop.NotifyItemsChanged();
        }
        #endregion

        #region Private Method
        #endregion

        #region Other
        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Un lock 
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
        /// lock function
        /// </exception>
        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
