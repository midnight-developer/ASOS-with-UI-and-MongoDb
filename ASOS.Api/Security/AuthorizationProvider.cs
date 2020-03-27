using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ASOS.Api.Data;
using ASOS.Api.Models;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ASOS.Api.Security
{
  public class AuthorizationProvider : OpenIdConnectServerProvider
  {
    public override Task ValidateTokenRequest(ValidateTokenRequestContext context)
    {
      if (!context.Request.IsPasswordGrantType() && !context.Request.IsRefreshTokenGrantType())
      {
        context.Reject(OpenIdConnectConstants.Errors.UnsupportedGrantType,
          "Only resource owner password credentials and refresh token are accepted by this authorization server");
        return Task.FromResult(0);
      }
      context.Skip();
      return Task.FromResult(0);
    }

    public override async Task HandleTokenRequest(HandleTokenRequestContext context)
    {
      if (context.Request.IsPasswordGrantType())
      {
        var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        var requestUsername = context.Request.Username;
        var requestPassword = context.Request.Password;

        if (!await userRepository.UserExists(requestUsername))
        {
          RejectAttempt(context);
          return;
        }

        var user = await userRepository.AuthenticateUser(requestUsername, requestPassword);

        if (user == null)
        {
          RejectAttempt(context);
          return;
        }

        var identity = new ClaimsIdentity(context.Scheme.Name, OpenIdConnectConstants.Claims.Name,
          OpenIdConnectConstants.Claims.Role);

        identity.AddClaim(OpenIdConnectConstants.Claims.Subject, user.Id);
        identity.AddClaim(OpenIdConnectConstants.Claims.Username, user.Username,
          OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);

        var properties = BuildProperties(user);
        var authenticationProperties = new AuthenticationProperties(properties);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var authenticationTicket = new AuthenticationTicket(claimsPrincipal, authenticationProperties, context.Scheme.Name);

        authenticationTicket.SetScopes(OpenIdConnectConstants.Scopes.OfflineAccess);
        context.Validate(authenticationTicket);
      }
    }

    public override Task 
      ApplyTokenResponse(ApplyTokenResponseContext context)
    {
      if (string.IsNullOrEmpty(context.Error))
      {
        context.Response["firstname"] = context.Ticket.Properties.GetProperty("firstname");
        context.Response["lastname"] = context.Ticket.Properties.GetProperty("lastname");
        context.Response["username"] = context.Ticket.Properties.GetProperty("username");
      }
      return Task.FromResult(0);
    }

    private static Dictionary<string, string> BuildProperties(User user)
    {
      return new Dictionary<string, string>
      {
        {"lastname", user.LastName },
        {"firstname", user.FirstName },
        {"username", user.Username }
      };
    }
    private static void RejectAttempt(HandleTokenRequestContext context)
    {
      context.Reject(OpenIdConnectConstants.Errors.InvalidGrant, "Invalid Credentials");
    }
  }
}