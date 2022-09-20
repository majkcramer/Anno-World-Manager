using Anno_World_Manager.ImExPort.helper;
using Anno_World_Manager.model;
using FluentResults;
using FileDBReader.src.XmlRepresentation;
using FileDBSerializing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Anno_World_Manager.ImExPort
{
    internal static class ReaderA7tinfo
    {
        private static String a7tinfoContentXPath = "//Content";
        private static String a7tinfoMapTemplateXPath = "//Content/MapTemplate";
        private static String a7tinfoMapSizePath = "//Content/MapTemplate/Size";
        private static String a7tinfoPlayableArea = "//Content/MapTemplate/PlayableArea";
        private static String a7tinfoRandomlyPlacedThirdParties = "//Content/MapTemplate/RandomlyPlacedThirdParties";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapPath"></param>
        /// <returns></returns>
        internal static Result<A7tinfo> ReadVanillaMap(String mapPath)
        {
            //  TODO: Prio 1 - Nicht hier überschreiben
            mapPath = "data/dlc10/scenario02/sessions/maps/scenario_02_colony_01/scenario_02_colony_01.a7tinfo";

            System.IO.Stream stream = Runtime.Anno1800GameData.DataArchive.OpenRead(mapPath);

            if (stream == null)
            {
                Log.Logger.Error("Could not load the following Vanilla MapTemplate: {0}", mapPath);
                return Result.Fail(String.Empty);
            }

            //  Set Stream Position to begin
            stream.Seek(0, SeekOrigin.Begin);
            //  Read the compression version number from the byte stream
            //  This may be important when writing back to a MapTemplate
            var version = VersionDetector.GetCompressionVersion(stream);
            //  Set the position of the stream back to the beginning
            stream.Seek(0, SeekOrigin.Begin);
            //  Initialize the decompressing read from the stream
            FileDBReader.src.XmlRepresentation.Reader xmlreader = new Reader();
            System.Xml.XmlDocument doc = xmlreader.Read(stream);

            return ParseXML(doc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapPath"></param>
        internal static void ReadFileMap(string mapPath)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private static Result<A7tinfo> ParseXML(XmlDocument xmlDocument)
        {
            //  predeclaration
            A7tinfo retval = new A7tinfo();
            Result<string> content_string = Result.Fail(String.Empty);

            //  First check if the XML structure has important nodes at correct position
            if (! XmlHelper.CheckXMLNodeExist(a7tinfoContentXPath, ref xmlDocument)) { return Result.Fail(String.Empty); };
            if (! XmlHelper.CheckXMLNodeExist(a7tinfoMapTemplateXPath, ref xmlDocument)) { return Result.Fail(String.Empty); }
            if (! XmlHelper.CheckXMLNodeExist(a7tinfoMapSizePath, ref xmlDocument)) { return Result.Fail(String.Empty); };

            #region MapSize
            //  Try to read HEX String
            content_string = XmlHelper.GetInnerXMLString(a7tinfoMapSizePath, ref xmlDocument);
            if (content_string.IsSuccess)
            {
                //  Try to decode HEX String.
                //  The hexadecimal string consists of two Int32 in big endian format
                //  Therefore, reformatting from big endian to little endian is explicitly set as a parameter.
                Result<(int, int)> mapSize = HexHelper.GetTwoInt32FromHex(content_string.Value, true);
                if (mapSize.IsSuccess)
                {
                    retval.MapSizeWidth = mapSize.Value.Item1;
                    retval.MapSizeHeight = mapSize.Value.Item2;
                }
            }
            #endregion

            #region PlayableArea
            //  Try to read Hex String
            content_string = XmlHelper.GetInnerXMLString(a7tinfoPlayableArea, ref xmlDocument);
            if (content_string.IsSuccess)
            {
                //  Try to decode HEX String.
                //  The hexadecimal string consists of four Int32 in big endian format
                //  Therefore, reformatting from big endian to little endian is explicitly set as a parameter.
                Result<(int, int, int, int)> playableArea = HexHelper.GetFourInt32FromHex(content_string.Value, true);
                if (playableArea.IsSuccess)
                {
                    retval.MapPlayableAreaX1 = playableArea.Value.Item1;
                    retval.MapPlayableAreaY1 = playableArea.Value.Item2;
                    retval.MapPlayableAreaX2 = playableArea.Value.Item3;
                    retval.MapPlayableAreaY2 = playableArea.Value.Item4;
                }
            }
            #endregion

            #region RandomlyPlacedThirdParties
            if (XmlHelper.CheckXMLNodeExist(a7tinfoRandomlyPlacedThirdParties, ref xmlDocument)) { retval.RandomlyPlacedThirdParties = true; };
            #endregion





            //
            retval.GuaranteeBasicUsability();

            return Result.Ok<A7tinfo>(retval);
        }


    }
}

