using System;
using GalaSoft.MvvmLight;
using System.Threading.Tasks;

namespace SkyFM.ViewModel
{
	/// <summary>Possible delay loading status of an object. Could be used as a VisualState</summary>
	public enum eDelayLoadedStatus: byte
	{
		NotFetched,
		RequestSent,
		Fetched,
		Failed,
	}

	public abstract class DelayLoadedViewModel: ViewModelBase
	{
		protected DelayLoadedViewModel()
		{
			if( IsInDesignMode )
				this._visualState = eDelayLoadedStatus.Fetched;
		}

		/// <summary>The <see cref="visualState" /> property's name.</summary>
		protected const string visualStatePropertyName = "visualState";
		private eDelayLoadedStatus _visualState = eDelayLoadedStatus.NotFetched;
		/// <summary>Gets the visualState property.</summary>
		public eDelayLoadedStatus visualState
		{
			get { return _visualState; }
			set
			{
				if( _visualState == value )
					return;
				_visualState = value;
				RaisePropertyChanged( visualStatePropertyName );
				// Global.startOnGuiThread( () => RaisePropertyChanged( visualStatePropertyName ) );
			}
		}

		/// <summary>The <see cref="failReason" /> property's name.</summary>
		public const string failReasonPropertyName = "failReason";
		private string _failReason = null;
		/// <summary>Gets the failReason property.</summary>
		public string failReason
		{
			get { return _failReason; }
			set
			{
				if( _failReason == value )
					return;
				_failReason = value;
				RaisePropertyChanged( failReasonPropertyName );
				// Global.startOnGuiThread( () => RaisePropertyChanged( failReasonPropertyName ) );
			}
		}

		protected abstract Task loadAsyncImpl();

		public Task loadAsync()
		{
			return loadAsync( false );
		}

		public async Task loadAsync( bool bForceReload )
		{
			switch( this.visualState )
			{
				case eDelayLoadedStatus.RequestSent:
					return;
				case eDelayLoadedStatus.Fetched:
					if( !bForceReload )
						return;
					break;
			}

			try
			{
				this.visualState = eDelayLoadedStatus.RequestSent;
				await this.loadAsyncImpl();
				this.failReason = null;
				this.visualState = eDelayLoadedStatus.Fetched;
			}
			catch( Exception ex )
			{
				this.failReason = ex.userFriendlyMessage();
				this.visualState = eDelayLoadedStatus.Failed;
				throw;
			}
		}
	}
}