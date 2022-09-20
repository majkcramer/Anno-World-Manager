using FileDBReader;
using FileDBReader.src.XmlRepresentation;
using FileDBSerializing;

using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;

namespace Anno_World_Manager.view
{
    /// <summary>
    /// Interaktionslogik für Maps_Overview.xaml
    /// </summary>
    public partial class Maps_Overview : UserControl
    {
        public Maps_Overview()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //  TODO: Prio 2 - Move to ViewModel and Model

            System.IO.Stream stream = Runtime.Anno1800GameData.DataArchive.OpenRead("data/dlc10/scenario02/sessions/maps/scenario_02_colony_01/scenario_02_colony_01.a7tinfo");

            stream.Seek(0, SeekOrigin.Begin);
            var version = VersionDetector.GetCompressionVersion(stream);
            stream.Seek(0, SeekOrigin.Begin);

            FileDBReader.src.XmlRepresentation.Reader r = new Reader();
            System.Xml.XmlDocument doc = r.Read(stream);

            

            //XmlDocument xmlDocument

                //ConvertFile()


        }
    }
}
