using System;
using System.Linq;
using UnityEngine;

namespace Resources.Scripts
{
   public class SoundManager : MonoBehaviour
   {
      
      public static SoundManager instance;
      
      public Sound[] SoundEffects;

      public Sound[] BackgroundMusics;
      private void Awake()
      {
         if (instance != null)
         {
            Destroy(gameObject);
            return;
         }
         instance = this;
         DontDestroyOnLoad(gameObject);
         
         // Initializing all the sounds
         foreach (var sound in SoundEffects.Concat(BackgroundMusics))
         {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
         }
      }

      public void SetBGMVolume(float volume)
      {
         foreach (var sound in BackgroundMusics)
         {
            sound.source.volume = volume;
         }
         print("Volume BGM setado para " + volume);
      }

      // ReSharper disable once InconsistentNaming
      public void SetSFXVolume(float volume)
      {
         foreach (var sound in SoundEffects)
         {
            sound.source.volume = volume;
         }
         print("Volume SFX setado para " + volume);
      }

      public void Play(string name)
      {
         var sound = Array.Find(SoundEffects.Concat(BackgroundMusics).ToArray(), sound => sound.name == name);
         if (sound == null)
         {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
         }
         sound.source.Play();
      }

      public void PlayBGM(string name)
      {
         foreach (var music in BackgroundMusics)
         {
            if (music.name == name)
            {
               music.source.Play();
            }
            else
               music.source.Pause();
         }
      }

      public void UnPauseBGM(string name)
      {
         foreach (var music in BackgroundMusics)
         {
            if (music.name == name)
            {
               music.source.UnPause();
            }
            else
               music.source.Pause();
         }
      }

      public void StopMusic()
      {
         foreach (var music in BackgroundMusics)
         {
            music.source.Stop();
         }
      }
   }
}