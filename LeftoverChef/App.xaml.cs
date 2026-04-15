// File: App.xaml.cs
// Main entry and data hub of the app
// Manages global variables and keeps data safe
using System.Collections.ObjectModel;
using SQLite; 
using System.IO;

namespace LeftoverChef;

public partial class App : Application
{
    // 全局静态列表用来防崩溃的，存内存里 (Global static lists, kept in memory as a fallback)
    public static ObservableCollection<Recipe> GlobalRecipes { get; set; } = new ObservableCollection<Recipe>();
    public static ObservableCollection<Ingredient> GlobalIngredients { get; set; } = new ObservableCollection<Ingredient>();

    private static LocalDatabase? _database;
    public static LocalDatabase Database
    {
        get
        {
            if (_database == null)
            {
                // 在手机本地沙盒里建一个叫 LeftoverChef.db3 的真实数据库文件 (Create the DB file in the phone's local storage)
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "LeftoverChef.db3");
                _database = new LocalDatabase(dbPath);
            }
            return _database;
        }
    }

    public App()
    {
        InitializeComponent();

        if (GlobalRecipes.Count == 0)
        {
            // --- 10 道中餐默认数据 (10 default Chinese Recipes) ---
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

            // --- 10 道西餐默认数据 (10 default Western Recipes) ---
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

        // App 刚启动时，检查数据库要不要塞初始数据 (Check and sync DB on startup)
        SeedDatabaseAsync();
    }

    //  20 道默认菜谱搬进数据库 (Method to seed default recipes into the DB)
    private async void SeedDatabaseAsync()
    {
        // 查硬盘里的数据库有几道菜 (Check how many recipes are currently in the DB)
        var dbRecipes = await Database.GetRecipesAsync();

        // 如果少于20道，就强制补齐初始数据 (If less than 20, force insert the defaults)
        if (dbRecipes.Count < 20)
        {
            // 遍历内存里的默认菜，挨个存进真实的数据库 (Loop through memory recipes and save to DB)
            foreach (var recipe in GlobalRecipes)
            {
                var newRecipeForDb = new Recipe
                {
                    Name = recipe.Name,
                    Category = recipe.Category,
                    CookingTime = recipe.CookingTime,
                    Ingredients = recipe.Ingredients,
                    Instructions = recipe.Instructions,
                    Description = recipe.Description
                };

                await Database.SaveRecipeAsync(newRecipeForDb);
            }
        }
    }

    protected override Window CreateWindow(IActivationState? activationState) => new Window(new AppShell());
}

// 菜谱的数据模型长这样 (Recipe data model)
public class Recipe
{
    [PrimaryKey, AutoIncrement] // 给数据库用的唯一身份证号，自动递增 (Primary key for DB, auto-increments)
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string CookingTime { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

// 冰箱里食材的数据模型 (Ingredient data model)
public class Ingredient
{
    [PrimaryKey, AutoIncrement] // 给数据库用的唯一身份证号，自动递增 (Primary key for DB, auto-increments)
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // 用来区分是放冷藏(Fridge)还是冷冻(Freezer)
}