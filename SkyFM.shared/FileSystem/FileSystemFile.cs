using System;
using System.IO;
using System.Threading;

namespace SkyFM.shared
{
	/// <summary>This class implements a file in the isolated storage guarder by the named mutex.</summary>
	/// <remarks>
	/// <para>This class is only useful for IPC. For single process, normal lock() is much more efficient. That's why its internal to this shared assembly.</para>
	/// <para>The mutex is created on demand, if you're not reading/writing, this class brings almost zero overhead.</para>
	/// </remarks>
	internal class FileSystemFile
	{
		readonly string m_fileName, m_mutexName;
		Mutex m_mutex;
		public FileSystemFile( string file, string mutex )
		{
			m_fileName = file;
			m_mutexName = mutex;
		}

		void waitMutex()
		{
			if( null == m_mutex )
				m_mutex = new Mutex( false, m_mutexName );
			m_mutex.WaitOne();
		}

		public bool readText( Action<StreamReader> actStream )
		{
			return this.readText( actStream, false );
		}

		public bool readText( Action<StreamReader> actStream, bool removeAfter )
		{
			waitMutex();
			try
			{
				return FileSystem.readText( m_fileName, actStream, removeAfter );
			}
			finally
			{
				m_mutex.ReleaseMutex();
			}
		}

		public void writeText( Action<TextWriter> actWrite )
		{
			waitMutex();
			try
			{
				FileSystem.writeText( m_fileName, actWrite );
			}
			finally
			{
				m_mutex.ReleaseMutex();
			}
		}

		public string readString( bool removeAfter )
		{
			string contents = null;
			this.readText( reader =>
			{
				contents = reader.ReadToEnd();
			},
			removeAfter );
			return contents;
		}

		public void writeString( string contents )
		{
			this.writeText( tw => tw.Write( contents ) );
		}
	}
}