using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anno_World_Manager.model
{
    internal class MapTemplates
    {
        /// <summary>
        /// 
        /// </summary>
        internal List<MapTemplate> KnownMapTemplates { get; set; } = new List<MapTemplate>();

        /// <summary>
        /// 
        /// </summary>
        internal void Initialize()
        {
            Log.Logger.Trace("called"); 

            var filename = AppContext.BaseDirectory + "maptemplates.json";
            using (System.IO.StreamReader file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                KnownMapTemplates = (List<MapTemplate>)serializer.Deserialize(file, typeof(List<MapTemplate>));
            }

            //  Fall-Back - If File is empty
            if (KnownMapTemplates == null) { KnownMapTemplates = new List<MapTemplate>(); }

            
        }

        /// <summary>
        /// Check whether there are more templates in Anno 1800 than were previously read in from the json.
        /// </summary>
        /// <remarks>
        /// Important: The check may only take place when the stream to the game data is completely available.
        /// </remarks>
        internal void CheckAgainstAnno1800Data()
        {
            IEnumerable<string> all_maptemplates = Runtime.Anno1800GameData.DataArchive.Find("**/*.a7tinfo");

            foreach (var maptemplate in all_maptemplates)
            {
                var check = KnownMapTemplates.Where(x => x.MapPath.Equals(maptemplate));

                switch (check.Count())
                {
                    case 0:
                        Log.Logger.Info("The following template exists in the game, but not in the dataset: {0}", maptemplate);
                        //  not found
                        break;
                    case 1: 
                        //  ok. 
                        break;
                    default:
                        //  This is unexpected. Because the template found in Anno 1800 exists several times in the database.
                        Log.Logger.Info("The following template exists in the game, but multiple times in the dataset: {0}", maptemplate);
                        break;
                }
            }

            /*
            
                Dictionary<string, Regex> templateGroups = new()
                {
                    ["DLCs"] = new(@"data\/(?!=sessions\/)([^\/]+)"),
                    ["Moderate"] = new(@"data\/sessions\/.+moderate"),
                    ["New World"] = new(@"data\/sessions\/.+colony01")
                };
            */
                //var mapTemplates = Settings.DataArchive.Find("**/*.a7tinfo");
            /*

                Maps = new()
                {
                    new MapGroup("Campaign", mapTemplates.Where(x => x.StartsWith(@"data/sessions/maps/campaign")), new(@"\/campaign_([^\/]+)\.")),
                    new MapGroup("Moderate, Archipelago", mapTemplates.Where(x => x.StartsWith(@"data/sessions/maps/pool/moderate/moderate_archipel")), new(@"\/([^\/]+)\.")),
                    new MapGroup("Moderate, Atoll", mapTemplates.Where(x => x.StartsWith(@"data/sessions/maps/pool/moderate/moderate_atoll")), new(@"\/([^\/]+)\.")),
                    new MapGroup("Moderate, Corners", mapTemplates.Where(x => x.StartsWith(@"data/sessions/maps/pool/moderate/moderate_corners")), new(@"\/([^\/]+)\.")),
                    new MapGroup("Moderate, Island Arc", mapTemplates.Where(x => x.StartsWith(@"data/sessions/maps/pool/moderate/moderate_islandarc")), new(@"\/([^\/]+)\.")),
                    new MapGroup("Moderate, Snowflake", mapTemplates.Where(x => x.StartsWith(@"data/sessions/maps/pool/moderate/moderate_snowflake")), new(@"\/([^\/]+)\.")),
                    new MapGroup("New World, Large", mapTemplates.Where(x => x.StartsWith(@"data/sessions/maps/pool/colony01/colony01_l_")), new(@"\/([^\/]+)\.")),
                    new MapGroup("New World, Medium", mapTemplates.Where(x => x.StartsWith(@"data/sessions/maps/pool/colony01/colony01_m_")), new(@"\/([^\/]+)\.")),
                    new MapGroup("New World, Small", mapTemplates.Where(x => x.StartsWith(@"data/sessions/maps/pool/colony01/colony01_s_")), new(@"\/([^\/]+)\.")),
                    new MapGroup("DLCs", mapTemplates.Where(x => !x.StartsWith(@"data/sessions/")), new(@"data\/([^\/]+)\/.+\/maps\/([^\/]+)"))
                    //new MapGroup("Moderate", mapTemplates.Where(x => x.StartsWith(@"data/sessions/maps/pool/moderate")), new(@"\/([^\/]+)\."))
                };
            */
        }
    }
}
