using IndentityApp.Models;
using IndentityApp.Services;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IndentityApp.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccountController : ControllerBase
    {
        const string relayStateReturnUrl = "ReturnUrl";
        private readonly Saml2Configuration config;

        public AccountController(IOptions<Saml2Configuration> configAccessor)
        {
            config = configAccessor.Value;
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            var binding = new ITfoxtec.Identity.Saml2.Saml2RedirectBinding();
            binding.SetRelayStateQuery(new Dictionary<string, string> { { relayStateReturnUrl, returnUrl ?? Url.Content("~/home") } });

            return binding.Bind(new Saml2AuthnRequest(config)).ToActionResult();
        }

        [Route("AssertionConsumerService")]
        [HttpPost]
        public async Task<IActionResult> AssertionConsumerService()
        {
            var binding = new Saml2PostBinding();
            var saml2AuthnResponse = new Saml2AuthnResponse(config);

            binding.ReadSamlResponse(Request.ToGenericHttpRequest(), saml2AuthnResponse);
            if (saml2AuthnResponse.Status != Saml2StatusCodes.Success)
            {
                throw new AuthenticationException($"SAML Response status: {saml2AuthnResponse.Status}");
            }
            binding.Unbind(Request.ToGenericHttpRequest(), saml2AuthnResponse);
            await saml2AuthnResponse.CreateSession(HttpContext, claimsTransform: (claimsPrincipal) => ClaimsTransform.Transform(claimsPrincipal));

            var relayStateQuery = binding.GetRelayStateQuery();
            var returnUrl = relayStateQuery.ContainsKey(relayStateReturnUrl) ? relayStateQuery[relayStateReturnUrl] : Url.Content("~/");
            return Redirect(returnUrl);
        }

        [Route("Logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect(Url.Content("~/"));
            }

            var binding = new Saml2PostBinding();
            var saml2LogoutRequest = await new Saml2LogoutRequest(config, User).DeleteSession(HttpContext);
            return Redirect("~/");
        }

        [Route("CurrentUser")]
        [HttpGet]
        public User CurrentUser()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }
            else
            {
                return new User
                {
                    FirstName = User.Claims.Where(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")).FirstOrDefault().Value,
                    LastName = User.Claims.Where(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")).FirstOrDefault().Value,
                    Email = User.Claims.Where(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")).FirstOrDefault().Value
                };
            }
        }

        #region Private Methods
        #endregion
    }
}
