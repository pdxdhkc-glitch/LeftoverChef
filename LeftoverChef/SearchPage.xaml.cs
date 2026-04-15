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

    // 搜索逻辑 (Search logic)
    private async void OnSearchClicked(object sender, EventArgs e)
    {
        // 格式化输入 (Normalize input)
        string query = SearchEntry.Text?.ToLower().Trim() ?? "";
        ResultsList.Children.Clear();

        // 空输入检查 (Empty check)
        if (string.IsNullOrWhiteSpace(query))
        {
            // 震动保护 (Vibration fallback)
            try { Microsoft.Maui.Devices.Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200)); } catch { }

            await this.DisplayAlertAsync("Notice", "Please enter a keyword to search.", "OK");
            return;
        }

        // 取数据 (Fetch from DB)
        var allRecipes = await App.Database.GetRecipesAsync();

        // 模糊搜索 (Fuzzy match)
        var matches = allRecipes.Where(r =>
            (r.Name != null && r.Name.ToLower().Contains(query)) ||
            (r.Ingredients != null && r.Ingredients.ToLower().Contains(query))
        ).ToList();

        // 无结果处理 (Empty state)
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

        // 渲染列表 (Render cards)
        foreach (var r in matches)
        {
            // 外边框 (Card container)
            var card = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                BackgroundColor = Color.FromRgba(255, 255, 255, 0.95),
                StrokeThickness = 0,
                Padding = new Thickness(0),
                Margin = new Thickness(0, 5)
            };

            // 布局分三列 (Grid layout)
            var grid = new Grid
            {
                ColumnDefinitions = {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                }
            };

            // 左侧彩条 (Category bar)
            var colorBar = new BoxView
            {
                WidthRequest = 6,
                BackgroundColor = r.Category == "Chinese" ? Color.FromArgb("#E65100") : Color.FromArgb("#283593")
            };

            // 文字信息 (Text info)
            var textLayout = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(15, 10),
                Spacing = 2
            };
            textLayout.Children.Add(new Label { Text = r.Name, FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
            textLayout.Children.Add(new Label { Text = "⏱️ " + r.CookingTime + " | " + r.Category, FontSize = 13, TextColor = Color.FromArgb("#7F8C8D") });

            //检查库存 (Inventory check button)
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

            // 比对逻辑 (Comparison logic)
            checkBtn.Clicked += async (s, args) =>
            {
                // 1. 获取库存 (Get fridge items)
                var myIngredients = await App.Database.GetIngredientsAsync();
                var myInvNames = myIngredients.Select(i => i.Name.ToLower().Trim()).ToList();

                // 2. 解析菜谱配料 (Parse required items)
                var requiredItems = r.Ingredients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(i => i.Trim().ToLower())
                                                 .ToList();

                var missingItems = new List<string>();

                // 3. 找缺漏 (Identify missing items)
                foreach (var req in requiredItems)
                {
                    // 包含匹配 (Contains check)
                    if (!myInvNames.Any(inv => inv.Contains(req) || req.Contains(inv)))
                    {
                        missingItems.Add(req);
                    }
                }

                // 4. 结果反馈 (Show results)
                if (missingItems.Count == 0)
                {
                    await DisplayAlertAsync("Great!", "You have all the ingredients in your fridge!", "Awesome");
                }
                else
                {
                    // 格式化清单 (Format list)
                    var missingText = string.Join("\n- ", missingItems.Select(m => char.ToUpper(m[0]) + m.Substring(1)));
                    await DisplayAlertAsync("Missing Ingredients", $"You need to buy:\n- {missingText}", "Got it");
                }
            };

            // 组装 UI (Assemble card)
            grid.Add(colorBar, 0);
            grid.Add(textLayout, 1);
            grid.Add(checkBtn, 2);
            card.Content = grid;

            // 详情跳转 (Go to details)
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, args) =>
            {
                await Navigation.PushAsync(new StoragePage(r));
            };
            // 绑定文字点击 (Bind text click)
            textLayout.GestureRecognizers.Add(tap);

            ResultsList.Children.Add(card);
        }
    }

    // 回主页 (Back to home)
    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}