﻿using System;
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
using Explorer.Entities;
using Explorer.Models;

namespace Explorer.Controls
{
    public sealed partial class FileBrowser : UserControl
    {
        public FileBrowserModel ViewModel
        {
            get { return (FileBrowserModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); Bindings.Update(); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (FileBrowserModel), typeof (FileBrowser), new PropertyMetadata(null));


        public FileBrowser()
        {
            this.InitializeComponent();
            ((FrameworkElement) this.Content).DataContext = this;
        }

        private void TextBoxPath_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                ViewModel.NavigateTo(new FileSystemElement { Path = TextBoxPath.Text });
        }

        private void OpenPowershell_Clicked(object sender, RoutedEventArgs e)
        {
            ViewModel.LaunchExe("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe", $"-noexit -command \"cd {ViewModel.Path}\"");
        }
    }
}