// SearchPage.xaml.cs
using System.Linq;
namespace LeftoverChef;

public partial class SearchPage : ContentPage
{
    public SearchPage() { InitializeComponent(); }
    private void OnSearchClicked(object sender, EventArgs e)
    {
        ResultsList.Children.Clear();
        var kw = SearchEntry.Text?.ToLower() ?? "";
        if (string.IsNullOrEmpty(kw)) return;
        var res = App.GlobalRecipes.Where(r => r.Name.ToLower().Contains(kw)).ToList();
        foreach (var r in res)
        {
            ResultsList.Children.Add(new Border { BackgroundColor = Color.FromRgba(255, 255, 255, 0.8), Padding = 15, Content = new Label { Text = "📖 " + r.Name, TextColor = Colors.Black, FontSize = 18 } });
        }
    }
    private async void OnHomeClicked(object sender, EventArgs e) => await Navigation.PopAsync();
}