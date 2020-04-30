using System.ComponentModel.DataAnnotations;

namespace LiveChat.Models
{
    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}