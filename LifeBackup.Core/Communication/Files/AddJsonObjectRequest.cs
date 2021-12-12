using Newtonsoft.Json;

namespace LifeBackup.Core.Communication.Files
{
    public class AddJsonObjectRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("datesent")]
        public DateTime DateSent { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }
}