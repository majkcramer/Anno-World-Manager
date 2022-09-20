using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anno_World_Manager.model
{
    internal class MapTemplate
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsVanilla { get; set; }

        public WorldRegion Region { get; set; }

        public string MapPath { get; set; }



        /// <summary>
        /// Ctor - Set Propertys with default values
        /// </summary>
        internal MapTemplate()
        {
            Name = String.Empty;
            Description = String.Empty;
            IsVanilla = false;
            Region = WorldRegion.OldWorld;
            MapPath = String.Empty;
        }
    }
}
