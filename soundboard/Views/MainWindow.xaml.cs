using System.Windows;
using soundboard.ViewModels; // Не забудь этот using

namespace soundboard.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Этот метод срабатывает, когда ты бросаешь файл в окно
        private void Window_Drop(object sender, DragEventArgs e)
        {
            // Получаем доступ к нашей ViewModel
            if (DataContext is MainWindowViewModel vm)
            {
                // Вызываем команду обработки файла вручную
                vm.DropFileCommand.Execute(e);
            }
        }
    }
}