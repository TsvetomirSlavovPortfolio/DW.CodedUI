﻿#region License
/*--------------------------------------------------------------------------------
	Copyright (c) 2009-2012 David Wendland

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
--------------------------------------------------------------------------------*/
#endregion License

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AutomationElementFinder
{
	/// <summary>
	/// Represents an item control which has a title on the left side
	/// </summary>
	public class TitledItem : ContentControl
	{
		static TitledItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TitledItem), new FrameworkPropertyMetadata(typeof(TitledItem)));
		}

		/// <summary>
		/// Gets or sets the title of the control
		/// </summary>
		/// <value>If not set: null</value>
		[Category("Common Properties")]
		[Description("Gets or sets the title of the control")]
		[DefaultValue(null)]
		public object Title
		{
			get { return (object)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		/// <summary>
		/// Identifies the Title dependency property
		/// </summary>
		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(object), typeof(TitledItem), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets or sets the VerticalAlignment of the title
		/// </summary>
		/// <value>If not set: VerticalAlignment.Center</value>
		[Category("Common Properties")]
		[Description("Gets or sets the VerticalAlignment of the title")]
		[DefaultValue(VerticalAlignment.Center)]
		public VerticalAlignment VerticalTitleAlignment
		{
			get { return (VerticalAlignment)GetValue(VerticalTitleAlignmentProperty); }
			set { SetValue(VerticalTitleAlignmentProperty, value); }
		}

		/// <summary>
		/// Identifies the VerticalTitleAlignment dependency property
		/// </summary>
		public static readonly DependencyProperty VerticalTitleAlignmentProperty =
			DependencyProperty.Register("VerticalTitleAlignment", typeof(VerticalAlignment), typeof(TitledItem), new UIPropertyMetadata(VerticalAlignment.Center));

		/// <summary>
		/// Gets or sets the HorizontalAlignment of the title
		/// </summary>
		/// <value>If not set: HorizontalAlignment.Right</value>
		[Category("Common Properties")]
		[Description("Gets or sets the HorizontalAlignment of the title")]
		[DefaultValue(HorizontalAlignment.Right)]
		public HorizontalAlignment HorizontalTitleAlignment
		{
			get { return (HorizontalAlignment)GetValue(HorizontalTitleAlignmentProperty); }
			set { SetValue(HorizontalTitleAlignmentProperty, value); }
		}

		/// <summary>
		/// Identifies the HorizontalTitleAlignment dependency property
		/// </summary>
		public static readonly DependencyProperty HorizontalTitleAlignmentProperty =
			DependencyProperty.Register("HorizontalTitleAlignment", typeof(HorizontalAlignment), typeof(TitledItem), new UIPropertyMetadata(HorizontalAlignment.Right));

		/// <summary>
		/// Gets or sets the margin of the title
		/// </summary>
		/// <value>If not set: new Thickness(5, 0, 5, 0)</value>
		[Category("Common Properties")]
		[Description("Gets or sets the margin of the title")]
		public Thickness TitleMargin
		{
			get { return (Thickness)GetValue(TitleMarginProperty); }
			set { SetValue(TitleMarginProperty, value); }
		}

		/// <summary>
		/// Identifies the TitleMargin dependency property
		/// </summary>
		public static readonly DependencyProperty TitleMarginProperty =
			DependencyProperty.Register("TitleMargin", typeof(Thickness), typeof(TitledItem), new UIPropertyMetadata(new Thickness(5, 0, 5, 0)));

		/// <summary>
		/// Gets or sets the margin of the content
		/// </summary>
		/// <value>If not set: new Thickness(0, 2, 0, 2)</value>
		[Category("Common Properties")]
		[Description("Gets or sets the margin of the content")]
		public Thickness ContentMargin
		{
			get { return (Thickness)GetValue(ContentMarginProperty); }
			set { SetValue(ContentMarginProperty, value); }
		}

		/// <summary>
		/// Identifies the ContentMargin dependency property
		/// </summary>
		public static readonly DependencyProperty ContentMarginProperty =
			DependencyProperty.Register("ContentMargin", typeof(Thickness), typeof(TitledItem), new UIPropertyMetadata(new Thickness(0, 2, 0, 2)));
	}
}
