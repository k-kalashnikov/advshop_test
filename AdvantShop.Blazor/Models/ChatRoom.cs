using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Blazor.Models
{
    public class ChatRoom
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }


        public IEnumerable<ChatMessage> Messages { get; set; }
    }
}
