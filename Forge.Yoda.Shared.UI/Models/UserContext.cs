using Forge.Security.Jwt.Shared.Client.Models;
using Forge.Security.Jwt.Shared.Client.Services;
using System.ComponentModel;

namespace Forge.Yoda.Shared.UI.Models
{

    public class UserContext : INotifyPropertyChanged
    {

        protected readonly IAuthenticationService _authenticationService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public UserContext(IAuthenticationService authenticationService)
        {
            if (authenticationService == null) throw new ArgumentNullException(nameof(authenticationService));
            _authenticationService = authenticationService;
            _authenticationService.OnUserAuthenticationStateChanged += OnUserAuthenticationStateChangedEventHandler;
        }

        public User CurrentUser { get; protected set; } = new User();

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual async void OnUserAuthenticationStateChangedEventHandler(object? sender, UserDataEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.UserId))
            {
                await PropagateUserAsync();
            }
            else
            {
                ResetUser();
            }
        }

        protected virtual async Task PropagateUserAsync()
        {
            ParsedTokenData tokenData = await _authenticationService.GetCurrentUserInfoAsync();
            if (tokenData != null)
            {
                CurrentUser = new User(tokenData);
                NotifyPropertyChanged(nameof(CurrentUser));
            }
        }

        protected virtual void ResetUser()
        {
            CurrentUser = new User();
            NotifyPropertyChanged(nameof(CurrentUser));
        }

    }

}
