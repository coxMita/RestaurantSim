using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using RestaurantSimulator.ViewModels;

namespace RestaurantSimulator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private void OnStartSimulation(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            // Generează random clienți folosind rețetele disponibile
            vm.GenerateRandomClients(vm.AvailableRecipes.Select(x => x.Recipe).ToList());
        }
    }




}