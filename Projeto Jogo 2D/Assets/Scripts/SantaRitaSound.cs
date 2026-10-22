using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

// Foley CC0 de Kenney. Música continua nos sistemas de região aprovados.
public sealed class SantaRitaSound : MonoBehaviour
{
    private static SantaRitaSound instance;
    private readonly AudioSource[] voices = new AudioSource[6];
    private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
    private readonly Dictionary<string, float> lastPlayed = new Dictionary<string, float>();
    private int voice;
    private bool paused;
    public static int EventsPlayed { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        instance = null; EventsPlayed = 0;
        SceneManager.sceneLoaded -= OnScene;
        SceneManager.sceneLoaded += OnScene;
    }
    private static void OnScene(Scene scene, LoadSceneMode mode)
    {
        if (FindAnyObjectByType<PlayerController>() is PlayerController player && player.GetComponent<SantaRitaFootsteps>() == null)
            player.gameObject.AddComponent<SantaRitaFootsteps>();
    }
    private static SantaRitaSound Ensure()
    {
        if (instance != null) return instance;
        var go = new GameObject("SantaRita_Foley"); DontDestroyOnLoad(go);
        instance = go.AddComponent<SantaRitaSound>();
        for (int i = 0; i < instance.voices.Length; i++)
        {
            var source = go.AddComponent<AudioSource>(); source.playOnAwake = false;
            source.spatialBlend = 0; instance.voices[i] = source;
        }
        return instance;
    }
    private void Update()
    {
        bool pause=Time.timeScale==0 || MenuPausa.Aberto;
        if(pause==paused)return;paused=pause;
        foreach(var source in voices)if(source!=null){if(pause)source.Pause();else source.UnPause();}
    }
    public static void Play(string id, float volume = .2f, float pitch = 1, float interval = .055f)
    {
        if (Time.timeScale == 0 || PlayerPrefs.GetFloat("audio_efeitos", 1) <= 0) return;
        var sound = Ensure();
        if (sound.lastPlayed.TryGetValue(id, out float last) && Time.unscaledTime - last < interval) return;
        if (!sound.clips.TryGetValue(id, out var clip))
        { clip = Resources.Load<AudioClip>("Audio/Foley/" + id); sound.clips[id] = clip; }
        if (clip == null) return;
        sound.lastPlayed[id] = Time.unscaledTime;
        var source = sound.voices[sound.voice++ % sound.voices.Length];
        source.Stop(); source.pitch = Mathf.Clamp(pitch, .8f, 1.2f);
        source.clip = clip; source.volume = Mathf.Clamp01(volume * PlayerPrefs.GetFloat("audio_efeitos", 1));
        source.Play(); EventsPlayed++;
    }
}

