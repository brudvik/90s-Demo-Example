using NAudio.Wave;

namespace DemoTest.Audio;

/// <summary>
/// Represents a looping audio stream that continuously reads from a WaveStream.
/// </summary>
/// <param name="reader"></param>
public class LoopStream(WaveStream reader) : ISampleProvider
{
    private readonly ISampleProvider source = reader.ToSampleProvider();
    private readonly WaveStream reader = reader;

    /// <summary>
    /// Gets the WaveFormat of the audio stream, which describes the format of the audio data (e.g., sample rate, channels).
    /// </summary>
    public WaveFormat WaveFormat => source.WaveFormat;

    /// <summary>
    /// Reads a specified number of samples from the audio stream into the provided buffer, looping if necessary.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int Read(float[] buffer, int offset, int count)
    {
        int totalRead = 0;
        while (totalRead < count)
        {
            int read = source.Read(buffer, offset + totalRead, count - totalRead);
            if (read == 0)
            {
                reader.Position = 0;
            }
            totalRead += read;
        }
        return totalRead;
    }
}
