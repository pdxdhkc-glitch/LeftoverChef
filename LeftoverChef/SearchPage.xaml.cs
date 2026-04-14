// File: SearchPage.xaml.cs
// Search engine connected to DB
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace LeftoverChef;

public partial class SearchPage : ContentPage
{
    public SearchPage()
    {
        InitializeComponent();
    }

    // 搜索按钮点击 (Search button click)
    private async void OnSearchClicked(object sender, EventArgs e)
    {
        // 去掉前后空格转小写 (Trim spaces and convert to lowercase)
        string query = SearchEntry.Text?.ToLower().Trim() ?? "";
        ResultsList.Children.Clear();

        // 判空拦截 (Prevent empty search)
        if (string.IsNullOrWhiteSpace(query))
        {
            await this.DisplayAlertAsync("Notice", "Please enter a keyword to search.", "OK");
            return;
        }

        // 抓取全部菜谱查库 (Fetch from DB)
        var allRecipes = await App.Database.GetRecipesAsync();

        // 模糊匹配菜名和配料 (Fuzzy match name or ingredients)
        var matches = allRecipes.Where(r =>
            (r.Name != null && r.Name.ToLower().Contains(query)) ||
            (r.Ingredients != null && r.Ingredients.ToLower().Contains(query))
        ).ToList();

        // 没搜到给提示 (Show empty state message)
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

        // 遍历生成搜索结果卡片 (Generate result cards)
        foreach (var r in matches)
        {
            // 卡片外壳 (Card shell)
            var card = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                BackgroundColor = Color.FromRgba(255, 255, 255, 0.95),
                StrokeThickness = 0, // 去掉边框 (Remove border)
                Padding = new Thickness(0), // 内边距清零给彩条腾地方 (Reset padding for color bar)
                Margin = new Thickness(0, 5)
            };

            // 左彩条，右文字 (Left color bar, right text)
            var grid = new Grid
            {
                ColumnDefinitions = {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star)
                }
            };

            // 菜系彩条 (Category color bar)
            var colorBar = new BoxView
            {
                WidthRequest = 6,
                BackgroundColor = r.Category == "Chinese" ? Color.FromArgb("#E65100") : Color.FromArgb("#283593")
            };

            // 文字排版 (Text layout)
            var textLayout = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(15, 10),
                Spacing = 2
            };
            textLayout.Children.Add(new Label { Text = r.Name, FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
            textLayout.Children.Add(new Label { Text = "⏱️ " + r.CookingTime + " | " + r.Category, FontSize = 13, TextColor = Color.FromArgb("#7F8C8D") });

            // 拼装卡片 (Assemble card)
            grid.Add(colorBar, 0);
            grid.Add(textLayout, 1);
            card.Content = grid;

            // 点击进详情 (Tap to see details)
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, args) =>
            {
                await Navigation.PushAsync(new StoragePage(r));
            };
            card.GestureRecognizers.Add(tap);

            ResultsList.Children.Add(card);
        }
    }

    // 🏠 返回主页 (Navigate back to home)
    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}