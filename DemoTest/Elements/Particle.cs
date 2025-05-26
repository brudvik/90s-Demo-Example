namespace DemoTest.Elements;

/// <summary>
/// Represents a particle in a particle system, with properties for position, velocity, life duration, and color.
/// </summary>
public class Particle
{
    /// <summary>
    /// Gets or sets the position of the particle in 2D space.
    /// </summary>
    public PointF Position;

    /// <summary>
    /// Gets or sets the velocity of the particle, which determines its movement direction and speed.
    /// </summary>
    public PointF Velocity;

    /// <summary>
    /// Gets or sets the remaining life duration of the particle, which determines how long it will exist before disappearing.
    /// </summary>
    public float Life;

    /// <summary>
    /// Gets or sets the color of the particle, which can be used for visual effects in the particle system.
    /// </summary>
    public Color Color;
}
