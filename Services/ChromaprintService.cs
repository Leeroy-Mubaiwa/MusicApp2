using NAudio.Wave;
using System.Text;

namespace MusicApp2.Services
{
    public class ChromaprintService
    {
        private readonly ILogger<ChromaprintService> _logger;

        public ChromaprintService(ILogger<ChromaprintService> logger)
        {
            _logger = logger;
        }

        public async Task<string> CreateFingerprint(string filePath)
        {
            try
            {
                using (var reader = new AudioFileReader(filePath))
                {
                    // Convert to mono and 16-bit PCM
                    var mono = new WaveFormatConversionStream(new WaveFormat(44100, 1), reader);
                    var pcm = new WaveFormatConversionStream(new WaveFormat(44100, 16, 1), mono);

                    // Read the audio data
                    var buffer = new byte[4096];
                    var samples = new List<short>();
                    int bytesRead;

                    while ((bytesRead = await pcm.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        for (int i = 0; i < bytesRead; i += 2)
                        {
                            samples.Add(BitConverter.ToInt16(buffer, i));
                        }
                    }

                    // Create fingerprint using Chromaprint algorithm
                    var fingerprint = CreateChromaprintFingerprint(samples.ToArray());
                    return Convert.ToBase64String(fingerprint);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audio fingerprint");
                throw;
            }
        }

        private byte[] CreateChromaprintFingerprint(short[] samples)
        {
            // This is a simplified version of the Chromaprint algorithm
            // In a real implementation, you would use the actual Chromaprint library

            // 1. Apply FFT to get frequency components
            var fft = ApplyFFT(samples);

            // 2. Create frequency bands
            var bands = CreateFrequencyBands(fft);

            // 3. Create fingerprint from frequency bands
            return CreateFingerprintFromBands(bands);
        }

        private double[] ApplyFFT(short[] samples)
        {
            // Simplified FFT implementation
            // In a real implementation, use a proper FFT library
            var n = samples.Length;
            var fft = new double[n];

            for (int k = 0; k < n; k++)
            {
                double sumReal = 0;
                double sumImag = 0;

                for (int t = 0; t < n; t++)
                {
                    double angle = 2 * Math.PI * t * k / n;
                    sumReal += samples[t] * Math.Cos(angle);
                    sumImag += samples[t] * Math.Sin(angle);
                }

                fft[k] = Math.Sqrt(sumReal * sumReal + sumImag * sumImag);
            }

            return fft;
        }

        private double[] CreateFrequencyBands(double[] fft)
        {
            // Create 12 frequency bands (similar to musical octaves)
            var bands = new double[12];
            var bandSize = fft.Length / 12;

            for (int i = 0; i < 12; i++)
            {
                double sum = 0;
                for (int j = 0; j < bandSize; j++)
                {
                    sum += fft[i * bandSize + j];
                }
                bands[i] = sum / bandSize;
            }

            return bands;
        }

        private byte[] CreateFingerprintFromBands(double[] bands)
        {
            // Create a fingerprint by comparing adjacent bands
            var fingerprint = new List<byte>();

            for (int i = 0; i < bands.Length - 1; i++)
            {
                byte value = (byte)(bands[i] > bands[i + 1] ? 1 : 0);
                fingerprint.Add(value);
            }

            return fingerprint.ToArray();
        }
    }
}