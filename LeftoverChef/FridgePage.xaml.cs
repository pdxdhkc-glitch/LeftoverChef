// File: FridgePage.xaml.cs
// Role: Fridge inventory management and Smart Match logic.
// Function: Implements the core 'Smart Match' algorithm to suggest recipes based on available ingredients.
// Advanced: Features an Undo stack for ingredient deletion recovery.using System;
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

        public FridgePage() { InitializeComponent(); }

        //智能匹配逻辑 (Smart Match Logic)
        private async void OnSmartMatchTapped(object sender, EventArgs e)
        {
            // 1. 获取冰箱里所有食材的名字 (Get all names from fridge)
            var fridgeItems = App.GlobalIngredients.Select(i => i.Name.ToLower()).ToList();

            if (fridgeItems.Count == 0)
            {
                await DisplayAlert("Empty", "Add some food to your fridge first!", "OK");
                return;
            }

            // 2. 在菜谱库中寻找匹配 (Search for matches in global recipes)
            var matches = App.GlobalRecipes
                .Select(recipe => new
                {
                    Data = recipe,
                    // 计算有多少个冰箱食材出现在了菜谱的 Ingredients 字符串里
                    Count = fridgeItems.Count(item => recipe.Ingredients.ToLower().Contains(item))
                })
                .Where(m => m.Count > 0) // 至少匹配到一个
                .OrderByDescending(m => m.Count) // 按匹配数量从高到低排
                .Take(3) // 只取前三名
                .ToList();

            // 3. 展示结果 (Display results)
            if (matches.Count == 0)
            {
                await DisplayAlert("No Match", "Try adding different ingredients!", "OK");
            }
            else
            {
                string resultText = "Based on your fridge, we suggest:\n\n";
                foreach (var m in matches)
                {
                    resultText += $"⭐ {m.Data.Name} ({m.Count} items matched)\n";
                }
                await DisplayAlert("Smart Match Results", resultText, "Got it!");
            }
        }

        // --- 以下是原有的逻辑，保持不变 ---

        private async void OnCategoryTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is string category)
            {
                _currentCategory = category;
                ListTitleLabel.Text = category == "Fridge" ? "🥬 Fridge Items" : "❄️ Freezer Items";
                RefreshList();
                await TransitionViews(CategoryView, IngredientListGrid);
                _currentState = 1;
                DynamicBottomButton.Text = "⬅️ Back to Storage";
            }
        }

        private void OnAddIngredientClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewIngredientEntry.Text)) return;
            App.GlobalIngredients.Add(new Ingredient { Name = NewIngredientEntry.Text, Category = _currentCategory });
            NewIngredientEntry.Text = string.Empty;
            RefreshList();
        }

        private void OnDeleteIngredientClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Ingredient item)
            {
                _undoStack.Push(new List<Ingredient> { item });
                App.GlobalIngredients.Remove(item);
                RefreshList();
            }
        }

        private void OnDeleteAllClicked(object sender, EventArgs e)
        {
            var current = App.GlobalIngredients.Where(i => i.Category == _currentCategory).ToList();
            if (current.Count > 0)
            {
                _undoStack.Push(new List<Ingredient>(current));
                foreach (var item in current) App.GlobalIngredients.Remove(item);
                RefreshList();
            }
        }

        private async void OnUndoClicked(object sender, EventArgs e)
        {
            if (_undoStack.Count > 0)
            {
                foreach (var item in _undoStack.Pop()) App.GlobalIngredients.Add(item);
                RefreshList();
            }
            else await DisplayAlertAsync("Undo", "Nothing to restore", "OK");
        }

        private async void OnDynamicBottomClicked(object sender, EventArgs e)
        {
            if (_currentState == 1)
            {
                await TransitionViews(IngredientListGrid, CategoryView);
                _currentState = 0;
                DynamicBottomButton.Text = "🏠 Back to Home";
            }
            else { await Navigation.PopAsync(); }
        }

        private async Task TransitionViews(VisualElement viewToHide, VisualElement viewToShow)
        {
            await viewToHide.FadeToAsync(0, 200);
            viewToHide.IsVisible = false;
            viewToShow.Opacity = 0;
            viewToShow.IsVisible = true;
            await viewToShow.FadeToAsync(1, 200);
        }

        private void RefreshList()
        {
            var filtered = App.GlobalIngredients.Where(i => i.Category == _currentCategory).ToList();
            BindableLayout.SetItemsSource(IngredientListContainer, filtered);
        }
    }
}