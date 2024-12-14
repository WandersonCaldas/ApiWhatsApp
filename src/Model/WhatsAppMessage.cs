using System.ComponentModel.DataAnnotations;

namespace src.Model
{
    public class WhatsAppMessage
    {
        [Required]
        public string To { get; set; } = "";

        [Required]
        public string Body { get; set; } = "";
    }
}
