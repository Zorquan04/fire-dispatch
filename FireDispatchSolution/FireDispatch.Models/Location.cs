namespace FireDispatch.Models;

// Reprezentuje położenie w stopniach dziesiętnych (WGS-84)
public record Location(double Latitude, double Longitude)
{
// Prosta metoda na obliczenie przybliżonej odległości euklidesowej w stopniach
    public double DistanceTo(Location other)
    {
        var dLat = Latitude - other.Latitude;
        var dLon = Longitude - other.Longitude;
        return Math.Sqrt(dLat * dLat + dLon * dLon);
    }
}