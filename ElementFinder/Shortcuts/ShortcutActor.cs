﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ElementFinder.Properties;
using ElementFinder.ViewModels;

namespace ElementFinder.Shortcuts
{
    public class ShortcutActor
    {
        public ShortcutActor(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _shortcutsCollector = new ShortcutsCollector();
            Refresh();
            _shortcutsCollector.Start();
        }

        private readonly MainViewModel _mainViewModel;
        private readonly ShortcutsCollector _shortcutsCollector;

        public void Refresh()
        {
            _shortcutsCollector.ClearShortcuts();

            var shortCuts = new List<IShortcut>();
            
            var enableDisableShortcut = CreateShortcut(() => _mainViewModel.IsEnabled = !_mainViewModel.IsEnabled, Settings.Default.EnableDisableShortcut, false);
            if (enableDisableShortcut != null)
                shortCuts.Add(enableDisableShortcut);
            
            var copyAutomationIdShortcut = CreateShortcut(() => _mainViewModel.CopyAutomationId(), Settings.Default.CopyAutomationIdShortcut);
            if (copyAutomationIdShortcut != null)
                shortCuts.Add(copyAutomationIdShortcut);
            
            var bringOnTop = CreateShortcut(() => BringOnTop(_mainViewModel), Settings.Default.BringOnTopShortcut);
            if (bringOnTop != null)
                shortCuts.Add(bringOnTop);

            var toggleViewShortcut = CreateShortcut(() => ToggleView(_mainViewModel), Settings.Default.ToggleViewShortcut);
            if (toggleViewShortcut != null)
                shortCuts.Add(toggleViewShortcut);

            _shortcutsCollector.SetShortcuts(shortCuts.ToArray());
        }

        private void ToggleView(MainViewModel mainViewModel)
        {
            mainViewModel.OnToggleView();
        }

        private IShortcut CreateShortcut(Action action, string keysSettings, bool ifEnabled = true)
        {
            var keyStrings = keysSettings.Split(new []{" + "}, StringSplitOptions.RemoveEmptyEntries);
            var keys = ToKeys(keyStrings);

            if (!keys.Any())
                return null;

            var toUseAction = action;
            if (ifEnabled)
                toUseAction = () =>
                {
                    if (_mainViewModel.IsEnabled)
                        action();
                };

            return new Shortcut(toUseAction, () => { }, keys.ToArray());
        }

        private List<Key> ToKeys(string[] keyStrings)
        {
            var keys = new List<Key>();
            foreach (var keyString in keyStrings)
            {
                Key key;
                if (Enum.TryParse(keyString, true, out key))
                    keys.Add(key);
            }

            return keys;
        }

        private void SetEnabledOnlyShortcut(Action action)
        {
            if (_mainViewModel.IsEnabled)
                action();
        }

        public void StartShortcuts()
        {
            _shortcutsCollector.Start();
        }

        public void StopShortcuts()
        {
            _shortcutsCollector.Stop();
        }

        private void BringOnTop(MainViewModel mainViewModel)
        {
            var oldTopMost = mainViewModel.TopMost;
            mainViewModel.TopMost = false;
            mainViewModel.TopMost = true;
            mainViewModel.TopMost = oldTopMost;
        }
    }
}
