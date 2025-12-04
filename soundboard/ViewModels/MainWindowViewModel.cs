using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using soundboard.Models;
using soundboard.Services;
using soundboard.Views; // Для открытия окна редактирования
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows;

namespace soundboard.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IAudioService _audioService;
        private readonly IFilePickerService _filePickerService;
        private readonly GlobalHotkeyService _hotkeyService; // Новый сервис
        private const string SaveFileName = "sounds.json";

        public ObservableCollection<SoundItem> Sounds { get; set; }

        // Список устройств вывода
        public ObservableCollection<string> OutputDevices { get; set; }

        [ObservableProperty]
        private int _selectedDeviceIndex = 0;

        [ObservableProperty]
        private float _volume = 1.0f;

        [ObservableProperty]
        private string _searchText = "";

        public MainWindowViewModel()
        {
            _audioService = new NAudioService();
            _filePickerService = new WpfFilePickerService();
            _hotkeyService = new GlobalHotkeyService();

            Sounds = new ObservableCollection<SoundItem>();
            OutputDevices = new ObservableCollection<string>();

            // Загружаем список устройств
            var devices = _audioService.GetOutputDevices();
            if (devices.Count == 0) OutputDevices.Add("Default Device");
            else foreach (var d in devices) OutputDevices.Add(d);

            // Подписываемся на глобальные нажатия клавиш
            _hotkeyService.OnKeyPressed += OnGlobalKeyPressed;

            LoadSounds();
        }

        // Метод вызывается при смене текста поиска
        partial void OnSearchTextChanged(string value)
        {
            foreach (var sound in Sounds)
            {
                if (string.IsNullOrWhiteSpace(value)) sound.IsVisible = true;
                else sound.IsVisible = sound.Name.ToLower().Contains(value.ToLower());
            }
        }

        // Метод вызывается при смене устройства
        partial void OnSelectedDeviceIndexChanged(int value)
        {
            _audioService.SetOutputDevice(value);
        }

        partial void OnVolumeChanged(float value) => _audioService.SetVolume(value);

        // Обработка нажатия клавиш (даже если свернуто)
        private void OnGlobalKeyPressed(Key key)
        {
            // Ищем звук с такой клавишей
            var soundToPlay = Sounds.FirstOrDefault(s => s.Hotkey == key);
            if (soundToPlay != null)
            {
                // Запускаем в главном потоке UI
                Application.Current.Dispatcher.Invoke(() => PlaySound(soundToPlay));
            }
        }

        [RelayCommand]
        private void PlaySound(SoundItem sound)
        {
            if (sound != null && !string.IsNullOrEmpty(sound.FilePath))
                _audioService.PlaySound(sound.FilePath, Volume);
        }

        [RelayCommand]
        private void StopSound() => _audioService.StopSound();

        [RelayCommand]
        private void AddSound()
        {
            string? filePath = _filePickerService.PickAudioFile();
            if (filePath != null)
            {
                Sounds.Add(new SoundItem { Name = Path.GetFileNameWithoutExtension(filePath), FilePath = filePath });
                SaveSounds();
            }
        }

        [RelayCommand]
        private void EditSound(SoundItem sound)
        {
            // МЫ ИЗМЕНИЛИ ИМЯ КЛАССА ЗДЕСЬ:
            var editWindow = new SoundEditor(sound);

            editWindow.Owner = Application.Current.MainWindow;

            if (editWindow.ShowDialog() == true)
            {
                SaveSounds();
            }
        }

        [RelayCommand]
        private void RemoveSound(SoundItem sound)
        {
            if (Sounds.Contains(sound)) { Sounds.Remove(sound); SaveSounds(); }
        }

        // Поддержка Drag & Drop файлов
        [RelayCommand]
        private void DropFile(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext == ".mp3" || ext == ".wav")
                    {
                        Sounds.Add(new SoundItem { Name = Path.GetFileNameWithoutExtension(file), FilePath = file });
                    }
                }
                SaveSounds();
            }
        }

        private void SaveSounds()
        {
            try { File.WriteAllText(SaveFileName, JsonSerializer.Serialize(Sounds)); } catch { }
        }

        private void LoadSounds()
        {
            if (File.Exists(SaveFileName))
            {
                try
                {
                    var list = JsonSerializer.Deserialize<List<SoundItem>>(File.ReadAllText(SaveFileName));
                    if (list != null) foreach (var s in list) Sounds.Add(s);
                }
                catch { }
            }
        }
    }
}