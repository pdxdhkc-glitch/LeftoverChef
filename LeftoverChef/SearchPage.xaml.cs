namespace LeftoverChef;

// Recipe search logic
// 食谱搜索逻辑
public partial class SearchPage : ContentPage
{
    // Initialize search page
    // 初始化搜索页面
    public SearchPage() { InitializeComponent(); }

    // Filter recipes by ingredients
    // 根据食材筛选食谱
    private void OnFindMatchingRecipesClicked(object sender, EventArgs e)
    {
        // Get search keyword
        // 获取搜索关键词
        string searchKeyword = IngredientInput.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(searchKeyword))
        {
            BindableLayout.SetItemsSource(ResultsList, null);
            return;
        }

        // Perform case-insensitive search
        // 执行不区分大小写的搜索
        var matchedRecipes = App.GlobalRecipes
            .Where(r => r.Ingredients.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Update the results display
        // 更新结果显示列表
        BindableLayout.SetItemsSource(ResultsList, matchedRecipes);
    }

    // Return to main menu
    // 返回主菜单
    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}