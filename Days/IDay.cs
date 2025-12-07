namespace AdventOfCode.Days;

public abstract class IDay
{
    public abstract string InputFile { get; }

    public abstract string Part1();
    public abstract string Part2();

    protected string[] GetInputFile()
    {
        if (string.IsNullOrWhiteSpace(InputFile))
            throw new InvalidOperationException("InputFile must be provided by the implementing class.");

        var path = Path.Combine("Inputs", InputFile);

        if (!File.Exists(path))
            throw new FileNotFoundException($"Input file not found: {path}", path);

        return File.ReadAllLines(path);
    }
}
