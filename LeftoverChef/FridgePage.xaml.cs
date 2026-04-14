// File: FridgePage.xaml.cs
// Fridge and Smart Match
// Match recipes from DB based on ingredients
// Includes undo feature wao

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace LeftoverChef
{
    public partial class FridgePage : ContentPage
    {
        // 撤销用的栈 (Undo stack)
        private Stack<List<Ingredient>> _undoStack = new Stack<List<Ingredient>>();

        // 当前分类：冷藏或冷冻 (Current category)
        private string _currentCategory = "";

        // 页面状态：0分类 1列表 (State: 0 categories, 1 list)
        private int _currentState = 0;

        public FridgePage() { InitializeComponent(); }

        // 智能匹配 (Smart Match Logic)
        private async void OnSmartMatchTapped(object sender, EventArgs e)
        {
            // 从数据库拿所有冰箱食材 (Get fridge items from DB)
            var allIngredients = await App.Database.GetIngredientsAsync();
            var fridgeItems = allIngredients.Select(i => i.Name.ToLower()).ToList();

            // 冰箱没东西就拦截 (Stop if empty)
            if (fridgeItems.Count == 0)
            {
                await this.DisplayAlertAsync("Empty", "Add some food to your fridge first!", "OK");
                return;
            }

            // 抓取所有菜谱 (Fetch all recipes)
            var allRecipes = await App.Database.GetRecipesAsync();

            // 核心匹配算法 (Core match algorithm)
            var matches = allRecipes
                .Select(recipe => new
                {
                    Data = recipe,
                    // 算算命中了几个食材 (Count matched ingredients)
                    Count = fridgeItems.Count(item => recipe.Ingredients != null && recipe.Ingredients.ToLower().Contains(item))
                })
                .Where(m => m.Count > 0) // 过滤掉没命中的 (Remove 0 matches)
                .OrderByDescending(m => m.Count) // 匹配多的排前面 (Sort by highest match)
                .Take(3) // 取前三名 (Top 3)
                .ToList();

            // 弹窗显示结果 (Show results)
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

        // 点分类进列表 (Tap category to open list)
        private async void OnCategoryTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is string category)
            {
                _currentCategory = category;
                ListTitleLabel.Text = category == "Fridge" ? "🥬 Fridge Items" : "❄️ Freezer Items";

                await RefreshListAsync(); // 刷新UI (Refresh UI)

                await TransitionViews(CategoryView, IngredientListGrid);

                _currentState = 1;
                DynamicBottomButton.Text = "⬅️ Back to Storage";
            }
        }

        // 添加新食材 (Add ingredient)
        private async void OnAddIngredientClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewIngredientEntry.Text))
            {
                Microsoft.Maui.Devices.Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
                return;
            }

            var newIng = new Ingredient { Name = NewIngredientEntry.Text, Category = _currentCategory };

            // 存入真实数据库 (Save to DB)
            await App.Database.SaveIngredientAsync(newIng);

            // 同步到内存防报错 (Sync to memory fallback)
            App.GlobalIngredients.Add(newIng);

            NewIngredientEntry.Text = string.Empty;
            await RefreshListAsync();
        }

        // 删除食材 (Delete ingredient)
        private async void OnDeleteIngredientClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Ingredient item)
            {
                _undoStack.Push(new List<Ingredient> { item });

                // 从数据库删 (Delete from DB)
                await App.Database.DeleteIngredientAsync(item);

                // 从内存删 (Delete from memory)
                var memItem = App.GlobalIngredients.FirstOrDefault(m => m.Id == item.Id || m.Name == item.Name);
                if (memItem != null) App.GlobalIngredients.Remove(memItem);

                await RefreshListAsync();
            }
        }

        // 清空当前分类 (Clear current category)
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

        // 撤销删除 (Undo delete)
        private async void OnUndoClicked(object sender, EventArgs e)
        {
            if (_undoStack.Count > 0)
            {
                foreach (var item in _undoStack.Pop())
                {
                    item.Id = 0; // 清空Id，让数据库当新数据存 (Reset ID to insert as new)
                    await App.Database.SaveIngredientAsync(item);
                    App.GlobalIngredients.Add(item);
                }
                await RefreshListAsync();
            }
            else await this.DisplayAlertAsync("Undo", "Nothing to restore", "OK");
        }

        // 底部返回按钮逻辑 (Bottom back button logic)
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

        // 渐变切页动画 (Fade transition)
        private async Task TransitionViews(VisualElement viewToHide, VisualElement viewToShow)
        {
            await viewToHide.FadeToAsync(0, 150);
            viewToHide.IsVisible = false;
            viewToShow.Opacity = 0;
            viewToShow.IsVisible = true;
            await viewToShow.FadeToAsync(1, 150);
        }

        // 从数据库捞数据刷新列表 (Refresh list from DB)
        private async Task RefreshListAsync()
        {
            var allIngredients = await App.Database.GetIngredientsAsync();
            var filtered = allIngredients.Where(i => i.Category == _currentCategory).ToList();
            BindableLayout.SetItemsSource(IngredientListContainer, filtered);
        }
    }
}