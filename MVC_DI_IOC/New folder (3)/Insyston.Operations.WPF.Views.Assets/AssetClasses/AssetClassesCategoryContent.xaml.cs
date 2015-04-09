using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Insyston.Operations.WPF.ViewModels.Assets.AssetClasses;
using Telerik.Windows.Controls;

namespace Insyston.Operations.WPF.Views.Assets.AssetClasses
{
    /// <summary>
    /// Interaction logic for AssetClassesCategoryContent.xaml
    /// </summary>
    public partial class AssetClassesCategoryContent : UserControl
    {
        public AssetClassesCategoryContent()
        {
            InitializeComponent();

            this.Loaded += AssetClassesCategoryContent_Loaded;
        }

        void AssetClassesCategoryContent_Loaded(object sender, RoutedEventArgs e)
        {
            AssetClassesCategoryView parent = this.ParentOfType<UserControl>() as AssetClassesCategoryView;     

            if (parent != null)
            {
                AssetClassesCategoryViewModel model = parent.DataContext as AssetClassesCategoryViewModel;
                model.OnStoryBoardChanged += OnStoryBoardChanged;
                this.OnStoryBoardChanged("AssetClassesCategoryDetailState");
            }
        }

        private void OnStoryBoardChanged(string storyBoard)
        {
            ((Storyboard)this.Resources["AssetClassesCategoryDetailState"]).Stop();
            ((Storyboard)this.Resources["AssetClassesCategoryFeaturesState"]).Stop();
            ((Storyboard)this.Resources["AssetClassesCategoryAssetTypesState"]).Stop();
            ((Storyboard)this.Resources["AssetClassesCategoryAssignFeaturesState"]).Stop();
            ((Storyboard)this.Resources["AssetClassesCategoryUpdateDepreciationState"]).Stop();
            ((Storyboard)this.Resources[storyBoard]).Begin();
        }
    }
}
