using System.ComponentModel.DataAnnotations;

namespace JwtTest.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="User Name Is Required)"]
        public string UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email Is Required)"]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Is Required)"]

        public string Password { get; set; }
    }
}
