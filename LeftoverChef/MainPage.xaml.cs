// File: MainPage.xaml.cs
// Role: Main entry point and navigation controller.
// Function: Handles page transitions and implements the 'ScaleToAsync' bounce animation for a tactile UI experience.
using System.Threading.Tasks; // 确保顶部有这个引用

namespace LeftoverChef;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async Task AnimateClick(View view)
    {
        // 第一阶段
        await view.ScaleTo(0.95, 100, Easing.CubicOut);

        // 第二阶段
        await view.ScaleTo(1.0, 150, Easing.BounceOut);
    }

    private async void OnFridgeClicked(object sender, EventArgs e)
    {
        // 触发动画 (Trigger animation)
        if (sender is View view) await AnimateClick(view);

        // 动画执行完后，再进行原本的页面跳转
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
        if (sender is View view) await AnimateClick(view); // 退出按钮动画

        bool answer = await DisplayAlertAsync("Exit", "Are you sure you want to exit?", "Yes", "No");
        if (answer)
        {
            Application.Current?.Quit();
        }
    }
}
