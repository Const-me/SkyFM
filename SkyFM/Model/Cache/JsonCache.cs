using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkyFM.Model.JSON;
using SkyFM.shared;

namespace SkyFM.Model
{
	/// <summary>This class represents a cache of the JSON data from the server: stream sets, track history.</summary>
	class JsonCache
	{
		// const string qualityKey = "appleapp_premium_medium";

		const string fileChannelsCache = "channels.json";
		const string fileHistoryCache = "history.json";

		ChannelsCache m_channels = null;
		TrackHistoryCache m_trackHistory = null;

		private JsonCache()
		{
			// StreamsCache.instance.update( false );
			FileSystem.readObject( fileChannelsCache, out m_channels );
			FileSystem.readObject( fileHistoryCache, out m_trackHistory );
		}

		static JsonCache s_cache;
		public static JsonCache instance { get {
			if( null != s_cache )
				return s_cache;
			s_cache = new JsonCache();
			return s_cache;
		} }

#if DEBUG
		private JsonCache( BatchUpdateResponse response )
		{
			initWithResponse( response );
		}

		public static void initWithDebugData( BatchUpdateResponse response )
		{
			s_cache = new JsonCache( response );
		}
#endif
		void initWithResponse( BatchUpdateResponse response )
		{
			m_channels = new ChannelsCache( response );
			m_trackHistory = new TrackHistoryCache( response );
		}

		void saveResponse( BatchUpdateResponse response )
		{
			initWithResponse( response );
			FileSystem.writeObject( m_channels, fileChannelsCache );
			FileSystem.writeObject( m_trackHistory, fileHistoryCache );
		}

		public void writeStreamListForQuality( string qualityKey )
		{
			m_channels.streamlists[ qualityKey ].write();
		}

		void saveHistory( Dictionary<int, TrackHistory> history )
		{
			m_trackHistory = new TrackHistoryCache( history );
			FileSystem.writeObject( m_trackHistory, fileHistoryCache );
		}

		static readonly TimeSpan s_tsChannelsCacheLifetime = TimeSpan.FromDays( 1 );
		static readonly TimeSpan s_tsHistoryCacheLifetime = TimeSpan.FromMinutes( 5 );

		public async Task update()
		{
			do
			{
				if( null == m_channels )
					break;	// no channels at all
				if( m_channels.downloaded_at.newerThen( s_tsChannelsCacheLifetime ) )
				{
					// channels still fresh, only update the history
					await updateHistory();
					return;
				}
			}
			while( false );

			BatchUpdateResponse response = await Rpc.batchUpdate();
			saveResponse( response );
		}

		public bool needsUpdating { get {
#if DEBUG
			return true;
#endif
			if( null == m_channels || null == m_trackHistory )
				return true;
			if( m_channels.downloaded_at.olderThen( s_tsChannelsCacheLifetime ) )
				return true;
			if( m_trackHistory.downloaded_at.olderThen( s_tsHistoryCacheLifetime ) )
				return true;
			return false;
		} }

		async Task updateHistory()
		{
			do
			{
				if( null == m_trackHistory )
					break;
				if( m_trackHistory.downloaded_at.newerThen( s_tsHistoryCacheLifetime ) )
					return;
			}
			while( false );

			Dictionary<int, TrackHistory> response = await Rpc.trackHistory();
			saveHistory( response );
		}

		public TrackHistory historyForChannel( int idChannel )
		{
			if( null == m_trackHistory )
				return null;
			return m_trackHistory.track_history.itemOrDefault( idChannel );
		}

		public ChannelFilter channelFilterForKey( string k )
		{
			if( null == m_channels || null == m_channels.channel_filters )
				return null;
			return m_channels.channel_filters
				.Where( cf => cf.key == k )
				.FirstOrDefault();
		}

		public Dictionary<string, StreamList> getStreamLists() { return m_channels.streamlists; }

		Dictionary<int, string> m_channelsNames = new Dictionary<int, string>();
		public string channelNameForId( int i )
		{
			string res;
			if( m_channelsNames.TryGetValue( i, out res ) )
				return res;
			var chan = channelFilterForKey( "default" )
				.channels
				.Where( ch => ch.id == i )
				.FirstOrDefault();
			if( null == chan )
				return "----";
			res = chan.name.ToLower();;
			m_channelsNames[ i ] = res;
			return res;
		}
	}
}