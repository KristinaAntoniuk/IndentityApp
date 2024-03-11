using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using IndentityApp.DTOs.Account;
using IndentityApp.Models;
using IndentityApp.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

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
                    return Ok(new JsonResult(new { title = "Account Created", message = "Account has been created. Please, confirm youe email address." }));
                }
                return BadRequest("Failed to send email. Please contact admin.");
            }
            catch(Exception ex)
            {
                return BadRequest("Failed to send email. Please contact admin.");
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
        #endregion
    }
}
