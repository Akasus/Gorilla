using UnityEngine;

//Warning! This only works properly in 2D games!
//It's like a Spotted Radio in 3D

//The Idea behind this Script is to Avoid more than one AudioSource in a
//2D related Game and also allows better control over the Audio.
//The most important thing of the Idea was the Point that Objects who were destroyed stop the
//Audio immediately. To avoid this the GameObject which holds the AudioContainer also takes the
//AudioSource and is always active.
//So no one can disturb the Audio from being played!

namespace Audio
{


    public class AudioContainer : MonoBehaviour
    {
        public static AudioContainer Main;

        /// <summary>
        ///     The Position of the AudioClip for the Projectile in the Array.
        /// </summary>
        [SerializeField] private int projectileSoundIndex;

        /// <summary>
        ///     This Array stores all AudioClips you want to use in the Scene!
        /// </summary>
        public AudioClip[] sounds;

        /// <summary>
        ///     The Main Audio Source the AudioContainer will use.
        /// </summary>
        private AudioSource _thisAudio;

        // Start is called before the first frame update
        private void Start()
        {
            _thisAudio = GetComponent<AudioSource>();
            Main = this;
        }

        /// <summary>
        ///     Plays the Sound with the given index of the Array once
        /// </summary>
        /// <param name="index">The Position of the Track in the AudioContainer</param>
        public void PlaySoundWithIndex(int index)
        {
            _thisAudio.PlayOneShot(sounds[index]);
        }

        /// <summary>
        ///     Starts playing the Projectile Sound repeatedly, wont stop unless told otherwise!
        /// </summary>
        public void PlayProjectileLoopSound()
        {
            _thisAudio.clip = sounds[projectileSoundIndex];
            _thisAudio.loop = true;
            _thisAudio.Play();
        }

        /// <summary>
        ///     This Stops the repeating Projectile Sound immediately!
        /// </summary>
        public void StopProjectileLoopSound()
        {
            _thisAudio.loop = false;
            _thisAudio.Stop();
        }
    }
}