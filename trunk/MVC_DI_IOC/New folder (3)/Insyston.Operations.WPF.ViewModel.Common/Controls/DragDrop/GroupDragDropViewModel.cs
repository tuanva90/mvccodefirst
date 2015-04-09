// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupDragDropViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the GroupDragDropViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace Insyston.Operations.WPF.ViewModels.Common.Controls
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Insyston.Operations.WPF.ViewModels.Common.Controls.DragDrop;

    /// <summary>
    /// The group drag drop view model.
    /// </summary>
    public class GroupDragDropViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The _ group drag drop source.
        /// </summary>
        private ObservableCollection<ListDragDropViewModel> _groupDragDropSource;

        /// <summary>
        /// Gets or sets the group drag drop source.
        /// </summary>
        public ObservableCollection<ListDragDropViewModel> GroupDragDropSource
        {
            get
            {
                return this._groupDragDropSource;
            }
            set
            {
                if (this._groupDragDropSource != value)
                {
                    this._groupDragDropSource = value;
                    this.OnPropertyChanged("GroupDragDropSource");
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

        public void NotifyItemsChanged()
        {
            foreach (var source in GroupDragDropSource)
            {
                source.Items.CollectionChanged += Items_CollectionChanged;
            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("Items");
        }

    }
}
