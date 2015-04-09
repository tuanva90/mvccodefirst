// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicGridViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The dynamic grid view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WPFDynamic.ViewModels.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net.Mime;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Caliburn.Micro;

    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Enums;
    using Insyston.Operations.WPF.ViewModels.Common.FilteringControls;
    using Insyston.Operations.WPF.ViewModels.Common.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;
    using Insyston.Operations.WPF.ViewModels.Common.Model.Filtering;

    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.GridView;

    using WPF.DataTable.Models;

    using GridViewColumnCollection = Telerik.Windows.Controls.GridViewColumnCollection;

    /// <summary>
    /// The rebind data event handler.
    /// </summary>
    public delegate void RebindDataEventHandler();

    /// <summary>
    /// The dynamic grid view model.
    /// </summary>
    public class DynamicGridViewModel : INotifyPropertyChanged
    {
        
        /// <summary>
        /// Gets or sets the original width.
        /// </summary>
        public int OriginalWidth { get; set; }

        /// <summary>
        /// The grouped item changed.
        /// </summary>
        public Action<object, object> GroupedItemChanged;

        /// <summary>
        /// The list group by items.
        /// </summary>
        private List<ItemGroupPanel> ListGroupByItems;

        /// <summary>
        /// The is size change.
        /// </summary>
        private bool IsSizeChange = false;

        /// <summary>
        /// Set value for column except this list properties name because this is special properties.
        /// </summary>
        public static readonly List<string> ListPropertiesName = new List<string> { "ErrorsChanged", "HasErrors", "IsValid" };

        /// <summary>
        /// The selected item changed.
        /// </summary>
        public Action<object> SelectedItemChanged;

        /// <summary>
        /// The updated item changed.
        /// </summary>
        public event EventHandler<EventArgs> UpdatedItemChanged;

        /// <summary>
        /// The deleted item changed.
        /// </summary>
        public event EventHandler<EventArgs> DeletedItemChanged;

        /// <summary>
        /// The canceled item changed.
        /// </summary>
        public event EventHandler<EventArgs> CanceledItemChanged;

        /// <summary>
        /// The row detail loading.
        /// </summary>
        public event EventHandler<EventArgs> RowDetailLoading;

        /// <summary>
        /// The selected radio change.
        /// </summary>
        public event EventHandler<EventArgs> SelectedRadioChange;

        /// <summary>
        /// The re bind data.
        /// </summary>
        public event RebindDataEventHandler ReBindData;

        /// <summary>
        /// The validate row.
        /// </summary>
        public Func<object, bool> ValidateRow;

        /// <summary>
        /// The added new item.
        /// </summary>
        public event EventHandler<EventArgs> AddedNewItem;

        /// <summary>
        /// Gets or sets a value indicating whether is set selected item.
        /// </summary>
        public bool IsSetSelectedItem
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether has value.
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// Gets or sets the t base class.
        /// </summary>
        public Type TBaseClass { get; set; }

        /// <summary>
        /// The _members table.
        /// </summary>
        private DataTable _membersTable;

        /// <summary>
        /// The _feature width.
        /// </summary>
        private int _maxWidthGrid;

        /// <summary>
        /// Gets or sets the max width grid.
        /// </summary>
        public int MaxWidthGrid
        {
            get
            {
                return this._maxWidthGrid;
            }
            set
            {
                _maxWidthGrid = value;
                OnPropertyChanged("MaxWidthGrid");
            }
        }

        /// <summary>
        /// Gets or sets the members table.
        /// </summary>
        public DataTable MembersTable
        {
            get
            {
                return _membersTable;
            }
            set
            {
                _membersTable = value;
                OnPropertyChanged("MembersTable");
            }
        }

        /// <summary>
        /// The _selected product.
        /// </summary>
        private object _selectedItem;

        /// <summary>
        /// The _toolbar command grid view model.
        /// </summary>
        private ToolbarCommandGridViewModel _toolbarCommandGridViewModel;

        /// <summary>
        /// Gets or sets the toolbar command grid view model.
        /// </summary>
        public ToolbarCommandGridViewModel ToolbarCommandGridViewModel
        {
            get
            {
                return _toolbarCommandGridViewModel;
            }
            set
            {
                _toolbarCommandGridViewModel = value;
                OnPropertyChanged("ToolbarCommandGridViewModel");
            }
        }

        /// <summary>
        /// The _toolbar visibility changed.
        /// </summary>
        private Visibility _toolbarVisibilityChanged;

        /// <summary>
        /// Gets or sets the toolbar visibility changed.
        /// </summary>
        public Visibility ToolbarVisibilityChanged
        {
            get
            {
                return _toolbarVisibilityChanged;
            }
            set
            {
                _toolbarVisibilityChanged = value;
                OnPropertyChanged("ToolbarVisibilityChanged");
            }
        }

        /// <summary>
        /// Gets or sets the selected product.
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
                if (SelectedItemChanged != null && !IsSetSelectedItem)
                {
                    SelectedItemChanged(SelectedItem);
                }
                else
                {
                    IsSetSelectedItem = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the grid columns.
        /// </summary>
        public List<DynamicColumn> GridColumns { get; set; }

        /// <summary>
        /// Gets or sets the grid data rows.
        /// </summary>
        public List<object> GridDataRows { get; set; }

        /// <summary>
        /// The _columns.
        /// </summary>
        private GridViewColumnCollection _columns;

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        public GridViewColumnCollection Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                _columns = value;
                OnPropertyChanged("Columns");
            }
        }

        /// <summary>
        /// The _is enable hover.
        /// </summary>
        private bool _isEnableHover;

        /// <summary>
        /// Gets or sets a value indicating whether is enable hover row.
        /// </summary>
        public bool IsEnableHoverRow
        {
            get
            {
                return this._isEnableHover;
            }
            set
            {
                this._isEnableHover = value;
                OnPropertyChanged("IsEnableHoverRow");
            }

        }

        /// <summary>
        /// The _is enable radio.
        /// </summary>
        private bool _isEnableRadio;

        /// <summary>
        /// Gets or sets a value indicating whether is enable radio button row.
        /// </summary>
        public bool IsEnableRadioButtonRow
        {
            get
            {
                return this._isEnableRadio;
            }
            set
            {
                this._isEnableRadio = value;
                OnPropertyChanged("IsEnableRadioButtonRow");
            }

        }

        /// <summary>
        /// The _is select all row.
        /// </summary>
        private bool? _isSelectAllRow;

        /// <summary>
        /// Gets or sets a value indicating whether is select all row.
        /// </summary>
        public bool? IsSelectAllRow
        {
            get
            {
                return this._isSelectAllRow;
            }
            set
            {
                this._isSelectAllRow = value;
                OnPropertyChanged("IsSelectAllRow");
                if (value.HasValue && value.Value)
                {
                    SelectedRows = MembersTable.Rows.Select(d => d.RowObject).ToList();
                }
                else
                {
                    SelectedRows = new List<object>();
                }
            }

        }

        /// <summary>
        /// The _selected items.
        /// </summary>
        private IEnumerable<object> _selectedItems;

        /// <summary>
        /// Gets or sets the selected items.
        /// </summary>
        public IEnumerable<object> SelectedItems
        {
            get
            {
                return this._selectedItems;
            }
            set
            {
                this._selectedItems = value;

                // OnPropertyChanged("SelectedItems");
            }
        }

        /// <summary>
        /// The _selected radio items.
        /// </summary>
        private object _selectedRadioItems;

        /// <summary>
        /// Gets or sets the selected radio items.
        /// </summary>
        public object SelectedRadioItems
        {
            get
            {
                return this._selectedRadioItems;
            }
            set
            {
                this._selectedRadioItems = value;

                // OnPropertyChanged("SelectedItems");
            }
        }
        
        /// <summary>
        /// The _is check item changed.
        /// </summary>
        private bool _isCheckItemChanged;

        /// <summary>
        /// Gets or sets a value indicating whether is check item changed.
        /// </summary>
        public bool IsCheckItemChanged
        {
            get
            {
                return this._isCheckItemChanged;
            }
            set
            {
                this._isCheckItemChanged = value;
                OnPropertyChanged("IsCheckItemChanged");
            }

        }

        /// <summary>
        /// The selected rows.
        /// </summary>
        private IEnumerable<object> _selectedRows;

        /// <summary>
        /// Gets or sets the selected rows.
        /// </summary>
        public IEnumerable<object> SelectedRows
        {
            get
            {
                return this._selectedRows;
            }
            set
            {
                this._selectedRows = value;
                OnPropertyChanged("SelectedRows");
            }
        }

        /// <summary>
        /// The _selected radio rows.
        /// </summary>
        private object _selectedRadioRows;

        /// <summary>
        /// Gets or sets the selected radio rows.
        /// </summary>
        public object SelectedRadioRows
        {
            get
            {
                return this._selectedRadioRows;
            }
            set
            {
                this._selectedRadioRows = value;
                OnPropertyChanged("SelectedRadioRows");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filtering generate.
        /// </summary>
        public bool FilteringGenerate { get; set; }

        /// <summary>
        /// The _is show group panel.
        /// </summary>
        private bool _isShowGroupPanel;

        /// <summary>
        /// Gets or sets a value indicating whether is show group panel.
        /// </summary>
        public bool IsShowGroupPanel
        {
            get
            {
                return this._isShowGroupPanel;
            }
            set
            {
                this._isShowGroupPanel = value;
                OnPropertyChanged("IsShowGroupPanel");
            }

        }

        /// <summary>
        /// The _is show group panel.
        /// </summary>
        private bool _isGridReadOnly;

        /// <summary>
        /// Gets or sets a value indicating whether is show group panel.
        /// </summary>
        public bool IsGridReadOnly
        {
            get
            {
                return this._isGridReadOnly;
            }
            set
            {
                this._isGridReadOnly = value;
                OnPropertyChanged("IsGridReadOnly");
            }

        }

        /// <summary>
        /// Gets or sets the row detail template key.
        /// </summary>
        public string RowDetailTemplateKey { get; set; }

        /// <summary>
        /// Gets or sets the original editing items.
        /// </summary>
        public Dictionary<string, DataRow> OriginalEditingItems { get; set; } 

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicGridViewModel"/> class.
        /// </summary>
        /// <param name="dataType">
        /// The data type.
        /// </param>
        public DynamicGridViewModel(Type dataType)
        {
            TBaseClass = dataType;
            OriginalEditingItems = new Dictionary<string, DataRow>();
            this.ListGroupByItems = new List<ItemGroupPanel>();
            this.IsGridReadOnly = true;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// The load rad grid view.
        /// </summary>
        public void LoadRadGridView()
        {
            CreateMemberTableColumns();
            GetDataForGridView();
            SetSelectedRowsByProperty();
            SetSelectedRadioRowsByProperty();
        }

        /// <summary>
        /// The get data for grid view.
        /// </summary>
        public void GetDataForGridView()
        {
            if (GridDataRows != null && GridDataRows.Count > 0)
            {
                MembersTable.Rows.Clear();
                
                foreach (object drow in GridDataRows)
                {
                    var row = MembersTable.NewRow();
                    for (int i = 0; i < drow.GetType().GetProperties().Count(); i++)
                    {
                        PropertyInfo pinfo = drow.GetType().GetProperties()[i];
                        if (!ListPropertiesName.Contains(pinfo.Name))
                        {
                            row[pinfo.Name] = pinfo.GetValue(drow, null);
                        }
                    }
                    MembersTable.Rows.Add(row);
                }
                GridDataRows.Clear();
            }
        }

        /// <summary>
        /// The insert row.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public void InsertRow(int index, object data)
        {
            if (index >= 0 && data != null)
            {
                var row = MembersTable.NewRow();
                for (int i = 0; i < data.GetType().GetProperties().Count(); i++)
                {
                    PropertyInfo pinfo = data.GetType().GetProperties()[i];
                    if (!ListPropertiesName.Contains(pinfo.Name))
                    {
                        row[pinfo.Name] = pinfo.GetValue(data, null);
                    }
                }
                MembersTable.Rows.Insert(index, row);
            }

            // Raise event for exist new item on grid 
            if (this.AddedNewItem != null)
            {
                this.AddedNewItem(data, null);
            }

            // rebind data
            if (this.ReBindData != null)
            {
                this.ReBindData();
            }
        }

        /// <summary>
        /// The add row.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void AddRow(object data)
        {
            if (data != null)
            {
                var row = MembersTable.NewRow();
                for (int i = 0; i < data.GetType().GetProperties().Count(); i++)
                {
                    PropertyInfo pinfo = data.GetType().GetProperties()[i];
                    if (!ListPropertiesName.Contains(pinfo.Name))
                    {
                        row[pinfo.Name] = pinfo.GetValue(data, null);
                    }
                }
                MembersTable.Rows.Add(row);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The create member table columns.
        /// </summary>
        private void CreateMemberTableColumns()
        {
            if (MembersTable == null)
            {
                _membersTable = new DataTable(Guid.NewGuid().ToString());
                _membersTable.TBaseClass = TBaseClass;
            }
            else
            {
                // Remove all data of  Grid, we did not use MembersTable.Rows.Clear() because sometimes data of Grid change but design of grid not change.
                int rowCount = MembersTable.Rows.Count;
                int index = 0;
                while (index <= rowCount - 1)
                {
                    MembersTable.Rows.RemoveAt(0);
                    index++;
                }

            }
            _membersTable.Columns.Clear();

            foreach (var systemColum in GridColumns)
            {
                DataColumn column = new DataColumn
                {
                    ColumnName = systemColum.ColumnName,
                    Header = systemColum.Header,
                    DataType = typeof(int),
                };
                column.IsReadOnly = true;
                _membersTable.Columns.Add(column);
            }

            MembersTable = _membersTable;
            Columns = GeneratedColumnsForGrid();

            foreach (var column in Columns)
            {
                var txt = new TextBlock();
                txt.Text = column.Header.ToString();
                txt.TextAlignment = column.HeaderTextAlignment;
                txt.TextWrapping = TextWrapping.Wrap;
                txt.FontSize = 11;
                column.Header = txt;
            }
        }

        /// <summary>
        /// The generated columns for grid.
        /// </summary>
        /// <returns>
        /// The <see cref="GridViewColumnCollection"/>.
        /// </returns>
        private GridViewColumnCollection GeneratedColumnsForGrid()
        {
            if (MembersTable == null)
            {
                return null;
            }
            var columns = new GridViewColumnCollection();

            /*
            // Toggle Row Details.
            if (!string.IsNullOrEmpty(RowDetailTemplateKey))
            {
                GridViewToggleRowDetailsColumn toggleColumn = new GridViewToggleRowDetailsColumn();
                toggleColumn.SetBinding(GridViewToggleRowDetailsColumn.IsVisibleProperty, new Binding
                {
                    Path = new PropertyPath("IsShowToggleRowDetail"),
                    Mode = BindingMode.TwoWay
                });

                columns.Add(toggleColumn);
            }
            */

            // Get Data Template for Columns
            DataTemplate columnSelectedHoverTemplate = (DataTemplate)Application.Current.Resources["ColumnSelectedHoverTemplate"];
            DataTemplate columnCheckedTemplate = (DataTemplate)Application.Current.Resources["ColumnCheckedTemplate"];
            DataTemplate columnRadioTemplate = (DataTemplate)Application.Current.Resources["ColumnRadioSelectedHoverTemplate"];
            DataTemplate columnEditDelSelectedHoverTemplate = (DataTemplate)Application.Current.Resources["ColumnEditDelSelectedHoverTemplate"];
            foreach (var dcolumn in MembersTable.Columns)
            {
                var gridColumn = new GridViewDataColumn { Header = dcolumn.Header, UniqueName = dcolumn.ColumnName };
                DynamicColumn customColumn = GridColumns.FirstOrDefault(d => d.ColumnName == dcolumn.ColumnName);

                var path = gridColumn.DataMemberBinding.Path.Path;
                gridColumn.DataMemberBinding = new Binding
                {
                    Path = new PropertyPath(path),
                    Mode = BindingMode.TwoWay
                };

                if (customColumn != null)
                {
                    // Set Data Template for Columns
                    if (customColumn.IsSelectedColumn && columnSelectedHoverTemplate != null)
                    {
                        gridColumn.CellTemplate = columnSelectedHoverTemplate;
                    }
                    if (customColumn.IsRadioSelectedColumn && columnRadioTemplate != null)
                    {
                        gridColumn.CellTemplate = columnRadioTemplate;
                    }
                    if (customColumn.Width != 0)
                    {
                        gridColumn.Width = customColumn.Width;
                    }
                    if (customColumn.MinWidth != 0)
                    {
                        gridColumn.MinWidth = customColumn.MinWidth; 
                    }
                    if (customColumn.DataFormatString != null)
                    {
                        gridColumn.DataFormatString = customColumn.DataFormatString;
                    }
                    if (!this.IsGridReadOnly)
                    {
                        gridColumn.IsReadOnly = customColumn.IsReadOnly;
                    }
                    gridColumn.TextAlignment = customColumn.TextAlignment;
                    gridColumn.HeaderTextAlignment = customColumn.HeaderTextAlignment;

                    if (FilteringGenerate)
                    {
                        switch (customColumn.FilteringTemplate)
                        {
                            case RadGridViewEnum.FilteringDataList:
                                gridColumn.IsFilterable = true;
                                gridColumn.FilterMemberPath = gridColumn.UniqueName;
                                //// column.DataMemberBinding = column.UniqueName;
                                gridColumn.FilteringControl = new FilteringDataList
                                                                  {
                                                                      ItemsSource = customColumn.FilteringDataSource,
                                                                      IsMultiSelect = true,
                                                                      Title = customColumn.FilteringTitle
                                                                  };
                                break;
                            case RadGridViewEnum.TextFieldFilter:
                                gridColumn.IsFilterable = true;
                                gridColumn.FilterMemberPath = gridColumn.UniqueName;
                                gridColumn.FilteringControl = new TextFieldFilter
                                                                  {
                                                                      AllOperators1 = customColumn.AllOperators,
                                                                      AllOperators2 = customColumn.AllOperators,
                                                                      AllLogicalOperator = customColumn.LogicalOperators,
                                                                      IsNumber = false
                                                                  };
                                break;
                            case RadGridViewEnum.FilteringComparision:
                                gridColumn.IsFilterable = true;
                                gridColumn.FilterMemberPath = gridColumn.UniqueName;
                                gridColumn.FilteringControl = new FilteringComparision
                                                                  {
                                                                      AllOperators1 = customColumn.AllOperators,
                                                                      AllOperators2 = customColumn.AllOperators,
                                                                      AllLogicalOperator = customColumn.LogicalOperators,
                                                                      IsNumber = false
                                                                  };
                                break;
                            case RadGridViewEnum.TextFieldDataListFilter:
                                gridColumn.IsFilterable = true;
                                gridColumn.FilterMemberPath = gridColumn.UniqueName;
                                gridColumn.FilteringControl = new TextFieldDataListFilter
                                                                  {
                                                                      ItemsSource = customColumn.FilteringDataSource,
                                                                      IsMultiSelect = true,
                                                                      Title = customColumn.FilteringTitle,
                                                                      AllOperators1 = customColumn.AllOperators,
                                                                      AllOperators2 = customColumn.AllOperators,
                                                                      AllLogicalOperator = customColumn.LogicalOperators,
                                                                      IsNumber = false
                                                                  };
                                break;
                            default:
                                gridColumn.IsFilterable = false;
                                break;
                        }
                    }
                    else
                    {
                        gridColumn.IsFilterable = false;
                    }

                    switch (customColumn.ColumnTemplate)
                    {
                        case RadGridViewEnum.ColumnCheckedTemplate:
                            if (columnCheckedTemplate != null)
                            {
                                gridColumn.CellTemplate = columnCheckedTemplate;
                            }
                            else
                            {
                                gridColumn.CellTemplate = columnRadioTemplate;
                            }
                            break;
                        case RadGridViewEnum.ColumnRadioSelectedHoverTemplate:
                            if (customColumn.IsRadioSelectedColumn)
                            {
                                gridColumn.CellTemplate = columnRadioTemplate;
                            }
                            break;
                        case RadGridViewEnum.ColumnSelectedHoverTemplate:
                            break;
                        case RadGridViewEnum.ColumnEditDelSelectedHoverTemplate:
                            if (columnEditDelSelectedHoverTemplate != null)
                            {
                                gridColumn.CellTemplate = columnEditDelSelectedHoverTemplate;
                            }
                            break;
                    }
                }

                columns.Add(gridColumn);
            }
            return columns;
        }

        /// <summary>
        /// The update source for filter.
        /// </summary>
        /// <param name="updatedCol">
        /// The updated col.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public void UpdateSourceForFilter(object updatedCol, int index, string name)
        {
            List<FilteringDataItem> items = updatedCol as List<FilteringDataItem>;
            var filteringDataList = this.Columns[index].FilteringControl as FilteringDataList;
            if (filteringDataList != null && items != null)
            {
                filteringDataList.ItemsSource = items;
                FilteringDataItem item = filteringDataList.ItemsSource.Cast<FilteringDataItem>().FirstOrDefault(a => a.Text == name);
                if (item != null)
                {
                    if (filteringDataList.IsActive)
                    {
                        item.PropertyChanged -= filteringDataList.SelectFilterPropertyChanged;
                        item.PropertyChanged += filteringDataList.SelectFilterPropertyChanged;
                    }
                    item.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// The add source for filter.
        /// </summary>
        /// <param name="updatedCol">
        /// The updated col.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public void AddSourceForFilter(object updatedCol, int index, string name)
        {
            List<FilteringDataItem> items = updatedCol as List<FilteringDataItem>;
            var filteringDataList = this.Columns[index].FilteringControl as FilteringDataList;
            if (filteringDataList != null && items != null)
            {
                filteringDataList.ItemsSource = items;
                FilteringDataItem item = filteringDataList.ItemsSource.Cast<FilteringDataItem>().FirstOrDefault(a => a.Text == name);
                if (item != null && !filteringDataList.IsActive)
                {
                    item.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// The set selected rows by property.
        /// </summary>
        private void SetSelectedRowsByProperty()
        {
            if (MembersTable == null)
            {
                return;
            }

            if (MembersTable.Rows.Count > 0)
            {
                var rowObject = MembersTable.Rows[0].RowObject;
                if (rowObject.GetType().GetProperty("IsSelected") != null
                    && rowObject.GetType().GetProperty("IsSelected").PropertyType == typeof(bool))
                {
                    if (MembersTable != null)
                    {
                        SelectedItems = from it in MembersTable.Rows.Select(d => d.RowObject).AsQueryable<dynamic>().ToList()
                                        where it.IsSelected == true
                                        select it;
                    }
                }
            }
        }

        /// <summary>
        /// The set selected radio rows by property.
        /// </summary>
        private void SetSelectedRadioRowsByProperty()
        {
            if (MembersTable == null)
            {
                return;
            }

            if (MembersTable.Rows.Count > 0)
            {
                var rowObject = MembersTable.Rows[0].RowObject;
                if (rowObject.GetType().GetProperty("IsRadioSelected") != null
                    && rowObject.GetType().GetProperty("IsRadioSelected").PropertyType == typeof(bool))
                {
                    if (MembersTable != null)
                    {
                        SelectedRadioItems = from it in MembersTable.Rows.Select(d => d.RowObject).AsQueryable<dynamic>().ToList()
                                        where it.IsRadioSelected == true
                                        select it;
                    }
                }
            }
        }
        #endregion

        #region Events of RadGridView

        /// <summary>
        /// The members grid loaded.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void MembersGridLoaded(object item)
        {
            if (!string.IsNullOrEmpty(RowDetailTemplateKey))
            {
                var executionContext = (ActionExecutionContext)item;
                RadGridView gridView = executionContext.Source as RadGridView;
                if (gridView != null && gridView.RowDetailsTemplate == null)
                {
                    if (Application.Current.Resources.Contains(RowDetailTemplateKey))
                    {
                        DataTemplate detailTemplate = (DataTemplate)Application.Current.Resources[RowDetailTemplateKey];
                        gridView.RowDetailsTemplate = detailTemplate;
                    }
                }
            }
        }

        /// <summary>
        /// The members grid selection changed.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void MembersGridSelectionChanged(object item)
        {
            var executionContext = (ActionExecutionContext)item;
            RadGridView gridView = executionContext.Source as RadGridView;
            SelectionChangeEventArgs eventArgs = executionContext.EventArgs as SelectionChangeEventArgs;
            if (gridView != null && eventArgs != null)
            {
                SelectedItems = gridView.SelectedItems;

                if (gridView.Items.Count > 0)
                {
                    SetSelectedRowsByProperty();
                    SetSelectedRadioRowsByProperty();
                }
            }
        }

        /// <summary>
        /// The dynamic grid_ on column width changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void DynamicGrid_OnColumnWidthChanged(object sender, object e)
        {
            var executionContext = (ActionExecutionContext)sender;
            RadGridView gridView = executionContext.Source as RadGridView;
            ColumnWidthChangedEventArgs eventArgs = e as ColumnWidthChangedEventArgs;

            if (gridView != null && gridView.IsGrouping && this.MaxWidthGrid != 0)
            {
                if (eventArgs != null && eventArgs.NewWidth.Value < eventArgs.OriginalWidth.Value)
                {
                    // Set value for max width and original width of Grid when column resize in Group mode.
                    this.MaxWidthGrid = this.MaxWidthGrid - (int)(eventArgs.OriginalWidth.Value - eventArgs.NewWidth.Value);
                    this.OriginalWidth = this.OriginalWidth - (int)(eventArgs.OriginalWidth.Value - eventArgs.NewWidth.Value);
                    GridViewDataColumn columnDetail = eventArgs.Column as GridViewDataColumn;

                    if (columnDetail != null)
                    {
                        ItemGroupPanel item =
                                    this.ListGroupByItems.FirstOrDefault(
                                        x => x.ColumnName.Equals(columnDetail.Header));
                        if (item != null)
                        {
                            item.LastWidth = item.LastWidth - (int)(eventArgs.OriginalWidth.Value - eventArgs.NewWidth.Value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The dynamic grid_ on size changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void DynamicGrid_OnSizeChanged(object sender, object e)
        {
            SizeChangedEventArgs eventArgs = e as SizeChangedEventArgs;

            if (this.IsShowGroupPanel && this.MaxWidthGrid != 0)
            {
                if (this.IsSizeChange && this.ListGroupByItems.Count > 0)
                {
                    if (eventArgs != null)
                    {
                        var lastOrDefault = this.ListGroupByItems.LastOrDefault();
                        if (lastOrDefault != null)
                        {
                            // Set value of LastWidth of column that be group when column size changed.
                            if ((int)eventArgs.PreviousSize.Width > (int)eventArgs.NewSize.Width)
                            {
                                lastOrDefault.LastWidth = (int)eventArgs.NewSize.Width;
                            }
                            else
                            {
                                lastOrDefault.LastWidth = (int)eventArgs.PreviousSize.Width;
                            }
                            
                            this.IsSizeChange = false;
                        }
                    }
                }
                else
                {
                    // Call event to set max width when not in Grouping.
                    if (this.GroupedItemChanged != null)
                    {
                        if (eventArgs != null)
                        {
                            this.GroupedItemChanged(null, (int)eventArgs.NewSize.Width);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The dynamic grid_ on grouped.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void DynamicGrid_OnGrouped(object sender, object e)
        {
            GridViewGroupedEventArgs eventArgs = e as GridViewGroupedEventArgs;

            if (this.MaxWidthGrid != 0)
            {
                if (eventArgs != null)
                {
                    // Get detail of item that be grouped by.
                    ColumnGroupDescriptor columnGroupDes = eventArgs.GroupDescriptor as ColumnGroupDescriptor;
                    if (columnGroupDes != null)
                    {
                        switch (eventArgs.Action)
                        {
                            case GroupingEventAction.Place:
                                // Insert item grouped into ListGroupByItems
                                // And call to the action GroupItemChanged to notification.
                                if (this.ListGroupByItems.Count == 0)
                                {
                                    this.OriginalWidth = this.MaxWidthGrid;
                                }

                                if (this.GroupedItemChanged != null)
                                {
                                    this.GroupedItemChanged(sender, -1);
                                }
                                this.ListGroupByItems.Add(new ItemGroupPanel { ColumnName = columnGroupDes.DisplayContent.ToString() });
                                this.IsSizeChange = true;
                                break;
                            case GroupingEventAction.Remove:
                                // Delete item grouped into ListGroupByItems
                                // And call to the action GroupItemChanged to notification.
                                ItemGroupPanel item =
                                    this.ListGroupByItems.FirstOrDefault(
                                        x => x.ColumnName.Equals(columnGroupDes.DisplayContent.ToString()));
                                if (item != null)
                                {
                                    if (this.ListGroupByItems.Count == 1)
                                    {
                                        if (this.GroupedItemChanged != null)
                                        {
                                            this.GroupedItemChanged(sender, this.OriginalWidth);
                                        }
                                    }
                                    else
                                    {
                                        if (this.GroupedItemChanged != null)
                                        {
                                            this.GroupedItemChanged(sender, item.LastWidth);
                                        }
                                    }
                                    
                                    this.ListGroupByItems.RemoveAll(x => x.ColumnName.Equals(columnGroupDes.DisplayContent.ToString()));
                                }

                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The loading row details.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void LoadingRowDetails(object item)
        {
            var executionContext = (ActionExecutionContext)item;
            GridViewRowDetailsEventArgs eventArgs = executionContext.EventArgs as GridViewRowDetailsEventArgs;
            if (eventArgs != null)
            {
                if (RowDetailLoading != null)
                {
                    RowDetailLoading(eventArgs.Row.DataContext, null);
                }
            }
            
        }

        /// <summary>
        /// The is checked item changed.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void IsCheckedItemChanged(object item)
        {
            var executionContext = (ActionExecutionContext)item;
            CheckBox checkBox = executionContext.Source as CheckBox;
            RadioButton radio = executionContext.Source as RadioButton;
            if (checkBox != null)
            {
                GridViewRow row = checkBox.ParentOfType<GridViewRow>();
                if (row != null)
                {
                    dynamic rowObject = row.DataContext;
                    SetValueForCheckedItem(rowObject, checkBox.IsChecked ?? false);
                }
            }
            if (radio != null)
            {
                GridViewRow row = radio.ParentOfType<GridViewRow>();
                if (row != null)
                {
                    dynamic rowObject = row.DataContext;
                    if (this.SelectedRadioChange != null)
                    {
                        this.SelectedRadioChange(rowObject, null);
                    }
                    SetValueForRadioCheckedItem(rowObject, radio.IsChecked ?? false);

                    var rowObject1 = MembersTable.Rows[0].RowObject;
                    if (rowObject1.GetType().GetProperty("IsRadioSelected") != null
                        && rowObject1.GetType().GetProperty("IsRadioSelected").PropertyType == typeof(bool))
                    {
                        SelectedRadioItems = rowObject;
                    }
                }
            }
            IsCheckItemChanged = true;
        }

        /// <summary>
        /// The un checked item changed.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void UnCheckedItemChanged(object item)
        {
            var executionContext = (ActionExecutionContext)item;
            CheckBox checkBox = executionContext.Source as CheckBox;
            RadioButton radio = executionContext.Source as RadioButton;
            if (checkBox != null)
            {
                GridViewRow row = checkBox.ParentOfType<GridViewRow>();
                if (row != null)
                {
                    dynamic rowObject = row.DataContext;
                    SetValueForCheckedItem(rowObject, checkBox.IsChecked ?? false);
                }
            }
            if (radio != null)
            {
                GridViewRow row = radio.ParentOfType<GridViewRow>();
                if (row != null)
                {
                    dynamic rowObject = row.DataContext;
                    SetValueForRadioCheckedItem(rowObject, radio.IsChecked ?? false);
                }
            }
            IsCheckItemChanged = true;
        }

        /// <summary>
        /// The update row.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public void UpdateRow(int index, object data)
        {
            if (data != null)
            {
                var row = MembersTable.Rows[index];
                for (int i = 0; i < data.GetType().GetProperties().Count(); i++)
                {
                    PropertyInfo pinfo = data.GetType().GetProperties()[i];
                    if (!ListPropertiesName.Contains(pinfo.Name))
                    {
                        row[pinfo.Name] = pinfo.GetValue(data, null);
                    }
                }
                MembersTable.Rows[index] = row;
            }

            // Rebind data
            if (this.ReBindData != null)
            {
                this.ReBindData();
            }
        }

        /// <summary>
        /// The delete row.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public void DeleteRow(int index)
        {
            MembersTable.Rows.RemoveAt(index);
        }

        /// <summary>
        /// The get index of grid.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetIndexOfGrid(string columnName, object id)
        {
            int index = -1;
            DataRow editItem = null;
            foreach (var row in MembersTable.Rows)
            {
                if (row[columnName].ToString() == id.ToString())
                {
                    editItem = row;
                    break;
                }
            }
            if (editItem != null)
            {
                index = MembersTable.Rows.IndexOf(editItem);
            }
            return index;
        }
        
        /// <summary>
        /// The update row detail.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void UpdateRowDetail(object item)
        {
            var executionContext = (ActionExecutionContext)item;
            Button btupdate = executionContext.Source as Button;
            if (btupdate != null)
            {
                GridViewRow row = btupdate.ParentOfType<GridViewRow>();
                
                dynamic mappingObject = row.DataContext;

                HasError = false;

                if (ValidateRow != null)
                {
                     HasError = ValidateRow(mappingObject);
                }
                if (!HasError)
                {
                    if (UpdatedItemChanged != null)
                    {
                        UpdatedItemChanged(mappingObject, null);
                    }

                    if (RowDetailLoading != null)
                    {
                        RowDetailLoading(mappingObject, null);
                    }

                    // Update IsExpandDetail value for editing item.
                    if (mappingObject.GetType().GetProperty("IsExpandDetail") != null
                        && mappingObject.GetType().GetProperty("IsExpandDetail").PropertyType == typeof(bool))
                    {
                        mappingObject.IsExpandDetail = false;
                    }

                    if (mappingObject.GetType().GetProperty("IsNewRecord") != null
                        && mappingObject.GetType().GetProperty("IsNewRecord").PropertyType == typeof(bool))
                    {
                        mappingObject.IsNewRecord = false;
                    }
                    int index = MembersTable.Rows.Select(d => d.RowObject).ToList().IndexOf(mappingObject);
                    this.MembersTable.Rows[index] = this.CreateRowItem(mappingObject);
                }
            }
        }

        /// <summary>
        /// The delete row detail.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void DeleteRowDetail(object item)
        {
            var executionContext = (ActionExecutionContext)item;
            Button btupdate = executionContext.Source as Button;

            if (btupdate != null)
            {
                GridViewRow row = btupdate.ParentOfType<GridViewRow>();
                object source = row.DataContext;
                DataRow editItem = null;
                foreach (var m in MembersTable.Rows)
                {
                    if (source != null && source == m.RowObject)
                    {
                        editItem = m;
                        break;
                    }
                }
                if (editItem != null)
                {
                    int index = MembersTable.Rows.IndexOf(editItem);
                    MembersTable.Rows.RemoveAt(index);
                }
                if (DeletedItemChanged != null)
                {
                    DeletedItemChanged(source, null);
                }
            }
        }

        /// <summary>
        /// The edit row detail.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void EditRowDetail(object item)
        {
            var executionContext = (ActionExecutionContext)item;
            Button btupdate = executionContext.Source as Button;
            if (btupdate != null)
            {
                GridViewRow row = btupdate.ParentOfType<GridViewRow>();
                row.DetailsVisibility = Visibility.Visible;
                dynamic mappingObject = row.DataContext;

                // get & add  Original value of editing item to a collection.
                if (mappingObject.GetType().GetProperty("GuidId") != null
                    && mappingObject.GetType().GetProperty("GuidId").PropertyType == typeof(Guid))
                {
                    object source = row.DataContext;
                    string itemKey = mappingObject.GuidId.ToString();
                    if (OriginalEditingItems.ContainsKey(itemKey))
                    {
                        OriginalEditingItems[itemKey] = this.CreateRowItem(source);
                    }
                    else
                    {
                        OriginalEditingItems.Add(itemKey, this.CreateRowItem(source));
                    }
                }

                // Update IsExpandDetail value for editing item.
                if (mappingObject.GetType().GetProperty("IsExpandDetail") != null
                    && mappingObject.GetType().GetProperty("IsExpandDetail").PropertyType == typeof(bool))
                {
                    mappingObject.IsExpandDetail = true;
                }

            }
        }

        /// <summary>
        /// The cancel row detail.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void CancelRowDetail(object item)
        {
            var executionContext = (ActionExecutionContext)item;
            Button btupdate = executionContext.Source as Button;
            if (btupdate != null)
            {
                GridViewRow row = btupdate.ParentOfType<GridViewRow>();
                row.DetailsVisibility = Visibility.Collapsed;
                dynamic mappingObject = row.DataContext;

                HandleCancelEditItem(mappingObject);

                // Update IsExpandDetail value for editing item.
                if (mappingObject.GetType().GetProperty("IsExpandDetail") != null
                    && mappingObject.GetType().GetProperty("IsExpandDetail").PropertyType == typeof(bool))
                {
                    mappingObject.IsExpandDetail = false;
                }

                if (mappingObject.GetType().GetProperty("IsNewRecord") != null
                         && mappingObject.GetType().GetProperty("IsNewRecord").PropertyType == typeof(bool))
                {
                    if (mappingObject.IsNewRecord)
                        {
                            // Raise event for exist new item on grid 
                            if (this.AddedNewItem != null)
                            {
                                this.AddedNewItem(false, null);
                            }
                        }
                }

                if (CanceledItemChanged != null)
                {
                    // int index = MembersTable.Rows.Select(d => d.RowObject).ToList().IndexOf(mappingObject);
                    CanceledItemChanged(mappingObject, null);
                }
            }
        }

        /// <summary>
        /// The set value for checked item.
        /// </summary>
        /// <param name="mappingObject">
        /// The mapping object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void SetValueForCheckedItem(dynamic mappingObject, bool value)
        {
            if (mappingObject.GetType().GetProperty("IsSelected") != null
                        && mappingObject.GetType().GetProperty("IsSelected").PropertyType == typeof(bool))
            {
                mappingObject.IsSelected = value;
            }
        }

        /// <summary>
        /// The set value for radio checked item.
        /// </summary>
        /// <param name="mappingObject">
        /// The mapping object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void SetValueForRadioCheckedItem(dynamic mappingObject, bool value)
        {
            if (mappingObject != null)
            {
                if (mappingObject.GetType().GetProperty("IsRadioSelected") != null
                    && mappingObject.GetType().GetProperty("IsRadioSelected").PropertyType == typeof(bool))
                {
                    mappingObject.IsRadioSelected = value;
                }
            }
        }

        /// <summary>
        /// The handle cancel edit item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        private void HandleCancelEditItem(dynamic item)
        {
            if (item.GetType().GetProperty("GuidId") != null
                   && item.GetType().GetProperty("GuidId").PropertyType == typeof(Guid))
            {
                string key = item.GuidId.ToString();
                if (OriginalEditingItems.ContainsKey(key))
                {
                    DataRow originalItem = OriginalEditingItems[key];
                    int index = MembersTable.Rows.Select(d => d.RowObject).ToList().IndexOf(item);
                    MembersTable.Rows[index] = originalItem;
                }
            }
        }

        /// <summary>
        /// The create row item.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="DataRow"/>.
        /// </returns>
        private DataRow CreateRowItem(object data)
        {
            if (data != null)
            {
                var row = MembersTable.NewRow();
                for (int i = 0; i < data.GetType().GetProperties().Count(); i++)
                {
                    PropertyInfo pinfo = data.GetType().GetProperties()[i];
                    if (!ListPropertiesName.Contains(pinfo.Name))
                    {
                        row[pinfo.Name] = pinfo.GetValue(data, null);
                    }
                }

                return row;
            }
            return null;
        }

        /// <summary>
        /// The delete new record when have another new record.
        /// </summary>
        /// <param name="columnName">
        /// The column Name.
        /// </param>
        public void HandleNewRecord(string columnName)
        {
            if (MembersTable != null)
            {
                var row = MembersTable.Rows[0];
                object a = 0;
                if (row[columnName].ToString() == a.ToString())
                {
                    MembersTable.Rows.RemoveAt(0);
                }
            }
        }

        #endregion

        #region Notify Property Changed

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
