﻿using System;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class TokBoxSession
    {
        public string Id { get; set; }
        public Guid ProfileGuid { get; set; }
        public string IP { get; set; }
        public string SessionId { get; set; }
        public DateTime LastModified { get; set; }
    }
}