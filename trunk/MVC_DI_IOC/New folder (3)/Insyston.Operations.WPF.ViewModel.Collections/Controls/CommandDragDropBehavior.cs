using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Telerik.Windows.Controls.DragDrop;

namespace Insyston.Operations.WPF.ViewModels.Collections.Controls
{
    public class CommandDragDropBehavior : Behavior<ItemsControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            RadDragAndDropManager.AddDragQueryHandler(this.AssociatedObject, OnDragQuery);
            RadDragAndDropManager.AddDragInfoHandler(this.AssociatedObject, OnDragInfo);

            RadDragAndDropManager.AddDropQueryHandler(this.AssociatedObject, OnDropQuery);
            RadDragAndDropManager.AddDropInfoHandler(this.AssociatedObject, OnDropInfo);
        }

        public void OnDragQuery(object sender, DragDropQueryEventArgs args)
        {
            var param = new DragDropParameter
            {
                ItemsSource = this.AssociatedObject.ItemsSource,
                DragStatus = args.Options.Status
            };

            if (DragCommand != null && DragCommand.CanExecute(param))
            {
                args.QueryResult = true;
            }
        }

        public void OnDragInfo(object sender, DragDropEventArgs args)
        {
            var param = new DragDropParameter
            {
                DraggedItem = ((FrameworkElement)args.Options.Source).DataContext,
                ItemsSource = this.AssociatedObject.ItemsSource,
                DragStatus = args.Options.Status
            };

            if (DragCommand != null && DragCommand.CanExecute(param))
            {
                DragCommand.Execute(param);

                args.Options.Payload = param.DraggedItem;

                if (args.Options.DragCue == null)
                {
                    var dragCue = new ContentControl {ContentTemplate = DragCueTemplate, Content = args.Options.Payload};

                    args.Options.DragCue = dragCue;
                }
            }
        }

        public void OnDropQuery(object sender, DragDropQueryEventArgs args)
        {
            //Note that this.AssociatedObject.ItemsSource will represent the destination ItemsSource.
            var param = new DragDropParameter
            {
                DraggedItem = args.Options.Payload,
                ItemsSource = this.AssociatedObject.ItemsSource,
                DragStatus = args.Options.Status
            };

            if (DropCommand != null && DropCommand.CanExecute(param) && args.Options.Source != args.Options.Destination)
            {
                args.QueryResult = true;
            }
        }

        public void OnDropInfo(object sender, DragDropEventArgs args)
        {
            //Note that this.AssociatedObject.ItemsSource will represent the destination ItemsSource.
            var param = new DragDropParameter
            {
                DraggedItem = args.Options.Payload,
                ItemsSource = this.AssociatedObject.ItemsSource,
                DragStatus = args.Options.Status
            };

            if (DropCommand != null && DropCommand.CanExecute(param))
            {
                DropCommand.Execute(param);
            }
        }



        public ICommand DragCommand
        {
            get { return (ICommand)GetValue(DragCommandProperty); }
            set { SetValue(DragCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DragCommandProperty =
            DependencyProperty.Register("DragCommand", typeof(ICommand), typeof(CommandDragDropBehavior), new PropertyMetadata(null));



        public ICommand DropCommand
        {
            get { return (ICommand)GetValue(DropCommandProperty); }
            set { SetValue(DropCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.Register("DropCommand", typeof(ICommand), typeof(CommandDragDropBehavior), new PropertyMetadata(null));

        public DataTemplate DragCueTemplate
        {
            get { return (DataTemplate)GetValue(DragCueTemplateProperty); }
            set { SetValue(DragCueTemplateProperty, value); }
        }

        public static readonly DependencyProperty DragCueTemplateProperty =
            DependencyProperty.Register("DragCueTemplate", typeof(DataTemplate), typeof(CommandDragDropBehavior), new PropertyMetadata(null));
    }
}
