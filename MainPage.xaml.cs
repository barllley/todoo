using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace todoo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new TodoListViewModel();
        }

        private void OnEntryCompleted(object sender, EventArgs e)
        {
            var entry = (Entry)sender;
            var viewModel = (TodoListViewModel)BindingContext;

            // проверка, можно ли добавить задачу
            if (viewModel.AddCommand.CanExecute(entry.Text))
            {
                viewModel.AddCommand.Execute(entry.Text); // добавление задачи
            }

            entry.Text = string.Empty; // очистка поля
        }

    }

    public class TodoItem : INotifyPropertyChanged
    {
        private bool _isCompleted; // состояния задачи

        public required string Text { get; set; }

        // состояния выполнения задачи, truе если задача выполнена
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged(); // уведомляем интерфейс об изменении
                }
            }
        }

        // Событие, которое вызывается при изменении свойства.
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TodoListViewModel : INotifyPropertyChanged
    {
        public TodoListViewModel()
        {
            Items = new ObservableCollection<TodoItem> { new TodoItem { Text = "test" } }; //тестовая задача

            AddCommand = new Command<string>(AddItem, CanAddItem);

            RemoveCommand = new Command<TodoItem>(RemoveItem);
        }

        public ObservableCollection<TodoItem> Items { get; }

        private string _newItemText; // поле для хранения текста новой задачи

        public string NewItemText
        {
            get => _newItemText;
            set
            {
                if (SetField(ref _newItemText, value))
                {
                    ((Command)AddCommand).ChangeCanExecute(); // доступность добавление задачи
                }
            }
        }


        public ICommand AddCommand { get; }

        private void AddItem(string itemText)
        {
            if (ValidateItemText(itemText)) // проверка валидации
            {
                Items.Add(new TodoItem { Text = itemText });
                NewItemText = string.Empty;
            }
        }

        private bool CanAddItem(string itemText) => !string.IsNullOrWhiteSpace(itemText) && ValidateItemText(itemText);

        public ICommand RemoveCommand { get; }

        private void RemoveItem(TodoItem item)
        {
            Items.Remove(item);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        //вызов события изменения свойства
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // обновление значения поля
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private bool ValidateItemText(string itemText)
        {
            return itemText.Length <= 50;
        }
    }
}
