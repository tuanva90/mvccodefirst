using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Insyston.Operations.WPF.Views.Assets.AssetClasses
{
    using System.Windows.Media.Animation;

    using Insyston.Operations.WPF.ViewModels.Assets.AssetClasses;

    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for AssetClassesMakeView.xaml
    /// </summary>
    public partial class AssetClassesMakeView : UserControl
    {
        public AssetClassesMakeView()
        {
            InitializeComponent();
            this.Loaded += AssetClassesMakeView_Loaded;
        }

        private async void AssetClassesMakeView_Loaded(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as AssetClassesMakeViewModel;
            if (model != null)
            {
                ((Storyboard)this.Resources["MainViewState"]).Begin();
                ((Storyboard)this.Resources["DetailState"]).Begin();
                model.StepChanged += model_StepChanged;
                model.OnStoryBoardChanged += model_OnStoryBoardChanged;
                model.OnStepAsync("Start");
                
            }
        }

        private void model_OnStoryBoardChanged(string storyboard)
        {
            ((Storyboard)this.Resources["DetailState"]).Stop();
            ((Storyboard)this.Resources["BulkState"]).Stop();
            ((Storyboard)this.Resources[storyboard]).Begin();
        }

        private void model_StepChanged(string storyBoard)
        {

            ((Storyboard)this.Resources["MainViewState"]).Stop();
            ((Storyboard)this.Resources["MainContentState"]).Stop();
            ((Storyboard)this.Resources[storyBoard]).Begin();
        }
    }
}

