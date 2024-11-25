﻿using System;

namespace Common
{
    public class User
    {
        public int id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string src { get; set; }
        public string temp_src { get; set; }
        public User(int id,string login, string password, string src)
        {
            this.id = id;
            this.login = login;
            this.password = password;
            this.src = src;
            temp_src = src;
        }
        public User() { }
    }
}
