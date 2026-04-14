// File: MainPage.xaml.cs
// Main page & navigation
// Handles clicks and bounce animations
using System.Threading.Tasks; // 必须引用这个做动画 (Required for animations)

namespace LeftoverChef;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async Task AnimateClick(View view)
    {
        // 按下变小 (Shrink down)
        await view.ScaleToAsync(0.95, 100, Easing.CubicOut);

        // 弹回原样 (Bounce back)
        await view.ScaleToAsync(1.0, 150, Easing.BounceOut);
    }

    private async void OnFridgeClicked(object sender, EventArgs e)
    {
        // 先播动画 (Play animation first)
        if (sender is View view) await AnimateClick(view);

        // 播完再跳页 (Navigate after animation)
        await Navigation.PushAsync(new FridgePage());
    }

    private async void OnStorageClicked(object sender, EventArgs e)
    {
        if (sender is View view) await AnimateClick(view);
        await Navigation.PushAsync(new StoragePage());
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        if (sender is View view) await AnimateClick(view);
        await Navigation.PushAsync(new SearchPage());
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        if (sender is View view) await AnimateClick(view);
        await Navigation.PushAsync(new CreatePage());
    }

    private async void OnExitClicked(object sender, EventArgs e)
    {
        if (sender is View view) await AnimateClick(view); // 退出按钮动画 (Exit btn animation)

        // 弹窗确认退出 (Confirm exit)
        bool answer = await DisplayAlertAsync("Exit", "Are you sure you want to exit?", "Yes", "No");
        if (answer)
        {
            Application.Current?.Quit();
        }
    }
}