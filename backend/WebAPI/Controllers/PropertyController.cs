using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos;
using WebAPI.Interfaces;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class PropertyController : BaseController
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public PropertyController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.photoService = photoService;
        }

        // property/list/2
        [HttpGet("list/{sellRent}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyList(int sellRent)
        {
            var properties = await uow.PropertyRepository.GetPropertiesAsync(sellRent);
            var propertyListDto = mapper.Map<IEnumerable<PropertyListDto>>(properties); // create a map in automapper profile class
            return Ok(propertyListDto);
        }

        // property/detail/2
        [HttpGet("detail/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyDetail(int id)
        {
            var property = await uow.PropertyRepository.GetPropertyDetailAsync(id);
            var propertyDTO = mapper.Map<PropertyDetailDto>(property);
            return Ok(propertyDTO);
        }
        
        // property/add
        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddProperty(PropertyDto propertyDto)
        {
            var property = mapper.Map<Property>(propertyDto);
            var userId = GetUserId();
            property.PostedBy = userId;
            property.LastUpdatedBy = userId;
            uow.PropertyRepository.AddProperty(property);
            await uow.SaveAsync();
            return StatusCode(201);
        }

        // property/photo/1
        [HttpPost("add/photo/{propId}")]
        [Authorize]
        public async Task<IActionResult> AddPropertyPhoto(IFormFile file, int propId)
        {
            var result = await photoService.UploadPhotoAsync(file);
            if (result.Error != null)
                return BadRequest(result.Error.Message);
            var property = await uow.PropertyRepository.GetPropertyByIdAsync(propId);
            var photo = new Photo
            {
                ImageUrl = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (property.Photos.Count == 0)
                photo.IsPrimary = true;
            property.Photos.Add(photo);
            await uow.SaveAsync();
            return StatusCode(201);
        }

        // property/set-primary-photo/1/fffkffe
        [HttpPost("set-primary-photo/{propId}/{photoPublicId}")]
        [Authorize]
        public async Task<IActionResult> SetPrimaryPhoto(int propId, string photoPublicId)
        {
            var userId = GetUserId();
            var property = await uow.PropertyRepository.GetPropertyByIdAsync(propId);
            if (property == null)
                return BadRequest("No such property or photo exists");

            if (property.PostedBy != userId)
                return BadRequest("You are not authorized to change this photo");
            var photo = property.Photos.FirstOrDefault(p => p.PublicId == photoPublicId);
            if (photo == null)
                return BadRequest("No such photo exists");
            if (photo.IsPrimary)
                return BadRequest("This is already a primary photo");
            var currentPrimary = property.Photos.FirstOrDefault(p => p.IsPrimary);
            if(currentPrimary != null)
                currentPrimary.IsPrimary = false;
            photo.IsPrimary = true;
            if (await uow.SaveAsync())
                return NoContent();
            return BadRequest("Some error has occured setting the primary photo");
        }

        // property/set-primary-photo/1/fffkffe
        [HttpDelete("delete-photo/{propId}/{photoPublicId}")]
        [Authorize]
        public async Task<IActionResult> DeletePhoto(int propId, string photoPublicId)
        {
            var userId = GetUserId();
            var property = await uow.PropertyRepository.GetPropertyByIdAsync(propId);
            if (property == null)
                return BadRequest("No such property or photo exists");

            if (property.PostedBy != userId)
                return BadRequest("You are not authorized to delete the photo");
            var photo = property.Photos.FirstOrDefault(p => p.PublicId == photoPublicId);
            if (photo == null)
                return BadRequest("No such photo exists");

            if (photo.IsPrimary)
                return BadRequest("You cannot delete the primary photo");
            // delete from cloud 
            var result = await photoService.DeletePhotoAsync(photoPublicId);
            if (result.Error != null)
                return BadRequest(result.Error.Message);

            property.Photos.Remove(photo);

            if (await uow.SaveAsync())
                return Ok();
            return BadRequest("Some error has occured deleting the photo");
        }

    }
}
