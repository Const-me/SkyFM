using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkyFM.agent.MSS
{
	/// <summary>This class represents read-only forward-only CircularBuffer iterator state.</summary>
	class CircularBufferReader: iByteStream
	{
		public CircularBufferReader( CircularBuffer src )
		{
			this.source = src;
		}

		public CircularBuffer source { get; private set; }

		public int ptrFirst { get; private set; }
		public int cbLength{ get; private set; }

		public int ReadBytes( int cb, Action<int, int> onData )
		{
			if( this.cbLength <= 0 )
				return 0;

			if( cb > this.cbLength )
				cb = cbLength;

			if( this.ptrFirst + cb <= BufferConfig.cbTotal )
			{
				// Simple case here: the requested chunk does not overlap buffer's tail
				if( null != onData )
					onData( this.ptrFirst, cb );
				this.ptrFirst += cb;
				cbLength -= cb;
				return cb;
			}

			int res = 0;

			if( this.ptrFirst <= BufferConfig.cbTotal )
			{
				// First piece of the requested data is from the current position to buffer's tail
				// This piece may be zero-length.
				// Then wrap the buffer.
				int cbToEnd = BufferConfig.cbTotal - this.ptrFirst;
				if( null != onData && cbToEnd > 0 )
					onData( this.ptrFirst, cbToEnd );
				this.cbLength -= cbToEnd;
				res += cbToEnd;
				cb -= cbToEnd;
				this.ptrFirst = 0;
			}

			// Final piece of the requested data starts from the head of the buffer, the recursion is adequate here.
			// This piece may be zero-length as well.
			if( cb > 0 )
				res += ReadBytes( cb, onData );
			return res;
		}

		public bool reset()
		{
			ptrFirst = source.ptrFirst;
			cbLength = source.cbLength;
			return cbLength > 0;
		}

		/// <summary>Reset the reader so it will up to specified count of bytes from the original source</summary>
		/// <param name="cb">Length</param>
		/// <returns>False if there's not enough data in the buffer</returns>
		public bool resetToLength( int cb )
		{
			reset();

			if( this.cbLength >= cb )
			{
				this.cbLength = cb;
				return true;
			}
			return false;
		}


		public iByteStream Clone()
		{
			CircularBufferReader res = new CircularBufferReader( this.source );
			res.ptrFirst = this.ptrFirst;
			res.cbLength = this.cbLength;
			return res;
		}

		public void assignFrom( iByteStream iSrc )
		{
			CircularBufferReader src = iSrc as CircularBufferReader;
			if( null == src )
				throw new Exception( "Type mismatch" );
			if( src.source != this.source )
				throw new Exception( "The readers are from different buffers" );
			this.ptrFirst = src.ptrFirst;
			this.cbLength = src.cbLength;
		}

		public void setNewLength( int newLen )
		{
			if( newLen <= this.cbLength )
				throw new Exception( "setNewLength should never decrease the length" );
			this.cbLength = newLen;
		}
	}
}