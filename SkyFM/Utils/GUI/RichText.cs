using System;
using System.Windows.Controls;
using System.Xml;
using System.IO;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace SkyFM
{
	internal class RichTextReader
	{
		readonly XmlReader m_reader;

		public RichTextReader( string xml )
		{
			m_reader = XmlReader.Create( new StringReader( xml ), new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment } );
		}

		public bool Read()
		{
			if( !m_reader.Read() )
				return false;
			this.m_name = null;
			switch( m_reader.NodeType )
			{
				case XmlNodeType.Element:
				case XmlNodeType.EndElement:
				case XmlNodeType.Attribute:
					this.m_name = m_reader.Name.ToLower();
					break;
			}
			return true;
		}

		public bool ReadInside( string name )
		{
			if( !this.Read() )
				return false;
			if( m_reader.NodeType == XmlNodeType.EndElement && m_name == name )
				return false;
			return true;
		}

		string m_name;
		public string Name { get { return m_name; } }
		public string Value { get { return m_reader.Value; } }

		public bool isStartElement( string name )
		{
			return m_reader.NodeType == XmlNodeType.Element && m_name == name;
		}

		public bool MoveToAttribute(string name)
		{
			return m_reader.MoveToAttribute( name );
		}

		public bool isText() { return m_reader.NodeType == XmlNodeType.Text; }
	}

	internal static class RichText
	{
		internal static bool addText( this InlineCollection inlines, RichTextReader reader )
		{
			if( !reader.isText() )
				return false;
			var res = new Run() { Text = reader.Value };
			inlines.Add( res );
			return true;
		}

		internal static Hyperlink readLink( this RichTextReader reader )
		{
			if( !reader.isStartElement( "a" ) )
				throw new Exception();
			if( !reader.MoveToAttribute( "href" ) )
				throw new Exception( "No link target" );

			var res = new Hyperlink()
			{
				Foreground = App.Current.Resources[ "PhoneAccentBrush" ] as SolidColorBrush,
				TargetName = "_blank",
				NavigateUri = new Uri( reader.Value, UriKind.Absolute ),
			};

			while( reader.ReadInside( "a" ) )
				res.Inlines.addText( reader );
			
			return res;
		}

		internal static Paragraph readParagraph( this RichTextReader reader )
		{
			if( !reader.isStartElement( "p" ) )
				throw new Exception();

			Paragraph res = new Paragraph();
			if( reader.MoveToAttribute( "align" ) )
				res.TextAlignment = reader.Value.EnumParseOrDefault( TextAlignment.Justify );

			while( reader.ReadInside( "p" ) )
			{
				res.Inlines.addText( reader );
				if( reader.isStartElement("a") )
					res.Inlines.Add( reader.readLink() );
			}
			return res;
		}

		// Sample input: "<p align='left'><a href='http://www.sky.fm/'>Sky.fm</a>is kool</p>"
		public static void setRichText( this RichTextBox tb, string xml )
		{
			tb.Blocks.Clear();
			RichTextReader reader = new RichTextReader( xml );

			while( reader.Read() )
			{
				if( reader.isStartElement( "p" ) )
					tb.Blocks.Add( reader.readParagraph() );
			}
		}
	}
}