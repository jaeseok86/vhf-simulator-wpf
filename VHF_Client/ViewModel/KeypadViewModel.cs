using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using VHF_Client.Common;
using VHF_Client.Model;

namespace VHF_Client.ViewModel
{
    class KeypadViewModel : ViewModelBase
    {
        public enum ChannelMode
        {
            ITU = 0,
            USA,
            CANADA
        }

        public Command Number1Command { get; set; }
        public Command Number2Command { get; set; }
        public Command Number3Command { get; set; }
        public Command Number4Command { get; set; }
        public Command Number5Command { get; set; }
        public Command Number6Command { get; set; }
        public Command Number7Command { get; set; }
        public Command Number8Command { get; set; }
        public Command Number9Command { get; set; }
        public Command Number0Command { get; set; }

        public Command DelButtonExecute_cmd { get; set; }
        public Command EnterCommand { get; set; }

        public Command FunctionCommand { get; set; }

        public Command ClearCommand { get; set; }
        public Command MainCommand { get; set; }
        public Command DscCommand { get; set; }
        public Command MenuCommand { get; set; }

        ChannelInfo channelInfo = ChannelInfo.Instance;

        Mediator mediator = Mediator.instance;

        Mediator.InputTextType inputDscTextType = Mediator.InputTextType.None;

        string inputText = "";

        public string TempNewChannel // Enter 입력 전 새로 사용할 채널을 표시하는데 사용되는 정보
        {
            get => channelInfo.tempNewChannel;
            set => SetProperty(ref channelInfo.tempNewChannel, value);
        }

        //string vhfChannel = "";
        public string VHFChannel // 실제 음성 및 DSC 메시지 송수신 처리에 사용되는 채널
        {
            get => channelInfo.vhfChannel;
            set => SetProperty(ref channelInfo.vhfChannel, value);
        }

        private System.Timers.Timer tempChannelResetTimer;

        private bool CanExecute(object obj)
        {
            return true;
        }

        private Dictionary<int, string[]> buttonMappings;
        private Dictionary<int, int> buttonState;
        private int lastFocusedPosition = 0; // Keeps track of the last position in the TextBox
        private bool inputInProgress = false; // Tracks if input is in progress before ENT is pressed
        private int currentButton = -1; // Tracks the current button being rotated
        private bool firstInputIsNumber = true; // Ensures the first input is the number

        bool isPower25W = true;

        bool isFunction = false; // 추후 채널 및 Test 입력을 위한 경우와, 별도 기능을 위한 것인지 확인시 사용

        bool isInputText = false; //++ 숫자 채널이 아닌 문자도 사용될 경우 (DistressCallView에서 값을 입력하는 흐름여부 확인. 추후 모든 Text 입력시에 함께 사용 필요)

        WatchType watchType = WatchType.Normal;
        ChannelMode channelMode = ChannelMode.ITU;

        Mediator.InputTextType inputDscTexttype = Mediator.InputTextType.None; 

        public KeypadViewModel()
        {
            Number1Command = new Command(SetTempChannel, CanExecute);
            Number2Command = new Command(SetTempChannel, CanExecute);
            Number3Command = new Command(SetTempChannel, CanExecute);
            Number4Command = new Command(SetTempChannel, CanExecute);
            Number5Command = new Command(SetTempChannel, CanExecute);
            Number6Command = new Command(SetTempChannel, CanExecute);
            Number7Command = new Command(SetTempChannel, CanExecute);
            Number8Command = new Command(SetTempChannel, CanExecute);
            Number9Command = new Command(SetTempChannel, CanExecute);
            Number0Command = new Command(SetTempChannel, CanExecute);

            ClearCommand = new Command(ClearTempNewChannel, CanExecute);
            EnterCommand = new Command(EnterValue, CanExecute);
            MainCommand = new Command(VisibleMainScreen, CanExecute);
            DscCommand = new Command(VisibleDscMenuList, CanExecute);
            MenuCommand = new Command(VisibleMenuList, CanExecute);

            DelButtonExecute_cmd = new Command(DelButton_func, CanExecute);

            FunctionCommand = new Command(SetFunction, CanExecute);

            InitializeButtonMappings();

            InitTempChannelResetTimer();

            mediator.DscTextboxFocusAction += Mediator_DscTextboxFocusAction;
        }

        private void Mediator_DscTextboxFocusAction(Mediator.InputTextType type, string text)
        {
            inputDscTextType = type;
            inputText = text;

            lastFocusedPosition = text.Length;

            if (inputDscTextType == Mediator.InputTextType.None)
            {
                isInputText = false;
            }
            else
            {
                isInputText = true;
            }
        }

        private void InitTempChannelResetTimer()
        {
            tempChannelResetTimer = new System.Timers.Timer(800); // 채널 선택시 연속으로 (숫자) 채널을 선택하는 흐름 유지 주기
            tempChannelResetTimer.Elapsed += TempChannelResetTimer_Elapsed;
        }

        private void TempChannelResetTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 버튼을 통하여 채널 정보 입력시 특정 초 이후에 기존 tempChannel Clear 적용
            // ++ 추후 최종 채널 확정 후 에 Server로 채널 정보를 보낼지 고려. But 그러면 1초 이후에 Server는 Client의 선택채널을 알게됨.
            // ++ 확정되지 않은 채널 사용시 도중에 의도하지 않았던 채널의 음성을 받은 수도 있음. but 이정도는 실제 VHF에서도 동작할 흐름 일듯함.

            TempNewChannel = "";
            tempChannelResetTimer.Stop();
        }

        private void InitializeButtonMappings()
        {
            // Define the character sets for each button (phone keypad style)
            buttonMappings = new Dictionary<int, string[]>
            {
                { 0, new string[] { "0", "*", "_", "/" } },     // 0, *, _, /
                { 1, new string[] { "1", " ", "Q", "Z" } },     // 1, " ", Q, Z
                { 2, new string[] { "2", "A", "B", "C" } },     // 2, A, B, C
                { 3, new string[] { "3", "D", "E", "F" } },     // 3, D, E, F
                { 4, new string[] { "4", "G", "H", "I" } },     // 4, G, H, I
                { 5, new string[] { "5", "J", "K", "L" } },     // 5, J, K, L
                { 6, new string[] { "6", "M", "N", "O" } },     // 6, M, N, O
                { 7, new string[] { "7", "P", "R", "S" } },     // 7, P, R, S
                { 8, new string[] { "8", "T", "U", "V" } },     // 8, T, U, V
                { 9, new string[] { "9", "W", "X", "Y" } }      // 9, W, X, Y
            };

            // Keep track of the current cycle state for each button
            buttonState = new Dictionary<int, int>();
            for (int i = 0; i <= 9; i++)
            {
                buttonState[i] = 0;
            }
        }

        private void ClearTempNewChannel(object obj)
        {
            //Clear는 채널 입력에 대한 Clear가 아닌 기능들에 대한 Clear임 (ex : DSC Call 소리 종료, 입력 흐름 취소 등)
        }

        private void EnterValue(object obj)
        {
            if (isInputText)
            {
                EnterButton_Click(); //문자 입력 흐름인 경우 키입력 흐름으로 사용 // 입력 확정을 위해 사용됨.
            }
            else
            {
                // Power는 구현하는 디지털 VHF에서는 의미 없음.

                isPower25W = !isPower25W;

                string power = "";

                if (isPower25W)
                {
                    power = "25W";
                }
                else
                {
                    power = "01W";
                }

                mediator.NotifyPowerChanged(power);
            }
        }

        private void EnterButton_Click()
        {
            // Confirm the current input and move the caret to the next position
            lastFocusedPosition = inputText.Length; // Move to the end of the text
            //InputTextBox.CaretIndex = lastFocusedPosition;

            inputInProgress = false; // Allow new input after ENT is pressed
            firstInputIsNumber = true; // Reset to ensure first input is number on next click
        }

        private void VisibleMainScreen(object obj)
        {
            mediator.NotifyShowMainScreen();
            //ShowMainScreen();
        }

        private void VisibleDscMenuList(object obj)
        {
        }

        private void VisibleMenuList(object obj)
        {
        }

        private void SetTempChannel(object obj) // 버튼을 통한 새로 사용될 임시 채널 입력
        {
            if (isInputText)
            {
                InputTextInfo(Convert.ToInt32(obj));
            }
            else
            {
                if (isFunction)
                {
                    DoFunction(Convert.ToInt32(obj));
                }
                else
                {
                    // Temp Channel 정보를 사용하는 이유는 사용될 수 없는 채널을 입력할 경우 이전에 사용한 채널로 돌아가기 위해 사용할 예정임. (현재 해당 기능은 구현하지 않은 상태)
                    SetTempNewChannel(obj.ToString());

                    SetVHFChannel();
                }
            }
        }

        private void SetVHFChannel()
        {
            if ((TempNewChannel == "") && (VHFChannel != ""))
            {
                //새로 입력한 임시 채널은 없지만, 현재 사용중인 채널이 있는 경우
                SetChannel(VHFChannel);
            }
            else
            {
                // 일반적인 흐름
                SetChannel(TempNewChannel);
            }
        }

        private void SetChannel(string channel) // 실제 사용되는 채널을 입력 (임시 채널 정보를 최종 사용)
        {
            //VHFChannel = channel;

            ClearTempNewChannelInfo();

            //SendSelectedChannel();

            //mediator.NotifyChannelChanged(VHFChannel);
            mediator.NotifyChannelChanged(channel);
        }

        private void ClearTempNewChannelInfo()
        {
            //TempNewChannel = "";

            tempChannelResetTimer.Stop(); //TempNewChannel를 바로 "" 초기화 하는 것이 아닌 특정 초(ex :2초) 이내에서 추가 입력이 없을 경우 초기화
            tempChannelResetTimer.Start();

            //ShowMainScreen();
            mediator.NotifyShowMainScreen();
        }

        //숫자 버튼을 선택하여 해당 숫자를 새로입력 할 채널의 정보에 추가 (SetChannel Method가 호출되기 전까진 임시 채널로 사용됨)
        private void SetTempNewChannel(string Number)
        {
            if (TempNewChannel.Length < 3)  // Limit channel length to 3 digits
            {
                TempNewChannel += Number;
                //ShowMainScreen();
                mediator.NotifyShowMainScreen();
            }
        }

        private void InputTextInfo(int buttonNumber)
        {
            // If user presses a different button before pressing ENT, reset input state (ENT 버튼 클리전 현재 로테이션 중인 키패드가 아닌 다른 키를 누를 경우)
            if (inputInProgress && buttonNumber != currentButton)
            {
                buttonState[currentButton] = 0; // Reset previous button rotation
                firstInputIsNumber = true; // Reset first input to be a number for the new button
                inputInProgress = false;
            }

            // Get the current character set for the button
            string[] charSet = buttonMappings[buttonNumber];

            // If it's the first input, ensure the number is input first
            if (firstInputIsNumber)
            {
                buttonState[buttonNumber] = 0; // Reset to the first character (the number)
                firstInputIsNumber = false; // After this, allow rotation to letters
            }
            else
            {
                // Rotate through the characters after the first input
                buttonState[buttonNumber] = (buttonState[buttonNumber] + 1) % charSet.Length;
            }

            string selectedChar = charSet[buttonState[buttonNumber]];

            // Replace or insert the selected character at the last focused position
            if (lastFocusedPosition < inputText.Length)
            {
                // If a character is already present, replace it
                inputText = inputText.Remove(lastFocusedPosition, 1).Insert(lastFocusedPosition, selectedChar);
            }
            else
            {
                // Otherwise, add the character
                inputText += selectedChar;
            }

            // Keep track of the button and that input is in progress until ENT is pressed
            currentButton = buttonNumber;
            inputInProgress = true;

            if (inputDscTextType == Mediator.InputTextType.mmsi)
            {
                mediator.NotifyMmsiChanged(inputText);
            }
            else
            {
                mediator.NotifyDscTextboxValueChanged(inputText);
            }
        }

        // UPDATED method to handle the DEL button click
        private void DelButton_Click()
        {
            //if (TxtInputText.Length > 0)
            //{
            //    // Remove the last character from the TextBox
            //    TxtInputText = TxtInputText.Remove(TxtInputText.Length - 1, 1);
            //    lastFocusedPosition = TxtInputText.Length; // Update the caret position to the end

            //    //InputTextBox.CaretIndex = lastFocusedPosition;
            //}
        }

        private void DelButton_func(object obj)
        {
            DelButton_Click();
        }

        private void SetFunction(object obj)
        {
            isFunction = !isFunction;
        }

        private void DoFunction(int buttonNumber)
        {
            switch (buttonNumber)
            {
                case 1: //DUAL : dual watch 기능 ON/OFF
                    watchType = GetWatchType(WatchType.DualWatch);
                    break;
                case 2: //TRI : triple watch 기능 ON/OFF
                    //triple watch 기능
                    watchType = GetWatchType(WatchType.TripleWatch);

                    break;
                case 3: //DIM : BACK-LIGHT(내부 조명) 4단계 //프로그램에서는 필요 없음.
                    break;
                case 4: //SCN : 화면에 ALL 이 반전되면서 모든 채널을 SCANNING 한다. (종료 시 CLR)
                    //isAllWatch = !isAllWatch;
                    watchType = GetWatchType(WatchType.AllWatch);
                    break;
                case 5: //TSCN : 화면에 ALL 이 TAG로 반전되면서 TAG를 선택한 채널만 SCANNING 한다.
                    //isTagWatch = !isTagWatch;
                    watchType = GetWatchType(WatchType.TagWatch);

                    break;
                case 6: //TAG : 선택된 채널에 TAG를 설정할 수 있다.
                    break;
                case 7: //ITU : ITU 채널 선택 (ITU 모드로 설정)
                    channelMode = ChannelMode.ITU;
                    break;
                case 8: //USA : USA 채널 선택 (USA 모드로 설정)
                    channelMode = ChannelMode.USA;
                    break;
                case 9: //CAN : CANADA 채널 선택 (CAN 모드로 설정)
                    channelMode = ChannelMode.CANADA;
                    break;
                case 0: //TEST : 장비 자체 TEST 메뉴로 전환된다.
                    break;
                default:
                    break;
            }

            mediator.NotifyWatchTypeChanged(watchType);
            mediator.NotifyChannelModeChanged(GetChannelModeValue());
        }

        private WatchType GetWatchType(WatchType newWatchType)
        {
            if(watchType == newWatchType)
            {
                return WatchType.Normal;
            }
            else
            {
                return newWatchType;
            }
        }

        private string GetChannelModeValue()
        {
            string typeValue = ""; // Normal

            switch (channelMode)
            {
                case ChannelMode.ITU :
                    typeValue = "ITU";
                    break;
                case ChannelMode.USA:
                    typeValue = "USA";
                    break;
                case ChannelMode.CANADA:
                    typeValue = "CANADA";
                    break;
                default:
                    break;
            }

            return typeValue;
        }
    }
}
