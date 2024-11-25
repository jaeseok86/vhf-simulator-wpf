using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using System.IO;

namespace VHF_Server
{
    public class CommunicationManager
    {
        // 종료시 자원 해제등을 관리하기 위한 Handler 흐름 추가

        // Import SetConsoleCtrlHandler from kernel32.dll to handle the console close event
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate handler, bool add);
        private delegate bool ConsoleEventDelegate();
        private static ConsoleEventDelegate handler;

        private UdpClient udpServer;
        private int serverPort = 5007;
        private Dictionary<string, ClientInfo> clientMap = new Dictionary<string, ClientInfo>(); // 각 Client의 정보 Map

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(); //비동기에 대한 작업 취소 및 해제 / 관리를 위한 사용

        private Task receiveTask;  // 비동기 작업의 완료 흐름 등을 확인하기 위해 선언후 사용

        private bool isSelfEcho = false;
        private bool isShowSendVoiceInfo = false;

        public CommunicationManager()
        {
            init();
        }

        public async Task init()
        {
            Console.WriteLine("Starting VHF Voice Chat Server...");

            LoadClientInfo("clients.ini");  // ini 파일의 각 Client의 IP 정보를 Map에 입력

            udpServer = new UdpClient(serverPort);

            // X 버튼을 눌렀을 경우에 대한 종료 흐름 Handler 등록
            handler += new ConsoleEventDelegate(ShutdownHandler);
            SetConsoleCtrlHandler(handler, true);

            receiveTask = CheckReceivedData(cancellationTokenSource.Token);

            Console.WriteLine("Server is running. Press Enter to stop.");
            Console.ReadLine();

            // 서버 종료 및 자원 해제
            await ShutdownServer();
        }

        // This method is called when the user presses closes the console window
        private bool ShutdownHandler()
        {
            // 비동기 메서드를 동기적으로 실행
            ShutdownServer().Wait(); // 동기적으로 ShutdownServer 완료 대기
            return true; // Return true to prevent the default behavior of terminating immediately
        }

        private void LoadClientInfo(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("INI file not found.");
                return;
            }

            int defTempMmis = 111111111;
            int tempMmis = 0;

            int index = 0;

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (line.StartsWith("Client"))  // Read lines starting with "Client"
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        string clientName = parts[0].Trim();
                        string clientIP = parts[1].Trim();

                        tempMmis = defTempMmis * (++index); // client 1~9, 10은 우선 고려제외

                        // Create an IPEndPoint for the client
                        clientMap[clientName] = new ClientInfo { EndPoint = new IPEndPoint(IPAddress.Parse(clientIP), 5001), SelectedChannel = "---" , MMSI = tempMmis.ToString()};
                    }
                }
                else if(line.Contains("IsSelfEcho"))
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        if("true" == parts[1].Trim())
                        {
                            isSelfEcho = true;
                        }
                        else
                        {
                            isSelfEcho = false;
                        }
                    }
                }
                else if (line.Contains("IsShowSendVoiceInfo"))
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        if ("true" == parts[1].Trim())
                        {
                            isShowSendVoiceInfo = true;
                        }
                        else
                        {
                            isShowSendVoiceInfo = false;
                        }
                    }
                }
            }
        }

        private async Task CheckReceivedData(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        UdpReceiveResult result = await udpServer.ReceiveAsync(); // Non-blocking receive

                        IPEndPoint remoteEndPoint = result.RemoteEndPoint;
                        byte[] receivedData = result.Buffer;
                        string receivedString = Encoding.UTF8.GetString(receivedData);

                        if (receivedString.StartsWith("SELECTED_CHANNEL:"))
                        {
                            SetSelectedChannel(receivedString);
                        }
                        else if (receivedString.StartsWith("MMSI:"))
                        {
                            SetMMSI(receivedString);
                        }
                        else if (receivedString.StartsWith("DSC:"))
                        {
                            SendDscMessage(receivedString, remoteEndPoint);
                        }
                        else // Process voice communication
                        {
                            // //비동기적으로 음성 메시지를 처리
                            //Task.Run(() => SendVoice(receivedData, remoteEndPoint));

                            SendVoice(receivedData, remoteEndPoint);
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        // UDP 서버가 이미 닫힌 경우 발생하는 예외 처리
                        Console.WriteLine("UDP server has been closed. Exiting receive loop.");
                        break;
                    }
                    catch (SocketException ex)
                    {
                        // 예외 처리

                        //Console.WriteLine($"SocketException 발생: {ex.Message} (코드: {ex.SocketErrorCode})");

                        //UDP는 비연결형 프로토콜로, 패킷이 손실되거나 순서가 바뀌는 경우가 발생할 수 있습니다. 따라서 ConnectionReset 오류는 UDP의 기본적인 특성 때문에 발생할 수 있으며, 이는 보통 무시해도 됩니다.

                        //Windows Loopback 문제: 로컬에서 클라이언트와 서버를 같은 컴퓨터에서 실행하면 Windows의 loopback 네트워크 메커니즘과 관련된 문제가 발생할 수 있습니다. 이 경우, 클라이언트와 서버를 서로 다른 컴퓨터에서 실행하여 테스트해보세요

                        //// SocketError.ConnectionReset 처리
                        //if (ex.SocketErrorCode == SocketError.ConnectionReset)
                        //{
                        //    Console.WriteLine("서버(VHF Client)가 수신 포트를 열지 않았거나 ICMP Destination Unreachable 메시지를 받았습니다.");
                        //    // ICMP Destination Unreachable

                        //    // 클라이언트가 서버에 데이터를 전송했지만, 서버가 해당 포트에서 수신 대기 중이 아닐 때.
                        //}
                        //else
                        //{
                        //    Console.WriteLine($"기타 소켓 오류: {ex.SocketErrorCode}");
                        //}

                        //break;
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket error: {ex.Message}");
            }
        }

        // Process the selected channel sent by the client
        private void SetSelectedChannel(string receivedString)
        {
            string[] parts = receivedString.Split(':');
            if (parts.Length == 3)
            {
                string clientName = parts[1];
                string selectedChannel = parts[2];

                if (clientMap.ContainsKey(clientName))
                {
                    clientMap[clientName].SelectedChannel = selectedChannel;
                    Console.WriteLine($"Updated {clientName}'s selected channel to {selectedChannel}");
                }
            }
        }

        private void SetMMSI(string receivedString)
        {
            string[] parts = receivedString.Split(':');
            if (parts.Length == 3)
            {
                string clientName = parts[1];
                string mmsi = parts[2];

                if (clientMap.ContainsKey(clientName))
                {
                    clientMap[clientName].MMSI = mmsi;
                    Console.WriteLine($"Updated {clientName}'s MMSI si {mmsi}");
                }
            }
        }

        // Process a DSC message and forward to clients with matching channels
        private void SendDscMessage(string receivedString, IPEndPoint remoteEndPoint)
        {
            string jsonMessage = receivedString.Substring(4); // Remove DSC: prefix
            DscMessage dscMessage = JsonConvert.DeserializeObject<DscMessage>(jsonMessage);

            foreach (var client in clientMap)
            {
                // DSC Message는 채널 비교없시 바로 송신. DSC는 법적으로 채널 70을 사용함. DSC는 DSC 송수신 장비에서 채널 70을 항상 감시하며 수신 되어야 함.

                if (IsCanSendDsc(client.Value.EndPoint.Equals(remoteEndPoint)))
                {
                    if(dscMessage.format == "INDIVIDUAL")
                    {
                        if(client.Value.MMSI != dscMessage.address) // INDIVIDUAL 인 경우, Client의 MMSI와 DSC의 Target MMSI와 같은 경우에만 송신
                        {
                            continue;
                        }
                    }

                    byte[] messageData = Encoding.UTF8.GetBytes(receivedString);
                    udpServer.Send(messageData, messageData.Length, client.Value.EndPoint);
                    Console.WriteLine($"DSC Message [{dscMessage.format}] to {client.Key} on channel 70");
                }
            }
        }

        private void SendVoice(byte[] receivedData, IPEndPoint remoteEndPoint)
        {
            if (receivedData.Length > 0)
            {
                string receivedChannel = Encoding.UTF8.GetString(receivedData, 0, 3); // Extract the channel from the message
                byte[] audioData = new byte[receivedData.Length - 3];
                Buffer.BlockCopy(receivedData, 3, audioData, 0, audioData.Length);

                // 채널 정보 전송 (음성 데이터가 들어온 순간에 모든 클라이언트에게 채널 정보를 보냄)
                //SendUsedVoiceChannel(receivedChannel);

                foreach (var client in clientMap)
                {
                    if (IsCanSendInfo(client.Value.SelectedChannel, receivedChannel, client.Value.EndPoint.Equals(remoteEndPoint)))
                    {
                        byte[] sendData = new byte[3 + audioData.Length]; // 3 bytes for the channel
                        Buffer.BlockCopy(Encoding.UTF8.GetBytes(receivedChannel), 0, sendData, 0, 3);
                        Buffer.BlockCopy(audioData, 0, sendData, 3, audioData.Length);

                        // Send the message asynchronously
                        Task.Run(() => udpServer.Send(sendData, sendData.Length, client.Value.EndPoint));

                        if (isShowSendVoiceInfo)
                        {
                            Console.WriteLine($"Broadcasted voice to {client.Key} on channel {receivedChannel}");
                        }
                    }
                }
            }
        }

        private bool IsCanSendInfo(string clientSelectedChannel, string receivedChannel, bool isSourceClient)
        {
            bool isCanSendInfo = false;

            if (receivedChannel == "016" || receivedChannel == "009") 
            {
                // Ch 16과 Ch 9는 Dual Watch 와 Triple Watch 상태인 Client에서 같이 수신되며 Ch 16은 비상채널로 항상 모든 배는 수신 받아야 함.
                //++ Client가 All 인 경우가 애매해짐.
                // All까지 적용하기 위해서는 각 Client 별 Dual, Triple, All 상태는 받아 Server에서 관리해야함.
                // 나중에 MMSI 등의 정보들도 관리하기 위해 DB를 사용해야 할 수도 있음.

                if (isSelfEcho)
                {
                        isCanSendInfo = true;
                }
                else
                {
                    if (false == isSourceClient) // 보낸 Client를 제외하고 보냄
                    {
                        isCanSendInfo = true;
                    }
                }
            }
            else
            {
                //정상적인 흐름(각 Client가 선택한 채널을 확인하여 같은 채널을 선택했을 경우에만 정보를 보내는 흐름)
                if (isSelfEcho)
                {
                    if (clientSelectedChannel == receivedChannel)
                    {
                        isCanSendInfo = true;
                    }
                }
                else
                {
                    if ((clientSelectedChannel == receivedChannel) && (false == isSourceClient)) // 보낸 Client 제외시
                    {
                        isCanSendInfo = true;
                    }
                }
            }

            //// (임시 / Client의 DualWatch, TripleWatch, All 을 시헝하기 위함) Client가 선택한 채널과 상관없이 보내는 경우
            /// ++ 이 경우 다른 Client가 실행되고 있지 않은 상태에서 전손시 에외가 발생함.
            //if (isSelfEcho)
            //{
            //    isCanSendInfo = true;
            //}
            //else
            //{
            //    if (false == isSourceClient) // 보낸 Client 제외시
            //    {
            //        isCanSendInfo = true;
            //    }
            //}

            return isCanSendInfo;
        }

        private bool IsCanSendDsc(bool isSourceClient)
        {
            bool isCanSendInfo = false;

            if (isSelfEcho)
            {
                isCanSendInfo = true;
            }
            else
            {
                if (false == isSourceClient) // 보낸 Client 제외시
                {
                    isCanSendInfo = true;
                }
            }

            return isCanSendInfo;
        }

        private async Task ShutdownServer()
        {
            // 비동기 작업 취소 및 자원 해제
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();  // 작업 취소 요청

                // 비동기 작업이 완료될 때까지 대기 (CheckReceivedData)
                if (receiveTask != null)  // receiveTask가 null이 아닌 경우에만 대기
                {
                    udpServer?.Close();
                    await receiveTask;
                }

                cancellationTokenSource.Dispose(); // 자원 해제
            }

            Console.WriteLine("Server shutdown complete. All resources released.");
            Environment.Exit(0); // Exit the application cleanly
        }

        #region 모든 Client에게 현재 음성이 전송 중인 채널 정보를 전송
        private void SendUsedVoiceChannel(string receivedChannel)
        {
            // 채널 정보 메시지 생성
            string channelInfoMessage = $"CHANNEL_INFO:{receivedChannel}";

            // 모든 클라이언트에게 채널 정보 전송
            byte[] messageData = Encoding.UTF8.GetBytes(channelInfoMessage);

            foreach (var client in clientMap)
            {
                // 비동기적으로 메시지 전송
                Task.Run(() => udpServer.Send(messageData, messageData.Length, client.Value.EndPoint));
                Console.WriteLine($"Sent channel info to {client.Key}: Channel {receivedChannel} is now broadcasting.");
            }
        }
        #endregion
    }
}
