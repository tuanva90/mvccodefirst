// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Users.xaml.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for User.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.View.Security
{
    using System.Windows;
    using System.Windows.Media.Animation;

    using Insyston.Operations.WPF.View.Common;
    using Insyston.Operations.WPF.ViewModel.Security;

    /// <summary>
    /// Interaction logic for User.xaml
    /// </summary>
    public partial class Users
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Users"/> class.
        /// </summary>
        public Users()
        {
            this.InitializeComponent();            
            this.Loaded += this.Users_Loaded;
            this.SizeChanged += this.Users_SizeChanged;
        }

        /// <summary>
        /// The users_ resize grid.
        /// </summary>
        private void Users_ResizeGrid()
        {
            this.CloseEdit();
            GridSummary.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// The users_ step changed.
        /// </summary>
        /// <param name="stepName">
        /// The step name.
        /// </param>
        private void Users_StepChanged(string stepName)
        {
            if ((stepName == ViewUsersViewModel.EnumSteps.SelectUser.ToString() || stepName == ViewUsersViewModel.EnumSteps.Add.ToString() ||
                 stepName == ViewUsersViewModel.EnumSteps.Edit.ToString()) && ((ViewUsersViewModel)this.DataContext).IsUserSelected == false)
            {
                this.GridSummary_MouseDoubleClick(null, null);
            }
        }

        /// <summary>
        /// The edit users_ step changed.
        /// </summary>
        /// <param name="stepName">
        /// The step name.
        /// </param>
        private void EditUsers_StepChanged(string stepName)
        {
            if (stepName == EditUserViewModel.EnumSteps.Cancel.ToString() && ((ViewUsersViewModel)this.DataContext).SelectedUser.IsNewUser)
            {
                this.CloseEdit();   
            }
        }

        /// <summary>
        /// The users_ loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void Users_Loaded(object sender, RoutedEventArgs e)
        {
            ((ViewUsersViewModel)this.DataContext).StepChanged += this.Users_StepChanged;
            ((ViewUsersViewModel)this.DataContext).Edit.StepChanged += this.EditUsers_StepChanged;
            ((ViewUsersViewModel)this.DataContext).ReSizeGrid = this.Users_ResizeGrid;
            await((ViewUsersViewModel)this.DataContext).OnStepAsync(ViewUsersViewModel.EnumSteps.Start);
        }

        /// <summary>
        /// The grid summary_ mouse double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void GridSummary_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e != null && this.GridSummary.SelectedItem == null)
            {
                return;
            }

            if (((ViewUsersViewModel)this.DataContext).IsUserSelected == false)
            {
                this.SelectedView.Margin = new Thickness(5);
                ((ViewUsersViewModel)this.DataContext).IsUserSelected = true;
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
                await((ViewUsersViewModel)this.DataContext).OnStepAsync(ViewUsersViewModel.EnumSteps.SelectUser);
            }
        }

        /*private void Splitter_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
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
                    if (storyBoard != null)
                    {
                        ((DoubleAnimation)storyBoard.Children[0]).From = this.SelectedView.ActualWidth;
                        ((DoubleAnimation)storyBoard.Children[0]).To = this.MainGrid.ColumnDefinitions[0].ActualWidth - 15;

                        storyBoard.Begin();
                    }

                    Point mousePosition = e.MouseDevice.GetPosition(this.MainGrid);

                    if (this.MainGrid.ColumnDefinitions[2].ActualWidth < 220 && mousePosition.X > this.MainGrid.ActualWidth - 240)
                    {
                        e.Handled = true;
                    }
                }
            }
        }
        */

        /// <summary>
        /// The splitter_ preview mouse move.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        ///     The e.
        /// </param>
        /// <summary>
        /// The grid summary_ key down.
        /// </summary>
        private void GridSummary_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.GridSummary_MouseDoubleClick(sender, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// The grid summary_ sorted.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GridSummary_Sorted(object sender, Telerik.Windows.Controls.GridViewSortedEventArgs e)
        {
            ((ViewUsersViewModel)this.DataContext).OnSortChanged(e.Column.UniqueName, e.Column.SortingState);
        }

        /// <summary>
        /// The grid summary_ filtered.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GridSummary_Filtered(object sender, Telerik.Windows.Controls.GridView.GridViewFilteredEventArgs e)
        {
            ((ViewUsersViewModel)this.DataContext).OnFilterChanged(
                e.ColumnFilterDescriptor.Column.UniqueName,
                e.ColumnFilterDescriptor.DistinctFilter.DistinctValues,
                e.ColumnFilterDescriptor.FieldFilter.Filter1,
                e.ColumnFilterDescriptor.FieldFilter.Filter2,
                e.ColumnFilterDescriptor.FieldFilter.LogicalOperator);
        }

        /// <summary>
        /// The grid summary_ loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void GridSummary_Loaded(object sender, RoutedEventArgs e)
        {
            this.GridSummary.SetDefaultSortFilterSettings(await((ViewUsersViewModel)this.DataContext).GetUserDefaultSettingsAsync());
        }

        /// <summary>
        /// The users_ size changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Users_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (((ViewUsersViewModel)this.DataContext).SelectedUser != null || ((ViewUsersViewModel)this.DataContext).IsUserSelected)
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

        /// <summary>
        /// The close edit.
        /// </summary>
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

                ViewUsersViewModel context = this.DataContext as ViewUsersViewModel;
                if (context != null)
                {
                    context.SelectedUser = null;
                    context.IsUserSelected = false;
                    context.CloseCommandsEdit();
                }

                storyBoard.Begin();
            }
        }
    }
}
