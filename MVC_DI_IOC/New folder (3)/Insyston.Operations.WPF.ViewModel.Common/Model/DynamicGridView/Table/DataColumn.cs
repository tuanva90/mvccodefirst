// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataColumn.cs" company="Brightstar Corporation">
//   Copyright (c) Brightstar Corporation. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataColumn type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace WPF.DataTable.Models
{
    using System;

    /// <summary>
    /// The data column.
    /// </summary>
    public class DataColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataColumn"/> class.
        /// </summary>
        public DataColumn()
        {
            DataType = typeof(object);
        }

        /// <summary>
        /// Gets or sets the data type.
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is read only.
        /// </summary>
        public bool IsReadOnly { get; set; }
    }
}
