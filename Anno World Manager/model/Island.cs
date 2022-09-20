using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anno_World_Manager.Base;
using Newtonsoft.Json.Converters;

namespace Anno_World_Manager.model
{

    public class Island : ObservableBase
    {
        /// <summary>
        /// Island name
        /// </summary>
        /// <remarks>
        /// If it is an island from Anno 1800 (vanilla), then the Anno internal name.
        /// </remarks>
        /// <example>
        /// colony01_l_02_river_01
        /// </example>
        public string Name { get; set; } = String.Empty;
        /// <summary>
        /// A plausible and speakable name for the island for the user
        /// </summary>
        public string Label { get; set; } = String.Empty;
        /// <summary>
        /// List of regions in which this island is regularly used in Anno.
        /// </summary>
        public List<WorldRegion> Regions { get; set; } = new List<WorldRegion>();
        /// <summary>
        /// Rough categorization of the island size
        /// </summary>
        /// <remarks>
        /// The categorization is based on the classification as it is made internally in Anno 1800 itself. However, plausibility and usability for the end user is the top priority.
        /// </remarks>
        [JsonConverter(typeof(StringEnumConverter))]
        public IslandSize Size { get; set; }

        /// <summary>
        /// Dimension of the island on the X axis
        /// </summary>
        public int IslandSizeX { get; set; }
        /// <summary>
        /// Dimension of the island on the Y axis
        /// </summary>
        public int IslandSizeY { get; set; }
        /// <summary>
        /// Number of tiles consisting of water
        /// </summary>
        public int CountedWaterTiles { get; set; }
        /// <summary>
        /// Number of tiles with non-buildable landmass
        /// </summary>
        public int CountedNonBuildableTiles { get; set; }
        /// <summary>
        /// Number of tiles with buildable land mass
        /// </summary>
        public int CountedBuildableTiles { get; set; }


        public int SizeInTitle { get; set; }            //  TODO: Prio 1 - Entfernen. Wird ohnehin nicht genutzt.

        public int SizeMapInTitle { get; set; }         //  TODO: Prio 1 - Entferne diese Maßeinheit in der Software (z.B. bei Inselgröße)

        [JsonConverter(typeof(StringEnumConverter))]
        public IslandType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ThirdPartyType ThirdPartyType { get; set; } = ThirdPartyType.Unknown;

        /// <summary>
        /// Is it possible to settle the island?
        /// </summary>
        public IslandCanBeColonized CanBeColonized { get; set; } = IslandCanBeColonized.DontKnow;

        /// <summary>
        /// Are there rivers on the island?
        /// </summary>
        public bool HasRiver { get; set; }
        
        /// <summary>
        /// Whether the island is included in Anno 1800. (=Vanilla Island)
        /// </summary>
        public bool IsVanillaIsland { get; set; }

        /// <summary>
        /// When Vanilla Island: Path to .a7m
        /// </summary>
        public string VanillaMapPath { get; set; } = String.Empty;

        /// <summary>
        /// When Vanilla Island: Name of the opposite island (river free versus with rivers) [only if exists].
        /// </summary>
        public string VanillaOpositeRiveredIslandName { get; set; } = String.Empty;

        /// <summary>
        /// When Vanilla Island: Path to Island mapimage.png
        /// </summary>
        /// <example>
        /// data\dlc03\sessions\islands\pool\colony03_a02_03\_gamedata\colony03_a02_03\mapimage.png
        /// </example>
        public string VanillaImagePathMapimage { get; set; }

        /// <summary>
        /// When Vanilla Island: Path to Island activemapimage.png
        /// </summary>
        /// <example>
        /// data\sessions\islands\pool\colony01\colony01_l_02\_gamedata\colony01_l_02_river_01\activemapimage.png
        /// </example>
        public string VanillaImagePathActivemapimage { get; set; }

        /// <summary>
        /// When Vanilla Island: Path to Island gamemapimage.png
        /// </summary>
        /// <example>
        /// data\sessions\islands\pool\colony01\colony01_l_02\_gamedata\colony01_l_02_river_01\gamemapimage.png
        /// </example>
        public string VanillaImagePathGamemapimage { get; set; }

        /// <summary>
        /// When Vanilla Island: Is Pool Island ?
        /// </summary>
        public bool VanillaIsPool { get; set; }

        

        


        


        /*
         "name": "arctic_colony03_a02_03",
  "label": "",
  "regions": { "Arctic" },
  "size": "Small",
  "sizeintitle": "256",
  "sizemapintitle" : "256",
  "type": "Cliff",
  "mappath": "data/dlc03/sessions/islands/pool/colony03_a02_03/colony03_a02_03.a7m",
  "image": "data\dlc03\sessions\islands\pool\colony03_a02_03\_gamedata\colony03_a02_03\mapimage.png",
  "ispool": "false",
         */

    }



    public enum IslandType
    {
        Cliff,
        Decoration,
        ThirdParty,
        Normal,

        Unknown
    }

    public enum ThirdPartyType
    {

        Blake,  //  3rdparty02
        PirateHarlow, // 3rdparty03
        PirateLaFortune, // 3rdparty04
        Sarmento, // 3rdparty05
        Nate, // 3rdparty06
        Bleakworth, // 3rdparty07
        Kahina, // 3rdparty08
        Inuit, // 3rdparty09
        Ketama, // 3rdparty10

        None,
        Unknown,
    }

    public enum IslandSize
    {
        Small,
        Medium,
        Large
    }

    public enum IslandCanBeColonized
    {
        Yes,
        No,
        DontKnow,
    }
}
