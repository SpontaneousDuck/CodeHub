﻿using CodeHub.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace CodeHub.Views
{
    public sealed partial class AppearanceView : SettingsDetailPageBase
    {
        public AppearanceView()
        {
            this.InitializeComponent();

            if (SettingsService.GetSetting("AppTheme") == "Dark")
            {
                DarkThemeButton.Visibility = Visibility.Collapsed;
                LightThemeButton.Visibility = Visibility.Visible;
            }
            else
            {
                DarkThemeButton.Visibility = Visibility.Visible;
                LightThemeButton.Visibility = Visibility.Collapsed;
            }
        }
        private void LightThemeButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SettingsService.SaveSetting("AppTheme", "Light");
            DarkThemeButton.Visibility = Visibility.Visible;
            LightThemeButton.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }

        private void DarkThemeButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SettingsService.SaveSetting("AppTheme", "Dark");
            DarkThemeButton.Visibility = Visibility.Collapsed;
            LightThemeButton.Visibility = Visibility.Visible;
            e.Handled = true;
        }
        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            TryNavigateBackForDesktopState(e.NewState.Name);
        }
    }
}
