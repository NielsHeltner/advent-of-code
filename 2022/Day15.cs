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

        var circles = pairs.Select(x => 
        {
            var sensor = x.Item1;
            var beacon = x.Item2;

            var dx = Math.Abs(sensor.X - beacon.X);
            var dy = Math.Abs(sensor.Y - beacon.Y);

            var radius = dx + dy;

            return new Circle(sensor.X, sensor.Y, radius);
        }).ToList();

        var candidates = new HashSet<Position>();

        foreach (var circle in circles)
        {
            foreach (var circle2 in circles)
            {
                if (circle == circle2)
                    continue;
                
                var dx = Math.Abs(circle.X - circle2.X);
                var xSign = Math.Sign(dx);
                var dy = Math.Abs(circle.Y - circle2.Y);
                var distance = dx + dy;

                var distanceForGap = circle.R + circle2.R + 2;
                if (distance == distanceForGap)
                {
                    var startX = Math.Max(circle.X - circle.R, circle2.X - circle2.R);
                    var endX = Math.Min(circle.X + circle.R, circle2.X + circle2.R);

                    var startY = Math.Max(circle.Y - circle.R, circle2.Y - circle2.R);
                    var endY = Math.Min(circle.Y + circle.R, circle2.Y + circle2.R);
                    
                    for (var y = startY; y < endY; y++)
                    {
                        var distCenter = Math.Abs(circle.Y - y);
                        var outsideParameter = circle.R + 1 - distCenter;

                        var x = circle.X + (outsideParameter * xSign);
                        if (x >= 0 && x <= max &&
                            x >= startX && x <= endX &&
                            y >= 0 && y <= max)
                        {
                            candidates.Add(new Position(x, y));
                        }
                    }
                }
            }
        }


        var scannedNonBeacons = scanned[rowOfInterest].Values.Where(isBeacon => !isBeacon).Count();
        Console.WriteLine(scannedNonBeacons);

        var targetBeacon = candidates
            .Where(x => !ContainsPos(circles, x))
            .Single();
        var frequency = ((long)targetBeacon.X * max) + targetBeacon.Y;
        Console.WriteLine(frequency);
    }

    bool ContainsPos(List<Circle> circles, Position position)
    {
        foreach (var circle in circles)
        {
            if (ContainsPos(circle, position))
                return true;
        }

        return false;
    }

    bool ContainsPos(Circle circle, Position position)
    {
        var dx = Math.Abs(position.X - circle.X);
        var dy = Math.Abs(position.Y - circle.Y);
        var distance = dx + dy;

        return distance <= circle.R;
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
    record Circle(int X, int Y, int R): Element(X, Y);
}