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

namespace Anno_World_Manager.ImExPort2
{
    internal class Writer
    {
        public static async Task WriteAsync(object data, Stream? stream)
        {
            if (stream is null)
                return;

            await Task.Run(() =>
            {
                try
                {
                    FileDBConvert.SerializeObject(data, new() { Version = FileDBDocumentVersion.Version1 }, stream);
                }
                catch
                {
                }
            });
        }

        public static async Task WriteToXmlAsync(object data, Stream? stream)
        {
            if (stream is null)
                return;

            await Task.Run(() =>
            {
                try
                {
                    FileDBDocumentSerializer serializer = new(new FileDBSerializerOptions());
                    IFileDBDocument doc = serializer.WriteObjectStructureToFileDBDocument(data);

                    // convert to xml with bytes
                    FileDbXmlConverter converter = new();
                    XmlDocument xmlWithBytes = converter.ToXml(doc);

                    // interpret bytes
                    XmlDocument? interpreterDocument = helper.GetEmbeddedXmlDocument(helper.RessouceXMLTemplate_a7tinfo);  //  TODO: Prio 2 - Übergebe als Parameter welches XML Ressourcen Template bezogen werden soll
                    if (interpreterDocument is null)
                        return;
                    XmlDocument xmlDocument = new XmlInterpreter().Interpret(xmlWithBytes, new(interpreterDocument));

                    xmlDocument.Save(stream);
                }
                catch
                {
                }
            });
        }
    }
}
