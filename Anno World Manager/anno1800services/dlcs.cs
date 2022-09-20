using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Anno_World_Manager.anno1800services
{

    /*
        data0: General game files
        data1: Shaders used in the basegame
        data2: Icons and images used in the basegame
        data3: Video used in the basegame
        data4: Blacklisted words in every language and fonts for the game
        data5: Maps used in the basegame
        data6: Islands used in the basegame
        data7: Benchmarks
        data8: Buildings used in the basegame
        data9: Sounds used in the basegame
        data10: The Anarchist DLC
        data11: Sunken Treasures DLC
        data12: Botanica DLC
        data13: The Passage DLC
        data14: Seat Of Power DLC
        data15: Bright Harvest DLC
        data16: Land Of Lions DLC
        data17: Docklands DLC
        data18: Tourist Season DLC
        data19: The High Life DLC
        data20: Pedestrian Zone Pack
        data21: Eden Burning Scenario
        data22: Seeds of Change DLC
     * 
     */
    internal class dlcs
    {
        public bool HasDLCTheAnarchist { get; set; } = false;

        /// <summary>
        /// Local installation of DLC Sunken Treasures found
        /// </summary>
        /// <remarks>
        /// Is relevant for the editor, because this DLC contains island information
        /// </remarks>
        public bool HasDLCSunkenTreasures { get; set; } = false;
        public bool HasDLCThePassage{ get; set; } = false;
        public bool HasDLCTheLandOfLions { get; set; } = false;
        


       

        public void Initialize(string p_anno1800pathdata)
        {
            //  Check: The Anarchist exists
            if (File.Exists(Path.Combine(p_anno1800pathdata, "data10.rda"))) { HasDLCTheAnarchist = true; }
            //  Check: Sunken Treasures exists
            if (File.Exists(Path.Combine(p_anno1800pathdata, "data11.rda"))) { HasDLCSunkenTreasures = true; }
            //  Check: The Passage exists
            if (File.Exists(Path.Combine(p_anno1800pathdata, "data13.rda"))) { HasDLCThePassage = true; }
            //  Check: Lands of Lions exists
            if (File.Exists(Path.Combine(p_anno1800pathdata, "data16.rda"))) { HasDLCTheLandOfLions = true; }
            

        }
    }
}
