using NAudio.Dsp;
using NAudio.Wave;

namespace DemoTest.Audio;

/// <summary>
/// Represents a sample aggregator that performs Fast Fourier Transform (FFT) on audio samples.
/// </summary>
public class SampleAggregator : ISampleProvider
{
    private readonly ISampleProvider source;
    private readonly Complex[] fftBuffer;
    private int fftPos;
    private readonly int fftLength;
    private readonly FftEventArgs args;
    private readonly object lockObj = new();

    /// <summary>
    /// Represents the event that is raised when FFT calculation is completed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FftCalculatedEventHandler(object sender, FftEventArgs e);

    /// <summary>
    /// Occurs when the FFT calculation is completed and the results are available.
    /// </summary>
    public event FftCalculatedEventHandler? FftCalculated;

    /// <summary>
    /// Gets the WaveFormat of the audio stream, which describes the format of the audio data (e.g., sample rate, channels).
    /// </summary>
    public WaveFormat WaveFormat => source.WaveFormat;

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleAggregator"/> class with the specified audio source and FFT length.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="fftLength"></param>
    public SampleAggregator(ISampleProvider source, int fftLength = 1024)
    {
        this.source = source;
        this.fftLength = fftLength;
        fftBuffer = new Complex[fftLength];
        args = new FftEventArgs { Result = new Complex[fftLength] };
    }

    /// <summary>
    /// Reads a specified number of samples from the audio stream into the provided buffer, performing FFT on the samples.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int Read(float[] buffer, int offset, int count)
    {
        int samplesRead = source.Read(buffer, offset, count);
        for (int i = 0; i < samplesRead; i++)
        {
            lock (lockObj)
            {
                fftBuffer[fftPos].X = buffer[offset + i];
                fftBuffer[fftPos].Y = 0;
                fftPos++;
                if (fftPos >= fftLength)
                {
                    fftPos = 0;
                    Array.Copy(fftBuffer, args.Result, fftLength);
                    FastFourierTransform.FFT(true, (int)Math.Log(fftLength, 2), args.Result);
                    FftCalculated?.Invoke(this, args);
                }
            }
        }
        return samplesRead;
    }
}
