using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeManager : MonoBehaviour {

    private static AudioVolumeManager instance;
    private float musicVolume = 0.75f;
    private float soundEffectVolume = 0f;
    private float voiceVolume = 0.85f;
    private string fileName = "volumeData.dat";
    [NonSerialized] public AudioSource ouroborosAudioSource;

    [SerializeField] private Sound[] themes;
    [SerializeField] private Sound[] soundEffects;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static AudioVolumeManager GetInstance()
    {
        if (instance == null)
        {
            Debug.Log("No instance of " + typeof(AudioVolumeManager));
            return null;
        }
        return instance;
    }

    public void PlayTheme(string name)
    {
        Sound t = Array.Find(themes, theme => theme.name == name);
        if(t == null)
        {
            Debug.Log("Theme " + name + " not found");
            return;
        }
        if (t.source.isPlaying)
        {
            return;
        }
        foreach(Sound s in themes)
        {
            s.source.Stop();
        }
        t.source.Play();
    }

    public void PlaySoundEffect(string name)
    {
        Sound t = Array.Find(soundEffects, sound => sound.name == name);
        if (t == null)
        {
            Debug.Log("Sound effect " + name + " not found");
            return;
        }
        t.source.Play();
    }
    public void PlaySoundEffectRandomPitch(string name)
    {
        Sound t = Array.Find(soundEffects, sound => sound.name == name);
        if (t == null)
        {
            Debug.Log("Sound effect " + name + " not found");
            return;
        }
        t.source.pitch = UnityEngine.Random.Range(0.8f, 1.4f);
        t.source.Play();
    }

    public IEnumerator FadeTheme(string newTheme, float fadeDuration)
    {
        Sound newT = Array.Find(themes, theme => theme.name == newTheme);
        if (newT == null)
        {
            Debug.LogWarning("Sound effect " + name + " not found");
            yield break;
        }
        AudioSource current = GetCurrentTheme();
        if(current == null || current == newT.source)
        {
            Debug.LogWarning("No theme playing");
            yield break;
        }
        newT.source.volume = 0.0f;
        newT.source.Play();
        float step = musicVolume / fadeDuration;
        while (newT.source.volume <= musicVolume)
        {
            current.volume -= step * Time.unscaledDeltaTime;
            newT.source.volume += step * Time.unscaledDeltaTime;
            yield return null;
        }
        current.Stop();
        current.volume = musicVolume;
        newT.source.volume = musicVolume;

        

    }
    private void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.OpenOrCreate);
        VolumeData data = new VolumeData(musicVolume, soundEffectVolume, voiceVolume);
        bf.Serialize(file, data);
        file.Close();
    }

    private void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/" + fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
            VolumeData data = (VolumeData) bf.Deserialize(file);
            file.Close();
            musicVolume = data.music;
            soundEffectVolume = data.soundEffect;
            voiceVolume = data.voice;
        }
    }
    
    private void OnEnable()
    {
        /*Load();
        foreach (Sound m in themes)
        {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.clip;
            m.source.pitch = m.pitch;
            m.source.volume = musicVolume;
            m.source.loop = m.loop;
            m.source.spatialBlend = 0.0f;
        }
        foreach (Sound s in soundEffects)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            if(s.name == "Ouroboros")
            {
                ouroborosAudioSource = s.source;
            }
            s.source.clip = s.clip;
            s.source.pitch = s.pitch;
            s.source.volume = soundEffectVolume;
            s.source.spatialBlend = 0.0f;
        }*/
    }

    private void OnDisable()
    {
        //Save();
    }

    public float MusicVolume
    {
        get
        {
            return musicVolume;
        }
        set
        {
            musicVolume = value;
            foreach (Sound m in themes)
            {
                m.source.volume = musicVolume;
            }
        }
    }

    public float SoundEffectVolume
    {
        get
        {
            return soundEffectVolume;
        }
        set
        {
            soundEffectVolume = value;
            foreach (Sound m in soundEffects)
            {
                m.source.volume = soundEffectVolume;
            }
        }
    }

    public float VoiceVolume
    {
        get
        {
            return voiceVolume;
        }
        set
        {
            voiceVolume = value;
        }
    }

    public AudioSource GetCurrentTheme()
    {
        foreach(Sound t in themes)
        {
            if (t.source.isPlaying)
            {
                return t.source;
            }
        }
        return null;
    }
}

[Serializable]
class VolumeData
{
    public float music;
    public float soundEffect;
    public float voice;

    public VolumeData(float m, float s, float v)
    {
        music = m;
        soundEffect = s;
        voice = v;
    }
}
