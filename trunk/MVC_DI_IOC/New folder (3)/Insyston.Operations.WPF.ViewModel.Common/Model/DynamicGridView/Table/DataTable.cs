// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTable.cs" company="Brightstar Corporation">
//   Copyright (c) Brightstar Corporation. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WPF.DataTable.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Linq.Dynamic;
    using Telerik.Windows.Controls;
    using WPF.DataTable.Models;

    /// <summary>
    /// The data table.
    /// </summary>
    /// <typeparam name="TBaseClass">
    /// Base Class Type
    /// </typeparam>
    public class DataTable : IEnumerable, INotifyCollectionChanged
    {
        public Type TBaseClass { get; set; }
        /// <summary>
        /// The columns.
        /// </summary>
        private IList<DataColumn> columns;

        /// <summary>
        /// The rows.
        /// </summary>
        private ObservableCollection<DataRow> rows;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTable{TBaseClass}"/> class.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        public DataTable(string typeName)
        {
            this.TypeName = typeName;
        }

        /// <summary>
        /// The collection changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Gets the type name.
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        public IList<DataColumn> Columns
        {
            get
            {
                if (this.columns == null)
                {
                    this.columns = new List<DataColumn>();
                }

                return this.columns;
            }
        }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        public IList<DataRow> Rows
        {
            get
            {
                if (this.rows == null)
                {
                    this.rows = new ObservableCollection<DataRow>();
                    this.rows.CollectionChanged += this.OnRowsCollectionChanged;
                }

                return this.rows;
            }
        }

        /// <summary>
        /// The new row.
        /// </summary>
        /// <returns>
        /// </returns>
        public DataRow NewRow()
        {
            return new DataRow(this);

        }
       
        public IEnumerator GetEnumerator()
        {
            return this.InternalView.GetEnumerator();
        }

        public IList ToList()
        {
            return this.InternalView;
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = this.CollectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Seals this instance.
        /// </summary>
        /// <Created>19/07/2012</Created>
        private void Seal()
        {
            this.columns = new ReadOnlyCollection<DataColumn>(this.Columns);
        }
        private void OnRowsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.InternalView.Insert(e.NewStartingIndex, ((DataRow)e.NewItems[0]).RowObject);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.InternalView.RemoveAt(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.InternalView.Remove(((DataRow)e.OldItems[0]).RowObject);
                    this.InternalView.Insert(e.NewStartingIndex, ((DataRow)e.NewItems[0]).RowObject);
                    break;
                default:
                    this.InternalView.Clear();
                    this.Rows.Select(r => r.RowObject).ToList().ForEach(o => this.InternalView.Add(o));
                    break;
            }
        }
        private IList InternalView
        {
            get
            {
                if (this.internalView == null)
                {
                    this.CreateInternalView();
                }

                return this.internalView;
            }
        }
        /// <summary>
        /// Creates the internal view.
        /// </summary>
        /// <Created>19/07/2012</Created>
        private void CreateInternalView()
        {
            this.internalView = (IList)Activator.CreateInstance(typeof(ObservableCollection<>).MakeGenericType(TBaseClass));
            ((INotifyCollectionChanged)this.internalView).CollectionChanged += (s, e) => this.OnCollectionChanged(e);
        }
        /// <summary>
        /// The internal view.
        /// </summary>
        private IList internalView;
    }
}