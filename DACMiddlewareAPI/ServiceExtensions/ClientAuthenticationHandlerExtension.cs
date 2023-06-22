using DACMiddlewareAPI.Middleware;
using Microsoft.AspNetCore.Authentication;

namespace DACMiddlewareAPI.ServiceExtensions;

public static class ClientAuthenticationHandlerExtension
{
    public static AuthenticationBuilder AddClientAuthentication(this AuthenticationBuilder builder, Action<AuthenticationSchemeOptions> configureOptions)
    {
        return builder.AddScheme<AuthenticationSchemeOptions, ClientAuthenticationHandler>("ClientAuth", "ClientAuth", configureOptions);
    }
}
