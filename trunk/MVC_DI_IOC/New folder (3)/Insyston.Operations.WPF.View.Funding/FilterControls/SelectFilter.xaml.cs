using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Insyston.Operations.Common;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;

namespace Insyston.Operations.WPF.Views.Funding.FilterControls
{
    /// <summary>
    /// Interaction logic for MultiSelectComboBoxFilter.xaml
    /// </summary>
    public partial class SelectFilter : UserControl, IFilteringControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(
                "IsActive",
                typeof(bool),
                typeof(SelectFilter),
                new System.Windows.PropertyMetadata(false));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(SelectFilter), null);

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register(
                "GroupName",
                typeof(string),
                typeof(SelectFilter), null);

        private CompositeFilterDescriptor _Filter;
        private GridViewBoundColumnBase _Column;
        private bool _IsMultiSelect;

        public SelectFilter()
        {
            this.InitializeComponent();
            this.GroupName = this.GetHashCode().ToString();
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

        public bool IsMultiSelect
        {
            get
            {
                return this._IsMultiSelect;
            }
            set
            {
                if (value != this._IsMultiSelect)
                {
                    this._IsMultiSelect = value;
                    
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("IsMultiSelect"));                        
                    }
                }
            }
        }

        public void Prepare(Telerik.Windows.Controls.GridViewColumn columnToPrepare)
        {
            this._Column = columnToPrepare as GridViewBoundColumnBase;

            if (this._Column == null)
            {
                return;
            }

            this.TitleTextBlock.Text = this.Title;
            if (this.ItemsSource == null)
            {
                return;
            }
            foreach (SelectListViewModel item in this.ItemsSource)
            {
                ((ObservableModel)item).PropertyChanged -= this.SelectFilter_PropertyChanged;
                ((ObservableModel)item).PropertyChanged += this.SelectFilter_PropertyChanged;
            }

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

        private void SelectFilter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyFinder.IsProperty(e.PropertyName, () => ((SelectListViewModel)sender).IsSelected))
            { 
                this.CreateFilters();
            }
        }

        private void OnFilter(object sender, RoutedEventArgs e)
        {
            var popup = ((Button)sender).ParentOfType<System.Windows.Controls.Primitives.Popup>();
            popup.IsOpen = false;
        }

        private void CreateFilters()
        {
            this._Column.DataControl.FilterDescriptors.SuspendNotifications();
            if (this._Filter == null)
            {
                this._Filter = new CompositeFilterDescriptor();
                this._Filter.LogicalOperator = FilterCompositionLogicalOperator.Or;
                ((CompositeFilterDescriptor)this._Column.DataControl.FilterDescriptors[1]).FilterDescriptors.Add(this._Filter);
            }

            this._Filter.FilterDescriptors.Clear();
            foreach (SelectListViewModel item in this.ItemsSource)
            {
                if (item.IsSelected)
                {
                    if (this._Column.DataMemberBinding.Path.Path == this._Column.FilterMemberPath)
                    {
                        this._Filter.FilterDescriptors.Add(new FilterDescriptor(this._Column.FilterMemberPath,
                            FilterOperator.IsEqualTo, item.Text));
                    }
                    else
                    {
                        this._Filter.FilterDescriptors.Add(new FilterDescriptor(this._Column.FilterMemberPath,
                            FilterOperator.IsEqualTo, item.Id));
                    }
                }
            }
            this._Column.DataControl.FilterDescriptors.ResumeNotifications();
            this.IsActive = true; 
        }

        private void OnClearFilter(object sender, RoutedEventArgs e)
        {
            this._Column.DataControl.FilterDescriptors.SuspendNotifications();
            foreach (SelectListViewModel item in this.ItemsSource)
            {
                ((ObservableModel)item).PropertyChanged -= this.SelectFilter_PropertyChanged;
                item.IsSelected = false;
            }
            if (this._Filter != null)
            {
                this._Filter.FilterDescriptors.Clear();
            }
            this.IsActive = false;
            var popup = ((Button)sender).ParentOfType<System.Windows.Controls.Primitives.Popup>();
            popup.IsOpen = false;
            this._Column.DataControl.FilterDescriptors.ResumeNotifications();          
        }
    }
}
