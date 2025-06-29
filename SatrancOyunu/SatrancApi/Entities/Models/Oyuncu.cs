﻿using SatrancApi.Entities.Models;

namespace SatrancAPI.Entities.Models
{
    public class Oyuncu:BaseEntitiy
    {
        public Guid Id { get; set; }
        public string ?isim { get; set; }
        public string ?email {  get; set; }
        public string ?Sifre { get; set; } 
        public Renk renk { get; set; }
        // Navigation properties
        public ICollection<Hamle>? Hamleler { get; set; }  // Oyuncu ile birden fazla hamle ilişkisi
        public ICollection<Tas> ?Taslar { get; set; }  // Oyuncu ile birden fazla taş ilişkisi
    }
}
