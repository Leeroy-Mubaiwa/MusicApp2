namespace MusicApp2.Models
{
    public class AcoustIdSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.acoustid.org/v2/";
    }
}