﻿/*  
	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at
	
	http://www.apache.org/licenses/LICENSE-2.0
	
	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Controls;
using System.Diagnostics;

namespace WP7CordovaClassLib.Cordova.Commands
{
    /// <summary>
    /// Implements audio record and play back functionality.
    /// </summary>
    internal class AudioPlayer : IDisposable
    {
        #region Constants

        // AudioPlayer states
        private const int MediaNone = 0;
        private const int MediaStarting = 1;
        private const int MediaRunning = 2;
        private const int MediaPaused = 3;
        private const int MediaStopped = 4;

        // AudioPlayer messages
        private const int MediaState = 1;
        private const int MediaDuration = 2;
        private const int MediaPosition = 3;
        private const int MediaError = 9;

        // AudioPlayer errors
        private const int MediaErrorPlayModeSet = 1;
        private const int MediaErrorAlreadyRecording = 2;
        private const int MediaErrorStartingRecording = 3;
        private const int MediaErrorRecordModeSet = 4;
        private const int MediaErrorStartingPlayback = 5;
        private const int MediaErrorResumeState = 6;
        private const int MediaErrorPauseState = 7;
        private const int MediaErrorStopState = 8;

        //TODO: get rid of this callback, it should be universal
        private const string CallbackFunction = "CordovaMediaonStatus";

        #endregion

        /// <summary>
        /// The AudioHandler object
        /// </summary>
        private Media handler;

        /// <summary>
        /// Temporary buffer to store audio chunk
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// Xna game loop dispatcher
        /// </summary>
        DispatcherTimer dtXna;

        /// <summary>
        /// Output buffer
        /// </summary>
        private MemoryStream memoryStream;

        /// <summary>
        /// The id of this player (used to identify Media object in JavaScript)
        /// </summary>
        private String id;

        /// <summary>
        /// State of recording or playback
        /// </summary>
        private int state = MediaNone;

        /// <summary>
        /// File name to play or record to
        /// </summary>
        private String audioFile = null;

        /// <summary>
        /// Duration of audio
        /// </summary>
        private double duration = -1;

        /// <summary>
        /// Audio player object
        /// </summary>
        private MediaElement player = null;

        /// <summary>
        /// Audio source
        /// </summary>
        private Microphone recorder;

        /// <summary>
        /// Internal flag specified that we should only open audio w/o playing it
        /// </summary>
        private bool prepareOnly = false;

        /// <summary>
        /// Creates AudioPlayer instance
        /// </summary>
        /// <param name="handler">Media object</param>
        /// <param name="id">player id</param>
        public AudioPlayer(Media handler, String id)
        {
            this.handler = handler;
            this.id = id;
        }

        /// <summary>
        /// Destroys player and stop audio playing or recording
        /// </summary>
        public void Dispose()
        {
            Debug.WriteLine("Dispose :: " + this.audioFile);
            if (this.player != null)
            {
                this.stopPlaying();
                this.player = null;
            }
            if (this.recorder != null)
            {
                this.stopRecording();
                this.recorder = null;
            }

            this.FinalizeXnaGameLoop();
        }

        /// <summary>
        /// Starts recording, data is stored in memory
        /// </summary>
        /// <param name="filePath"></param>
        public void startRecording(string filePath)
        {
            if (this.player != null)
            {
                this.handler.InvokeCustomScript(new ScriptCallback(CallbackFunction, this.id, MediaError, MediaErrorPlayModeSet));
            }
            else if (this.recorder == null)
            {
                try
                {
                    this.audioFile = filePath;
                    this.InitializeXnaGameLoop();
                    this.recorder = Microphone.Default;
                    this.recorder.BufferDuration = TimeSpan.FromMilliseconds(500);
                    this.buffer = new byte[recorder.GetSampleSizeInBytes(this.recorder.BufferDuration)];
                    this.recorder.BufferReady += new EventHandler<EventArgs>(recorderBufferReady);
                    this.memoryStream = new MemoryStream();
                    this.WriteWavHeader(this.memoryStream, this.recorder.SampleRate);
                    this.recorder.Start();
                    FrameworkDispatcher.Update();
                    this.SetState(MediaRunning);
                }
                catch (Exception)
                {
                    this.handler.InvokeCustomScript(new ScriptCallback(CallbackFunction, this.id, MediaError, MediaErrorStartingRecording));
                }
            }
            else
            {

                this.handler.InvokeCustomScript(new ScriptCallback(CallbackFunction, this.id, MediaError, MediaErrorAlreadyRecording));
            }
        }

        /// <summary>
        /// Stops recording
        /// </summary>
        public void stopRecording()
        {
            if (this.recorder != null)
            {
                if (this.state == MediaRunning)
                {
                    try
                    {
                        this.recorder.Stop();
                        this.recorder.BufferReady -= recorderBufferReady;
                        this.recorder = null;
                        SaveAudioClipToLocalStorage();
                        this.FinalizeXnaGameLoop();
                        this.SetState(MediaStopped);
                    }
                    catch (Exception)
                    {
                        //TODO 
                    }
                }
            }
        }

        /// <summary>
        /// Starts or resume playing audio file
        /// </summary>
        /// <param name="filePath">The name of the audio file</param>
        /// <summary>
        /// Starts or resume playing audio file
        /// </summary>
        /// <param name="filePath">The name of the audio file</param>
        public void startPlaying(string filePath)
        {
            if (this.recorder != null)
            {
                this.handler.InvokeCustomScript(new ScriptCallback(CallbackFunction, this.id, MediaError, MediaErrorRecordModeSet));
                return;
            }


            if (this.player == null)
            {
                try
                {
                    if (this.player == null)
                    {
                        // this.player is a MediaElement, it must be added to the visual tree in order to play
                        PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
                        if (frame != null)
                        {
                            PhoneApplicationPage page = frame.Content as PhoneApplicationPage;
                            if (page != null)
                            {
                                Grid grid = page.FindName("LayoutRoot") as Grid;
                                if (grid != null)
                                {

                                    this.player = new MediaElement();
                                    this.player.Name = "playerMediaElement";
                                    grid.Children.Add(this.player);
                                    this.player.Visibility = Visibility.Collapsed;
                                    this.player.MediaOpened += MediaOpened;
                                    this.player.MediaEnded += MediaEnded;
                                    this.player.MediaFailed += MediaFailed;
                                }
                            }
                        }

                    }
                    this.audioFile = filePath;

                    Uri uri = new Uri(filePath, UriKind.RelativeOrAbsolute);
                    if (uri.IsAbsoluteUri)
                    {
                        this.player.Source = uri;
                    }
                    else
                    {
                        using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            if (isoFile.FileExists(filePath))
                            {
                                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filePath, FileMode.Open, isoFile))
                                {
                                    this.player.SetSource(stream);
                                }
                            }
                            else
                            {
                                Debug.WriteLine("Error: source doesn't exist :: " + filePath);
                                throw new ArgumentException("Source doesn't exist");
                            }
                        }
                    }
                    this.SetState(MediaStarting);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                    this.handler.InvokeCustomScript(new ScriptCallback(CallbackFunction, this.id, MediaError, MediaErrorStartingPlayback));
                }
            }
            else
            {
                if (this.state != MediaRunning)
                {
                    this.player.Play();
                    this.SetState(MediaRunning);
                }
                else
                {
                    this.handler.InvokeCustomScript(new ScriptCallback(CallbackFunction, this.id, MediaError, MediaErrorResumeState));
                }
            }
        }

        /// <summary>
        /// Callback to be invoked when the media source is ready for playback
        /// </summary>
        private void MediaOpened(object sender, RoutedEventArgs arg)
        {
            if (!this.prepareOnly)
            {
                this.player.Play();
                this.SetState(MediaRunning);
            }

            this.duration = this.player.NaturalDuration.TimeSpan.TotalSeconds;
            this.prepareOnly = false;
            this.handler.InvokeCustomScript(new ScriptCallback(CallbackFunction, this.id, MediaDuration, this.duration));
        }

        /// <summary>
        /// Callback to be invoked when playback of a media source has completed
        /// </summary>
        private void MediaEnded(object sender, RoutedEventArgs arg)
        {
            this.SetState(MediaStopped);
        }

        /// <summary>
        /// Callback to be invoked when playback of a media source has failed
        /// </summary>
        private void MediaFailed(object sender, RoutedEventArgs arg)
        {
            player.Stop();
            this.handler.InvokeCustomScript(new ScriptCallback(CallbackFunction, this.id, MediaError.ToString(), "Media failed"));
        }

        /// <summary>
        /// Seek or jump to a new time in the track
        /// </summary>
        /// <param name="milliseconds">The new track position</param>
        public void seekToPlaying(int milliseconds)
        {
            if (this.player != null)
            {
                TimeSpan tsPos = new TimeSpan(0, 0, 0, 0, milliseconds);
                this.player.Position = tsPos;
                this.handler.InvokeCustomScript(new ScriptCallback(CallbackFunction, this.id, MediaPosition, milliseconds / 1000.0f));
            }
        }

        /// <summary>
        /// Set the volume of the player
        /// </summary>
        /// <param name="vol">volume 0.0-1.0, default value is 0.5</param>
        public void setVolume(double vol)
        {
            if (this.player != null)
            {
                this.player.Volume = vol;
            }
        }

        /// <summary>
        /// Pauses playing
        /// </summary>
        public void pausePlaying()
        {
            if (this.state == MediaRunning)
            {
                this.player.Pause();
                this.SetState(MediaPaused);
            }
            else
            {
                this.handler.InvokeCustomScript(new ScriptCallback(CallbackFunction, this.id, MediaError, MediaErrorPauseState));
            }
        }


        /// <summary>
        /// Stops playing the audio file
        /// </summary>
        public void stopPlaying()
        {
            if ((this.state == MediaRunning) || (this.state == MediaPaused))
            {
                this.player.Stop();

                this.player.Position = new TimeSpan(0L);
                this.SetState(MediaStopped);
            }
            else
            {
                this.handler.InvokeCustomScript(new ScriptCallback(CallbackFunction, this.id, MediaError, MediaErrorStopState));
            }
        }

        /// <summary>
        /// Gets current position of playback
        /// </summary>
        /// <returns>current position</returns>
        public double getCurrentPosition()
        {
            if ((this.state == MediaRunning) || (this.state == MediaPaused))
            {
                double currentPosition = this.player.Position.TotalSeconds;
                this.handler.InvokeCustomScript(new ScriptCallback(CallbackFunction, this.id, MediaPosition, currentPosition));
                return currentPosition;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the duration of the audio file
        /// </summary>
        /// <param name="filePath">The name of the audio file</param>
        /// <returns>track duration</returns>
        public double getDuration(string filePath)
        {
            if (this.recorder != null)
            {
                return (-2);
            }

            if (this.player != null)
            {
                return this.duration;

            }
            else
            {
                this.prepareOnly = true;
                this.startPlaying(filePath);
                return this.duration;
            }
        }

        /// <summary>
        /// Sets the state and send it to JavaScript
        /// </summary>
        /// <param name="state">state</param>
        private void SetState(int state)
        {
            if (this.state != state)
            {
                this.handler.InvokeCustomScript(new ScriptCallback(CallbackFunction, this.id, MediaState, state));
            }

            this.state = state;
        }

        #region record methods

        /// <summary>
        /// Copies data from recorder to memory storages and updates recording state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recorderBufferReady(object sender, EventArgs e)
        {
            this.recorder.GetData(this.buffer);
            this.memoryStream.Write(this.buffer, 0, this.buffer.Length);
        }

        /// <summary>
        /// Writes audio data from memory to isolated storage
        /// </summary>
        /// <returns></returns>
        private void SaveAudioClipToLocalStorage()
        {
            if (this.memoryStream == null || this.memoryStream.Length <= 0)
            {
                return;
            }

            this.UpdateWavHeader(this.memoryStream);

            try
            {
                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    string directory = Path.GetDirectoryName(audioFile);

                    if (!isoFile.DirectoryExists(directory))
                    {
                        isoFile.CreateDirectory(directory);
                    }

                    this.memoryStream.Seek(0, SeekOrigin.Begin);

                    using (IsolatedStorageFileStream fileStream = isoFile.CreateFile(audioFile))
                    {
                        this.memoryStream.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception)
            {
                //TODO: log or do something else
                throw;
            }
        }



        #region Wav format
        // Original source http://damianblog.com/2011/02/07/storing-wp7-recorded-audio-as-wav-format-streams/

        /// <summary>
        /// Adds wav file format header to the stream
        /// https://ccrma.stanford.edu/courses/422/projects/WaveFormat/
        /// </summary>
        /// <param name="stream">Wav stream</param>
        /// <param name="sampleRate">Sample Rate</param>
        private void WriteWavHeader(Stream stream, int sampleRate)
        {
            const int bitsPerSample = 16;
            const int bytesPerSample = bitsPerSample / 8;
            var encoding = System.Text.Encoding.UTF8;

            // ChunkID Contains the letters "RIFF" in ASCII form (0x52494646 big-endian form).
            stream.Write(encoding.GetBytes("RIFF"), 0, 4);

            // NOTE this will be filled in later
            stream.Write(BitConverter.GetBytes(0), 0, 4);

            // Format Contains the letters "WAVE"(0x57415645 big-endian form).
            stream.Write(encoding.GetBytes("WAVE"), 0, 4);

            // Subchunk1ID Contains the letters "fmt " (0x666d7420 big-endian form).
            stream.Write(encoding.GetBytes("fmt "), 0, 4);

            // Subchunk1Size 16 for PCM.  This is the size of therest of the Subchunk which follows this number.
            stream.Write(BitConverter.GetBytes(16), 0, 4);

            // AudioFormat PCM = 1 (i.e. Linear quantization) Values other than 1 indicate some form of compression.
            stream.Write(BitConverter.GetBytes((short)1), 0, 2);

            // NumChannels Mono = 1, Stereo = 2, etc.
            stream.Write(BitConverter.GetBytes((short)1), 0, 2);

            // SampleRate 8000, 44100, etc.
            stream.Write(BitConverter.GetBytes(sampleRate), 0, 4);

            // ByteRate =  SampleRate * NumChannels * BitsPerSample/8
            stream.Write(BitConverter.GetBytes(sampleRate * bytesPerSample), 0, 4);

            // BlockAlign NumChannels * BitsPerSample/8 The number of bytes for one sample including all channels.
            stream.Write(BitConverter.GetBytes((short)(bytesPerSample)), 0, 2);

            // BitsPerSample    8 bits = 8, 16 bits = 16, etc.
            stream.Write(BitConverter.GetBytes((short)(bitsPerSample)), 0, 2);

            // Subchunk2ID Contains the letters "data" (0x64617461 big-endian form).
            stream.Write(encoding.GetBytes("data"), 0, 4);

            // NOTE to be filled in later
            stream.Write(BitConverter.GetBytes(0), 0, 4);
        }

        /// <summary>
        /// Updates wav file format header
        /// https://ccrma.stanford.edu/courses/422/projects/WaveFormat/
        /// </summary>
        /// <param name="stream">Wav stream</param>
        private void UpdateWavHeader(Stream stream)
        {
            if (!stream.CanSeek) throw new Exception("Can't seek stream to update wav header");

            var oldPos = stream.Position;

            // ChunkSize  36 + SubChunk2Size
            stream.Seek(4, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes((int)stream.Length - 8), 0, 4);

            // Subchunk2Size == NumSamples * NumChannels * BitsPerSample/8 This is the number of bytes in the data.
            stream.Seek(40, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes((int)stream.Length - 44), 0, 4);

            stream.Seek(oldPos, SeekOrigin.Begin);
        }

        #endregion

        #region Xna loop
        /// <summary>
        /// Special initialization required for the microphone: XNA game loop
        /// </summary>
        private void InitializeXnaGameLoop()
        {
            // Timer to simulate the XNA game loop (Microphone is from XNA)
            this.dtXna = new DispatcherTimer();
            this.dtXna.Interval = TimeSpan.FromMilliseconds(33);
            this.dtXna.Tick += delegate { try { FrameworkDispatcher.Update(); } catch { } };
            this.dtXna.Start();
        }
        /// <summary>
        /// Finalizes XNA game loop for microphone
        /// </summary>
        private void FinalizeXnaGameLoop()
        {
            // Timer to simulate the XNA game loop (Microphone is from XNA)
            this.dtXna.Stop();
            this.dtXna = null;
        }

        #endregion

        #endregion
    }
}
