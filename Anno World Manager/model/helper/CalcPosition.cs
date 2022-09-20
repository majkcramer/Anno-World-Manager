using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anno_World_Manager.model.helper
{
    internal static class CalcPosition
    {
        internal static int FlipYMapEntity(int sessionSizeY, int entitySizeY, int entityPositionY)
        {
            return sessionSizeY - entitySizeY - entityPositionY;
        }
    }
}
