using System;

namespace SkyFM.shared
{
	/// <summary>This class holds the data passed from the agent to the UI process.</summary>
	public abstract class PlayerStatus
	{
		/// <summary>Update frequency.</summary>
		/// <remarks>Player statistic is updated with this frequency.
		/// GUI process polls for changes with the same frequency.
		/// Metadata (i.e. track name) is updated as soon as it's available, i.e. much less frequent.</remarks>
		public static readonly TimeSpan updateInterval = TimeSpan.FromSeconds( 0.5 );

		public override string ToString()
		{
			return String.Format( "{0}|{1}", m_bufferFill, m_cbTotalNetworkBytes );
		}

		protected bool tryParse( string msg )
		{
			if( String.IsNullOrEmpty( msg ) )
				return false;
			string[] fields = msg.Split( '|' );
			if( fields.Length < 2 )
				return false;
			if( !byte.TryParse( fields[ 0 ], out m_bufferFill ) )
				return false;
			if( !long.TryParse( fields[ 1 ], out m_cbTotalNetworkBytes ) )
				return false;
			return true;
		}

		protected byte m_bufferFill;
		protected long m_cbTotalNetworkBytes;

		protected static double bufferPercentage( byte buff )
		{
			return (double)( buff ) / 255.0;
		}
	}
}