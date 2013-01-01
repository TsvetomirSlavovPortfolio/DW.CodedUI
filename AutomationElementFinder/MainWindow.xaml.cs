﻿#region License

/*--------------------------------------------------------------------------------
    Copyright (c) 2012-2013 David Wendland

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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using System.Windows.Threading;
using DW.CodedUI.BasicElements;
using DW.CodedUI.UITree;
using DW.CodedUI.Utilities;

namespace AutomationElementFinder
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _timer;
        private readonly int _currentProcessId;
        private BasicElementInfo _currentSelectedElement;
        private Highlighter _highlighter;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            _currentProcessId = Process.GetCurrentProcess().Id;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += HandleSearchTimerTick;

            elementTree.SelectedItemChanged += HandleSelectedItemChanged;

            Elements = new ObservableCollection<BasicElementInfo>();
            IsActivated = true;
            ShowHighlight = true;
            ReadFullTree = true;
        }

        #region FindElements

        private void EnableSearch()
        {
            _timer.Start();
        }

        private void DisableSearch()
        {
            _timer.Stop();
        }

        private void HandleSearchTimerTick(object sender, EventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control) || !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                return;

            var mouse = System.Windows.Forms.Cursor.Position;
            var elements = GetAllElementsByPosition(mouse);
            if (elements == null)
                return;

            Elements.Clear();
            foreach (var element in elements)
                Elements.Add(element);
            if (Elements.Any())
            {
                _currentSelectedElement = Elements.First();
                _currentSelectedElement.IsSelected = true;
            }
            if (ShowHighlight)
                HighlightElement(_currentSelectedElement);
        }

        private IEnumerable<BasicElementInfo> GetAllElementsByPosition(System.Drawing.Point position)
        {
            var items = new List<BasicElementInfo>();

            try
            {
                var point = new Point(position.X, position.Y);
                var element = AutomationElement.FromPoint(point);
                if (element == null || !Helper.IsAvailable(element))
                    return null;

                if (element.Current.ProcessId == _currentProcessId)
                    return null;

                items.Add(BasicElementFinder.GetFullUITree(element));
                if (!ReadFullTree)
                    return items;

                var toppestParent = GetParent(element);
                var tree = BasicElementFinder.GetFullUITree(toppestParent);
                items.AddRange(GetAllElementsByPosition(tree, point));
            }
            catch (Exception ex)
            {
                return items;
            }
            return items;
        }

        private IEnumerable<BasicElementInfo> GetAllElementsByPosition(BasicElementInfo tree, Point position)
        {
            var items = new List<BasicElementInfo>();
            foreach (var child in tree.Children)
            {
                if (!Helper.IsAvailable(child.AutomationElement))
                    continue;
                if (child.AutomationElement.Current.BoundingRectangle.Contains(position))
                    items.Add(child);
                items.AddRange(GetAllElementsByPosition(child, position));
            }
            return items;
        }

        private AutomationElement GetParent(AutomationElement element)
        {
            var parent = TreeWalker.ControlViewWalker.GetParent(element);
            if (parent == null || parent.Current.FrameworkId != "WPF" || !Helper.IsAvailable(parent))
                return element;

            while (parent.Current.ClassName != "Window")
            {
                parent = TreeWalker.ControlViewWalker.GetParent(parent);
                if (!Helper.IsAvailable(parent))
                    return null;
            }
            return parent;
        }

        #endregion FindElements

        #region Highlight
        
        private void HandleSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _currentSelectedElement = null;
            if (e.NewValue != null)
            {
                _currentSelectedElement = (BasicElementInfo)e.NewValue;
                if (ShowHighlight)
                {
                    HighlightElement(_currentSelectedElement);
                    elementTree.Focus();
                }
            }
        }

        private void HighlightElement(BasicElementInfo element)
        {
            if (_highlighter != null)
                _highlighter.Close();

            if (element == null)
                return;

            if (!Helper.IsAvailable(element.AutomationElement))
                return;

            if (element.AutomationElement.Current.IsOffscreen)
                return;

            _highlighter = new Highlighter();
            _highlighter.Highlight(element.AutomationElement);
        }

        private void EnableHighlight()
        {
            if (_currentSelectedElement != null)
                HighlightElement(_currentSelectedElement);
        }

        private void DisableHighlight()
        {
            _highlighter.Close();
        }

        #endregion Highlight

        #region Properties

        public ObservableCollection<BasicElementInfo> Elements { get; set; }

        public bool IsActivated
        {
            get { return _isActivated; }
            set
            {
                _isActivated = value;
                NotifyPropertyChanged(() => IsActivated);

                if (value)
                    EnableSearch();
                else
                    DisableSearch();
            }
        }
        private bool _isActivated;

        public bool ShowHighlight
        {
            get { return _showHighlight; }
            set
            {
                _showHighlight = value;
                NotifyPropertyChanged(() => ShowHighlight);

                if (value)
                    EnableHighlight();
                else
                    DisableHighlight();
            }
        }
        private bool _showHighlight;

        public bool ReadFullTree
        {
            get { return _readFullTree; }
            set
            {
                _readFullTree = value;
                NotifyPropertyChanged(() => ReadFullTree);
            }
        }
        private bool _readFullTree;

        #endregion Properties

        #region NotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged<T>(Expression<Func<T>> property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var memberExpression = property.Body as MemberExpression;
                handler(this, new PropertyChangedEventArgs(memberExpression.Member.Name));
            }
        }

        #endregion NotifyPropertyChanged
    }
}
