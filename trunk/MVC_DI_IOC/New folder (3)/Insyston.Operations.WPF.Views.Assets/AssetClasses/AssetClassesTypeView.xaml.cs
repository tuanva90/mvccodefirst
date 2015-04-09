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
using Insyston.Operations.WPF.ViewModels.Assets.AssetClasses;

namespace Insyston.Operations.WPF.Views.Assets.AssetClasses
{
    using Insyston.Operations.WPF.ViewModels.Assets;

    /// <summary>
    /// Interaction logic for AssetClassesTypeView.xaml
    /// </summary>
    public partial class AssetClassesTypeView : UserControl
    {
        public AssetClassesTypeView()
        {
            InitializeComponent();
            this.Loaded += AssetClassesCategoryView_Loaded;
        }

        async void AssetClassesCategoryView_Loaded(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as AssetClassesTypeViewModel;
            if (model != null)
            {
                this.model_StepChanged("MainViewState");
                model.StepChanged += model_StepChanged;
                model.OnStoryBoardChanged += model_OnStoryBoardChanged;
                await model.OnStepAsync("Start");

                if (model.PermissionTypeDetail.CanSee)
                {
                    this.model_OnStoryBoardChanged("AssetClassesTypeDetailState");
                    model.CurrentTab = Asset.EnumSteps.AssetClassesTypeDetailState;
                }
                else if (model.PermissionTypeFeature.CanSee)
                {
                    this.model_OnStoryBoardChanged("AssetClassesTypeFeaturesState");
                    model.CurrentTab = Asset.EnumSteps.AssetClassesTypeFeaturesState;
                }
                else if (model.PermissionTypeMake.CanSee)
                {
                    this.model_OnStoryBoardChanged("AssetClassesTypeMakeState");
                    model.CurrentTab = Asset.EnumSteps.AssetClassesTypeMakeState;
                }
            }
        }

        void model_OnStoryBoardChanged(string storyBoard)
        {
            //((Storyboard)this.Resources["AssetClassesCategoryDetailState"]).Stop();
            ((Storyboard)this.Resources["AssetClassesTypeFeaturesState"]).Stop();
            ((Storyboard)this.Resources["AssetClassesTypeMakeState"]).Stop();
            ((Storyboard)this.Resources["AssetClassesTypeAssignFeaturesState"]).Stop();
            ((Storyboard)this.Resources["AssetClassesTypeAssignMakeState"]).Stop();
            ((Storyboard)this.Resources["AssetClassesTypeUpdateDepreciationState"]).Stop();
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
