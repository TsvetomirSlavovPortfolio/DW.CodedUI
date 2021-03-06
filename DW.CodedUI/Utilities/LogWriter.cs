﻿using System;
using System.IO;
using System.Text;
using DW.CodedUI.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DW.CodedUI.Utilities
{
    /// <summary>
    /// Write the log file(s) with the entries created during execute tests.
    /// </summary>
    public static class LogWriter
    {
        /// <summary>
        /// Writes the log file(s). Its intended to be used on the text cleanup.
        /// </summary>
        /// <param name="testContext">The TextContext of the test class.</param>
        /// <remarks>You get the test context created by adding a public property into the class with the cleanup. 
        /// For configuring see <see cref="DW.CodedUI.CodedUIEnvironment.LoggerSettings" />.</remarks>
        /// <example>
        /// <code lang="csharp">
        /// <![CDATA[
        /// [CodedUITest]
        /// public class SomethingTests
        /// {
        ///     public TestContext TestContext { get; set; }
        /// 
        ///     [TestInitialize]
        ///     public void Setup()
        ///     {
        ///         CodedUIEnvironment.LoggerSettings.LogFilesDirectory = @"D:\CodedUI_Logs";
        ///     }
        /// 
        ///     [TestCleanup]
        ///     public void Cleanup()
        ///     {
        ///         LogWriter.Write(TestContext);
        ///     }
        /// 
        ///     [TestMethod]
        ///     public void Any_Test_Using_The_DW_CodedUI()
        ///     {
        ///         // Something
        ///     }
        /// }]]>
        /// </code>
        /// </example>
        public static void Write(TestContext testContext)
        {
            if (!CodedUIEnvironment.LoggerSettings.IsEnabled)
                return;
            
            LogPool.StartDateTimeWritten = false; // On next LogPool.Append the StartDateTime will be set again

            if (CodedUIEnvironment.LoggerSettings.InstantLoggingContext != null)
            {
                ProcessInstantWrittenLog(testContext);
                return;
            }

            if (testContext.CurrentTestOutcome != UnitTestOutcome.Passed || CodedUIEnvironment.LoggerSettings.LogPassedTestsToo)
            {
                var filePath = GetLogFileName(testContext);
                if (filePath == null)
                    return;

                var logs = new StringBuilder();
                AppendHeader(testContext, logs);

                var logLines = LogPool.PopList();
                foreach (var line in logLines)
                    logs.AppendLine(line);

                File.AppendAllText(filePath, logs.ToString());
            }
        }

        private static void ProcessInstantWrittenLog(TestContext testContext)
        {
            var shortFilePath = GetShortFileName(testContext);
            if (shortFilePath == null)
                return;

            if (testContext.CurrentTestOutcome != UnitTestOutcome.Passed || CodedUIEnvironment.LoggerSettings.LogPassedTestsToo)
            {
                var filePath = GetLogFileName(testContext);
                if (filePath == null)
                    return;

                if (shortFilePath != filePath)
                    File.Move(shortFilePath, filePath);

                var logs = new StringBuilder();
                AppendHeader(testContext, logs);

                var logLines = File.ReadAllLines(filePath);
                foreach (var line in logLines)
                    logs.AppendLine(line);

                File.WriteAllText(filePath, logs.ToString());
            }
            else
            {
                if (File.Exists(shortFilePath))
                    File.Delete(shortFilePath);
            }
        }

        internal static void WriteInstant(string message)
        {
            if (!CodedUIEnvironment.LoggerSettings.IsEnabled)
                return;

            var testContext = CodedUIEnvironment.LoggerSettings.InstantLoggingContext;

            var filePath = GetShortFileName(testContext);
            if (filePath == null)
                return;

            File.AppendAllLines(filePath, new [] { message });
        }

        private static string GetShortFileName(TestContext testContext)
        {
            var logDirectory = CreateLogDirectory();
            if (string.IsNullOrWhiteSpace(logDirectory))
                return null;

            var methodName = testContext.TestName;
            return Path.Combine(logDirectory, methodName + ".txt");
        }

        private static string GetLogFileName(TestContext testContext)
        {
            var logDirectory = CreateLogDirectory();
            if (string.IsNullOrWhiteSpace(logDirectory))
                return null;

            var methodName = testContext.TestName;
            var filePath = logDirectory;
            if (CodedUIEnvironment.LoggerSettings.AddTestResultToFileName)
                filePath = Path.Combine(logDirectory, testContext.CurrentTestOutcome + " - " + methodName + ".txt");
            else
                filePath = Path.Combine(logDirectory, methodName + ".txt");
            return filePath;
        }

        private static string CreateLogDirectory()
        {
            try
            {
                var logDirectory = CodedUIEnvironment.LoggerSettings.LogFilesDirectory;
                if (string.IsNullOrWhiteSpace(logDirectory))
                    return string.Empty;

                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);

                return logDirectory;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void AppendHeader(TestContext testContext, StringBuilder logs)
        {
            var startTime = LogPool.StartDateTime;
            var endTime = DateTime.Now;

            logs.AppendLine(string.Format("Executed test: {0}.{1}()", testContext.FullyQualifiedTestClassName, testContext.TestName));
            logs.AppendLine(string.Format("Result: {0}", testContext.CurrentTestOutcome));
            logs.AppendLine(string.Format("Start time: {0}", startTime.ToString("dd/MM/yyyy HH:mm:ss.fff")));
            logs.AppendLine(string.Format("Testrun finished at: {0}", endTime.ToString("dd/MM/yyyy HH:mm:ss.fff")));
            logs.AppendLine(string.Format("Execution time: {0}", (endTime - startTime).ToString("hh\\:mm\\:ss\\.fff")));
            logs.AppendLine();
        }
    }
}
