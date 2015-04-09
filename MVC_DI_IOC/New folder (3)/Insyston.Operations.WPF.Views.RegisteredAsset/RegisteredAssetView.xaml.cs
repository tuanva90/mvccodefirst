// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisteredAssetView.xaml.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Interaction logic for RegisteredAssetView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.Views.RegisteredAsset
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;

    using Insyston.Operations.WPF.ViewModels.RegisteredAsset;

    /// <summary>
    /// Interaction logic for RegisteredAssetView.xaml
    /// </summary>
    public partial class RegisteredAssetView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredAssetView"/> class.
        /// </summary>
        public RegisteredAssetView()
        {
            this.InitializeComponent();
            this.Loaded -= this.RegisteredAssetView_Loaded;
            this.Loaded += this.RegisteredAssetView_Loaded;
        }

        /// <summary>
        /// The registered asset view_ loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public async void RegisteredAssetView_Loaded(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as RegisteredAssetViewModel;
            if (model != null)
            {
                this.model_StepChanged("GridSummaryState");
                model.StepChanged += this.model_StepChanged;
                model.OnStoryBoardChanged += this.model_OnStoryBoardChanged;
                await model.OnStepAsync("Start");
            }
        }

        private void model_OnStoryBoardChanged(string storyboard)
        {
            //((Storyboard)this.Resources["DetailsState"]).Stop();
            ((Storyboard)this.Resources["DepreciationState"]).Stop();
            ((Storyboard)this.Resources["DisposalState"]).Stop();
            ((Storyboard)this.Resources["HistoryState"]).Stop();
            ((Storyboard)this.Resources[storyboard]).Begin();
        }

        /// <summary>
        /// The model_ step changed.
        /// </summary>
        /// <param name="storyBoard">
        /// The story Board.
        /// </param>
        private void model_StepChanged(string storyBoard)
        {           
            ((Storyboard)this.Resources["GridSummaryState"]).Stop();
            ((Storyboard)this.Resources["GridContentState"]).Stop();
            ((Storyboard)this.Resources[storyBoard]).Begin();
        }
    }
}
