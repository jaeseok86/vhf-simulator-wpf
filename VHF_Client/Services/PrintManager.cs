using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VHF_Client.Services
{
    [SuppressMessage("Interoperability", "CA1416:플랫폼 호환성 검토", Justification = "Windows 전용 애플리케이션")]
    public class PrintManager
    {
        // 전역 멤버 변수로 프린터 설정 유지
        private PrintDocument printDoc;
        private string printMessage; // 출력할 문자열 저장


        public PrintManager()
        {
            InitPrinter();
        }

        private void InitPrinter()
        {
            printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = new PrinterSettings().PrinterName; // 기본 프린터 설정
            printDoc.PrintPage += PrintPageHandler;
        }

        private void PrintPageHandler(object sender, PrintPageEventArgs e)
        {
            if (!string.IsNullOrEmpty(printMessage))
            {
                // 출력할 텍스트 설정
                e.Graphics.DrawString(printMessage, new Font("Arial", 12), System.Drawing.Brushes.Black, new PointF(100, 100));
            }
        }

        public void PrintDSCMessage(string dscMessage)
        {
            try
            {
                printMessage = dscMessage; // 전역 변수에 출력할 문자열 저장
                printDoc.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("프린터 오류: " + ex.Message);
            }
        }

        public void ReleasePrinterResources() // 프린터 자원 해제
        {
            if (printDoc != null)
            {
                printDoc.Dispose();
                printDoc = null;
            }
        }

    }
}
