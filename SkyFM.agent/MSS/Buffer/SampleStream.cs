using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SkyFM.agent.MSS
{
	class SampleStream: Stream
	{
		readonly iByteStream source;
		public SampleStream( iByteStream src )
		{
			this.source = src;
		}

		int cbLength;
		int cbPosition;

		public void feedNextFrame( int cbLength )
		{
			this.cbLength = cbLength;
			this.cbPosition = 0;
		}

		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return false; } }
		public override bool CanWrite { get { return false; } }

		public override void Flush() { throw new NotSupportedException(); }

		public override long Length { get { return cbLength; } }

		public override long Position
		{
			get { return cbPosition; }
			set { throw new NotSupportedException(); }
		}

		public override int Read( byte[] buffer, int offset, int count )
		{
			int cbLeft = cbLength - cbPosition;
			if( cbLeft <= 0 )
			{
				Logger.warning( "The OS is attempting to read beyond the end of ma stream." );
				return 0;
			}
			int cbToRead = Math.Min( count, cbLeft );
			int cbRead = this.source.Read( buffer, offset, cbToRead );
			if( cbRead <= 0 )
			{
				throw new Exception( "iByteStream.Read failed for some reason :-(" );
			}

			this.cbPosition += cbRead;
			/* if( this.cbPosition >= this.cbLength )
			{
				// Read the stream to the end.

			} */
			return cbRead;
		}

		public override long Seek( long offset, SeekOrigin origin )
		{
			throw new NotSupportedException();
		}

		public override void SetLength( long value )
		{
			throw new NotSupportedException();
		}

		public override void Write( byte[] buffer, int offset, int count )
		{
			throw new NotSupportedException();
		}
	}
}