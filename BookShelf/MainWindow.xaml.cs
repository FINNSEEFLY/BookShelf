using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;
using System.IO;
using System.Runtime.CompilerServices;

namespace BookShelf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ObservableCollection<Book> _books = new ObservableCollection<Book>
        {
            new Book("9785170166824", "Бойцовский клуб", "Чак Паланик", "ACT", 1996, 18),
            new Book("0201835959", "Мифический человеко-месяц", "Фредерик Брукс", "Addison", 1975, 71),
            new Book("9785699923595", "Fahrenheit 451", "Рэй Брэдбери", "Эксмо", 2017, 19),
            new Book("9785170800858", "О дивный новый мир", "Хаксли Олдос", "ACT", 2014, 9)
        };
        //new Book( "9785446109609","Чистый код. Создание, анализ и рефакторинг","Роберт Мартин","Питер",2019,84)
        
        private ObservableCollection<Book> _collection = new ObservableCollection<Book>();
        private const string RegExp = @"\b[0-9]{9,}[X]?\b";
        private readonly SaveFileDialog _saveFileDialog = new SaveFileDialog();
        private readonly OpenFileDialog _openFileDialog = new OpenFileDialog();

        private delegate ObservableCollection<Book> BookShelfSort(ObservableCollection<Book> x);

        private Dictionary<string, BookShelfSort> _sortDictionary =
            new Dictionary<string, BookShelfSort>
            {
                ["IsbnAscending"] = books => new ObservableCollection<Book>(books.OrderBy(x=> x.Isbn)),
                ["IsbnDescending"] = books=> new ObservableCollection<Book>(books.OrderByDescending(x=>x.Isbn)),
                ["AuthorAscending"] = books => new ObservableCollection<Book>(books.OrderBy(x=>x.Author)),
                ["AuthorDescending"] = books=> new ObservableCollection<Book>(books.OrderByDescending(x=>x.Author)),
                ["TitleAscending"] = books=> new ObservableCollection<Book>(books.OrderBy(x=>x.Title)),
                ["TitleDescending"] = books=> new ObservableCollection<Book>(books.OrderByDescending(x=>x.Title)),
                ["YearAscending"] = books=> new ObservableCollection<Book>(books.OrderBy(x=>x.Year)),
                ["YearDescending"] = books=> new ObservableCollection<Book>(books.OrderByDescending(x=>x.Year)),
                ["PublisherAscending"] = books=> new ObservableCollection<Book>(books.OrderBy(x=>x.Publisher)), 
                ["PublisherDescending"] = books=> new ObservableCollection<Book>(books.OrderByDescending(x=>x.Publisher)),
                ["PriceAscending"] = books=> new ObservableCollection<Book>(books.OrderBy(x=>x.Price)),
                ["PriceDescending"] = books=> new ObservableCollection<Book>(books.OrderByDescending(x=>x.Price)) 
            };

        public MainWindow()
        {
            InitializeComponent();
            GridBooks.ItemsSource = _books;
            InitializeFileDialogs();
        }

        private void InitializeFileDialogs()
        {
            _saveFileDialog.FileName = "Bookshelf";
            _saveFileDialog.Title = "Сохранить библиотеку";
            _saveFileDialog.Filter = "Файл библиотеки (*.bsh)|*.bsh";
            _saveFileDialog.DefaultExt = ".bsh";
            _openFileDialog.FileName = "Bookshelf";
            _openFileDialog.Title = "Загрузить библиотеку";
            _openFileDialog.Filter = "Файл библиотеки (*.bsh)|*.bsh";
            _openFileDialog.DefaultExt = ".bsh";
            _openFileDialog.Multiselect = false;
        }           

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            MainWindowFrame.Close();
        }

        private void AddBookClick(object sender, RoutedEventArgs e)
        {
            var isbn = TbIsbn.Text.Trim();
            var author = TbAuthor.Text.Trim();
            var title = TbTitle.Text.Trim();
            var price = TbPrice.Text.Trim();
            var publisher = TbPublisher.Text.Trim();
            var year = TbYear.Text.Trim();
            if (IsFieldsNotClear())
            {
                isbn = FixIsbn(isbn);
                if (IsIsbnCorrect(isbn))
                {
                    if (_books.Any(book => book.Isbn.Equals(isbn)))
                    {
                        MessageBox.Show("Книга с таким ISBN уже добавлена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("В введенном ISBN скорее ввсего допущена ошибка\nПерепроверьте данные", "Не верный ISBN", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var newBook = new Book(isbn,title,author,publisher,Convert.ToInt32(year),Convert.ToInt32(price));
                _books.Add(newBook);
            }
            else
            {
                MessageBox.Show("Судя по всему, не все поля заполнены корректно", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ValidateNumber(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789".IndexOf(e.Text) < 0;
        }

        private void ValidateIsbn(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789 X-_".IndexOf(e.Text) < 0;
        }

        private bool IsFieldsNotClear()
        {
            return TbIsbn.Text.Trim().Length != 0 &&
                   TbAuthor.Text.Trim().Length != 0 &&
                   TbPrice.Text.Trim().Length != 0 &&
                   TbPublisher.Text.Trim().Length != 0 &&
                   TbYear.Text.Trim().Length != 0;
        }
        private static bool IsIsbnCorrect(string isbn)
        {
            if (!Regex.IsMatch(isbn, RegExp)) return false;
            switch (isbn.Length)
            {
                case 10:
                {
                    var checkSum = 0;
                    for (var i = 0; i < 9; i++)
                    {
                        checkSum += int.Parse(isbn[i].ToString()) * (i + 1);
                    }
                    checkSum %= 11;
                    return checkSum == int.Parse(isbn[9].ToString()) || (checkSum == 10 && isbn[9] == 'X');
                }
                /*  Алгоритм вычисления контрольной суммы ISBN (13-ти символьный)
                     *  Все пункты выполнять без контрольной суммы
                     *  1) Сложить все цифры на четных позициях
                     *  2) Умножить результат прошлого пункта на 3 
                     *  3) Сложить все цифры на нечетных позициях
                     *  4) Сложить числа из пункта 3 и 2
                     *  5) Взять последнюю цифру (Number mod 10)
                     *  6) Вычитаем из 10 результат 5-го пункта если он не равен 0
                    */
                case 13:
                {
                    var checkSum = 0;
                    for (var i = 0; i < 12; i++)
                    {
                        checkSum += i % 2 == 0 ? int.Parse(isbn[i].ToString()) : int.Parse((isbn[i]).ToString()) * 3;
                    }
                    checkSum %= 10;
                    if (checkSum != 0)
                        checkSum = 10 - checkSum;
                    return checkSum == int.Parse(isbn[12].ToString());
                }
                default:
                    return false;
            }
        }

        private static string FixIsbn(string isbn)
        {
            return isbn.Replace("-", "").Replace(" ", "");
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            var selectedBook = (Book)GridBooks.SelectedItem;
            if (selectedBook != null)
            {
                _books.Remove(selectedBook);
            }
        }

        private void SaveBookshelf(object sender, RoutedEventArgs e)
        {
            if (_saveFileDialog.ShowDialog() == false)
                return;
            var formatter = new BinaryFormatter();
            using var fileStream = new FileStream(_saveFileDialog.FileName, FileMode.OpenOrCreate);
            formatter.Serialize(fileStream, _books);
        }

        private void LoadBookshelf(object sender, RoutedEventArgs e)
        {
            if (_openFileDialog.ShowDialog() == false)
                return;
            var formatter = new BinaryFormatter();
            using var fileStream = new FileStream(_openFileDialog.FileName, FileMode.Open);
            try
            {
                var loadedBookshelf = (ObservableCollection<Book>)formatter.Deserialize(fileStream);
                _books.Clear();
                foreach (Book book in loadedBookshelf)
                {
                    _books.Add(book);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Файл поврежден, чтение невозможно \n" + "Расшифровка ошибки: \n" + exception.Message, "Ошибка чтения файла", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectSorting(object sender, RoutedEventArgs e)
        {
            var tag = ((MenuItem) e.OriginalSource).Tag;
            try
            {
                GridBooks.ItemsSource = _books = _sortDictionary[(string) tag](_books);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Stack Tace: \n"+ex.StackTrace+"\nException:\n"+ex.Message);
            }
        }
    }
}

