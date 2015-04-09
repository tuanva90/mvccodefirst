// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupAssetFeatureDragDropViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the GroupAssetFeatureDragDropViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetFeatures
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// The group asset feature drag drop view model.
    /// </summary>
    public class GroupAssetFeatureDragDropViewModel : INotifyPropertyChanged
    {
        #region Variables

        /// <summary>
        /// The _group asset drag drop source.
        /// </summary>
        private ObservableCollection<ItemAssetFeatureDragDropViewModel> _groupAssetDragDropSource;

        #endregion

        #region Event

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the group asset drag drop source.
        /// </summary>
        public ObservableCollection<ItemAssetFeatureDragDropViewModel> GroupAssetDragDropSource
        {
            get
            {
                return this._groupAssetDragDropSource;
            }

            set
            {
                if (this._groupAssetDragDropSource != value)
                {
                    this._groupAssetDragDropSource = value;
                    this.OnPropertyChanged("GroupAssetDragDropSource");
                }
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// The notify items changed.
        /// </summary>
        public void NotifyItemsChanged()
        {
            foreach (var source in this.GroupAssetDragDropSource)
            {
                source.AssetCategoryViewModel.Items.CollectionChanged += this.Items_CollectionChanged;
                source.AssetTypeViewModel.Items.CollectionChanged += this.Items_CollectionChanged;
            }
        }

        #endregion

        #region Private Method
        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Other

        /// <summary>
        /// The items_ collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("Items");
        }

        #endregion
    }
}
