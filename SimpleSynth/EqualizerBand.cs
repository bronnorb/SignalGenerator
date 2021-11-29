using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSynth
{
    public class EqualizerBand
    {
        public float Frequency { get; set; }

        public float Gain { get; set; }
        public float Bandwidth { get; set; }

        public EqualizerBand(float frequency, float gain, float bandwidth)
        {
            Frequency = frequency;
            Gain = gain;
            Bandwidth = bandwidth;
        }
    }
}
