using System;
using System.IO;

namespace VHF_Client
{
    public class IniFileReader
    {
        private readonly string filePath;

        public bool isSetDbInfoByFile = false;

        public string serverIp = "";
        public int serverPort = 5007;    //def

        public int localPort = 5001;     //def
        public string clientName = "";      // LocalName

        public bool isDualWatch = false;
        public bool isTripleWatch = false;
        public bool isAllWatch = false;

        // true  : 키보드 Ctrl 키 눌림 여부에 따라 음성 전송 (PPT 마이크가 없는 경우 이어폰의 마이크를 사용하며 PPT 버튼 대신 Ctrl Key 눌림으로 대신함)
        // false : PTT 마이크의 PTT 버튼이 눌린상태에서 유효한 음성(진폭)이 있을 경우 음성 전송
        public bool isVoiceSendingByKeyboard = false;

        public IniFileReader(string filePath)
        {
            this.filePath = filePath;

            SetInfo();
        }

        private void SetInfo()
        {
            serverIp = ReadValue("ServerInfo", "ServerIp");
            serverPort = Convert.ToInt32(ReadValue("ServerInfo", "ServerPort"));

            localPort = Convert.ToInt32(ReadValue("LocalInfo", "LocalPort"));
            clientName = ReadValue("LocalInfo", "ClientName");

            if ("true" == ReadValue("MultiInfo", "IsDualWatch"))
            {
                isDualWatch = true;
            }
            else
            {
                isDualWatch = false;
            }

            if ("true" == ReadValue("MultiInfo", "IsTripleWatch"))
            {
                isTripleWatch = true;
            }
            else
            {
                isTripleWatch = false;
            }

            if ("true" == ReadValue("MultiInfo", "IsAllWatch"))
            {
                isAllWatch = true;
            }
            else
            {
                isAllWatch = false;
            }

            if ("true" == ReadValue("VoiceControlInfo", "IsVoiceSendingByKeyboard"))
            {
                isVoiceSendingByKeyboard = true;
            }
            else
            {
                isVoiceSendingByKeyboard = false;
            }
        }

        private string ReadValue(string section, string key)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);

                string currentSection = null;

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();

                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                    {
                        currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    }
                    else if (currentSection == section)
                    {
                        var parts = trimmedLine.Split('=');

                        if (parts.Length == 2)
                        {
                            var currentKey = parts[0].Trim();
                            var value = parts[1].Trim();

                            if (currentKey == key)
                            {
                                return value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., file not found, permission issues, etc.
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            // Return null if the section or key is not found
            return null;
        }
    }
}
