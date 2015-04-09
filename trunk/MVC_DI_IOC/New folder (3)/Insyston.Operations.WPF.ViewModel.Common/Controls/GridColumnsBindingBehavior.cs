// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridColumnsBindingBehavior.cs" company="Brightstar Corporation">
//   Copyright (c) Brightstar Corporation. All rights reserved.
// </copyright>
// <summary>
//   The grid columns binding behavior.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WPFDynamic.Helpers.Behaviors
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.Windows;

    using Telerik.Windows.Controls;

    /// <summary>
    /// The grid columns binding behavior.
    /// </summary>
    public class GridColumnsBindingBehavior
    {
        /// <summary>
        /// The grid.
        /// </summary>
        private readonly RadGridView grid;

        /// <summary>
        /// The columns.
        /// </summary>
        private readonly INotifyCollectionChanged columns;

        /// <summary>
        /// The columns property.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached(
            "Columns",
            typeof(INotifyCollectionChanged),
            typeof(GridColumnsBindingBehavior),
            new PropertyMetadata(OnColumnsPropertyChanged));

        /// <summary>
        /// The set columns.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="columns">
        /// The columns.
        /// </param>
        public static void SetColumns(DependencyObject dependencyObject, INotifyCollectionChanged columns)
        {
            dependencyObject.SetValue(ColumnsProperty, columns);
        }

        /// <summary>
        /// The get columns.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <returns>
        /// The <see cref="INotifyCollectionChanged"/>.
        /// </returns>
        public static INotifyCollectionChanged GetColumns(DependencyObject dependencyObject)
        {
            return (INotifyCollectionChanged)dependencyObject.GetValue(ColumnsProperty);
        }

        /// <summary>
        /// The on columns property changed.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnColumnsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RadGridView grid = dependencyObject as RadGridView;
            INotifyCollectionChanged columns = e.NewValue as INotifyCollectionChanged;

            if (grid != null && columns != null)
            {
                GridColumnsBindingBehavior behavior = new GridColumnsBindingBehavior(grid, columns);
                behavior.Attach();
            }
        }

        /// <summary>
        /// The attach.
        /// </summary>
        private void Attach()
        {
            if (grid != null && columns != null)
            {
                Transfer(GetColumns(grid) as IList, grid.Columns);

                columns.CollectionChanged -= this.ContextColumnsCollectionChanged;
                columns.CollectionChanged += this.ContextColumnsCollectionChanged;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridColumnsBindingBehavior"/> class.
        /// </summary>
        /// <param name="grid">
        /// The grid.
        /// </param>
        /// <param name="columns">
        /// The columns.
        /// </param>
        public GridColumnsBindingBehavior(RadGridView grid, INotifyCollectionChanged columns)
        {
            this.grid = grid;
            this.columns = columns;
        }

        /// <summary>
        /// The context ContextColumnsCollectionChanged changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ContextColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UnsubscribeFromEvents();

            Transfer(GetColumns(grid) as IList, grid.Columns);

            SubscribeToEvents();
        }

        /// <summary>
        /// The grid columns_ collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GridColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UnsubscribeFromEvents();

            Transfer(grid.Columns, GetColumns(grid) as IList);

            SubscribeToEvents();
        }

        /// <summary>
        /// The subscribe to events.
        /// </summary>
        private void SubscribeToEvents()
        {
            grid.Columns.CollectionChanged += this.GridColumnsCollectionChanged;

            if (GetColumns(grid) != null)
            {
                GetColumns(grid).CollectionChanged += this.ContextColumnsCollectionChanged;
            }
        }

        /// <summary>
        /// The unsubscribe from events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            grid.Columns.CollectionChanged -= this.GridColumnsCollectionChanged;

            if (GetColumns(grid) != null)
            {
                GetColumns(grid).CollectionChanged -= this.ContextColumnsCollectionChanged;
            }
        }

        /// <summary>
        /// The transfer.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        public static void Transfer(IList source, IList target)
        {
            if (source == null || target == null)
            {
                return;
            }
            
           // Comment this line to remain the columns that we design in the UI.
            target.Clear();

            foreach (object o in source)
            {
                target.Add(o);
            }
        }
    }
}
