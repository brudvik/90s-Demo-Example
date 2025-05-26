namespace DemoTest.Elements;

/// <summary>
/// Represents a particle in a warp effect, with properties for angle, radius, speed, spiral tightness, life duration, and color.
/// </summary>
public class WarpParticle
{
    /// <summary>
    /// Gets or sets the angle of the particle in radians, which determines its position in a circular path.
    /// </summary>
    public float Angle;

    /// <summary>
    /// Gets or sets the radius of the particle's circular path, which determines how far it is from the center point.
    /// </summary>
    public float Radius;

    /// <summary>
    /// Gets or sets the speed of the particle's movement along its circular path, which determines how fast it moves.
    /// </summary>
    public float Speed;

    /// <summary>
    /// Gets or sets the tightness of the spiral effect, which determines how tightly the particle spirals around its path.
    /// </summary>
    public float SpiralTightness;

    /// <summary>
    /// Gets or sets the remaining life duration of the particle, which determines how long it will exist before disappearing.
    /// </summary>
    public float Life;

    /// <summary>
    /// Gets or sets the color of the particle, which can be used for visual effects in the warp effect.
    /// </summary>
    public Color Color;
}
