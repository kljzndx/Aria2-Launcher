﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Aria2Launcher.ViewModel;
using Path = System.IO.Path;

namespace Aria2Launcher.Views
{
    /// <summary>
    /// Aria2ConfigureWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Aria2ConfigureWindow : Window
    {
        private Aria2ConfigureViewModel _viewModel;
        
        public Aria2ConfigureWindow()
        {
            InitializeComponent();
            _viewModel = (Aria2ConfigureViewModel) this.DataContext;
        }

        private void Aria2ConfigureWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(_viewModel.AppConfService.Aria2DirPath, "aria2.conf");
            if (File.Exists(path))
            {
                _viewModel.Aria2ConfService.Load(path);
            }
            else
            {
                MessageBox.Show("找不到 aria2.conf 文件");
                this.Close();
            }
        }

        private void Ok_Button_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Aria2ConfService.Save();
            this.Close();
        }

        private void Cancel_Button_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
