using System.Linq;
using System.Threading.Tasks;

namespace LeftoverChef;

public partial class StoragePage : ContentPage
{
    private Stack<List<Recipe>> _undoStack = new Stack<List<Recipe>>(); // 撤销栈
    private string _currentCat = ""; // 当前选择的分类
    private int _state = 0; // 0:分类页, 1:列表页, 2:详情页

    public StoragePage() { InitializeComponent(); }

    // 处理分类点击：跳转到列表
    private async void OnCategoryTapped(object sender, TappedEventArgs e)
    {
        var param = e.Parameter?.ToString();
        if (string.IsNullOrEmpty(param)) return;
        _currentCat = param;
        ListTitleLabel.Text = _currentCat + " Recipes";
        RefreshList();
        await Transition(CategoryView, RecipeListGrid);
        _state = 1; DynamicBottomButton.Text = "⬅️ Categories";
    }

    // 动态生成食谱卡片列表
    private void RefreshList()
    {
        RecipeListContainer.Children.Clear();
        var list = App.GlobalRecipes.Where(r => r.Category == _currentCat).ToList();
        foreach (var r in list)
        {
            var card = new Border { StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 }, BackgroundColor = Color.FromRgba(255, 255, 255, 0.8), Padding = 15, Margin = new Thickness(0, 5) };
            var grid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };

            var label = new Label { Text = "📖 " + r.Name, VerticalOptions = LayoutOptions.Center, FontSize = 18, TextColor = Colors.Black };
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) => {
                DetailName.Text = r.Name; DetailTime.Text = "⏱️ " + r.CookingTime;
                DetailIngredients.Text = r.Ingredients; DetailInstructions.Text = r.Instructions; DetailDescription.Text = r.Description;
                await Transition(RecipeListGrid, DetailView); _state = 2; DynamicBottomButton.Text = "⬅️ List";
            };
            label.GestureRecognizers.Add(tap);

            var delBtn = new Button { Text = "🗑️", TextColor = Colors.Red, BackgroundColor = Colors.Transparent, FontSize = 20 };
            delBtn.Clicked += (s, e) => { _undoStack.Push(new List<Recipe> { r }); App.GlobalRecipes.Remove(r); RefreshList(); };

            grid.Add(label, 0); grid.Add(delBtn, 1);
            card.Content = grid;
            RecipeListContainer.Children.Add(card);
        }
    }

    // 全删逻辑
    private void OnDeleteAllClicked(object sender, EventArgs e)
    {
        var current = App.GlobalRecipes.Where(r => r.Category == _currentCat).ToList();
        if (current.Count > 0) { _undoStack.Push(new List<Recipe>(current)); foreach (var r in current) App.GlobalRecipes.Remove(r); RefreshList(); }
    }

    // 撤销逻辑
    private async void OnUndoClicked(object sender, EventArgs e)
    {
        if (_undoStack.Count > 0) { foreach (var r in _undoStack.Pop()) App.GlobalRecipes.Add(r); RefreshList(); }
        else await DisplayAlertAsync("Undo", "Nothing to restore", "OK");
    }

    // 动态返回按钮：根据当前状态回退视图
    private async void OnDynamicBottomClicked(object sender, EventArgs e)
    {
        if (_state == 2) { await Transition(DetailView, RecipeListGrid); _state = 1; DynamicBottomButton.Text = "⬅️ Categories"; }
        else if (_state == 1) { await Transition(RecipeListGrid, CategoryView); _state = 0; DynamicBottomButton.Text = "🏠 Home"; }
        else await Navigation.PopAsync();
    }

    // 视图切换动画方法
    private async Task Transition(VisualElement h, VisualElement s) { await h.FadeToAsync(0, 150); h.IsVisible = false; s.Opacity = 0; s.IsVisible = true; await s.FadeToAsync(1, 150); }
}