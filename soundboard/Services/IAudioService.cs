using System.Collections.Generic;

namespace soundboard.Services
{
    public interface IAudioService
    {
        void PlaySound(string filePath, float volume);
        void StopSound();
        void SetVolume(float volume);

        // Новые методы
        List<string> GetOutputDevices();
        void SetOutputDevice(int deviceIndex);
    }
}