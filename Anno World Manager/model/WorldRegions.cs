using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anno_World_Manager.model
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WorldRegion
    {
        OldWorld = 1,
        CapTrelawney = 2,
        NewWorld = 3,
        Enbesa = 4,
        Arctic = 5,
        CommunityCreated = 6,


        ScenarioTheAnarchist = 100,
        ScenarioEdenBurning = 101,



        NotSetYet = 9999,
    }
}
