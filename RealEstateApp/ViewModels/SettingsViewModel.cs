using Microsoft.Maui.Storage;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    const string VolumeKey = "tts_volume";
    const string PitchKey = "tts_pitch";

    double volume;
    public double Volume
    {
        get => volume;
        set => SetProperty(ref volume, value);
    }

    double pitch;
    public double Pitch
    {
        get => pitch;
        set => SetProperty(ref pitch, value);
    }

    public ICommand ResetCommand => new Command(ResetDefaults);

    public void LoadSettings()
    {
        Volume = Preferences.Default.Get(VolumeKey, 0.5);
        Pitch = Preferences.Default.Get(PitchKey, 1.0);
    }

    public void SaveSettings()
    {
        Preferences.Default.Set(VolumeKey, Volume);
        Preferences.Default.Set(PitchKey, Pitch);
    }

    void ResetDefaults()
    {
        Volume = 0.5;
        Pitch = 1.0;
    }
}
