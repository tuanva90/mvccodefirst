// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextFieldDataListFilter.xaml.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for TextFieldDataListFilter.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.FilteringControls
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Insyston.Operations.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Model.Filtering;

    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.GridView;
    using Telerik.Windows.Data;

    /// <summary>
    /// Interaction logic for TextFieldDataListFilter.xaml
    /// </summary>
    public partial class TextFieldDataListFilter : UserControl, IFilteringControl, INotifyPropertyChanged
    {
        /// <summary>
        /// The _local selected operator 1.
        /// </summary>
        private FilterOperator? _localSelectedOperator1;

        /// <summary>
        /// The _local selected operator 2.
        /// </summary>
        private FilterOperator? _localSelectedOperator2;

        /// <summary>
        /// The _local selected logical operator.
        /// </summary>
        private FilterCompositionLogicalOperator? _localSelectedLogicalOperator;

        /// <summary>
        /// The _value 1.
        /// </summary>
        private string _value1;

        /// <summary>
        /// The _value 2.
        /// </summary>
        private string _value2;

        /// <summary>
        /// The _text value 1.
        /// </summary>
        private string _textValue1;

        /// <summary>
        /// The _text value 2.
        /// </summary>
        private string _textValue2;

        /// <summary>
        /// Gets or sets a value indicating whether _ is number.
        /// </summary>
        private bool _IsNumber { get; set; }

        /// <summary>
        /// The _is visibility number.
        /// </summary>
        private Visibility _isVisibilityNumber;

        /// <summary>
        /// The _is visibility text.
        /// </summary>
        private Visibility _isVisibilityText;

        #region Dependency Properties

        /// <summary>
        /// The is active property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(
                "IsActive",
                typeof(bool),
                typeof(TextFieldDataListFilter),
                new PropertyMetadata(false));

        /// <summary>
        /// The items source property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(TextFieldDataListFilter),
            null);

        /// <summary>
        /// The group name property.
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(
            "GroupName",
            typeof(string),
            typeof(TextFieldDataListFilter),
            null);

        /// <summary>
        /// The all operators 1 property.
        /// </summary>
        public static readonly DependencyProperty AllOperators1Property =
            DependencyProperty.Register(
                "AllOperators1",
                typeof(IEnumerable),
                typeof(TextFieldDataListFilter),
                new PropertyMetadata(null));

        /// <summary>
        /// The all operators 2 property.
        /// </summary>
        public static readonly DependencyProperty AllOperators2Property =
            DependencyProperty.Register(
                "AllOperators2",
                typeof(IEnumerable),
                typeof(TextFieldDataListFilter),
                new PropertyMetadata(null));

        /// <summary>
        /// The all logical operator property.
        /// </summary>
        public static readonly DependencyProperty AllLogicalOperatorProperty =
            DependencyProperty.Register("AllLogicalOperator", typeof(IEnumerable), typeof(TextFieldDataListFilter), new PropertyMetadata(null));

        /// <summary>
        /// The selected operator 1 property.
        /// </summary>
        public static readonly DependencyProperty SelectedOperator1Property =
            DependencyProperty.Register("SelectedOperator1", typeof(FilterOperator?), typeof(TextFieldDataListFilter), null);

        /// <summary>
        /// The selected operator 2 property.
        /// </summary>
        public static readonly DependencyProperty SelectedOperator2Property =
            DependencyProperty.Register("SelectedOperator2", typeof(FilterOperator?), typeof(TextFieldDataListFilter), null);

        /// <summary>
        /// The selected logical operator property.
        /// </summary>
        public static readonly DependencyProperty SelectedLogicalOperatorProperty =
            DependencyProperty.Register("SelectedLogicalOperator", typeof(FilterCompositionLogicalOperator?), typeof(TextFieldDataListFilter), null);

        /// <summary>
        /// The number value 1 property.
        /// </summary>
        public static readonly DependencyProperty NumberValue1Property =
            DependencyProperty.Register("NumberValue1", typeof(int?), typeof(TextFieldDataListFilter), null);

        /// <summary>
        /// The number value 2 property.
        /// </summary>
        public static readonly DependencyProperty NumberValue2Property =
            DependencyProperty.Register("NumberValue2", typeof(int?), typeof(TextFieldDataListFilter), null);

        /// <summary>
        /// The text value 1 property.
        /// </summary>
        public static readonly DependencyProperty TextValue1Property =
            DependencyProperty.Register("TextValue1", typeof(string), typeof(TextFieldDataListFilter), null);

        /// <summary>
        /// The text value 2 property.
        /// </summary>
        public static readonly DependencyProperty TextValue2Property =
            DependencyProperty.Register("TextValue2", typeof(string), typeof(TextFieldDataListFilter), null);

        #endregion

        #region Private Properties

        /// <summary>
        /// The _filter.
        /// </summary>
        private CompositeFilterDescriptor _filter1;

        /// <summary>
        /// The _filter 2.
        /// </summary>
        private CompositeFilterDescriptor _filter2;

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
        /// Initializes a new instance of the <see cref="TextFieldDataListFilter"/> class.
        /// </summary>
        public TextFieldDataListFilter()
        {
            InitializeComponent();
            this.DataContext = this;
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

        /// <summary>
        /// Gets or sets the all operators 1.
        /// </summary>
        public IEnumerable AllOperators1
        {
            get
            {
                return (IEnumerable)this.GetValue(AllOperators1Property);
            }
            set
            {
                this.SetValue(AllOperators1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the all operators 2.
        /// </summary>
        public IEnumerable AllOperators2
        {
            get
            {
                return (IEnumerable)this.GetValue(AllOperators2Property);
            }
            set
            {
                this.SetValue(AllOperators2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the all logical operator.
        /// </summary>
        public IEnumerable AllLogicalOperator
        {
            get
            {
                return (IEnumerable)this.GetValue(AllLogicalOperatorProperty);
            }
            set
            {
                this.SetValue(AllLogicalOperatorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected operator 1.
        /// </summary>
        public FilterOperator? SelectedOperator1
        {
            get
            {
                return (FilterOperator?)this.GetValue(SelectedOperator1Property);
            }
            set
            {
                this.SetValue(SelectedOperator1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected operator 2.
        /// </summary>
        public FilterOperator? SelectedOperator2
        {
            get
            {
                return (FilterOperator?)this.GetValue(SelectedOperator2Property);
            }
            set
            {
                this.SetValue(SelectedOperator2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected logical operator.
        /// </summary>
        public FilterCompositionLogicalOperator? SelectedLogicalOperator
        {
            get
            {
                return (FilterCompositionLogicalOperator?)this.GetValue(SelectedLogicalOperatorProperty);
            }
            set
            {
                this.SetValue(SelectedLogicalOperatorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the number value 1.
        /// </summary>
        public int? NumberValue1
        {
            get
            {
                return (int?)this.GetValue(NumberValue1Property);
            }
            set
            {
                this.SetValue(NumberValue1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the number value 2.
        /// </summary>
        public int? NumberValue2
        {
            get
            {
                return (int?)this.GetValue(NumberValue2Property);
            }
            set
            {
                this.SetValue(NumberValue2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the text value 1.
        /// </summary>
        public string TextValue1
        {
            get
            {
                return this.GetValue(TextValue1Property) != null ? this.GetValue(TextValue1Property).ToString() : null;
            }
            set
            {
                this.SetValue(TextValue1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the text value 2.
        /// </summary>
        public string TextValue2
        {
            get
            {
                return this.GetValue(TextValue2Property) != null ? this.GetValue(TextValue2Property).ToString() : null;
            }
            set
            {
                this.SetValue(TextValue2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the local selected operator 1.
        /// </summary>
        public FilterOperator? LocalSelectedOperator1
        {
            get
            {
                return this._localSelectedOperator1;
            }
            set
            {
                this._localSelectedOperator1 = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LocalSelectedOperator1"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the local selected operator 2.
        /// </summary>
        public FilterOperator? LocalSelectedOperator2
        {
            get
            {
                return this._localSelectedOperator2;
            }
            set
            {
                this._localSelectedOperator2 = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LocalSelectedOperator2"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the local selected logical operator.
        /// </summary>
        public FilterCompositionLogicalOperator? LocalSelectedLogicalOperator
        {
            get
            {
                return this._localSelectedLogicalOperator;
            }
            set
            {
                this._localSelectedLogicalOperator = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LocalSelectedLogicalOperator"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the number input value 1.
        /// </summary>
        public string NumberInputValue1
        {
            get
            {
                return this._value1;
            }
            set
            {
                this._value1 = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("NumberInputValue1"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the number input value 2.
        /// </summary>
        public string NumberInputValue2
        {
            get
            {
                return this._value2;
            }
            set
            {
                this._value2 = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("NumberInputValue2"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the text input value 1.
        /// </summary>
        public string TextInputValue1
        {
            get
            {
                return this._textValue1;
            }
            set
            {
                this._textValue1 = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("TextInputValue1"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the text input value 2.
        /// </summary>
        public string TextInputValue2
        {
            get
            {
                return this._textValue2;
            }
            set
            {
                this._textValue2 = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("TextInputValue2"));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is number.
        /// </summary>
        public bool IsNumber
        {
            get
            {
                return this._IsNumber;
            }
            set
            {
                this._IsNumber = value;
                if (value)
                {
                    IsVisiblilityNumber = Visibility.Visible;
                    IsVisiblilityText = Visibility.Collapsed;
                }
                else
                {
                    IsVisiblilityNumber = Visibility.Collapsed;
                    IsVisiblilityText = Visibility.Visible;
                }

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("IsNumber"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the is visiblility number.
        /// </summary>
        public Visibility IsVisiblilityNumber
        {
            get
            {
                return this._isVisibilityNumber;
            }
            set
            {
                this._isVisibilityNumber = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("IsVisiblilityNumber"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the is visiblility text.
        /// </summary>
        public Visibility IsVisiblilityText
        {
            get
            {
                return this._isVisibilityText;
            }
            set
            {
                this._isVisibilityText = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("IsVisiblilityText"));
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

            // filter text
            this.LocalSelectedLogicalOperator = this.SelectedLogicalOperator;
            this.LocalSelectedOperator1 = this.SelectedOperator1;
            this.LocalSelectedOperator2 = this.SelectedOperator2;

            this.NumberInputValue1 = this.NumberValue1.HasValue ? this.NumberValue1.ToString() : null;
            this.NumberInputValue2 = this.NumberValue2.HasValue ? this.NumberValue2.ToString() : null;
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
                this.CreateFiltersForTextFieldFilter();
                this.CreateFiltersForSelectFilter();
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
            this.CreateFiltersForSelectFilter();
            this.CreateFiltersForTextFieldFilter();
            var popup = ((Button)sender).ParentOfType<System.Windows.Controls.Primitives.Popup>();
            popup.IsOpen = false;
        }

        /// <summary>
        /// The create filters.
        /// </summary>
        private void CreateFiltersForSelectFilter()
        {
            this.IsItemSelect = true;
            this._column.DataControl.FilterDescriptors.SuspendNotifications();
            if (this._filter1 == null)
            {
                this._filter1 = new CompositeFilterDescriptor();
                this._filter1.LogicalOperator = FilterCompositionLogicalOperator.Or;
                ((CompositeFilterDescriptor)this._column.DataControl.FilterDescriptors[1]).FilterDescriptors.Add(
                    this._filter1);
            }

            this._filter1.FilterDescriptors.Clear();

            List<FilteringDataItem> selectedItems =
                this.ItemsSource.Cast<FilteringDataItem>().Where(d => d.IsSelected).ToList();

            List<FilterDescriptor> filterDescriptors = new List<FilterDescriptor>();
            foreach (FilteringDataItem item in selectedItems)
            {
                if (item.IsSelected)
                {
                    if (this._column.DataMemberBinding.Path.Path == this._column.FilterMemberPath)
                    {
                        filterDescriptors.Add(
                            new FilterDescriptor(this._column.FilterMemberPath, FilterOperator.IsEqualTo, item.Text));
                    }
                    else
                    {
                        filterDescriptors.Add(
                            new FilterDescriptor(this._column.FilterMemberPath, FilterOperator.IsEqualTo, item.Id));
                    }
                }
            }
            this._filter1.FilterDescriptors.AddRange(filterDescriptors);

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
        /// The create filters 2.
        /// </summary>
        private void CreateFiltersForTextFieldFilter()
        {
            // Filter text
            this._column.DataControl.FilterDescriptors.SuspendNotifications();
            if (this._filter2 == null)
            {
                this._filter2 = new CompositeFilterDescriptor();
                this._filter2.LogicalOperator = FilterCompositionLogicalOperator.Or;
                ((CompositeFilterDescriptor)this._column.DataControl.FilterDescriptors[1]).FilterDescriptors.Add(this._filter2);
            }
            bool isValidValue = IsNumber ? (!string.IsNullOrEmpty(NumberInputValue1) && !string.IsNullOrEmpty(NumberInputValue2)) : 
                                           (!string.IsNullOrEmpty(TextInputValue1) && !string.IsNullOrEmpty(TextInputValue2));
            if (LocalSelectedLogicalOperator.HasValue && this.LocalSelectedOperator1.HasValue && this.LocalSelectedOperator2.HasValue
                && isValidValue)
            {
                this.SelectedLogicalOperator = LocalSelectedLogicalOperator;
                this._filter2.LogicalOperator = LocalSelectedLogicalOperator.Value;
            }

            this._filter2.FilterDescriptors.Clear();

            if (this.LocalSelectedOperator1.HasValue && !string.IsNullOrEmpty(NumberInputValue1))
            {
                this.SelectedOperator1 = this.LocalSelectedOperator1;
                if (IsNumber)
                {
                    this.NumberValue1 = int.Parse(this.NumberInputValue1);
                    this._filter2.FilterDescriptors.Add(new FilterDescriptor(this._column.FilterMemberPath, this.SelectedOperator1.Value, NumberValue1));
                }
               
            }

            if (this.LocalSelectedOperator1.HasValue && !string.IsNullOrEmpty(TextInputValue1))
            {
                this.SelectedOperator1 = this.LocalSelectedOperator1;
                this.TextValue1 = this.TextInputValue1;
                this._filter2.FilterDescriptors.Add(new FilterDescriptor(this._column.FilterMemberPath, this.SelectedOperator1.Value, TextValue1));
            }

            if (this.LocalSelectedOperator2.HasValue && !string.IsNullOrEmpty(NumberInputValue2))
            {
                this.SelectedOperator2 = this.LocalSelectedOperator2;
                this.NumberValue2 = int.Parse(NumberInputValue2);
                this._filter2.FilterDescriptors.Add(new FilterDescriptor(this._column.FilterMemberPath, this.SelectedOperator2.Value, NumberValue2));

            }

            if (this.LocalSelectedOperator2.HasValue && !string.IsNullOrEmpty(TextInputValue2))
            {
                this.SelectedOperator2 = this.LocalSelectedOperator2;
                this.TextValue2 = this.TextInputValue2;
                this._filter2.FilterDescriptors.Add(new FilterDescriptor(this._column.FilterMemberPath, this.SelectedOperator2.Value, TextValue2));
            }

            this._column.DataControl.FilterDescriptors.ResumeNotifications();

            this.IsActive = true;

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
            if (this._filter1 != null || this._filter2 != null)
            {
                this._column.DataControl.FilterDescriptors.SuspendNotifications();
                foreach (FilteringDataItem item in this.ItemsSource)
                {
                    item.PropertyChanged -= this.SelectFilterPropertyChanged;
                    item.IsSelected = true;
                }
                if (this._filter1 != null)
                {
                    this._filter1.FilterDescriptors.Clear();
                }

                 if (this._filter2 != null)
                {
                    this._filter2.FilterDescriptors.Clear();
                }

                SelectedLogicalOperator = null;
                SelectedOperator1 = null;
                SelectedOperator2 = null;
                NumberValue1 = null;
                NumberValue2 = null;
                TextValue1 = string.Empty;
                TextValue2 = string.Empty;
                TextInputValue1 = string.Empty;
                TextInputValue2 = string.Empty;
                this.IsActive = false;

                this._column.DataControl.FilterDescriptors.ResumeNotifications();
            }
        }

        /// <summary>
        /// The txt value_ preview text input.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void txtValue_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Decimal [^0-9.-]+. -- [^0-9]+
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        /// <summary>
        /// The text box_ preview executed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBox_PreviewExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((e.Command == ApplicationCommands.Cut) ||
                (e.Command == ApplicationCommands.Copy) ||
                (e.Command == ApplicationCommands.Paste))
            {
                e.Handled = true;
                e.CanExecute = false;
            }
        }
        #endregion
    }
}
