using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;


namespace SimpleSynth
{

    public class LowPassFilter : ISampleProvider
    {
        private readonly ISampleProvider sourceProvider;
        private readonly BiQuadFilter[,] filters;
        private readonly float frequency;
        private readonly float q;
        private readonly int channels;
        private readonly int bandCount;
        private bool updated;

        public LowPassFilter(ISampleProvider sourceProvider, float frequency, float q)
        {
            this.sourceProvider = sourceProvider;
            this.frequency = frequency;
            this.q = q;
            channels = sourceProvider.WaveFormat.Channels;
            filters = new BiQuadFilter[channels, 1];
            CreateLowPassFilter();
        }

        private void CreateLowPassFilter()
        {


            filters[0,0] = BiQuadFilter.LowPassFilter(sourceProvider.WaveFormat.SampleRate, frequency, q);
            filters[1, 0] = BiQuadFilter.LowPassFilter(sourceProvider.WaveFormat.SampleRate, frequency, q);

        }


        //private void CreateFilters()
        //{
        //    for (int bandIndex = 0; bandIndex < bandCount; bandIndex++)
        //    {
        //        var band = bands[bandIndex];
        //        for (int n = 0; n < channels; n++)
        //        {
        //            if (filters[n, bandIndex] == null)
        //                filters[n, bandIndex] = BiQuadFilter.PeakingEQ(sourceProvider.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
        //            else
        //                filters[n, bandIndex].SetPeakingEq(sourceProvider.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
        //        }
        //    }
        //}

        public void Update()
        {
            updated = true;
            CreateLowPassFilter();
        }

        public WaveFormat WaveFormat => sourceProvider.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = sourceProvider.Read(buffer, offset, count);

            if (updated)
            {
                CreateLowPassFilter();
                updated = false;
            }

            for (int n = 0; n < samplesRead; n++)
            {
                int ch = n % channels;

                for (int band = 0; band < bandCount; band++)
                {
                    buffer[offset + n] = filters[ch, band].Transform(buffer[offset + n]);
                }
            }
            return samplesRead;
        }
    }

}

