using Caliburn.Micro;
using Insyston.Operations.Business.Common.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls;
using WPF.DataTable.Models;

namespace Insyston.Operations.WPF.ViewModels.Common.Controls
{
    using System.Windows.Controls;

    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;

    using GridViewColumnCollection = Telerik.Windows.Controls.GridViewColumnCollection;

    public class DynamicHoverGridViewModel:Screen
    {
        public Action<object> SelectedItemChanged;

        public bool IsSetSelectedItem
        {
            get;
            set;
        }

        public Type TBaseClass { get; set; }

        /// <summary>
        /// The _members table.
        /// </summary>
        private DataTable _membersTable;

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
                this.NotifyOfPropertyChange(() => MembersTable);
                // OnPropertyChanged("MembersTable");
            }
        }

        /// <summary>
        /// The _selected product.
        /// </summary>
        private object _selectedItem;

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

                this.NotifyOfPropertyChange(() => MembersTable);
                // OnPropertyChanged("SelectedItem");

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

        public List<DynamicColumn> GridColumns { get; set; }

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
                this.NotifyOfPropertyChange(() => Columns);
                // OnPropertyChanged("Columns");
            }
        }

        #region Constructors
        public DynamicHoverGridViewModel(Type dataType)
        {
            TBaseClass = dataType;
        }
        #endregion

        #region Dynamic Methods
        public void LoadRadGridView()
        {
            
            CreateMemberTableColumns();
            GetDataForGridView();
        }
        private void CreateMemberTableColumns()
        {

            if (MembersTable == null)
            {
                _membersTable = new DataTable(Guid.NewGuid().ToString());
                _membersTable.TBaseClass = TBaseClass;
            }
            _membersTable.Columns.Clear();

            foreach (var systemColum in GridColumns)
            {
                DataColumn column = new DataColumn
                {
                    ColumnName = systemColum.ColumnName,
                    Header = systemColum.Header,
                    DataType = typeof(string)
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
                txt.TextWrapping = TextWrapping.Wrap;

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
            var collection = new List<GridViewDataColumn>();

            DataTemplate ColumnCheckboxSelected = (DataTemplate)Application.Current.Resources["ColumnCheckboxSelected"];

            foreach (var dcolumn in MembersTable.Columns)
            {              
                var gridColumn = new GridViewDataColumn { Header = dcolumn.Header, UniqueName = dcolumn.ColumnName };
                DynamicColumn customColumn = GridColumns.FirstOrDefault(d => d.ColumnName == dcolumn.ColumnName);

                var path = gridColumn.DataMemberBinding.Path.Path;
                gridColumn.DataMemberBinding = new Binding
                {
                    Path = new PropertyPath(path),
                    Mode = BindingMode.TwoWay,
                };
                if (customColumn != null && customColumn.IsSelectedColumn && ColumnCheckboxSelected != null)
                {
                    gridColumn.CellTemplate = ColumnCheckboxSelected;
                }

                columns.Add(gridColumn);
            }
            return columns;
        }

        public void GetDataForGridView()
        {
            if (GridDataRows != null && GridDataRows.Count > 0)
            {
                MembersTable.Rows.Clear();
                object data = GridDataRows[0];
                foreach (object drow in GridDataRows)
                {
                    var row = MembersTable.NewRow();
                    for (int i = 0; i < drow.GetType().GetProperties().Count(); i++)
                    {
                        PropertyInfo pInfo = drow.GetType().GetProperties()[i];
                        row[pInfo.Name] = pInfo.GetValue(drow, null);
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
                    PropertyInfo pInfo = data.GetType().GetProperties()[i];
                    row[pInfo.Name] = pInfo.GetValue(data, null);
                }
                MembersTable.Rows.Insert(index, row);
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
                    PropertyInfo pInfo = data.GetType().GetProperties()[i];
                    row[pInfo.Name] = pInfo.GetValue(data, null);
                }
                MembersTable.Rows.Add(row);
            }
        }

        #endregion

        //public event PropertyChangedEventHandler PropertyChanged;
        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }
}
