using MusicApp2.Models;

namespace MusicApp2.Services
{
    public class CollaborationService
    {
        private readonly DataService _dataService;

        public CollaborationService(DataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<CollaborationRequest> CreateRequest(int trackId, string requestingArtistId, string trackOwnerId, string message)
        {
            var request = new CollaborationRequest
            {
                TrackId = trackId,
                RequestingArtistId = requestingArtistId,
                TrackOwnerId = trackOwnerId,
                RequestMessage = message,
                RequestDate = DateTime.Now,
                IsApproved = false
            };

            await _dataService.Add(request);
            return request;
        }

        public async Task<List<CollaborationRequest>> GetRequestsForUser(string userId)
        {
            var requests = await _dataService.GetAll<CollaborationRequest>();
            return requests.Where(r => r.TrackOwnerId == userId).ToList();
        }

        public async Task<List<CollaborationRequest>> GetSentRequests(string userId)
        {
            var requests = await _dataService.GetAll<CollaborationRequest>();
            return requests.Where(r => r.RequestingArtistId == userId).ToList();
        }

        public async Task UpdateRequestStatus(int requestId, bool isApproved, string responseMessage)
        {
            var requests = await _dataService.GetAll<CollaborationRequest>();
            var request = requests.FirstOrDefault(r => r.Id == requestId);

            if (request != null)
            {
                request.IsApproved = isApproved;
                request.ResponseMessage = responseMessage;
                await _dataService.Update(request, r => r.Id == requestId);
            }
        }
    }
}