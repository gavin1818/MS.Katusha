﻿using System;

namespace MS.Katusha.Interfaces.Services.Models
{
    public class ApiUser
    {
        public Guid Guid { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}