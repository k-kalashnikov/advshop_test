using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Blazor.Models
{
    public class ChatUser
    {
        [Key]
        public string Name { get; set; }

        [Required]
        public string ConnectionId { get; set; }
    }
}
