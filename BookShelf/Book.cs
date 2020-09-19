using System;
using System.Collections.Generic;
using System.Text;

namespace BookShelf
{
    [Serializable]
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
