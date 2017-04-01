using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace SkyFM.shared
{
	/// <summary>This class implements read/writes routines for the isolated storage files.</summary>
	public static class FileSystem
	{
		public static bool readFile( string fileName, Action<Stream> actRead )
		{
			return readFile( fileName, actRead, false );
		}

		public static bool readFile( string fileName, Action<Stream> actRead, bool removeAfter )
		{
			using( IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication() )
			{
				if( !storage.FileExists( fileName ) )
					return false;
				using( IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream( fileName, FileMode.Open, storage ) )
					actRead( fileStream );
				if( removeAfter )
				{
					try
					{
						storage.DeleteFile( fileName );
					}
					catch (System.Exception ex)
					{
						ex.logWarning( "Unable to delete file {0}", fileName );
					}
				}
				return true;
			}
		}

		public static void writeFile( string fileName, Action<Stream> actWrite )
		{
			using( IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication() )
			{
				using( IsolatedStorageFileStream fs = new IsolatedStorageFileStream( fileName, FileMode.Create, storage ) )
				{
					actWrite( fs );
					fs.Close();
				}
			}
		}

		public static bool readText( string fileName, Action<StreamReader> actRead )
		{
			return readText( fileName, actRead, false );
		}

		public static bool readText( string fileName, Action<StreamReader> actRead, bool removeAfter )
		{
			return readFile( fileName, binStream =>
			{
				using( StreamReader txtReader = new StreamReader( binStream ) )
					actRead( txtReader );
			}, removeAfter );
		}

		public static void writeText( string fileName, Action<TextWriter> actWrite )
		{
			writeFile( fileName, binStream =>
			{
				using( StreamWriter textStream = new StreamWriter( binStream ) )
				{
					actWrite( textStream );
				}
			} );
		}
	}
}