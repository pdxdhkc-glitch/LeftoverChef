namespace LeftoverChef;

public partial class StoragePage : ContentPage
{
    // Store the deleted recipe and its index for the Undo feature
    // 存储被删除的食谱和它的位置，用于撤回功能
    private Recipe? _lastDeletedRecipe;
    private int _lastDeletedIndex = -1;

    public StoragePage() { InitializeComponent(); }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindableLayout.SetItemsSource(AllRecipesList, App.GlobalRecipes);
    }

    // Delete Logic / 删除逻辑
    private async void OnDeleteRecipeClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var recipeToDelete = button?.CommandParameter as Recipe;

        if (recipeToDelete != null)
        {
            bool isConfirm = await DisplayAlertAsync("Delete", $"Delete '{recipeToDelete.Name}'?", "Yes", "No");
            if (isConfirm)
            {
                // Backup before deleting / 删除前先备份数据
                _lastDeletedIndex = App.GlobalRecipes.IndexOf(recipeToDelete);
                _lastDeletedRecipe = recipeToDelete;

                // Remove from database / 从数据库删除
                App.GlobalRecipes.Remove(recipeToDelete);

                // Show the Undo button / 显示撤回按钮
                UndoButton.IsVisible = true;
            }
        }
    }

    // Undo Logic / 撤回逻辑
    private void OnUndoClicked(object sender, EventArgs e)
    {
        if (_lastDeletedRecipe != null && _lastDeletedIndex >= 0)
        {
            // Restore recipe to its original position / 恢复食谱到原来的位置
            App.GlobalRecipes.Insert(_lastDeletedIndex, _lastDeletedRecipe);

            // Clear backup and hide button / 清空备份并隐藏按钮
            _lastDeletedRecipe = null;
            _lastDeletedIndex = -1;
            UndoButton.IsVisible = false;
        }
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}