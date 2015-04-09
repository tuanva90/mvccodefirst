// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListDragDropViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the ListDragDropViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Controls.DragDrop
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;

    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The list drag drop view model.
    /// </summary>
    public class ListDragDropViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The _ id.
        /// </summary>
        private int _id;

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
                    _id = value;
                    this.OnPropertyChanged("Id");
                }
            }
        }

        /// <summary>
        /// The header name.
        /// </summary>
        private string _headerName;

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
                    _headerName = value;
                    this.OnPropertyChanged("HeaderName");
                }
            }
        }

        /// <summary>
        /// The _is constant source.
        /// </summary>
        private bool _isConstantSource;

        /// <summary>
        /// Gets or sets a value indicating whether is constant source.
        /// </summary>
        public bool IsConstantSource
        {
            get
            {
                return this._isConstantSource;
            }
            set
            {
                if (this._isConstantSource != value)
                {
                    _isConstantSource = value;
                    this.OnPropertyChanged("IsConstantSource");
                }
            }
        }

        /// <summary>
        /// The _change visibility header.
        /// </summary>
        private Visibility _changeVisibilityHeader;

        /// <summary>
        /// Gets or sets the change visibility header.
        /// </summary>
        public Visibility ChangeVisibilityHeader
        {
            get
            {
                return this._changeVisibilityHeader;
            }
            set
            {
                if (this._changeVisibilityHeader != value)
                {
                    _changeVisibilityHeader = value;
                    this.OnPropertyChanged("ChangeVisibilityHeader");
                }
            }
        }
        
        /// <summary>
        /// The _items.
        /// </summary>
        private ObservableCollection<ItemDragDrop> _items;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListDragDropViewModel"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="headerName">
        /// The header Name.
        /// </param>
        public ListDragDropViewModel(int id, string headerName)
        {
            this._id = id;
            this._headerName = headerName;
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public ObservableCollection<ItemDragDrop> Items
        {
            get
            {
                return this._items;
            }
            set
            {
                if (this._items != value)
                {
                    this._items = value;
                    this.OnPropertyChanged("Items");
                }
            }
        }

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

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
