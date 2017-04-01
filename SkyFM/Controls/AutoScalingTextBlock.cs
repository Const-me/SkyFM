//-----------------------------------------------------------------------
// <copyright file="AutoScalingTextBlock.cs" company="Edward McLeod-Jones">
//    Copyright (c) 2010 Edward McLeod-Jones
//
//    Permission is hereby granted, free of charge, to any person
//    obtaining a copy of this software and associated documentation
//    files (the "Software"), to deal in the Software without
//    restriction, including without limitation the rights to use,
//    copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the
//    Software is furnished to do so, subject to the following
//    conditions:
//
//    The above copyright notice and this permission notice shall be
//    included in all copies or substantial portions of the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//    OTHER DEALINGS IN THE SOFTWARE.
//
//    Read more about the MIT License for software at it's wikipedia
//    page here: http://en.wikipedia.org/wiki/MIT_License
// 
//    If you happen to use this code in a project, please email me
//    edward.mcleodjones@gmail.com and let me know how and where you use 
//    it. That is, after all, the fun part of sharing code: knowing
//    that it gets used.
//   
//    Find out more about me and view my portfolio at http://edventuro.us/.
// </copyright>
//-----------------------------------------------------------------------

namespace XamlExtrensions
{
	using System;
	using System.Net;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Markup;

	/// <summary>
	/// A simple text control that shrinks or expands the font of the text to
	/// display all of the text in the preferred size of the textblock.
	/// Dependency Properties:
	/// - MinFontSize: This is the smallest size the font will be reduced to. Defaults to 8pt.
	/// - MaxFontSize: This is the largest size the font will be increased to. Defaults to 20.
	/// - ScalingMode: This controls if the font size should be scaled only up, or down, or both ways
	/// to fit the text within the boundaries of the textbox.  Defaults to BothWays.
	/// - StepSize:    This is the point size the font will be increased or decreased 
	/// by each iteration until the text fits the desired size. Higher amounts will require fewer iterations,
	/// so will be faster, but the changes will be more abrupt. Defaults to 0.5.
	/// </summary>
	public class AutoScalingTextBlock: ContentControl
	{
		#region Text (DependencyProperty)

		/// <summary>
		/// Gets or sets the Text DependencyProperty. This is the text that will be displayed.
		/// </summary>
		public string Text
		{
			get { return (string)GetValue( TextProperty ); }
			set { SetValue( TextProperty, value ); }
		}
		public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register( "Text", typeof( string ), typeof( AutoScalingTextBlock ),
			new PropertyMetadata( null, new PropertyChangedCallback( OnTextChanged ) ) );

		private static void OnTextChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			( (AutoScalingTextBlock)d ).OnTextChanged( e );
		}

		protected virtual void OnTextChanged( DependencyPropertyChangedEventArgs e )
		{
			this.InvalidateMeasure();
		}

		#endregion

		#region MinFontSize (DependencyProperty)

		private double _minFontSize = 8d;

		/// <summary>
		/// Gets or sets the MinFontSize property. This is the smallest size the font will be reduced to. Defaults to 8pt.
		/// </summary>
		public double MinFontSize
		{
			get { return (double)GetValue( MinFontSizeProperty ); }
			set { SetValue( MinFontSizeProperty, value ); }
		}
		public static readonly DependencyProperty MinFontSizeProperty =
            DependencyProperty.Register( "MinFontSize", typeof( double ), typeof( AutoScalingTextBlock ),
			new PropertyMetadata( 8d, new PropertyChangedCallback( OnMinFontSizeChanged ) ) );

		private static void OnMinFontSizeChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			( (AutoScalingTextBlock)d ).OnMinFontSizeChanged( e );
		}

		protected virtual void OnMinFontSizeChanged( DependencyPropertyChangedEventArgs e )
		{
			_minFontSize = (double)e.NewValue;
			this.InvalidateMeasure();
		}

		#endregion

		#region MaxFontSize (DependencyProperty)

		private double _maxFontSize = 20d;

		/// <summary>
		/// Gets or sets the MaxFontSize property. This is the largest size the font will be increased to. Defaults to 20.
		/// </summary>
		public double MaxFontSize
		{
			get { return (double)GetValue( MaxFontSizeProperty ); }
			set { SetValue( MaxFontSizeProperty, value ); }
		}
		public static readonly DependencyProperty MaxFontSizeProperty =
            DependencyProperty.Register( "MaxFontSize", typeof( double ), typeof( AutoScalingTextBlock ),
			new PropertyMetadata( 20d, new PropertyChangedCallback( OnMaxFontSizeChanged ) ) );

		private static void OnMaxFontSizeChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			( (AutoScalingTextBlock)d ).OnMaxFontSizeChanged( e );
		}

		protected virtual void OnMaxFontSizeChanged( DependencyPropertyChangedEventArgs e )
		{
			_maxFontSize = (double)e.NewValue;
			this.InvalidateMeasure();
		}

		#endregion

		#region ScalingMode (DependencyProperty)

		public enum ScalingModeOptions { BothWays, UpOnly, DownOnly };

		private ScalingModeOptions _scalingMode = ScalingModeOptions.BothWays;

		/// <summary>
		/// Gets or sets the ScalingMode property. This controls if the font size should be scaled only up, or down, or both ways
		/// to fit the text within the boundaries of the textbox.  Defaults to BothWays.
		/// </summary>
		public ScalingModeOptions ScalingMode
		{
			get { return (ScalingModeOptions)GetValue( ScalingModeProperty ); }
			set { SetValue( ScalingModeProperty, value ); }
		}
		public static readonly DependencyProperty ScalingModeProperty =
            DependencyProperty.Register( "ScalingMode", typeof( ScalingModeOptions ), typeof( AutoScalingTextBlock ),
			new PropertyMetadata( ScalingModeOptions.BothWays, new PropertyChangedCallback( OnScalingModeChanged ) ) );

		private static void OnScalingModeChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			( (AutoScalingTextBlock)d ).OnScalingModeChanged( e );
		}

		protected virtual void OnScalingModeChanged( DependencyPropertyChangedEventArgs e )
		{
			// _scalingMode = (ScalingModeOptions) Enum.Parse(typeof(ScalingModeOptions), (string) e.NewValue, true);
			_scalingMode = (ScalingModeOptions)e.NewValue;
			this.InvalidateMeasure();
		}

		#endregion

		#region Wrap (DependencyProperty)
		private TextWrapping _textWrapping = TextWrapping.NoWrap;

		public TextWrapping TextWrapping
		{
			get { return (TextWrapping)GetValue( TextWrappingProperty ); }
			set { SetValue( TextWrappingProperty, value ); }
		}
		public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register( "TextWrapping", typeof( TextWrapping ), typeof( AutoScalingTextBlock ),
			new PropertyMetadata( TextWrapping.NoWrap, new PropertyChangedCallback( OnTextWrappingChanged ) ) );

		private static void OnTextWrappingChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var astb = ( (AutoScalingTextBlock)d );
			astb._textWrapping = (TextWrapping)e.NewValue;
			astb.InvalidateMeasure();
		}
		#endregion

		/* #region StepSize (DependencyProperty)

		private double _stepSize = 0.5d;

		/// <summary>
		/// Gets or sets the StepSize property. This is the point size the font will be increased or decreased 
		/// by each iteration until the text fits the desired size. Higher amounts will require fewer iterations,
		/// so will be faster, but the changes will be more abrupt. Defaults to 0.5.
		/// </summary>
		public double StepSize
		{
			get { return (double)GetValue( StepSizeProperty ); }
			set { SetValue( StepSizeProperty, value ); }
		}
		public static readonly DependencyProperty StepSizeProperty =
            DependencyProperty.Register( "StepSize", typeof( double ), typeof( AutoScalingTextBlock ),
			new PropertyMetadata( 0.5d, new PropertyChangedCallback( OnStepSizeChanged ) ) );

		private static void OnStepSizeChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			( (AutoScalingTextBlock)d ).OnStepSizeChanged( e );
		}

		protected virtual void OnStepSizeChanged( DependencyPropertyChangedEventArgs e )
		{
			_stepSize = (double)e.NewValue;
			this.InvalidateMeasure();
		}

		#endregion */

		/// <summary>
		/// A TextBlock that is set as the control's content and is ultimately the control 
		/// that displays our text
		/// </summary>
		private TextBlock textBlock;

		/// <summary>
		/// Initializes a new instance of the DynamicTextBlock class
		/// </summary>
		public AutoScalingTextBlock()
		{
			// Create our TextBlock and initialize
			this.textBlock = new TextBlock();
			this.Content = this.textBlock;

			// Force TextWrapping on
			// this.textBlock.TextWrapping = TextWrapping.Wrap;
			this.textBlock.TextWrapping = TextWrapping.NoWrap;
		}

		const double stepUp = 1.125;
		const double stepDown = 1.0 / stepUp;



		/// <summary>
		/// Handles the measure part of the measure and arrange layout process. During this process
		/// we measure the textBlock that we've created as content with increasingly bigger/smaller font sizes
		/// until we find the font size that fits.
		/// </summary>
		/// <param name="availableSize">The available size</param>
		/// <returns>The base implementation of Measure</returns>
		protected override Size MeasureOverride( Size availableSize )
		{
			bool isMultiline = ( _textWrapping == TextWrapping.Wrap );

			double width = isMultiline ? availableSize.Width : double.PositiveInfinity;
			Size unboundSize = new Size( width, double.PositiveInfinity );

			// Set the text and measure it to see if it fits without alteration
			this.textBlock.Text = this.Text;
			this.textBlock.FontSize = this.FontSize;
			this.textBlock.TextWrapping = this.TextWrapping;

			Size textSize = base.MeasureOverride( unboundSize );

			Func<bool> isFit = null;
			if( isMultiline )
				isFit = () => ( textSize.Height < availableSize.Height );
			else
				isFit = () => ( textSize.Height < availableSize.Height && textSize.Width < availableSize.Width );

			// Scale up first if necessary
			while( isFit() )
			{
				// Increase the font size
				// this.textBlock.FontSize = this.textBlock.FontSize + _stepSize;
				this.textBlock.FontSize *= stepUp;
				textSize = base.MeasureOverride( unboundSize );

				if( _scalingMode == ScalingModeOptions.DownOnly )
				{
					if( this.textBlock.FontSize > this.FontSize )
					{
						this.textBlock.FontSize = this.FontSize;
						break;
					}
				}

				if( this.textBlock.FontSize >= _maxFontSize )
				{
					this.textBlock.FontSize = _maxFontSize;
					break;
				}
			}

			// Then scale down if neccessary
			while( !isFit() )
			{
				// Reduce the font size
				// this.textBlock.FontSize = this.textBlock.FontSize - _stepSize;
				this.textBlock.FontSize *= stepDown;
				textSize = base.MeasureOverride( unboundSize );

				if( _scalingMode == ScalingModeOptions.UpOnly )
				{
					if( this.textBlock.FontSize < this.FontSize )
					{
						this.textBlock.FontSize = this.FontSize;
						break;
					}
				}

				if( this.textBlock.FontSize <= _minFontSize )
				{
					this.textBlock.FontSize = _minFontSize;
					break;
				}
			}

			return base.MeasureOverride( availableSize );
		}
	}
}