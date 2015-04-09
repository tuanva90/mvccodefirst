// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnConfig.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The column config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Controls
{
    using System.Collections.Generic;

    /// <summary>
    /// The column config.
    /// </summary>
    public class ColumnConfig
    {
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        public IEnumerable<Column> Columns { get; set; }
    }
}