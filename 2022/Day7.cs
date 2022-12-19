namespace AdventOfCode;

public class Day7
{
    public async Task RunAsync()
    {
        var input = await System.IO.File.ReadAllTextAsync("Day7Input.txt");
        var commandsInput = input.Split('$', StringSplitOptions.RemoveEmptyEntries);
        var commands = new List<ICommand>();
        foreach (var commandInput in commandsInput)
        {
            var command = ParseCommand(commandInput);
            commands.Add(command);
        }

        var root = new Directory("/", new List<IEle>());
        var nagivationHistory = new Stack<Directory>();
        var currentDir = root;
        foreach (var command in commands)
        {
            if (command is Cd cd)
            {
                if (cd.Dir == "..")
                    currentDir = nagivationHistory.Pop();
                else if (cd.Dir == "/")
                {
                    currentDir = root;
                    nagivationHistory.Clear();
                }
                else
                {
                    nagivationHistory.Push(currentDir);
                    currentDir = currentDir.Elements
                        .OfType<Directory>()
                        .First(x => x.Name == cd.Dir);
                }
            }
            else if (command is Ls ls)
            {
                foreach (var ele in ls.Elements)
                {
                    if (ele is Dir dir)
                        currentDir.Elements.Add(new Directory(dir.Name, new List<IEle>()));
                    else if (ele is File file)
                        currentDir.Elements.Add(new Fil(file.Name, file.Size));
                }
            }
        }

        var sizes = GetFlatDirs(root)
            .Select(x => (Dir: x, Size: CalculateSize(x)))
            .ToList();
        
        var part1 = sizes.Where(x => x.Size <= 100000).ToList();
        Console.WriteLine(part1.Sum(x => x.Size));

        var usedSpace = CalculateSize(root);
        var unusedSpace = Capacity - usedSpace;
        var missingSpace = Required - unusedSpace;
        var part2 = sizes.Where(x => x.Size >= missingSpace).MinBy(x => x.Size);
        Console.WriteLine(part2.Size);
    }

    const int Capacity = 70000000;
    const int Required = 30000000;

    List<Directory> GetFlatDirs(Directory dir)
    {
        var result = new List<Directory>();
        foreach (var d in dir.Elements.OfType<Directory>())
        {
            result.Add(d);
            result.AddRange(GetFlatDirs(d));
        }

        return result;
    }

    int CalculateSize(Directory dir)
    {
        var size = 0;
        foreach (var ele in dir.Elements)
        {
            if (ele is Fil f)
                size += f.Size;
            else if (ele is Directory d)
                size += CalculateSize(d);
        }

        return size;
    }

    interface IEle{}
    record Directory(string Name, List<IEle> Elements) : IEle;
    record Fil(string Name, int Size) : IEle;

    ICommand ParseCommand(string commandInput)
    {
        var inputOutputSplit = commandInput.Split(Environment.NewLine, 2);
        var commandArgSplit = inputOutputSplit[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        ICommand command = commandArgSplit[0] switch
        {
            "cd" => new Cd(commandArgSplit[1]),
            "ls" => new Ls(ParseElements(inputOutputSplit[1])),
            _ => throw new Exception()
        };

        return command;
    }

    List<IElement> ParseElements(string output)
    {
        var outputLines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var result = new List<IElement>();
        foreach (var outputLine in outputLines)
        {
            var tokens = outputLine.Split(' ', 2);
            if (tokens[0] == "dir")
                result.Add(new Dir(tokens[1]));
            else
                result.Add(new File(tokens[1], int.Parse(tokens[0])));
        }

        return result;
    }

    interface ICommand{}

    record Cd(string Dir) : ICommand;

    record Ls(List<IElement> Elements) : ICommand;

    interface IElement{}

    record File(string Name, int Size) : IElement;

    record Dir(string Name) : IElement;
}