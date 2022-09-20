using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anno_World_Manager.model
{
    /// <summary>
    /// Internal class to map the content of an A7tinfo.
    /// </summary>
    public class A7tinfo
    {
        private const int mapSizeWidthMin = 200;
        private const int mapSizeHeightMin = 200;

        public bool IsReadyToBeUsed { get; set; } = false;

        #region <MapSize>
        /// <summary>
        /// Represents the content of <MapSize>{MapSizeWidth|MapSizeHeight}</MapSize>
        /// </summary>
        /// <example>
        /// <MapSize>0001000000010000</MapSize>
        /// </example>
        public int MapSizeWidth { get; set; } = 0;
        /// <summary>
        /// Represents the content of <MapSize>{MapSizeWidth|MapSizeHeight}</MapSize>
        /// </summary>
        /// <example>
        /// <MapSize>0001000000010000</MapSize>
        /// </example>
        public int MapSizeHeight { get; set; } = 0;
        #endregion

        #region <PlayableArea>
        /// <summary>
        /// Represents the content of <PlayableArea>{MapPlayableAreaX1|MapPlayableAreaY1|MapPlayableAreaX2|MapPlayableAreaX2}</PlayableArea>
        /// </summary>
        /// <example>
        /// <PlayableArea>00010000000100000005000000050000</PlayableArea>
        /// </example>
        public int MapPlayableAreaX1 { get; set; } = 0;
        /// <summary>
        /// Represents the content of <PlayableArea>{MapPlayableAreaX1|MapPlayableAreaY1|MapPlayableAreaX2|MapPlayableAreaX2}</PlayableArea>
        /// </summary>
        /// <example>
        /// <PlayableArea>00010000000100000005000000050000</PlayableArea>
        /// </example>
        public int MapPlayableAreaY1 { get; set; } = 0;
        /// <summary>
        /// Represents the content of <PlayableArea>{MapPlayableAreaX1|MapPlayableAreaY1|MapPlayableAreaX2|MapPlayableAreaX2}</PlayableArea>
        /// </summary>
        /// <example>
        /// <PlayableArea>00010000000100000005000000050000</PlayableArea>
        /// </example>
        public int MapPlayableAreaX2 { get; set; } = 0;
        /// <summary>
        /// Represents the content of <PlayableArea>{MapPlayableAreaX1|MapPlayableAreaY1|MapPlayableAreaX2|MapPlayableAreaX2}</PlayableArea>
        /// </summary>
        /// <example>
        /// <PlayableArea>00010000000100000005000000050000</PlayableArea>
        /// </example>
        public int MapPlayableAreaY2 { get; set; } = 0;
        #endregion

        public bool RandomlyPlacedThirdParties { get; set; } = false;

        /// <summary>
        /// Guarantees basic usability of the map. 
        /// </summary>
        internal void GuaranteeBasicUsability()
        {
            //  MapSize
            if (MapSizeWidth < mapSizeWidthMin) { MapSizeWidth = mapSizeWidthMin; }
            if (MapSizeHeight < mapSizeHeightMin) { MapSizeHeight = mapSizeHeightMin; }

            // TODO: Implement Playable Area Guarantee

            IsReadyToBeUsed = true;
        }
    }
}
