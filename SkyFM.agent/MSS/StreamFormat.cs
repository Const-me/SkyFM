using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace SkyFM.agent.MSS
{
	/// <summary>This abstract class implements format-specific audio stream parsing.</summary>
	/// <remarks>This library contains 2 separate implementations, <see cref="MpegStreamFormat"/> for MP3 streams, and <see cref="HeAacStreamFormat"/> for HE-AAC streams.</remarks>
	abstract class StreamFormat
	{
		public iSamplesConsumer consumer { get; private set; }

		protected static byte shiftMask( int val, byte shift, byte mask )
		{
			return (byte)( ( val >> shift ) & mask );
		}

		public readonly iLocator locator;

		public StreamFormat( iLocator locator )
		{
			this.locator = locator;

			this.mediaSourceAttributes = new Dictionary<MediaSourceAttributesKeys, string>()
			{
				{ MediaSourceAttributesKeys.CanSeek, "0" },
				{ MediaSourceAttributesKeys.Duration, "0" },
			};

			mediaStreamAttributes = new Dictionary<MediaStreamAttributeKeys, string>( 1 );

			MediaStreamDescription msd = new MediaStreamDescription( MediaStreamType.Audio, this.mediaStreamAttributes );
			this.mediaStreams = new MediaStreamDescription[ 1 ] { msd };
		}

		public IDictionary<MediaSourceAttributesKeys, string> mediaSourceAttributes { get; private set; }
		readonly Dictionary<MediaStreamAttributeKeys, string> mediaStreamAttributes;
		public IEnumerable<MediaStreamDescription> availableMediaStreams { get { return mediaStreams; } }
		readonly MediaStreamDescription[] mediaStreams;

		protected enum eSetupFrameResult: byte
		{
			/// <summary>Setup next frame completed OK.</summary>
			OK = 0,

			/// <summary>Not enough data in the source bufer to setup next frame</summary>
			NoData = 1,

			/// <summary>The stream is NOT positioned in a valid audio frame</summary>
			Fail = 2,
		}

		/// <summary>Called e.g. during the seek.</summary>
		/// <param name="bytes">Source stream.</param>
		/// <returns>if the provided stream was positioned on the start of the valid audio frame, returns the frame's length in bytes.</returns>
		protected abstract eSetupFrameResult trySetupNextFrame( iByteStream src, bool deepSearch );

		/// <summary>Construct the codec's private data and store is in the mediaStreamAttributes dictionary.</summary>
		/// <remarks>Is guaranteed to call immediately after the successful trySetupNextFrame.</remarks>
		public void fillCodecPrivateData()
		{
			var fmt = this.constructCodecPrivateData();
			StringBuilder sb = new StringBuilder( 128 );
			fmt.PrintHex( sb );
			string strHex = sb.ToString();
			this.mediaStreamAttributes[ MediaStreamAttributeKeys.CodecPrivateData ] = strHex;
			Logger.info( "result: {0}", strHex );
		}

		/// <summary>Construct the structure holding codec's private data</summary>
		/// <remarks>Is guaranteed to call immediately after the successful trySetupNextFrame.</remarks>
		protected abstract WAVEFORMATEX constructCodecPrivateData();

		protected abstract long getSamplePlacement( out int offset, out int count );

		iByteStream srcTemp = null;

		/// <summary>Search the input stream for the frame header</summary>
		/// <param name="deepSearch">true to check one frame ahead, false to only check the single header</param>
		/// <returns></returns>
		public bool synchronizeStream( bool deepSearch )
		{
			if( null == srcTemp )
				srcTemp = locator.mainSoundStream.Clone();
			else
				srcTemp.assignFrom( locator.mainSoundStream );

			while( true )
			{
				var res = this.trySetupNextFrame( srcTemp, deepSearch );
				if( res == eSetupFrameResult.Fail )
				{
					// Not at the start of the frame: skip a byte and retry.
					if( locator.mainSoundStream.ReadByte() < 0 )
						return false;
					srcTemp.assignFrom( locator.mainSoundStream );
					deepSearch = true;	// we're obviously out of sync, so deep search might be a good idea.
					continue;
				}
				if( res == eSetupFrameResult.OK )
					return true;
				if( res == eSetupFrameResult.NoData )
					return false;
			}
		}

		SampleStream sampleStream;

		private static readonly Dictionary<MediaSampleAttributeKeys, string> s_emptySampleAttributes = new Dictionary<MediaSampleAttributeKeys, string>();

		long currentTimestamp = 0;

		protected static long audioDurationFromBufferSize( int avgBitrate, int audioDataSize )
		{
			if( avgBitrate <= 0 )
				return 0;
			return (long)audioDataSize * 10000000 / ( avgBitrate / 8 );
		}

		public MediaStreamSample constructSample()
		{
			if( null == sampleStream )
				sampleStream = new SampleStream( locator.mainSoundStream );

			int offset, count;
			long duration = this.getSamplePlacement( out offset, out count );
			sampleStream.feedNextFrame( offset + count );
			var res = new MediaStreamSample( this.mediaStreams[ 0 ], sampleStream, offset, count, currentTimestamp, s_emptySampleAttributes );
			currentTimestamp += duration;
			return res;
		}

		public MediaStreamSample constructEmptySample()
		{
			return new MediaStreamSample( this.mediaStreams[ 0 ], null, 0, 0, 0, s_emptySampleAttributes );
		}
	}
}