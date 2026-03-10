namespace LeftoverChef;

public partial class MainPage : ContentPage
{
    public MainPage() { InitializeComponent(); }
    private async void OnSearchClicked(object sender, EventArgs e) { await Navigation.PushAsync(new SearchPage()); }
    private async void OnStorageClicked(object sender, EventArgs e) { await Navigation.PushAsync(new StoragePage()); }
    private async void OnCreateClicked(object sender, EventArgs e) { await Navigation.PushAsync(new CreatePage()); }
    private void OnExitClicked(object sender, EventArgs e) { Application.Current.Quit(); }
}