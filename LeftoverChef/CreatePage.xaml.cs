// CreatePage.xaml.cs
namespace LeftoverChef;

public partial class CreatePage : ContentPage
{
    public CreatePage() { InitializeComponent(); }
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (CategoryPicker.SelectedIndex == -1 || string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            await DisplayAlertAsync("Notice", "Fill name and category", "OK"); return;
        }
        App.GlobalRecipes.Add(new Recipe
        {
            Name = NameEntry.Text,
            Category = CategoryPicker.SelectedItem?.ToString() ?? "Chinese",
            CookingTime = TimeEntry.Text ?? "20m",
            Description = DescEntry.Text ?? "",
            Ingredients = IngredientsEditor.Text ?? "",
            Instructions = InstructionsEditor.Text ?? ""
        });
        await DisplayAlertAsync("Success", "Recipe saved!", "OK");
        await Navigation.PopAsync();
    }
    private async void OnHomeClicked(object sender, EventArgs e) => await Navigation.PopAsync();
}