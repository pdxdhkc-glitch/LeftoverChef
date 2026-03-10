namespace LeftoverChef;

public partial class SearchPage : ContentPage
{
    public SearchPage() { InitializeComponent(); }

    private void OnFindMatchingRecipesClicked(object sender, EventArgs e)
    {
        string searchKeyword = IngredientInput.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(searchKeyword))
        {
            BindableLayout.SetItemsSource(ResultsList, null);
            return;
        }

        var matchedRecipes = App.GlobalRecipes
            .Where(r => r.Ingredients.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Update the list / 更新列表数据
        BindableLayout.SetItemsSource(ResultsList, matchedRecipes);
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}   