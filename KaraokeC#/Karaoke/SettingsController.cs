using System;
using NAudio.CoreAudioApi;

namespace Karaoke
{
    internal class SettingsController
    {
        public int SamplingRate { get; private set; } = 16384;
        public int FFTSize { get; private set; } = 2048;
        public int MeasuresPerSec { get; private set; } = 30;
        public double MinEnergy { get; private set; } = 1e-4;
        public int Volume { get; private set; } = 100;
        public MMDevice SelectedInputDevice { get; private set; }
        public MMDevice SelectedOutputDevice { get; private set; }

        // Clear stored device references (useful after device re-enumeration)
        public void ClearDeviceSelection()
        {
            SelectedInputDevice = null;
            SelectedOutputDevice = null;
        }

        // Set devices explicitly (used when user selects from list)
        public void SetSelectedDevices(MMDevice input, MMDevice output)
        {
            SelectedInputDevice = input;
            SelectedOutputDevice = output;
        }
        public string SelectedSongName { get; private set; }
        public float Latency { get; set; } = 0f;

        public void UpdateFromUI(string samplingRateText, string fftSizeText, int measuresPerSecValue, double minEnergyValue,
                                 int volumeValue, object inputDeviceObj, object outputDeviceObj, decimal latencyValue, object selectedSongObj)
        {
            if (int.TryParse(samplingRateText, out var sr)) SamplingRate = sr;
            if (int.TryParse(fftSizeText, out var fs)) FFTSize = fs;
            MeasuresPerSec = measuresPerSecValue;
            MinEnergy = minEnergyValue;
            Volume = volumeValue;

            SelectedInputDevice = inputDeviceObj as MMDevice;
            SelectedOutputDevice = outputDeviceObj as MMDevice;
            SelectedSongName = selectedSongObj as string;

            Latency = (float)latencyValue;
        }
    }
}
