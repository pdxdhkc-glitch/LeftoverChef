using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace LeftoverChef
{
    public partial class FridgePage : ContentPage
    {
        private string _currentCategory = "";
        private int _currentState = 0;

        public FridgePage() { InitializeComponent(); }

        private async void OnCategoryTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is string category)
            {
                _currentCategory = category;
                ListTitleLabel.Text = category == "Fridge" ? "🥬 Fridge Items" : "❄️ Freezer Items";
                RefreshList();
                await TransitionViews(CategoryView, IngredientListGrid);
                _currentState = 1;
                DynamicBottomButton.Text = "⬅️ Back to Storage";
            }
        }

        private void OnAddIngredientClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewIngredientEntry.Text)) return;
            App.GlobalIngredients.Add(new Ingredient { Name = NewIngredientEntry.Text, Category = _currentCategory });
            NewIngredientEntry.Text = string.Empty;
            RefreshList();
        }

        private void OnDeleteIngredientClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Ingredient item)
            {
                App.GlobalIngredients.Remove(item);
                RefreshList();
            }
        }

        private async void OnDynamicBottomClicked(object sender, EventArgs e)
        {
            if (_currentState == 1)
            {
                await TransitionViews(IngredientListGrid, CategoryView);
                _currentState = 0;
                DynamicBottomButton.Text = "🏠 Back to Home";
            }
            else { await Navigation.PopAsync(); }
        }

        private async Task TransitionViews(Grid viewToHide, Grid viewToShow)
        {
            await viewToHide.FadeToAsync(0, 200);
            viewToHide.IsVisible = false;
            viewToShow.Opacity = 0;
            viewToShow.IsVisible = true;
            await viewToShow.FadeToAsync(1, 200);
        }

        private void RefreshList()
        {
            var filtered = App.GlobalIngredients.Where(i => i.Category == _currentCategory).ToList();
            BindableLayout.SetItemsSource(IngredientListContainer, filtered);
        }
    }
}