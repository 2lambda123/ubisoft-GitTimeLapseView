﻿using System;
using System.Linq;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;

namespace GitTimelapseView.Services
{
    public class ThemingService : ServiceBase
    {
        private readonly ThemeInfo _lightTheme = new ThemeInfo("Light")
        {
            MonacoTheme = "vs",
        };

        private readonly ThemeInfo _darkTheme = new ThemeInfo("Dark")
        {
            IsDark = true,
            MonacoTheme = "vs-dark",
        };

        public ThemingService(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            Themes = new ThemeInfo[]
            {
                _lightTheme,
                _darkTheme,
            };
            var themeProperty = Properties.Settings.Default.Theme;
            var theme = !string.IsNullOrEmpty(themeProperty)
                ? Themes.FirstOrDefault(x => x.Name.Equals(themeProperty, StringComparison.OrdinalIgnoreCase)) ?? Themes[0]
                : Themes[0];

            Theme = theme;
            ApplyTheme(theme, saveSettings: false, reloadWindow: false);
        }

        public ThemeInfo Theme { get; private set; }

        public ThemeInfo[] Themes { get; }

        public void ApplyTheme(ThemeInfo themeInfo, bool saveSettings = true, bool reloadWindow = true)
        {
            Theme = themeInfo;
            if (saveSettings)
            {
                Properties.Settings.Default.Theme = themeInfo.Name;
                Properties.Settings.Default.Save();
            }

            if (reloadWindow && App.Current?.MainWindow is MainWindow mainWindow)
            {
                mainWindow.Reload();
            }

            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(themeInfo.IsDark ? MaterialDesignThemes.Wpf.Theme.Dark : MaterialDesignThemes.Wpf.Theme.Light);
            paletteHelper.SetTheme(theme);
        }
    }
}
