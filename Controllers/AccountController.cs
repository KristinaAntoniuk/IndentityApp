using IndentityApp.DTOs.Account;
using IndentityApp.Models;
using IndentityApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IndentityApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTService jWTService;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly IConfiguration config;
        private readonly EmailService emailService;

        public AccountController(JWTService jWTService, 
                                 SignInManager<User> signInManager, 
                                 UserManager<User> userManager, 
                                 EmailService emailService,
                                 IConfiguration config)
        {
            this.jWTService = jWTService;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.config = config;
            this.emailService = emailService;
        }

        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDTO>> RefreshUserToken()
        {
            var user = await userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
            return CreateApplicationUserDTO(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null) { return Unauthorized("Invalid username."); }

            if (user.EmailConfirmed == false) { return Unauthorized("Please confirm your email."); }
            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) { return Unauthorized("Invalid password."); }

            return CreateApplicationUserDTO(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (await CheckEmailExistsAsync(model.Email)) { return BadRequest($"User with the email {model.Email} already exists."); }

            var newUser = new User
            {
                FirstName = model.FirstName.ToLower(),
                LastName = model.LastName.ToLower(),
                UserName = model.Email.ToLower(),
                Email = model.Email.ToLower()
            };

            var result = await userManager.CreateAsync(newUser, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors.ToString());

            try
            {
                bool emailSent = await SendConfirmEmailAsync(newUser);
                if (emailSent)
                {
                    return Ok(new JsonResult(new { title = "Account Created", message = "Account has been created. Please, confirm your email address." }));
                }
                return BadRequest("Failed to send email. Please contact admin.");
            }
            catch(Exception ex)
            {
                return BadRequest("Failed to send email. Please contact admin.");
            }
        }

        [HttpPut("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("This email address has not been registered yet");

            if (user.EmailConfirmed == true) return BadRequest("Your email was confirmed before. You can login to your account.");

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);


                var result = await userManager.ConfirmEmailAsync(user, decodedToken);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Email confirmed", message = "Your email address is confirmed. You can login now." }));
                }
                else
                {
                    return BadRequest("Invalid token. Please try again");
                }
            }
            catch(Exception ex)
            {
                return BadRequest("Invalid token. Please try again");
            }
        }

        [HttpPost("resend-email-confirmation-link/{email}")]
        public async Task<IActionResult> ResendEmailConfirmationLink(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Invalif email.");
            var user = await userManager.FindByEmailAsync(email);

            if (user == null) return Unauthorized("This email address has not been registered yet.");
            if (user.EmailConfirmed == true) return BadRequest("Your email address is already confirmed. Please login to your account.");

            try
            {
                bool emailSent = await SendConfirmEmailAsync(user);
                if (emailSent)
                {
                    return Ok(new JsonResult(new { title = "Confirmation link resent", message = "Please, confirm your email address." }));
                }
                return BadRequest("Failed to send email. Please contact admin.");
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to send email. Please contact admin.");
            }

        }

        [HttpPost("forgot-username-or-password/{email}")]
        public async Task<IActionResult> ForgotUsernameOrPassword(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email");

            var user = await userManager.FindByEmailAsync(email);

            if (user == null) return Unauthorized("This email address has not been registered yet.");
            if (user.EmailConfirmed != true) return BadRequest("Your email address has not been confirmed. Please, confirm your email address first.");

            try
            {
                if (await SendForgotUsernameOrPasswordEmail(user))
                {
                    return Ok(new JsonResult(new { title = "Forgot username or password email is sent", message = "Please check your email." }));
                }
                else
                {
                    return BadRequest("Failed to send an email. Please contact your administrator.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to send an email. Please contact your administrator.");
            }
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("This email address has not been registered yet.");
            if (user.EmailConfirmed != true) return BadRequest("Your email address has not been confirmed. Please, confirm your email address first.");

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);


                var result = await userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Password reset success", message = "Your password has been reset." }));
                }
                else
                {
                    return BadRequest("Invalid token. Please try again");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid token. Please try again");
            }
        }

        #region Private Methods
        private UserDTO CreateApplicationUserDTO(User user)
        {
            return new UserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = jWTService.CreateJWT(user)
            };
        }

        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }

        private async Task<bool> SendConfirmEmailAsync(User user)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{config["JWT:ClientUrl"]}/{config["Email:ConfirmEmailPath"]}?token={token}&email={user.Email}";

            var body = $"<p>Hello: {user.FirstName} {user.LastName}. </p>" +
                 "<p>Please confirm your email address by clicking on the following link:</p>" +
                 $"<p><a href=\"{url}\">Click here</a></p>" +
                 "<p>Thank you,</p>" +
                 $"<br>{config["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(user.Email, "Confirm your email", body);
            return await emailService.SendEmailAsync(emailSend);
        }

        private async Task<bool> SendForgotUsernameOrPasswordEmail(User user)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{config["JWT:ClientUrl"]}/{config["Email:ResetPasswordPath"]}?token={token}&email={user.Email}";

            var body = $"<p>Hello: {user.FirstName} {user.LastName}. </p>" +
                $"<p>Username: {user.UserName}.</p>" +
                "<p>In order to reset your password, please click on the following link.</p>" +
                $"<p><a href=\"{url}\">Click here</a></p>" +
                "<p>Thank you,</p>" +
                $"<br>{config["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(user.Email, "Forgot username or password", body);
            return await emailService.SendEmailAsync(emailSend);
        }
        #endregion
    }
}
