using NAudio.Wave;

namespace DemoTest.Audio;

/// <summary>
/// Represents a music visualizer that plays audio and provides spectrum data for visualization.
/// </summary>
public class MusicVisualizer
{
    private readonly WasapiOut output;
    private readonly SampleAggregator aggregator;

    /// <summary>
    /// Gets the current spectrum data from the audio stream.
    /// </summary>
    public float[]? Spectrum { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicVisualizer"/> class with the specified audio file path.
    /// </summary>
    /// <param name="filePath"></param>
    public MusicVisualizer(string filePath)
    {
        var reader = new AudioFileReader(filePath);
        aggregator = new SampleAggregator(new LoopStream(reader));
        aggregator.FftCalculated += (s, e) =>
        {
            Spectrum = new float[e.Result.Length];
            for (int i = 0; i < Spectrum.Length; i++)
                Spectrum[i] = e.Result[i].X * e.Result[i].X + e.Result[i].Y * e.Result[i].Y;
        };
        output = new WasapiOut();
        output.Init(aggregator);
    }

    /// <summary>
    /// Plays the audio stream and starts visualizing the music.
    /// </summary>
    public void Play() => output.Play();

    /// <summary>
    /// Stops the audio stream and stops visualizing the music.
    /// </summary>
    public void Stop() => output.Stop();

    /// <summary>
    /// Releases the resources used by the <see cref="MusicVisualizer"/> instance.
    /// </summary>
    public void Dispose() => output.Dispose();
}
