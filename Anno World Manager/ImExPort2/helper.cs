using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Anno_World_Manager.ImExPort2
{
    internal static class helper
    {
        internal static String RessouceXMLTemplate_a7tinfo = "Anno_World_Manager.ImExPort2.templates.a7tinfo";


        //  TODO: Rewirte for Result<>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        internal static XmlDocument? GetEmbeddedXmlDocument(string resourceName)
        {
            Assembly me = Assembly.GetExecutingAssembly();
            using (var resource = me.GetManifestResourceStream(resourceName))
            {
                if (resource is not null)
                {
                    XmlDocument doc = new();
                    doc.Load(resource);
                    return doc;
                }
            }
            return null;
        }
    }
}
