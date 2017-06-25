using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeependAncestry.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;

namespace DeependAncestry
{
    public static class AncestryData 
    {
        private static readonly string FilePath = $"{AppDomain.CurrentDomain.BaseDirectory}/App_Data/data_large.json";

        private static readonly int _maximumLevel = 10;
        public static PeopleList ReadAncestryPeopleData()
        {
            PeopleList peopleList = new PeopleList {people = new List<People>()};

            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {

                // Advance the reader to the start of the first array (which should be value of the "Stores" property)
                while (reader.TokenType != JsonToken.StartArray)
                    reader.Read();

                // Now process each store individually
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        JObject obj = JObject.Load(reader);

                        if (obj["gender"] != null)
                        {
                            var people = obj.ToObject<People>();
                            peopleList.people.Add(people);
                        }
                    }
                }
            }
            return peopleList;
        }

        public static PlaceList ReadAncestryPlacesData()
        {
            PlaceList placeList = new PlaceList {places = new List<Place>()};

            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {

                // Advance the reader to the start of the first array (which should be value of the "Stores" property)
                while (reader.TokenType != JsonToken.StartArray)
                    reader.Read();

                // Now process each store individually
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        JObject obj = JObject.Load(reader);

                        if (obj["gender"] == null)
                        {
                            var place = obj.ToObject<Place>();
                            placeList.places.Add(place);
                        }
                    }
                }
            }
            return placeList;
        }

        public static IPagedList<People> SearchPeople(string name, bool? isMale, bool? isFemale, int? page)
        {

            var peopleList = AncestryCacheManager.PeopleList;
            var people =
                peopleList.people.Where(p => p.name?.IndexOf(name, StringComparison.OrdinalIgnoreCase) > -1).ToList();

            var male = isMale ?? false;
            var female = isFemale ?? false;

            if (male && !female)
            {
                people = people.Where(p => p.gender == "M").ToList();
            }

            if (female && !male)
            {
                people = people.Where(p => p.gender == "F").ToList();
            }

            if (people.Any() && people.Count > 0)
            {
                foreach (var p in people)
                {
                    var place = GetPlace(p.place_id);
                    p.BirthPlace = place?.Name;
                }
                return people.ToPagedList(page ?? 1, 10);
            }

            return null;
        }

        public static Place GetPlace(int id)
        {

            var placeList = AncestryCacheManager.PlaceList;
            return placeList.places.FirstOrDefault(p => p.Id == id);
        }

        public static IPagedList<People> SearchPeopleByDirection(string name, string direction, bool? isMale, bool? isFemale, int? page)
        {
            People peopleByName;
            List<People> peopleByDirection = new List<People>();

            var peopleList = AncestryCacheManager.PeopleList;
            //Get the people by the name entered in the search
            peopleByName = peopleList.people.FirstOrDefault(p => p.name.ToLower() == name.ToLower());

            if (peopleByName != null)
            {
                //get the ancestors for the above found people
                if (direction == "ancestors")
                {
                    peopleByDirection = GetRelations(peopleByName).ToList();
                }
                else
                {
                    //get the descendents for the above found people
                    peopleByDirection = GetRelations(peopleByName, true).ToList();
                }
            }
            
            var male = isMale ?? false;
            var female = isFemale ?? false;

            if (peopleByDirection.Any() && peopleByDirection.Count > 0)
            {
                if (male && !female)
                {
                    peopleByDirection = peopleByDirection.Where(p => p.gender == "M").ToList();
                }

                if (female && !male)
                {
                    peopleByDirection = peopleByDirection.Where(p => p.gender == "F").ToList();
                }

                if (peopleByDirection.Any() && peopleByDirection.Count > 0)
                {
                    foreach (var p in peopleByDirection)
                    {
                        var place = GetPlace(p.place_id);
                        p.BirthPlace = place?.Name;
                    }
                    return peopleByDirection.ToPagedList(page ?? 1, 10);
                }
            }

            return null;
        }
        
        private static IEnumerable<People> GetRelations(People people, bool isDescendants = false)
        {
            var groups = isDescendants ? AncestryCacheManager.FlatDesecndants : AncestryCacheManager.FlatAncestors;
            var relations = GetRelationData(people.id, groups);
            var peopleList = AncestryCacheManager.PeopleList.people;
            return peopleList.Where(x => relations.Contains(x.id));
        }

        public static List<KeyValuePair<int, int>> ConvertToKeyValuePair(bool isDescendants = false)
        {
            List<KeyValuePair<int, int>> groups = new List<KeyValuePair<int, int>>();
            var people = AncestryCacheManager.PeopleList.people;
            foreach (var p in people)
            {
                if (p.father_id.HasValue && p.father_id > -1 && !isDescendants)
                {
                    groups.Add(new KeyValuePair<int, int>(p.id, p.father_id.Value));
                }

                if (p.father_id.HasValue && p.father_id > -1 && isDescendants)
                {
                    groups.Add(new KeyValuePair<int, int>(p.father_id.Value, p.id));
                }

                if (p.mother_id.HasValue && p.mother_id > -1 && !isDescendants)
                {
                    groups.Add(new KeyValuePair<int, int>(p.id, p.mother_id.Value));
                }

                
                if (p.mother_id.HasValue && p.mother_id > -1 && isDescendants)
                {
                    groups.Add(new KeyValuePair<int, int>(p.mother_id.Value, p.id));
                }
            }
            return groups;
        }

        private static List<int> GetRelationData(int id, List<KeyValuePair<int, int>> newLst)
        {
            return GetRelationData(id, 0, newLst);
        }

        private static List<int> GetRelationData(int id, int currentLevel, List<KeyValuePair<int, int>> newLst)
        {
            var list = new List<int>();
            if (currentLevel < _maximumLevel)
            {
                for (int i = 0; i < newLst.Count; i++)
                {
                    if (Convert.ToInt32(newLst[i].Key) == id)
                    {
                        if (!list.Contains(Convert.ToInt32(newLst[i].Value)))
                        {
                            list.Add(Convert.ToInt32(newLst[i].Value));
                            var l = GetRelationData(newLst[i].Value, currentLevel + 1, newLst);
                            list.AddRange(l);
                        }
                    }
                }
            }
            return list;
        }
        
    }
}