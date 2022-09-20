using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anno_World_Manager.model
{
    public enum SessionType
    {
        /// <summary>
        /// Corresponds to a vanilla session. This overwrites the template for a region implemented in the game.
        /// </summary>
        SessionVanilla,
        /// <summary>
        /// Corresponds to an additional session that can be sailed independently on the world map in addition to the sessions created in the vanilla game.
        /// </summary>
        SessionCustom,
        /// <summary>
        /// The user has not yet made a decision.
        /// </summary>
        NotSetYet,
    }
}
