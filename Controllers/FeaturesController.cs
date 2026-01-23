using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RENTORA.API.Models;
using RENTORA.API.Models.DTOs;
using RENTORA.API.Repository.IRepository;
using RENTORA.API.WebSettings;

namespace RENTORA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeaturesController : ControllerBase
    {
        private readonly IFeaturesRepository _featuresRepo;
        private readonly IFileUploadService _fileUploadService;

        public FeaturesController(IFeaturesRepository featuresRepo, IFileUploadService fileUploadService)
        {
            _featuresRepo = featuresRepo;
            _fileUploadService = fileUploadService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var features = await _featuresRepo.GetAllAsync();
            return Ok(features);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var feature = await _featuresRepo.GetByIdAsync(id);
            if (feature == null) return NotFound();
            return Ok(feature);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] FeaturesDto featuresDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var feature = new Features
            {
                Name = featuresDto.Name,
                Description = featuresDto.Description,
                Category = featuresDto.Category
            };

            if (featuresDto.ImageFile != null)
            {
                if (!_fileUploadService.IsValidFileType(featuresDto.ImageFile, new[] { ".jpg", ".jpeg", ".png", ".webp" }))
                {
                    return BadRequest("Invalid file type. Only JPG, JPEG, PNG, WEBP are allowed.");
                }

                string imageUrl = await _fileUploadService.UploadFileAsync(featuresDto.ImageFile, "features");
                feature.ImageUrl = imageUrl;
            }

            var createdFeature = await _featuresRepo.CreateAsync(feature);
            return CreatedAtAction(nameof(GetById), new { id = createdFeature.Id }, createdFeature);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] FeaturesDto featuresDto)
        {
            var existingFeature = await _featuresRepo.GetByIdAsync(id);
            if (existingFeature == null) return NotFound();

            existingFeature.Name = featuresDto.Name;
            existingFeature.Description = featuresDto.Description;
            existingFeature.Category = featuresDto.Category;

            if (featuresDto.ImageFile != null)
            {
                 if (!_fileUploadService.IsValidFileType(featuresDto.ImageFile, new[] { ".jpg", ".jpeg", ".png", ".webp" }))
                {
                    return BadRequest("Invalid file type. Only JPG, JPEG, PNG, WEBP are allowed.");
                }

                // Delete old image if exists
                if (!string.IsNullOrEmpty(existingFeature.ImageUrl))
                {
                    // Optionally delete old file
                    // await _fileUploadService.DeleteFileAsync(existingFeature.ImageUrl); 
                }

                string imageUrl = await _fileUploadService.UploadFileAsync(featuresDto.ImageFile, "features");
                existingFeature.ImageUrl = imageUrl;
            }

            var updatedFeature = await _featuresRepo.UpdateAsync(id, existingFeature);
            return Ok(updatedFeature);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingFeature = await _featuresRepo.GetByIdAsync(id);
            if (existingFeature == null) return NotFound();

            if (!string.IsNullOrEmpty(existingFeature.ImageUrl))
            {
                 await _fileUploadService.DeleteFileAsync(existingFeature.ImageUrl);
            }

            await _featuresRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
