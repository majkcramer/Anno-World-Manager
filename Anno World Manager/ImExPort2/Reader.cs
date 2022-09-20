using FileDBReader.src.XmlRepresentation;
using FileDBReader;
using FileDBSerializing.ObjectSerializer;
using FileDBSerializing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Anno_World_Manager.model;
using Anno_World_Manager.ImExPort2.model;
using FluentResults;
using System.Diagnostics;

namespace Anno_World_Manager.ImExPort2
{
    internal class Reader
    {

        #region Read a7tinfo in various Variants

        public static async Task<Result<a7tinfoModel>> A7tinfoTemplateFromA7tinfoFileAsync(string filePath)
        {
            try
            {
                Log.Logger.Debug("Open a7tinfo File Stream from: {0}", filePath);
                var stream = File.OpenRead(filePath);
                if (stream == null)
                {
                    Log.Logger.Error("Stream == null from File {0}", filePath);
                    return Result.Fail(String.Empty);
                }
                return await A7tinfoModelFromA7tinfoAsync(stream, filePath);
            }
            catch (Exception ex)
            {
                Log.Logger.Error("While File Open {0} the following Exception {1} appeared", filePath, ex.Message);
                return Result.Fail(String.Empty);
            }
            
            
        }

        /// <summary>
        /// Read from Anno 1800 FileDB Stream a c# a7tinfo Model 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="internalPath"></param>
        /// <returns></returns>
        public static async Task<Result<a7tinfoModel>> A7tinfoModelFromA7tinfoAsync(Stream stream, String filepath)
        {
            Result<a7tinfoModel> doc = await ReadAsync<model.a7tinfoModel>(stream); 
            //return await FromTemplateDocument(doc, DetectRegionFromPath(internalPath));
            return doc;
        }

        public static async Task<Result<a7tinfoModel>> A7tinfoTemplateFromXmlAsync(string filePath)
        {
            Result<a7tinfoModel> doc = await ReadFromXmlAsync<model.a7tinfoModel>(File.OpenRead(filePath));
            //return await FromTemplateDocument(doc, DetectRegionFromPath(filePath));
            return doc;
        }
        
        public static async Task<Result<a7tinfoModel>> A7tinfoTemplateFromXmlAsync(Stream? stream, string internalPath)
        {
            Result<a7tinfoModel> doc = await ReadFromXmlAsync<ImExPort2.model.a7tinfoModel>(stream);
            //return await FromTemplateDocument(doc, DetectRegionFromPath(internalPath));
            return doc;
        }

        #endregion



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static async Task<Result<T>> ReadAsync<T>(Stream stream) where T : class, new()
        {
            if (stream is null)
            {
                String err_msg = "stream is null";
                Log.Logger.Debug(err_msg);
                return Result.Fail(err_msg);
            }

            try
            {
                Result<T> retval2 = await Task.Run(() =>
                {
                    try
                    {
                        //  Set Stream Position to begin
                        stream.Seek(0, SeekOrigin.Begin);
                        //  Read the compression version number from the byte stream
                        //  This may be important when writing back to a MapTemplate
                        FileDBDocumentVersion version = VersionDetector.GetCompressionVersion(stream);
                        Log.Logger.Debug("Stream contains Compression Version: {0}", version);
                        //  Set the position of the stream back to the beginning
                        stream.Seek(0, SeekOrigin.Begin);
                        //  Deserialize the Stream to <T>
                        T? retval = FileDBConvert.DeserializeObject<T>(
                            stream,
                            new FileDBSerializerOptions()
                            {
                                //  reequired
                                Version = version,
                                //  a good idea, if our T does not contain 
                                IgnoreMissingProperties = true
                            });
                        
                        if (retval == null) 
                        {
                            Log.Logger.Error("Could not deserialize Stream");
                            return Result.Fail(String.Empty);
                        }
                        return Result.Ok(retval); 
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Debug("Minor Exception while working with stream: {0}", ex);
                        return Result.Fail(String.Empty);
                    }
                }); 
                return retval2;
            }
            catch (Exception ex)
            {
                Log.Logger.Debug("Major Exception while working with stream: {0}", ex);
                return Result.Fail(String.Empty);
            }
        }

        /// <summary>
        /// Read a c# Model Class from XML Stream
        /// </summary>
        /// <typeparam name="T">Generic Type of c# Model</typeparam>
        /// <param name="stream">Stream of XML to read from</param>
        /// <returns>Fluent Result of generic C# Model Class</returns>
        private static async Task<Result<T>> ReadFromXmlAsync<T>(Stream stream) where T : class, new()
        {
            if (stream is null) { return Result.Fail("stream == null"); }
                
            return await Task.Run(() => {
                try
                {
                    // load xml
                    XmlDocument xmlDocument = new();
                    xmlDocument.Load(stream);

                    // convert to bytes
                    XmlDocument? interpreterDocument = helper.GetEmbeddedXmlDocument(helper.RessouceXMLTemplate_a7tinfo);  //  TODO: Prio 2 - Übergebe als Parameter welches XML Ressourcen Template bezogen werden soll
                    if (interpreterDocument is null) return Result.Fail("interpreter Document is null");
                    XmlDocument xmlWithBytes = new XmlExporter().Export(xmlDocument, new(interpreterDocument));

                    // convert to FileDB
                    XmlFileDbConverter<FileDBDocument_V1> converter = new();
                    IFileDBDocument doc = converter.ToFileDb(xmlWithBytes);

                    // construct deserialize into objects
                    FileDBDocumentDeserializer<T> deserializer = new(new FileDBSerializerOptions()
                    {
                        IgnoreMissingProperties = true
                    });

                    T? retval = deserializer.GetObjectStructureFromFileDBDocument(doc);

                    if (retval == null)
                    {
                        return Result.Fail("Deserialized Model == null");
                    }
                    return Result.Ok(retval);
                }
                catch (Exception ex)
                {
                    return Result.Fail(ex.Message);
                }
            });
        }

    }
}
