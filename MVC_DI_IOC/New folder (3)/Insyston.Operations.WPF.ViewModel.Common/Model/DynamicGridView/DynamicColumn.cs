// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicColumn.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The dynamic column.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;

    using Insyston.Operations.WPF.ViewModels.Common.Enums;
    using Insyston.Operations.WPF.ViewModels.Common.Model.Filtering;

    using Telerik.Windows.Data;

    /// <summary>
    /// The dynamic column.
    /// </summary>
    public class DynamicColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicColumn"/> class.
        /// </summary>
        public DynamicColumn()
        {
            FilteringTemplate = RadGridViewEnum.None;
            ColumnTemplate = RadGridViewEnum.None;
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is selected column.
        /// </summary>
        public bool IsSelectedColumn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is radio selected column.
        /// </summary>
        public bool IsRadioSelectedColumn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is read only.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the min width.
        /// </summary>
        public int MinWidth { get; set; }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        public TextAlignment TextAlignment
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the header text alignment.
        /// </summary>
        public TextAlignment HeaderTextAlignment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filtering template.
        /// </summary>
        public RadGridViewEnum FilteringTemplate { get; set; }

        /// <summary>
        /// Gets or sets the filtering data source.
        /// </summary>
        public List<FilteringDataItem> FilteringDataSource { get; set; }

        /// <summary>
        /// Gets or sets the all operators.
        /// </summary>
        public IEnumerable AllOperators { get; set; }

        /// <summary>
        /// Gets or sets the logical operators.
        /// </summary>
        public IEnumerable LogicalOperators { get; set; }

        /// <summary>
        /// Gets or sets the filtering title.
        /// </summary>
        public string FilteringTitle { get; set; }

        /// <summary>
        /// Gets or sets the column template.
        /// </summary>
        public RadGridViewEnum ColumnTemplate { get; set; }

        /// <summary>
        /// Gets or sets the data format string.
        /// </summary>
        public string DataFormatString { get; set; }
    }
}
