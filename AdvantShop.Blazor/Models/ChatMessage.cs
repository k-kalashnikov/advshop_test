using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Blazor.Models
{
    public class ChatMessage
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime CreateAt { get; set; }

        [Required]
        public string SendBy { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public long ChatRoomId { get; set; }
    }
}
