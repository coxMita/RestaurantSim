using System.Collections.Generic;
using System.Linq;

public class StationManager
{
    private List<CookingStation> _stations;

    public StationManager(int stationCount)
    {
        _stations = new List<CookingStation>();
        for (int i = 0; i < stationCount; i++)
        {
            _stations.Add(new CookingStation($"Station {i + 1}"));
        }
    }

    public CookingStation GetAvailableStation()
    {
        return _stations.FirstOrDefault(s => !s.IsBusy);
    }

    public List<CookingStation> GetAllStations()
    {
        return _stations;
    }
}
