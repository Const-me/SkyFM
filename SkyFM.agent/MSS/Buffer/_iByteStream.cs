using System;
using System.Collections.Generic;
using System.Linq;

namespace SkyFM.agent.MSS
{
	/// <summary>This interface represents a read-only stream of bytes in RAM.</summary>
	interface iByteStream
	{
		/// <summary>The buffer with the data</summary>
		CircularBuffer source { get; }

		/// <summary>Either read or skip the bytes from the stream</summary>
		/// <param name="cb">How many bytes</param>
		/// <param name="onData">Is called for every continuous buffer segment in the requested data chunk.</param>
		/// <returns>Count of bytes to read/skip.</returns>
		int ReadBytes( int cb, Action<int, int> onData );

		/// <summary>Clone the state of the stream.</summary>
		/// <returns>A new clone of the stream.</returns>
		/// <remarks>This operation does not copy any data! Only the stream position is copied.</remarks>
		iByteStream Clone();

		/// <summary>Assign the stream position from another stream.</summary>
		/// <param name="src"></param>
		void assignFrom( iByteStream src );
	}

	internal static class ByteStreamExt
	{
		/// <summary>Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.</summary>
		/// <returns>The unsigned byte cast to an Int32, or -1 if at the end of the stream.</returns>
		public static int ReadByte( this iByteStream stm )
		{
			int res = -1;
			stm.ReadBytes( 1, ( offset, count ) => { 
				res = stm.source.buff[ offset ];
			} );
			return res;
		}

		/// <summary>Skip the specified count of bytes from the stream.</summary>
		/// <param name="cb">By how many bytes?</param>
		/// <returns>Count of skipped bytes</returns>
		public static int Skip( this iByteStream stm, int cb )
		{
			return stm.ReadBytes( cb, null );
		}

		/// <summary>Skip the specified count of bytes from the stream.</summary>
		/// <param name="stm"></param>
		/// <param name="cb"></param>
		/// <returns>true if all bytes skipped, false if there was not that many bytes in the stream.</returns>
		public static bool trySkip( this iByteStream stm, int cb )
		{
			return cb == stm.Skip( cb );
		}

		/// <summary>Read data from the stream into the specified buffer.</summary>
		/// <param name="stm"></param>
		/// <param name="dest"></param>
		/// <param name="destOffset"></param>
		/// <param name="readCount"></param>
		/// <returns></returns>
		public static int Read( this iByteStream stm, byte[] dest, int destOffset, int readCount )
		{
			byte[] src = stm.source.buff;
			return stm.ReadBytes( readCount, ( srcOffset, count ) =>
			{
				Array.Copy( src, srcOffset, dest, destOffset, count );
				destOffset += count;
			} );
		}
	}
}