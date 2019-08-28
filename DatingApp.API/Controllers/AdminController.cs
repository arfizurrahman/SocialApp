using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [Authorize(Policy="RequiredAdminRole")]
        [HttpGet("userWithRoles")]
        public IActionResult GetUserWithRoles() 
        {
            return Ok("Only admin users can see this");
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public IActionResult GetPhotosForModeration() 
        {
            return Ok("Only admins and moderators can see this");
        }
    }
}