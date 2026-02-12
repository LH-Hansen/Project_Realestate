using RealEstateApp.ViewModels;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;

namespace RealEstateApp.Views;

public partial class AddEditPropertyPage : ContentPage
{
    public AddEditPropertyPage(AddEditPropertyPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        vm.GetLocationAction = GetCurrentLocation;
    }

    async Task GetCurrentLocation()
    {
        var permission =
            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        if (permission != PermissionStatus.Granted)
            return;

        var request = new GeolocationRequest(
            GeolocationAccuracy.Medium,
            TimeSpan.FromSeconds(10));

        var location =
            await Geolocation.Default.GetLocationAsync(request);

        if (location != null &&
            BindingContext is AddEditPropertyPageViewModel vm &&
            vm.Property != null)
        {
            vm.Property.Latitude = location.Latitude;
            vm.Property.Longitude = location.Longitude;

            vm.RefreshProperty();
        }
    }
}
