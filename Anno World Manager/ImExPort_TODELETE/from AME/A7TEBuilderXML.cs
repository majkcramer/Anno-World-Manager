﻿using System;
using System.Text;


namespace Anno_World_Manager.ImExPort
{
    internal class A7TEBuilderXML : BuilderXMLBase
    {
            public A7TEBuilderXML(int mapSize)
            {
                MapSize = mapSize;
            }

            private int MapSize { get; }

            public override BuilderXMLItem MakeXML()
            {
                XMLSection root = new XMLSection("AnnoEditorLevel", 0, false);
                root.AddValueChild("FileVersion", "3");

                var dimensions = root.AddChildSection("Dimensions");
                dimensions.AddValueChild("X", MapSize.ToString());
                dimensions.AddValueChild("Y", MapSize.ToString());

                var chunkSize = root.AddChildSection("ChunkSize");
                chunkSize.AddValueChild("X", "64");
                chunkSize.AddValueChild("Y", "64");

                var chunks = root.AddChildSection("Chunks");

                for (int column = 0; column < MapSize / 64; column++)
                {
                    var columnSection = chunks.AddChildSection("Column");

                    for (int chunkID = 0; chunkID < MapSize / 64; chunkID++)
                    {
                        columnSection.AddValueChild("Chunk", column + "x" + chunkID);
                    }

                }


                return root;
            }
        }
    }

