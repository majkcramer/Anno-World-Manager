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

namespace Anno_World_Manager.view
{
    /// <summary>
    /// Interaktionslogik für WorldMap.xaml
    /// </summary>
    public partial class WorldMap : UserControl
    {
        public WorldMap()
        {
            InitializeComponent();

            
            //  This is a quick Fix. Regular RenderTransform Rotatio will not set afterward the Grid Cell Alignments.
            continent_newworld.LayoutTransform = new RotateTransform(-90);
            continent_oldworld.LayoutTransform = new RotateTransform(90);
            continent_captrelawney.LayoutTransform = new RotateTransform(90);
            continent_enbesa.LayoutTransform = new RotateTransform(180);

            //  TODO: Prio 9 - A northstar would be nice to see on the map
            //background.UriSource = new Uri("pack://application:,,,/Images/selfcreated/worldmap_background.svg"); ;
        }
    }
}
