﻿using System;
using System.Diagnostics;
using System.Globalization;
using MS.Katusha.Domain;
using System.Data.Entity;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure;
using MS.Katusha.Repositories.DB;

namespace MS.Katusha.Test
{
    public class KatushaContextInitializerTest : DropCreateDatabaseAlways<KatushaDbContext> //DropCreateDatabaseAlways<KatushaContext>// DropCreateDatabaseIfModelChanges<KatushaContext>
    {
        private KatushaDbContext _dbContext;
        protected override void Seed(KatushaDbContext dbContext)
        {
            _dbContext = dbContext;
            CreateSampleData();
        }

        private void CreateSampleUser(Sex gender)
        {
            var userRepository = new UserRepositoryDB(_dbContext);
            var user = new User
            {
                Gender = (byte) gender,
                Email = "mertsakarya@bigmail.com",
                UserName = "mertsakarya"+DateTime.Now.Millisecond.ToString(CultureInfo.InvariantCulture),
                Password = "690514",
                EmailValidated = true,
                Expires = DateTime.Now.AddYears(1)
          };
            user = userRepository.Add(user);
            _dbContext.SaveChanges();

            if (gender == Sex.Male) CreateSampleMan(user, user.Guid);
            else CreateSampleGirl(user, user.Guid);
            Debug.WriteLine(user);
        }

        private void CreateSampleMan(User user, Guid guid)
        {
            long id = user.Id;
            var now = DateTime.Now.ToUniversalTime();
            var manRepository = new ProfileRepositoryDB(_dbContext);
            var stateRepository = new StateRepositoryDB(_dbContext);
            var countriesToVisitRepository = new CountriesToVisitRepositoryDB(_dbContext);
            var languagesSpokenRepository = new LanguagesSpokenRepositoryDB(_dbContext);
            var photoRepository = new PhotoRepositoryDB(_dbContext);
            var searchingForRepository = new SearchingForRepositoryDB(_dbContext);

            var man = new Profile
            {
                Gender = user.Gender,
                UserId = user.Id,
                Description = "TestMan" + id.ToString(CultureInfo.InvariantCulture),
                DickSize = (byte)DickSize.Medium,
                DickThickness = (byte)DickThickness.Thick,
                Height = 170,
                BirthYear = 1970,
                FriendlyName = "mertiko" + id.ToString(CultureInfo.InvariantCulture),
                City = "Istanbul" + id.ToString(CultureInfo.InvariantCulture),
                From = (byte)Country.Turkey,
            };

            man = manRepository.Add(man, guid);
            _dbContext.SaveChanges();

            man.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { ProfileId = man.Id, Language = (byte)Language.English }));
            man.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { ProfileId = man.Id, Language = (byte)Language.Russian }));
            man.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { ProfileId = man.Id, Language = (byte)Language.Turkish }));
            man.Searches.Add(searchingForRepository.Add(new SearchingFor { ProfileId = man.Id, Search = (byte)LookingFor.LongTimeRelationship }));
            man.Searches.Add(searchingForRepository.Add(new SearchingFor { ProfileId = man.Id, Search = (byte)LookingFor.Friend }));
            man.Searches.Add(searchingForRepository.Add(new SearchingFor { ProfileId = man.Id, Search = (byte)LookingFor.OneNight }));
            man.Searches.Add(searchingForRepository.Add(new SearchingFor { ProfileId = man.Id, Search = (byte)LookingFor.Sex }));
            var photo = photoRepository.Add(new Photo { ProfileId = man.Id, Description = "Açıklama" + id.ToString(CultureInfo.InvariantCulture) });
            var photo2 = photoRepository.Add(new Photo { ProfileId = man.Id, Description = "Açıklama2" + id.ToString(CultureInfo.InvariantCulture) });
            man.Photos.Add(photo);
            man.Photos.Add(photo2);

            man.CountriesToVisit.Add(countriesToVisitRepository.Add(new CountriesToVisit { ProfileId = man.Id, Country = (byte)Country.Turkey }));
            man.CountriesToVisit.Add(countriesToVisitRepository.Add(new CountriesToVisit { ProfileId = man.Id, Country = (byte)Country.Ukraine }));
            _dbContext.SaveChanges();

            var state = new State
            {
                Existance = (byte)Existance.Active,
                LastOnline = now.AddMinutes(-1),
                Status = (byte)Status.Online,
                MembershipType = (byte)MembershipType.Normal,
                ModifiedDate = now,
                Profile = man,
                ProfileId = man.Id
            };

            stateRepository.Add(state);
            _dbContext.SaveChanges();
        }

        private void CreateSampleGirl(User user, Guid guid)
        {
            long id = user.Id;
            var now = DateTime.Now.ToUniversalTime();
            var girlRepository = new ProfileRepositoryDB(_dbContext);
            var stateRepository = new StateRepositoryDB(_dbContext);
            var countriesToVisitRepository = new CountriesToVisitRepositoryDB(_dbContext);
            var languagesSpokenRepository = new LanguagesSpokenRepositoryDB(_dbContext);
            var photoRepository = new PhotoRepositoryDB(_dbContext);

            var girl = new Profile
            {
                Gender = user.Gender,
                UserId = id,
                Description = "TestGirl" + id.ToString(CultureInfo.InvariantCulture),
                BreastSize = (byte)BreastSize.Large,
                Height = 170,
                BirthYear = 1980,
                ModifiedDate = now,
                FriendlyName = "girl_mertiko" + id.ToString(CultureInfo.InvariantCulture),
                City = "Ankara" + id.ToString(CultureInfo.InvariantCulture),
                From = (byte)Country.Turkey
            };
            girl = girlRepository.Add(girl, guid);
            _dbContext.SaveChanges();

            girl.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { ProfileId = girl.Id, Language = (byte)Language.English }));
            girl.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { ProfileId = girl.Id, Language = (byte)Language.Russian }));
            girl.LanguagesSpoken.Add(languagesSpokenRepository.Add(new LanguagesSpoken { ProfileId = girl.Id, Language = (byte)Language.Turkish }));
            var photo = photoRepository.Add(new Photo { ProfileId = girl.Id, Description = "Açıklama" + id.ToString(CultureInfo.InvariantCulture) });
            var photo2 = photoRepository.Add(new Photo { ProfileId = girl.Id, Description = "Açıklama2" + id.ToString(CultureInfo.InvariantCulture) });
            girl.Photos.Add(photo);
            girl.Photos.Add(photo2);


            girl.CountriesToVisit.Add(countriesToVisitRepository.Add(new CountriesToVisit { ProfileId = girl.Id, Country = (byte)Country.Turkey }));
            girl.CountriesToVisit.Add(countriesToVisitRepository.Add(new CountriesToVisit { ProfileId = girl.Id, Country = (byte)Country.Ukraine }));
            _dbContext.SaveChanges();

            var state = new State
            {
                Existance = (byte)Existance.Active,
                LastOnline = now.AddMinutes(-1),
                Status = (byte)Status.Online,
                MembershipType = (byte)MembershipType.Normal,
                ModifiedDate = now,
                Profile = girl,
                ProfileId = girl.Id
            };
            stateRepository.Add(state);
            _dbContext.SaveChanges();
        }


        public void CreateSampleData()
        {
            var result = ReloadResources.Set(_dbContext);
            if (result != null) foreach(var line in result) Debug.WriteLine(line);
            CreateSampleUser(Sex.Male);
            CreateSampleUser(Sex.Male);
            CreateSampleUser(Sex.Male);
            CreateSampleUser(Sex.Female);
            CreateSampleUser(Sex.Female);
            CreateSampleUser(Sex.Female);
            Debug.WriteLine("Created Sample Data");
        }
    }

}