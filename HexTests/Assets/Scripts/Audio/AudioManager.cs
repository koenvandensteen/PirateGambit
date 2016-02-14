//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource), typeof(AudioSource))]
public class AudioManager : MonoBehaviour {

    public static AudioManager Instance { get; private set; }

    public AudioMixer MyAudioMixer;
    private Dictionary<string, AudioClip> _soundLibrary;
    public AudioSource EffectAudioSource;
    public AudioSource MusicAudioSource;

    public string MenuMusic;
    void Awake() {
        if (Instance != null && Instance != this) {
            //destroy other instances
            Destroy(gameObject);
            return;
        }

        //Singleton instance
        Instance = this;
        EffectAudioSource = GetComponent<AudioSource>();
        FillLibrary();

        //on't destroy between scenes
        DontDestroyOnLoad(gameObject);

    }

    void Start() {
        UpdateVolumes();
    }

    public void StartMusic() {
        PlayMusic(MenuMusic);
    }
    public void StartMusic(float fadeTime) {
        PlayMusic(MenuMusic, fadeTime);
    }

    public void UpdateVolumes() {
        MyAudioMixer.SetFloat("EffectVolume", PlayerPrefs.GetFloat("EffectVolume"));
        MyAudioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        MyAudioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
    }


    void FillLibrary() {
        _soundLibrary = new Dictionary<string, AudioClip>();
        foreach (var item in Resources.LoadAll<AudioClip>("Audio")) {
            _soundLibrary.Add(item.name, item);
        }
    }

    public void PlaySound(string soundName) {
        PlaySound(soundName, 1);
    }

    public void PlaySound(string soundName, float volScale) {
        int variations = 0;
        new List<string>(_soundLibrary.Keys).ForEach(i => {
            if (i.Contains(soundName))
                ++variations;

        });

        if (variations == 0) {
            Debug.LogErrorFormat("AudioManager::PlaySound >> AudioClip \"{0}\" not found", soundName);
            return;
        }

        int soundIndex = Random.Range(0, variations);

        string filename = string.Format("{0}_{1}", soundName, soundIndex.ToString("00"));

        if (!_soundLibrary.ContainsKey(filename)) {
            return;
        }

        if (EffectAudioSource != null) {
            EffectAudioSource.PlayOneShot(_soundLibrary[filename], volScale);
        }
    }
    
    public void PlayAmbientSfx(string soundName)
    {
        //if the clip is already playing, continue to play.
        if (EffectAudioSource.clip != null && EffectAudioSource.clip.name == soundName)
        {
            if (!EffectAudioSource.isPlaying)
            {
                EffectAudioSource.Play();
            }
            return;
        }

        AudioClip clip;

        if(_soundLibrary.TryGetValue(soundName, out clip))
        {
            if (EffectAudioSource != null)
            {
                EffectAudioSource.clip = clip;
                EffectAudioSource.Play();
            }
        }
    }

    public void StopCurrentAmbientSfx()
    {
        EffectAudioSource.Stop();
    }

    public void PlayMusic(string soundName) {
        AudioClip clip;
        if (_soundLibrary.TryGetValue(soundName, out clip)) {
            if (MusicAudioSource != null) {
                MusicAudioSource.clip = clip;
                MusicAudioSource.Play();
            }
        }
    }
    public void PlayMusic(string soundName, float fadeTime) {
        AudioClip clip;
        if (_soundLibrary.TryGetValue(soundName, out clip)) {
            if (MusicAudioSource != null) {
                MusicAudioSource.clip = clip;
                MusicAudioSource.Play();

                StartCoroutine(MusicFadeIn(fadeTime));
            }
        }
    }

    IEnumerator MusicFadeIn(float seconds) {
        if (seconds == 0) {
            MusicAudioSource.volume = 1;
        } else {
            MusicAudioSource.volume = 0;
            while (MusicAudioSource.volume < 1) {
                MusicAudioSource.volume += Time.deltaTime / seconds;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
