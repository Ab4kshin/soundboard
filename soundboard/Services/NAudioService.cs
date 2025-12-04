using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace soundboard.Services
{
    public class NAudioService : IAudioService
    {
        private WaveOutEvent? _outputDevice;
        private AudioFileReader? _audioFile;
        private int _currentDeviceIndex = -1; // -1 = Default Device

        public List<string> GetOutputDevices()
        {
            List<string> devices = new List<string>();
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var caps = WaveOut.GetCapabilities(i);
                devices.Add(caps.ProductName);
            }
            return devices;
        }

        public void SetOutputDevice(int deviceIndex)
        {
            _currentDeviceIndex = deviceIndex;
        }

        public void PlaySound(string filePath, float volume)
        {
            StopSound();

            if (!System.IO.File.Exists(filePath)) return;

            try
            {
                _outputDevice = new WaveOutEvent()
                {
                    DeviceNumber = _currentDeviceIndex
                };

                _audioFile = new AudioFileReader(filePath);
                _outputDevice.Volume = Math.Clamp(volume, 0.0f, 1.0f);
                _outputDevice.Init(_audioFile);
                _outputDevice.Play();

                _outputDevice.PlaybackStopped += (s, e) => StopSound();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        public void StopSound()
        {
            try { _outputDevice?.Stop(); } catch { }
            _outputDevice?.Dispose();
            _outputDevice = null;
            _audioFile?.Dispose();
            _audioFile = null;
        }

        public void SetVolume(float volume)
        {
            if (_outputDevice != null)
            {
                _outputDevice.Volume = Math.Clamp(volume, 0.0f, 1.0f);
            }
        }
    }
}