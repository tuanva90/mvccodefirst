using Insyston.Operations.WPF.ViewModels.Assets.AssetClasses;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Insyston.Operations.WPF.Views.Assets.AssetClasses
{
    using Insyston.Operations.WPF.ViewModels.Assets;

    /// <summary>
    /// Interaction logic for AssetClassesCategoryView.xaml
    /// </summary>
    public partial class AssetClassesCategoryView : UserControl
    {
        public AssetClassesCategoryView()
        {
            InitializeComponent();
            this.Loaded += AssetClassesCategoryView_Loaded;
        }

        async void AssetClassesCategoryView_Loaded(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as AssetClassesCategoryViewModel;
            if (model != null)
            {
                this.model_StepChanged("MainViewState");
                model.StepChanged += model_StepChanged;
                model.OnStoryBoardChanged += model_OnStoryBoardChanged;
                await model.OnStepAsync("Start");
                if (model.PermissionCategoryDetail.CanSee)
                {
                    this.model_OnStoryBoardChanged("AssetClassesCategoryDetailState");
                    model.CurrentTab = Asset.EnumSteps.AssetClassesCategoryDetailState;
                }
                else if (model.PermissionCategoryFeature.CanSee)
                {
                    this.model_OnStoryBoardChanged("AssetClassesCategoryFeaturesState");
                    model.CurrentTab = Asset.EnumSteps.AssetClassesCategoryFeaturesState;
                }
                else if (model.PermissionCategoryType.CanSee)
                {
                    this.model_OnStoryBoardChanged("AssetClassesCategoryAssetTypesState");
                    model.CurrentTab = Asset.EnumSteps.AssetClassesCategoryAssetTypesState;
                }
            }
        }

        void model_OnStoryBoardChanged(string storyBoard)
        {
            //((Storyboard)this.Resources["AssetClassesCategoryDetailState"]).Stop();
            ((Storyboard)this.Resources["AssetClassesCategoryFeaturesState"]).Stop();
            ((Storyboard)this.Resources["AssetClassesCategoryAssetTypesState"]).Stop();
            ((Storyboard)this.Resources["AssetClassesCategoryAssignFeaturesState"]).Stop();
            ((Storyboard)this.Resources["AssetClassesCategoryAssignTypesState"]).Stop();
            ((Storyboard)this.Resources["AssetClassesCategoryUpdateDepreciationState"]).Stop();
            ((Storyboard)this.Resources[storyBoard]).Begin();
        }

        void model_StepChanged(string storyBoard)
        {
            ((Storyboard)this.Resources["MainViewState"]).Stop();
            ((Storyboard)this.Resources["MainContentState"]).Stop();
            ((Storyboard)this.Resources["BulkUpdateState"]).Stop();
            ((Storyboard)this.Resources[storyBoard]).Begin();
        }
    }
}
