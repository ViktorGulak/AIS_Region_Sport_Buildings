using System;

namespace AIS_Region_Sport_Buildings.Models
{
    class LocalityType
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // Конструктор по умолчанию (обязательно для WPF)
        public LocalityType() { }
        public LocalityType(int id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
