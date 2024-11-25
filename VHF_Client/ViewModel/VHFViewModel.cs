using NAudio.CoreAudioApi;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
//using System.Windows.Threading;
using VHF_Client.Model;
using System.Timers;
using NAudio.Wave.SampleProviders;

using System.Diagnostics; // 시간 측정을 위한

using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics;
using VHF_Client.Common;
using VHF_Client.Services.SimulationInformationProviders;
using VHF_Client.Services;
using System.Diagnostics.CodeAnalysis;

namespace VHF_Client.ViewModel
{
    [SuppressMessage("Interoperability", "CA1416:플랫폼 호환성 검토", Justification = "Windows 전용 애플리케이션")]
    public class VHFViewModel : ViewModelBase
    {

        #region Define Button Command

        public Command Channel16orCurrentCommand { get; set; }
        public Command Channel70Command { get; set; }

        public Command DistressCommand { get; set; }

        public Command DcsMenuCommand { get; set; }
        public Command MenuCommand { get; set; }

        #endregion

        private UdpClient udpClient;

        //waveIn, waveOut 보다 WasapiCapture, WasapiOut가 딜레이가 적음
        private WasapiCapture wasapiCapture; //Windows Audio Session API
        private WasapiOut wasapiOut;

        private WaveOutEvent frequencyWaveOut;
        private BufferedWaveProvider waveProvider;
        private SignalGenerator signalGenerator;

        public ChannelInfo channelInfo = ChannelInfo.Instance;

        Mediator mediator = Mediator.instance;

        private bool isTalking = false;

        //private string serverIP = "192.168.100.164"; // Server IP (Local Desktop)
        //private int serverPort = 5007; // Server listening port
        //private int localPort = 5001;  // Local port to receive voice data

        //private string clientName = "Client1"; // Client identifier (set this as needed) (임시 Client 구분 정보)

        private string serverIP = ""; // Server IP (Local Desktop)
        private int serverPort; // Server listening port
        private int localPort;  // Local port to receive voice data

        private string clientName = ""; // Client identifier (set this as needed) (임시 Client 구분 정보)

        public string OrgUsedChannel // 실제 음성 및 DSC 메시지 송수신 처리에 사용되는 채널
        {
            get => channelInfo.orgUsedChannel;
            set => SetProperty(ref channelInfo.orgUsedChannel, value);

            //get => vhfChannel;
            //set => SetProperty(ref vhfChannel, value);
        }
                
        private bool isPrintDscMsg = false; // 필요에 따라 값 설정 (True 설정시 DSC Messge가 PC에 기본 설정된 프린트로 출력됨)

        // true  : 키보드 (왼쪽) Ctrl 키 눌림 여부에 따라 음성 전송 (PPT 마이크가 없는 경우 이어폰의 마이크를 사용하며 PPT 버튼 대신 Ctrl Key 눌림으로 대신함)
        // false : PTT 마이크의 PTT 버튼이 눌린상태에서 유효한 음성(진폭)이 있을 경우 음성 전송
        bool isVoiceSendingByKeyboard = true;

        //private readonly float silenceThreshold = 0.001f;  // (PC 파워 소리 정도 감지 :0.0005f; : 이하는 너무 예민)  ++ 권장? (0.001f;) 그러나 아무 말이 없을때 RX가 됨) 이 값으로 음성의 최소 진폭을 설정 (0.02f;)
        //private readonly float silenceThreshold = 0.001f;  // (0.001f는 3.5 마이크, 4핀, PTT 핸드 마이크 사용시 적용) / (PC 파워 소리 정도 감지 :0.0005f; : 이하는 너무 예민)  ++ 권장? (0.001f;) 그러나 아무 말이 없을때 RX가 됨) 이 값으로 음성의 최소 진폭을 설정 (0.02f;) 

        private readonly float silenceThreshold = 0.0005f;  // (PC 파워 소리 정도 감지 :0.0005f; : On/Off USB 마이크 사용시 적용)

        //private readonly float silenceThreshold = 0.002f;  // (혹시나 3.5 4핀 -> 3핀 변환 후 USB 변환으로 확인시 / 의도대로 되진 않지만 확인은 가능함) (PC 파워 소리 정도 감지 :0.0005f; : On/Off USB 마이크 사용시 적용)


        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(); //비동기 작업에 대한 자원 관리를 위해 사용됨.

        private Task receiveTask;

        private Brush distAlarmCircleColor = Brushes.White;

        //DispatcherTimer channelTimer = null;
        System.Timers.Timer channelTimer = null;

        SharedMemorySimulationInfoProvider simInfoProvider = new SharedMemorySimulationInfoProvider(); //IOS의 Data를 SharedMomry에서 취득하여 위도, 경도, 시간 정보에 사용

        ChannelNameProvider channelNameProvider = new ChannelNameProvider();

        PrintManager printManager = new PrintManager();

        public Brush DistAlarmCircleColor
        {
            get => distAlarmCircleColor;
            set => SetProperty(ref distAlarmCircleColor, value);
        }

        private Color otherAlarmCircleColor = Color.White;
        public Color OtherAlarmCircleColor
        {
            get => otherAlarmCircleColor;
            set => SetProperty(ref otherAlarmCircleColor, value);
        }

        string serverStatus = "";
        public string ServerStatus
        {
            get => serverStatus;
            set => SetProperty(ref serverStatus, value);
        }

        string usingVoiceChannel = "";
        public string UsingVoiceChannel
        {
            get => usingVoiceChannel;
            set => SetProperty(ref usingVoiceChannel, value);
        }

        public string TempNewChannel // Enter 입력 전 새로 사용할 채널을 표시하는데 사용되는 정보
        {
            get => channelInfo.tempNewChannel;
            set => SetProperty(ref channelInfo.tempNewChannel, value);
        }

        public string VHFChannel // 실제 음성 및 DSC 메시지 송수신 처리에 사용되는 채널
        {
            get => channelInfo.vhfChannel;
            set => SetProperty(ref channelInfo.vhfChannel, value);
        }

        public WatchType WatchType
        {
            get => channelInfo.watchType;
            set => SetProperty(ref channelInfo.watchType, value);
        }

        public string WatchTypeValue
        {
            get => channelInfo.watchTypeValue;
            set => SetProperty(ref channelInfo.watchTypeValue, value);
        }

        public string ChannelMode
        {
            get => channelInfo.channelMode;
            set => SetProperty(ref channelInfo.channelMode, value);
        }

        string dscMessage = "CALL";
        public string DSCMessage
        {
            get => dscMessage;
            set => SetProperty(ref dscMessage, value);
        }

        private Visibility isDSCMsgVisible = Visibility.Collapsed;
        public Visibility IsDSCMsgVisible
        {
            get => isDSCMsgVisible;
            set => SetProperty(ref isDSCMsgVisible, value);
        }

        private Visibility isDSCMenuVisible = Visibility.Collapsed;
        public Visibility IsDSCMenuVisible
        {
            get => isDSCMenuVisible;
            set => SetProperty(ref isDSCMenuVisible, value);
        }

        private Visibility isMenuVisible = Visibility.Collapsed;
        public Visibility IsMenuVisible
        {
            get => isMenuVisible;
            set => SetProperty(ref isMenuVisible, value);
        }

        private Visibility isChVhfVisible = Visibility.Collapsed;
        public Visibility IsChVhfVisible
        {
            get => isChVhfVisible;
            set => SetProperty(ref isChVhfVisible, value);
        }

        private Visibility isCh16Visible = Visibility.Collapsed;
        public Visibility IsCh16Visible
        {
            get => isCh16Visible;
            set => SetProperty(ref isCh16Visible, value);
        }

        private Visibility isCh09Visible = Visibility.Collapsed;
        public Visibility IsCh09Visible
        {
            get => isCh09Visible;
            set => SetProperty(ref isCh09Visible, value);
        }

        private bool isDistressAlarmBlinking;
        public bool IsDistressAlarmBlinking
        {
            get => isDistressAlarmBlinking;
            set => SetProperty(ref isDistressAlarmBlinking, value);
        }

        private bool isOtherAlarmBlinking;
        public bool IsOtherAlarmBlinking
        {
            get => isOtherAlarmBlinking;
            set => SetProperty(ref isOtherAlarmBlinking, value);
        }

        private string dataFlowStatus = "RX";
        public string DataFlowStatus
        {
            get => dataFlowStatus;
            set => SetProperty(ref dataFlowStatus, value);
        }

        private string transmitPower = "25W";
        public string TransmitPower
        {
            get => transmitPower;
            set => SetProperty(ref transmitPower, value);
        }

        private string latitude = "";
        public string Latitude
        {
            get => latitude;
            set => SetProperty(ref latitude, value);
        }

        private string longitude;
        public string Longitude
        {
            get => longitude;
            set => SetProperty(ref longitude, value);
        }

        private string localtime;
        public string Localtime
        {
            get => localtime;
            set => SetProperty(ref localtime, value);
        }

        private string channelName = "DISTRESS"; // ch 16으로 시작하는 흐름에서 Def
        public string ChannelName
        {
            get => channelName;
            set => SetProperty(ref channelName, value);
        }

        public VHFViewModel()
        {
            Init();
        }

        private void Init()
        {
            InitConnectionInfo();

            InitButtonCommand();

            if (WaveIn.DeviceCount == 0)
            {
                MessageBox.Show("마이크 장치가 없습니다. 정상적으로 실행되지 않습니다.");
                return;
            }

            InitVoiceChat(); // 음성 송/수신 설정 및 시작

            InitToneGenerator(); // 3핀으로 변환 후 연결시 항상 TX가 됨. 4핀 자체로 연결시 누를경우만 TX (silenceThreshold 를 0.02f로 해도) / 4핀 바로 연결시 silenceThreshold = 0.001f;로 해야함. (주석시 누를후 놓을 경우 RX로 변경됨) / USB 마이크 사용시 해당 기능은 없어도 됨.

            if (false == isVoiceSendingByKeyboard)
            {
                // PTT 마이크를 통한 음성 전송 제어시 프로그램 시작시 호출됨.
                // 지속적으로 마이크 입력을 확인하고 일정 진폭 이상의 음성이 있을 경우 Server로 전송
                // (키보드 사용 흐름시에는 키보드 (Ctrl Key)가 누른 경우에 해당 흐름이 진행됨.)
                StartSendVoice();
            }

            mediator.ChannelChangeAction += Mediator_ChannelChangeAction;
            mediator.ShowMainScreenAction += Mediator_ShowMainScreenAction;
            mediator.WatchTypeChangeAction += Mediator_WatchTypeChangeAction;
            mediator.CannelModeChangeAction += Mediator_CannelModeChangeAction;
            mediator.PowerChangeAction += Mediator_PowerChangeAction;

            mediator.LatitudeChangeAction += Mediator_LatitudeChangeAction;
            mediator.LongitudeChangeAction += Mediator_LongitudeChangeAction;
            mediator.LoacaltimeChangeAction += Mediator_LoacaltimeChangeAction;

            mediator.DscMessageSendAction += Mediator_DscMessageSendAction;

            mediator.AutoPrintChangeAction += Mediator_AutoPrintChangeAction;

            mediator.MmsiSendAction += Mediator_MmsiSendAction;

            InitChannelTimer();
        }

        private void Mediator_MmsiSendAction(string mmsi)
        {
            SendMmsi(mmsi);
        }

        private void Mediator_AutoPrintChangeAction(bool isAutoPrint)
        {
            isPrintDscMsg = isAutoPrint;
        }

        private void Mediator_DscMessageSendAction(DscMessage obj)
        {
            SendDscMessage(obj as DscMessage);
        }

        private void Mediator_LoacaltimeChangeAction(string localtime)
        {
            Localtime = localtime;
        }

        private void Mediator_LongitudeChangeAction(string longitude)
        {
            Longitude = longitude;
        }

        private void Mediator_LatitudeChangeAction(string latitude)
        {
            Latitude = latitude;
        }

        private void Mediator_PowerChangeAction(string power)
        {
            TransmitPower = power;
        }

        private void InitChannelTimer()
        {
            //channelTimer = new DispatcherTimer();
            //channelTimer.Interval = TimeSpan.FromMilliseconds(500);
            //channelTimer.Tick += ChannelTimer_Tick;
            //channelTimer.Start();

            channelTimer = new System.Timers.Timer(500);
            channelTimer.Elapsed += ChannelTimer_Tick;
            channelTimer.AutoReset = true;
            channelTimer.Start();
        }

        private void ChannelTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // UI 스레드에서 안전하게 UI를 업데이트
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (WatchType == WatchType.DualWatch)
                    {
                        IsCh09Visible = Visibility.Collapsed; // 기존에 보이던 Triple Watch용 채널 9 표시를 하지 않음

                        if (IsCh16Visible == Visibility.Collapsed)
                        {
                            IsCh16Visible = Visibility.Visible;
                            IsChVhfVisible = Visibility.Collapsed;
                        }
                        else
                        {
                            IsCh16Visible = Visibility.Collapsed;
                            IsChVhfVisible = Visibility.Visible;
                        }
                    }
                    else if (WatchType == WatchType.TripleWatch)
                    {
                        if (IsCh16Visible == Visibility.Collapsed && IsCh09Visible == Visibility.Collapsed)
                        {
                            IsCh16Visible = Visibility.Visible;
                            IsChVhfVisible = Visibility.Collapsed;
                        }
                        else if (IsCh16Visible == Visibility.Visible && IsCh09Visible == Visibility.Collapsed)
                        {
                            IsCh16Visible = Visibility.Collapsed;
                            IsCh09Visible = Visibility.Visible;
                            IsChVhfVisible = Visibility.Collapsed;
                        }
                        else // if (IsCh16Visible == Visibility.Collapsed && IsCh09Visible == Visibility.Visible)
                        {
                            IsCh16Visible = Visibility.Collapsed;
                            IsCh09Visible = Visibility.Collapsed;
                            IsChVhfVisible = Visibility.Visible;
                        }
                    }
                    else // All, Tag, Normal
                    {
                        IsCh16Visible = Visibility.Collapsed;
                        IsCh09Visible = Visibility.Collapsed;
                        IsChVhfVisible = Visibility.Visible;
                    }
                });
            }
            catch(Exception ex)
            {
                MessageBox.Show($"ChannelTimer_Tick : {ex.Message}");
            }
        }

        private void Mediator_CannelModeChangeAction(string channelMode)
        {
            ChannelMode = channelMode;
        }

        private void Mediator_WatchTypeChangeAction(WatchType watchType)
        {
            WatchType = watchType;

            WatchTypeValue = GetWatchTypeValue();
        }

        private string GetWatchTypeValue()
        {
            string typeValue = ""; // Normal

            switch (WatchType)
            {
                case WatchType.DualWatch:
                    typeValue = "DUAL";
                    break;
                case WatchType.TripleWatch:
                    typeValue = "TRIPLE";
                    break;
                case WatchType.AllWatch:
                    typeValue = "ALL";
                    break;
                case WatchType.TagWatch:
                    typeValue = "TAG";
                    break;
                default:
                    break;
            }

            return typeValue;
        }

        private void Mediator_ChannelChangeAction(string channel)
        {
            VHFChannel = channel;

            ChannelName = channelNameProvider.GetChannelName(VHFChannel);

            SendSelectedChannel();
        }

        private void Mediator_ShowMainScreenAction()
        {
            ShowMainScreen();
        }

        //소리가 들리지 않는 저주파 톤을 일정하게 출력
        //특정 주파수의 신호를 생성하여 스피커로 출력하고, 이를 소프트웨어에서 감지
        //사람이 들을 수 없는 범위의 주파수를 사용, 소프트웨어는 명확히 PTT 상태를 확인

        // PPT 버튼을 누른상태에서 TX가 유지되도록 하기 위해 사용
        public void InitToneGenerator()
        {
            frequencyWaveOut = new WaveOutEvent();

            signalGenerator = new SignalGenerator
            {
                Gain = 0.2,
                Frequency = 20, // 미가청 주파수
                Type = SignalGeneratorType.Sin
            };

            frequencyWaveOut.Init(signalGenerator);
            frequencyWaveOut.Play();
        }

        private void InitConnectionInfo()
        {
            IniFileReader iniFileReader = new IniFileReader("config.ini");

            serverIP = iniFileReader.serverIp;
            serverPort = iniFileReader.serverPort;

            localPort = iniFileReader.localPort;
            clientName = iniFileReader.clientName;

            if(iniFileReader.isDualWatch)
            {
                WatchType = WatchType.DualWatch;
            }
            else if (iniFileReader.isTripleWatch)
            {
                WatchType = WatchType.TripleWatch;
            }
            else if (iniFileReader.isAllWatch)
            {
                WatchType = WatchType.AllWatch;
            }
            else
            {
                WatchType = WatchType.Normal;
            }

            isVoiceSendingByKeyboard = iniFileReader.isVoiceSendingByKeyboard;
        }

        public bool IsVoiceSendingByKeyboard()
        {
            return isVoiceSendingByKeyboard;
        }

        private void InitButtonCommand()
        {
            Channel16orCurrentCommand   = new Command(Set16orCurrentChannel, CanExecute);
            Channel70Command            = new Command(Set70Channel, CanExecute);

            DistressCommand             = new Command(SendDistressMessage, CanExecute);

            DcsMenuCommand              = new Command(VisibleDcsMenu, CanExecute);
            MenuCommand                 = new Command(VisibleMenu, CanExecute);
        }        

        //private async Task StartVoiceChat()
        private void InitVoiceChat()
        {
            // Setup UDP client
            udpClient = new UdpClient(localPort);
            udpClient.Connect(serverIP, serverPort);
            //udpClient.Client.ReceiveTimeout = 5000;

            wasapiCapture = new WasapiCapture();
            wasapiCapture.WaveFormat = new WaveFormat(8000, 16, 1);
            wasapiCapture.DataAvailable += SendAudio;

            waveProvider = new BufferedWaveProvider(wasapiCapture.WaveFormat);
            //waveProvider.BufferLength = 320; //800(바이트) : 50ms, 320 : 20ms 정보의 버퍼 크기 (320 및 800 사용시 실시간 음성수신 Buffer가 full이됨)

            wasapiOut = new WasapiOut();            
            wasapiOut.Init(waveProvider);
            wasapiOut.Play();

            // Start receiving audio and DSC Message from Server
            // 비동기 작업을 receiveTask로 실행
            receiveTask = CheckReceivedData(cancellationTokenSource.Token);

            // Send the initial selected channel to the server
            SendSelectedChannel();
        }

        private async void SendAudio(object sender, WaveInEventArgs e) // 비동기로 진행시
        {
            if (VHFChannel == "") // 유효한 채널이 설정되지 않은 경우
            {
                if (false == isVoiceSendingByKeyboard)
                {
                    // 키보드 키를 통한 음성 전송 여부를 처리하지 않고 PTT 마이크를 통해 제어할 경우,
                    // 음성을 보내지 않은 흐름인 경우 음성 전송 상태를 표시하지 않게 해당 위치에서 초기화 함.
                    DataFlowStatus = "RX";
                }
                return;
            }

            //// 오디오 입력에서 FFT 계산
            //float[] inputFft = recorder.ComputeFft(e.Buffer);
            //if (recorder.IsMatchingSample(inputFft, recorder.releaseSampleFft))
            //{
            //    Console.WriteLine("Skip Button Sound");
            //    return; // 버튼 눌림, 놓음 소리는 보내지 않음.
            //}

            //Stopwatch stopwatch = new Stopwatch(); //수행 시간 측정
            //stopwatch.Start();

            if (false == isVoiceSendingByKeyboard)
            {
                // PTT 마이크 버튼을 통한 흐름인 경우에만 음성 진폭은 사용함.
                // (키보드 사용 + 이어폰 등의 마이크 사용시 작은 소리가 전달되지 않는 경우등이 있어 이 경우 키보드를 누를 상태에서는 모든 음성을 전송함.)

                // 오디오 데이터를 float로 변환
                float[] buffer = new float[e.Buffer.Length / 2]; // 16bit -> float
                for (int index = 0; index < buffer.Length; index++)
                {
                    buffer[index] = BitConverter.ToInt16(e.Buffer, index * 2) / 32768f; // Normalize the audio
                }

                //#region 오디오 입력의 유사성을 계산하여 확인하는 방식
                //// 오디오 입력에서 FFT 계산
                //float[] inputFft = recorder.PerformFft(buffer);
                //if (recorder.IsMatchingSample(inputFft, recorder.releaseSampleFft))
                //{
                //    //Console.WriteLine("Skip Button Sound");

                //    //DataFlowStatus = "RX_Button Release";

                //    return; // 버튼 눌림, 놓음 소리는 보내지 않음.
                //}
                //#endregion

                // 음성이 존재하는지 확인 (실제로 유효한 범위의 신호인지 확인 / 너무 작은 소리는 skip 됨)
                if (false == IsVoiceDetected(buffer))
                {
                    if (false == isVoiceSendingByKeyboard)
                    {
                        DataFlowStatus = "RX";
                        //Console.WriteLine($"---------------------------");
                    }
                    return;
                }
            }

            //stopwatch.Stop();
            //// measure elapsed time in milliseconds
            //long elapsedMs = stopwatch.ElapsedMilliseconds;
            //// display the elapsed time
            ////MessageBox.Show($"Elapsed time: {elapsedMs} ms");
            //Console.WriteLine($"Elapsed time: {elapsedMs} ms");

            DataFlowStatus = "TX";

            // Convert the displayed channel to a padded 3-character string (e.g., "16" -> "016")
            string paddedChannel = VHFChannel.PadLeft(3, '0');  // 채널 번호를 3자리로 패딩

            // Encode channel as byte array
            //byte[] channelData = Encoding.UTF8.GetBytes(selectedChannel);
            byte[] channelData = Encoding.UTF8.GetBytes(paddedChannel);
            byte[] sendData = new byte[channelData.Length + e.BytesRecorded];

            // Combine channel data with audio data
            Buffer.BlockCopy(channelData, 0, sendData, 0, channelData.Length);
            Buffer.BlockCopy(e.Buffer, 0, sendData, channelData.Length, e.BytesRecorded);

            await udpClient.SendAsync(sendData, sendData.Length); // 비동기 전송으로 변경
        }

        private bool IsVoiceDetected(float[] audioBuffer)
        {
            // 음성 신호의 평균 진폭을 계산하여 무음 여부 확인
            foreach (var sample in audioBuffer)
            {
                if (Math.Abs(sample) > silenceThreshold)
                {
                    return true; // 음성이 감지되었을 때
                }
            }
            return false; // 무음일 때
        }

        private async Task CheckReceivedData(CancellationToken cancellationToken) // 비동기로 진행시
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    //byte[] receivedData = udpClient.Receive(ref remoteEndPoint); // Blocking call
                    UdpReceiveResult result = await udpClient.ReceiveAsync(); // 비동기 호출로 변경
                    byte[] receivedData = result.Buffer;

                    if (receivedData.Length > 0)
                    {
                        string receivedString = Encoding.UTF8.GetString(receivedData);
                        if (receivedString.StartsWith("DSC:"))
                        {
                            SetDscMessages(receivedData); // 수신 DSC Msg 처리
                        }
                        //else if (receivedString.StartsWith("CHANNEL_INFO:"))
                        //{
                        //    CheckUsedVoiceChannel(receivedString); // 채널 음성 전송 시작 처리
                        //}
                        else
                        {
                            if (DataFlowStatus == "RX") // TX 상태에서는 음성 수신을 무시함.
                            {
                                CheckReceivedAudio(receivedData); // 수신 음성 처리
                            }
                        }
                    }

                    ServerStatus = "";
                }
                catch (ObjectDisposedException)
                {
                    // This exception is expected if the UDP client is closed while receiving
                    break; // 루프를 벗어남
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.TimedOut)
                    {
                        //ServerStatus = "Waiting for server to become available ...";

                        await Task.Delay(1000);

                    }
                    else
                    {
                        //ServerStatus = "Waiting for server to become available ...";

                        await Task.Delay(1000);
                    }
                }
            }
        }

        private void SetDscMessages(byte[] receivedData)
        {
            try
            {
                string receivedString = Encoding.UTF8.GetString(receivedData);

                string jsonMessage = receivedString.Substring(4); // Remove DSC: prefix
                DscMessage dscMessage = JsonConvert.DeserializeObject<DscMessage>(jsonMessage);

                string dscMsg = "";
                if(dscMessage.format == "DISTRESS")
                {
                    dscMsg = $"------------------------------------------------------------------\n" +
                               $"\t           [DSC] DISTRESS ALERT \n" +
                               $"------------------------------------------------------------------\n\n" +
                               $"Format :\t{dscMessage.format}\n\n" +                               
                               $"Nature :\t\t{dscMessage.nature}\n\n" +
                               $"Latitude :\t{dscMessage.latitude}\n\n" +
                               $"Longitude :\t{dscMessage.longitude}\n\n" +
                               $"Time (UTC):\t{dscMessage.utcTime}";
                }
                else if (dscMessage.format == "ALL SHIP")
                {
                    dscMsg = $"------------------------------------------------------------------\n" +
                               $"\t           [DSC] ALL SHIP ALERT \n" +
                               $"------------------------------------------------------------------\n\n" +
                               $"Format :\t{dscMessage.format}\n\n" +
                               $"category :\t{dscMessage.category}\n\n" +
                               $"Latitude :\t{dscMessage.latitude}\n\n" +
                               $"Longitude :\t{dscMessage.longitude}\n\n" +
                               $"Time (UTC):\t{dscMessage.utcTime}";
                }
                else if (dscMessage.format == "INDIVIDUAL")
                {
                    dscMsg = $"------------------------------------------------------------------\n" +
                               $"\t           [DSC] INDIVIDUAL ALERT \n" +
                               $"------------------------------------------------------------------\n\n" +
                               $"Format :\t{dscMessage.format}\n\n" +
                               $"category :\t{dscMessage.category}\n\n" +
                               $"Latitude :\t{dscMessage.latitude}\n\n" +
                               $"Longitude :\t{dscMessage.longitude}\n\n" +
                               $"Time (UTC):\t{dscMessage.utcTime}";
                }

                DSCMessage = dscMsg;

                VisibleDSCMsg(true);

                if (isPrintDscMsg)
                {
                    printManager.PrintDSCMessage(dscMsg);
                }

                if (dscMessage.format == "DISTRESS")
                {
                    StartDistressAlarm();
                }
                else // ALL SHIP, INDIVIDUAL
                {
                    StartOtherAlarm(); //++ Other도 점등 되어야 하는지 확이 필요.
                }
            }
            catch (Exception ex)
            {
                // Handle the exception when the client stops
                MessageBox.Show(ex.Message, "DSC Message");
            }
        }

        private void VisibleDSCMsg(bool isVisible)
        {
            if (isVisible)
            {
                IsDSCMsgVisible = Visibility.Visible;
                IsDSCMenuVisible = Visibility.Collapsed;
                IsMenuVisible = Visibility.Collapsed;
            }
            else
            {
                IsDSCMsgVisible = Visibility.Collapsed;
                IsDSCMenuVisible = Visibility.Collapsed;
                IsMenuVisible = Visibility.Collapsed;
            }
        }

        public async Task StartDistressAlarm()
        {
            IsDistressAlarmBlinking = true;
            await Task.Delay(1000); // Wait

            IsDistressAlarmBlinking = false;
        }

        public async Task StartOtherAlarm()
        {
            IsOtherAlarmBlinking = true;
            await Task.Delay(1000); // Wait

            IsOtherAlarmBlinking = false;
        }

        // Sends the selected channel to the server
        private void SendSelectedChannel()
        {
            if (udpClient != null)
            {
                string paddedChannel = VHFChannel.PadLeft(3, '0'); // Ensure it's 3 characters long
                string message = $"SELECTED_CHANNEL:{clientName}:{paddedChannel}";
                byte[] messageData = Encoding.UTF8.GetBytes(message);
                udpClient.Send(messageData, messageData.Length);
            }
        }

        public async Task CloseProcess()
        {
            printManager?.ReleasePrinterResources();

            await StopVoiceChat(); // 비동기 작업이 완료될 때까지 대기
        }

        private async Task StopVoiceChat()
        {
            if(simInfoProvider != null)
            {
                simInfoProvider.StopSimualtorTimer();
            }

            if (false == isVoiceSendingByKeyboard)
            {
                StopSendVoice(); //PTT 마이크를 통한 음성 전송 제어시 프로그램 종료시 호출됨.
            }

            wasapiCapture?.StopRecording();
            wasapiCapture?.Dispose();

            wasapiOut?.Stop();
            wasapiOut?.Dispose();

            frequencyWaveOut?.Stop();
            frequencyWaveOut?.Dispose();

            if (channelTimer != null)
            {
                channelTimer.Stop();
                channelTimer.Elapsed -= ChannelTimer_Tick;
                channelTimer.AutoReset = false;
                channelTimer.Dispose();
            }

            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel(); //작업 취소 요청

                try
                {
                    // 비동기 작업이 완료될 때까지 대기 (CheckReceivedData 대기)
                    if (receiveTask != null)  // receiveTask가 null이 아닌 경우에만 대기
                    {
                        udpClient?.Close(); // UdpClient를 닫아 비동기 수신을 강제로 종료
                        await receiveTask; // CheckReceivedData가 정상적으로 종료될 때까지 대기
                    }
                }
                catch (OperationCanceledException)
                {
                    // 비동기 작업이 취소되었음을 처리
                }
                finally
                {
                    cancellationTokenSource.Dispose(); //자원 해제
                }

                //cancellationTokenSource.Dispose(); //자원 해제
            }

            //waveIn?.StopRecording();
            //waveIn?.Dispose();

            //waveOut?.Stop();
            //waveOut?.Dispose();






            //wasapiCapture?.StopRecording();
            //wasapiCapture?.Dispose();

            //wasapiOut?.Stop();
            //wasapiOut?.Dispose();

            //frequencyWaveOut?.Stop();
            //frequencyWaveOut?.Dispose();

            //if (channelTimer != null)
            //{
            //    channelTimer.Stop();
            //    channelTimer.Elapsed -= ChannelTimer_Tick;
            //    channelTimer.AutoReset = false;
            //    channelTimer.Dispose();
            //}

            udpClient = null; // Ensure it's cleared
        }
        
        private void ShowMainScreen()
        {
            if (IsDSCMsgVisible == Visibility.Visible) // DSC 화면 가리기
            {
                VisibleDSCMsg(false);
            }

            IsDSCMenuVisible = Visibility.Collapsed;
            IsMenuVisible = Visibility.Collapsed;

            //gridMainScreen.Visibility = Visibility.Visible;
            //gridDscMenu.Visibility = Visibility.Collapsed;
            //gridSettingMenu.Visibility = Visibility.Collapsed;
        }

        private void ClearTempNewChannel(object obj)
        {
            //Clear는 채널 입력에 대한 Clear가 아닌 기능들에 대한 Clear임 (ex : DSC Call 소리 종료, 입력 흐름 취소 등)

        }

        private void SendDistressMessage(object obj)
        {
            //if (VHFChannel == "")
            //{
            //    //채널이 선택되지 않았을 경우에는 Msg를 보내지 않음. 추후 DISTRESS 버튼으로 인한 DSC 전송시 Ch 70 이 자동 선택된 후 보내는 흐름으로 변경 고려. 그러나 DSC는 70번 채널을 사용하지만, 현재 VHF의 채널도 70으로 변경되어야 하는지는 아직 명확하지 않음.

            //    return;
            //}

            SendDscMessage();
        }

        // Sends a DSC message with multiple targets (including "all"), the channel, and the message type
        private void SendDscMessage(DscMessage dscMessage = null) // DISTRESS 버튼에 의한 DSC 전송
        {
            if (udpClient != null)
            {
                if (dscMessage == null) // DISTRESS 버튼에 의한 DSC 전송
                {
                    dscMessage = new DscMessage //++ 추후 내부 Data로 전송 필요
                    {
                        format = "DISTRESS",
                        nature = "UNDEFINED", // 기본

                        latitude = Latitude,
                        longitude = Longitude,
                        utcTime = Localtime
                    };
                }

                string jsonMessage = JsonConvert.SerializeObject(dscMessage);
                byte[] messageData = Encoding.UTF8.GetBytes("DSC:" + jsonMessage); // Prefix DSC to differentiate

                udpClient.Send(messageData, messageData.Length);

                if (dscMessage.format == "DISTRESS")
                {
                    StartDistressAlarm(); // DSC Message 송신시에도 Distress Alarm이 점등 된다.
                }
                else // ALL SHIP, INDIVIDUAL
                {
                    StartOtherAlarm(); //++ Other도 점등 되어야 하는지 확이 필요.
                }
            }
        }

        private void SendMmsi(string mmsi) // 자선 MMSI 전송
        {
            if (udpClient != null)
            {
                string message = $"MMSI:{clientName}:{mmsi}";
                byte[] messageData = Encoding.UTF8.GetBytes(message);
                udpClient.Send(messageData, messageData.Length);
            }
        }


        private void VisibleDcsMenu(object obj)
        {
            if(IsDSCMenuVisible == Visibility.Visible)
            {
                IsDSCMenuVisible = Visibility.Collapsed;
            }
            else
            {
                IsDSCMenuVisible = Visibility.Visible;
                IsMenuVisible = Visibility.Collapsed;
            }
        }

        private void VisibleMenu(object obj)
        {
            if (IsMenuVisible == Visibility.Visible)
            {
                IsMenuVisible = Visibility.Collapsed;
            }
            else
            {
                IsMenuVisible = Visibility.Visible;
                IsDSCMenuVisible = Visibility.Collapsed;
            }
        }

        private void Set16orCurrentChannel(object obj)
        {
            if (VHFChannel == "16")
            {
                if (OrgUsedChannel != "")
                {
                    //한번이라도 16번이 아닌 채널이 선택됬었을 경우.
                    SetChannel(OrgUsedChannel);
                }
            }
            else
            {
                if (VHFChannel == "")
                {
                    OrgUsedChannel = "16"; //한번도 채널이 선택된적 없는 경우 사용되는 16번 채널이 Org 채널이 됨
                }
                else
                {
                    OrgUsedChannel = VHFChannel;
                }

                SetChannel("16"); //우선은 16번 채널 설정으로 사용. 추후 현재 채널과 16번 채널 변환에 사용
            }

            ShowMainScreen();
        }

        private void Set70Channel(object obj)
        {
            SetChannel("70");

            ShowMainScreen();
        }

        private void VisibleMainScreen(object obj)
        {
            ShowMainScreen();
        }

        private bool CanExecute(object obj)
        {
            return true;
        }

        private void EnterValue(object obj)
        {
            // Menu 등에서 항목 선택 및 입력 확정을 위해 사용됨.
        }

        private void SetChannel(string channel) // 실제 사용되는 채널을 입력 (임시 채널 정보를 최종 사용)
        {
            VHFChannel = channel;

            //ClearTempNewChannelInfo();

            TempNewChannel = ""; //KeyPad가 아닌 VHFView의 Button을 통한 채널 변경시 Temp 채널을 Timer를 사용하지 않고 바로 초기화 함.

            SendSelectedChannel();
        }

        public void StartSendVoice()
        {
            if (!isTalking)
            {
                if (WaveIn.DeviceCount == 0)
                {
                    MessageBox.Show("마이크 장치가 없습니다.");
                    return;
                }

                isTalking = true;
                //waveIn.StartRecording();  // Start capturing audio

                wasapiCapture.StartRecording();
            }
        }

        public void StopSendVoice()
        {
            if (isTalking)
            {
                isTalking = false;
                //waveIn.StopRecording();  // Stop capturing audio

                wasapiCapture.StopRecording();

                DataFlowStatus = "RX";
            }
        }

        private void CheckReceivedAudio(byte[] receivedData)
        {
            try
            {
                // Extract the channel and audio data
                string receivedChannel = Encoding.UTF8.GetString(receivedData, 0, 3); // Assuming 3-byte channel
                byte[] audioData = new byte[receivedData.Length - 3];
                Buffer.BlockCopy(receivedData, 3, audioData, 0, audioData.Length);

                // Play received audio if the channel matches

                bool isPlayAudio = false;

                if (receivedChannel == VHFChannel.PadLeft(3, '0')) // 현재 채널과 같은 경우 처리
                {
                    isPlayAudio = true;
                }
                else
                {
                    if(WatchType == WatchType.DualWatch) //if(isDualWatch) // 현재 채널 / 16번 채널 상호 전환 하면서 수신하는 요건
                    {
                        if(receivedChannel == "016")
                        {
                            isPlayAudio = true;
                        }
                    }
                    else if(WatchType == WatchType.TripleWatch) //else if(isTripleWatch) // 현재 채널 / 16번 채널 / 9번 채널 상호 전환 하면서 수신하는 요건
                    {
                        if(receivedChannel == "016" || receivedChannel == "009")
                        {
                            isPlayAudio = true;
                        }
                    }
                    else if(WatchType == WatchType.AllWatch) //else if(isAllWatch) // 모든 채널을 SCANNING 하는 요건
                    {
                        isPlayAudio = true;
                    }
                }

                if(isPlayAudio)
                {
                    waveProvider.AddSamples(audioData, 0, audioData.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Received Audio");
            }
        }

        #region 특정 채널의 음성 사용 확인
        private void CheckUsedVoiceChannel(string broadcastMessage)
        {
            // Example message format: "CHANNEL_INFO:016"
            string[] parts = broadcastMessage.Split(':');
            if (parts.Length == 2)
            {
                string broadcastChannel = parts[1];

                // If the broadcast channel matches the selected channel, display a notification
                if (broadcastChannel == VHFChannel.PadLeft(3, '0'))
                {
                    UsingVoiceChannel = $"Voice transmission started on channel {broadcastChannel}";
                    // 추가로 UI나 알림창에 표시할 수 있음
                }
            }
        }
        #endregion
    }
}
