namespace RestaurantSimulator.ViewModels;

using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;

public class MainWindowViewModel : ReactiveObject
{
    public ObservableCollection<CookingStation> Stations { get; } = new();
    public ObservableCollection<RecipeItemViewModel> AvailableRecipes { get; } = new();
    private StationManager _stationManager;
    private DataLoader _dataLoader;
    public ICommand CookRecipeCommand { get; }
    public ObservableCollection<ClientTable> ClientTables { get; } = new();
    private Random _random = new Random();
    public ObservableCollection<string> RecipeHistory { get; } = new();
public ICommand StopSimulationCommand { get; }


public ICommand ToggleSpeedCommand { get; }
private bool _isSpeedBoosted;
public bool IsSpeedBoosted
{
    get => _isSpeedBoosted;
    set
    {
        _isSpeedBoosted = value;
        this.RaisePropertyChanged(nameof(SpeedButtonText));
    }
}
public string SpeedButtonText => IsSpeedBoosted ? "Normal Speed" : "Speed x2";

    public MainWindowViewModel()
    {
        _dataLoader = new DataLoader();
        _dataLoader.LoadData("ExerciseJSON.json");

        _stationManager = new StationManager(3);

        for (int i = 0; i < 4; i++)
        {
            ClientTables.Add(new ClientTable
            {
                RequestedRecipe = "Waiting...",
                IsWaiting = false,
                IsServed = false
            });
        }



        CookRecipeCommand = new RelayCommand<Recipe>(async (recipe) =>
{
    var station = _stationManager.GetAvailableStation();
    if (station != null)
    {
        Console.WriteLine($"Starting cooking {recipe.Name} on {station.Name}");
        await station.StartCookingAsync(recipe);
        await ServeClient(recipe);



    }
});
StopSimulationCommand = new RelayCommand(() => StopSimulation());
ToggleSpeedCommand = new RelayCommand(() => ToggleSpeed());


        foreach (var recipe in _dataLoader.Recipes)
        {
            AvailableRecipes.Add(new RecipeItemViewModel(recipe, CookRecipeCommand)); ;
        }

        foreach (var station in _stationManager.GetAllStations())
        {
            station.ProgressChanged += OnStationProgressChanged;
            Stations.Add(station);
        }




    }

    private void OnStationProgressChanged(CookingStation station)
    {
        this.RaisePropertyChanged(nameof(Stations));
    }

    public async void StartSimulation()
    {

    }

    public void GenerateRandomClients(List<Recipe> recipes)
    {
        foreach (var table in ClientTables)
        {
            if (!table.IsWaiting && !table.IsServed)
            {
                var randomRecipe = recipes[_random.Next(recipes.Count)];
                table.RequestedRecipe = $"I want {randomRecipe.Name}";
                table.IsWaiting = true;
            }
        }
    }


    public async Task ServeClient(Recipe recipe)
    {
        var servedTable = ClientTables.FirstOrDefault(table => table.RequestedRecipe.Contains(recipe.Name));

        if (servedTable != null)
        {
            // 1. Update: the order has been served
            servedTable.RequestedRecipe = "The order has been served!";
            RecipeHistory.Add(recipe.Name);
            servedTable.IsWaiting = false;
            servedTable.IsServed = true;

            // 2. Așteptăm 2 secunde
            await Task.Delay(2000);

            servedTable.RequestedRecipe = "Waiting...";
            servedTable.IsServed = false;

            await Task.Delay(5000);

            var randomRecipe = _random.Next(AvailableRecipes.Count);
            servedTable.RequestedRecipe = $"I want {AvailableRecipes[randomRecipe].Recipe.Name}";
            servedTable.IsWaiting = true;


        }
    }
public void StopSimulation()
{
    // 1. Golește stațiile
    Stations.Clear();

    // 2. Golește mesele
    ClientTables.Clear();

    // 3. Golește istoricul de rețete
    RecipeHistory.Clear();

    // 4. Reîncarcă stațiile
    _stationManager = new StationManager(3);
    foreach (var station in _stationManager.GetAllStations())
    {
        station.ProgressChanged += OnStationProgressChanged;
        Stations.Add(station);
    }

    // 5. Reîncarcă mesele
    for (int i = 0; i < 4; i++)
    {
        ClientTables.Add(new ClientTable
        {
            RequestedRecipe = "Waiting...",
            IsWaiting = false,
            IsServed = false
        });
    }

    Console.WriteLine("Simulation stopped and reset!");
}

public void ToggleSpeed()
{
    IsSpeedBoosted = !IsSpeedBoosted;
    Console.WriteLine($"Speed boost toggled: {(IsSpeedBoosted ? "2x Speed" : "Normal Speed")}");
foreach (var station in _stationManager.GetAllStations())
{
    station.ViewModelInstance = this;
    station.AdjustTiming(IsSpeedBoosted);
    station.ProgressChanged += OnStationProgressChanged;
}



}





}

