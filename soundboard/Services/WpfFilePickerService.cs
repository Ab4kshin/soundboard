using Microsoft.Win32;

namespace soundboard.Services
{
    public class WpfFilePickerService : IFilePickerService
    {
        public string? PickAudioFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files (*.mp3, *.wav)|*.mp3;*.wav|All files (*.*)|*.*",
                Title = "Выберите аудиофайл"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }
    }
}