using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Insyston.Operations.WPF.Views.Common;
using Insyston.Operations.WPF.ViewModels.Security;

namespace Insyston.Operations.WPF.Views.Security
{
    /// <summary>
    /// Interaction logic for User.xaml
    /// </summary>
    public partial class GroupsView : UserControl
    {
        public GroupsView()
        {
            this.InitializeComponent();
            this.Loaded += this.Groups_Loaded;
            this.SizeChanged += this.Groups_SizeChanged;            
        }

        private void Groups_ResizeGrid()
        {
            this.CloseEdit();
            GridSummary.Visibility = Visibility.Visible;
        }

        private void Groups_Stepchanged(string stepName)
        {
            if ((stepName == GroupsViewModel.EnumSteps.Add.ToString() || stepName == GroupsViewModel.EnumSteps.SelectGroup.ToString() ||
                 stepName == GroupsViewModel.EnumSteps.Edit.ToString()) && ((GroupsViewModel)this.DataContext).IsGroupSelected == false)
            {
                this.GridSummary_MouseDoubleClick(null, null);
            }
        }

        private void EditGroups_StepChanged(string stepName)
        {
            if (stepName == EditGroupViewModel.EnumSteps.Cancel.ToString() && ((GroupsViewModel)this.DataContext).SelectedGroup.IsNewGroup == true)
            {
                this.CloseEdit();
                GridSummary.Visibility = Visibility.Visible;
            }
        }

        private async void Groups_Loaded(object sender, RoutedEventArgs e)
        {
            ((GroupsViewModel)this.DataContext).StepChanged += this.Groups_Stepchanged;
            ((GroupsViewModel)this.DataContext).Edit.StepChanged += this.EditGroups_StepChanged;
            ((GroupsViewModel)this.DataContext).ReSizeGrid = this.Groups_ResizeGrid;
            await ((GroupsViewModel)this.DataContext).OnStepAsync(GroupsViewModel.EnumSteps.Start);
        }
       
        private async void GridSummary_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e != null && this.GridSummary.SelectedItem == null)
            {
                return;
            }

            if (((GroupsViewModel)this.DataContext).IsGroupSelected == false)
            {
                this.SelectedView.Margin = new Thickness(5);
                ((GroupsViewModel)this.DataContext).IsGroupSelected = true;
                Storyboard storyBoard = this.Resources["GridTransition"] as Storyboard;
                if (storyBoard != null)
                {
                    ((DoubleAnimation)storyBoard.Children[0]).From = 0;
                    ((DoubleAnimation)storyBoard.Children[0]).To = this.MainGrid.ActualWidth;

                    this.SelectedView.Visibility = Visibility.Visible;
                    storyBoard.Begin();
                }
                this.GridSummary.Visibility = Visibility.Collapsed;
            }
            else
            {
                await ((GroupsViewModel)this.DataContext).OnStepAsync(Insyston.Operations.WPF.ViewModels.Security.GroupsViewModel.EnumSteps.SelectGroup);
            }
        }

        //private void Splitter_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
        //    { 
        //        if (this.MainGrid.ColumnDefinitions[this.MainGrid.ColumnDefinitions.Count - 1].ActualWidth >= this.MainGrid.ActualWidth * .65)
        //        {
        //            this.CloseEdit();
        //        }
        //        else
        //        { 
        //            Storyboard storyBoard = this.Resources["ControlTransition"] as Storyboard;
        //            ((DoubleAnimation)storyBoard.Children[0]).From = this.SelectedView.ActualWidth;
        //            ((DoubleAnimation)storyBoard.Children[0]).To = this.MainGrid.ColumnDefinitions[0].ActualWidth - 15;

        //            storyBoard.Begin();

        //            Point mousePosition = e.MouseDevice.GetPosition(this.MainGrid);

        //            if (this.MainGrid.ColumnDefinitions[2].ActualWidth < 200 && mousePosition.X > this.MainGrid.ActualWidth - 220)
        //            {
        //                e.Handled = true;
        //            }
        //        }
        //    }
        //}

        private void GridSummary_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.GridSummary_MouseDoubleClick(sender, null);
                e.Handled = true;
            }
        }

        private void GridSummary_Sorted(object sender, Telerik.Windows.Controls.GridViewSortedEventArgs e)
        {
            ((GroupsViewModel)this.DataContext).OnSortChanged(e.Column.UniqueName, e.Column.SortingState);
        }

        private void GridSummary_Filtered(object sender, Telerik.Windows.Controls.GridView.GridViewFilteredEventArgs e)
        {
            ((GroupsViewModel)this.DataContext).OnFilterChanged(e.ColumnFilterDescriptor.Column.UniqueName, e.ColumnFilterDescriptor.DistinctFilter.DistinctValues, e.ColumnFilterDescriptor.FieldFilter.Filter1,
                e.ColumnFilterDescriptor.FieldFilter.Filter2, e.ColumnFilterDescriptor.FieldFilter.LogicalOperator); 
        }

        private async void GridSummary_Loaded(object sender, RoutedEventArgs e)
        {
            this.GridSummary.SetDefaultSortFilterSettings(await ((GroupsViewModel)this.DataContext).UserDefaultSettingsAsync());
        }
                 
        private void Groups_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (((GroupsViewModel)this.DataContext).SelectedGroup != null || ((GroupsViewModel)this.DataContext).IsGroupSelected)
            {
                Storyboard storyBoard = this.Resources["ControlTransition"] as Storyboard;
                if (storyBoard != null)
                {
                    ((DoubleAnimation)storyBoard.Children[0]).From = this.ActualWidth;
                    ((DoubleAnimation)storyBoard.Children[0]).To = this.ActualWidth;

                    storyBoard.Completed += (o, args) =>
                    {
                        this.MainGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                        this.MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
                        this.MainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                    };

                    storyBoard.Begin();
                }
            }
        }

        private void CloseEdit()
        {
            Storyboard storyBoard = this.Resources["ControlTransition"] as Storyboard;
            if (storyBoard != null)
            {

                ((DoubleAnimation)storyBoard.Children[0]).From = this.SelectedView.ActualWidth;
                ((DoubleAnimation)storyBoard.Children[0]).To = 0;

                storyBoard.Completed += (o, args) =>
                    {
                        if (this.SelectedView.ActualWidth <= 100)
                        {
                            this.MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
                            this.MainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                            this.SelectedView.Margin = new Thickness(0);
                            this.SelectedView.Visibility = Visibility.Visible;
                            this.Splitter.Visibility = Visibility.Collapsed;
                        }
                    };

                GroupsViewModel context = this.DataContext as GroupsViewModel;
                if (context != null)
                {
                    context.SelectedGroup = null;
                    context.IsGroupSelected = false;
                    context.CloseCommandsEdit();
                }

                storyBoard.Begin();
            }
        }
    }
}
