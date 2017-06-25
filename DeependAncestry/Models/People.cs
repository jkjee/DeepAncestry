using Newtonsoft.Json;

namespace DeependAncestry.Models
{
    public class People
    {
        public int id { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public int place_id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? father_id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? mother_id { get; set; }
        public int level { get; set; }

        [JsonIgnore]
        public string BirthPlace { get; set; }
    }

}