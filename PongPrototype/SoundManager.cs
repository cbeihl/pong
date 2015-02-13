using SlimDX.DirectSound;
using SlimDX.Multimedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pong
{
    class SoundManager
    {
        IntPtr windowHandle;
        DirectSound directSound;
        PrimarySoundBuffer primaryBuffer;

        SecondarySoundBuffer bgMusicBuffer, paddleHitBuffer, wallHitBuffer;

        public SoundManager(IntPtr windowHandle)
        {
            this.windowHandle = windowHandle;
            initDirectSound();
        }

        private void initDirectSound()
        {
            // create DirectSound object.
            directSound = new DirectSound();

            // set cooperative level.
            directSound.SetCooperativeLevel(windowHandle, SlimDX.DirectSound.CooperativeLevel.Priority);

            // create the primary sound buffer.
            SoundBufferDescription desc = new SoundBufferDescription();
            desc.Flags = SlimDX.DirectSound.BufferFlags.PrimaryBuffer;
            primaryBuffer = new PrimarySoundBuffer(directSound, desc);

            // create secondary sound buffer
            bgMusicBuffer = loadSoundFile("hustlepong_10.wav");
            paddleHitBuffer = loadSoundFile("tennis_ball_hit_by_racket.wav");
            wallHitBuffer = loadSoundFile("tennis_ball_single_bounce_floor_001.wav");
        }

        public void PlayMusicLoop()
        {
            // play our music and have it loop continuously.
            bgMusicBuffer.Play(0, SlimDX.DirectSound.PlayFlags.Looping);
        }

        public void PlayPaddleHit()
        {
            paddleHitBuffer.Play(0, PlayFlags.None);
        }

        public void PlayWallHit()
        {
            wallHitBuffer.Play(0, PlayFlags.None);
        }

        private SecondarySoundBuffer loadSoundFile(String filename)
        {
            SecondarySoundBuffer sndBuffer = null;
            using (WaveStream wavFile = new WaveStream(Application.StartupPath + "\\Resources\\" + filename))
            {
                SoundBufferDescription sndBufferDesc;
                sndBufferDesc = new SoundBufferDescription();
                sndBufferDesc.SizeInBytes = (int)wavFile.Length;
                sndBufferDesc.Flags = SlimDX.DirectSound.BufferFlags.ControlVolume;
                sndBufferDesc.Format = wavFile.Format;

                sndBuffer = new SecondarySoundBuffer(directSound, sndBufferDesc);

                // now load the sound.
                byte[] wavData = new byte[sndBufferDesc.SizeInBytes];
                wavFile.Read(wavData, 0, (int)wavFile.Length);
                sndBuffer.Write(wavData, 0, LockFlags.None);
            }

            return sndBuffer;
        }

    }
}
