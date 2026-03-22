namespace LeftoverChef;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnFridgeClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FridgePage());
    }

    private async void OnStorageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new StoragePage());
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SearchPage());
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CreatePage());
    }

    private async void OnExitClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlertAsync("Exit", "Are you sure you want to exit?", "Yes", "No");
        if (answer)
        {
            Application.Current?.Quit();
        }
    }
}