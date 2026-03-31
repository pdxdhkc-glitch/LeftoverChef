// File: StoragePage.xaml.cs
// Role: Recipe library browser and detailed view handler.
// Function: Dynamically generates recipe cards with category-specific color coding and shadows.
// Features: Multi-state navigation (Categories -> List -> Detail).using System.Linq;
using System.Threading.Tasks;

namespace LeftoverChef;

public partial class StoragePage : ContentPage
{
    // 存删掉的数据用来撤销 (Undo stack)
    private Stack<List<Recipe>> _undoStack = new Stack<List<Recipe>>();

    // 当前分类 (Current category)
    private string _currentCat = "";

    // 0:分类 1:列表 2:详情 3:搜索直达 (State tracker)
    private int _state = 0;

    // 默认主页入口 (Default constructor)
    public StoragePage() { InitializeComponent(); }

    // 专门接收搜索传来的菜 (Constructor for Search result)
    public StoragePage(Recipe searchedRecipe)
    {
        InitializeComponent();

        // 填数据 (Fill UI with data)
        DetailName.Text = searchedRecipe.Name;
        DetailTime.Text = "⏱️ " + searchedRecipe.CookingTime;
        DetailIngredients.Text = searchedRecipe.Ingredients;
        DetailInstructions.Text = searchedRecipe.Instructions;
        DetailDescription.Text = searchedRecipe.Description;

        // 隐藏列表，直露详情 (Show detail view directly)
        CategoryView.IsVisible = false;
        RecipeListGrid.IsVisible = false;
        DetailView.IsVisible = true;
        DetailView.Opacity = 1;

        // 标为3，处理返回逻辑 (Set state to 3 for back nav)
        _state = 3;
        DynamicBottomButton.Text = "⬅️ Back to Search";
    }

    private async void OnCategoryTapped(object sender, TappedEventArgs e)
    {
        // 判空 (Return if null)
        var param = e.Parameter?.ToString();
        if (string.IsNullOrEmpty(param)) return;

        _currentCat = param;
        ListTitleLabel.Text = _currentCat + " Recipes";

        // 刷新并切到列表 (Refresh and show list)
        RefreshList();
        await Transition(CategoryView, RecipeListGrid);

        _state = 1;
        DynamicBottomButton.Text = "⬅️ Categories";
    }

    // 重新设计动态卡片UI 
    private void RefreshList()
    {
        RecipeListContainer.Children.Clear();
        var list = App.GlobalRecipes.Where(r => r.Category == _currentCat).ToList();

        foreach (var r in list)
        {
            // 卡片外壳加阴影 (Card shell with shadow)
            var card = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                BackgroundColor = Color.FromRgba(255, 255, 255, 0.95), // 稍微不透明点更有质感
                StrokeThickness = 0, // 砍掉生硬边框 (Remove border line)
                Padding = new Thickness(0), // 清零内边距给彩条腾地方 (Reset padding)
                Margin = new Thickness(0, 6)
            };
            card.Shadow = new Shadow { Brush = Brush.Black, Opacity = 0.08f, Offset = new Point(0, 4), Radius = 8 };

            // 三列网格布局 (3-column layout)
            var grid = new Grid
            {
                ColumnDefinitions = {
                    new ColumnDefinition(GridLength.Auto), // 左侧彩条
                    new ColumnDefinition(GridLength.Star), // 中间文字
                    new ColumnDefinition(GridLength.Auto)  // 右侧按钮
                }
            };

            // 左侧菜系区分彩条 (Category color bar)
            var colorBar = new BoxView
            {
                WidthRequest = 6,
                // 中餐给橙色，西餐给深蓝色 (Orange for Chinese, Blue for Western)
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

            // 整个卡片绑点击进详情 (Tap card for details)
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) => {
                DetailName.Text = r.Name; DetailTime.Text = "⏱️ " + r.CookingTime;
                DetailIngredients.Text = r.Ingredients; DetailInstructions.Text = r.Instructions; DetailDescription.Text = r.Description;
                await Transition(RecipeListGrid, DetailView);
                _state = 2;
                DynamicBottomButton.Text = "⬅️ List";
            };
            card.GestureRecognizers.Add(tap);

            // 删除键 (Delete button)
            var delBtn = new Button { Text = "🗑️", TextColor = Colors.Red, BackgroundColor = Colors.Transparent, FontSize = 20, Margin = new Thickness(0, 0, 10, 0) };
            delBtn.Clicked += (s, e) => { _undoStack.Push(new List<Recipe> { r }); App.GlobalRecipes.Remove(r); RefreshList(); };

            // 把零件拼起来 (Assemble parts)
            grid.Add(colorBar, 0);
            grid.Add(textLayout, 1);
            grid.Add(delBtn, 2);
            card.Content = grid;

            RecipeListContainer.Children.Add(card);
        }
    }

    private void OnDeleteAllClicked(object sender, EventArgs e)
    {
        var current = App.GlobalRecipes.Where(r => r.Category == _currentCat).ToList();
        if (current.Count > 0)
        {
            _undoStack.Push(new List<Recipe>(current));
            foreach (var r in current) App.GlobalRecipes.Remove(r);
            RefreshList();
        }
    }

    private async void OnUndoClicked(object sender, EventArgs e)
    {
        // 栈里有东西才撤销 (Undo if stack has items)
        if (_undoStack.Count > 0)
        {
            foreach (var r in _undoStack.Pop()) App.GlobalRecipes.Add(r);
            RefreshList();
        }
        else await DisplayAlertAsync("Undo", "Nothing to restore", "OK");
    }

    private async void OnDynamicBottomClicked(object sender, EventArgs e)
    {
        // 按层级回退 (Handle back navigation by state)
        if (_state == 3)
        {
            // 搜出来的直接退回搜索页 (Pop back to search page)
            await Navigation.PopAsync();
        }
        else if (_state == 2)
        {
            await Transition(DetailView, RecipeListGrid);
            _state = 1;
            DynamicBottomButton.Text = "⬅️ Categories";
        }
        else if (_state == 1)
        {
            await Transition(RecipeListGrid, CategoryView);
            _state = 0;
            DynamicBottomButton.Text = "🏠 Home";
        }
        else
        {
            await Navigation.PopAsync();
        }
    }

    // 渐变切页动画 (Fade transition)
    private async Task Transition(VisualElement h, VisualElement s)
    {
        await h.FadeToAsync(0, 150);
        h.IsVisible = false;
        s.Opacity = 0;
        s.IsVisible = true;
        await s.FadeToAsync(1, 150);
    }
}