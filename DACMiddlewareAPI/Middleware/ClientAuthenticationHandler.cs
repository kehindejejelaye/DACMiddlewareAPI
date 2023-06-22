using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DACMiddlewareAPI.Interfaces;

namespace DACMiddlewareAPI.Middleware;

public class ClientAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
        private const string AppIdHeaderName = "APP_ID";
        private const string AppKeyHeaderName = "APP_KEY";

        private readonly IUserRepository _userRepository;

        public ClientAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserRepository userRepository)
            : base(options, logger, encoder, clock)
        {
            _userRepository = userRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AppIdHeaderName) || !Request.Headers.ContainsKey(AppKeyHeaderName))
            {
                return AuthenticateResult.Fail("Missing app_id or app_key header.");
            }

            var appId = Request.Headers[AppIdHeaderName].ToString();
            var appKey = Request.Headers[AppKeyHeaderName].ToString();

            var client = await _userRepository.GetClient(appId);

            if (client == null)
            {
                return AuthenticateResult.Fail("Invalid app_id.");
            }

            if (!string.Equals(client.AppKeyHash, appKey))
            {
                return AuthenticateResult.Fail("Invalid app_key.");
            }

            var claims = new[]
            {
                new Claim("appId", appId.ToString()),
                // Add additional claims as needed
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
 }

    //public static class AppIdAppKeyAuthenticationExtensions
    //{
    //    public static AuthenticationBuilder AddAppIdAppKeyAuthentication(this AuthenticationBuilder builder, Action<AuthenticationSchemeOptions> configureOptions)
    //    {
    //        return builder.AddScheme<AuthenticationSchemeOptions, AppIdAppKeyAuthenticationHandler>("AppIdAppKey", "AppIdAppKey", configureOptions);
    //    }
    //}

