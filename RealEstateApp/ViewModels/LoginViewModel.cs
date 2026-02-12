using Microsoft.Maui.Storage;
using RealEstateApp.Models;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

public class LoginViewModel : BaseViewModel
{
    const string AccessKey = "access_token";
    const string RefreshKey = "refresh_token";

    public string Username { get; set; }
    public string Password { get; set; }

    public ICommand LoginCommand =>
        new Command(async () => await LoginAsync());

    public async Task LoginAsync()
    {
        var result = Authenticate();

        if (!result.Succeeded)
        {
            await Shell.Current.DisplayAlert(
                "Login failed",
                "Invalid credentials",
                "OK");
            return;
        }

        await SecureStorage.Default.SetAsync(
            AccessKey,
            result.AccessToken);

        await SecureStorage.Default.SetAsync(
            RefreshKey,
            result.RefreshToken);

        await Shell.Current.GoToAsync("//propertylist");
    }

    LoginResult Authenticate()
    {
        if (Username == "admin" && Password == "admin")
        {
            var rnd = new Random();

            return new LoginResult
            {
                Succeeded = true,
                AccessToken = rnd.Next().ToString(),
                RefreshToken = rnd.Next().ToString()
            };
        }

        return new LoginResult { Succeeded = false };
    }

    public static async Task<bool> HasSavedLogin()
    {
        var token =
            await SecureStorage.Default.GetAsync(AccessKey);

        return token != null;
    }

    public static void Logout()
    {
        SecureStorage.Default.Remove(AccessKey);
        SecureStorage.Default.Remove(RefreshKey);
    }
}
