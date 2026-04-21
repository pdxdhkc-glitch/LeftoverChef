// File: CreatePage.xaml.cs
// Role: User-generated content (UGC) handler.
// Function: Validates and saves new user recipes into the local SQLite database and global collection.
// Features: System browser integration for external recipe links.
namespace LeftoverChef;

public partial class CreatePage : ContentPage
{
    public CreatePage() { InitializeComponent(); }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Check required fields
        if (CategoryPicker.SelectedIndex == -1 || string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            Microsoft.Maui.Devices.Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200));
            await DisplayAlertAsync("Notice", "Fill name and category", "OK");
            return;
        }

        // Package form into Recipe object
        var newRecipe = new Recipe
        {
            Name = NameEntry.Text,
            Category = CategoryPicker.SelectedItem?.ToString() ?? "Chinese",
            CookingTime = TimeEntry.Text ?? "20m",
            Description = DescEntry.Text ?? "",
            Ingredients = IngredientsEditor.Text ?? "",
            Instructions = InstructionsEditor.Text ?? ""
        };

        await App.Database.SaveRecipeAsync(newRecipe);

        // Add to memory list
        // Safety fallback
        App.GlobalRecipes.Add(newRecipe);

        // Show alert and go back
        await DisplayAlertAsync("Success", "Recipe saved to database!", "OK");
        await Navigation.PopAsync();
    }

    // Open external web link
    private async void OnWebsiteLinkTapped(object sender, TappedEventArgs e)
    {
        // Get URL string
        var url = e.Parameter?.ToString();
        if (string.IsNullOrEmpty(url)) return;

        try
        {
            // Open native browser
            await Launcher.Default.OpenAsync(new Uri(url));
        }
        catch
        {
            // Prevent crash on error
            await DisplayAlertAsync("Error", "Could not open website", "OK");
        }
    }

    // Back home
    private async void OnHomeClicked(object sender, EventArgs e) => await Navigation.PopAsync();
}