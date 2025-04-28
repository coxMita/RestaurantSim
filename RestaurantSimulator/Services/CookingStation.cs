using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using ReactiveUI;

public class CookingStation : ReactiveObject
{
    public string Name { get; }
    public bool IsBusy { get; private set; }

    public int CurrentStepIndex { get; private set; }

private int _recipeTimeRemaining;
public int RecipeTimeRemaining
{
    get => _recipeTimeRemaining;
    set => this.RaiseAndSetIfChanged(ref _recipeTimeRemaining, value);
}

private int _stepTimeRemaining;
public int StepTimeRemaining
{
    get => _stepTimeRemaining;
    set => this.RaiseAndSetIfChanged(ref _stepTimeRemaining, value);
}

    private double _progress;
    public double Progress
    {
        get => _progress;
        set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    private Recipe _currentRecipe;
    public Recipe CurrentRecipe
    {
        get => _currentRecipe;
        set => this.RaiseAndSetIfChanged(ref _currentRecipe, value);
    }

    private double _stepProgress;
    public double StepProgress
    {
        get => _stepProgress;
        set => this.RaiseAndSetIfChanged(ref _stepProgress, value);
    }

    private string _currentStepDescription;
    public string CurrentStepDescription
    {
        get => _currentStepDescription;
        set => this.RaiseAndSetIfChanged(ref _currentStepDescription, value);
    }
    public bool IsBoosted { get; private set; }

    public event Action<CookingStation> ProgressChanged;

    public CookingStation(string name)
    {
        Name = name;
        IsBusy = false;
    }

    public async Task StartCookingAsync(Recipe recipe)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
    {
        CurrentRecipe = recipe;
        IsBusy = true;
        Progress = 0;
          StepProgress = 0;
            // Calculează timpul total al rețetei
    RecipeTimeRemaining = recipe.Steps.Sum(s => s.Duration);
    });

        int totalSteps = recipe.Steps.Count;
        int completedSteps = 0;

        foreach (var step in recipe.Steps)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                CurrentStepDescription = step.Step;
                StepProgress = 0;
            });

        int remainingStepTime = step.Duration;

while (remainingStepTime > 0)
{
    await Task.Delay(IsBoosted ? 500 : 1000);

    remainingStepTime--;
    RecipeTimeRemaining--;

    double overallProgress = (completedSteps + (double)(step.Duration - remainingStepTime) / step.Duration) / totalSteps * 100;
    double stepLocalProgress = (double)(step.Duration - remainingStepTime) / step.Duration * 100;

    await Dispatcher.UIThread.InvokeAsync(() =>
    {
        Progress = overallProgress;
        StepProgress = stepLocalProgress;
        StepTimeRemaining = remainingStepTime;
    });
}

            completedSteps++;
        }

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            Progress = 0;
            StepProgress = 0;
            CurrentStepDescription = "Done";
            IsBusy = false;
            CurrentRecipe = null;

        });
    }
}
