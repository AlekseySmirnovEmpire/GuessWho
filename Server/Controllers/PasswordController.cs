using Core.Database.Email;
using Core.Models.Users;
using Core.Server.Database.Email;
using Core.Server.Services;
using Core.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[Route("/api/v1/[controller]")]
public class PasswordController(
    ILogger<PasswordController> logger,
    IEmailService emailService,
    UserService userService,
    IEmailTemplatingService emailTemplatingService)
    : BaseApiController
{
    [Route("email/{userId:long}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult SendEmailForResetPassword(long userId)
    {
        try
        {
            var user = userService.FindById(userId);
            emailService.PutNewMessageInQueue(new EmailSendingQueue(
                "Сброс пароля",
                emailTemplatingService.GenerateEmailTemplate(SubstitutionEmailTemplates.ResetPassword, user),
                user!.Email,
                EmailPriority.High));

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);

            return BadRequest(new { status = false });
        }
    }

    [Route("reset")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ResetPassword([FromBody] ResetPasswordModel model)
    {
        try
        {
            return Ok(new { status = userService.ResetUserPassword(model) });
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);

            return BadRequest(new { status = false });
        }
    }
}