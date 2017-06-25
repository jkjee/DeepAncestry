using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using DeependAncestry.Models;

namespace DeependAncestry
{
    public static class AncestryCacheManager
    {
        private static readonly MemoryCache AncestryCache = MemoryCache.Default;

        public static PeopleList PeopleList
        {
            get
            {
                if (!AncestryCache.Contains("PeopleList"))
                    RefreshPeopleList();
                return AncestryCache.Get("PeopleList") as PeopleList;
            }
        }

        public static PlaceList PlaceList
        {
            get
            {
                if (!AncestryCache.Contains("PlaceList"))
                    RefreshPlacesList();
                return AncestryCache.Get("PlaceList") as PlaceList;
            }
        }

        public static List<KeyValuePair<int, int>> FlatAncestors
        {
            get
            {
                if (!AncestryCache.Contains("FlatAncestors"))
                    RefreshFlatAncestors();
                return AncestryCache.Get("FlatAncestors") as List<KeyValuePair<int, int>>;
            }
        }

        public static List<KeyValuePair<int, int>> FlatDesecndants
        {
            get
            {
                if (!AncestryCache.Contains("FlatDesecndants"))
                    RefreshFlatDesecndants();
                return AncestryCache.Get("FlatDesecndants") as List<KeyValuePair<int, int>>;
            }
        }

        private static void RefreshPeopleList()
        {
            var peopleList = AncestryData.ReadAncestryPeopleData();

            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy {AbsoluteExpiration = DateTime.Now.AddDays(1)};

            AncestryCache.Add("PeopleList", peopleList, cacheItemPolicy);
        }

        private static void RefreshPlacesList()
        {
            var placesList = AncestryData.ReadAncestryPlacesData();

            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy {AbsoluteExpiration = DateTime.Now.AddDays(1)};

            AncestryCache.Add("PlaceList", placesList, cacheItemPolicy);
        }

        private static void RefreshFlatAncestors()
        {
            var flatAncestors = AncestryData.ConvertToKeyValuePair();
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy {AbsoluteExpiration = DateTime.Now.AddDays(1)};

            AncestryCache.Add("FlatAncestors", flatAncestors, cacheItemPolicy);
        }

        private static void RefreshFlatDesecndants()
        {
            var flatDesecndants = AncestryData.ConvertToKeyValuePair(true);
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy {AbsoluteExpiration = DateTime.Now.AddDays(1)};

            AncestryCache.Add("FlatDesecndants", flatDesecndants, cacheItemPolicy);
        }
    }

}