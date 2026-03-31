using System.Linq;
namespace LeftoverChef;

public partial class SearchPage : ContentPage
{
    public SearchPage() { InitializeComponent(); }

    private void OnSearchClicked(object sender, EventArgs e)
    {
        // 搜之前清屏 (Clear previous results)
        ResultsList.Children.Clear();

        // 拿关键字防null (Get keyword, handle null)
        var kw = SearchEntry.Text?.ToLower() ?? "";

        // 空的就不搜了 (Return if empty)
        if (string.IsNullOrEmpty(kw)) return;

        // 找匹配 (Filter matching recipes)
        var res = App.GlobalRecipes.Where(r => r.Name.ToLower().Contains(kw)).ToList();

        foreach (var r in res)
        {
            //卡片外壳加阴影 (Card shell with shadow)
            var card = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                BackgroundColor = Color.FromRgba(255, 255, 255, 0.95),
                StrokeThickness = 0,
                Padding = new Thickness(0),
                Margin = new Thickness(0, 6)
            };
            card.Shadow = new Shadow { Brush = Brush.Black, Opacity = 0.08f, Offset = new Point(0, 4), Radius = 8 };

            // 三列布局 (3-column layout)
            var grid = new Grid
            {
                ColumnDefinitions = {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                }
            };

            // 左侧菜系彩条 (Category color bar)
            var colorBar = new BoxView
            {
                WidthRequest = 6,
                BackgroundColor = r.Category == "Chinese" ? Color.FromArgb("#E65100") : Color.FromArgb("#283593")
            };

            // 中间文字排版 (Text layout)
            var textLayout = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(15, 10),
                Spacing = 2
            };
            var nameLabel = new Label { Text = r.Name, FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black };
            var timeLabel = new Label { Text = "⏱️ " + r.CookingTime, FontSize = 13, TextColor = Color.FromArgb("#7F8C8D") };
            textLayout.Children.Add(nameLabel);
            textLayout.Children.Add(timeLabel);

            // 右侧指引箭头 (Right arrow icon)
            var arrowLabel = new Label { Text = "➡️", VerticalOptions = LayoutOptions.Center, FontSize = 18, Margin = new Thickness(0, 0, 15, 0) };

            // 绑点击事件 (Add tap event)
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, args) => {
                // 传数据，直达详情 (Navigate to details with data)
                await Navigation.PushAsync(new StoragePage(r));
            };
            card.GestureRecognizers.Add(tap);

            // 把零件拼起来 (Assemble parts)
            grid.Add(colorBar, 0);
            grid.Add(textLayout, 1);
            grid.Add(arrowLabel, 2);
            card.Content = grid;

            // 卡片加到列表 (Add card to list)
            ResultsList.Children.Add(card);
        }
    }

    // 退回主页 (Pop to Home)
    private async void OnHomeClicked(object sender, EventArgs e) => await Navigation.PopAsync();
}