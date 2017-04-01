using System;
using Microsoft.Phone.Controls;

namespace SkyFM
{
	/// <summary>Description for AboutPage.</summary>
	public partial class AboutPage: XamlExtrensions.Page
	{
		/// <summary>Initializes a new instance of the AboutPage class.</summary>
		public AboutPage()
		{
			InitializeComponent();

			// Version text
			string strVersion = SharedUtils.getCurrentVersion().ToString();
			while( strVersion.EndsWith( ".0" ) )
				strVersion = strVersion.Substring( 0, strVersion.Length - 2 );

			string strBuildDate = Global.buildDateTimeUtc.ToLocalTime().ToString( "d" );

			string versionString = String.Format( STR.about_version_fmt, strVersion, strBuildDate );

			this.tbVersion.Text = versionString;

			// Rich text
			this.rtTrademark.setRichText( STR.about_trademark_richtext );
			this.rtThirdparty.setRichText( STR.about_thirdparty_richtext );
			this.rtLocalization.setRichText( STR.about_localization_richtext );
		}
	}
}