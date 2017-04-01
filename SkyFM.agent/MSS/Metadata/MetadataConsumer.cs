using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace SkyFM.agent.MSS
{
	/// <summary>This class consumes the metadata as it arrives from the stream, parses it, and pushes to iAudioTrack.</summary>
	class MetadataConsumer: iMetadataConsumer
	{
		static readonly Regex s_reTitle = new Regex( @"StreamTitle=['""](.*?)['""]", RegexOptions.IgnoreCase | RegexOptions.Compiled );
		readonly iAudioTrack m_track;

		public MetadataConsumer( iAudioTrack track )
		{
			m_track = track;
		}

		void iMetadataConsumer.onMetadata( byte[] metadata, int length )
		{
			// Truncate training zeros
			while( true )
			{
				if( length <= 0 )
					return;
				if( 0 != metadata[ length - 1 ] )
					break;
				length--;
			}

			// Construct the string. Lets hope the encoding is UTF8, the shoutcast spec says nothing 'bout it.
			string strMetadata = Encoding.UTF8.GetString( metadata, 0, length );

			var m = s_reTitle.Match( strMetadata );
			if( !m.Success )
			{
				Logger.warning( "StreamTitle not found in {0}", strMetadata );
				return;
			}

			string newTitleComplete = m.Groups[ 1 ].Value;

			string artist, title;
			parseArtistTitle( newTitleComplete, out artist, out title );

			Logger.info( "title = {0}, artist={1}", title, artist );

			m_track.updateMetadata( title, artist );
		}

		static readonly char[] s_dashes = new char[ 3 ] { '-', '–', '—' };
		static readonly Regex s_reDashAndSpaces = new Regex( " [-–—] ", RegexOptions.Compiled );

		static void cutByDash( string complete, int dashPos, out string artist, out string title )
		{
			artist = complete
				.Substring( 0, dashPos )
				.Trim();
			title = complete
				.Substring( dashPos + 1 )
				.Trim();
		}

		static bool parseArtistTitle( string complete, out string artist, out string title )
		{
			title = complete;
			artist = null;

			int idxFirstDash = complete.IndexOfAny( s_dashes );
			if( idxFirstDash < 0 )
				return false;	// no dashes at all

			int idxSecond = complete.IndexOfAny( s_dashes, idxFirstDash + 1 );
			if( idxSecond < 0 )
			{
				// Single dash
				cutByDash( complete, idxFirstDash, out artist, out title );
				return true;
			}

			// At least two dashes - try to find one surrounded by spaces.
			var matches = s_reDashAndSpaces.Matches( complete );
			if( matches.Count == 1 )
			{
				cutByDash( complete, matches[ 0 ].Index + 1, out artist, out title );
				return true;
			}
			return false;
		}
	}
}