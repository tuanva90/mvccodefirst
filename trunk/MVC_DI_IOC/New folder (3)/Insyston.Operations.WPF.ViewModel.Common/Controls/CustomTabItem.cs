// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomTabItem.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The custom tab item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Telerik.Windows.Controls;

    /// <summary>
    /// The custom tab item.
    /// </summary>
    public class CustomTabItem : RadTabItem
    {
        public List<ItemValidateContent> ItemvalidateTabContent;
    }
}
