using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Anno_World_Manager.ImExPort.helper
{
    internal static class XmlHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodepath"></param>
        /// <param name="xmlDocument"></param>
        /// <param name="minimumExpectedCount"></param>
        /// <returns></returns>
        internal static bool CheckXMLNodeExist(String nodepath, ref XmlDocument xmlDocument, int minimumExpectedCount = 1)
        {
            bool retval = false;
            switch (minimumExpectedCount)
            {
                case 0:
                    //  Parameter does not make sense. Realy.
                    Log.Logger.Error("Dear developer, why do you want to check if a node exists and expect 0 results at the same time?");
                    break;
                case 1:
                    //  Check if there is minimum one XML Node
                    XmlNode? xmlNode = xmlDocument.SelectSingleNode(nodepath);
                    if (xmlNode == null)
                    {
                        Log.Logger.Debug("Could not find the node '{0}' in the XML document", nodepath);
                    }
                    else
                    { retval = true; }
                    break;
                default:
                    //  Check if there are > 1 XML Nodes
                    XmlNodeList? xmlNodes = xmlDocument.SelectNodes(nodepath);
                    if (xmlNodes == null)
                    {
                        Log.Logger.Debug("Could not find any nodes '{0}' in the XML document", nodepath);
                    }
                    else
                    {
                        if (xmlNodes.Count >= minimumExpectedCount)
                        {
                            retval = true;
                        }
                        else
                        {
                            Log.Logger.Debug("Could not find enought nodes '{0}' - expedted >= {1} | found = {2}", nodepath, minimumExpectedCount, xmlNodes.Count);
                        }
                    }
                    break;
            }
            return retval;
        }


        internal static Result<String> GetInnerXMLString(String nodePath, ref XmlDocument xmlDataDocument)
        {
            XmlNode? xmlNode = xmlDataDocument.SelectSingleNode(nodePath);
            if (xmlNode == null)
            {
                return Result.Fail(String.Empty);
            }
            return Result.Ok(xmlNode.InnerText);
        }
    }
}
