using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace SkyFM.Model.JSON
{
	/// <summary>A response to GET /v1/sky/mobile/batch_update?asset_group_key=mobile_icons&stream_set_key=appleapp,appleapp_high,appleapp_low,appleapp_premium,appleapp_premium_high,appleapp_premium_low,appleapp_premium_medium</summary>
	public class BatchUpdateResponse
	{
		/* public class AssetGroup
		{
			public class Asset
			{
				/* [JsonProperty]
				public int channel_id;

				[JsonProperty]
				public string ext;

				[JsonProperty, JsonConverter( typeof( ByteArrayConvertor ) )]
				public byte[] md5;

				[JsonProperty]
				public Uri url; * /
			}

			[JsonProperty]
			public Asset[] assets;

			[JsonProperty]
			public int network_id;
		}

		[JsonProperty]
		public AssetGroup asset_group; */

		/* public class Asset
		{
			[JsonProperty, JsonConverter( typeof( ByteArrayConvertor ) )]
			public byte[] content_hash;

			[JsonProperty, JsonConverter( typeof( IsoDateTimeConverter ) )]
			public DateTime created_at, updated_at;

			[JsonProperty]
			public string name, description;

			[JsonProperty]
			public int id, image_id, network_id;
		}

		[JsonProperty]
		public Asset[] assets; */

		/* [JsonProperty, JsonConverter( typeof( IsoDateTimeConverter ) )]
		public DateTime cached_at; */

		[JsonProperty]
		public ChannelFilter[] channel_filters;

		[JsonProperty, JsonConverter( typeof( DictionaryConverter<string, StreamList> ) )]
		public Dictionary<string, StreamList> streamlists;

		[JsonProperty, JsonConverter( typeof( DictionaryConverter<int, TrackHistory> ) )]
		public Dictionary<int, TrackHistory> track_history;
	}
}