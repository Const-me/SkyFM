using System;
using System.Threading.Tasks;
using System.Windows;

namespace SkyFM
{
	public static class msgbox
	{
		public static async Task error( string txt )
		{
			MessageBox.Show( txt );
			await TaskEx.Yield();
		}
	}
}