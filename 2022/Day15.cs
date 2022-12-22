namespace AdventOfCode;

public class Day15
{
    const int rowOfInterest = 2000000;

    public async Task RunAsync()
    {
        var input = await File.ReadAllLinesAsync("Day15Input.txt");
        var pairs = input.Select(x => Parse(x)).ToList();

        var scanned = new Dictionary<int, Dictionary<int, bool>>(); // key = y, key = x, value = isBeacon
        foreach (var (sensor, beacon) in pairs)
        {
            var dx = Math.Abs(sensor.X - beacon.X);
            var dy = Math.Abs(sensor.Y - beacon.Y);

            var reach = dx + dy;

            for (var x = -reach; x < reach; x++)
            {
                var y = Math.Abs(x) - reach;
                var maxY = Math.Abs(y);

                if (sensor.Y + y > rowOfInterest || sensor.Y + maxY < rowOfInterest)
                    continue;
                
                var xPos = sensor.X + x;

                var isBeaconPos = xPos == beacon.X && rowOfInterest == beacon.Y;

                if (!scanned.TryGetValue(rowOfInterest, out var xs))
                    scanned[rowOfInterest] = new Dictionary<int, bool>();
                if (!scanned[rowOfInterest].TryGetValue(xPos, out var isBeacon) || !isBeacon)
                    scanned[rowOfInterest][xPos] = isBeaconPos;
                /*for (; y <= maxY; y++)
                {
                    var xPos = sensor.X + x;
                    var yPos = sensor.Y + y;

                    var isBeaconPos = xPos == beacon.X && yPos == beacon.Y;

                    if (!scanned.TryGetValue(yPos, out var xs))
                        scanned[yPos] = new Dictionary<int, bool>();
                    if (!scanned[yPos].TryGetValue(xPos, out var isBeacon) || !isBeacon)
                        scanned[yPos][xPos] = isBeaconPos;
                }*/
            }
        }

        const int max = 4000000;
        var notScanned = new HashSet<Position>();
        for (var x = 0; x <= max; x++)
            for (var y = 0; y <= max; y++)
                notScanned.Add(new Position(x, y));
        foreach (var (sensor, beacon) in pairs)
        {
            var dx = Math.Abs(sensor.X - beacon.X);
            var dy = Math.Abs(sensor.Y - beacon.Y);

            var reach = dx + dy;

            for (var x = Math.Max(-reach, 0); x < Math.Min(reach, max); x++)
            {
                var y = Math.Max(Math.Abs(x) - reach, 0);
                var maxY = Math.Min(Math.Abs(y), 0);
                for (; y <= maxY; y++)
                {
                    var xPos = sensor.X + x;
                    var yPos = sensor.Y + y;

                    notScanned.Remove(new Position(xPos, yPos));
                }
            }
        }

        var scannedNonBeacons = scanned[rowOfInterest].Values.Where(isBeacon => !isBeacon).Count();
        Console.WriteLine(scannedNonBeacons);

        var targetBeacon = notScanned.Single();
    }

    (Sensor, Beacon) Parse(string input)
    {
        var split = input.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
        var sensorTokens = split[0].Split("at").Last();
        var beaconTokens = split[1].Split("at").Last();

        var sensorCoords = ParseCoords(sensorTokens);
        var beaconCoords = ParseCoords(beaconTokens);

        return (new Sensor(sensorCoords.Item1, sensorCoords.Item2), new Beacon(beaconCoords.Item1, beaconCoords.Item2));
    }

    (int, int) ParseCoords(string input)
    {
        var split = input.Split(',', 2, StringSplitOptions.RemoveEmptyEntries);
        var x = split[0].Split('=').Last();
        var y = split[1].Split('=').Last();

        return (int.Parse(x), int.Parse(y));
    }

    abstract record Element(int X, int Y);
    record Sensor(int X, int Y) : Element(X, Y);
    record Beacon(int X, int Y) : Element(X, Y);
    record Position(int X, int Y) : Element(X, Y);
}