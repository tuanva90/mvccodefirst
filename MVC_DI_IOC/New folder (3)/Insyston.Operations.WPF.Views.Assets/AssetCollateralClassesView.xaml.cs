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
using WPFDynamic.Controls;

namespace Insyston.Operations.WPF.Views.Assets
{
    using System.Windows.Media.Animation;

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Security;
    using Insyston.Operations.WPF.ViewModels.Assets;

    /// <summary>
    /// Interaction logic for AssetCollateralClassesView.xaml
    /// </summary>
    public partial class AssetCollateralClassesView : UserControl
    {
        public AssetCollateralClassesView()
        {
            InitializeComponent();
            this.Loaded += this.AssetCollateralClassesView_Loaded;
        }

        private async void AssetCollateralClassesView_Loaded(object sender, RoutedEventArgs e)
        {
            AssetCollateralClassesViewModel model = this.DataContext as AssetCollateralClassesViewModel;
            if (model != null)
            {
                model.OnStoryBoardChanged -= this.OnStoryBoardChanged;
                model.OnStoryBoardChanged += this.OnStoryBoardChanged;
                model.StepChanged += this.AssetCollateralClassesView_Stepchanged;
                this.OnStoryBoardChanged("GridSummaryState");
                //this.AssetCollateralClassesView_Stepchanged("DetailsState");
                var permissionCollateralDetail = Authorisation.GetPermission(Components.SystemManagementAssetCollateralClasses, Forms.CollateralClassesDetail);
                var permissionCollateralType = Authorisation.GetPermission(Components.SystemManagementAssetCollateralClasses, Forms.CollateralClassesType);
                
                if (permissionCollateralDetail.CanSee)
                {
                    this.AssetCollateralClassesView_Stepchanged("DetailsState");
                    model.CurrentTab = Asset.EnumSteps.DetailsState;
                }

                if (permissionCollateralType.CanSee)
                {
                    if (!permissionCollateralDetail.CanSee)
                    {
                        this.AssetCollateralClassesView_Stepchanged("AssignedToState");
                        model.CurrentTab = Asset.EnumSteps.AssignedToState;
                    }
                }
                await model.OnStepAsync(Asset.EnumSteps.Start);
            }
        }

        private void AssetCollateralClassesView_Stepchanged(string storyBoard)
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
