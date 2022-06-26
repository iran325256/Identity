using System.ComponentModel.DataAnnotations;

namespace JwtTest.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name Is Required")]
        public string UserName { get; set; }

        
        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; set; }
      
        }
}
