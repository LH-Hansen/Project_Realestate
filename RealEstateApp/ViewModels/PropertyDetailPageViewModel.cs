using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Windows.Input;
using Microsoft.Maui.Media;

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

    // =========================
    // ===== SPEECH (NEW) ======
    // =========================

    bool isSpeaking;
    public bool IsSpeaking
    {
        get => isSpeaking;
        set
        {
            if (SetProperty(ref isSpeaking, value))
                OnPropertyChanged(nameof(IsNotSpeaking)); // keep UI in sync
        }
    }

    // NEW: no converter needed
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
            await TextToSpeech.Default.SpeakAsync(
                Property.Description,
                new SpeechOptions { Pitch = 1.0f, Volume = 1.0f },
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
}
