using NAudio.Dsp;
using NAudio.Midi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleSynth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MidiIn midiIn;
        WaveOutEvent waveOut = new WaveOutEvent();
        bool notePlaying = false;
        Equalizer eq;
        EqualizerBand bass;
        EqualizerBand mid;
        EqualizerBand treble;
        EqualizerBand[] bands;
       

        public MainWindow()
        {
            InitializeComponent();
            //bass = new EqualizerBand(100f, 1f, 20f);
            //mid = new EqualizerBand(400f, 1f, 20f);
            //treble = new EqualizerBand(1000f, 1f, 20f);
            bands = new EqualizerBand[3];
            bands[0] = new EqualizerBand(100f, 50f, 10f);
            bands[1] = new EqualizerBand(1000f, 0f, 10f);
            bands[2] = new EqualizerBand(10000f, -50f, 10f);

            for (int device = 0; device < MidiIn.NumberOfDevices; device++)
            {
                midiInSelector.Items.Add(MidiIn.DeviceInfo(device).ProductName);
            }
            if (midiInSelector.Items.Count > 0)
            {
                midiInSelector.SelectedIndex = 0;
            }

            midiIn = new MidiIn(midiInSelector.SelectedIndex);
            midiIn.MessageReceived += midiIn_MessageReceived;
            midiIn.Start();
            
        }

        public void midiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            

            if (e.MidiEvent == null && e.MidiEvent.CommandCode == MidiCommandCode.AutoSensing)
            {
                return;
            }
            else if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOn)
            {
                NoteEvent ne = (NoteEvent)e.MidiEvent;

                if(ne.Velocity > 0 && notePlaying == false)
                {
                    var saw = new SignalGenerator()
                    {
                        Type = SignalGeneratorType.SawTooth,
                        Gain = ne.Velocity / 127d,
                        Frequency = Math.Pow(2d, (double)(ne.NoteNumber - 69) / 12) * 440
                    };

                    //eq = new Equalizer(saw, bands);
                    var lowPass = new LowPassFilter(saw, 100, -50);
                    

                    waveOut.Init(lowPass);
                    waveOut.Play();
                    
                    notePlaying = true;
                }
                else if(ne.Velocity == 0)
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                    notePlaying = false;
                }
            }
            


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            midiIn.Stop();
            midiIn.Dispose();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            midiIn.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            midiIn.Stop();
            midiIn.Dispose();
        }
    }
}
