/*
 * This code is part of Arcade Car Physics for Unity by Saarg (2018)
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

using UnityEngine;

namespace VehicleBehaviour {
    [RequireComponent(typeof(CarController))]

    public class EngineSoundManager : MonoBehaviour {

        [Header("AudioClips")]
        public AudioClip starting;
        public AudioClip rolling;
        public AudioClip stopping;
        public AudioClip[] songs;
        public AudioClip[] soundEffects;

        [Header("pitch parameter")]
        public float flatoutSpeed = 20.0f;
        [Range(0.0f, 3.0f)]
        public float minPitch = 0.7f;
        [Range(0.0f, 0.1f)]
        public float pitchSpeed = 0.05f;

        private AudioSource source;
        private AudioSource radioSource;
        private AudioSource effectsSource;
        
        private CarController vehicle;
        
        [SerializeField] private KeyCode changeSong = KeyCode.P;
        [SerializeField] private KeyCode muteRadio = KeyCode.M;

        private int rnd, currentsong;
        private float volume;
        
        private void Start () {
            source = gameObject.AddComponent<AudioSource>();
            radioSource = gameObject.AddComponent<AudioSource>();
            effectsSource = gameObject.AddComponent<AudioSource>();

            source.volume = 0.2f;
            radioSource.volume = 0.1f;
            effectsSource.volume = 0.1f;
            effectsSource.playOnAwake = false;
            
            vehicle = GetComponent<CarController>();
            rnd = Random.Range(0, 22);
            currentsong = rnd;
            MuteSounds(true);
        }
        
        private void Update () {

            if (!source.mute)
            {
                if (source.clip == stopping || !source.clip)
                {
                    PlayClip(source, starting);
                }

                PlayClip(source, rolling);

                if (source.clip == rolling || source.clip == starting)
                {
                    source.pitch = Mathf.Lerp(source.pitch, minPitch + Mathf.Abs(vehicle.Speed) / flatoutSpeed, pitchSpeed);
                }

                if (!radioSource.mute)
                {
                    if (vehicle.Speed > 2 && !radioSource.isPlaying)
                    {
                        PlayClip(radioSource, songs[currentsong]);
                        volume = radioSource.volume;
                    }
                    else if (vehicle.Speed > 2 && radioSource.isPlaying)
                    {
                        radioSource.volume = volume;
                    } 
                    else if (vehicle.Speed == 0 && radioSource.isPlaying)
                    {
                        radioSource.volume = 0;
                    }
                    else if (vehicle.Speed < 0 && radioSource.isPlaying)
                    {
                        radioSource.volume = volume / 3;
                    }
                
                    if(Input.GetKeyDown(changeSong) || Input.GetKeyDown(KeyCode.Joystick1Button8))
                    {
                        radioSource.Stop();
                        currentsong = Random.Range(0, 22);
                        radioSource.clip = songs[currentsong];
                        radioSource.Play();
                    }
                }

                if(Input.GetKeyDown(muteRadio))
                {
                    radioSource.mute = !radioSource.mute;
                }
            }
        }
        
        public void MuteSounds(bool enable)
        {
            radioSource.mute = enable;
            source.mute = enable;
            source.clip = enable ? stopping : source.clip;
            effectsSource.mute = enable;
        }

        public void BrakeSound()
        {
            PlayClip(effectsSource, soundEffects[0]);
        }
        
        public void JumpSound()
        {
            PlayClip(effectsSource, soundEffects[1]);
        }

        private void PlayClip(AudioSource audioSource, AudioClip clip)
        {
            if (!audioSource.isPlaying || audioSource.clip != clip)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

    }
}
