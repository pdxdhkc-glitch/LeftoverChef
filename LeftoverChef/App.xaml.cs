// File: App.xaml.cs
// Role: The central data hub of LeftoverChef.
// Function: Manages global static collections (GlobalRecipes, GlobalIngredients) to ensure data persistence across pages.
// Contains: Pre-set library of 20 Chinese and Western recipes.
using System.Collections.ObjectModel;

namespace LeftoverChef;

public partial class App : Application
{
    // 全局静态变量，确保数据在页面跳转时不会丢失
    public static ObservableCollection<Recipe> GlobalRecipes { get; set; } = new ObservableCollection<Recipe>();
    public static ObservableCollection<Ingredient> GlobalIngredients { get; set; } = new ObservableCollection<Ingredient>();

    public App()
    {
        InitializeComponent();

        if (GlobalRecipes.Count == 0)
        {
            // --- 10 Chinese Recipes (中餐) ---
            GlobalRecipes.Add(new Recipe { Name = "Kung Pao Chicken", Category = "Chinese", CookingTime = "25 mins", Ingredients = "Chicken, Peanuts, Chili", Instructions = "Stir-fry chicken with peanuts and spicy sauce.", Description = "Classic spicy Sichuan dish." });
            GlobalRecipes.Add(new Recipe { Name = "Mapo Tofu", Category = "Chinese", CookingTime = "20 mins", Ingredients = "Tofu, Minced Pork, Bean Paste", Instructions = "Simmer tofu in spicy minced meat sauce.", Description = "Famous numbing and spicy tofu." });
            GlobalRecipes.Add(new Recipe { Name = "Peking Duck", Category = "Chinese", CookingTime = "120 mins", Ingredients = "Duck, Honey, Pancakes", Instructions = "Roast duck and serve with pancakes.", Description = "World famous crispy duck." });
            GlobalRecipes.Add(new Recipe { Name = "Shrimp Dumplings", Category = "Chinese", CookingTime = "40 mins", Ingredients = "Shrimp, Starch, Bamboo", Instructions = "Fill wrappers and steam for 6 mins.", Description = "Delicate Cantonese Dim Sum." });
            GlobalRecipes.Add(new Recipe { Name = "Sweet and Sour Pork", Category = "Chinese", CookingTime = "35 mins", Ingredients = "Pork, Pineapple, Ketchup", Instructions = "Fry pork and toss in tangy sauce.", Description = "Crispy pork with fruity glaze." });
            GlobalRecipes.Add(new Recipe { Name = "Beef Chow Fun", Category = "Chinese", CookingTime = "15 mins", Ingredients = "Rice Noodles, Beef, Soy Sauce", Instructions = "Stir-fry beef and noodles at high heat.", Description = "Smoky Cantonese staple." });
            GlobalRecipes.Add(new Recipe { Name = "Hot and Sour Soup", Category = "Chinese", CookingTime = "20 mins", Ingredients = "Tofu, Mushrooms, Vinegar", Instructions = "Boil broth and season with vinegar/pepper.", Description = "Classic appetizer soup." });
            GlobalRecipes.Add(new Recipe { Name = "Steamed Fish", Category = "Chinese", CookingTime = "15 mins", Ingredients = "Fish, Ginger, Soy Sauce", Instructions = "Steam fish and pour hot oil over it.", Description = "Healthy and fresh seafood." });
            GlobalRecipes.Add(new Recipe { Name = "Boiled Beef", Category = "Chinese", CookingTime = "45 mins", Ingredients = "Beef, Cabbage, Chili Oil", Instructions = "Poach beef in fiery broth over cabbage.", Description = "Sichuan spicy favorite." });
            GlobalRecipes.Add(new Recipe { Name = "Char Siu", Category = "Chinese", CookingTime = "60 mins", Ingredients = "Pork, Honey, Hoisin Sauce", Instructions = "Marinate and roast until glazed.", Description = "Sweet Cantonese BBQ pork." });

            // --- 10 Western Recipes (西餐) ---
            GlobalRecipes.Add(new Recipe { Name = "Beef Wellington", Category = "Western", CookingTime = "90 mins", Ingredients = "Beef, Pastry, Mushrooms", Instructions = "Wrap beef in pastry and bake golden.", Description = "Elegant English masterpiece." });
            GlobalRecipes.Add(new Recipe { Name = "Spaghetti Carbonara", Category = "Western", CookingTime = "20 mins", Ingredients = "Pasta, Egg, Guanciale", Instructions = "Mix eggs and cheese into hot pasta.", Description = "Rich and creamy Roman pasta." });
            GlobalRecipes.Add(new Recipe { Name = "Margherita Pizza", Category = "Western", CookingTime = "25 mins", Ingredients = "Dough, Tomato, Mozzarella", Instructions = "Bake dough with fresh toppings.", Description = "Authentic Italian flavors." });
            GlobalRecipes.Add(new Recipe { Name = "Classic Caesar Salad", Category = "Western", CookingTime = "15 mins", Ingredients = "Lettuce, Croutons, Parmesan", Instructions = "Toss Romaine with savory dressing.", Description = "Crisp and garlicky salad." });
            GlobalRecipes.Add(new Recipe { Name = "Grilled Ribeye Steak", Category = "Western", CookingTime = "15 mins", Ingredients = "Steak, Butter, Rosemary", Instructions = "Sear steak and baste with butter.", Description = "Juicy and tender beef." });
            GlobalRecipes.Add(new Recipe { Name = "Mac and Cheese", Category = "Western", CookingTime = "40 mins", Ingredients = "Macaroni, Cheddar, Milk", Instructions = "Make cheese roux and bake with pasta.", Description = "Ultimate baked comfort food." });
            GlobalRecipes.Add(new Recipe { Name = "French Onion Soup", Category = "Western", CookingTime = "60 mins", Ingredients = "Onions, Broth, Gruyere", Instructions = "Caramelize onions and broil with cheese.", Description = "Deeply flavorful savory soup." });
            GlobalRecipes.Add(new Recipe { Name = "Chicken Alfredo", Category = "Western", CookingTime = "30 mins", Ingredients = "Pasta, Chicken, Heavy Cream", Instructions = "Cook chicken and toss with cream sauce.", Description = "Luxurious creamy pasta." });
            GlobalRecipes.Add(new Recipe { Name = "Fish and Chips", Category = "Western", CookingTime = "40 mins", Ingredients = "Cod, Potato, Beer Batter", Instructions = "Batter and fry fish until crispy.", Description = "British seaside classic." });
            GlobalRecipes.Add(new Recipe { Name = "Eggs Benedict", Category = "Western", CookingTime = "25 mins", Ingredients = "Muffin, Poached Egg, Hollandaise", Instructions = "Top toasted muffin with egg and sauce.", Description = "Decadent brunch favorite." });
        }
    }

    protected override Window CreateWindow(IActivationState? activationState) => new Window(new AppShell());
}

// 食谱类定义(Recipe Class Definition)
public class Recipe
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string CookingTime { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

// 食材类定义(Food ingredient definition)
public class Ingredient
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Fridge 或 Freezer
}
