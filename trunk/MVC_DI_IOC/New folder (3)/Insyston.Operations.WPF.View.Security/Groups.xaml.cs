using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Insyston.Operations.WPF.View.Common;
using Insyston.Operations.WPF.ViewModel.Security;

namespace Insyston.Operations.WPF.View.Security
{
    /// <summary>
    /// Interaction logic for User.xaml
    /// </summary>
    public partial class Groups : UserControl
    {
        public Groups()
        {
            this.InitializeComponent();
            this.DataContext = new ViewGroupsViewModel();
            ((ViewGroupsViewModel)this.DataContext).StepChanged += this.Groups_Stepchanged;
            ((ViewGroupsViewModel)this.DataContext).Edit.StepChanged += this.EditGroups_StepChanged;

            this.Loaded += this.Groups_Loaded;
            this.SizeChanged += this.Groups_SizeChanged;            
        }
        
        private void Groups_Stepchanged(string stepName)
        {
            if ((stepName == ViewGroupsViewModel.EnumSteps.Add.ToString() || stepName == ViewGroupsViewModel.EnumSteps.SelectGroup.ToString() ||
                 stepName == ViewGroupsViewModel.EnumSteps.Edit.ToString()) && ((ViewGroupsViewModel)this.DataContext).IsGroupSelected == false)
            {
                this.GridSummary_MouseDoubleClick(null, null);
            }
        }

        private void EditGroups_StepChanged(string stepName)
        {
            if (stepName == EditGroupViewModel.EnumSteps.Cancel.ToString() && ((ViewGroupsViewModel)this.DataContext).SelectedGroup.IsNewGroup == true)
            {
                this.CloseEdit();
            }
        }

        private async void Groups_Loaded(object sender, RoutedEventArgs e)
        {
            await ((ViewGroupsViewModel)this.DataContext).OnStepAsync(ViewGroupsViewModel.EnumSteps.Start);
        }
       
        private async void GridSummary_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e != null && this.GridSummary.SelectedItem == null)
            {
                return;
            }

            if (((ViewGroupsViewModel)this.DataContext).IsGroupSelected == false)
            {
                this.SelectedView.Margin = new Thickness(5);
                ((ViewGroupsViewModel)this.DataContext).IsGroupSelected = true;
                Storyboard storyBoard = this.Resources["GridTransition"] as Storyboard;
                ((DoubleAnimation)storyBoard.Children[0]).To = this.MainGrid.ActualWidth - 205;
                this.SelectedView.Visibility = System.Windows.Visibility.Visible;
                this.Splitter.Visibility = System.Windows.Visibility.Visible;                
                storyBoard.Begin();
                this.Splitter.PreviewMouseMove += this.Splitter_PreviewMouseMove;
            }
            else
            {
                await ((ViewGroupsViewModel)this.DataContext).OnStepAsync(Insyston.Operations.WPF.ViewModel.Security.ViewGroupsViewModel.EnumSteps.SelectGroup);
            }
        }

        private void Splitter_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            { 
                if (this.MainGrid.ColumnDefinitions[this.MainGrid.ColumnDefinitions.Count - 1].ActualWidth >= this.MainGrid.ActualWidth * .65)
                {
                    this.CloseEdit();
                }
                else
                { 
                    Storyboard storyBoard = this.Resources["ControlTransition"] as Storyboard;
                    ((DoubleAnimation)storyBoard.Children[0]).From = this.SelectedView.ActualWidth;
                    ((DoubleAnimation)storyBoard.Children[0]).To = this.MainGrid.ColumnDefinitions[0].ActualWidth - 15;

                    storyBoard.Begin();

                    Point mousePosition = e.MouseDevice.GetPosition(this.MainGrid);

                    if (this.MainGrid.ColumnDefinitions[2].ActualWidth < 200 && mousePosition.X > this.MainGrid.ActualWidth - 220)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

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
            ((ViewGroupsViewModel)this.DataContext).OnSortChanged(e.Column.UniqueName, e.Column.SortingState);
        }

        private void GridSummary_Filtered(object sender, Telerik.Windows.Controls.GridView.GridViewFilteredEventArgs e)
        {
            ((ViewGroupsViewModel)this.DataContext).OnFilterChanged(e.ColumnFilterDescriptor.Column.UniqueName, e.ColumnFilterDescriptor.DistinctFilter.DistinctValues, e.ColumnFilterDescriptor.FieldFilter.Filter1,
                e.ColumnFilterDescriptor.FieldFilter.Filter2, e.ColumnFilterDescriptor.FieldFilter.LogicalOperator); 
        }

        private async void GridSummary_Loaded(object sender, RoutedEventArgs e)
        {
            this.GridSummary.SetDefaultSortFilterSettings(await ((ViewGroupsViewModel)this.DataContext).UserDefaultSettingsAsync());
        }
                 
        private void Groups_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (((ViewGroupsViewModel)this.DataContext).IsGroupSelected)
            {
                this.Splitter.PreviewMouseMove -= this.Splitter_PreviewMouseMove;
                Storyboard storyBoard = this.Resources["ControlTransition"] as Storyboard;
                ((DoubleAnimation)storyBoard.Children[0]).From = this.SelectedView.ActualWidth;
                ((DoubleAnimation)storyBoard.Children[0]).To = this.ActualWidth - 205;                

                storyBoard.Completed += (o, args) =>
                {
                    this.MainGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    this.MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
                    this.MainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                    this.Splitter.PreviewMouseMove += this.Splitter_PreviewMouseMove;
                };

                storyBoard.Begin();
            }
        }

        private void CloseEdit()
        {
            Storyboard storyBoard = this.Resources["ControlTransition"] as Storyboard;
            ((DoubleAnimation)storyBoard.Children[0]).From = this.SelectedView.ActualWidth;
            ((DoubleAnimation)storyBoard.Children[0]).To = 0;

            storyBoard.Completed += new EventHandler((o, args) =>
            {
                if (this.SelectedView.ActualWidth <= 100)
                { 
                    this.MainGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    this.MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
                    this.MainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                    this.Splitter.PreviewMouseMove -= this.Splitter_PreviewMouseMove;                    
                    this.SelectedView.Margin = new Thickness(0);
                    this.Splitter.Visibility = System.Windows.Visibility.Collapsed;
                    this.SelectedView.Visibility = System.Windows.Visibility.Collapsed;
                }
            });

            ViewGroupsViewModel context = this.DataContext as ViewGroupsViewModel;
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
