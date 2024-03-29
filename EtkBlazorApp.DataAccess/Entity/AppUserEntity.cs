﻿using System;

namespace EtkBlazorApp.DataAccess.Entity
{
    public class AppUserEntity
    {
        public int user_id { get; set; }
        public string group_name { get; set; }
        public int user_group_id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public DateTime creation_date { get; set; }
        public DateTime? last_login_date { get; set; }
        public string permission { get; set; }
        
        public bool status { get; set; }
        public string ip { get; set; }
    }

    public class AppUserGroupEntity
    {
        public int user_group_id { get; set; }
        public string name { get; set; }
        public string permission { get; set; }
    }
}
