using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;

namespace Insyston.Operations.WPF.Views.Funding.FilterControls
{
    /// <summary>
    /// Interaction logic for MultiSelectComboBoxFilter.xaml
    /// </summary>
    public partial class DateFilter : UserControl, IFilteringControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(
                "IsActive",
                typeof(bool),
                typeof(DateFilter),
                new System.Windows.PropertyMetadata(false));

        public static readonly DependencyProperty FromDateSelectedOperatorProperty =
            DependencyProperty.Register(
                "FromDateSelectedOperator",
                typeof(FilterOperator?),
                typeof(DateFilter), null);

        public static readonly DependencyProperty ToDateSelectedOperatorProperty =
            DependencyProperty.Register(
                "ToDateSelectedOperator",
                typeof(FilterOperator?),
                typeof(DateFilter), null);

        public static readonly DependencyProperty FromDateValueProperty =
            DependencyProperty.Register(
                "FromDateValue",
                typeof(DateTime?),
                typeof(DateFilter), null);

        public static readonly DependencyProperty ToDateValueProperty =
            DependencyProperty.Register(
                "ToDateValue",
                typeof(DateTime?),
                typeof(DateFilter), null);

        public static readonly DependencyProperty AllFromDateOperatorsProperty =
            DependencyProperty.Register(
                "AllFromDateOperators",
                typeof(IEnumerable),
                typeof(DateFilter),
                new System.Windows.PropertyMetadata(null));

        public static readonly DependencyProperty AllToDateOperatorsProperty =
            DependencyProperty.Register(
                "AllToDateOperators",
                typeof(IEnumerable),
                typeof(DateFilter),
                new System.Windows.PropertyMetadata(null));

        private GridViewBoundColumnBase _Column;
        private CompositeFilterDescriptor _Filter;

        private Visibility _ToDateVisibility;
        private FilterOperator? _LocalFromDateSelectedOperator;
        private FilterOperator? _LocalToDateSelectedOperator;
        private DateTime? _LocalFromDateValue;
        private DateTime? _LocalToDateValue;

        public DateFilter()
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

        public IEnumerable AllFromDateOperators
        {
            get
            {
                return (IEnumerable)this.GetValue(AllFromDateOperatorsProperty);
            }
            set
            {
                this.SetValue(AllFromDateOperatorsProperty, value);
            }
        }

        public IEnumerable AllToDateOperators
        {
            get
            {
                return (IEnumerable)this.GetValue(AllToDateOperatorsProperty);
            }
            set
            {
                this.SetValue(AllToDateOperatorsProperty, value);
            }
        }

        public Visibility ToDateVisibility
        {
            get
            {
                return this._ToDateVisibility;
            }
            set
            {
                if (value != this._ToDateVisibility)
                {
                    this._ToDateVisibility = value;
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("ToDateVisibility"));
                    }
                }
            }
        }

        public FilterOperator? FromDateSelectedOperator
        {
            get
            {
                return (FilterOperator?)this.GetValue(FromDateSelectedOperatorProperty);
            }
            set
            {
                this.SetValue(FromDateSelectedOperatorProperty, value);
            }
        }

        public FilterOperator? LocalFromDateSelectedOperator
        {
            get
            {
                return this._LocalFromDateSelectedOperator;
            }
            set
            {
                this._LocalFromDateSelectedOperator = value;

                if (value == FilterOperator.IsGreaterThanOrEqualTo || value == FilterOperator.IsGreaterThan)
                {
                    this.ToDateVisibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.ToDateVisibility = System.Windows.Visibility.Collapsed;
                }
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LocalFromDateSelectedOperator"));
                }
            }
        }

        public FilterOperator? ToDateSelectedOperator
        {
            get
            {
                return (FilterOperator?)this.GetValue(ToDateSelectedOperatorProperty);
            }
            set
            {
                this.SetValue(ToDateSelectedOperatorProperty, value);
            }
        }

        public FilterOperator? LocalToDateSelectedOperator
        {
            get
            {
                return this._LocalToDateSelectedOperator;
            }
            set
            {
                this._LocalToDateSelectedOperator = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LocalToDateSelectedOperator"));
                }
            }
        }

        public DateTime? FromDateValue
        {
            get
            {
                return (DateTime?)this.GetValue(FromDateValueProperty);
            }
            set
            {
                this.SetValue(FromDateValueProperty, value);
            }
        }

        public DateTime? LocalFromDateValue
        {
            get
            {
                return this._LocalFromDateValue;
            }
            set
            {
                this._LocalFromDateValue = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LocalFromDateValue"));
                }
            }
        }

        public DateTime? ToDateValue
        {
            get
            {
                return (DateTime?)this.GetValue(ToDateValueProperty);
            }
            set
            {
                this.SetValue(ToDateValueProperty, value);
            }
        }

        public DateTime? LocalToDateValue
        {
            get
            {
                return this._LocalToDateValue;
            }
            set
            {
                this._LocalToDateValue = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LocalToDateValue"));
                }
            }
        }

        public void Prepare(Telerik.Windows.Controls.GridViewColumn columnToPrepare)
        {
            this._Column = columnToPrepare as GridViewBoundColumnBase;
            if (this.FromDateSelectedOperator.HasValue &&
                (this.FromDateSelectedOperator.Value == FilterOperator.IsGreaterThan || this.FromDateSelectedOperator.Value == FilterOperator.IsGreaterThanOrEqualTo))
            {
                this.ToDateVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.ToDateVisibility = System.Windows.Visibility.Collapsed;
            }
            this.LocalFromDateSelectedOperator = this.FromDateSelectedOperator;
            this.LocalFromDateValue = this.FromDateValue;
            this.LocalToDateSelectedOperator = this.ToDateSelectedOperator;
            this.LocalToDateValue = this.ToDateValue;

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
            this.CreateFilters();
            var popup = ((Button)sender).ParentOfType<System.Windows.Controls.Primitives.Popup>();
            popup.IsOpen = false;
        }

        private void CreateFilters()
        {
            this._Column.DataControl.FilterDescriptors.SuspendNotifications();
            if (this._Filter == null)
            {
                this._Filter = new CompositeFilterDescriptor();
                this._Filter.LogicalOperator = FilterCompositionLogicalOperator.And;
                ((CompositeFilterDescriptor)this._Column.DataControl.FilterDescriptors[1]).FilterDescriptors.Add(this._Filter);
            }
            this._Filter.FilterDescriptors.Clear();

            if (this.LocalFromDateSelectedOperator.HasValue && this.LocalFromDateValue.HasValue)
            {
                this.FromDateSelectedOperator = this.LocalFromDateSelectedOperator;
                this.FromDateValue = this.LocalFromDateValue;
                this.ToDateSelectedOperator = this.LocalToDateSelectedOperator;
                this.ToDateValue = this.LocalToDateValue;

                this._Filter.FilterDescriptors.Add(new FilterDescriptor(this._Column.FilterMemberPath,
                    this.FromDateSelectedOperator.Value, this.FromDateValue.Value));
      
                if (this.ToDateSelectedOperator.HasValue && this.ToDateValue.HasValue)
                {
                    this._Filter.FilterDescriptors.Add(new FilterDescriptor(this._Column.FilterMemberPath,
                        this.ToDateSelectedOperator.Value, this.ToDateValue.Value));
                }
            }
            this._Column.DataControl.FilterDescriptors.ResumeNotifications();

            this.IsActive = true;
            //if (LocalFromDateSelectedOperator.HasValue && LocalFromDateValue.HasValue)
            //{
            //    FromDateSelectedOperator = LocalFromDateSelectedOperator;
            //    FromDateValue = LocalFromDateValue;
            //    ToDateSelectedOperator = LocalToDateSelectedOperator;
            //    ToDateValue = LocalToDateValue;
            //    this._Column.ColumnFilterDescriptor.SuspendNotifications();
            //    this._Column.ColumnFilterDescriptor.FieldFilter.Filter1.Operator = FromDateSelectedOperator.Value;
            //    this._Column.ColumnFilterDescriptor.FieldFilter.Filter1.Value = FromDateValue.Value;
            //    if (ToDateSelectedOperator.HasValue && ToDateValue.HasValue)
            //    {
            //        this._Column.ColumnFilterDescriptor.FieldFilter.Filter2.Operator = ToDateSelectedOperator.Value;
            //        this._Column.ColumnFilterDescriptor.FieldFilter.Filter2.Value = ToDateValue.Value;
            //    }
            //    this._Column.ColumnFilterDescriptor.ResumeNotifications();
            //}
            //this.IsActive = true;
        }

        private void OnClearFilter(object sender, RoutedEventArgs e)
        {
            this._Column.DataControl.FilterDescriptors.SuspendNotifications();
            if (this._Filter != null)
            {
                this._Filter.FilterDescriptors.Clear();
            }
            this.FromDateSelectedOperator = null;
            this.FromDateValue = null;
            this.ToDateSelectedOperator = null;
            this.ToDateValue = null;

            this.IsActive = false;
            var popup = ((Button)sender).ParentOfType<System.Windows.Controls.Primitives.Popup>();
            popup.IsOpen = false;
            this._Column.DataControl.FilterDescriptors.ResumeNotifications();
            //this._Column.ColumnFilterDescriptor.FieldFilter.Clear();
            //FromDateSelectedOperator = null;            
            //FromDateValue = null;
            //ToDateSelectedOperator = null;
            //ToDateValue = null;
            //this.IsActive = false;
            //var popup = ((Button)sender).ParentOfType<System.Windows.Controls.Primitives.Popup>();
            //popup.IsOpen = false;
        }
    }
}