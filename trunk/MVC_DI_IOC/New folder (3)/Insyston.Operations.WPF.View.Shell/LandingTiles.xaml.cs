using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Insyston.Operations.WPF.ViewModels.Common.Model;

namespace Insyston.Operations.WPF.Views.Shell
{
    /// <summary>
    /// Interaction logic for Landing.xaml
    /// </summary>
    public partial class LandingTiles : UserControl
    {
        public LandingTiles()
        {
            this.InitializeComponent();
        }

        private void ModulesRadTileList_AutoGeneratingTile(object sender, Telerik.Windows.Controls.AutoGeneratingTileEventArgs e)
        {
            //this.CurrentModule.Children
            ExplorerItem item = e.Tile.Content as ExplorerItem;
            e.Tile.IsEnabled = item.IsEnabled;
            e.Tile.TileType = item.TileType;
            // e.Tile.Group = item.TileGroup;
            e.Tile.Background = new SolidColorBrush(item.Colour);
        }
    }
}
