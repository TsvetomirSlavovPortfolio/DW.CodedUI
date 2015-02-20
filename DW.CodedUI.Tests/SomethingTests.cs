﻿using DW.CodedUI.BasicElements;
using DW.CodedUI.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITesting;

namespace DW.CodedUI.Tests
{
    [CodedUITest]
    public class SomethingTests
    {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Setup()
        {
            CodedUIEnvironment.LoggerSettings.LogFilesDirectory = @"D:\Logs";
            //CodedUIEnvironment.LoggerSettings.IsEnabled = true;
            CodedUIEnvironment.LoggerSettings.LogPassedTestsToo = true;
            CodedUIEnvironment.LoggerSettings.AddTestResultToFileName = true;
        }

        [TestCleanup]
        public void Cleanup()
        {
            LogWriter.Write(TestContext);
        }

        [TestMethod]
        public void Notepad_AddAndRemoveLineBreakAndClose()
        {
            Do.Launch(@"C:\Windows\System32\notepad.exe").And.Wait(1000);
            var window = WindowFinder.Search(Use.Process("notepad"));

            var viewMenuItem = UI.GetChild(By.Name("Ansicht"), From.Element(window));
            MouseEx.Click(viewMenuItem);
            DynamicSleep.Wait(1000);

            var subMenuItem = UI.GetChild(By.AutomationId("27"), From.Element(window));
            MouseEx.Click(subMenuItem);
            DynamicSleep.Wait(1000);

            MouseEx.Click(viewMenuItem);
            DynamicSleep.Wait(1000);

            subMenuItem = UI.GetChild(By.Name("Statusleiste"), From.Element(window));
            MouseEx.Click(subMenuItem);
            DynamicSleep.Wait(1000);

            MouseEx.Click(window.CloseButton);
        }

        [TestMethod]
        public void Notepad_AddSomeTextAndCloseWithoutSave()
        {
            Do.Launch(@"C:\Windows\System32\notepad.exe").And.Wait(1000);
            var window = WindowFinder.Search(Use.Process("notepad"));

            KeyboardEx.SendKeys(window, "das ist ein Text");
            DynamicSleep.Wait(1000);

            MouseEx.Click(window.CloseButton);
            DynamicSleep.Wait(1000);

            var messageBox = WindowFinder.Search<BasicMessageBox>(Use.Title("Editor"));
            var dontSaveButton = UI.GetChild(By.AutomationId("CommandButton_7"), From.Element(messageBox));
            MouseEx.Click(dontSaveButton);
            DynamicSleep.Wait(1000);
        }
    }
}
