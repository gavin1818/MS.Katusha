﻿using System.Linq;
using MS.Katusha.Domain.Entities;
using Raven.Client.Indexes;


namespace MS.Katusha.Repositories.RavenDB.Indexes
{
    public class StateFacetsIndex : AbstractIndexCreationTask<State>
    {
        public StateFacetsIndex()
        {
            Map = states => from p in states
                select new {
                    CountryCode = p.CountryCode, 
                    CityCode = p.CityCode, 
                    Gender = p.Gender, 
                    BodyBuild = p.BodyBuild, 
                    HairColor = p.HairColor, 
                    EyeColor = p.EyeColor, 
                    Height = p.Height,
                    BirthYear = p.BirthYear
                };
        }
    }
}