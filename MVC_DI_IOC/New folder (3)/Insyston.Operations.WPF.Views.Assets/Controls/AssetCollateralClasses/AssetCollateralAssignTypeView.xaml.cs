using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Insyston.Operations.Bussiness.Assets.Model;
using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetCollateralClasses;

namespace Insyston.Operations.WPF.Views.Assets.Controls.AssetCollateralClasses
{
    /// <summary>
    /// Interaction logic for AssetCollateralAssignTypeView.xaml
    /// </summary>
    public partial class AssetCollateralAssignTypeView : UserControl
    {
        public AssetCollateralAssignTypeView()
        {
            InitializeComponent();
            //this.Loaded += Collateral_Loaded;
        }
        private async void Collateral_Loaded(object sender, RoutedEventArgs e)
        {
            AssetCollateralRowItem item1 = new AssetCollateralRowItem
            {
                CollateralClassID = 601,
                Description = "Agriculture"
            };
            AssetCollateralRowItem item2 = new AssetCollateralRowItem
            {
                CollateralClassID = 602,
                Description = "Crops"
            };
            ObservableCollection<AssetCollateralRowItem> typeItems;
            typeItems = new ObservableCollection<AssetCollateralRowItem>();
            typeItems.Add(item1);
            typeItems.Add(item2);
            this.DataContext = new AssetCollateralAssignTypeViewModel();
            await (DataContext as AssetCollateralAssignTypeViewModel).GetListCollateralItems(typeItems);
        }
    }
}
