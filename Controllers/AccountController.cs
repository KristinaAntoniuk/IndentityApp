using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using IndentityApp.DTOs.Account;
using IndentityApp.Models;
using IndentityApp.Services;

namespace IndentityApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTService jWTService;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public AccountController(JWTService jWTService, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            this.jWTService = jWTService;
            this.signInManager = signInManager;
            this.userManager = userManager;
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
                Email = model.Email.ToLower(),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(newUser, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors.ToString());

            return Ok("Account has been created. You can login.");
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
        #endregion
    }
}
