using Blazored.LocalStorage;
using Blazored.SessionStorage;

namespace HIOF.GamingSocial.GUI.Services;

public class AuthService
{
    private ISessionStorageService _sessionStorage;
    private ILocalStorageService _localStorage;
    public bool IsLoggedIn { get; private set; }

    public event Action OnChange;



    public AuthService(ISessionStorageService sessionStorage, ILocalStorageService localStorage)
    {
        _sessionStorage = sessionStorage;
        _localStorage = localStorage;
    }

    public async Task Login()
    {
        IsLoggedIn = true;
        await _sessionStorage.SetItemAsync("IsLoggedIn", "true");
        await _localStorage.SetItemAsync("IsLoggedIn", "true");
        NotifyStateChanged();
    }

    public async Task Logout()
    {
        IsLoggedIn = false;
        await _sessionStorage.RemoveItemAsync("IsLoggedIn");
        await _localStorage.RemoveItemAsync("IsLoggedIn");
        NotifyStateChanged();
    }

    public async Task CheckLoginStatus()
    {
        var sessionStorageResult = await _sessionStorage.GetItemAsync<string>("IsLoggedIn");
        var localStorageResult = await _localStorage.GetItemAsync<string>("IsLoggedIn");
        if (sessionStorageResult == "true")
        {
            IsLoggedIn = true;
        }
        else if (localStorageResult == "true")
        {
            IsLoggedIn = true;
            await _sessionStorage.SetItemAsync("IsLoggedIn", "true");
        }
        else
        {
            IsLoggedIn = false;
        }
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

}