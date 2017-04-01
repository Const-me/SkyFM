using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SkyFM.Model
{
	public class ChannelFilter
	{
		public class Channel
		{
			// [JsonProperty]
			// public string ad_channels, channel_director, description, key, name;
			[JsonProperty]
			public string channel_director, description, name;

			// [JsonProperty, JsonConverter( typeof( IsoDateTimeConverter ) )]
			// public DateTime created_at, updated_at;

			// [JsonProperty]
			// public int asset_id, id, network_id, old_id, tracklist_server_id;
			[JsonProperty]
			public int id;

			// [JsonProperty]
			// public int? forum_id, premium_id;

			[JsonProperty]
			// public Uri asset_url;
			public string asset_url;
		}

		[JsonProperty]
		public Channel[] channels;

		/* [JsonProperty]
		public int id, network_id, position;

		[JsonProperty]
		public string key, name; */

		[JsonProperty]
		public string key;
	}

	public class TrackHistory
	{
		/* [JsonProperty]
		public Uri art_url;

		[JsonProperty]
		public string artist, title, track;

		[JsonProperty]
		public int channel_id, length, network_id, track_id;

		// Sometimes null.
		[JsonProperty]
		public int? duration;

		[JsonProperty, JsonConverter( typeof( UnixDateTimeConverter ) )]
		public DateTime started; */

		[JsonProperty]
		public string track;
	}
}