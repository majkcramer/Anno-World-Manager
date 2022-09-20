using Anno_World_Manager.ImExPort;
using FluentResults;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Anno_World_Manager.model
{
    internal class Islands
    {
        private static readonly Random rnd = new((int)DateTime.Now.Ticks);


        /// <summary>
        /// 
        /// </summary>
        internal List<Island> KnownIslands { get; set; } = new List<Island>();


        /// <summary>
        /// 
        /// </summary>
        internal void Initialize()
        {
            Log.Logger.Trace("called");

            var filename = AppContext.BaseDirectory + "islands.json";
            using (System.IO.StreamReader file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                KnownIslands = (List<Island>)serializer.Deserialize(file, typeof(List<Island>));
            }

            //  Fall-Back - If File is empty
            if (KnownIslands == null) { KnownIslands = new List<Island>(); }
        }

        

        /// <summary>
        /// Check whether there are more islands in Anno 1800 than were previously read in from the json.
        /// </summary>
        /// <remarks>
        /// Important: The check may only take place when the stream to the game data is completely available.
        /// </remarks>
        internal void CheckAgainstAnno1800Data()
        {
            IEnumerable<string> all_islands_path = Runtime.Anno1800GameData.DataArchive.Find("**/*.a7m");

            foreach (String island_path in all_islands_path)
            {
                var check = KnownIslands.Where(x => x.VanillaMapPath.Equals(island_path));

                switch (check.Count())
                {
                    case 0:
                        //  not found
                        Log.Logger.Info("The following island exists in the game, but not in the dataset: {0}", island_path);
                        break;
                    case 1:
                        //  ok. 
                        break;
                    default:
                        //  This is unexpected. Because the template found in Anno 1800 exists several times in the database.
                        Log.Logger.Info("The following island exists in the game, but multiple times in the dataset: {0}", island_path);
                        break;
                }
            }
        }

        internal Result<Island> GetRandomIslandFromPool(WorldRegion region, IslandSize size)
        {
            IEnumerable<Island> potentialIslands = KnownIslands
                .Where(x => x.Regions.Contains(region))
                .Where(x => x.VanillaIsPool == true)
                .Where(x => x.Size == size);

            if (potentialIslands != null)
            {
                int c = potentialIslands.Count();

                if (c > 0)
                {
                    int index = rnd.Next(0, c - 1);
                    return Result.Ok(potentialIslands.ElementAt(index));
                }
                else
                {

                }
            }
            else
            {

            }
            return Result.Fail(String.Empty);
        }

        internal Result<Island> GetIsland(String mappath)
        {
            var potentialIsland = KnownIslands.Where(x => x.VanillaMapPath == mappath);
            if (potentialIsland != null)
            {
                int c = potentialIsland.Count();
                if (c == 1)
                {
                    return Result.Ok(potentialIsland.ElementAt(0));
                }
            }
            return Result.Fail(String.Empty);
        }

        internal void doMaybe_magic_data_dump_islands()
        {
            const String thirdPartyString = "3rdparty";

            List<Island> islands_to_serialize = new List<Island>();
            string foldername = AppContext.BaseDirectory + @"\" + Runtime.Secret_Magic_Data_Dump_Island_Folder;

            if (Directory.Exists(foldername))
            {
                //  Get every Island path
                IEnumerable<string> all_island_path = Runtime.Anno1800GameData.DataArchive.Find("**/*.a7m");

                //  TEST
                IEnumerable<string> test = Runtime.Anno1800GameData.DataArchive.Find("**/*colony01_l_02_river_01*.*");

                //  Iterate 
                foreach (String island_a7m_path in all_island_path)
                {
                    //  READ Island gamedata.dat (more precise: gamedata.xml)
                    int pos = island_a7m_path.LastIndexOf("/") + 1;
                    String island_name_filename = island_a7m_path.Substring(pos, island_a7m_path.Length - pos);
                    String island_name = island_name_filename.Replace(".a7m", "");

                    String local_gamedatxml = @"D:\code_AnnoWorldManager\systematic_a7m\data\" + island_name + @"\gamedata.xml";
                    var result_a7tgamedata = ReaderGamedataXml.ReadFileData(local_gamedatxml);  //  TODO: Von local gamedata.xml auf Stream umstellen
                    if (result_a7tgamedata.IsFailed)
                    {
                        Log.Logger.Error("Could not found gamedata.xml");
                    }

                    //  READ Island a7minfo
                    string island_a7minfo_path = island_a7m_path.Replace(".a7m", ".a7minfo");
                    var result = ReaderA7minfo.ReadVanillaMapInfo(island_a7minfo_path);

                    if (result.IsSuccess && result_a7tgamedata.IsSuccess)
                    {
                        A7minfo island_a7minfo = result.Value;
                        A7tgamedata island_gamedata = result_a7tgamedata.Value;

                        //  Export drawn_island_gamedata Island Titles
                        String fn_drawn_island_tiles = foldername + @"\i" + island_name + ".png";
                        var resultimage = island_gamedata.DrawTiles_RGB();
                        if (resultimage.IsSuccess)
                        {
                            System.Windows.Media.Imaging.PngBitmapEncoder encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                            Guid photoID = System.Guid.NewGuid();
                            encoder.Frames.Add(BitmapFrame.Create(resultimage.Value));

                            using (var filestream = new FileStream(fn_drawn_island_tiles, FileMode.Create))
                                encoder.Save(filestream);
                        }

                        //
                        Log.Logger.Debug("{0} Island Width: {1}", island_a7m_path, island_a7minfo.MapSizeWidth);
                        Log.Logger.Debug("{0} Island Height: {1}", island_a7m_path, island_a7minfo.MapSizeHeight);

                        //  Yes, the following is "interpretation". I dont have better informations yet.
                        //
                        Island island = new Island();

                        //  Name
                        island.Name = island_name;
                        //  Label
                        island.Label = String.Empty;
                        //  Regions
                        if (island_a7m_path.Contains("data/dlc01/")) { island.Regions.Add(WorldRegion.CapTrelawney); }
                        if (island_a7m_path.Contains("data/dlc03/")) { island.Regions.Add(WorldRegion.Arctic); }
                        if (island_a7m_path.Contains("data/dlc06/")) { island.Regions.Add(WorldRegion.Enbesa); }
                        if (island_a7m_path.Contains("data/dlc10/")) { island.Regions.Add(WorldRegion.ScenarioTheAnarchist); }
                        if (island_a7m_path.Contains("data/eyo21/")) { island.Regions.Add(WorldRegion.ScenarioEdenBurning); }
                        if (island_a7m_path.Contains("colony01")) { island.Regions.Add(WorldRegion.NewWorld); }
                        if (island.Regions.Count == 0) { island.Regions.Add(WorldRegion.OldWorld); }
                        //                        if (island_a7m_path.Contains("data/sessions/")) { ; island.Regions.Add(WorldRegion.NewWorld); }     //  Lässt sich anhand des filenames nicht unterscheiden

                        /* OLD: Size - anhand des Inselnamen
                        if (island_name.Contains("_l_")) { island.Size = IslandSize.Large; }
                        if (island_name.Contains("_a01_")) { island.Size = IslandSize.Large; } //   Arctic
                        if (island_name.Contains("_m_")) { island.Size = IslandSize.Medium; }
                        if (island_name.Contains("_a02_")) { island.Size = IslandSize.Medium; } //  Arctic
                        if (island_name.Contains("_s_")) { island.Size = IslandSize.Small; }
                        */

                        //  Identify Island Size by Tiles
                        //  Remark: In the vanilla dataset, the X and Y dimensions are always identical. Due to the square tile size of the islands, only one dimension is used here for size comparison.
                        switch (island_gamedata.IslandSizeX)
                        {
                            //  Small Island = 0 - 192 Tiles
                            case <= 192:
                                island.Size = IslandSize.Small;
                                break;
                            //  Medium Island = 193 - 319 Tiles
                            case <= 320:
                                island.Size = IslandSize.Medium;
                                break;
                            // Large Island = 320 - open upwards Tiles
                            default:
                                island.Size = IslandSize.Large;
                                break;
                        }

                        //  Island Size
                        island.IslandSizeX = island_gamedata.IslandSizeX;
                        island.IslandSizeY = island_gamedata.IslandSizeY;

                        //  Tiles
                        island.CountedWaterTiles = island_gamedata.CountedWaterTiles;
                        island.CountedNonBuildableTiles = island_gamedata.CountedNonBuildableTiles;
                        island.CountedBuildableTiles = island_gamedata.CountedBuildableTiles;

                        //  SizeMapInTitle
                        island.SizeMapInTitle = island_a7minfo.MapSizeWidth;
                        //  Type
                        island.Type = IslandType.Normal;
                        if (island.CountedBuildableTiles == 0) { island.Type = IslandType.Decoration; }
                        if (island_a7m_path.Contains(thirdPartyString)) { island.Type = IslandType.ThirdParty; }



                        //
                        if (island_name.Contains(thirdPartyString))
                        {
                            int thirdPartyString_position = island.Name.IndexOf(thirdPartyString);
                            int thirdPartyString_lenght = thirdPartyString.Length;
                            int thirdPartyToCut = thirdPartyString_position + thirdPartyString_lenght;
                            String thirdPartyNumber = island_name.Substring(thirdPartyToCut, 2);

                            switch (thirdPartyNumber)
                            {
                                case "02":
                                    island.ThirdPartyType = model.ThirdPartyType.Blake;
                                    break;
                                case "03":
                                    island.ThirdPartyType = model.ThirdPartyType.PirateHarlow;
                                    break;
                                case "04":
                                    island.ThirdPartyType = model.ThirdPartyType.PirateLaFortune;
                                    break;
                                case "05":
                                    island.ThirdPartyType = model.ThirdPartyType.Sarmento;
                                    break;
                                case "06":
                                    island.ThirdPartyType = model.ThirdPartyType.Nate;
                                    break;
                                case "07":
                                    island.ThirdPartyType = model.ThirdPartyType.Bleakworth;
                                    break;
                                case "08":
                                    island.ThirdPartyType = model.ThirdPartyType.Kahina;
                                    break;
                                case "09":
                                    island.ThirdPartyType = model.ThirdPartyType.Inuit;
                                    break;
                                case "10":
                                    island.ThirdPartyType = model.ThirdPartyType.Ketama;
                                    break;

                                default:
                                    island.ThirdPartyType = model.ThirdPartyType.Unknown;
                                    Log.Logger.Warn("Unknown Third Party found: {0}", thirdPartyNumber);
                                    break;
                            }
                        }
                        else
                        {
                            island.ThirdPartyType = ThirdPartyType.None;
                        }

                        //  CanBeColonized 
                        if (island.CountedBuildableTiles > 0)
                        {
                            island.CanBeColonized = IslandCanBeColonized.Yes;
                        }
                        else
                        {
                            island.CanBeColonized = IslandCanBeColonized.No;
                        }

                        //  MapPath
                        island.VanillaMapPath = island_a7m_path;
                        //  Image1
                        var imagepath = island_a7m_path.Substring(0, pos);
                        imagepath += "_gamedata/" + island_name + "/";
                        var image1 = imagepath + "mapimage.png";
                        var image2 = imagepath + "activemapimage.png";
                        var image_alpha = imagepath + "gamemapimage.png";
                        island.VanillaImagePathMapimage = image1.Replace("/", @"\");
                        island.VanillaImagePathActivemapimage = image2.Replace("/", @"\");
                        island.VanillaImagePathGamemapimage = image_alpha.Replace("/", @"\");
                        //  IsPool
                        if (island_a7m_path.Contains("/pool/")) { island.VanillaIsPool = true; } else { island.VanillaIsPool = false; }
                        //  HasRiver
                        if (island_a7m_path.Contains("_river_")) { island.HasRiver = true; } else { island.HasRiver = false; }
                        //  Oposite
                        if (island.HasRiver) { island.VanillaOpositeRiveredIslandName = island_a7m_path.Replace("_river_", ""); }



                        //  Dedicated special treatment of individual islands
                        switch (island_name)
                        {
                            case "campaign_colony01_atoll_01":
                                island.Label = "Large Atoll";
                                break;
                            case "campaign_colony01_burnt_island_01":
                                island.Label = "Eden Burning - La Xultuna";
                                break;
                            case "campaign_colony01_la_isla_01":
                                island.HasRiver = true;
                                break;
                            case "campaign_colony01_prosperity_01":
                                island.HasRiver = true;
                                break;
                            case "campaign_colony01_prologue":
                                island.HasRiver = true;
                                break;
                            case "campaign_moderate_player01_01":
                                island.HasRiver = true;
                                break;
                            case "colony01_l_01":
                                island.VanillaOpositeRiveredIslandName = "colony01_l_01_river_01";
                                break;
                            case "colony01_l_01_river_01":
                                island.VanillaOpositeRiveredIslandName = "colony01_l_01";
                                island.HasRiver = true;
                                break;
                            case "colony01_l_02":
                                island.VanillaOpositeRiveredIslandName = "colony01_l_02_river_01";
                                break;
                            case "colony01_l_02_river_01":
                                island.VanillaOpositeRiveredIslandName = "colony01_l_02";
                                island.HasRiver = true;
                                break;
                            case "colony01_l_03":
                                island.VanillaOpositeRiveredIslandName = "colony01_l_03_river_01";
                                break;
                            case "colony01_l_03_river_01":
                                island.VanillaOpositeRiveredIslandName = "colony01_l_03";
                                island.HasRiver = true;
                                break;
                            case "colony01_l_04":
                                island.VanillaOpositeRiveredIslandName = "colony01_l_04_river_01";
                                break;
                            case "colony01_l_04_river_01":
                                island.VanillaOpositeRiveredIslandName = "colony01_l_04";
                                island.HasRiver = true;
                                break;
                            case "colony01_l_05":
                                island.VanillaOpositeRiveredIslandName = "colony01_l_05_river_01";
                                break;
                            case "colony01_l_05_river_01":
                                island.VanillaOpositeRiveredIslandName = "colony01_l_05";
                                island.HasRiver = true;
                                break;
                            case "colony01_m_01":
                                island.VanillaOpositeRiveredIslandName = "colony01_m_01_river_01";
                                break;
                            case "colony01_m_01_river_01":
                                island.VanillaOpositeRiveredIslandName = "colony01_m_01";
                                island.HasRiver = true;
                                break;
                            case "colony01_m_02":
                                island.VanillaOpositeRiveredIslandName = "colony01_m_02_river_01";
                                break;
                            case "colony01_m_02_river_01":
                                island.VanillaOpositeRiveredIslandName = "colony01_m_02";
                                island.HasRiver = true;
                                break;
                            case "colony01_m_03":
                                island.VanillaOpositeRiveredIslandName = "colony01_m_03_river_01";
                                break;
                            case "colony01_m_03_river_01":
                                island.VanillaOpositeRiveredIslandName = "colony01_m_03";
                                island.HasRiver = true;
                                break;
                            case "colony01_m_04":
                                island.VanillaOpositeRiveredIslandName = "colony01_m_04_river_01";
                                break;
                            case "colony01_m_04_river_01":
                                island.VanillaOpositeRiveredIslandName = "colony01_m_04";
                                island.HasRiver = true;
                                break;
                            case "colony01_m_05":
                                island.VanillaOpositeRiveredIslandName = "colony01_m_05_river_01";
                                break;
                            case "colony01_m_05_river_01":
                                island.VanillaOpositeRiveredIslandName = "colony01_m_05";
                                island.HasRiver = true;
                                break;
                            case "colony01_m_06":
                                island.VanillaOpositeRiveredIslandName = "colony01_m_06_river_01";
                                break;
                            case "colony01_m_06_river_01":
                                island.VanillaOpositeRiveredIslandName = "colony01_m_06";
                                island.HasRiver = true;
                                break;
                            case "community_island":
                                island.VanillaOpositeRiveredIslandName = "community_island_river_01";
                                break;
                            case "community_island_river_01":
                                island.VanillaOpositeRiveredIslandName = "community_island";
                                island.HasRiver = true;
                                break;
                            case "moderate_l_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_01_river_01";
                                break;
                            case "moderate_l_01_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_01";
                                island.HasRiver = true;
                                break;
                            case "moderate_l_02":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_02_river_01";
                                break;
                            case "moderate_l_02_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_02";
                                island.HasRiver = true;
                                break;
                            case "moderate_l_03":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_03_river_01";
                                break;
                            case "moderate_l_03_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_03";
                                island.HasRiver = true;
                                break;
                            case "moderate_l_04":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_04_river_01";
                                break;
                            case "moderate_l_04_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_04";
                                island.HasRiver = true;
                                break;
                            case "moderate_l_05":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_05_river_01";
                                break;
                            case "moderate_l_05_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_05";
                                island.HasRiver = true;
                                break;
                            case "moderate_l_06":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_06_river_01";
                                break;
                            case "moderate_l_06_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_06";
                                island.HasRiver = true;
                                break;
                            case "moderate_l_07":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_07_river_01";
                                break;
                            case "moderate_l_07_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_07";
                                island.HasRiver = true;
                                break;
                            case "moderate_l_08":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_08_river_01";
                                break;
                            case "moderate_l_08_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_08";
                                island.HasRiver = true;
                                break;
                            case "moderate_l_09":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_09_river_01";
                                break;
                            case "moderate_l_09_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_09";
                                island.HasRiver = true;
                                break;
                            case "moderate_l_10":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_10_river_01";
                                break;
                            case "moderate_l_10_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_10";
                                island.HasRiver = true;
                                break;
                            case "moderate_l_11":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_11_river_01";
                                break;
                            case "moderate_l_11_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_11";
                                island.HasRiver = true;
                                break;
                            case "moderate_l_12":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_12_river_01";
                                break;
                            case "moderate_l_12_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_12";
                                island.HasRiver = true;
                                break;
                            case "moderate_l_13":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_13_river_01";
                                break;
                            case "moderate_l_13_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_13";
                                island.HasRiver = true;
                                break;
                            case "moderate_l_14":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_14_river_01";
                                break;
                            case "moderate_l_14_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_l_14";
                                island.HasRiver = true;
                                break;
                            case "moderate_m_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_01_river_01";
                                break;
                            case "moderate_m_01_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_01";
                                island.HasRiver = true;
                                break;
                            case "moderate_m_02":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_02_river_01";
                                break;
                            case "moderate_m_02_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_02";
                                island.HasRiver = true;
                                break;
                            case "moderate_m_03":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_03_river_01";
                                break;
                            case "moderate_m_03_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_03";
                                island.HasRiver = true;
                                break;
                            case "moderate_m_04":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_04_river_01";
                                break;
                            case "moderate_m_04_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_04";
                                island.HasRiver = true;
                                break;
                            case "moderate_m_05":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_05_river_01";
                                break;
                            case "moderate_m_05_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_05";
                                island.HasRiver = true;
                                break;
                            case "moderate_m_06":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_06_river_01";
                                break;
                            case "moderate_m_06_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_06";
                                island.HasRiver = true;
                                break;
                            case "moderate_m_07":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_07_river_01";
                                break;
                            case "moderate_m_07_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_07";
                                island.HasRiver = true;
                                break;
                            case "moderate_m_08":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_08_river_01";
                                break;
                            case "moderate_m_08_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_08";
                                island.HasRiver = true;
                                break;
                            case "moderate_m_09":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_09_river_01";
                                break;
                            case "moderate_m_09_river_01":
                                island.VanillaOpositeRiveredIslandName = "moderate_m_09";
                                island.HasRiver = true;
                                break;
                            case "colony02_storyisland_01":
                                island.Type = IslandType.Decoration;
                                break;
                            case "colony02_storyisland_02":
                                island.Type = IslandType.Decoration;
                                break;
                        }




                        // short check
                        var temp = new Pngs();
                        bool readable1 = temp.IsDataPathReadable(island.VanillaImagePathMapimage);
                        bool readable2 = temp.IsDataPathReadable(island.VanillaImagePathActivemapimage);
                        bool readable3 = temp.IsDataPathReadable(island.VanillaImagePathGamemapimage);

                        if (readable1 != true | readable2 != true | readable3 != true)
                        {

                        }

                        islands_to_serialize.Add(island);

                    }
                    else
                    {
                        Log.Logger.Debug("can't read {0}", island_a7minfo_path);
                    }
                }



                //  Write HTML Page

                string htmlfilename = foldername + @"\islands.html";

                using (FileStream fs = new FileStream(htmlfilename, FileMode.Create))
                {
                    using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                    {
                        w.WriteLine("<html><header></header><body>");
                        w.WriteLine("<table>");

                        foreach (var island in islands_to_serialize)
                        {
                            w.WriteLine("<tr>");


                            w.WriteLine("<td><table>");
                            w.WriteLine("<tr><td>Name:</td><td>" + island.Name + "</td></tr>");
                            w.WriteLine("<tr><td>Label:</td><td>" + island.Label + "</td></tr>");
                            List<string> island_regions = island.Regions.ConvertAll(f => f.ToString());
                            w.WriteLine("<tr><td>Regions:</td><td>" + String.Join(", ", island_regions) + "</td></tr>");
                            w.WriteLine("<tr><td>Size:</td><td>" + island.Size.ToString() + "</td></tr>");
                            w.WriteLine("<tr><td>IslandSizeX :</td><td>" + island.IslandSizeX.ToString() + "</td></tr>");
                            w.WriteLine("<tr><td>IslandSizeY :</td><td>" + island.IslandSizeY.ToString() + "</td></tr>");
                            w.WriteLine("<tr><td>CountedWaterTiles  :</td><td>" + island.CountedWaterTiles.ToString() + "</td></tr>");
                            w.WriteLine("<tr><td>CountedNonBuildableTiles  :</td><td>" + island.CountedNonBuildableTiles.ToString() + "</td></tr>");
                            w.WriteLine("<tr><td>CountedBuildableTiles  :</td><td>" + island.CountedBuildableTiles.ToString() + "</td></tr>");

                            w.WriteLine("<tr><td>SizeInTitle:</td><td>" + island.SizeInTitle + "</td></tr>");
                            w.WriteLine("<tr><td>SizeMapInTitle:</td><td>" + island.SizeMapInTitle + "</td></tr>");

                            w.WriteLine("<tr><td>Type:</td><td>" + island.Type.ToString() + "</td></tr>");
                            w.WriteLine("<tr><td>ThirdPartyType:</td><td>" + island.ThirdPartyType.ToString() + "</td></tr>");
                            w.WriteLine("<tr><td>CanBeColonized:</td><td>" + island.CanBeColonized.ToString() + "</td></tr>");

                            w.WriteLine("<tr><td>MapPath:</td><td>" + island.VanillaMapPath + "</td></tr>");
                            w.WriteLine("<tr><td>Image1:</td><td>" + island.VanillaImagePathMapimage + "</td></tr>");
                            w.WriteLine("<tr><td>Image2:</td><td>" + island.VanillaImagePathActivemapimage + "</td></tr>");
                            w.WriteLine("<tr><td>ImageAlpha:</td><td>" + island.VanillaImagePathGamemapimage + "</td></tr>");
                            w.WriteLine("<tr><td>IsPool:</td><td>" + island.VanillaIsPool + "</td></tr>");
                            w.WriteLine("<tr><td>HasRiver:</td><td>" + island.HasRiver + "</td></tr>");
                            w.WriteLine("<tr><td>OpositeRiveredIslandMapPath:</td><td>" + island.VanillaOpositeRiveredIslandName + "</td></tr>");

                            w.WriteLine("</table></td>");

                            var local_png_folder = @"..\" + Runtime.Secret_Magic_Data_Dump_Png_Folder + @"\";
                            var local_image1_filename = local_png_folder + island.VanillaImagePathMapimage.Replace("\\", "%23") + ".png";
                            var local_image2_filename = local_png_folder + island.VanillaImagePathActivemapimage.Replace("\\", "%23") + ".png";
                            var local_imageAlpha_filename = local_png_folder + island.VanillaImagePathGamemapimage.Replace("\\", "%23") + ".png";
                            String fn_drawn_island_tiles = foldername + @"\i" + island.Name + ".png";

                            w.WriteLine("<td>");
                            w.WriteLine("<b>" + island.Name + "</br>");
                            w.WriteLine("<img src=\"" + fn_drawn_island_tiles + "\">");
                            w.WriteLine("</td>");

                            w.WriteLine("<td>");
                            w.WriteLine("<b>" + island.Name + "</br>");
                            w.WriteLine("<img src=\"" + local_image1_filename + "\">");
                            w.WriteLine("</td>");

                            w.WriteLine("<td>");
                            w.WriteLine("<b>" + island.Name + "</br>");
                            w.WriteLine("<img src=\"" + local_image2_filename + "\">");
                            w.WriteLine("</td>");

                            w.WriteLine("<td>");
                            w.WriteLine("<b>" + island.Name + "</br>");
                            w.WriteLine("<img src=\"" + local_imageAlpha_filename + "\">");
                            w.WriteLine("</td>");

                            w.WriteLine("</tr>");
                        }

                        w.WriteLine("</table>");
                        w.WriteLine("</body></html>");

                        w.Flush();
                    }
                }

                //  Write JSON 

                string jsonfilename = foldername + @"\islands.json";
                using (StreamWriter file = File.CreateText(jsonfilename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, islands_to_serialize);
                }

            }
        }
    }
}
