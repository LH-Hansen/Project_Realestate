using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Windows.Input;
using Microsoft.Maui.Media;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(PropertyListItem), "MyPropertyListItem")]
public class PropertyDetailPageViewModel : BaseViewModel
{
    private readonly IPropertyService service;

    public PropertyDetailPageViewModel(IPropertyService service)
    {
        this.service = service;
    }

    Property property;
    public Property Property
    {
        get => property;
        set => SetProperty(ref property, value);
    }

    Agent agent;
    public Agent Agent
    {
        get => agent;
        set => SetProperty(ref agent, value);
    }

    PropertyListItem propertyListItem;
    public PropertyListItem PropertyListItem
    {
        set
        {
            SetProperty(ref propertyListItem, value);
            Property = propertyListItem.Property;
            Agent = service.GetAgents().FirstOrDefault(x => x.Id == Property.AgentId);
        }
    }

    private Command editPropertyCommand;
    public ICommand EditPropertyCommand =>
        editPropertyCommand ??= new Command(async () => await GotoEditProperty());

    async Task GotoEditProperty()
    {
        if (Property == null)
            return;

        await Shell.Current.GoToAsync(
            $"{nameof(AddEditPropertyPage)}?mode=editproperty",
            true,
            new Dictionary<string, object> { { "MyProperty", Property } });
    }

    bool isSpeaking;
    public bool IsSpeaking
    {
        get => isSpeaking;
        set
        {
            if (SetProperty(ref isSpeaking, value))
                OnPropertyChanged(nameof(IsNotSpeaking)); 
        }
    }

    public bool IsNotSpeaking => !IsSpeaking;

    CancellationTokenSource speechToken;

    public ICommand SpeakCommand => new Command(async () => await Speak());
    public ICommand StopCommand => new Command(StopSpeaking);

    async Task Speak()
    {
        if (string.IsNullOrWhiteSpace(Property?.Description))
            return;

        speechToken?.Cancel();
        speechToken = new CancellationTokenSource();

        IsSpeaking = true;

        try
        {
            var volume = Preferences.Default.Get("tts_volume", 0.5);
            var pitch = Preferences.Default.Get("tts_pitch", 1.0);

            await TextToSpeech.Default.SpeakAsync(
                Property.Description,
                new SpeechOptions
                {
                    Pitch = (float)pitch,
                    Volume = (float)volume
                },
                speechToken.Token);

        }
        finally
        {
            IsSpeaking = false;
        }
    }

    void StopSpeaking()
    {
        speechToken?.Cancel();
        IsSpeaking = false;
    }

    public ICommand VendorPhoneCommand => new Command(async () =>
    {
        if (Property?.Vendor == null)
            return;

        string action = await Shell.Current.DisplayActionSheet(
            Property.Vendor.Phone,
            "Cancel",
            null,
            "Call",
            "SMS");

        if (action == "Call")
        {
            try
            {
                PhoneDialer.Default.Open(Property.Vendor.Phone);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error",
                    "Calling not supported.", "OK");
            }
        }
        else if (action == "SMS")
        {
            try
            {
                var message = new SmsMessage(
                    $"Hej, {Property.Vendor.FirstName}, angående {Property.Address}",
                    Property.Vendor.Phone);

                await Sms.Default.ComposeAsync(message);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error",
                    "SMS not supported.", "OK");
            }
        }
    });

    public ICommand VendorEmailCommand => new Command(async () =>
    {
        if (Property?.Vendor == null)
            return;

        try
        {
            var folder = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments);

            var filePath = Path.Combine(folder, "property.txt");

            File.WriteAllText(filePath, Property.Address);

            var email = new EmailMessage
            {
                Subject = "Property information",
                Body = "See attached property details.",
                To = new List<string> { Property.Vendor.Email }
            };

            email.Attachments.Add(
                new EmailAttachment(filePath));

            await Email.Default.ComposeAsync(email);
        }
        catch
        {
            await Shell.Current.DisplayAlert("Error",
                "Email not supported.", "OK");
        }
    });

    public ICommand OpenMapCommand => new Command(async () =>
    {
        if (Property?.Latitude == null || Property?.Longitude == null)
            return;

        var location = new Location(
            Property.Latitude.Value,
            Property.Longitude.Value);

        var options = new MapLaunchOptions
        {
            Name = Property.Address
        };

        await Map.Default.OpenAsync(location, options);
    });

    public ICommand OpenNavigationCommand => new Command(async () =>
    {
        if (Property?.Latitude == null || Property?.Longitude == null)
            return;

        var location = new Location(
            Property.Latitude.Value,
            Property.Longitude.Value);

        var options = new MapLaunchOptions
        {
            Name = Property.Address,
            NavigationMode = NavigationMode.Driving
        };

        await Map.Default.OpenAsync(location, options);
    });



}
