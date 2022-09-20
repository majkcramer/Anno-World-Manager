
using System;
using System.IO;
using System.Text;


namespace Anno_World_Manager.ImExPort
{
    internal abstract class BuilderXMLBase
    {
            public abstract BuilderXMLItem MakeXML();

            public void WriteXMLToFile(string filePath)
            {
                BuilderXMLItem item = MakeXML();

                using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    item.WriteToStream(sw);
                }
            }

            public void WriteXMLToStream(Stream target)
            {
                BuilderXMLItem item = MakeXML();

                using (StreamWriter sw = new StreamWriter(target, Encoding.UTF8, leaveOpen: true))
                {
                    item.WriteToStream(sw);
                }
            }
        }
    }

