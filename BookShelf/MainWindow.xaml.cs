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

namespace BookShelf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BookList Books = new BookList
        {
            new Book { Isbn = "9785170166824", Author = "Чак Паланик", Title = "Бойцовский клуб", Year = 1996, Publisher = "ACT", Price = 18},
            new Book { Isbn = "0201835959", Author = "Фредерик Брукс", Title = "Мифический человеко-месяц", Year = 1975, Publisher = "Addison–Wesley", Price = 71}
        };
        const string RegExp = @"\b[0-9]{9,}[X]?\b";

        public MainWindow()
        {
            InitializeComponent();
            GridBooks.ItemsSource = Books;
        }
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            MainWindowFrame.Close();
        }
        private void AddBookClick(object sender, RoutedEventArgs e)
        {
            var isbn = TbIsbn.Text.Trim();
            var author = TbAuthor.Text.Trim();
            var price = TbPrice.Text.Trim();
            var publisher = TbPublisher.Text.Trim();
            var year = TbYear.Text.Trim();
            if (IsFieldsNotClear())
            {
                isbn = FixIsbn(isbn);
                if (IsIsbnCorrect(isbn))
                {

                }
                else
                {
                    MessageBox.Show("В введенном ISBN скорее ввсего допущена ошибка\nПерепроверьте данные", "Не верный ISBN", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var newBook = new Book()
                {
                    Isbn = isbn,
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
        private void CheckingForNumber(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
        private void CheckingForNumberInISBN(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0) && e.Text != "X" && e.Text != "-" && e.Text != " ")
            {
                e.Handled = true;
            }
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
                        checkSum += Convert.ToInt32(isbn[i]) * (i + 1);
                    }
                    checkSum %= 11;
                    if (checkSum == isbn[9] || (checkSum == 10 && isbn[9] == 'X'))
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
                 *  6) Вычитаем из 10 результат 5-го пункта
                */
                else if (isbn.Length == 13)
                {
                    int checkSum = 0;
                    for (int i = 0; i < 12; i++)
                    {
                        checkSum += i % 2 == 0 ? int.Parse(isbn[i].ToString()) : int.Parse((isbn[i]).ToString()) * 3;
                    }
                    checkSum %= 10;
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
    }
}
