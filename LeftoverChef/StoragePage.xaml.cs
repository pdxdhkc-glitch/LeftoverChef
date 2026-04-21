// File: StoragePage.xaml.cs
// Recipe library and detail view
// Fetches data directly from SQLite
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace LeftoverChef;

public partial class StoragePage : ContentPage
{
    // Undo stack
    private Stack<List<Recipe>> _undoStack = new Stack<List<Recipe>>();

    // Current category
    private string _currentCat = "";

    // State: 0=Cat, 1=List, 2=Detail, 3=Search
    private int _state = 0;

    // Default constructor
    public StoragePage() { InitializeComponent(); }

    // Constructor for search results
    public StoragePage(Recipe searchedRecipe)
    {
        InitializeComponent();

        // Fill details
        DetailName.Text = searchedRecipe.Name;
        DetailTime.Text = "⏱️ " + searchedRecipe.CookingTime;
        DetailIngredients.Text = searchedRecipe.Ingredients;
        DetailInstructions.Text = searchedRecipe.Instructions;
        DetailDescription.Text = searchedRecipe.Description;

        // Hide lists, show detail directly
        CategoryView.IsVisible = false;
        RecipeListGrid.IsVisible = false;
        DetailView.IsVisible = true;
        DetailView.Opacity = 1;
        _state = 3;
        DynamicBottomButton.Text = "⬅️ Back to Search";
    }

    private async void OnCategoryTapped(object sender, TappedEventArgs e)
    {
        var param = e.Parameter?.ToString();
        if (string.IsNullOrEmpty(param)) return;

        _currentCat = param;
        ListTitleLabel.Text = _currentCat + " Recipes";

        // Refresh data from DB
        await RefreshListAsync();
        await Transition(CategoryView, RecipeListGrid);

        _state = 1;
        DynamicBottomButton.Text = "⬅️ Categories";
    }

    // Core: Query DB instead of memory
    private async Task RefreshListAsync()
    {
        RecipeListContainer.Children.Clear();

        // Fetch all recipes from SQLite
        var allRecipes = await App.Database.GetRecipesAsync();

        // Filter by category
        var list = allRecipes.Where(r => r.Category == _currentCat).ToList();

        foreach (var r in list)
        {
            var card = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                BackgroundColor = Color.FromRgba(255, 255, 255, 0.95),
                StrokeThickness = 0, // Remove border
                Padding = new Thickness(0), // Reset padding
                Margin = new Thickness(0, 6)
            };
            card.Shadow = new Shadow { Brush = Brush.Black, Opacity = 0.08f, Offset = new Point(0, 4), Radius = 8 };

            var grid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };

            // Category color bar
            var colorBar = new BoxView { WidthRequest = 6, BackgroundColor = r.Category == "Chinese" ? Color.FromArgb("#E65100") : Color.FromArgb("#283593") };

            var textLayout = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Padding = new Thickness(15, 10), Spacing = 2 };
            textLayout.Children.Add(new Label { Text = r.Name, FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
            textLayout.Children.Add(new Label { Text = "⏱️ " + r.CookingTime, FontSize = 13, TextColor = Color.FromArgb("#7F8C8D") });

            // Tap to view details
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) => {
                DetailName.Text = r.Name; DetailTime.Text = "⏱️ " + r.CookingTime;
                DetailIngredients.Text = r.Ingredients; DetailInstructions.Text = r.Instructions; DetailDescription.Text = r.Description;
                await Transition(RecipeListGrid, DetailView);
                _state = 2;
                DynamicBottomButton.Text = "⬅️ List";
            };
            card.GestureRecognizers.Add(tap);

            // Delete button
            var delBtn = new Button { Text = "🗑️", TextColor = Colors.Red, BackgroundColor = Colors.Transparent, FontSize = 20, Margin = new Thickness(0, 0, 10, 0) };
            delBtn.Clicked += async (s, e) => {
                _undoStack.Push(new List<Recipe> { r });
                // Delete from DB
                await App.Database.DeleteRecipeAsync(r);
                await RefreshListAsync();
            };

            grid.Add(colorBar, 0); grid.Add(textLayout, 1); grid.Add(delBtn, 2);
            card.Content = grid;
            RecipeListContainer.Children.Add(card);
        }
    }

    private async void OnDeleteAllClicked(object sender, EventArgs e)
    {
        var all = await App.Database.GetRecipesAsync();
        var current = all.Where(r => r.Category == _currentCat).ToList();
        if (current.Count > 0)
        {
            _undoStack.Push(new List<Recipe>(current));
            foreach (var r in current) await App.Database.DeleteRecipeAsync(r);
            await RefreshListAsync();
        }
    }

    private async void OnUndoClicked(object sender, EventArgs e)
    {
        if (_undoStack.Count > 0)
        {
            foreach (var r in _undoStack.Pop())
            {
                r.Id = 0; // Reset ID to insert as new
                await App.Database.SaveRecipeAsync(r);
            }
            await RefreshListAsync();
        }
        else await this.DisplayAlertAsync("Undo", "Nothing to restore", "OK");
    }

    private async void OnDynamicBottomClicked(object sender, EventArgs e)
    {
        // Handle back navigation
        if (_state == 3) await Navigation.PopAsync();
        else if (_state == 2) { await Transition(DetailView, RecipeListGrid); _state = 1; DynamicBottomButton.Text = "⬅️ Categories"; }
        else if (_state == 1) { await Transition(RecipeListGrid, CategoryView); _state = 0; DynamicBottomButton.Text = "🏠 Home"; }
        else await Navigation.PopAsync();
    }

    // Fade transition
    private async Task Transition(VisualElement h, VisualElement s)
    {
        await h.FadeToAsync(0, 150);
        h.IsVisible = false;
        s.Opacity = 0; s.IsVisible = true;
        await s.FadeToAsync(1, 150);
    }

    // Share button logic 
    private async void OnShareClicked(object sender, EventArgs e)
    {
        // Extract current recipe text
        string recipeName = DetailName.Text;
        string ingredients = DetailIngredients.Text;
        string shareText = $"Check out this recipe: {recipeName}\nIngredients: {ingredients}";

        // Encode text for URL safe
        string encodedText = Uri.EscapeDataString(shareText);

        // Show action sheet
        string action = await DisplayActionSheetAsync("Share to...", "Cancel", null, "WhatsApp", "WeChat");

        if (action == "WhatsApp")
        {
            try
            {
                // Try app protocol, fallback to web
                bool opened = await Launcher.Default.OpenAsync($"whatsapp://send?text={encodedText}");
                if (!opened) await Launcher.Default.OpenAsync($"https://wa.me/?text={encodedText}");
            }
            catch
            {
                await Launcher.Default.OpenAsync($"https://wa.me/?text={encodedText}");
            }
        }
        else if (action == "WeChat")
        {
            try
            {
                // WeChat opens app or official site
                bool opened = await Launcher.Default.OpenAsync("weixin://");
                if (!opened) await Launcher.Default.OpenAsync("https://weixin.qq.com/");
            }
            catch
            {
                await Launcher.Default.OpenAsync("https://weixin.qq.com/");
            }
        }
    }
}