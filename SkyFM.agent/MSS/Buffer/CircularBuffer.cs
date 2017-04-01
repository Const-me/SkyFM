using System;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace SkyFM.agent.MSS
{
	internal class CircularBuffer
	{
		/// <summary>Actual circular buffer holding the data.</summary>
		public readonly byte[] buff = new byte[ BufferConfig.cbTotal ];

		public readonly object syncRoot = new object();

		/// <summary>Pointer to the start of the buffered data.</summary>
		public int ptrFirst { get; private set; }

		/// <summary>Length of the buffered data, including the metadata.</summary>
		public int cbLength { get; private set; }

		/// <summary>Available buffer space.</summary>
		public int cbAvailable { get { return BufferConfig.cbTotal - cbLength; } }

		public virtual void commitNewData( int cbReceived )
		{
			lock( this.syncRoot )
			{
				this.cbLength += cbReceived;
				if( this.cbLength > BufferConfig.cbTotal )
					throw new Exception( "The buffer has no space for the new data." );
			}
		}

		public void getFreeBuffer( out int offset, out int count )
		{
			lock( syncRoot )
			{
				count = BufferConfig.cbReceive;
				count = Math.Min( count, cbAvailable );
				if( count <= 0 )
					throw new Exception( "No buffer space" );
				offset = BufferConfig.Wrap( ptrFirst + cbLength );
				count = Math.Min( count, BufferConfig.cbTotal - offset );
			}
		}

		public bool hasFreeSpace()
		{
			lock( this.syncRoot )
			{
				return cbAvailable > BufferConfig.cbReceive;
			}
		}

		public void Clear()
		{
			this.cbLength = 0;
			this.ptrFirst = 0;
		}

		public CircularBufferReader createReader()
		{
			lock( this.syncRoot )
			{
				return new CircularBufferReader( this );
			}
		}

		public void syncWithReader( CircularBufferReader reader )
		{
			lock( this.syncRoot )
			{
				// Discard read parts
				int ptrNewStart = reader.ptrFirst;
				if( ptrNewStart != this.ptrFirst )
				{
					int cb = BufferConfig.Wrap( ptrNewStart - this.ptrFirst + BufferConfig.cbTotal );
					this.cbLength -= cb;
					this.ptrFirst = ptrNewStart;
				}

				if( this.cbLength > reader.cbLength )
					reader.setNewLength( this.cbLength );
			}
		}

		public byte bufferFullness { get {
			int len = cbLength;	//< atomic operation
			int size = BufferConfig.cbTotal - ( 2 * BufferConfig.cbReceive );
			if( len >= size )
				return 255;
			return (byte)( len * 255 / size );
		} }
	}
}