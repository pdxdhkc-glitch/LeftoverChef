// File: FridgePage.xaml.cs
// Role: Fridge inventory management and Smart Match logic.
// Function: Implements the core 'Smart Match' algorithm to suggest recipes based on available ingredients.
// Advanced: Features an Undo stack for ingredient deletion recovery.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace LeftoverChef
{
    public partial class FridgePage : ContentPage
    {
        // 用于撤销删除操作的栈 (Stack for undoing deletions)
        private Stack<List<Ingredient>> _undoStack = new Stack<List<Ingredient>>();

        // 记录当前选中的是冷藏还是冷冻 (Track currently selected category)
        private string _currentCategory = "";

        // 页面状态层级：0为主视图，1为列表视图 (Page state: 0 for categories, 1 for list view)
        private int _currentState = 0;

        public FridgePage() { InitializeComponent(); }

        //智能匹配 (Smart Match Logic)
        private async void OnSmartMatchTapped(object sender, EventArgs e)
        {
            // 1. 获取冰箱里所有食材的名字并转小写 (Get all names from fridge and convert to lowercase)
            var fridgeItems = App.GlobalIngredients.Select(i => i.Name.ToLower()).ToList();

            // 判空拦截 (Stop if fridge is empty)
            if (fridgeItems.Count == 0)
            {
                await DisplayAlert("Empty", "Add some food to your fridge first!", "OK");
                return;
            }
                
            // 2. Select (Search for matches in global recipes)
            var matches = App.GlobalRecipes
                .Select(recipe => new
                {
                    Data = recipe,
                    // 计算冰箱里的食材在菜谱配料表里命中了多少次 (Count how many fridge items exist in recipe ingredients)
                    Count = fridgeItems.Count(item => recipe.Ingredients.ToLower().Contains(item))
                })
                .Where(m => m.Count > 0) // 过滤掉完全不匹配的 (Keep only recipes with at least 1 match)
                .OrderByDescending(m => m.Count) // 按匹配命中数降序排列 (Sort by match count descending)
                .Take(3) // 匹配度最高的前三名 (Take top 3 results)
                .ToList();

            // 3. 弹窗展示比对结果 (Display results via alert)
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


        // 点击分类卡片进入对应列表 (Navigate to ingredient list based on category tapped)
        private async void OnCategoryTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is string category)
            {
                _currentCategory = category; // 保存当前分类 (Save active category)
                ListTitleLabel.Text = category == "Fridge" ? "🥬 Fridge Items" : "❄️ Freezer Items";

                RefreshList(); // 刷新 UI 列表 (Update UI list)

                // 执行页面切换动画 (Execute view transition animation)
                await TransitionViews(CategoryView, IngredientListGrid);

                _currentState = 1; // 标记进入了列表层级 (Mark state as list view)
                DynamicBottomButton.Text = "⬅️ Back to Storage";
            }
        }

        // Add new ingredient
        private void OnAddIngredientClicked(object sender, EventArgs e)
        {
            // 防空内容提交 (Prevent empty submissions)
            if (string.IsNullOrWhiteSpace(NewIngredientEntry.Text)) return;

            // 存入全局静态列表 (Save to global static list)
            App.GlobalIngredients.Add(new Ingredient { Name = NewIngredientEntry.Text, Category = _currentCategory });
            NewIngredientEntry.Text = string.Empty; // 清空输入框 (Clear entry field)
            RefreshList();
        }

        // 删除单个食材 (Delete a single ingredient)
        private void OnDeleteIngredientClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Ingredient item)
            {
                // 单删 (Push to undo stack as backup)
                _undoStack.Push(new List<Ingredient> { item });
                App.GlobalIngredients.Remove(item);
                RefreshList();
            }
        }

        // 一键清空当前分类下的所有食材 (Clear all ingredients in current category)
        private void OnDeleteAllClicked(object sender, EventArgs e)
        {
            var current = App.GlobalIngredients.Where(i => i.Category == _currentCategory).ToList();
            if (current.Count > 0)
            {
                // 批量备份到撤销栈 (Batch backup to undo stack)
                _undoStack.Push(new List<Ingredient>(current));
                foreach (var item in current) App.GlobalIngredients.Remove(item);
                RefreshList();
            }
        }

        // 上一步 (Undo the last deletion action)
        private async void OnUndoClicked(object sender, EventArgs e)
        {
            if (_undoStack.Count > 0)
            {
                // 撤销 (Pop data from stack and restore)
                foreach (var item in _undoStack.Pop()) App.GlobalIngredients.Add(item);
                RefreshList();
            }
            else await DisplayAlertAsync("Undo", "Nothing to restore", "OK");
        }

        // 底部动态按钮的路由控制 (Dynamic bottom button routing control)
        private async void OnDynamicBottomClicked(object sender, EventArgs e)
        {
            if (_currentState == 1)
            {
                // 如果在列表页，则退回到分类页 (If in list view, go back to category view)
                await TransitionViews(IngredientListGrid, CategoryView);
                _currentState = 0;
                DynamicBottomButton.Text = "🏠 Back to Home";
            }
            else
            {
                // 如果在分类页，直接退出当前页面 (If in category view, pop page from navigation stack)
                await Navigation.PopAsync();
            }
        }

        // 视图平滑过渡动画 (Smooth fade in/out animation for view transitions)
        private async Task TransitionViews(VisualElement viewToHide, VisualElement viewToShow)
        {
            await viewToHide.FadeToAsync(0, 200);
            viewToHide.IsVisible = false;
            viewToShow.Opacity = 0;
            viewToShow.IsVisible = true;
            await viewToShow.FadeToAsync(1, 200);
        }

        // 刷新绑定的列表数据 (Refresh the bound list data)
        private void RefreshList()
        {
            // 过滤出当前分类的食材并重新绑定 (Filter items by active category and rebind)
            var filtered = App.GlobalIngredients.Where(i => i.Category == _currentCategory).ToList();
            BindableLayout.SetItemsSource(IngredientListContainer, filtered);
        }
    }
}