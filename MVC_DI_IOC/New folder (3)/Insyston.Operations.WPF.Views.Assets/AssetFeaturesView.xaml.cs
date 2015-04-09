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

namespace Insyston.Operations.WPF.Views.Assets
{
    using System.Windows.Media.Animation;

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Security;
    using Insyston.Operations.WPF.ViewModels.Assets;

    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for AssetFeaturesView.xaml
    /// </summary>
    public partial class AssetFeaturesView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetFeaturesView"/> class.
        /// </summary>
        public AssetFeaturesView()
        {
            InitializeComponent();
            this.Loaded += this.AssetFeaturesView_Loaded;
        }

        /// <summary>
        /// The asset features view_ loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void AssetFeaturesView_Loaded(object sender, RoutedEventArgs e)
        {
            AssetFeaturesViewModel model = this.DataContext as AssetFeaturesViewModel;
            if (model != null)
            {
                model.OnStoryBoardChanged -= this.OnStoryBoardChanged;
                model.OnStoryBoardChanged += this.OnStoryBoardChanged;
                model.StepChanged += this.AssetFeatures_Stepchanged;
                this.OnStoryBoardChanged("GridSummaryState");
                var permissionFeatureDetail = Authorisation.GetPermission(Components.SystemManagementAssetFeatures, Forms.FeaturesDetail);
                var permissionFeatureAssignTo = Authorisation.GetPermission(Components.SystemManagementAssetFeatures, Forms.FeaturesAssignTo);
                if (permissionFeatureDetail.CanSee)
                {
                    this.AssetFeatures_Stepchanged("DetailsState");
                    model.CurrentTab = Asset.EnumSteps.DetailsState;
                }

                if (permissionFeatureAssignTo.CanSee)
                {
                    if (!permissionFeatureDetail.CanSee)
                    {
                        this.AssetFeatures_Stepchanged("AssignedToState");
                        model.CurrentTab = Asset.EnumSteps.AssignedToState;
                    }
                }


                await model.OnStepAsync(Asset.EnumSteps.Start);
            }
        }

        private void AssetFeatures_Stepchanged(string storyBoard)
        {
            ((Storyboard)this.Resources["AssignedToState"]).Stop();
            ((Storyboard)this.Resources["DetailsState"]).Stop();
            ((Storyboard)this.Resources[storyBoard]).Begin();
        }

        /// <summary>
        /// The on story board changed.
        /// </summary>
        /// <param name="storyBoard">
        /// The story board.
        /// </param>
        private void OnStoryBoardChanged(string storyBoard)
        {
            ((Storyboard)this.Resources["AssignFeatureState"]).Stop();
            ((Storyboard)this.Resources["GridContentState"]).Stop();
            ((Storyboard)this.Resources["GridSummaryState"]).Stop();
            ((Storyboard)this.Resources[storyBoard]).Begin();
        }
    }
}
