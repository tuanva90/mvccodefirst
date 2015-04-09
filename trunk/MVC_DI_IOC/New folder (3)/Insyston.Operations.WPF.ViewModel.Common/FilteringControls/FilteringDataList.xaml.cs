// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilteringDataList.xaml.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for FilteringDataList.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.FilteringControls
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    using Insyston.Operations.Common;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Model.Filtering;

    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.GridView;
    using Telerik.Windows.Data;

    /// <summary>
    /// Interaction logic for FilteringDataList
    /// </summary>
    public partial class FilteringDataList : UserControl, IFilteringControl, INotifyPropertyChanged
    {
        #region Dependency Properties

        /// <summary>
        /// The is active property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(
                "IsActive",
                typeof(bool),
                typeof(FilteringDataList),
                new PropertyMetadata(false));

        /// <summary>
        /// The items source property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(FilteringDataList),
            null);

        /// <summary>
        /// The group name property.
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(
            "GroupName",
            typeof(string),
            typeof(FilteringDataList),
            null);

        #endregion

        #region Private Properties

        /// <summary>
        /// The _filter.
        /// </summary>
        private CompositeFilterDescriptor _filter;

        /// <summary>
        /// The _column.
        /// </summary>
        private GridViewBoundColumnBase _column;

        /// <summary>
        /// The _is multi select.
        /// </summary>
        private bool _isMultiSelect;

        /// <summary>
        /// The _is select all.
        /// </summary>
        private bool _isSelectAll;

        /// <summary>
        /// Gets or sets a value indicating whether is item select.
        /// </summary>
        private bool IsItemSelect { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringDataList"/> class.
        /// </summary>
        public FilteringDataList()
        {
            InitializeComponent();
            this.GroupName = this.GetHashCode().ToString(CultureInfo.InvariantCulture);
            this.DataContext = this;
            this.IsItemSelect = true;
            this.IsSelectAll = true;
        }

        #region Public Properties

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return (bool)this.GetValue(IsActiveProperty);
            }
            set
            {
                this.SetValue(IsActiveProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)this.GetValue(ItemsSourceProperty);
            }
            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        public string GroupName
        {
            get
            {
                return this.GetValue(GroupNameProperty).ToString();
            }
            set
            {
                this.SetValue(GroupNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is multi select.
        /// </summary>
        public bool IsMultiSelect
        {
            get
            {
                return this._isMultiSelect;
            }
            set
            {
                if (value != this._isMultiSelect)
                {
                    this._isMultiSelect = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("IsMultiSelect"));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is select all.
        /// </summary>
        public bool IsSelectAll
        {
            get
            {
                return this._isSelectAll;
            }
            set
            {
                if (value != this._isSelectAll)
                {
                    this._isSelectAll = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("IsSelectAll"));
                    }

                    if (!IsItemSelect)
                    {
                        this._column.DataControl.FilterDescriptors.SuspendNotifications();
                        foreach (FilteringDataItem item in this.ItemsSource)
                        {
                            if (this.IsSelectAll)
                            {
                                item.IsSelected = true;
                                this.IsSelectAll = true;
                            }
                            else
                            {
                                item.IsSelected = false;
                            }
                        }
                        this._column.DataControl.FilterDescriptors.ResumeNotifications();
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The prepare.
        /// </summary>
        /// <param name="columnToPrepare">
        /// The column to prepare.
        /// </param>
        public void Prepare(Telerik.Windows.Controls.GridViewColumn columnToPrepare)
        {
            this._column = columnToPrepare as GridViewBoundColumnBase;
            this.IsItemSelect = false;
            if (this._column == null)
            {
                return;
            }

            this.TitleTextBlock.Text = this.Title;
            if (this.ItemsSource == null)
            {
                return;
            }

            int count = 0;
            foreach (FilteringDataItem item in this.ItemsSource)
            {
                item.PropertyChanged -= this.SelectFilterPropertyChanged;
                item.PropertyChanged += this.SelectFilterPropertyChanged;
                if (item.IsSelected) count++;
            }
            if (count != this.ItemsSource.Cast<FilteringDataItem>().Count())
            {
                this.IsItemSelect = true;
                this.IsSelectAll = false;
            }
            this.IsItemSelect = false;

            foreach (CompositeFilterDescriptor item in ((CompositeFilterDescriptor)this._column.DataControl.FilterDescriptors[1]).FilterDescriptors)
            {
                if (item.FilterDescriptors.Count > 0)
                {
                    if (((FilterDescriptor)item.FilterDescriptors[0]).Member == this._column.FilterMemberPath)
                    {
                        this._filter = item;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// The clear filter on column.
        /// </summary>
        public void ClearFilterOnColumn()
        {
            ClearAllFilter();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The select filter property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void SelectFilterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyFinder.IsProperty(e.PropertyName, () => ((FilteringDataItem)sender).IsSelected))
            {
                this.CreateFilters();
            }
        }

        /// <summary>
        /// The on filter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnFilter(object sender, RoutedEventArgs e)
        {
            var popup = ((Button)sender).ParentOfType<System.Windows.Controls.Primitives.Popup>();
            popup.IsOpen = false;
        }

        /// <summary>
        /// The create filters.
        /// </summary>
        private void CreateFilters()
        {
            this.IsItemSelect = true;
            this._column.DataControl.FilterDescriptors.SuspendNotifications();
            if (this._filter == null)
            {
                this._filter = new CompositeFilterDescriptor();
                this._filter.LogicalOperator = FilterCompositionLogicalOperator.Or;
                ((CompositeFilterDescriptor)this._column.DataControl.FilterDescriptors[1]).FilterDescriptors.Add(this._filter);
            }

            this._filter.FilterDescriptors.Clear();

            List<FilteringDataItem> selectedItems = this.ItemsSource.Cast<FilteringDataItem>().Where(d => d.IsSelected).ToList();
            
            List<FilterDescriptor> filterDescriptors = new List<FilterDescriptor>();
            foreach (FilteringDataItem item in selectedItems)
            {
                if (item.IsSelected)
                {
                    if (this._column.DataMemberBinding.Path.Path == this._column.FilterMemberPath)
                    {
                        filterDescriptors.Add(new FilterDescriptor(this._column.FilterMemberPath, FilterOperator.IsEqualTo, item.Text));
                    }
                    else
                    {
                        filterDescriptors.Add(new FilterDescriptor(this._column.FilterMemberPath, FilterOperator.IsEqualTo, item.Id));
                    }
                }
            }
            this._filter.FilterDescriptors.AddRange(filterDescriptors);

            // Check if all item is selected
            if (selectedItems.Count() == this.ItemsSource.Cast<FilteringDataItem>().Count())
            {
                IsSelectAll = true;
            }
            else
            {
                IsSelectAll = false;
            }

            this._column.DataControl.FilterDescriptors.ResumeNotifications();
            this.IsActive = true;
            this.IsItemSelect = false;
        }

        /// <summary>
        /// The on clear filter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnClearFilter(object sender, RoutedEventArgs e)
        {
            ClearAllFilter();
            this.IsSelectAll = true;
            var popup = ((Button)sender).ParentOfType<System.Windows.Controls.Primitives.Popup>();
            popup.IsOpen = false;
        }

        /// <summary>
        /// The clear all filter.
        /// </summary>
        private void ClearAllFilter()
        {
            if (this._filter != null)
            {
                this._column.DataControl.FilterDescriptors.SuspendNotifications();
                foreach (FilteringDataItem item in this.ItemsSource)
                {
                    item.PropertyChanged -= this.SelectFilterPropertyChanged;
                    item.IsSelected = true;
                }
                if (this._filter != null)
                {
                    this._filter.FilterDescriptors.Clear();
                }
                this.IsActive = false;
                this._column.DataControl.FilterDescriptors.ResumeNotifications();
            }
        }

        #endregion
    }
}
