using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace VHF_Client
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private NotifyIcon notifyIcon;

        [SuppressMessage("Interoperability", "CA1416:플랫폼 호환성 검토", Justification = "Windows 전용 애플리케이션")]
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // NotifyIcon 설정
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon("icon.ico"), // 트레이에 표시할 아이콘 파일
                Visible = true,
                Text = "VHF Client - 실행 중"
            };

            // 컨텍스트 메뉴 설정
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Show", null, Show_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Hide", null, Hide_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Exit", null, Exit_Click);

            // 아이콘 더블 클릭 시 동작 연결
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick; ;
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if(Application.Current.MainWindow != null)
            {
                if(Application.Current.MainWindow.Visibility == Visibility.Visible)
                {
                    HideWindow();
                }
                else
                {
                    ShowWindow();
                }
            }
        }

        private void Show_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }

        private void ShowWindow()
        {
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Show();
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                Application.Current.MainWindow.Activate();
            }
        }

        private void Hide_Click(object sender, EventArgs e)
        {
            HideWindow();
        }

        private void HideWindow()
        {
            Application.Current.MainWindow?.Hide();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            // 애플리케이션 종료
            notifyIcon.Dispose();
            Application.Current.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // 프로그램 종료 시 트레이 아이콘 제거
            notifyIcon.Dispose();
            base.OnExit(e);
        }
    }
}
