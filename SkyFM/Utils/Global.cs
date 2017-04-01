using System;
using System.Windows;
using System.Text.RegularExpressions;
using Microsoft.Phone.Controls;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Net;
using System.Reflection;
using Microsoft.Phone.Tasks;
using SkyFM;
using SkyFM.Model.JSON;

internal static partial class Global
{
	static readonly Regex s_nameWithExt = new Regex( @"\.\w{1,5}$", RegexOptions.Compiled );

	/// <summary>If the file name does not include an extension, append the specified extension; otherwise just return the name.</summary>
	/// <param name="name"></param>
	/// <param name="ext"></param>
	/// <returns></returns>
	public static string addExtensionIfNotThere( this string name, string ext )
	{
		if( String.IsNullOrEmpty( ext ) )
			return name;
		if( s_nameWithExt.IsMatch( name ) )
			return name;
		if( !ext.StartsWith( "." ) )
			ext = "." + ext;
		return name + ext;
	}

	const string removePreviousParam = "removePrevious";

	/// <summary>Construct the URI for the screen XAML.</summary>
	/// <param name="xamlName">Page file name without XAML</param>
	/// <returns>Uri pointing to "/Screens/Icons/{0}.xaml"</returns>
	public static Uri pageUri( string xamlName, bool bRemovePrevious )
	{
		StringBuilder sb = new StringBuilder( 64 );
		sb.AppendFormat( "/Pages/{0}", xamlName.addExtensionIfNotThere( "xaml" ) );
		if( bRemovePrevious )
			sb.AppendFormat( "?{0}=1", removePreviousParam );
		return new Uri( sb.ToString(), UriKind.Relative );
	}

	public static Uri pageUri( string xamlName )
	{
		return pageUri( xamlName, false );
	}

	public static Uri pageUri<tPage>()
	{
		return pageUri<tPage>( false );
	}

	public static Uri pageUri<tPage>( bool bRemovePrevious )
	{
		string xaml = typeof( tPage ).Name;
		return pageUri( xaml, bRemovePrevious );
	}

	/// <summary>If "removePrevious=1" param present in the page URI, remove the prev.page from the back stack.</summary>
	/// <param name="page"></param>
	/// <returns></returns>
	public static bool removePrevPageIfRequested( this PhoneApplicationPage page )
	{
		bool remove = false;
		if( page.NavigationContext.QueryString.ContainsKey( removePreviousParam ) )
		{
			remove = ( "1" == page.NavigationContext.QueryString[ removePreviousParam ] );
			page.NavigationContext.QueryString.Remove( removePreviousParam );
		}

		if( remove )
			page.NavigationService.RemoveBackEntry();
		return remove;
	}

	/// <summary>True if the code is running in the context of the form designer.</summary>
	public static bool inFormDesigner
	{ get {
		// return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
		// return DesignerProperties.GetIsInDesignMode( Application.Current.RootVisual );
		return DesignerProperties.IsInDesignTool;
	} }

	// http://stackoverflow.com/a/221941/126995
	public static byte[] ReadFully(  this Stream input )
	{
		byte[] buffer = new byte[ 4 * 1024 ];
		using( MemoryStream ms = new MemoryStream() )
		{
			int read;
			while( ( read = input.Read( buffer, 0, buffer.Length ) ) > 0 )
				ms.Write( buffer, 0, read );
			return ms.ToArray();
		}
	}

	public static bool newerThen( this DateTime d, TimeSpan ts )
	{
		return DateTime.UtcNow - d <= ts;
	}

	public static bool olderThen( this DateTime d, TimeSpan ts )
	{
		return !d.newerThen( ts );
	}

	// http://stackoverflow.com/a/1987721/126995

	static double RoundSignificantDigits( double value, int sigFigures, out int roundingPosition )
	{
		// this method will return a rounded double value at a number of significant figures.
		// the sigFigures parameter must be between 0 and 15, exclusive.

		roundingPosition = 0;

		// ToDo: Might want to compare with epsilon here
		if( value == 0.0d )
		{
			return value;
		}

		if( double.IsNaN( value ) )
		{
			return double.NaN;
		}

		if( double.IsPositiveInfinity( value ) )
		{
			return double.PositiveInfinity;
		}

		if( double.IsNegativeInfinity( value ) )
		{
			return double.NegativeInfinity;
		}

		if( sigFigures < 1 || sigFigures > 14 )
		{
			throw new ArgumentOutOfRangeException( "The sigFigures argument must be between 0 and 15 exclusive." );
		}

		// The resulting rounding position will be negative for rounding at whole numbers, and positive for decimal places.
		roundingPosition = sigFigures - 1 - (int)( Math.Floor( Math.Log10( Math.Abs( value ) ) ) );

		// try to use a rounding position directly, if no scale is needed.
		// this is because the scale multiplication after the rounding can introduce error, although 
		// this only happens when you're dealing with really tiny numbers, i.e 9.9e-14.
		if( roundingPosition > 0 && roundingPosition < 15 )
		{
			return Math.Round( value, roundingPosition );
		}

		// Shouldn't get here unless we need to scale it.
		// Set the scaling value, for rounding whole numbers or decimals past 15 places
		double scale = Math.Pow( 10, Math.Ceiling( Math.Log10( Math.Abs( value ) ) ) );

		return Math.Round( value / scale, sigFigures ) * scale;
	}

	public static double RoundSignificantDigits( this double value, int sigFigures )
	{
		int unneededRoundingPosition;
		return RoundSignificantDigits( value, sigFigures, out unneededRoundingPosition );
	}

	public static string ToString( this double value, int sigFigures )
	{
		// this method will round and then append zeros if needed.
		// i.e. if you round .002 to two significant figures, the resulting number should be .0020.

		var currentInfo = CultureInfo.CurrentCulture.NumberFormat;

		// ToDo: Might want to compare with epsilon here
		if( value == 0.0d )
		{
			return 0.0d.ToString( currentInfo );
		}

		if( double.IsNaN( value ) )
		{
			return currentInfo.NaNSymbol;
		}

		if( double.IsPositiveInfinity( value ) )
		{
			return currentInfo.PositiveInfinitySymbol;
		}

		if( double.IsNegativeInfinity( value ) )
		{
			return currentInfo.NegativeInfinitySymbol;
		}

		int roundingPosition = 0;
		double roundedValue = RoundSignificantDigits( value, sigFigures, out roundingPosition );

		// If the above rounding evaluates to zero, just return zero without padding.
		// Todo:  Might want to compare with epsilon here
		// Todo:  Consider whether it's more correct to return zero at a certain precision here (ie. .000 or zero at 3 sig figs).  For my purposes zero is zero.
		if( roundedValue == 0.0d )
		{
			return 0.0d.ToString( currentInfo );
		}

		// Check if the rounding position is positive and is not past 100 decimal places.
		// If the rounding position is greater than 100, string.format won't represent the number correctly.
		// ToDo:  What happens when the rounding position is greater than 100?
		if( roundingPosition > 0 && roundingPosition < 100 )
		{
			return string.Format( currentInfo, "{0:F" + roundingPosition + "}", roundedValue );
		}

		// Shouldn't get here unless it's a whole number
		// String.format is only needed when dealing with decimals (whole numbers won't need to be padded with zeros to the right.)
		return roundedValue.ToString( currentInfo );
	}

	public static void delay( Action act )
	{
		Deployment.Current.Dispatcher.BeginInvoke( act );
	}

	public static bool isLightUiTheme { get {
		var v = (Visibility)Application.Current.Resources[ "PhoneLightThemeVisibility" ];
		return v == Visibility.Visible;
	} }

	static Random s_rand = null;
	/// <summary></summary>
	/// <remarks>This method is not thread safe.</remarks>
	/// <param name="probability">probability of "true" result</param>
	/// <returns></returns>
	public static bool dbgRandTest( double probability )
	{
		if( probability <= 0 ) return false;
		if( probability >= 1 ) return true;
		if( null == s_rand )
			s_rand = new Random();
		return s_rand.NextDouble() <= probability;
	}

	private static void onPrevCrash( string contents )
	{
		if( SkyFM.Model.AppSettings.noCrashLogMails )
			return;

		if( MessageBox.Show( STR.sendreport_message_text, STR.sendreport_message_title, MessageBoxButton.OKCancel ) == MessageBoxResult.OK )
		{
			EmailComposeTask email = new EmailComposeTask();
			email.To = "soonts@gmail.com";
			email.Subject = STR.sendreport_message_mailSubject;
			email.Body = contents;
			email.Show();
		}
		else
			SkyFM.Model.AppSettings.noCrashLogMails = true;
	}

	public static bool checkCrashLog()
	{
		return SkyFM.ExceptionLogger.onPrevCrash( onPrevCrash );
	}

	// http://pwnedcode.wordpress.com/2009/05/19/fast-enum-tryparse-implementation/
	public static bool EnumTryParse<T>( this string enumText, out T enumValue ) where T: struct
	{
		enumValue = default( T );
		if( Enum.IsDefined( typeof( T ), enumText ) )
		{
			enumValue = (T)Enum.Parse( typeof( T ), enumText, true );
			return true;
		}
		return false;
	}

	public static T EnumParseOrDefault<T>( this string enumText, T defaultValue ) where T: struct
	{
		T res = defaultValue;
		if( !enumText.EnumTryParse( out res ) )
			return defaultValue;
		return res;
	}

	public static Uri uriFromString( string s )
	{
		if( String.IsNullOrEmpty( s ) )
			return null;
		if( s.StartsWith( "//" ) )
			s = Endpoints.strProtocol + s;
		return new UriBuilder( s ).Uri;
	}
}