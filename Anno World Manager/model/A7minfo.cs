using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anno_World_Manager.model
{
    internal class A7minfo
    {
        #region <MapSize>0001000000010000</MapSize>
        /// <summary>
        /// Represents the content of <MapSize>{MapSizeWidth|MapSizeHeight}</MapSize>
        /// </summary>
        /// <example>
        /// <MapSize>0001000000010000</MapSize>
        /// </summary>
        public int MapSizeWidth { get; set; } = 0;
        /// <summary>
        /// Represents the content of <MapSize>{MapSizeWidth|MapSizeHeight}</MapSize>
        /// </summary>
        /// <example>
        /// <MapSize>0001000000010000</MapSize>
        /// </example>
        public int MapSizeHeight { get; set; } = 0;
        #endregion
    }
}
