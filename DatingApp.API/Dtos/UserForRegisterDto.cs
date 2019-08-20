using System;
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

        [Required]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }

        public UserForRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}