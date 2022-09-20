using Anno_World_Manager.ImExPort.helper;
using Anno_World_Manager.model;
using FileDBReader.src.XmlRepresentation;
using FileDBSerializing;
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
    internal static class ReaderA7minfo
    {

        //  ----------- Important: If you are looking for a better example take a look at 'RederA7tinfo.cs' -----------------------
























        private static String a7minfoContentXPath = "//Content";
        private static String a7minfoMapSize = "//Content/MapSize";









        internal static Result<A7minfo> ReadVanillaMapInfo(String mapPath)
        {
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

            try
            {
                System.Xml.XmlDocument doc = xmlreader.Read(stream);
                return ParseXML(doc);
            }
            catch (Exception ex)
            {
                Log.Logger.Debug(ex.Message);
            }

            return Result.Fail(String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        private static Result<A7minfo> ParseXML(XmlDocument xmlDocument)
        {
            //  predeclaration
            A7minfo retval = new A7minfo();
            Result<string> content_string = Result.Fail(String.Empty);

            //  First check if the XML structure has important nodes at correct position
            if (!XmlHelper.CheckXMLNodeExist(a7minfoContentXPath, ref xmlDocument)) { return Result.Fail(String.Empty); }
            if (!XmlHelper.CheckXMLNodeExist(a7minfoMapSize, ref xmlDocument)) { return Result.Fail(String.Empty); }
            
            #region MapSize
            //  Try to read HEX String
            content_string = XmlHelper.GetInnerXMLString(a7minfoMapSize, ref xmlDocument);
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

            return Result.Ok<A7minfo>(retval);
        }
    }
}
