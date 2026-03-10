namespace LeftoverChef;

// Logic for the page where users create and save new recipes
// 用户创建并保存新食谱页面的逻辑
public partial class CreatePage : ContentPage
{
    // Constructor: initialize page components
    // 构造函数：初始化页面组件
    public CreatePage() { InitializeComponent(); }

    // Event handler for the Save button click
    // 保存按钮点击事件处理程序
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Get and trim input values, handling potential nulls
        // 获取并修整输入值，处理可能的空值
        string name = NewRecipeName.Text?.Trim() ?? string.Empty;
        string ingredients = NewRecipeIngredients.Text?.Trim() ?? string.Empty;
        string desc = NewRecipeDesc.Text?.Trim() ?? string.Empty;

        // Validate that mandatory fields are not empty
        // 验证必填字段不能为空
        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(ingredients))
        {
            // Add the new recipe to the global collection database
            // 将新食谱添加到全局集合数据库中
            App.GlobalRecipes.Add(new Recipe { Name = name, Ingredients = ingredients, Description = desc });

            // Show a success message to the user
            // 向用户显示成功提示
            await DisplayAlertAsync("Success", "Recipe saved to storage!", "OK");

            // Reset input fields for the next entry
            // 重置输入框以便下次输入
            NewRecipeName.Text = string.Empty;
            NewRecipeIngredients.Text = string.Empty;
            NewRecipeDesc.Text = string.Empty;
        }
        else
        {
            // Show a warning if validation fails
            // 如果验证失败则显示警告
            await DisplayAlertAsync("Warning", "Name and Ingredients cannot be empty.", "Got it");
        }
    }

    // Event handler to return to the application's root page
    // 返回应用程序根页面（主页）的事件处理程序
    private async void OnHomeClicked(object sender, EventArgs e)
    {
        // Pop all pages off the navigation stack until reaching the root
        // 从导航栈中弹出所有页面，直到返回根页面
        await Navigation.PopToRootAsync();
    }
}