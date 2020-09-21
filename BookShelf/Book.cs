using System;
using System.Collections.Generic;
using System.Text;

namespace BookShelf
{
    [Serializable]
    internal class Book
    {
        public string Isbn { get; }
        public string Title { get; }
        public string Author { get; }
        public string Publisher { get; }
        public int Year { get; }
        public double Price { get; }

        public Book()
        {
            this.Isbn = "";
            this.Title = "";
            this.Author = "";
            this.Publisher = "";
            this.Year = 0;
            this.Price = 0;
        }
        
        public Book(string isbn, string title, string author, string publisher, int year, double price)
        {
            this.Isbn = isbn;
            this.Title = title;
            this.Author = author;
            this.Publisher = publisher;
            this.Year = year;
            this.Price = price;
        }
        
        public override string ToString()
        {
            return "\"" + Title + "\" (" + Year + ") " + Author + "ISBN : " + Isbn;
        }

        public override int GetHashCode()
        {
            return string.GetHashCode(Isbn+Title+Author+Publisher+Year.ToString()+Price.ToString());
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

    }
}
