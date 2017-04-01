using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SkyFM.Model.JSON
{
	/// <summary>A response to "/v1/sky/members/authenticate" call</summary>
	public class LoginResponse
	{
		[JsonProperty]
		public bool activated;

		[JsonProperty]
		public string api_key; // TODO [low] change to byte[16] or GUID

		[JsonProperty]
		public bool confirmed;

		[JsonProperty]
		public string email, first_name, last_name;

		// TODO [low]: WTF is "fraudulent" : null ?

		[JsonProperty]
		public int id;

		[JsonProperty]
		public string listen_key;

		public class FavoriteChannel
		{
			[JsonProperty]
			public int channel_id, position;
		}

		[JsonProperty]
		public FavoriteChannel[] network_favorite_channels;

		public class Subscription
		{
			[JsonProperty]
			public bool auto_renew, billable, trial;

			[JsonProperty, JsonConverter( typeof( IsoDateTimeConverter ) )]
			public DateTime expires_on, starts_on;

			[JsonProperty]
			public int id;

			[JsonProperty]
			public string status;	// TODO [low]: replace with enum?

			public class Service
			{
				[JsonProperty]
				public int id;

				[JsonProperty]
				public string key, name;
			}

			[JsonProperty]
			public Service[] services;
		}

		[JsonProperty]
		public Subscription[] subscriptions;

		[JsonProperty]
		public string timezone;
	}
}