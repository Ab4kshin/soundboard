using System.ComponentModel;
using System.Windows.Input;
using System.Text.Json.Serialization;

namespace soundboard.Models
{
    public class SoundItem : INotifyPropertyChanged
    {
        // Инициализируем сразу, чтобы не было null
        private string _name = string.Empty;
        private Key _hotkey = Key.None;
        private bool _isVisible = true;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        // Инициализируем сразу
        public string FilePath { get; set; } = string.Empty;

        public Key Hotkey
        {
            get => _hotkey;
            set
            {
                _hotkey = value;
                OnPropertyChanged(nameof(Hotkey));
                OnPropertyChanged(nameof(HotkeyText));
            }
        }

        [JsonIgnore]
        public string HotkeyText => Hotkey == Key.None ? "" : Hotkey.ToString();

        [JsonIgnore]
        public bool IsVisible
        {
            get => _isVisible;
            set { _isVisible = value; OnPropertyChanged(nameof(IsVisible)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}