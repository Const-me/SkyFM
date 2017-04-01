using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using SkyFM.Model.JSON;

namespace SkyFM.Model
{
	[JsonObject]
	public class ChannelsCache
	{
		[JsonProperty]
		public DateTime downloaded_at;

		[JsonProperty]
		public ChannelFilter[] channel_filters;

		[JsonProperty, JsonConverter( typeof( DictionaryConverter<string, StreamList> ) )]
		public Dictionary<string, StreamList> streamlists;

		public ChannelsCache() { }
		public ChannelsCache( BatchUpdateResponse response )
		{
			this.downloaded_at = DateTime.UtcNow;
			this.channel_filters = response.channel_filters;
			this.streamlists = response.streamlists;
		}
	}

	[JsonObject]
	public class TrackHistoryCache
	{
		[JsonProperty]
		public DateTime downloaded_at;

		[JsonProperty, JsonConverter( typeof( DictionaryConverter<int, TrackHistory> ) )]
		public Dictionary<int, TrackHistory> track_history;

		public TrackHistoryCache() { }
		public TrackHistoryCache( BatchUpdateResponse response )
		{
			this.downloaded_at = DateTime.UtcNow;
			this.track_history = response.track_history;
		}

		public TrackHistoryCache( Dictionary<int, TrackHistory> history )
		{
			this.downloaded_at = DateTime.UtcNow;
			this.track_history = history;
		}
	}

	[JsonObject]
	public class LoginCache
	{
		[JsonProperty]
		public string userName, password;

		[JsonProperty]
		public DateTime downloaded_at;

		[JsonProperty]
		public LoginResponse response;

		public LoginCache() { }

		public LoginCache( string userName, string password, LoginResponse response )
		{
			this.downloaded_at = DateTime.UtcNow;
			this.userName = userName;
			this.password = password;
			this.response = response;
		}
	}
}