// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemAssetFeatureDragDropViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the ItemAssetFeatureDragDropViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetFeatures
{
    using System.ComponentModel;

    using Insyston.Operations.WPF.ViewModels.Common.Controls.DragDrop;

    /// <summary>
    /// The item asset feature drag drop view model.
    /// </summary>
    public class ItemAssetFeatureDragDropViewModel : INotifyPropertyChanged
    {
        #region Variables

        /// <summary>
        /// The _id.
        /// </summary>
        private int _id;

        /// <summary>
        /// The _header name.
        /// </summary>
        private string _headerName;

        /// <summary>
        /// The _asset type view model.
        /// </summary>
        private ListDragDropViewModel _assetTypeViewModel;

        /// <summary>
        /// The asset category view model.
        /// </summary>
        private ListDragDropViewModel _assetCategoryViewModel;

        #endregion

        #region Event

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id
        {
            get
            {
                return this._id;
            }

            set
            {
                if (this._id != value)
                {
                    this._id = value;
                    this.OnPropertyChanged("Id");
                }
            }
        }

        /// <summary>
        /// Gets or sets the header name.
        /// </summary>
        public string HeaderName
        {
            get
            {
                return this._headerName;
            }

            set
            {
                if (this._headerName != value)
                {
                    this._headerName = value;
                    this.OnPropertyChanged("HeaderName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the asset type view model.
        /// </summary>
        public ListDragDropViewModel AssetTypeViewModel
        {
            get
            {
                return this._assetTypeViewModel;
            }

            set
            {
                if (this._assetTypeViewModel != value)
                {
                    this._assetTypeViewModel = value;
                    this.OnPropertyChanged("AssetTypeViewModel");
                }
            }
        }

        /// <summary>
        /// Gets or sets the asset category view model.
        /// </summary>
        public ListDragDropViewModel AssetCategoryViewModel
        {
            get
            {
                return this._assetCategoryViewModel;
            }

            set
            {
                if (this._assetCategoryViewModel != value)
                {
                    this._assetCategoryViewModel = value;
                    this.OnPropertyChanged("AssetCategoryViewModel");
                }
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
    }
}
