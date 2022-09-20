using Anno_World_Manager.helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Anno_World_Manager
{
    internal static class Runtime
    {
        /// <summary>
        /// Local Directory Informations about Anno 1800
        /// </summary>
        internal static anno1800services.directorys Anno1800LocalPath = new anno1800services.directorys();

        /// <summary>
        /// Check which DLCs are installed locally on this PC
        /// </summary>
        internal static anno1800services.dlcs Anno1800Dlcs = new anno1800services.dlcs();
            
        /// <summary>
        /// Anno 1800 internal Game Data as IO Stream
        /// </summary>
        internal static anno1800services.datastructure Anno1800GameData = new anno1800services.datastructure();

        /// <summary>
        /// List of all known islands
        /// </summary>
        /// <remarks>
        /// The module can handle native islands from Anno 1800, as well as any islands created by the community.
        /// </remarks>
        internal static model.Islands IslandsKnown = new model.Islands();

        /// <summary>
        /// List of all known Map Templates
        /// </summary>
        /// <remarks>
        /// This module can handle Anno1800 vanilla Map Templates, as well as Map Templates created by the community.
        /// </remarks>
        internal static model.MapTemplates MapTemplatesKnows = new model.MapTemplates();

        /// <summary>
        /// List of all Sessions wihin Anno World Manager
        /// </summary>
        internal static model.Sessions SessionsKnown = new model.Sessions();


        internal static model.Pngs Pngs = new model.Pngs();


        #region Secret Magic Data Dump Folder
        /// <summary>
        /// If this subdirectory exists, then all found pngs will be extracted to this directory.
        /// Why? Exciting for research purposes.
        /// </summary>
        internal static String Secret_Magic_Data_Dump_Png_Folder = @"tmp_png_dump\";


        internal static String Secret_Magic_Data_Dump_Island_Folder = @"tmp_island_dump\";


        internal static String Secret_Magic_Data_Dump_Template_Folder = @"tmp_template_dump\";

        #endregion


        /// <summary>
        /// 
        /// </summary>
        internal static void Initialize()
        {

            //  TEMPORÄR: Einlesen einer "gamedata.xml"
            string filename = "D:\\code_AnnoWorldManager\\tmp_delete\\gamedata.xml";
            FluentResults.Result<model.A7tgamedata> test = ImExPort.ReaderGamedataXml.ReadFileData(filename);
            if (test.IsSuccess)
            {
                model.A7tgamedata dummy_island = test.Value;
                var resultimage = dummy_island.DrawTiles_RGB();
                if (resultimage.IsSuccess)
                {
                    System.Windows.Media.Imaging.PngBitmapEncoder encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                    Guid photoID = System.Guid.NewGuid();
                    encoder.Frames.Add(BitmapFrame.Create(resultimage.Value));

                    using (var filestream = new FileStream("D:\\code_AnnoWorldManager\\tmp_delete\\generated.png", FileMode.Create))
                        encoder.Save(filestream);
                }
            }
        


            //  Discover Anno1800 local Game Folders
            Anno1800LocalPath.Initialize();

            //  Discover Anno1800 installed Dlc(s)
            Anno1800Dlcs.Initialize(Anno1800LocalPath.path_data);

            //  Open Anno 1800 Game Data as Stream
            Anno1800GameData.LoadDataPath(Anno1800LocalPath.path_install);

            //  Load Island Repository
            IslandsKnown.Initialize();

            //  Load MapTemplates Repository
            MapTemplatesKnows.Initialize();

            //  Load Session Repository
            SessionsKnown.Initialize();

            Pngs.Initialize();
            

            //  TEST - 
            while (Anno1800GameData.IsLoading == true)
            {
                Task.Delay(500);
            }

            DelayFactory.DelayAction(500, new Action(() => { PngTest(); }));

            

        }

        /// <summary>
        /// 
        /// </summary>
        internal static void CheckAfterInitializationCompletet()
        {
            MapTemplatesKnows.CheckAgainstAnno1800Data();

            IslandsKnown.CheckAgainstAnno1800Data();

            //  Maybe: If the magic directory exists, save data for research purposes to extend the application.
            IslandsKnown.doMaybe_magic_data_dump_islands();
            Pngs.doMaybe_magic_data_dump_pngs();

            
        }

        private static void PngTest()
        {
            //  data/sessions/islands/campaign/colony01/chapter03/campaign_colony01_burnt_island_01/_gamedata/campaign_colony01_burnt_island_01/mapimage.png

            System.Windows.Media.Imaging.BitmapImage? png = new();
            try
            {
                //using Stream? stream = Settings.Instance.DataArchive?.OpenRead(island.ImageFile);
                using Stream stream = Anno1800GameData.DataArchive.OpenRead(@"data\dlc01\sessions\islands\pool\moderate_c_01\_gamedata\moderate_c_01\mapimage.png");
                if (stream is not null)
                {
                    png.BeginInit();
                    png.StreamSource = stream;
                    png.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    png.EndInit();
                    png.Freeze();
                }
            }
            catch
            {
                png = null;
            }
        }

       
    }
}
