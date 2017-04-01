using System;
using System.Collections.Generic;
using System.Linq;

namespace SkyFM.agent.MSS
{
	static class BufferConfig
	{
		/// <summary>Total circular buffer length.</summary>
		public const int cbTotal = 512 * 1024;

		/// <summary>Wrap the index around the length.</summary>
		/// <remarks>Because the total length is power of 2, cheap bitwise AND is enough for that.</remarks>
		/// <param name="pb">Offset in buffer</param>
		/// <returns></returns>
		public static int Wrap( int pb ) { return pb & 0x7FFFF; }

		/// <summary>Size of single Receive() buffer</summary>
		/// <seealso href="http://networking.ittoolbox.com/groups/technical-functional/tcp-ip-l/maximum-size-for-tcp-data-1651195"/>
		public const int cbReceive = 1448 * 3;

		/// <summary>Count of bytes to pre-buffer before start playing.</summary>
		/// <remarks>This value includes metadata.</remarks>
		public const int cbPreBuffer = 256 * 1024;

		/// <summary>When count of bytes in the buffer drops below this threshold, the player stops playing and starts buffering.</summary>
		/// <remarks>This value includes metadata.</remarks>
		public const int cbMinBuffer = 16 * 1024;

		/// <summary>Synchronization is performed on the window of this size.</summary>
		/// <remarks>This value includes metadata.</remarks>
		public const int cbSyncBuffer = 4 * 1024;

		public static readonly TimeSpan tsSleepTimeWhenBufferFull_normal = TimeSpan.FromSeconds( 1 );
		public static readonly TimeSpan tsSleepTimeWhenBufferFull_energySaver = TimeSpan.FromSeconds( 15 );
	}
}