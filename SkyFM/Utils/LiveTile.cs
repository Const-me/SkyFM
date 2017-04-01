using System;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Linq;
using Microsoft.Phone.Shell;
using Kawagoe.Storage;

namespace SkyFM
{
	static class LiveTile
	{
		const string liveTilesFolder = @"Shared\ShellContent";
		static string getFileName( int idChannel )
		{
			return String.Format( @"{0}\{1}.jpg", liveTilesFolder, idChannel );
		}
		static Uri getFileUri( int idChannel )
		{
			string str = String.Format( "isostore:/Shared/ShellContent/{0}.jpg", idChannel );
			return new Uri( str, UriKind.Absolute );
		}

		static bool prepareLiveTileImage( int idChannel, Uri imageUri )
		{
			string fileName = getFileName( idChannel );
			using( var store = IsolatedStorageFile.GetUserStoreForApplication() )
			{
				if( store.FileExists( fileName ) )
					return true;	// Already have the resized file
				if( !store.DirectoryExists( liveTilesFolder ) )
					store.CreateDirectory( liveTilesFolder );
			}

			WriteableBitmap bmp = ImageCache.Default.GetWriteable( imageUri );
			if( null == bmp )
				return false;	// Don't have the image
			// bmp = bmp.Resize( 173, 173, WriteableBitmapExtensions.Interpolation.Bilinear );
			bmp = bmp.ResizeBicubic( 173, 173 );

			FileSystem.writeFile( fileName, stm =>
			{
				bmp.SaveJpeg( stm, 173, 173, 0, 100 );
			} );
			return true;
		}

		public static bool createTile( int idChannel, Uri imageUri, string caption )
		{
			if( !prepareLiveTileImage( idChannel, imageUri ) )
				return false;
			StandardTileData data = new StandardTileData()
			{
				BackBackgroundImage = getFileUri(idChannel),
				Title = caption,
			};
			ShellTile.ActiveTiles.First().Update( data );
			return true;
		}
	}
}