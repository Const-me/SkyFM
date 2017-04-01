using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using SkyFM;
using System.Runtime.CompilerServices;

internal static partial class Global
{
	/// <summary>Check if the exception is/has a TaskCanceledException.</summary>
	/// <param name="ex"></param>
	/// <returns></returns>
	public static bool shouldBeIgnored( this Exception ex )
	{
		if( ex is TaskCanceledException )
			return true;
		if( ex is AggregateException )
		{
			var a = ex as AggregateException;
			a.Flatten();
			return a.InnerExceptions.Any( e => e is TaskCanceledException );
		}
		return false;
	}

	// http://stackoverflow.com/a/12678318/126995
	public static void IgnoreExceptions( this Task task )
	{
		if( task.IsCompleted )
		{
			if( task.IsFaulted )
			{
				var dummy = task.Exception;
			}
			return;
		}

		task.ContinueWith( c => { var dummy = c.Exception; },
			TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously );
	}

	static string ToReadableString( this TimeSpan ts )
	{
		long totalTicks = Math.Abs(ts.Ticks);
		if( 0 == totalTicks )
			return "zero";
		if( totalTicks < TimeSpan.TicksPerSecond )
			return String.Format( "{0:G4} ms", ts.TotalMilliseconds );	// less then 1 sec
		if( totalTicks < TimeSpan.TicksPerMinute )
			return String.Format( "{0:G4} sec", ts.TotalSeconds );	// less then 1 min
		return ts.ToString();
	}

	public static readonly TimeSpan pendingActionFrequency = TimeSpan.FromMilliseconds( 444 );

	/// <summary>Add timeout, cancellation and pending support to a long running task that doesn't support it.</summary>
	public static async Task<tResult> addTimeoutAndProgress<tResult>( this Task<tResult> tResponse, TimeSpan timeout, CancellationToken cancelToken, Action actPending, Action actTimeout )
	{
		TimeSpan delayLeft = timeout;

		if( tResponse.IsCompleted )
			return tResponse.Result;

		while( delayLeft.Ticks > 0 )
		{
			// Determine sleep time.
			TimeSpan toWait = delayLeft;
			if( null != actPending && toWait > pendingActionFrequency )
				toWait = pendingActionFrequency;

			// Create sleep task
			Task tDelay = TaskEx.Delay( toWait, cancelToken );

			// Wait for either sleep or target task to complete.
			Exception awaitException = null;
			try
			{
				await TaskEx.WhenAny( tResponse, tDelay ).ConfigureAwait( false );
			}
			catch (System.Exception ex)
			{
				if( !ex.shouldBeIgnored() )
				{
					tResponse.IgnoreExceptions();
					tDelay.IgnoreExceptions();
					throw;
				}
				awaitException = ex;
			}

			if( tResponse.IsCompleted )
			{
				// tResponse completed
				tDelay.IgnoreExceptions();
				return tResponse.Result;
			}

			if( null != awaitException || cancelToken.IsCancellationRequested )
			{
				// Cancelled.
				tResponse.IgnoreExceptions();
				tDelay.IgnoreExceptions();
				return default( tResult );
			}

			if( null != actPending )
			{
				try
				{
					actPending();
				}
				catch( System.Exception ex )
				{
					ex.log( "pending action failed" );
					tResponse.IgnoreExceptions();
					tDelay.IgnoreExceptions();
					throw;
				}
			}
			delayLeft -= toWait;
		}

		Logger.warning( "Throwing a timeout exception, the timeout was {0}", timeout.ToReadableString() );

		tResponse.IgnoreExceptions();
		if( null != actTimeout )
		{
			try
			{
				actTimeout();
			}
			catch (System.Exception ex)
			{
				ex.logWarning( "timeout finalizer failed" );
			}
		}

		throw new TimeoutException();
	}

	/// <summary>Add timeout + cancellation support to a task.</summary>
	/// <remarks>Unlike addTimeoutAndProgress, it doesn't split timeout into smaller intervals to report status.</remarks>
	public static async Task<tResult> addTimeout<tResult>( this Task<tResult> tResponse, TimeSpan timeout, CancellationToken cancelToken )
	{
		if( tResponse.IsCompleted )
			return tResponse.Result;

		// Create sleep task
		Task tDelay = TaskEx.Delay( timeout, cancelToken );

		// Wait for either sleep or target task to complete.
		Exception awaitException = null;
		try
		{
			await TaskEx.WhenAny( tResponse, tDelay ).ConfigureAwait( false );
		}
		catch( System.Exception ex )
		{
			if( !ex.shouldBeIgnored() )
			{
				tResponse.IgnoreExceptions();
				tDelay.IgnoreExceptions();
				throw;
			}
			awaitException = ex;
		}

		if( tResponse.IsCompleted )
		{
			// tResponse completed
			tDelay.IgnoreExceptions();
			return tResponse.Result;
		}

		if( null != awaitException || cancelToken.IsCancellationRequested )
		{
			// Cancelled.
			tResponse.IgnoreExceptions();
			tDelay.IgnoreExceptions();
			return default( tResult );
		}
		tResponse.IgnoreExceptions();

		Logger.warning( "Throwing a timeout exception, the timeout was {0}", timeout.ToReadableString() );
		throw new TimeoutException();
	}

	public static Task<tResult> addTimeout<tResult>( this Task<tResult> tResponse, TimeSpan timeout )
	{
		return tResponse.addTimeout( timeout, CancellationToken.None );
	}
}