﻿#region License
/*
The MIT License (MIT)

Copyright (c) 2012-2018 David Wendland

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

using DW.CodedUI.BasicElements;
using DW.CodedUI.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DW.CodedUI.Tests
{
    [TestClass]
    public class KeyboardExTests
    {
        private static BasicWindow _mainWindow;
        private static BasicWindow _testWindow;
        private static BasicEdit _textBox;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            Do.Launch(TestData.ApplicationPath).And.Wait(1000);
            _mainWindow = WindowFinder.Search(Use.AutomationId(TestData.MainWindowAutomationId));
            var currentButton = UI.GetChild<BasicButton>(By.AutomationId("CUI_KeyboardExTests_Button"), From.Element(_mainWindow));
            currentButton.Unsafe.Click();
            DynamicSleep.Wait(1000);
            _testWindow = WindowFinder.Search(Use.AutomationId("CUI_KeyboardExTestsWindow"), And.NoAssert());
            _textBox = UI.GetChild<BasicEdit>(By.AutomationId("CUI_InputTextBox"), From.Element(_testWindow));
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _testWindow.CloseButton.Unsafe.Click();
            DynamicSleep.Wait(1000);
            _mainWindow.CloseButton.Unsafe.Click();
            DynamicSleep.Wait(1000);
        }

        [TestMethod]
        public void TypeText_SomeText_TextIsWritten()
        {
            Assert.AreEqual("", _textBox.Text);

            KeyboardEx.TypeText(_textBox, "Anything Nice", 50).And.Wait(2000);

            Assert.AreEqual("Anything Nice", _textBox.Text);
            _textBox.Unsafe.SetValue("");
        }

        [TestMethod]
        public void TypeText_TypeLowerTextWithShiftHoldDownSeparatelly_TheTextAppearsInupperCase()
        {
            KeyboardEx.PressKey(_textBox, ModifierKeys.Shift);
            KeyboardEx.TypeText("demo", 50);
            KeyboardEx.ReleaseKey(ModifierKeys.Shift).And.Wait(2000);

            Assert.AreEqual("DEMO", _textBox.Text);
            _textBox.Unsafe.SetValue("");
        }

        [TestMethod]
        public void TypeText_TypeLowerTextWithShiftHoldDown_TheTextAppearsInupperCase()
        {
            KeyboardEx.TypeText(_textBox, "demo", ModifierKeys.Shift, 50).And.Wait(2000);

            Assert.AreEqual("DEMO", _textBox.Text);
            _textBox.Unsafe.SetValue("");
        }

        [TestMethod]
        public void TypeKey_ControlAAfterTypingText_AllGetsSelected()
        {
            KeyboardEx.TypeText(_textBox, "Peter Sausage");
            DynamicSleep.Wait(1000);

            KeyboardEx.TypeKey(_textBox, Key.A, ModifierKeys.Control).And.Wait(2000);
            
            Assert.AreEqual("Peter Sausage", _textBox.SelectedText);
            _textBox.Unsafe.SetValue("");
        }

        [TestMethod]
        public void TypeText_PressShiftAndArrowLeftSevenTimes_SelectsPartOfTheText()
        {
            KeyboardEx.TypeText(_textBox, "Peter Sausage");
            DynamicSleep.Wait(1000);

            Do.Action(() => KeyboardEx.TypeKey(Key.Left, ModifierKeys.Shift)).Repeat(6).And.Wait(500);

            Assert.AreEqual("Sausage", _textBox.SelectedText);
            _textBox.Unsafe.SetValue("");
        }

        [TestMethod]
        public void TypeText_MoreDifferentShiftSelections_SelectsTheTextAccordingly()
        {
            KeyboardEx.TypeText(_textBox, "Peter Sausage");
            KeyboardEx.PressKey(Key.Enter);
            KeyboardEx.TypeText("Was Here");
            DynamicSleep.Wait(1000);

            KeyboardEx.TypeKey(Key.Left, ModifierKeys.Shift);
            KeyboardEx.TypeKey(Key.Left, ModifierKeys.Shift);
            KeyboardEx.TypeKey(Key.Left, ModifierKeys.Shift);
            KeyboardEx.TypeKey(Key.Up, ModifierKeys.Shift);
            KeyboardEx.TypeKey(Key.Left, ModifierKeys.Shift).And.Wait(2000);

            Assert.AreEqual("Sausage\r\nWas Here", _textBox.SelectedText);
            _textBox.Unsafe.SetValue("");
        }

        [TestMethod]
        public void TypeKey_AltF4OnTheWindow_ClosesTheWindow()
        {
            KeyboardEx.TypeKey(_testWindow, Key.F4, ModifierKeys.Alt).And.Wait(2000);

            var testWindow = WindowFinder.Search(Use.AutomationId("CUI_KeyboardExTestsWindow"), And.NoAssert().And.Timeout(2000));
            Assert.IsNull(testWindow);
            var currentButton = UI.GetChild<BasicButton>(By.AutomationId("CUI_KeyboardExTests_Button"), From.Element(_mainWindow));
            currentButton.Unsafe.Click();
            DynamicSleep.Wait(1000);
            _testWindow = WindowFinder.Search(Use.AutomationId("CUI_KeyboardExTestsWindow"), And.NoAssert());
            _textBox = UI.GetChild<BasicEdit>(By.AutomationId("CUI_InputTextBox"), From.Element(_testWindow));
        }
    }
}
