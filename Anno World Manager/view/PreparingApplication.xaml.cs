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
using SVGImage.SVG;

namespace Anno_World_Manager.view
{
    /// <summary>
    /// Interaktionslogik für PreparingApplication.xaml
    /// </summary>
    public partial class PreparingApplication : UserControl
    {
        public PreparingApplication()
        {
            InitializeComponent();

            this.icon_heart0.UriSource = new Uri("pack://application:,,,/Images/ionic.io/heart.svg"); ;
            this.icon_heart0.Width = 16;
            this.icon_heart0.Height = 16;

            CopyPropertys(icon_heart0, icon_heart1);
            CopyPropertys(icon_heart0, icon_heart2);
            CopyPropertys(icon_heart0, icon_heart3);
            CopyPropertys(icon_heart0, icon_heart4);
            CopyPropertys(icon_heart0, icon_heart5);
            //CopyPropertys(icon_heart0, icon_heart6);

            
        }

        private void CopyPropertys(SVGImage.SVG.SVGImage p_from, SVGImage.SVG.SVGImage p_to)
        {
            p_to.UriSource = p_from.UriSource;
            p_to.Width = p_from.Width;
            p_to.Height = p_from.Height;
        }
    }
}
