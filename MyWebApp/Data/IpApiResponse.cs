using Newtonsoft.Json;

namespace MyWebApp.Data
{
    public sealed class IpApiResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;
        
        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
        
        [JsonProperty("continent")]
        public string Continent { get; set; } = string.Empty;
        
        [JsonProperty("continentCode")]
        public string ContinentCode { get; set; } = string.Empty;
        
        [JsonProperty("country")]
        public string Country { get; set; } = string.Empty;
        
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; } = string.Empty;
        
        [JsonProperty("region")]
        public string Region { get; set; } = string.Empty;
        
        [JsonProperty("regionName")]
        public string RegionName { get; set; } = string.Empty;
        
        [JsonProperty("city")]
        public string City { get; set; } = string.Empty;
        
        [JsonProperty("district")]
        public string District { get; set; } = string.Empty;
        
        [JsonProperty("zip")]
        public string Zip { get; set; } = string.Empty;

        [JsonProperty("lat")]
        public float Lat { get; set; }

        [JsonProperty("lon")]
        public float Lon { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; } = string.Empty;

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonProperty("isp")]
        public string ISP { get; set; } = string.Empty;

        [JsonProperty("org")]
        public string OrganizationName { get; set; } = string.Empty;

        [JsonProperty("as")]
        public string AS { get; set; } = string.Empty;

        [JsonProperty("asname")]
        public string ASname { get; set; } = string.Empty;

        [JsonProperty("reverse")]
        public string Reverse { get; set; } = string.Empty;

        [JsonProperty("mobile")]
        public bool Mobile { get; set; }

        [JsonProperty("proxy")]
        public bool Proxy { get; set; }

        [JsonProperty("hosting")]
        public bool Hosting { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; } = string.Empty;
    }
}
