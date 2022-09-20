﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Anno_World_Manager.ImExPort
{
    internal abstract class BuilderXMLItem
    {
        public BuilderXMLItem(string name, int indentationDepth, bool trailingNewline = true)
        {
            Name = name;
            IndentDepth = indentationDepth;
            TrailingNewline = trailingNewline;
        }

        private bool TrailingNewline { get; }
        public string Name { get; }
        private int IndentDepth { get; }
        public int NextIndentDepth { get { return IndentDepth + 1; } }

        protected string OpenTag { get { return $"<{Name}>"; } }
        protected string CloseTag { get { return $"</{Name}>" + (TrailingNewline ? "\r\n" : string.Empty); } }
        protected string OpenCloseTag { get { return $"<{Name} />" + (TrailingNewline ? "\r\n" : string.Empty); } }

        public abstract string Open();

        public abstract string Close();

        protected string GetIndent()
        {
            return new string(' ', IndentDepth * 2);
        }

        public abstract void WriteToStream(StreamWriter stream);
    }

    internal class XMLSection : BuilderXMLItem
    {
        public XMLSection(string name, int indentationDepth, bool trailingNewline = true) : base(name, indentationDepth, trailingNewline)
        {
            Children = new List<BuilderXMLItem>();
        }

        private List<BuilderXMLItem> Children { get; }

        public override string Open()
        {
            return GetIndent() + OpenTag + "\r\n";
        }
        public override string Close()
        {
            return GetIndent() + CloseTag;
        }

        public XMLSection AddChildSection(XMLSection child)
        {
            Children.Add(child);
            return child;
        }

        public XMLSection AddChildSection(string name)
        {
            XMLSection item = new XMLSection(name, this.NextIndentDepth);
            Children.Add(item);
            return item;
        }

        public void AddChild(BuilderXMLItem child)
        {
            Children.Add(child);
        }

        public void AddValueChild(string name, string? value)
        {
            XMLValueEntry item = new XMLValueEntry(name, this.NextIndentDepth, value);
            Children.Add(item);
        }

        public void AddActionChild(string name, XMLEntryContent value)
        {
            XMLActionEntry item = new XMLActionEntry(name, this.NextIndentDepth, value);
            Children.Add(item);
        }

        public override void WriteToStream(StreamWriter stream)
        {
            stream.Write(Open());
            foreach (BuilderXMLItem child in Children)
            {
                child.WriteToStream(stream);
            }
            stream.Write(Close());
        }
    }

    internal class XMLValueEntry : BuilderXMLItem
    {
        public XMLValueEntry(string name, int indentationDepth, string? value) : base(name, indentationDepth)
        {
            Value = value;
        }

        private string? Value { get; }

        public override string Open()
        {
            return GetIndent() + OpenTag;
        }

        public override string Close()
        {
            return CloseTag;
        }

        public override void WriteToStream(StreamWriter stream)
        {
            if (Value == null)
            {
                stream.Write(GetIndent() + OpenCloseTag);
            }
            else
            {
                stream.Write(Open());
                stream.Write(Value);
                stream.Write(Close());
            }
        }
    }

    internal class XMLActionEntry : BuilderXMLItem
    {
        public XMLActionEntry(string name, int indentationDepth, XMLEntryContent value) : base(name, indentationDepth)
        {
            ValueMaker = value;
        }

        private XMLEntryContent ValueMaker { get; }

        public override string Open()
        {
            return GetIndent() + OpenTag;
        }

        public override string Close()
        {
            return CloseTag;
        }

        public override void WriteToStream(StreamWriter stream)
        {
            stream.Write(Open());
            ValueMaker.WriteDataToStream(stream);
            stream.Write(Close());
        }
    }

    internal class XMLEntryContent
    {
        public XMLEntryContent(Func<string> stringMaker) : this(stringMaker, 1)
        {

        }

        public XMLEntryContent(Func<string> stringMaker, int repetitions)
        {
            StringMaker = stringMaker;
            Repetitions = repetitions;
        }

        private Func<String> StringMaker { get; }
        private int Repetitions { get; }

        public void WriteDataToStream(StreamWriter stream)
        {
            for (int cnt = 0; cnt < Repetitions; cnt++)
            {
                stream.Write(StringMaker());
            }
        }
    }
}
