using Core.Models.Images;
using Core.Server.Providers;
using Core.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Server.Filters;

namespace Server.Controllers;

[Route("/api/v1/[controller]")]
public class ImagesController(FileDataService service, ILogger<ImagesController> logger, IAuthProvider provider) 
    : BaseApiController
{
    [HttpPut]
    [JwtAuth]
    public IActionResult Upload([FromBody] ImageDataModel image)
    {
        try
        {
            var user = provider.GetCurrentUser(HttpContext);
            return Ok(new
            {
                avatarId = service.UploadImage(image, user.Id)
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{imageId:guid}")]
    public IActionResult Get(Guid imageId)
    {
        try
        {
            var file = service.FindFile(imageId);
            if (file == null) return NotFound();

            return File(file.Data, file.ContentType);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            return NotFound();
        }
    }
}