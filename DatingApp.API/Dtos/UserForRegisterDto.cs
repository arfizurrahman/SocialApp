using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required(ErrorMessage = "Please enter username")]
        public string Username { get; set; }

        [Required (ErrorMessage = "Please enter password")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Password must be between 4 to 10 characters")]
        public string Password { get; set; }
    }
}