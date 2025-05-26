using NAudio.Dsp;

namespace DemoTest.Audio;

/// <summary>
/// Represents the event arguments for FFT (Fast Fourier Transform) operations.
/// </summary>
public class FftEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the FFT size, which is the number of samples used in the FFT calculation.
    /// </summary>
    public required Complex[] Result { get; set; }
}
