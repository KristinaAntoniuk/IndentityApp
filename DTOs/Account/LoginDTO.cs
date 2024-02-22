using System.ComponentModel.DataAnnotations;

namespace IndentityApp.DTOs.Account
{
    public class LoginDTO
    {
        [Required (ErrorMessage = "Username is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
