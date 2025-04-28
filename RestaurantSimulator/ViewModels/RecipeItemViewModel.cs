using System.Windows.Input;
namespace RestaurantSimulator.ViewModels;
public class RecipeItemViewModel
{
    public Recipe Recipe { get; }
    public ICommand CookRecipeCommand { get; }

    public RecipeItemViewModel(Recipe recipe, ICommand cookRecipeCommand)
    {
        Recipe = recipe;
        CookRecipeCommand = cookRecipeCommand;
    }
}
