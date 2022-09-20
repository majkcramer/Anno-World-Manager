using Anno_World_Manager.ImExPort.helper;
using Anno_World_Manager.model;
using FluentResults;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Anno_World_Manager.ImExPort
{
    internal class ReaderGamedataXml
    {
        //  REMINDER: Die gamedata.data steckt in einer *.a7t

        private static String gamedataContentXPath = "//Content";
        private static String gamedataGameSessionManagerXPath = "//Content/GameSessionManager";
        private static String gamedataAreaIDsXPath = "//Content/GameSessionManager/AreaIDs";
        private static String gamedataAreaIDsxXPath = "//Content/GameSessionManager/AreaIDs/x";
        private static String gamedataAreaIDsyXPath = "//Content/GameSessionManager/AreaIDs/y";
        private static String gamedataAreaIDsvalPath = "//Content/GameSessionManager/AreaIDs/val";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapPath"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        internal static Result<A7tgamedata> ReadVanillaData(String mapPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read gamedata.xml from a local File
        /// </summary>
        /// <param name="filePath">Full Path and Filename to read the gamedata.xml from</param>
        /// <returns></returns>
        internal static Result<A7tgamedata> ReadFileData(string filePath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                //  Interesting: The XmlDocumet.LoadXml() Function has some Encoding Problems with FileDBReader decompressed outcome
                //  doc.LoadXml(filePath);   
                doc.Load(filePath);
                return ParseXML(doc, false);
            }
            catch (Exception ex)
            {
                Log.Logger.Debug(ex.Message);
                return Result.Fail(ex.Message);
            }
        }

        private static Result<A7tgamedata> ParseXML(XmlDocument xmlDocument, bool needsHexConversion = true)
        {
            //  predeclaration
            A7tgamedata retval = new A7tgamedata();
            Result<string> content_string = Result.Fail(String.Empty);

            //  First check if the XML structure has important nodes at correct position
            if (!XmlHelper.CheckXMLNodeExist(gamedataContentXPath, ref xmlDocument)) { return Result.Fail(String.Empty); };
            if (!XmlHelper.CheckXMLNodeExist(gamedataGameSessionManagerXPath, ref xmlDocument)) { return Result.Fail(String.Empty); }
            if (!XmlHelper.CheckXMLNodeExist(gamedataAreaIDsXPath, ref xmlDocument)) { return Result.Fail(String.Empty); };
            if (!XmlHelper.CheckXMLNodeExist(gamedataAreaIDsxXPath, ref xmlDocument)) { return Result.Fail(String.Empty); };
            if (!XmlHelper.CheckXMLNodeExist(gamedataAreaIDsyXPath, ref xmlDocument)) { return Result.Fail(String.Empty); };
            if (!XmlHelper.CheckXMLNodeExist(gamedataAreaIDsvalPath, ref xmlDocument)) { return Result.Fail(String.Empty); };

            #region Island SizeX
            //  Try to read Inner XML String
            content_string = XmlHelper.GetInnerXMLString(gamedataAreaIDsxXPath, ref xmlDocument);
            if (content_string.IsSuccess)
            {
                if (needsHexConversion)
                {
                    //  TODO: Prio 1 - Check functionality as soon new FileDBReader Version is integrated
                    //  Try to decode HEX String.
                    //  The hexadecimal string consists of one Int32 in big endian format
                    //  Therefore, reformatting from big endian to little endian is explicitly set as a parameter.
                    throw new NotImplementedException();
                    Result<Int16> islandSizeX = HexHelper.GetOneInt16FromHex(content_string.Value, true);
                    if (islandSizeX.IsSuccess)
                    {
                        retval.IslandSizeX = islandSizeX.Value;
                    }
                }
                else
                {
                    //  Seems to be a user manual exported File
                    Result<Int16> islandSizeX = StringHelper.ConvertStringToInt16(content_string.Value);
                    if (islandSizeX.IsSuccess)
                    {
                        retval.IslandSizeX = islandSizeX.Value;
                    }
                }
            }
            #endregion

            #region Island SizeY
            //  Try to read Inner XML String
            content_string = XmlHelper.GetInnerXMLString(gamedataAreaIDsyXPath, ref xmlDocument);
            if (content_string.IsSuccess)
            {
                if (needsHexConversion)
                {
                    //  TODO: Prio 1 - Check functionality as soon new FileDBReader Version is integrated
                    //  Try to decode HEX String.
                    //  The hexadecimal string consists of one Int32 in big endian format
                    //  Therefore, reformatting from big endian to little endian is explicitly set as a parameter.
                    throw new NotImplementedException();
                    Result<Int16> islandSizeY = HexHelper.GetOneInt16FromHex(content_string.Value, true);
                    if (islandSizeY.IsSuccess)
                    {
                        retval.IslandSizeY = islandSizeY.Value;
                    }
                }
                else
                {
                    //  Seems to be a user manual exported File
                    Result<Int16> islandSizeY = StringHelper.ConvertStringToInt16(content_string.Value);
                    if (islandSizeY.IsSuccess)
                    {
                        retval.IslandSizeY = islandSizeY.Value;
                    }
                }
            }
            #endregion

            #region Island Vals (Every Tile of the Island)
            //  Try to read Inner XML String
            content_string = XmlHelper.GetInnerXMLString(gamedataAreaIDsvalPath, ref xmlDocument);
            if (content_string.IsSuccess)
            {
                if (needsHexConversion)
                {
                    //  TODO: Prio 1 - Check functionality as soon new FileDBReader Version is integrated
                    //  Try to decode HEX String.
                    //  The hexadecimal string consists of a List of Int16 in big endian format
                    //  Therefore, reformatting from big endian to little endian is explicitly set as a parameter.
                    throw new NotImplementedException();
                    Result<List<Int16>> vals = HexHelper.GetListOfInt16FromHex(content_string.Value, true);
                    if (vals.IsSuccess)
                    {
                        retval.Tiles = vals.Value;
                    }
                }
                else
                {
                    //  Seems to be a user manual exported File
                    Result<List<Int16>> vals = StringHelper.ConvertStringToListOfInt16(content_string.Value);
                    if (vals.IsSuccess)
                    {
                        retval.Tiles = vals.Value;
                    }
                }
            }
            #endregion

            retval.Calculate();

            return Result.Ok<A7tgamedata>(retval);
        }
    }
}
