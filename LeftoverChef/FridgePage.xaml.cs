// File: FridgePage.xaml.cs
// Fridge & Smart Match
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace LeftoverChef
{
    public partial class FridgePage : ContentPage
    {
        private Stack<List<Ingredient>> _undoStack = new Stack<List<Ingredient>>();
        private string _currentCategory = "";
        private int _currentState = 0;

        // Max capacity
        private const int MAX_CAPACITY = 30;

        public FridgePage() { InitializeComponent(); }

        // Smart match logic
        private async void OnSmartMatchTapped(object sender, EventArgs e)
        {
            var allIngredients = await App.Database.GetIngredientsAsync();
            var fridgeItems = allIngredients.Select(i => i.Name.ToLower()).ToList();

            if (fridgeItems.Count == 0)
            {
                await this.DisplayAlertAsync("Empty", "Add some food to your fridge first!", "OK");
                return;
            }

            var allRecipes = await App.Database.GetRecipesAsync();

            var matches = allRecipes
                .Select(recipe => new
                {
                    Data = recipe,
                    Count = fridgeItems.Count(item => recipe.Ingredients != null && recipe.Ingredients.ToLower().Contains(item))
                })
                .Where(m => m.Count > 0)
                .OrderByDescending(m => m.Count)
                .Take(3)
                .ToList();

            if (matches.Count == 0)
            {
                await this.DisplayAlertAsync("No Match", "Try adding different ingredients!", "OK");
            }
            else
            {
                string resultText = "Based on your fridge, we suggest:\n\n";
                foreach (var m in matches)
                {
                    resultText += $"⭐ {m.Data.Name} ({m.Count} items matched)\n";
                }
                await this.DisplayAlertAsync("Smart Match Results", resultText, "Got it!");
            }
        }

        // Category switch
        private async void OnCategoryTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is string category)
            {
                _currentCategory = category;
                ListTitleLabel.Text = category == "Fridge" ? "🥬 Fridge Items" : "❄️ Freezer Items";

                await RefreshListAsync();
                await TransitionViews(CategoryView, IngredientListGrid);

                _currentState = 1;
                DynamicBottomButton.Text = "⬅️ Back to Storage";
            }
        }

        // Add item
        private async void OnAddIngredientClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewIngredientEntry.Text))
            {
                // Vibration fallback
                try { Microsoft.Maui.Devices.Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100)); } catch { }
                return;
            }

            // Capacity check
            var allIngredients = await App.Database.GetIngredientsAsync();
            int currentCount = allIngredients.Count(i => i.Category == _currentCategory);

            if (currentCount >= MAX_CAPACITY)
            {
                // Block and vibrate safely
                try { Microsoft.Maui.Devices.Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200)); } catch { }
                await this.DisplayAlertAsync("Storage Full", "You can only store up to 30 items here.", "OK");
                return;
            }

            var newIng = new Ingredient { Name = NewIngredientEntry.Text, Category = _currentCategory };

            await App.Database.SaveIngredientAsync(newIng);
            App.GlobalIngredients.Add(newIng);

            NewIngredientEntry.Text = string.Empty;
            await RefreshListAsync();
        }

        // Delete item
        private async void OnDeleteIngredientClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Ingredient item)
            {
                _undoStack.Push(new List<Ingredient> { item });

                await App.Database.DeleteIngredientAsync(item);

                var memItem = App.GlobalIngredients.FirstOrDefault(m => m.Id == item.Id || m.Name == item.Name);
                if (memItem != null) App.GlobalIngredients.Remove(memItem);

                await RefreshListAsync();
            }
        }

        // Clear all
        private async void OnDeleteAllClicked(object sender, EventArgs e)
        {
            var allIngredients = await App.Database.GetIngredientsAsync();
            var current = allIngredients.Where(i => i.Category == _currentCategory).ToList();

            if (current.Count > 0)
            {
                _undoStack.Push(new List<Ingredient>(current));
                foreach (var item in current)
                {
                    await App.Database.DeleteIngredientAsync(item);
                    var memItem = App.GlobalIngredients.FirstOrDefault(m => m.Id == item.Id || m.Name == item.Name);
                    if (memItem != null) App.GlobalIngredients.Remove(memItem);
                }
                await RefreshListAsync();
            }
        }

        // Undo action
        private async void OnUndoClicked(object sender, EventArgs e)
        {
            if (_undoStack.Count > 0)
            {
                foreach (var item in _undoStack.Pop())
                {
                    item.Id = 0;
                    await App.Database.SaveIngredientAsync(item);
                    App.GlobalIngredients.Add(item);
                }
                await RefreshListAsync();
            }
            else await this.DisplayAlertAsync("Undo", "Nothing to restore", "OK");
        }

        // Bottom navigation
        private async void OnDynamicBottomClicked(object sender, EventArgs e)
        {
            if (_currentState == 1)
            {
                await TransitionViews(IngredientListGrid, CategoryView);
                _currentState = 0;
                DynamicBottomButton.Text = "🏠 Back to Home";
            }
            else
            {
                await Navigation.PopAsync();
            }
        }

        // Fade transition
        private async Task TransitionViews(VisualElement viewToHide, VisualElement viewToShow)
        {
            await viewToHide.FadeToAsync(0, 150);
            viewToHide.IsVisible = false;
            viewToShow.Opacity = 0;
            viewToShow.IsVisible = true;
            await viewToShow.FadeToAsync(1, 150);
        }

        // Refresh list and progress UI
        private async Task RefreshListAsync()
        {
            var allIngredients = await App.Database.GetIngredientsAsync();
            var filtered = allIngredients.Where(i => i.Category == _currentCategory).ToList();
            BindableLayout.SetItemsSource(IngredientListContainer, filtered);

            // Update capacity UI
            int count = filtered.Count;
            CapacityLabel.Text = $"{count} / {MAX_CAPACITY}";
            CapacityProgressBar.Progress = (double)count / MAX_CAPACITY;

            // Color threshold logic
            if (count >= MAX_CAPACITY)
            {
                CapacityProgressBar.ProgressColor = Colors.Red;
            }
            else if (count >= 25)
            {
                CapacityProgressBar.ProgressColor = Color.FromArgb("#F39C12");
            }
            else
            {
                CapacityProgressBar.ProgressColor = Color.FromArgb("#28A745");
            }
        }
    }
}