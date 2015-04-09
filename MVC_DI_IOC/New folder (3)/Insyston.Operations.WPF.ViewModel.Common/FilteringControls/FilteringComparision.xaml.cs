// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilteringComparision.xaml.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for FilteringComparision.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.FilteringControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.GridView;
    using Telerik.Windows.Data;

    /// <summary>
    /// Interaction logic for FilteringComparision.xaml
    /// </summary>
    public partial class FilteringComparision : UserControl, IFilteringControl, INotifyPropertyChanged
    {
        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The _ column.
        /// </summary>
        private GridViewBoundColumnBase _column;

        /// <summary>
        /// The _ filter.
        /// </summary>
        private CompositeFilterDescriptor _filter;

        /// <summary>
        /// The _ local selected operator 1.
        /// </summary>
        private string _localSelectedOperator1;

        /// <summary>
        /// The _ local selected operator 2.
        /// </summary>
        private string _localSelectedOperator2;

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
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "IsActive",
            typeof(bool),
            typeof(FilteringComparision),
            new PropertyMetadata(false));

        /// <summary>
        /// The all operators 1 property.
        /// </summary>
        public static readonly DependencyProperty AllOperators1Property = DependencyProperty.Register(
            "AllOperators1",
            typeof(IEnumerable),
            typeof(FilteringComparision),
            new PropertyMetadata(null));

        /// <summary>
        /// The all operators 2 property.
        /// </summary>
        public static readonly DependencyProperty AllOperators2Property = DependencyProperty.Register(
            "AllOperators2",
            typeof(IEnumerable),
            typeof(FilteringComparision),
            new PropertyMetadata(null));

        /// <summary>
        /// The all logical operator property.
        /// </summary>
        public static readonly DependencyProperty AllLogicalOperatorProperty =
            DependencyProperty.Register(
                "AllLogicalOperator",
                typeof(IEnumerable),
                typeof(FilteringComparision),
                new PropertyMetadata(null));

        /// <summary>
        /// The selected operator 1 property.
        /// </summary>
        public static readonly DependencyProperty SelectedOperator1Property =
            DependencyProperty.Register("SelectedOperator1", typeof(FilterOperator?), typeof(FilteringComparision), null);

        /// <summary>
        /// The selected operator 2 property.
        /// </summary>
        public static readonly DependencyProperty SelectedOperator2Property =
            DependencyProperty.Register("SelectedOperator2", typeof(FilterOperator?), typeof(FilteringComparision), null);

        /// <summary>
        /// The selected logical operator property.
        /// </summary>
        public static readonly DependencyProperty SelectedLogicalOperatorProperty =
            DependencyProperty.Register(
                "SelectedLogicalOperator",
                typeof(FilterCompositionLogicalOperator?),
                typeof(FilteringComparision),
                null);

        /// <summary>
        /// The number value 1 property.
        /// </summary>
        public static readonly DependencyProperty NumberValue1Property = DependencyProperty.Register(
            "NumberValue1",
            typeof(int?),
            typeof(FilteringComparision),
            null);

        /// <summary>
        /// The number value 2 property.
        /// </summary>
        public static readonly DependencyProperty NumberValue2Property = DependencyProperty.Register(
            "NumberValue2",
            typeof(int?),
            typeof(FilteringComparision),
            null);

        /// <summary>
        /// The text value 1 property.
        /// </summary>
        public static readonly DependencyProperty TextValue1Property = DependencyProperty.Register(
            "TextValue1",
            typeof(string),
            typeof(FilteringComparision),
            null);

        /// <summary>
        /// The text value 2 property.
        /// </summary>
        public static readonly DependencyProperty TextValue2Property = DependencyProperty.Register(
            "TextValue2",
            typeof(string),
            typeof(FilteringComparision),
            null);


        #endregion

        #region Public Properties

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
        public string LocalSelectedOperator1
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
        public string LocalSelectedOperator2
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

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringComparision"/> class.
        /// </summary>
        public FilteringComparision()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        /// <summary>
        /// The prepare.
        /// </summary>
        /// <param name="columnToPrepare">
        /// The column to prepare.
        /// </param>
        public void Prepare(Telerik.Windows.Controls.GridViewColumn columnToPrepare)
        {
            this._column = columnToPrepare as GridViewBoundColumnBase;

            this.LocalSelectedLogicalOperator = this.SelectedLogicalOperator;
            if (this.SelectedOperator1.HasValue)
            {
                this.LocalSelectedOperator1 = this.SelectedOperator1.ToString();
            }

            this.LocalSelectedOperator2 = this.SelectedOperator2.ToString();

            this.NumberInputValue1 = this.NumberValue1.HasValue ? this.NumberValue1.ToString() : null;
            this.NumberInputValue2 = this.NumberValue2.HasValue ? this.NumberValue2.ToString() : null;

            foreach ( CompositeFilterDescriptor item in ((CompositeFilterDescriptor)this._column.DataControl.FilterDescriptors[1]).FilterDescriptors)
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
            this.CreateFilters();
            var popup = ((Button)sender).ParentOfType<System.Windows.Controls.Primitives.Popup>();
            popup.IsOpen = false;
        }

        /// <summary>
        /// The create filters.
        /// </summary>
        private void CreateFilters()
        {
            this._column.DataControl.FilterDescriptors.SuspendNotifications();
            if (this._filter == null)
            {
                this._filter = new CompositeFilterDescriptor();
                ((CompositeFilterDescriptor)this._column.DataControl.FilterDescriptors[1]).FilterDescriptors.Add(
                    this._filter);
            }
            bool isValidValue = IsNumber
                                    ? (!string.IsNullOrEmpty(NumberInputValue1)
                                       && !string.IsNullOrEmpty(NumberInputValue2))
                                    : (!string.IsNullOrEmpty(TextInputValue1) && !string.IsNullOrEmpty(TextInputValue2));
            if (LocalSelectedLogicalOperator.HasValue && !string.IsNullOrEmpty(this.LocalSelectedOperator1)
                && !string.IsNullOrEmpty(this.LocalSelectedOperator2) && isValidValue)
            {
                this.SelectedLogicalOperator = LocalSelectedLogicalOperator;
                this._filter.LogicalOperator = LocalSelectedLogicalOperator.Value;
            }

            this._filter.FilterDescriptors.Clear();


            if (!string.IsNullOrEmpty(this.LocalSelectedOperator1) && !string.IsNullOrEmpty(NumberInputValue1))
            {
                this.SelectedOperator1 = GetOperatorFromString(this.LocalSelectedOperator1);
                if (IsNumber)
                {
                    this.NumberValue1 = int.Parse(this.NumberInputValue1);
                    this._filter.FilterDescriptors.Add(
                        new FilterDescriptor(this._column.FilterMemberPath, this.SelectedOperator1.Value, NumberValue1));
                }

            }

            if (!string.IsNullOrEmpty(this.LocalSelectedOperator1) && !string.IsNullOrEmpty(TextInputValue1))
            {
                this.SelectedOperator1 = GetOperatorFromString(this.LocalSelectedOperator1);
                if (this.SelectedOperator1.HasValue)
                {
                    this.TextValue1 = this.TextInputValue1;

                    this._filter.FilterDescriptors.Add(
                        new FilterDescriptor(this._column.FilterMemberPath, this.SelectedOperator1.Value, TextValue1));
                }
            }

            if (!string.IsNullOrEmpty(this.LocalSelectedOperator2) && !string.IsNullOrEmpty(NumberInputValue2))
            {
                this.SelectedOperator2 = GetOperatorFromString(this.LocalSelectedOperator2);
                if (this.SelectedOperator2.HasValue)
                {
                    this.NumberValue2 = int.Parse(NumberInputValue2);
                    this._filter.FilterDescriptors.Add(
                        new FilterDescriptor(this._column.FilterMemberPath, this.SelectedOperator2.Value, NumberValue2));
                }

            }

            if (!string.IsNullOrEmpty(this.LocalSelectedOperator2) && !string.IsNullOrEmpty(TextInputValue2))
            {
                this.SelectedOperator2 = GetOperatorFromString(this.LocalSelectedOperator2);
                if (this.SelectedOperator2.HasValue)
                {
                    this.TextValue2 = this.TextInputValue2;
                    this._filter.FilterDescriptors.Add(
                        new FilterDescriptor(this._column.FilterMemberPath, this.SelectedOperator2.Value, TextValue2));
                }
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
            var popup = ((Button)sender).ParentOfType<System.Windows.Controls.Primitives.Popup>();
            popup.IsOpen = false;
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
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text); //  Decimal [^0-9.-]+. -- [^0-9]+
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
            if ((e.Command == ApplicationCommands.Cut) || (e.Command == ApplicationCommands.Copy)
                || (e.Command == ApplicationCommands.Paste))
            {
                e.Handled = true;
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// The clear all filter.
        /// </summary>
        private void ClearAllFilter()
        {
            if (this._filter != null)
            {
                this._column.DataControl.FilterDescriptors.SuspendNotifications();
                if (this._filter != null)
                {
                    this._filter.FilterDescriptors.Clear();
                }
                SelectedLogicalOperator = null;
                SelectedOperator1 = null;
                SelectedOperator2 = null;
                this.LocalSelectedOperator1 = null;
                this.LocalSelectedOperator2 = null;

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
        /// The get operator from string.
        /// </summary>
        /// <param name="operatorName">
        /// The operator name.
        /// </param>
        /// <returns>
        /// The FilterOperator.
        /// </returns>
        private FilterOperator? GetOperatorFromString(string operatorName)
        {
            FilterOperator value;
            if (Enum.TryParse(operatorName, out value))
            {
                return value;
            }
            return null;
        }
    }
}
