// File: CreatePage.xaml.cs
// Role: User-generated content (UGC) handler.
// Function: Validates and saves new user recipes into the global collection.
// Features: System browser integration for external recipe links.
namespace LeftoverChef;

public partial class CreatePage : ContentPage
{
    public CreatePage() { InitializeComponent(); }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // 判空必填项 (Check required fields)
        if (CategoryPicker.SelectedIndex == -1 || string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            await DisplayAlertAsync("Notice", "Fill name and category", "OK"); return;
        }

        // 表单 (Save to global list)
        App.GlobalRecipes.Add(new Recipe
        {
            Name = NameEntry.Text,
            Category = CategoryPicker.SelectedItem?.ToString() ?? "Chinese",
            CookingTime = TimeEntry.Text ?? "20m",
            Description = DescEntry.Text ?? "",
            Ingredients = IngredientsEditor.Text ?? "",
            Instructions = InstructionsEditor.Text ?? ""
        });

        // 弹窗并退出 (Show alert & pop)
        await DisplayAlertAsync("Success", "Recipe saved!", "OK");
        await Navigation.PopAsync();
    }

    // 点击跳外部网站 (Open external web link)
    private async void OnWebsiteLinkTapped(object sender, TappedEventArgs e)
    {
        // 网址 (Get URL)
        var url = e.Parameter?.ToString();
        if (string.IsNullOrEmpty(url)) return;

        try
        {
            // 唤醒系统浏览器 (Call system browser)
            await Launcher.Default.OpenAsync(new Uri(url));
        }
        catch
        {
            // 报错防崩 (Prevent crash on error)
            await DisplayAlertAsync("Error", "Could not open website", "OK");
        }
    }

    // 回主页 (Back home)
    private async void OnHomeClicked(object sender, EventArgs e) => await Navigation.PopAsync();
}