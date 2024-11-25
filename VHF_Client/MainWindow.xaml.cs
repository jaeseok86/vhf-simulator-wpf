using NAudio.Wave;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Drawing;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using VHF_Client.ViewModel;
using VHF_Client.View;

namespace VHF_Client
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        //ucVHF vhfClient = null;

        public MainWindow()
        {
            InitializeComponent();

            //MainContentControl.Content = new ucVHF();

            //vhfClient = MainContentControl.Content as ucVHF;

            this.KeyDown += MainWindow_KeyDown;
            this.KeyUp += MainWindow_KeyUp;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            gridVhfClient?.CloseProcess();

            base.OnClosing(e);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (false == gridVhfClient?.IsVoiceSendingByKeyboard())
            {
                return;
            }

            if (e.Key == Key.LeftCtrl)
            {
                gridVhfClient?.StartSendVoice();
            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (false == gridVhfClient?.IsVoiceSendingByKeyboard())
            {
                return;
            }

            if (e.Key == Key.LeftCtrl)
            {
                gridVhfClient?.StopSendVoice();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
