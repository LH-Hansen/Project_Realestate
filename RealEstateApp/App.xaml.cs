using RealEstateApp.ViewModels;
using RealEstateApp.Views;

namespace RealEstateApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        Task.Run(async () =>
        {
            if (!await LoginViewModel.HasSavedLogin())
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.GoToAsync(nameof(LoginPage));
                });
            }
        });
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        return new Window(new AppShell());
    }
}
