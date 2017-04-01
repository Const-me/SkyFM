using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyFM
{
	[JsonObject]
	class stream_set
	{
		[JsonProperty]
		public int id, network_id;

		[JsonProperty]
		public string name, key, description;

		[JsonObject]
		public class Stream
		{
			[JsonProperty]
			public int id;

			[JsonProperty]
			public string url;

			[JsonProperty]
			public string format;

			[JsonProperty]
			public int bitrate;
		}

		[JsonObject]
		public class stream_list
		{
			[JsonProperty]
			public int id;

			[JsonProperty]
			public string name;


		}
	}
}