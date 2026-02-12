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
        vm.GetCoordinatesFromAddressAction = GetCoordinatesFromAddress;
    }

    async Task GetCurrentLocation()
    {
        var permission =
            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        if (permission != PermissionStatus.Granted)
            return;

        var location =
            await Geolocation.Default.GetLocationAsync();

        if (location != null &&
            BindingContext is AddEditPropertyPageViewModel vm)
        {
            vm.Property.Latitude = location.Latitude;
            vm.Property.Longitude = location.Longitude;

            var placemarks =
                await Geocoding.Default.GetPlacemarksAsync(location);

            var place = placemarks?.FirstOrDefault();

            if (place != null)
            {
                vm.Property.Address =
                    $"{place.Thoroughfare} {place.SubThoroughfare}, {place.PostalCode} {place.Locality}";
            }

            vm.RefreshProperty();
        }
    }

    async Task GetCoordinatesFromAddress()
    {
        if (BindingContext is not AddEditPropertyPageViewModel vm)
            return;

        if (string.IsNullOrWhiteSpace(vm.Property.Address))
        {
            await DisplayAlert("Missing address",
                               "Please enter an address first.",
                               "OK");
            return;
        }

        var locations =
            await Geocoding.Default.GetLocationsAsync(vm.Property.Address);

        var location = locations?.FirstOrDefault();

        if (location != null)
        {
            vm.Property.Latitude = location.Latitude;
            vm.Property.Longitude = location.Longitude;

            vm.RefreshProperty();
        }
    }
}
