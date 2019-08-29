using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudniaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repository, 
            IMapper mapper, 
            IOptions<CloudinarySettings> cloudniaryConfig)
        {
            _repository = repository;
            _mapper = mapper;
            _cloudniaryConfig = cloudniaryConfig;

            Account acc = new Account(
                _cloudniaryConfig.Value.CloudName,
                _cloudniaryConfig.Value.ApiKey,
                _cloudniaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }
        
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repository.GetPhoto(id);
            
            var photo = _mapper.Map<PhotoToReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, 
            [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var userFromRepo = await _repository.GetUser(userId, true);

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if(file.Length > 0){
                using(var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams 
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                            .Width(500)
                            .Height(500)
                            .Crop("fill")
                            .Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId.ToString();

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if(!userFromRepo.Photos.Any(p => p.IsMain))
                photo.IsMain = true;
            
            userFromRepo.Photos.Add(photo);

            if(await _repository.SaveAll()){
                var photoToReturn = _mapper.Map<PhotoToReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }
            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var userFromRepo = await _repository.GetUser(userId, true);

            if(!userFromRepo.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repository.GetPhoto(id);

            if(photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");
            
            var currentMainPhoto = await _repository.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if(await _repository.SaveAll())
                return NoContent();

            return BadRequest("Could not set the main photo");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var userFromRepo = await _repository.GetUser(userId, true);

            if(!userFromRepo.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repository.GetPhoto(id);

            if(photoFromRepo.IsMain)
                return BadRequest("You cannot delete your main photo");

            if(photoFromRepo.PublicId != null){
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                if(result.Result == "ok"){
                    _repository.Delete(photoFromRepo);
                }
            }else{
                _repository.Delete(photoFromRepo);                
            }

            

            if(await _repository.SaveAll())
                return Ok();
            
            return BadRequest("Failed to delete the photo");
        }
    }
}