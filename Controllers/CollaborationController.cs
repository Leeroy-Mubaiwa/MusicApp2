using Microsoft.AspNetCore.Mvc;
using MusicApp2.Models;
using MusicApp2.Services;

namespace MusicApp2.Controllers
{
    public class CollaborationController : Controller
    {
        private readonly CollaborationService _collaborationService;
        private readonly MusicService _musicService;

        public CollaborationController(CollaborationService collaborationService, MusicService musicService)
        {
            _collaborationService = collaborationService;
            _musicService = musicService;
        }

        public async Task<IActionResult> MyRequests()
        {
            // In a real app, you'd get this from the authentication system
            var userId = "user1";
            var requests = await _collaborationService.GetRequestsForUser(userId);
            return View(requests);
        }

        public async Task<IActionResult> SentRequests()
        {
            // In a real app, you'd get this from the authentication system
            var userId = "user2";
            var requests = await _collaborationService.GetSentRequests(userId);
            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> RespondToRequest(int requestId, bool isApproved, string responseMessage)
        {
            await _collaborationService.UpdateRequestStatus(requestId, isApproved, responseMessage);
            return RedirectToAction("MyRequests");
        }
    }
}