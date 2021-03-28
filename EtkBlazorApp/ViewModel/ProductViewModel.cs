﻿using System;
using System.ComponentModel.DataAnnotations;

namespace EtkBlazorApp.ViewModel
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Sku { get; set; }
        public string Uri { get; set; }

        public decimal BasePrice { get; set; }

        public string BasePriceCurrency { get; set; }

        public string StockStatus { get; set; }
        
        public decimal Price { get; set; }
        
        [Range(0, 9999, ErrorMessage = "Количество должно быть в диапазоне от 0 до 9999")]
        public int Quantity { get; set; }

        public bool IsPriceInCurrency { get; set; }
    }
}
