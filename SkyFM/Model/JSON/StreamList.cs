using System;
using Newtonsoft.Json;

namespace SkyFM
{
	/// <summary>This class represents a stream list, as it comes from the JSON server.</summary>
	[JsonObject]
	public class StreamList
	{
		[JsonProperty]
		public int id;

		[JsonProperty]
		public string name;

		public class Channel
		{
			[JsonProperty]
			public int id;

			public class Stream
			{
				[JsonProperty]
				public int id, bitrate;

				public enum eFormat: byte { aac, mp3 };
				[JsonProperty]
				public eFormat format;

				[JsonProperty]
				public string url;
			}

			[JsonProperty]
			public Stream[] streams;
		}

		[JsonProperty]
		public Channel[] channels;
	}
}