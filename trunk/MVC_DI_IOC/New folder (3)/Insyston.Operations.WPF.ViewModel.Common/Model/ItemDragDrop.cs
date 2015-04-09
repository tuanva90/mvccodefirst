// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemDragDrop.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the ItemDragDrop type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Model
{
    /// <summary>
    /// The item drag drop.
    /// </summary>
    public class ItemDragDrop 
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is none drop item.
        /// </summary>
        public bool IsNoneDropItem { get; set; }
    }
}
