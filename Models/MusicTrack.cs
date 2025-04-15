using System.ComponentModel.DataAnnotations;

namespace MusicApp2.Models
{
    public class MusicTrack
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string ArtistName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; }

        public bool IsCopyrighted { get; set; }

        public string CopyrightDetails { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
    }
}