using Microsoft.AspNetCore.Mvc;
using MusicApp2.Models;
using MusicApp2.Services;

namespace MusicApp2.Controllers
{
    public class MusicController : Controller
    {
        private readonly MusicService _musicService;
        private readonly CollaborationService _collaborationService;

        public MusicController(MusicService musicService, CollaborationService collaborationService)
        {
            _musicService = musicService;
            _collaborationService = collaborationService;
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string title, string artistName)
        {
            // In a real app, you'd get this from the authentication system
            var userId = "user1"; 

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please select a file");
                return View();
            }

            var track = await _musicService.UploadTrack(file, title, artistName, userId);
            return RedirectToAction("Details", new { id = track.Id });
        }

        public async Task<IActionResult> MyTracks()
        {
            // In a real app, you'd get this from the authentication system
            var userId = "user1";
            var tracks = await _musicService.GetUserTracks(userId);
            return View(tracks);
        }

        public async Task<IActionResult> Details(int id)
        {
            var track = await _musicService.GetTrack(id);
            if (track == null)
            {
                return NotFound();
            }
            return View(track);
        }

        [HttpPost]
        public async Task<IActionResult> RequestCollaboration(int trackId, string message)
        {
            // In a real app, you'd get these from the authentication system
            var requestingArtistId = "user2";
            var track = await _musicService.GetTrack(trackId);
            
            if (track == null)
            {
                return NotFound();
            }

            await _collaborationService.CreateRequest(trackId, requestingArtistId, track.UserId, message);
            return RedirectToAction("Details", new { id = trackId });
        }
    }
} 