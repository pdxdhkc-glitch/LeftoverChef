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
        // 必填项检查，防空数据 (Check required fields)
        if (CategoryPicker.SelectedIndex == -1 || string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            Microsoft.Maui.Devices.Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200));
            await DisplayAlertAsync("Notice", "Fill name and category", "OK");
            return;
        }

        // 1. 把表单打包成菜谱对象 (Package form into Recipe object)
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

        // 加到内存列表 (Add to memory list)
        // 过渡期保护 (Safety fallback)
        App.GlobalRecipes.Add(newRecipe);

        // 弹窗并返回 (Show alert and go back)
        await DisplayAlertAsync("Success", "Recipe saved to database!", "OK");
        await Navigation.PopAsync();
    }

    // 点击跳外部网站 (Open external web link)
    private async void OnWebsiteLinkTapped(object sender, TappedEventArgs e)
    {
        // 获取网址字符串 (Get URL string)
        var url = e.Parameter?.ToString();
        if (string.IsNullOrEmpty(url)) return;

        try
        {
            // 调用系统浏览器 (Open native browser)
            await Launcher.Default.OpenAsync(new Uri(url));
        }
        catch
        {
            // 报错防崩溃 (Prevent crash on error)
            await DisplayAlertAsync("Error", "Could not open website", "OK");
        }
    }

    // 回主页 (Back home)
    private async void OnHomeClicked(object sender, EventArgs e) => await Navigation.PopAsync();
}