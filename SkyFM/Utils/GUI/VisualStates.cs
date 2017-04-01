using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Expression.Interactivity.Core;
using System.Diagnostics;
using System.Linq;

namespace XamlExtrensions
{
	/// <summary>This static class implements the bindable CurrentState property.</summary>
	public static class VisualStates
	{
		public static readonly DependencyProperty CurrentStateProperty =
           DependencyProperty.RegisterAttached( "CurrentState", typeof( String ), typeof( VisualStates ), new PropertyMetadata( TransitionToState ) );

		public static string GetCurrentState( DependencyObject obj )
		{
			return (string)obj.GetValue( CurrentStateProperty );
		}

		public static void SetCurrentState( DependencyObject obj, string value )
		{
			obj.SetValue( CurrentStateProperty, value );
		}

		private static void TransitionToState( object sender, DependencyPropertyChangedEventArgs args )
		{
			FrameworkElement elt = sender as FrameworkElement;
			if( null == elt )
				throw new ArgumentException( "CurrentState is only supported on the FrameworkElement" );

			string newState = args.NewValue.ToString();
			SkyFM.SharedUtils.startOnGuiThread( () =>
			{
				// Debug code below
				/* var groups = VisualStateManager.GetVisualStateGroups( elt )
					.Cast<VisualStateGroup>()
					.Select( vsg => vsg.Name )
					.ToArray();
				Debug.WriteLine( "VSM: Going to {0}, groups {1}", newState, String.Join( ", ", groups ) ); */
				if( !ExtendedVisualStateManager.GoToElementState( elt, newState, true ) )
					Debug.WriteLine( "VSM: failed to transition to state {0}.", newState );
			} );
		}
	}
}