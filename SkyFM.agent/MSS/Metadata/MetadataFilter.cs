using System;
using System.Collections.Generic;
using System.Linq;

namespace SkyFM.agent.MSS
{
	/// <summary>This class implements iByteStream of sound bytes on the top of another iByteStream containing sound + shoutcast metadata bytes.</summary>
	/// <remarks>The metadata encountered in the stream while reading is pushed into the optional MetadataProducer instance.</remarks>
	class MetadataFilter: iByteStream
	{
		public CircularBuffer source { get { return this.sourceStream.source; } }
		readonly iByteStream sourceStream;
		readonly int metadataInterval;
		public int metaByteCounter { get; private set; }
		readonly MetadataProducer metadataProducer;

		public MetadataFilter( iByteStream src, int metaInt, MetadataProducer metaProd )
		{
			this.sourceStream = src;
			this.metadataInterval = metaInt;
			this.metadataProducer = metaProd;
		}

		public int ReadBytes( int cb, Action<int, int> onData )
		{
			if( metadataInterval <= 0 )
				return sourceStream.ReadBytes( cb, onData );

			int res = 0;

			while( true )
			{
				if( metaByteCounter < 0 )
				{
					// Currently positioned on metadata
					if( null == this.metadataProducer )
						metaByteCounter += sourceStream.Skip( -metaByteCounter );
					else
						metaByteCounter += this.metadataProducer.produceMetadata( sourceStream, -metaByteCounter );
					if( metaByteCounter < 0 )
						return res;	// EOF trying to skip metadata
				}

				if( metaByteCounter < metadataInterval )
				{
					// Currently positioned on normal stream data
					int cbToRead = Math.Min( cb, metadataInterval - metaByteCounter );
					int cbRead = sourceStream.ReadBytes( cbToRead, onData );
					this.metaByteCounter += cbRead;
					cb -= cbRead;
					res += cbRead;

					if( cbRead < cbToRead )
						return res; // EOF trying to skip normal data
					if( cb <= 0 )
						return res;
				}

				if( metaByteCounter == metadataInterval )
				{
					// Currently positioned on the metadata length byte
					int metaLengthHeader = sourceStream.ReadByte();
					if( metaLengthHeader < 0 )
						return res;	// EOF trying to read metadata size
					int cbMetadataLength = 16 * metaLengthHeader;
					metaByteCounter = -cbMetadataLength;
					continue;
				}
			}
		}

		public iByteStream Clone()
		{
			MetadataFilter res = new MetadataFilter( this.sourceStream.Clone(), this.metadataInterval, null );
			res.metaByteCounter = this.metaByteCounter;
			return res;
		}

		public void assignFrom( iByteStream iSrc )
		{
			MetadataFilter src = iSrc as MetadataFilter;
			if( null == src )
				throw new Exception( "Type mismatch" );
			if( src.source != this.source )
				throw new Exception( "The readers are from different buffers" );

			this.sourceStream.assignFrom( src.sourceStream );
			this.metaByteCounter = src.metaByteCounter;
		}
	}
}