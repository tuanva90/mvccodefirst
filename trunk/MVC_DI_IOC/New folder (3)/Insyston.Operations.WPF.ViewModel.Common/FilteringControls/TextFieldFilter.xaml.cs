using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;

namespace Insyston.Operations.WPF.ViewModels.Common.FilteringControls
{
    /// <summary>
    /// Interaction logic for TextFieldFilter.xaml
    /// </summary>
    public partial class TextFieldFilter : UserControl, IFilteringControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private GridViewBoundColumnBase _Column;
        private CompositeFilterDescriptor _Filter;

        private FilterOperator? _LocalSelectedOperator1;
        private FilterOperator? _LocalSelectedOperator2;
        private FilterCompositionLogicalOperator? _LocalSelectedLogicalOperator;

        private string _Value1;
        private string _Value2;

        private string _TextValue1;
        private string _TextValue2;
        private bool _IsNumber { get; set; }

        private Visibility _IsVisibilityNumber;

        private Visibility _IsVisibilityText;

        #region Dependency Properties
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(
                "IsActive",
                typeof(bool),
                typeof(TextFieldFilter),
                new System.Windows.PropertyMetadata(false));

        public static readonly DependencyProperty AllOperators1Property =
            DependencyProperty.Register(
                "AllOperators1",
                typeof(IEnumerable),
                typeof(TextFieldFilter),
                new System.Windows.PropertyMetadata(null));

        public static readonly DependencyProperty AllOperators2Property =
            DependencyProperty.Register(
                "AllOperators2",
                typeof(IEnumerable),
                typeof(TextFieldFilter),
                new System.Windows.PropertyMetadata(null));

        public static readonly DependencyProperty AllLogicalOperatorProperty =
            DependencyProperty.Register(
                "AllLogicalOperator",
                typeof(IEnumerable),
                typeof(TextFieldFilter),
                new System.Windows.PropertyMetadata(null));

        public static readonly DependencyProperty SelectedOperator1Property =
            DependencyProperty.Register(
                "SelectedOperator1",
                typeof(FilterOperator?),
                typeof(TextFieldFilter), null);

        public static readonly DependencyProperty SelectedOperator2Property =
            DependencyProperty.Register(
                "SelectedOperator2",
                typeof(FilterOperator?),
                typeof(TextFieldFilter), null);
        public static readonly DependencyProperty SelectedLogicalOperatorProperty =
            DependencyProperty.Register(
                "SelectedLogicalOperator",
                typeof(FilterCompositionLogicalOperator?),
                typeof(TextFieldFilter), null);

        public static readonly DependencyProperty NumberValue1Property =
            DependencyProperty.Register(
                "NumberValue1",
                typeof(int?),
                typeof(TextFieldFilter), null);

        public static readonly DependencyProperty NumberValue2Property =
            DependencyProperty.Register(
                "NumberValue2",
                typeof(int?),
                typeof(TextFieldFilter), null);

        public static readonly DependencyProperty TextValue1Property =
            DependencyProperty.Register(
                "TextValue1",
                typeof(string),
                typeof(TextFieldFilter), null);

        public static readonly DependencyProperty TextValue2Property =
            DependencyProperty.Register(
                "TextValue2",
                typeof(string),
                typeof(TextFieldFilter), null);


        #endregion

        #region Public Properties
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

        public FilterOperator? LocalSelectedOperator1
        {
            get
            {
                return this._LocalSelectedOperator1;
            }
            set
            {
                this._LocalSelectedOperator1 = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LocalSelectedOperator1"));
                }
            }
        }
        public FilterOperator? LocalSelectedOperator2
        {
            get
            {
                return this._LocalSelectedOperator2;
            }
            set
            {
                this._LocalSelectedOperator2 = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LocalSelectedOperator2"));
                }
            }
        }
        public FilterCompositionLogicalOperator? LocalSelectedLogicalOperator
        {
            get
            {
                return this._LocalSelectedLogicalOperator;
            }
            set
            {
                this._LocalSelectedLogicalOperator = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LocalSelectedLogicalOperator"));
                }
            }
        }

        public string NumberInputValue1
        {
            get
            {
                return this._Value1;
            }
            set
            {
                this._Value1 = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("NumberInputValue1"));
                }
            }
        }
        public string NumberInputValue2
        {
            get
            {
                return this._Value2;
            }
            set
            {
                this._Value2 = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("NumberInputValue2"));
                }
            }
        }

        public string TextInputValue1
        {
            get
            {
                return this._TextValue1;
            }
            set
            {
                this._TextValue1 = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("TextInputValue1"));
                }
            }
        }
        public string TextInputValue2
        {
            get
            {
                return this._TextValue2;
            }
            set
            {
                this._TextValue2 = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("TextInputValue2"));
                }
            }
        }

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
                    IsVisiblilityNumber = System.Windows.Visibility.Visible;
                    IsVisiblilityText = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    IsVisiblilityNumber = System.Windows.Visibility.Collapsed;
                    IsVisiblilityText = System.Windows.Visibility.Visible;
                }

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("IsNumber"));
                }
            }
        }

        public Visibility IsVisiblilityNumber
        {
            get
            {
                return this._IsVisibilityNumber;
            }
            set
            {
                this._IsVisibilityNumber = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("IsVisiblilityNumber"));
                }
            }
        }

        public Visibility IsVisiblilityText
        {
            get
            {
                return this._IsVisibilityText;
            }
            set
            {
                this._IsVisibilityText = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("IsVisiblilityText"));
                }
            }
        }
        #endregion
        
        public TextFieldFilter()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public void Prepare(Telerik.Windows.Controls.GridViewColumn columnToPrepare)
        {
            this._Column = columnToPrepare as GridViewBoundColumnBase;
            
            this.LocalSelectedLogicalOperator = this.SelectedLogicalOperator;
            this.LocalSelectedOperator1 = this.SelectedOperator1;
            this.LocalSelectedOperator2 = this.SelectedOperator2;

            this.NumberInputValue1 = this.NumberValue1.HasValue ? this.NumberValue1.ToString() : null;
            this.NumberInputValue2 = this.NumberValue2.HasValue ? this.NumberValue2.ToString() : null;

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
        public void ClearFilterOnColumn()
        {
            ClearAllFilter();
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
                ((CompositeFilterDescriptor)this._Column.DataControl.FilterDescriptors[1]).FilterDescriptors.Add(this._Filter);
            }
            bool isValidValue = IsNumber ? (!string.IsNullOrEmpty(NumberInputValue1) && !string.IsNullOrEmpty(NumberInputValue2)) : 
                                           (!string.IsNullOrEmpty(TextInputValue1) && !string.IsNullOrEmpty(TextInputValue2));
            if (LocalSelectedLogicalOperator.HasValue && this.LocalSelectedOperator1.HasValue && this.LocalSelectedOperator2.HasValue
                && isValidValue)
            {
                this.SelectedLogicalOperator = LocalSelectedLogicalOperator;
                this._Filter.LogicalOperator = LocalSelectedLogicalOperator.Value;
            }

            this._Filter.FilterDescriptors.Clear();


            if (this.LocalSelectedOperator1.HasValue && !string.IsNullOrEmpty(NumberInputValue1))
            {
                this.SelectedOperator1 = this.LocalSelectedOperator1;
                if (IsNumber)
                {
                    this.NumberValue1 = int.Parse(this.NumberInputValue1);
                    this._Filter.FilterDescriptors.Add(new FilterDescriptor(this._Column.FilterMemberPath,
                        this.SelectedOperator1.Value, NumberValue1));
                }
               
            }

            if (this.LocalSelectedOperator1.HasValue && !string.IsNullOrEmpty(TextInputValue1))
            {
                this.SelectedOperator1 = this.LocalSelectedOperator1;
                this.TextValue1 = this.TextInputValue1;

                this._Filter.FilterDescriptors.Add(new FilterDescriptor(this._Column.FilterMemberPath,
                this.SelectedOperator1.Value, TextValue1));
            }

            if (this.LocalSelectedOperator2.HasValue && !string.IsNullOrEmpty(NumberInputValue2))
            {

                this.SelectedOperator2 = this.LocalSelectedOperator2;
                this.NumberValue2 = int.Parse(NumberInputValue2);
                this._Filter.FilterDescriptors.Add(new FilterDescriptor(this._Column.FilterMemberPath,
                    this.SelectedOperator2.Value, NumberValue2));

            }

            if (this.LocalSelectedOperator2.HasValue && !string.IsNullOrEmpty(TextInputValue2))
            {
                this.SelectedOperator2 = this.LocalSelectedOperator2;
                this.TextValue2 = this.TextInputValue2;
                this._Filter.FilterDescriptors.Add(new FilterDescriptor(this._Column.FilterMemberPath,
                    this.SelectedOperator2.Value, TextValue2));
            }

            this._Column.DataControl.FilterDescriptors.ResumeNotifications();

            this.IsActive = true;

        }
        private void OnClearFilter(object sender, RoutedEventArgs e)
        {
            ClearAllFilter();
            var popup = ((Button)sender).ParentOfType<System.Windows.Controls.Primitives.Popup>();
            popup.IsOpen = false;
        }
        private void txtValue_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);//  Decimal [^0-9.-]+. -- [^0-9]+
        }
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
        private void ClearAllFilter()
        {
            if (this._Filter != null)
            {
                this._Column.DataControl.FilterDescriptors.SuspendNotifications();
                if (this._Filter != null)
                {
                    this._Filter.FilterDescriptors.Clear();
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
                this._Column.DataControl.FilterDescriptors.ResumeNotifications();
            }
        }
    }
}
