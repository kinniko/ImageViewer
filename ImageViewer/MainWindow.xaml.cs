﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageFramework.DirectX;
using ImageViewer.Controller;
using ImageViewer.DirectX;
using ImageViewer.Models;
using SharpDX.Direct3D11;
using Device = ImageFramework.DirectX.Device;

namespace ImageViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ModelsEx models;
        public ViewModels.ViewModels ViewModel;


        public MainWindow()
        {
            InitializeComponent();

            try
            {
                models = new ModelsEx(this);
                ViewModel = new ViewModels.ViewModels(models);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

            DataContext = ViewModel;
            Width = models.Settings.WindowWidth;
            Height = models.Settings.WindowHeight;

            // handle startup arguments
            if (App.StartupArgs.Length == 0) return;

            var import = new ImportDialogController(models);
            foreach (var arg in App.StartupArgs)
            {
                import.ImportImage(arg);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            ViewModel.Dispose();
            base.OnClosed(e);
        }
    }
}
