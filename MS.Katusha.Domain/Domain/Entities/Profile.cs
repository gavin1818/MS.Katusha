using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;
using Newtonsoft.Json;

namespace MS.Katusha.Domain.Entities
{
    public class Profile : BaseFriendlyModel
    {
        public Profile()
        {
            Searches = new List<SearchingFor>();
            Photos = new List<Photo>();
            CountriesToVisit = new List<CountriesToVisit>();
            LanguagesSpoken = new List<LanguagesSpoken>();
            SentMessages = new List<Conversation>();
            RecievedMessages = new List<Conversation>();
            WhoVisited = new List<Visit>();
            Visited = new List<Visit>();
            Location = new Location();
        }

        public long UserId { get; set; }
        
        public User User { get; set; }

        [StringLength(64)]
        public string Name { get; set; }

        public Location Location { get; set; }

        public byte BodyBuild { get; set; }
        public byte EyeColor { get; set; }
        public byte HairColor { get; set; }
        public byte Smokes { get; set; }
        public byte Alcohol { get; set; }
        public byte Religion { get; set; }
        public byte Gender { get; set; }
        public byte DickSize { get; set; }
        public byte DickThickness { get; set; }
        public byte BreastSize { get; set; }

        [Range(100, 250)]
        public int Height { get; set; }

        [Range(1920, 2000)]
        public int BirthYear { get; set; }

        [StringLength(4000, MinimumLength = 3)]
        public string Description { get; set; }

        public Guid ProfilePhotoGuid { get; set; }

        public List<SearchingFor> Searches { get; set; }
        public List<CountriesToVisit> CountriesToVisit { get; set; }
        public List<LanguagesSpoken> LanguagesSpoken { get; set; }

        public List<Photo> Photos { get; set; }

        [JsonIgnore]
        public List<Conversation> SentMessages { get; set; }

        [JsonIgnore]
        public List<Conversation> RecievedMessages { get; set; }

        [JsonIgnore]
        public List<Visit> WhoVisited { get; set; }
        
        [JsonIgnore]
        public List<Visit> Visited { get; set; }

        public override string ToString()
        {
            var str = base.ToString() +
                      String.Format(
                          " | UserId: {0} | State: {1} | Name: {2} | From: {3} | City: {4} | BodyBuild: {5} | EyeColor: {6} | HairColor: {7} | Smokes: {8} | Alcohol: {9} | DickSize: {13} | DickThickness: {14} | BreastSize: {15} | Religion: {10} | Height: {11} | BirthYear: {12}",
                          (User == null) ? 0 : User.Id, "REMOVED", Name,
                          Location, "",
                          Enum.GetName(typeof (BodyBuild), BodyBuild),
                          Enum.GetName(typeof (EyeColor), EyeColor),
                          Enum.GetName(typeof (HairColor), HairColor),
                          Enum.GetName(typeof (Smokes), Smokes),
                          Enum.GetName(typeof (Alcohol), Alcohol),
                          Enum.GetName(typeof(Religion), Religion),
                          Enum.GetName(typeof(DickSize), DickSize),
                          Enum.GetName(typeof(DickThickness), DickThickness),
                          Enum.GetName(typeof(BreastSize), BreastSize),
                          Height,
                          BirthYear
                          );

            if (Searches != null && Searches.Count > 0)
            {
                str += "\r\nSearches: [\r\n";
                str = Searches.Aggregate(str, (current, search) => current + ("\t" + search.ToString() + "\r\n"));
                str += "]";
            }

            if (Photos != null && Photos.Count > 0)
            {
                str += "\r\nPhotos: [\r\n";
                str = Photos.Aggregate(str, (current, photo) => current + ("\t" + photo + "\r\n"));
                str += "]";
            }

            if (CountriesToVisit != null && CountriesToVisit.Count > 0)
            {
                str += "\r\nCountriesToVisit: [\r\n";
                str = CountriesToVisit.Aggregate(str, (current, countriesToVisit) => current + ("\t" + countriesToVisit + "\r\n"));
                str += "]";
            }

            if (LanguagesSpoken != null && LanguagesSpoken.Count > 0)
            {
                str += "\r\nLanguagesSpoken: [\r\n";
                str = LanguagesSpoken.Aggregate(str, (current, languagesSpoken) => current + ("\t" + languagesSpoken + "\r\n"));
                str += "]";
            }
            return str;
        }
    }
}