using System.Windows;
using System.Windows.Input;
using soundboard.Models;

namespace soundboard.Views
{
    public partial class SoundEditor : Window
    {
        public SoundItem Item { get; private set; }
        private bool _waitingForKey = false;

        public SoundEditor(SoundItem item)
        {
            InitializeComponent();
            Item = item;
            NameBox.Text = item.Name;
            HotkeyBtn.Content = item.Hotkey == Key.None ? "Нет клавиши" : item.Hotkey.ToString();

            this.KeyDown += SoundEditor_KeyDown;
        }

        private void HotkeyBtn_Click(object sender, RoutedEventArgs e)
        {
            _waitingForKey = true;
            HotkeyBtn.Content = "Нажми любую клавишу...";
        }

        private void SoundEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (_waitingForKey)
            {
                Item.Hotkey = e.Key;
                HotkeyBtn.Content = e.Key.ToString();
                _waitingForKey = false;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Item.Name = NameBox.Text;
            DialogResult = true;
            Close();
        }
    }
}