using System;

namespace SkyFM.agent.MSS
{
	class MetadataProducer
	{
		const int cbMaxMetadata = 4080;
		readonly byte[] buff = new byte[ cbMaxMetadata ];

		int length = 0;

		readonly iMetadataConsumer destination;

		public MetadataProducer( iMetadataConsumer destination )
		{
			this.destination = destination;
		}

		public int produceMetadata( iByteStream src, int cb )
		{
			int cbRead = src.Read( buff, length, cb );
			length += cbRead;
			if( cbRead < cb )
				return cbRead;   // EOF inside metadata

			// Metadata block read to the end.
			this.destination.onMetadata( buff, length );

			this.length = 0;
			return cbRead;
		}
	}
}