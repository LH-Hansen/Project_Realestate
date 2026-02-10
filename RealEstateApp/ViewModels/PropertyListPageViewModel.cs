using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;
public class PropertyListPageViewModel : BaseViewModel
{
    public ObservableCollection<PropertyListItem> PropertiesCollection { get; } = new();

    private readonly IPropertyService service;

    public PropertyListPageViewModel(IPropertyService service)
    {
        Title = "Property List";
        this.service = service;
    }

    bool isRefreshing;
    public bool IsRefreshing
    {
        get => isRefreshing;
        set => SetProperty(ref isRefreshing, value);
    }

    private Command getPropertiesCommand;
    public ICommand GetPropertiesCommand => getPropertiesCommand ??= new Command(async () => await GetPropertiesAsync());

    public async Task GetPropertiesAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var properties = service.GetProperties();

            PropertiesCollection.Clear();
            foreach (var property in properties)
                PropertiesCollection.Add(new PropertyListItem(property));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get properties: {ex.Message}");
            await Shell.Current.DisplayAlertAsync("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    private Command<PropertyListItem> goToDetailsCommand;
    public ICommand GoToDetailsCommand => goToDetailsCommand ??= new Command<PropertyListItem>(async (item) => await GoToDetails(item));

    public async Task GoToDetails(PropertyListItem propertyListItem)
    {
        if (propertyListItem == null)
            return;

        await Shell.Current.GoToAsync(nameof(PropertyDetailPage), true, new Dictionary<string, object>
        {
            {"MyPropertyListItem", propertyListItem }
        });
    }

    private Command goToAddPropertyCommand;
    public ICommand GoToAddPropertyCommand => goToAddPropertyCommand ??= new Command(async () => await GotoAddProperty());

    public async Task GotoAddProperty()
    {
        await Shell.Current.GoToAsync($"{nameof(AddEditPropertyPage)}?mode=newproperty", true, new Dictionary<string, object>
        {
            {"MyProperty", new Property() }
        });
    }
}
