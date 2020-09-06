using System;
using System.Collections.Generic;
using System.Text;

namespace BookShelf
{
    class Book
    {

        public string Isbn { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public int Year { get; set; }

        // Через Culture сделать выбор форматирования валюты
        public int Price { get; set; }

        public override string ToString()
        {
            return "\"" + Title + "\" (" + Year + ") " + Author; 
        }

    }
}
