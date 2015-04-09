using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Insyston.Operations.WPF.ViewModels.Funding;

namespace Insyston.Operations.WPF.Views.Funding
{
    using System.Windows.Threading;

    using Caliburn.Micro;

    /// <summary>
    /// Interaction logic for FundingSummary.xaml
    /// </summary>
    /// 
    public partial class FundingSummaryView : UserControl
    {
        /// <summary>
        /// Gets or sets a value indicating the funding module is detail or summary
        /// </summary>
        private bool IsDetailView { get; set; }

        /// <summary>
        /// Gets the  DataContext of the <see cref="FundingSummaryView"/>
        /// </summary>
        private FundingSummaryViewModel ViewModel
        {
            get
            {
                return this.DataContext as FundingSummaryViewModel;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FundingSummaryView"/> class.
        /// </summary>
        public FundingSummaryView()
        {
            this.InitializeComponent();
            this.Loaded += this.FundingSummary_Loaded;
            this.SizeChanged += this.FundingSummary_SizeChanged;
        }

        private void FundingSummary_StepChanged(string stepName)
        {
            if ((stepName == FundingSummaryViewModel.EnumStep.SelectTranche.ToString() ||
                 stepName == FundingSummaryViewModel.EnumStep.CreateNew.ToString()))
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
            if (null == ViewModel) return;
            if (IsDetailView)
            {
                var storyBoard = this.Resources["DetailsTransition"] as Storyboard;
                if (null != storyBoard)
                {
                    ((DoubleAnimation)storyBoard.Children[0]).From = this.FundingDetailsUserControl.ActualWidth;
                    ((DoubleAnimation)storyBoard.Children[0]).To = this.ActualWidth;
                    FundingSummaryTransition(Visibility.Collapsed);
                    storyBoard.Begin();
                }
            }
        }

        private async void FundingSummary_Loaded(object sender, RoutedEventArgs e)
        {
            if (null == ViewModel) return;
            ViewModel.StepChanged += this.FundingSummary_StepChanged;
            ViewModel.ReSizeGrid += FundingSummary_ResizeGrid;
            await ViewModel.OnStepAsync(FundingSummaryViewModel.EnumStep.Start);
        }

        private async void FundingSummary_ResizeGrid()
        {
            if (null == ViewModel)
                return;
            await ViewModel.OnStepAsync(FundingSummaryViewModel.EnumStep.Start);
        }

        private void DisplayForm()
        {
            if (null == ViewModel) return;
            var storyBoard = this.Resources["SummaryTransition"] as Storyboard;
            if (null != storyBoard)
            {
                IsDetailView = true;
                ((DoubleAnimation)storyBoard.Children[0]).To = this.MainGrid.ActualWidth;
                FundingSummaryTransition(Visibility.Collapsed);
                storyBoard.Begin();
            }
        }

        private void DisplaySummary()
        {
            if (null == ViewModel) return;

            var storyBoard = this.Resources["DetailsTransition"] as Storyboard;
            if (null != storyBoard)
            {
                IsDetailView = false;
                ((DoubleAnimation)storyBoard.Children[0]).From = this.FundingDetailsUserControl.ActualWidth;
                ((DoubleAnimation)storyBoard.Children[0]).To = 0;
                FundingSummaryTransition(Visibility.Visible);
                storyBoard.Begin();

            }
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments, 
        /// at the specified priority, on the thread that the <see cref="T:System.Windows.Threading.Dispatcher"/> was created on.
        /// </summary>
        /// <param name="visible">the visibility of the FundingSummaryGrid control</param>
        private void FundingSummaryTransition(Visibility visible)
        {
            Dispatcher.BeginInvoke(
                   new System.Action(
                   () =>
                   {
                       this.FundingSummaryGrid.Visibility = visible;
                   }),
                   DispatcherPriority.Render);
        }
    }
}