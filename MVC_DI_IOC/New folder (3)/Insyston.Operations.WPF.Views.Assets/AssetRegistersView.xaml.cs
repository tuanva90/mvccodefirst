// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetRegistersView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for AssetRegistersView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.Views.Assets
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;
    using Insyston.Operations.WPF.ViewModels.Assets;

    /// <summary>
    /// Interaction logic for AssetRegistersView.xaml
    /// </summary>
    public partial class AssetRegistersView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetRegistersView"/> class.
        /// </summary>
        public AssetRegistersView()
        {
            InitializeComponent();
            this.Loaded += AssetRegisterView_Loaded;
        }

        /// <summary>
        /// The asset register view_ loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void AssetRegisterView_Loaded(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as AssetRegistersViewModel;
            if (model != null)
            {
                ((Storyboard)this.Resources["MainViewState"]).Begin();
                ((Storyboard)this.Resources["DetailState"]).Begin();
                model.StepChanged += model_StepChanged;
                model.OnStoryBoardChanged += model_OnStoryBoardChanged;
                await model.OnStepAsync("Start");
            }
        }

        /// <summary>
        /// The model_ on story board changed.
        /// </summary>
        /// <param name="storyboard">
        /// The storyboard.
        /// </param>
        private void model_OnStoryBoardChanged(string storyboard)
        {
            ((Storyboard)this.Resources["DetailState"]).Stop();
            ((Storyboard)this.Resources[storyboard]).Begin();
        }

        /// <summary>
        /// The model_ step changed.
        /// </summary>
        /// <param name="storyBoard">
        /// The story board.
        /// </param>
        private void model_StepChanged(string storyBoard)
        {
            ((Storyboard)this.Resources["MainViewState"]).Stop();
            ((Storyboard)this.Resources["MainContentState"]).Stop();
            ((Storyboard)this.Resources[storyBoard]).Begin();
        }
    }
}

