using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using ChatGptMiniApp.Shared.Domain.Dtos;

public class PersistingRevalidatingAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly PersistentComponentState _state;
  private readonly IdentityOptions _options;

  private readonly PersistingComponentStateSubscription _subscription;

  private Task<AuthenticationState>? _authenticationStateTask;

  public PersistingRevalidatingAuthenticationStateProvider(
      ILoggerFactory loggerFactory,
      IServiceScopeFactory scopeFactory,
      PersistentComponentState state,
      IOptions<IdentityOptions> options)
      : base(loggerFactory)
  {
    _scopeFactory = scopeFactory;
    _state = state;
    _options = options.Value;

    AuthenticationStateChanged += OnAuthenticationStateChanged;
    _subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
  }

  protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

  protected override async Task<bool> ValidateAuthenticationStateAsync(
      AuthenticationState authenticationState, CancellationToken cancellationToken)
  {
    // Get the user manager from a new scope to ensure it fetches fresh data
    await using var scope = _scopeFactory.CreateAsyncScope();
    return ValidateSecurityStampAsync(authenticationState.User);
  }

  private bool ValidateSecurityStampAsync(ClaimsPrincipal principal)
  {
    return principal.Identity?.IsAuthenticated is not false;
  }

  private void OnAuthenticationStateChanged(Task<AuthenticationState> authenticationStateTask)
  {
    _authenticationStateTask = authenticationStateTask;
  }

  private async Task OnPersistingAsync()
  {
    if (_authenticationStateTask is null)
    {
      throw new UnreachableException($"Authentication state not set in {nameof(RevalidatingServerAuthenticationStateProvider)}.{nameof(OnPersistingAsync)}().");
    }

    var authenticationState = await _authenticationStateTask;
    var principal = authenticationState.User;

    if (principal.Identity?.IsAuthenticated == true)
    {
      var name = principal.FindFirst(ClaimTypes.Name)?.Value;
      var email = principal.FindFirst(ClaimTypes.Email)?.Value;

      if (name != null)
      {
        _state.PersistAsJson(nameof(UserInfo), new UserInfo
        {
          Name = name,
          Email = email
        });
      }
    }
  }

    protected override void Dispose(bool disposing)
  {
    _subscription.Dispose();
    AuthenticationStateChanged -= OnAuthenticationStateChanged;
    base.Dispose(disposing);
  }
}

