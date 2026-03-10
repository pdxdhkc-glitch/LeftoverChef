using System.Collections.ObjectModel;

namespace LeftoverChef;

// Data model representing a recipe
// 代表食谱的数据模型
public class Recipe
{
    // Initialize properties with string.Empty to avoid null warnings
    // 使用 string.Empty 初始化属性，避免空值警告
    public string Name { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public partial class App : Application
{
    // 10 built-in recipes serving as a global database
    // 10个内置食谱，作为全局数据库
    public static ObservableCollection<Recipe> GlobalRecipes { get; set; } = new ObservableCollection<Recipe>
    {
        new Recipe { Name = "Tomato & Egg", Ingredients = "Tomato, Egg", Description = "Classic comfort food, sweet and sour." },
        new Recipe { Name = "Pepper Pork", Ingredients = "Pepper, Pork", Description = "Quick, spicy and delicious." },
        new Recipe { Name = "Kung Pao Chicken", Ingredients = "Chicken, Peanut, Cucumber", Description = "Spicy, sweet, and crispy." },
        new Recipe { Name = "Mapo Tofu", Ingredients = "Tofu, Beef, Pepper", Description = "Spicy and flavorful." },
        new Recipe { Name = "Garlic Broccoli", Ingredients = "Broccoli, Garlic", Description = "Healthy and low-calorie." },
        new Recipe { Name = "Beef & Potato Stew", Ingredients = "Potato, Beef", Description = "Rich soup and soft potatoes." },
        new Recipe { Name = "Cucumber Salad", Ingredients = "Cucumber, Garlic, Vinegar", Description = "Refreshing summer dish." },
        new Recipe { Name = "Egg & Chives", Ingredients = "Chives, Egg", Description = "Simple and smells amazing." },
        new Recipe { Name = "Braised Pork", Ingredients = "Pork, Soy Sauce, Sugar", Description = "Melts in your mouth." },
        new Recipe { Name = "Sour & Spicy Potato", Ingredients = "Potato, Vinegar, Chili", Description = "Appetizing everyday dish." }
    };

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Wrap the MainPage in a NavigationPage
        // 用 NavigationPage 包裹 MainPage
        return new Window(new NavigationPage(new MainPage()));
    }
}