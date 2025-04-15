using MusicApp2.Models;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MusicApp2.Services
{
    public class MusicService
    {
        private readonly DataService _dataService;
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadsPath;
        private readonly AcoustIdService _acoustIdService;

        public MusicService(DataService dataService, IWebHostEnvironment environment, AcoustIdService acoustIdService)
        {
            _dataService = dataService;
            _environment = environment;
            _acoustIdService = acoustIdService;
            _uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(_uploadsPath);
        }

        public async Task<MusicTrack> UploadTrack(IFormFile file, string title, string artistName, string userId)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var track = new MusicTrack
            {
                Title = title,
                ArtistName = artistName,
                FilePath = $"/uploads/{fileName}",
                UploadDate = DateTime.Now,
                UserId = userId
            };

            // Use AcoustID for copyright detection
            var (isCopyrighted, details) = await _acoustIdService.AnalyzeTrack(filePath);
            track.IsCopyrighted = isCopyrighted;
            track.CopyrightDetails = details;

            await _dataService.Add(track);
            return track;
        }

        public async Task<List<MusicTrack>> GetUserTracks(string userId)
        {
            var tracks = await _dataService.GetAll<MusicTrack>();
            return tracks.Where(t => t.UserId == userId).ToList();
        }

        public async Task<MusicTrack?> GetTrack(int id)
        {
            var tracks = await _dataService.GetAll<MusicTrack>();
            return tracks.FirstOrDefault(t => t.Id == id);
        }
    }
}