﻿using Explorer.Entities;
using Explorer.Logic;
using Explorer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Explorer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static Grid RootGrid;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            RootGrid = xRootGrid;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // Register for changes
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            CoreTitleBar_LayoutMetricsChanged(coreTitleBar, null);

            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            // Set XAML element as draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Colors.Gray;
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {

        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            // Get the size of the caption controls area and back button 
            // (returned in logical pixels), and move your content around as necessary.
            LeftPaddingColumn.Width = new GridLength(sender.SystemOverlayLeftInset);
            RightPaddingColumn.Width = new GridLength(sender.SystemOverlayRightInset);

            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = sender.Height;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back) return;
            
            if (e.Parameter is ViewLifetimeControl vlc)
            {
                if (vlc.Context != "") ViewModel.FileBrowserModels.Add(JsonConvert.DeserializeObject<FSEBrowserModel>(vlc.Context));
                else ViewModel.OpenTab();

                vlc.Released += (s, ev) => { };
            }
            else
            {
                ViewModel.OpenTab();
            }

            base.OnNavigatedTo(e);
        }

        private async void Tabs_TabDraggedOutsideAsync(object sender, Microsoft.Toolkit.Uwp.UI.Controls.TabDraggedOutsideEventArgs e)
        {
            var tabModel = (FSEBrowserModel) e.Item;

            if (ViewModel.FileBrowserModels.Count > 1)
            {
                //Remove tab from current window
                ViewModel.FileBrowserModels.Remove(tabModel);
                
                // Need to serialize item to better provide transfer across window threads.
                var lifetimecontrol = await WindowManagerService.Current.TryShowAsStandaloneAsync("Explorer", typeof(MainPage), JsonConvert.SerializeObject(tabModel));
            }
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var key = e.Key;

            var ctrlDown = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);
            if (!ctrlDown) return;

            switch (key)
            {
                case VirtualKey.F:
                    SearchBox.Focus(FocusState.Programmatic);
                    break;
                case VirtualKey.T:
                    ViewModel.OpenTab(@switch: true);
                    break;
                case VirtualKey.W:
                    ViewModel.CloseCurrentTab();
                    break;
            }
        }

        
    }
}
