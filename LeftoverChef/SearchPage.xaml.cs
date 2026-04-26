// File: SearchPage.xaml.cs
// Search & Inventory Check
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace LeftoverChef;

public partial class SearchPage : ContentPage
{
    public SearchPage()
    {
        InitializeComponent();
    }

    // Search logic
    private async void OnSearchClicked(object sender, EventArgs e)
    {
        // Normalize input
        string query = SearchEntry.Text?.ToLower().Trim() ?? "";
        ResultsList.Children.Clear();

        // Empty check
        if (string.IsNullOrWhiteSpace(query))
        {
            // Vibration fallback with Debug log
            try
            {
                
                System.Diagnostics.Debug.WriteLine("=== 📱 Trigger system vibration！Bzzzz! ===");

                Microsoft.Maui.Devices.Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200));
            }
            catch { }

            await this.DisplayAlertAsync("Notice", "Please enter a keyword to search.", "OK");
            return;
        }
        // Fetch from DB
        var allRecipes = await App.Database.GetRecipesAsync();

        // Fuzzy match
        var matches = allRecipes.Where(r =>
            (r.Name != null && r.Name.ToLower().Contains(query)) ||
            (r.Ingredients != null && r.Ingredients.ToLower().Contains(query))
        ).ToList();

        // Empty state
        if (matches.Count == 0)
        {
            ResultsList.Children.Add(new Label
            {
                Text = "No recipes found. Try another keyword!",
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 20, 0, 0)
            });
            return;
        }

        // Render cards
        foreach (var r in matches)
        {
            // Card container
            var card = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                BackgroundColor = Color.FromRgba(255, 255, 255, 0.95),
                StrokeThickness = 0,
                Padding = new Thickness(0),
                Margin = new Thickness(0, 5)
            };

            // Grid layout
            var grid = new Grid
            {
                ColumnDefinitions = {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                }
            };

            // Category bar
            var colorBar = new BoxView
            {
                WidthRequest = 6,
                BackgroundColor = r.Category == "Chinese" ? Color.FromArgb("#E65100") : Color.FromArgb("#283593")
            };

            // Text info
            var textLayout = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(15, 10),
                Spacing = 2
            };
            textLayout.Children.Add(new Label { Text = r.Name, FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
            textLayout.Children.Add(new Label { Text = "⏱️ " + r.CookingTime + " | " + r.Category, FontSize = 13, TextColor = Color.FromArgb("#7F8C8D") });

            // Inventory check button
            var checkBtn = new Button
            {
                Text = "🛒 Check",
                FontSize = 12,
                Padding = new Thickness(10, 5),
                Margin = new Thickness(0, 0, 15, 0),
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Color.FromArgb("#F39C12"),
                TextColor = Colors.White,
                CornerRadius = 8
            };

            // Comparison logic
            checkBtn.Clicked += async (s, args) =>
            {
                // Get fridge items
                var myIngredients = await App.Database.GetIngredientsAsync();
                var myInvNames = myIngredients.Select(i => i.Name.ToLower().Trim()).ToList();

                // Parse required items
                var requiredItems = r.Ingredients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(i => i.Trim().ToLower())
                                                 .ToList();

                var missingItems = new List<string>();

                // Identify missing items
                foreach (var req in requiredItems)
                {
                    // Contains check
                    if (!myInvNames.Any(inv => inv.Contains(req) || req.Contains(inv)))
                    {
                        missingItems.Add(req);
                    }
                }

                // Show results
                if (missingItems.Count == 0)
                {
                    await DisplayAlertAsync("Great!", "You have all the ingredients in your fridge!", "Awesome");
                }
                else
                {
                    // Format list
                    var missingText = string.Join("\n- ", missingItems.Select(m => char.ToUpper(m[0]) + m.Substring(1)));
                    await DisplayAlertAsync("Missing Ingredients", $"You need to buy:\n- {missingText}", "Got it");
                }
            };

            // Assemble card
            grid.Add(colorBar, 0);
            grid.Add(textLayout, 1);
            grid.Add(checkBtn, 2);
            card.Content = grid;

            // Go to details
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, args) =>
            {
                await Navigation.PushAsync(new StoragePage(r));
            };
            // Bind text click
            textLayout.GestureRecognizers.Add(tap);

            ResultsList.Children.Add(card);
        }
    }

    // Back to home
    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}