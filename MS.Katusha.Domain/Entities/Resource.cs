﻿using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Entities
{
    public class Resource : BaseModel
    {
        public string ResourceKey { get; set; }
        public string Value { get; set; }
        public byte Language { get; set; }

        [NotMapped]
        public string Key
        {
            get { return ResourceKey + Language.ToString(); }
        }
    }
}