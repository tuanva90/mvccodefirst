using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Insyston.Operations.WPF.ViewModels.Funding;

namespace Insyston.Operations.WPF.Views.Funding
{
    /// <summary>
    /// Interaction logic for FundingSummary.xaml
    /// </summary>
    public partial class FundingSummary : UserControl
    {
        public FundingSummary()
        {
            this.InitializeComponent();
            this.DataContext = new FundingSummaryViewModel();
            ((FundingSummaryViewModel)this.DataContext).StepChanged += this.FundingSummary_StepChanged;
            this.Loaded += this.FundingSummary_Loaded;
            this.SizeChanged += this.FundingSummary_SizeChanged;
        }

        private void FundingSummary_StepChanged(string stepName)
        {
            if ((stepName == FundingSummaryViewModel.EnumStep.SelectTranche.ToString() ||
                 stepName == FundingSummaryViewModel.EnumStep.CreateNew.ToString()) &&
                ((FundingSummaryViewModel)this.DataContext).IsFundingSelected == false)
            {
                this.DisplayForm();
            }
            else if (stepName == FundingSummaryViewModel.EnumStep.Start.ToString())
            {
                this.DisplaySummary();
            }
        }

        private void FundingSummary_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (((FundingSummaryViewModel)this.DataContext).IsFundingSelected)
            {
                this.Splitter.PreviewMouseMove -= this.Splitter_PreviewMouseMove;
                Storyboard storyBoard = this.Resources["DetailsTransition"] as Storyboard;
                ((DoubleAnimation)storyBoard.Children[0]).From = this.FundingDetailsUserControl.ActualWidth;
                ((DoubleAnimation)storyBoard.Children[0]).To = this.ActualWidth - 220;

                storyBoard.Completed -= this.storyBoard_Completed_SizeChanged;
                storyBoard.Completed += this.storyBoard_Completed_SizeChanged;
                
                storyBoard.Begin();
            }
        }

        private async void FundingSummary_Loaded(object sender, RoutedEventArgs e)
        {
            await ((FundingSummaryViewModel)this.DataContext).OnStepAsync(FundingSummaryViewModel.EnumStep.Start);            
        }

        private void DisplayForm()
        {
            if (((FundingSummaryViewModel)this.DataContext).IsFundingSelected == false)
            {
                this.FundingDetailsUserControl.Margin = new Thickness(5);
                ((FundingSummaryViewModel)this.DataContext).IsFundingSelected = true;
                Storyboard storyBoard = this.Resources["SummaryTransition"] as Storyboard;
                ((DoubleAnimation)storyBoard.Children[0]).To = this.MainGrid.ActualWidth - 225;
                this.Splitter.Visibility = System.Windows.Visibility.Visible;
                storyBoard.Begin();
                this.Splitter.PreviewMouseMove += this.Splitter_PreviewMouseMove;
            }
        }

        private void Splitter_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if (this.MainGrid.ColumnDefinitions[this.MainGrid.ColumnDefinitions.Count - 1].ActualWidth >= this.MainGrid.ActualWidth * .65)
                {
                    this.DisplaySummary();
                }
                else
                {
                    Storyboard storyBoard = this.Resources["DetailsTransition"] as Storyboard;
                    ((DoubleAnimation)storyBoard.Children[0]).From = this.FundingDetailsUserControl.ActualWidth;
                    ((DoubleAnimation)storyBoard.Children[0]).To = this.MainGrid.ColumnDefinitions[0].ActualWidth - 15;

                    storyBoard.Begin();

                    Point mousePosition = e.MouseDevice.GetPosition(this.MainGrid);

                    if (this.MainGrid.ColumnDefinitions[2].ActualWidth < 220 && mousePosition.X > this.MainGrid.ActualWidth - 240)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void DisplaySummary()
        {
            Storyboard storyBoard = this.Resources["DetailsTransition"] as Storyboard;
            ((DoubleAnimation)storyBoard.Children[0]).From = this.FundingDetailsUserControl.ActualWidth;
            ((DoubleAnimation)storyBoard.Children[0]).To = 0;
            
            storyBoard.Completed -= this.storyBoard_Completed;            
            storyBoard.Completed += this.storyBoard_Completed;
            
            storyBoard.Begin();
        }

        private void storyBoard_Completed_SizeChanged(object sender, EventArgs e)
        {
            this.MainGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
            this.MainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
            this.Splitter.PreviewMouseMove += this.Splitter_PreviewMouseMove;
        }

        private void storyBoard_Completed(object sender, EventArgs e)
        {
            if (this.FundingDetailsUserControl.ActualWidth <= 100)
            {
                this.MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
                this.MainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
                this.Splitter.PreviewMouseMove -= this.Splitter_PreviewMouseMove;

                ((FundingSummaryViewModel)this.DataContext).IsFundingSelected = false;
                this.FundingDetailsUserControl.Margin = new Thickness(0);
                this.Splitter.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
