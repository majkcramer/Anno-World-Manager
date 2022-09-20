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
    /// Interaktionslogik für Navigation.xaml
    /// </summary>
    public partial class Navigation : UserControl
    {
        public Navigation()
        {
            InitializeComponent();

            this.hamburgermenue_icon.UriSource = new Uri("pack://application:,,,/Images/ionic.io/menu-outline.svg"); ;
            this.hamburgermenue_icon.Width = 16;
            this.hamburgermenue_icon.Height = 16;

            //  Source="/Anno_World_Manager;component/Images/menu-outline.svg"

        }
    }
}
