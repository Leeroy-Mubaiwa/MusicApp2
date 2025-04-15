using System.ComponentModel.DataAnnotations;

namespace MusicApp2.Models
{
    public class CollaborationRequest
    {
        public int Id { get; set; }

        [Required]
        public int TrackId { get; set; }

        [Required]
        public string RequestingArtistId { get; set; } = string.Empty;

        [Required]
        public string TrackOwnerId { get; set; } = string.Empty;

        [Required]
        public string RequestMessage { get; set; } = string.Empty;

        public DateTime RequestDate { get; set; }

        public bool IsApproved { get; set; }

        public string ResponseMessage { get; set; } = string.Empty;
    }
}