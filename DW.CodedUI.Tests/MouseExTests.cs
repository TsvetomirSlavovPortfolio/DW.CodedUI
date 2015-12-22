﻿#region License
/*
The MIT License (MIT)

Copyright (c) 2012-2015 David Wendland

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
THE SOFTWARE
*/
#endregion License

using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DW.CodedUI.Tests
{
    [TestClass]
    public class MouseExTests
    {
        [TestMethod]
        public void Move_To_PlacesTheMouse()
        {
            var destination = Position.Point(new Point(200, 300));

            MouseEx.Move(destination);
        }

        [TestMethod]
        public void Move_FromPointToPointFor3Seconds_MovesTheMouseAccordingly()
        {
            var source = Position.Point(new Point(100, 100));
            var destination = Position.Point(new Point(300, 400));

            MouseEx.Move(source, destination, 3000);
        }

        [TestMethod]
        public void Move_FromCurrentToPointFor3Seconds_MovesTheMouseAccordingly()
        {
            var source = Position.Current();
            var destination = Position.Point(new Point(300, 400));

            MouseEx.Move(source, destination, 2000);
        }

        [TestMethod]
        public void PressReleaseButtons_WithAnElementOnTheTopRight_DragsTheIconToAnotherPosition()
        {
            MouseEx.Move(Position.Point(new Point(3770, 45)));
            MouseEx.PressButtons(MouseButtons.Left);
            MouseEx.Move(Position.Point(new Point(3770, 45)), Position.Point(new Point(3770, 200)), 1000);
            MouseEx.ReleaseButtons(MouseButtons.Left);
        }
    }
}