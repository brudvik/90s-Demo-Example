namespace DemoTest.Elements;

/// <summary>
/// Represents an orbiter that moves in a circular path around a center point.
/// </summary>
public class Orbiter
{
    /// <summary>
    /// Gets or sets the center point around which the orbiter moves.
    /// </summary>
    public float Radius;

    /// <summary>
    /// Gets or sets the speed of the orbiter's movement around the center point.
    /// </summary>
    public float Speed;

    /// <summary>
    /// Gets or sets the current phase of the orbiter's movement, which determines its position along the circular path.
    /// </summary>
    public float Phase;

    /// <summary>
    /// Gets or sets the center point around which the orbiter moves.
    /// </summary>
    public List<PointF> Trail = [];
}
