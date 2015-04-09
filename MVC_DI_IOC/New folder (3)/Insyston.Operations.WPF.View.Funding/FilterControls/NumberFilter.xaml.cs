using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Insyston.Operations.Common;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;

namespace Insyston.Operations.WPF.Views.Funding.FilterControls
{
    /// <summary>
    /// Interaction logic for MultiSelectComboBoxFilter.xaml
    /// </summary>
    public partial class NumberFilter : UserControl, IFilteringControl, IDataErrorInfo, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(
                "IsActive",
                typeof(bool),
                typeof(NumberFilter),
                new System.Windows.PropertyMetadata(false));

        public static readonly DependencyProperty SelectedOperatorProperty =
            DependencyProperty.Register(
                "SelectedOperator",
                typeof(FilterOperator?),
                typeof(NumberFilter), null);

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(int),
                typeof(NumberFilter), null);

        public static readonly DependencyProperty AllOperatorsProperty =
            DependencyProperty.Register(
                "AllOperators",
                typeof(IEnumerable),
                typeof(NumberFilter),
                new System.Windows.PropertyMetadata(null));

        private GridViewBoundColumnBase _Column;

        private CompositeFilterDescriptor _Filter;

        private FilterOperator? _LocalSelectedOperator;
        private int _LocalValue;

        public NumberFilter()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Title { get; set; }
        
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

        public IEnumerable AllOperators
        {
            get
            {
                return (IEnumerable)this.GetValue(AllOperatorsProperty);
            }
            set
            {
                this.SetValue(AllOperatorsProperty, value);
            }
        }

        public FilterOperator? LocalSelectedOperator
        {
            get
            {
                return this._LocalSelectedOperator;
            }
            set
            {
                this._LocalSelectedOperator = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LocalSelectedOperator"));
                }
            }
        }

        public FilterOperator? SelectedOperator
        {
            get
            {
                return (FilterOperator?)this.GetValue(SelectedOperatorProperty);
            }
            set
            {
                this.SetValue(SelectedOperatorProperty, value);
            }
        }

        public int Value
        {
            get
            {
                return (int)this.GetValue(ValueProperty);
            }
            set
            {
                this.SetValue(ValueProperty, value);
            }
        }

        public int LocalValue
        {
            get
            {
                return this._LocalValue;
            }
            set
            {
                this._LocalValue = value; 
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LocalValue"));
                }
            }
        }

        public string Error
        {
            get
            {
                return this[string.Empty];
            }
        }
        
        public string this[string columnName]
        {
            get 
            {
                StringBuilder result = new StringBuilder();
                if (string.IsNullOrEmpty(columnName) || PropertyFinder.IsProperty(columnName, () => this.LocalValue))
                {
                    if (this.Value != null && !string.IsNullOrEmpty(this.Value.ToString()))
                    {
                        int number;
                        bool isNumber = int.TryParse(this.Value.ToString(), out number);
                        if (!isNumber)
                        {
                            result.AppendLine("The value should be a number!");
                        }
                    }
                }
                return result.ToString();
            }
        }
    
        public void Prepare(Telerik.Windows.Controls.GridViewColumn columnToPrepare)
        {
            this._Column = columnToPrepare as GridViewBoundColumnBase;
            this.LocalValue = this.Value;
            this.LocalSelectedOperator = this.SelectedOperator;

            foreach (CompositeFilterDescriptor item in ((CompositeFilterDescriptor)this._Column.DataControl.FilterDescriptors[1]).FilterDescriptors)
            {
                if (item.FilterDescriptors.Count > 0)
                {
                    if (((FilterDescriptor)item.FilterDescriptors[0]).Member == this._Column.FilterMemberPath)
                    {
                        this._Filter = item;
                        break;
                    }
                }
            }
        }
        
        private void OnFilter(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Error))
            {
                this.Value = this.LocalValue;
                this.SelectedOperator = this.LocalSelectedOperator;
                this.CreateFilters();
                var popup = ((Button)sender).ParentOfType<System.Windows.Controls.Primitives.Popup>();
                popup.IsOpen = false;
            }
        }

        private void CreateFilters()
        {
            if (this.SelectedOperator.HasValue)
            {
                this._Column.DataControl.FilterDescriptors.SuspendNotifications();
                if (this._Filter == null)
                {
                    this._Filter = new CompositeFilterDescriptor();
                    this._Filter.LogicalOperator = FilterCompositionLogicalOperator.And;
                    ((CompositeFilterDescriptor)this._Column.DataControl.FilterDescriptors[1]).FilterDescriptors.Add(this._Filter);
                }
                this._Filter.FilterDescriptors.Clear();
                this._Filter.FilterDescriptors.Add(new FilterDescriptor(this._Column.FilterMemberPath,
                    this.SelectedOperator.Value, this.Value));

                this._Column.DataControl.FilterDescriptors.ResumeNotifications();
                //this._Column.ColumnFilterDescriptor.SuspendNotifications();
                //this._Column.ColumnFilterDescriptor.FieldFilter.Filter1.Operator = SelectedOperator.Value;
                //this._Column.ColumnFilterDescriptor.FieldFilter.Filter1.Value = Value;
                //this._Column.ColumnFilterDescriptor.ResumeNotifications();
            }
            this.IsActive = true; 
        }

        private void OnClearFilter(object sender, RoutedEventArgs e)
        {
            this._Column.DataControl.FilterDescriptors.SuspendNotifications();
            if (this._Filter != null)
            {
                this._Filter.FilterDescriptors.Clear();
            }
            this.Value = -1;
            this.SelectedOperator = null;
            this.IsActive = false; 
            var popup = ((Button)sender).ParentOfType<System.Windows.Controls.Primitives.Popup>();
            popup.IsOpen = false;
            this._Column.DataControl.FilterDescriptors.ResumeNotifications();
        }
    }
}
