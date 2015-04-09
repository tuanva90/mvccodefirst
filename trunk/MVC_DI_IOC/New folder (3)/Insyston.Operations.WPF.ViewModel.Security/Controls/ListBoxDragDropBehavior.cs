using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.DragDrop;
using System.Collections.ObjectModel;
using System.Linq;
using Insyston.Operations.Business.Security.Model;
namespace Insyston.Operations.WPF.ViewModels.Security.Controls
{
    public class ListBoxDragDropBehavior
    {

        /// <summary>
        /// AssociatedObject Property
        /// </summary>
        public ListBox AssociatedObject { get; set; }

        private static readonly Dictionary<ListBox, ListBoxDragDropBehavior> instances;

        static ListBoxDragDropBehavior()
        {
            instances = new Dictionary<ListBox, ListBoxDragDropBehavior>();
        }

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            ListBoxDragDropBehavior behavior = GetAttachedBehavior(obj as ListBox);

            behavior.AssociatedObject = obj as ListBox;

            if (value)
            {
                behavior.Initialize();
            }
            else
            {
                behavior.CleanUp();
            }
            obj.SetValue(IsEnabledProperty, value);
        }
       
        // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ListBoxDragDropBehavior),
                new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));


        public static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            SetIsEnabled(dependencyObject, (bool)e.NewValue);
        }
        private static ListBoxDragDropBehavior GetAttachedBehavior(ListBox listBox)
        {
            if (!instances.ContainsKey(listBox))
            {
                instances[listBox] = new ListBoxDragDropBehavior {AssociatedObject = listBox};
            }

            return instances[listBox];
        }

        protected virtual void Initialize()
        {
            this.UnsubscribeFromDragDropEvents();
            this.SubscribeToDragDropEvents();
        }

        protected virtual void CleanUp()
        {
            this.UnsubscribeFromDragDropEvents();
        }

        private void SubscribeToDragDropEvents()
        {
            DragDropManager.AddDragInitializeHandler(this.AssociatedObject, OnDragInitialize);
            //DragDropManager.AddGiveFeedbackHandler(this.AssociatedObject, OnGiveFeedback);
            DragDropManager.AddDropHandler(this.AssociatedObject, OnDrop);
            DragDropManager.AddDragDropCompletedHandler(this.AssociatedObject, OnDragDropCompleted);
            //DragDropManager.AddPreviewDropHandler(this.AssociatedObject, OnPreviewDrop);
        }

        private void OnPreviewDrop(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
           
        }

        private void UnsubscribeFromDragDropEvents()
        {
            DragDropManager.RemoveDragInitializeHandler(this.AssociatedObject, OnDragInitialize);
            //DragDropManager.RemoveGiveFeedbackHandler(this.AssociatedObject, OnGiveFeedback);
            DragDropManager.RemoveDropHandler(this.AssociatedObject, OnDrop);
            DragDropManager.RemoveDragDropCompletedHandler(this.AssociatedObject, OnDragDropCompleted);
            //DragDropManager.RemovePreviewDropHandler(this.AssociatedObject, OnPreviewDrop);
        }

        private void OnDragInitialize(object sender, DragInitializeEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                var draggedItem = listBox.SelectedItems;
                string dataVisual = string.Empty;
                e.AllowedEffects = DragDropEffects.All;
                var data = DragDropPayloadManager.GeneratePayload(null);
                _CollectorForAdd.Clear();
                if (draggedItem != null)
                {
                    foreach(var item in draggedItem)
                    {
                        _CollectorForAdd.Add((Membership)item);
                        dataVisual += ((Membership)item).UserName + "; ";
                    }
                }
                dataVisual = dataVisual.Remove(dataVisual.Length - 2, 2);
                data.SetData("DraggedData", draggedItem);

                e.DragVisual = new DragVisual()
                {
                    Content = dataVisual,
                };

                e.DragVisualOffset = e.RelativeStartPoint;
                e.Data = data;
            }
        }

        private static List<Membership> _CollectorForAdd = new List<Membership>();
        private void OnGiveFeedback(object sender, Telerik.Windows.DragDrop.GiveFeedbackEventArgs e)
        {
            e.SetCursor(Cursors.Arrow);
            e.Handled = true;
        }

        private void OnDragDropCompleted(object sender, DragDropCompletedEventArgs e)
        {
            var draggedItem = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedData");

            if (draggedItem == null)
            {
                var listBox = sender as ListBox;
                if (listBox != null)
                {
                    var collection = listBox.ItemsSource as IList;
                    foreach (var item in _CollectorForAdd)
                    {
                        if (collection != null) collection.Remove(item);
                    }
                }
            }
        }

        private void OnDrop(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            var draggedItem = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedData") as List<Membership>;
            var listBox = sender as ListBox;

            if (listBox != null)
            {
                if (draggedItem != null)
                {
                    var list = listBox.ItemsSource as ObservableCollection<Membership>;
                    if (list != null)
                    {
                        foreach (var item in draggedItem)
                        {
                            Membership tmp = new Membership() { UserId = item.UserId, UserName = item.UserName };
                            if (list.Count(l => l.UserId == item.UserId) == 0)
                            {
                                list.Add(tmp);
                            }
                        }
                    }
                }
                else
                {
                    var list = listBox.ItemsSource as ObservableCollection<Membership>;
                    if (list != null)
                    {
                        foreach (var item in _CollectorForAdd)
                        {
                            Membership tmp = new Membership() { UserId = item.UserId, UserName = item.UserName };
                            if (list.Count(l => l.UserId == item.UserId) == 0)
                            {
                                list.Add(tmp);
                            }
                        }
                    }
                    e.Handled = true;
                }
            }
        }
    }
}
