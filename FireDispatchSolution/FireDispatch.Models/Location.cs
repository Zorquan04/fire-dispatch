namespace FireDispatch.Models;

// Represents the position in decimal degrees (WGS-84)
public record Location(double Latitude, double Longitude)
{
// A simple method to calculate approximate Euclidean distance in degrees
    public double DistanceTo(Location other)
    {
        var dLat = Latitude - other.Latitude;
        var dLon = Longitude - other.Longitude;
        return Math.Sqrt(dLat * dLat + dLon * dLon);
    }
}