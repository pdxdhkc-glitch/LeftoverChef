// File: App.xaml.cs
// Main entry and data hub of the app
// Manages global variables and keeps data safe
using System;
using System.Collections.ObjectModel;
using SQLite;
using System.IO;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core;         // Fix: Import V14 core library
using Plugin.LocalNotification.Core.Models;  // Fix: Import V14 Request data models

namespace LeftoverChef;

public partial class App : Application
{
    // Global static lists, kept in memory as a fallback
    public static ObservableCollection<Recipe> GlobalRecipes { get; set; } = new ObservableCollection<Recipe>();
    public static ObservableCollection<Ingredient> GlobalIngredients { get; set; } = new ObservableCollection<Ingredient>();

    private static LocalDatabase? _database;
    public static LocalDatabase Database
    {
        get
        {
            if (_database == null)
            {
                // Create the DB file in the phone's local storage
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
            // --- 10 default Chinese Recipes ---
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

            // --- 10 default Western Recipes ---
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

        // Check and sync DB on startup
        SeedDatabaseAsync();
    }

    // ⭐ Added: App Lifecycle - OnStart (Check permissions and schedule here)
    protected override async void OnStart()
    {
        base.OnStart();

        // 1. Check and request notification permission
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

        // 2. Schedule after permission is handled
        ScheduleDailyCookingReminder();
    }

    // Added: Daily 5 PM notification logic
    private void ScheduleDailyCookingReminder()
    {
        // 1. Get the exact time for 5 PM (17:00) today
        var notifyTime = DateTime.Today.AddHours(17);
        

        // 2. Fallback logic: If the user opens the app after 5 PM, schedule the first reminder for 5 PM tomorrow
        if (DateTime.Now > notifyTime)
        {
                   notifyTime = notifyTime.AddDays(1);
        }

        // 3. Build the notification request
        var request = new NotificationRequest
        {
            NotificationId = 1001, // Fixed ID to prevent duplicate notifications
            Title = "LeftoverChef 👨‍🍳",
            Description = "It's 5 PM! Time to check your fridge and cook something delicious.",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = notifyTime,
                RepeatType = NotificationRepeat.Daily // Set to repeat daily
            }
        };

        // 4. Send to the system local notification center
        LocalNotificationCenter.Current.Show(request);
    }

    //  Method to seed default recipes into the DB
    private async void SeedDatabaseAsync()
    {
        // Check how many recipes are currently in the DB
        var dbRecipes = await Database.GetRecipesAsync();

        // If less than 20, force insert the defaults
        if (dbRecipes.Count < 20)
        {
            // Loop through memory recipes and save to DB
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

// Recipe data model
public class Recipe
{
    [PrimaryKey, AutoIncrement] // Primary key for DB, auto-increments
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string CookingTime { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

// Ingredient data model
public class Ingredient
{
    [PrimaryKey, AutoIncrement] // Primary key for DB, auto-increments
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}