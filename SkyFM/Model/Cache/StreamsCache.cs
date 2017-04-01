using System;
using System.Linq;
using SkyFM.Model.JSON;
using System.Collections.Generic;

namespace SkyFM.Model
{
	static class StreamsCache
	{
		static StreamList streamListForQuality( this BatchUpdateResponse response, string k )
		{
			if( null == response || null == response.streamlists )
				return null;
			return response.streamlists.itemOrDefault( k );
		}

		/// <summary>Write the StreamList to the isolated storage, for the background agent process to pick up.</summary>
		/// <param name="lst">The streams list to write.</param>
		public static void write( this StreamList lst )
		{
			var vals = lst.channels
				.SelectMany( chan => 
					chan.streams.Select( stm => new { idChannel = chan.id, streamUri = stm.url } ) )
				.ToLookup( e => e.idChannel, e => e.streamUri );

			int bitrate = 1000 * lst.channels.First().streams.First().bitrate;
			SkyFM.shared.StreamsCache.Write( bitrate, vals );
			SkyFM.ViewModel.BackgroundPlayerViewModel.agentShouldReloadStreams = true;
		}
	}
}