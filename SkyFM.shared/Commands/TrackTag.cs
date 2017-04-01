using System;
using System.Threading;
using Microsoft.Phone.BackgroundAudio;

namespace SkyFM.shared
{
	/// <summary>This class constructs/updates audio track's Tag property.</summary>
	public static class TrackTag
	{
		static readonly Mutex s_mutex = new Mutex( false, "bdba6c69-9b1b-4489-83e5-394143d37f36" );

		/// <summary>Update frequency.</summary>
		/// <remarks>Player statistic is updated with this frequency.
		/// GUI process polls for changes with the same frequency.
		/// Metadata (i.e. track name) is updated as soon as it's available, i.e. much less frequent.</remarks>
		public static readonly TimeSpan statusUpdateInterval = TimeSpan.FromSeconds( 0.5 );

		static void update( this AudioTrack track, Action<AudioTrack> act )
		{
			s_mutex.WaitOne();
			try
			{
				track.BeginEdit();
				act( track );
				track.EndEdit();
			}
			catch( Exception ex )
			{
				ex.logWarning();
			}
			finally
			{
				s_mutex.ReleaseMutex();
			}
		}

		public static void updateStatus( this AudioTrack track, int idChannel, bool bPending, byte buff, long networkBytes, string errorText )
		{
			string statusTag = TagFields.statusMessage( idChannel, bPending, buff, networkBytes, errorText );
			track.update( t =>
			{
				t.Tag = statusTag;
			} );
		}

		public static TagFields parseTag( this AudioTrack track )
		{
			string tag = null;
			s_mutex.WaitOne();
			try
			{
				tag = track.Tag;
			}
			finally
			{
				s_mutex.ReleaseMutex();
			}

			if( String.IsNullOrEmpty( tag ) )
				return null;
			var args = SharedUtils.parseQueryString( tag );
			return new TagFields( args );
		}

		public static void updateMetadata( this AudioTrack track, string title, string artist )
		{
			track.update( t =>
			{
				t.Artist = artist;
				t.Title = title;
			} );
		}
	}
}