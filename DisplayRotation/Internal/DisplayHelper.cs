namespace DisplayRotation.Internal;

public class DisplayHelper
{
    public int Height { get; set; }
    public uint Id { get; init; }
    public string Name { get; init; }
    public int PositionX { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int PositionY { get; set; }
    public int Width { get; set; }
}