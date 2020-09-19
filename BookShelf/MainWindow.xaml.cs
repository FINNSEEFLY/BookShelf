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
    public partial class MainWindow : Window
    {
        ObservableCollection<Book> Books = new ObservableCollection<Book>
        {
            new Book { Isbn = "9785170166824", Author = "Чак Паланик", Title = "Бойцовский клуб", Year = 1996, Publisher = "ACT", Price = 18 },
            new Book { Isbn = "0201835959", Author = "Фредерик Брукс", Title = "Мифический человеко-месяц", Year = 1975, Publisher = "Addison–Wesley", Price = 71 },
            new Book { Isbn = "9785699923595", Author = "Рэй Брэдбери", Title = "Fahrenheit 451", Year = 2017, Publisher = "Эксмо", Price = 19 },
            new Book { Isbn = "9785170800858", Author = "Хаксли Олдос", Title = "О дивный новый мир", Year = 2014, Publisher = "ACT", Price = 9 }
            //new Book { Isbn = "9785446109609", Author = "Роберт Мартин", Title = "Чистый код. Создание, анализ и рефакторинг", Year = 2019, Publisher="Питер", Price = 84}
        };
        const string RegExp = @"\b[0-9]{9,}[X]?\b";
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        OpenFileDialog openFileDialog = new OpenFileDialog();


        public MainWindow()
        {
            InitializeComponent();
            GridBooks.ItemsSource = Books;
            saveFileDialog.FileName = "Bookshelf";
            saveFileDialog.Title = "Сохранить библиотеку";
            saveFileDialog.Filter = "Файл библиотеки (*.bsh)|*.bsh";
            saveFileDialog.DefaultExt = ".bsh";
            openFileDialog.FileName = "Bookshelf";
            openFileDialog.Title = "Загрузить библиотеку";
            openFileDialog.Filter = "Файл библиотеки (*.bsh)|*.bsh";
            openFileDialog.DefaultExt = ".bsh";
            openFileDialog.Multiselect = false;
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
                    foreach (var book in Books)
                    {
                        if (book.Isbn.Equals(isbn))
                        {
                            MessageBox.Show("Книга с таким ISBN уже добавлена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("В введенном ISBN скорее ввсего допущена ошибка\nПерепроверьте данные", "Не верный ISBN", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var newBook = new Book()
                {
                    Isbn = isbn,
                    Title = title,
                    Author = author,
                    Price = Convert.ToInt32(price),
                    Publisher = publisher,
                    Year = Convert.ToInt32(year)
                };
                Books.Add(newBook);
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

        private void ValidateISBN(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789 X-_".IndexOf(e.Text) < 0;
        }

        private bool IsFieldsNotClear()
        {
            if (TbIsbn.Text.Trim().Length != 0 &&
                TbAuthor.Text.Trim().Length != 0 &&
                TbPrice.Text.Trim().Length != 0 &&
                TbPublisher.Text.Trim().Length != 0 &&
                TbYear.Text.Trim().Length != 0
                )
            {
                return true;
            }
            else return false;

        }
        private bool IsIsbnCorrect(string isbn)
        {
            if (Regex.IsMatch(isbn, RegExp))
            {
                if (isbn.Length == 10)
                {
                    int checkSum = 0;
                    for (int i = 0; i < 9; i++)
                    {
                        checkSum += int.Parse(isbn[i].ToString()) * (i + 1);
                    }
                    checkSum %= 11;
                    if (checkSum == int.Parse(isbn[9].ToString()) || (checkSum == 10 && isbn[9] == 'X'))
                        return true;
                    else
                        return false;
                }
                /*  Все пункты выполнять без контрольной суммы
                 *  1) Сложить все цифры на четных позициях
                 *  2) Умножить результат прошлого пункта на 3 
                 *  3) Сложить все цифры на нечетных позициях
                 *  4) Сложить числа из пункта 3 и 2
                 *  5) Взять последнюю цифру (Number mod 10)
                 *  6) Вычитаем из 10 результат 5-го пункта если он не равен 0
                */
                else if (isbn.Length == 13)
                {
                    int checkSum = 0;
                    for (int i = 0; i < 12; i++)
                    {
                        checkSum += i % 2 == 0 ? int.Parse(isbn[i].ToString()) : int.Parse((isbn[i]).ToString()) * 3;
                    }
                    checkSum %= 10;
                    if (checkSum != 0)
                        checkSum = 10 - checkSum;
                    if (checkSum == int.Parse(isbn[12].ToString()) || (checkSum == 10 && isbn[12] == 'X'))
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private string FixIsbn(string isbn)
        {
            return isbn.Replace("-", "").Replace(" ", "");
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            Book selectedBook = (Book)GridBooks.SelectedItem;
            if (selectedBook != null)
            {
                Books.Remove(selectedBook);
            }
        }

        private void SaveBookshelf(object sender, RoutedEventArgs e)
        {
            if (saveFileDialog.ShowDialog() == false)
                return;
            var formatter = new BinaryFormatter();
            using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fileStream, Books);
            }
        }

        private void LoadBookshelf(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == false)
                return;
            var formatter = new BinaryFormatter();
            using (var fileStream = new FileStream(openFileDialog.FileName, FileMode.Open))
            {
                try
                {
                    var loadedBookshelf = new ObservableCollection<Book>();
                    loadedBookshelf = (ObservableCollection<Book>)formatter.Deserialize(fileStream);
                    Books.Clear();
                    foreach (var book in loadedBookshelf)
                    {
                        Books.Add(book);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Файл поврежден, чтение невозможно \n" + "Расшифровка ошибки: \n" + exception.Message, "Ошибка чтения файла", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SortBooksByIsbnAscending(object sender, RoutedEventArgs e)
        {
            GridBooks.ItemsSource = Books = new ObservableCollection<Book>(Books.OrderBy(x => x.Isbn));
        }

        private void SortBooksByIsbnDescending(object sender, RoutedEventArgs e)
        {
            GridBooks.ItemsSource = Books = new ObservableCollection<Book>(Books.OrderByDescending(x => x.Isbn));
        }

        private void SortBooksByPriceAscending(object sender, RoutedEventArgs e)
        {
            GridBooks.ItemsSource = Books = new ObservableCollection<Book>(Books.OrderBy(x => x.Price));
        }

        private void SortBooksByPriceDescending(object sender, RoutedEventArgs e)
        {
            GridBooks.ItemsSource = Books = new ObservableCollection<Book>(Books.OrderByDescending(x => x.Price));
        }

        private void SortBooksByAuthorAscending(object sender, RoutedEventArgs e)
        {
            GridBooks.ItemsSource = Books = new ObservableCollection<Book>(Books.OrderBy(x => x.Author));
        }

        private void SortBooksByAuthorDescending(object sender, RoutedEventArgs e)
        {
            GridBooks.ItemsSource = Books = new ObservableCollection<Book>(Books.OrderByDescending(x => x.Author));
        }

        private void SortBooksByTitleAscending(object sender, RoutedEventArgs e)
        {
            GridBooks.ItemsSource = Books = new ObservableCollection<Book>(Books.OrderBy(x => x.Title));
        }

        private void SortBooksByTitleDescending(object sender, RoutedEventArgs e)
        {
            GridBooks.ItemsSource = Books = new ObservableCollection<Book>(Books.OrderByDescending(x => x.Title));
        }

        private void SortBooksByYearAscending(object sender, RoutedEventArgs e)
        {
            GridBooks.ItemsSource = Books = new ObservableCollection<Book>(Books.OrderBy(x => x.Year));
        }

        private void SortBooksByYearDescending(object sender, RoutedEventArgs e)
        {
            GridBooks.ItemsSource = Books = new ObservableCollection<Book>(Books.OrderByDescending(x => x.Year));
        }

        private void SortBooksByPublisherAscending(object sender, RoutedEventArgs e)
        {
            GridBooks.ItemsSource = Books = new ObservableCollection<Book>(Books.OrderBy(x => x.Publisher));
        }

        private void SortBooksByPublisherDescending(object sender, RoutedEventArgs e)
        {
            GridBooks.ItemsSource = Books = new ObservableCollection<Book>(Books.OrderByDescending(x => x.Publisher));
        }
    }
}

