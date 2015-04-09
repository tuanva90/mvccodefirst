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
    /// Interaction logic for AssetClassesModelView.xaml
    /// </summary>
    public partial class AssetClassesModelView : UserControl
    {
        public AssetClassesModelView()
        {
            InitializeComponent();
            this.Loaded += AssetClassesModelView_Loaded;
        }

        private async void AssetClassesModelView_Loaded(object sender, RoutedEventArgs e)
        {           
            var model = this.DataContext as AssetClassesModelViewModel;
            if (model != null)
            {
                this.StepChanged("MainGridState");
                model.StepChanged -= StepChanged;
                model.StepChanged += StepChanged;
                await model.OnStepAsync(Asset.EnumSteps.Start);               
            }
        }

        private void StepChanged(string stepChanged)
        {
            ((Storyboard)this.Resources["MainGridState"]).Stop();
            ((Storyboard)this.Resources["DetailState"]).Stop();
            ((Storyboard)this.Resources[stepChanged]).Begin();
        }
    }
}
