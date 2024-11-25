using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHF_Client.ViewModel
{
    public class VolumeControlViewModel : ViewModelBase
    {
        private MMDeviceEnumerator deviceEnumerator; // 스피커 볼륨 UP/DOWN을 위해 사용됨.
        private MMDevice device;

        private float volumeValue;
        public float VolumeValue
        {
            get => volumeValue;
            set => SetProperty(ref volumeValue, value);
        }

        public VolumeControlViewModel()
        {
            InitAudio();
        }

        private void InitAudio()
        {
            deviceEnumerator = new MMDeviceEnumerator();
            device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            VolumeValue = device.AudioEndpointVolume.MasterVolumeLevelScalar; // 현재 볼륨으로 설정
        }

        public void ControlVolume(float value)
        {
            device.AudioEndpointVolume.MasterVolumeLevelScalar = value;
        }
    }
}
