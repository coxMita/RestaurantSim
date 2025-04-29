using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using ReactiveUI;
using RestaurantSimulator.ViewModels;

public class CookingStation : ReactiveObject
{public MainWindowViewModel ViewModelInstance { get; set; }
public void AdjustTiming(bool isBoosted)
{
    if (CurrentRecipe == null)
    {
        // Retinem pentru cand se va porni gătitul
        _pendingBoost = isBoosted;
        return;
    }

    // dacă e deja în gătit, aplicăm direct boost-ul curent
    if (isBoosted)
    {
        RecipeTimeRemaining /= 2;
        _currentStepRemainingTime /= 2;

        if (_adjustedSteps != null)
        {
            foreach (var step in _adjustedSteps)
                step.Duration /= 2;
        }
    }
    else
    {
        RecipeTimeRemaining *= 2;
        _currentStepRemainingTime *= 2;

        if (_adjustedSteps != null)
        {
            foreach (var step in _adjustedSteps)
                step.Duration *= 2;
        }
    }
}



private List<RecipeStep> _adjustedSteps;
private bool _pendingBoost;

    public string Name { get; }
    public bool IsBusy { get; private set; }

    public int CurrentStepIndex { get; private set; }

private double _recipeTimeRemaining;
public double RecipeTimeRemaining
{
    get => _recipeTimeRemaining;
    set => this.RaiseAndSetIfChanged(ref _recipeTimeRemaining, value);
}

private double _currentStepRemainingTime; // ⬅️ AICI O PUI

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
{   if (_pendingBoost && ViewModelInstance != null && ViewModelInstance.IsSpeedBoosted)
    {
        AdjustTiming(true);
    }

    await Dispatcher.UIThread.InvokeAsync(() =>
    {
        CurrentRecipe = recipe;
        IsBusy = true;
        Progress = 0;
        StepProgress = 0;
    });

_adjustedSteps = recipe.Steps.Select(s => new RecipeStep
{
    Step = s.Step,
Duration = _pendingBoost ? s.Duration / 2 : s.Duration

}).ToList();


int totalSteps = _adjustedSteps.Count;
int completedSteps = 0;
RecipeTimeRemaining = _adjustedSteps.Sum(s => s.Duration);


foreach (var step in _adjustedSteps)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            CurrentStepDescription = step.Step;
            StepProgress = 0;
        });

_currentStepRemainingTime = step.Duration;

while (_currentStepRemainingTime > 0)
        {
            await Task.Delay(1000);


           _currentStepRemainingTime -= 1;
RecipeTimeRemaining -= 1;
if (RecipeTimeRemaining <= 1.5)
    RecipeTimeRemaining = 0;
RecipeTimeRemaining = Math.Max(0, RecipeTimeRemaining);



           double overallProgress = (completedSteps + (step.Duration - _currentStepRemainingTime) / step.Duration) / totalSteps * 100;
double stepLocalProgress = (step.Duration - _currentStepRemainingTime) / step.Duration * 100;


            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Progress = overallProgress;
                StepProgress = stepLocalProgress;
                StepTimeRemaining = (int)_currentStepRemainingTime;
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
