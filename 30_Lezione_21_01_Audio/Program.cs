using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Audio;
using Aiv.Fast2D;
using OpenTK;

namespace _30_Lezione_21_01_Audio
{
    #region Teoria
    // Download this (you can take the sdk or the installer, the sdk needs to be put in the bin folder
    // of every project will use it, while the installer installs it on the pc for every project).
    // https://www.openal.org/downloads/

    // Then NuGet package Aiv.Audio for API

    // SOUND, SAMPLING AND FREQUENCY
    // Sound is an oscillation of particles in a propagation medium. In the case of sound propagating
    // through a fluid medium (like air) those obscillations are particles displacements from their
    // rest position and along the direction of the propagation, caused by vibrations made by a source
    // object and transmitted to near particles thanks to the medium properties. Particles themselves
    // transmit that movement to near particles and propagates like a wave (a sound wave).

    // So sound is a wave and it can be represented on a cartesian plane in which time is on the X
    // axis and particles displacements are on the Y axis.

    // Turning an analogic signal into digital is not an easy task. We need to convert a "continuous
    // signal" (analogic) into a "discrete signal" (digital, a number - from digit -).
    // A continuous signal is a natural sound: it's "infinitely precise" (if we try to measure it)
    // and its continuous nature means there are no time intervals in it's flow.
    // It is not possible to have those same properties on a digital signal: even if we are able to
    // perform a very deep measure of the original signal, we'll never be able to record such a
    // precise number (bit memory limits), furthermore any digital signal will always be a succession
    // of recording (numbers) with "cuts" (time intervals) in between the performed records (it's the
    // nature of a discrete signal) and no continuous flow.

    // The only thing we can do is sampling the analogic signal at a specific sampling frequency.
    // This means we get samples of that original sound at regular time intervals (the frequency),
    // then we'll be able to build back the sound wave using those samples (and interpolation for
    // the missing ones). A higher sampling frequency means we're getting more samples and we'll be
    // able to reach a higher fidelity reproducing the sampled sound.

    // Using a low-rate sampling frequency cause the so called aliasing artifact (adds low waves
    // to the original sound).

    // The Nyquist-Shannon Sampling Theorem tells us that using a frequency at least greater than
    // the doubled analogic sound frequency (wave repetitions in one second) grants a correct
    // theoretical record of the analogic signal. It's theoretical perks is caused by the fact that
    // we'll be still under the memory limits problem and by the process called quantization (trying
    // to reduce the number of digits used to memorize the signal). All of these introduces artifacts
    // in the reproduced sound wave.

    // Sampling Wiki page (with sampling rates)
    // https://en.wikipedia.org/wiki/Sampling_(signal_processing)#Sampling_rate

    // Sampling example
    // https://en.wikipedia.org/wiki/Sampling_(signal_processing)#/media/File:Signal_Sampling.svg

    // Aliasing example
    // https://it.wikipedia.org/wiki/Aliasing#/media/File:Aliasing-plot.png

    // Measure limits example
    // https://upload.wikimedia.org/wikipedia/commons/thumb/2/21/4-bit-linear-PCM.svg/1200px-4-bit-linear-PCM.svg.png

    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            #region Microfono
            Window win = new Window(800, 600, "Spatial Audio test");

            // Speakers on my PC
            foreach (string device in AudioDevice.Devices)
            {
                Console.WriteLine(device);
            }

            // Microphone on my PC
            foreach (string device in AudioDevice.CaptureDevices)
            {
                Console.WriteLine(device);
            } 



            // Microphone Config
            bool micRec = false;
            int frequency = 22050;  // sampling frequency (human range: from 20Hz to 20KHz)
            int channels = 1;
            float micMaxTime = 2.0f;
            
            AudioCapture microphone = new AudioCapture(frequency, channels, micMaxTime);
            AudioBuffer micBuffer = new AudioBuffer();  // Mic will be recorded on this buffer

            #endregion


            // AudioDevice (Listener) (THERE CAN BE ONLY ONE!!! MUST BE CREATED BEFORE THE SOURCES)
            AudioDevice playerEar = new AudioDevice();



            // AudioSources (Sound Emitters)
            #region Microfono
            AudioSource micSource = new AudioSource(); 
            #endregion
            AudioSource bgSource = new AudioSource();
            #region Shoot
            AudioSource source = new AudioSource(); 
            #endregion



            // Background AudioSource Config
            bgSource.Position = new Vector3(win.Width * 0.5f, win.Height * 0.5f, 0.0f);
            bgSource.ReferenceDistance = 100.0f;    // if listener over -> volume dropped in half before
                                                    // being influenced by RollOffFactor or MaxDistance
            bgSource.RolloffFactor = 5.0f;          // sound attenuation factor (default = 1.0f);
            bgSource.MaxDistance = 200.0f;          // max distance for attenuation to be applied



            // Audio Clips
            #region Shoot
            AudioClip shoot = new AudioClip("Assets/Sounds/cannonShoot.wav"); 
            #endregion
            AudioClip bgMusic = new AudioClip("Assets/Sounds/coffin_dance.wav");



            #region Ascoltatore
            // Player
            Texture playerTexture = new Texture("Assets/Graphics/ear.png");
            Sprite player = new Sprite(playerTexture.Width, playerTexture.Height);
            player.pivot = new Vector2(player.Width * 0.5f, player.Height * 0.5f);
            player.position = new Vector2(win.Width * 0.5f, win.Height * 0.5f);
            playerEar.Position = new Vector3(player.position);
            float playerSpeed = 200.0f; 
            #endregion



            #region Animation
            // Coffin Animation
            int animationFrames = 16;
            Texture sourceTexture = new Texture("Assets/Graphics/coffin_anim.png");
            Sprite sourceSprite = new Sprite(sourceTexture.Width / animationFrames, sourceTexture.Height);
            sourceSprite.pivot = new Vector2(sourceSprite.Width * 0.5f, sourceSprite.Height * 0.5f);
            sourceSprite.position = new Vector2(win.Width * 0.5f, win.Height * 0.5f);
            sourceSprite.scale = new Vector2(1.0f, 1.0f);

            Vector2 textureOffset = new Vector2(0.0f, 0.0f);

            Animation coffinAnim = new Animation((int)sourceSprite.Width, (int)sourceSprite.Height, 20, animationFrames);
            coffinAnim.Play(); 
            #endregion



            #region Shoot
            // Shoot Sound
            bool isBarPressed = false;
            Random rand = new Random(); 
            #endregion



            #region Frame
            // Debug Frames
            Texture frameTexture = new Texture("Assets/Graphics/Frame.png");

            Sprite frame_01 = new Sprite(frameTexture.Width, frameTexture.Height);
            frame_01.pivot = new Vector2(frame_01.Width * 0.5f, frame_01.Height * 0.5f);
            frame_01.position = new Vector2(win.Width * 0.5f, win.Height * 0.5f);
            frame_01.scale = new Vector2(bgSource.ReferenceDistance / frame_01.Width * 2, bgSource.ReferenceDistance / frame_01.Width * 2);

            Sprite frame_02 = new Sprite(frameTexture.Width, frameTexture.Height);
            frame_02.pivot = new Vector2(frame_02.Width * 0.5f, frame_02.Height * 0.5f);
            frame_02.position = new Vector2(win.Width * 0.5f, win.Height * 0.5f);
            frame_02.scale = new Vector2(bgSource.MaxDistance / frame_02.Width * 2, bgSource.MaxDistance / frame_02.Width * 2);

            #endregion


            while (win.IsOpened)
            {
                bgSource.Stream(bgMusic, win.DeltaTime);

                // INPUT

                #region Microfono
                //Microphone
                if (win.GetKey(KeyCode.R))
                {
                    // If not already recording
                    if (!micRec)
                    {
                        micRec = true;
                        micBuffer = new AudioBuffer();
                        microphone.Start();
                        Console.WriteLine("Rec");
                    }
                }
                else if (micRec)
                {
                    micRec = false;
                    microphone.Read(micBuffer);
                    micSource.Play(micBuffer);
                    Console.WriteLine("Play");
                }
                #endregion



                #region Ascoltatore
                // Player
                if (win.GetKey(KeyCode.D))
                {
                    player.position.X += playerSpeed * win.DeltaTime;
                }
                else if (win.GetKey(KeyCode.A))
                {
                    player.position.X -= playerSpeed * win.DeltaTime;
                }

                if (win.GetKey(KeyCode.S))
                {
                    player.position.Y += playerSpeed * win.DeltaTime;
                }
                else if (win.GetKey(KeyCode.W))
                {
                    player.position.Y -= playerSpeed * win.DeltaTime;
                } 
                #endregion



                #region Shoot
                // Shoot Sound
                if (win.GetKey(KeyCode.Space))
                {
                    if (!isBarPressed)
                    {
                        isBarPressed = true;
                        float randomPitch = rand.Next(8, 14) / 10.0f;
                        source.Pitch = randomPitch;

                        source.Play(shoot);
                    }
                }
                else if (isBarPressed)
                {
                    isBarPressed = false;
                } 
                #endregion



                // UPDATE
                playerEar.Position = new Vector3(player.position);
                coffinAnim.Update(win.DeltaTime, ref textureOffset);



                // DRAW
                sourceSprite.DrawTexture(sourceTexture, (int)textureOffset.X, (int)textureOffset.Y, (int)sourceSprite.Width, (int)sourceSprite.Height);
                player.DrawTexture(playerTexture);

                #region Frame
                // Debug Frames
                frame_01.DrawTexture(frameTexture);
                frame_02.DrawTexture(frameTexture); 
                #endregion

                win.Update();
            }
        }
    }
}
