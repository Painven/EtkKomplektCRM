﻿namespace EtkBlazorApp.DataAccess.Entity
{
    public class MonobrandEntity
    {
        public int monobrand_id { get; set; }
        public int manufacturer_id { get; set; }
        public string manufacturer_name { get; set; }
        public string currency_code { get; set; }
        public string website { get; set; }
        public bool is_update_enabled { get; set; }
    }
}
