using System;
using System.Windows.Media;
using System.Collections.Generic;
using Microsoft.Phone.Shell;

namespace SkyFM.ViewModel
{
	public static class ChannelPageColorer
	{
		static Color color( this uint val )
		{
			byte[] bytes = BitConverter.GetBytes( val );
			if( 0 == bytes[ 3 ] )
				bytes[ 3 ] = 255;
			return Color.FromArgb( bytes[ 3 ], bytes[ 2 ], bytes[ 1 ], bytes[ 0 ] );
		}

		static uint multiplyColor( this uint val, float by )
		{
			byte[] bytes = BitConverter.GetBytes( val );
			if( 0 == bytes[ 3 ] )
				bytes[ 3 ] = 255;
			for( int i = 0; i < 3; i++ )
			{
				float c = (float)bytes[ i ];
				c *= by;
				if( c < 255 )
					bytes[ i ] = (byte)( c + 0.5f );
				else
					bytes[ i ] = 255;
			}
			return BitConverter.ToUInt32( bytes, 0 );
		}

		static void color( this ChannelPage pg, string key, uint color )
		{
			var brush = pg.Resources[ key ] as SolidColorBrush;
			if( null == brush )
				return;
			var res = color.color();
			brush.Color = res;
			if( key == "colorStatus" )
				SystemTray.SetForegroundColor( pg, res );
		}

		static void setColors( this ChannelPage pg, uint top, uint topDim, uint topDimmer, uint playing, uint status )
		{
			pg.color( "colorTop", top );
			pg.color( "colorTopDim", topDim );
			pg.color( "colorTopDimmer", topDimmer );
			pg.color( "colorPlaying", playing );
			pg.color( "colorStatus", status );
		}

		static void setColors( this ChannelPage pg, uint[] colors )
		{
			if( 5 == colors.Length )
			{
				pg.setColors( colors[ 0 ], colors[ 1 ], colors[ 2 ], colors[ 3 ], colors[ 4 ] );
				return;
			}
			if( 3 == colors.Length )
			{
				float fDim = 0.666f, fDimmer = 0.333f;
				uint top = colors[ 0 ];
				pg.setColors( top, top.multiplyColor( fDim ), top.multiplyColor( fDimmer ), colors[ 1 ], colors[ 2 ] );
				return;
			}
		}

		static readonly Dictionary<int, uint[]> s_colors = new Dictionary<int, uint[]>()
		{
			{ 173, new uint[ 3 ]{ 0xFF9922, 0x99BB11, 0x119911 } },	// Smooth Lounge
			{ 20,  new uint[ 3 ]{ 0x36CCFF, 0x0088FF, 0x0000DD } },	// Smooth Jazz
			{ 128, new uint[ 3 ]{ 0x33AAFF, 0x0011FF, 0x220099 } },	// Modern blues
			{ 118, new uint[ 3 ]{ 0xEEDD11, 0x0099DD, 0x776600 } },	// Smooth Jazz 24'7
			{ 25,  new uint[ 3 ]{ 0xCC1100, 0x1177FF, 0x0000BB } },	// Country
			{ 23,  new uint[ 3 ]{ 0xDD2222, 0xFFCC33, 0x007733 } },	// Roots Reggae
			{ 41,  new uint[ 3 ]{ 0xDD2222, 0xAA9900, 0x556600 } },	// Uptempo Smooth Jazz
			{ 61,  new uint[ 3 ]{ 0xDD2222, 0xCC3399, 0x6600DD } },	// Love music
			{ 188, new uint[ 3 ]{ 0xFFFFDD, 0xFFBB66, 0xAA9966 } },	// Mellow Jazz
		};

		public static void color( ChannelPage page, int idChannel )
		{
			// if( true || Global.isLightUiTheme )
			if( Global.isLightUiTheme )
			{
				// All my custom coloring is for dark theme exclusively.
				page.color( "colorPlaying", 0x335588 );
				page.color( "colorStatus", 0x332277 );
				return;
			}

			uint[] arr;
			if( s_colors.TryGetValue( idChannel, out arr ) )
				page.setColors( arr );
		}
	}
}