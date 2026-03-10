namespace LeftoverChef;

// Main navigation logic
// 主页导航逻辑
public partial class MainPage : ContentPage
{
    // Initialize page
    // 初始化页面
    public MainPage() { InitializeComponent(); }

    // Go to Search
    // 跳转至搜索
    private async void OnSearchClicked(object sender, EventArgs e) { await Navigation.PushAsync(new SearchPage()); }

    // Go to Storage
    // 跳转至仓库
    private async void OnStorageClicked(object sender, EventArgs e) { await Navigation.PushAsync(new StoragePage()); }

    // Go to Create
    // 跳转至创建
    private async void OnCreateClicked(object sender, EventArgs e) { await Navigation.PushAsync(new CreatePage()); }

    // Exit App
    // 退出程序
    private void OnExitClicked(object sender, EventArgs e) { Application.Current.Quit(); }
}