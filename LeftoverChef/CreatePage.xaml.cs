namespace LeftoverChef;

public partial class CreatePage : ContentPage
{
    public CreatePage() { InitializeComponent(); }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string name = NewRecipeName.Text?.Trim() ?? string.Empty;
        string ingredients = NewRecipeIngredients.Text?.Trim() ?? string.Empty;
        string desc = NewRecipeDesc.Text?.Trim() ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(ingredients))
        {
            App.GlobalRecipes.Add(new Recipe { Name = name, Ingredients = ingredients, Description = desc });
            await DisplayAlertAsync("Success", "Recipe saved to storage!", "OK");

            NewRecipeName.Text = string.Empty;
            NewRecipeIngredients.Text = string.Empty;
            NewRecipeDesc.Text = string.Empty;
        }
        else
        {
            await DisplayAlertAsync("Warning", "Name and Ingredients cannot be empty.", "Got it");
        }
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}