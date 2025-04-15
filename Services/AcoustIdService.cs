using System.Net.Http.Json;
using MusicApp2.Models;

namespace MusicApp2.Services
{
    public class AcoustIdService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<AcoustIdService> _logger;
        private readonly ChromaprintService _chromaprintService;

        public AcoustIdService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AcoustIdService> logger,
            ChromaprintService chromaprintService)
        {
            _httpClient = httpClient;
            _apiKey = configuration["AcoustId:ApiKey"] ?? throw new ArgumentNullException("AcoustId:ApiKey");
            _logger = logger;
            _chromaprintService = chromaprintService;
        }

        public async Task<(bool IsCopyrighted, string Details)> AnalyzeTrack(string filePath)
        {
            try
            {
                // Create fingerprint using Chromaprint
                var fingerprint = await _chromaprintService.CreateFingerprint(filePath);
                if (string.IsNullOrEmpty(fingerprint))
                {
                    return (false, "Could not create audio fingerprint");
                }

                // Lookup matches
                var matches = await LookupFingerprint(fingerprint);
                if (matches?.Results?.Any() == true)
                {
                    var bestMatch = matches.Results.OrderByDescending(r => r.Score).First();
                    if (bestMatch.Score > 0.7) // 70% confidence threshold
                    {
                        var recording = bestMatch.Recordings?.FirstOrDefault();
                        if (recording != null)
                        {
                            return (true, $"Similar to: '{recording.Title}' by {recording.Artists?.FirstOrDefault()?.Name} (Score: {bestMatch.Score:F2})");
                        }
                    }
                }

                return (false, "No significant matches found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing track with AcoustID");
                return (false, "Error analyzing track");
            }
        }

        private async Task<AcoustIdResponse?> LookupFingerprint(string fingerprint)
        {
            var url = $"lookup?client={_apiKey}&meta=recordings+releasegroups+compress&fingerprint={fingerprint}";
            var response = await _httpClient.GetFromJsonAsync<AcoustIdResponse>(url);
            return response;
        }
    }

    public class AcoustIdResponse
    {
        public List<AcoustIdResult>? Results { get; set; }
    }

    public class AcoustIdResult
    {
        public double Score { get; set; }
        public List<AcoustIdRecording>? Recordings { get; set; }
    }

    public class AcoustIdRecording
    {
        public string? Title { get; set; }
        public List<AcoustIdArtist>? Artists { get; set; }
    }

    public class AcoustIdArtist
    {
        public string? Name { get; set; }
    }
}