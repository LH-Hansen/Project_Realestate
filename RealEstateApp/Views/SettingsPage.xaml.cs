using RealEstateApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateApp.Views
{
    public partial class SettingsPage : ContentPage
    {
        SettingsViewModel vm;

        public SettingsPage(SettingsViewModel vm)
        {
            InitializeComponent();
            BindingContext = this.vm = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.LoadSettings();
        }

        protected override void OnDisappearing()
        {
            vm.SaveSettings();
            base.OnDisappearing();
        }
    }

}
