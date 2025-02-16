using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;
using UnityEngine.Events;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
    {
    public static SoundManager instance;

    [Header("Audio Mixers")]
    [SerializeField] private AudioMixerGroup mixerMusic;
    [SerializeField] private AudioMixerGroup mixerFX;

    [SerializeField] private List<PhysicMaterial> physicsMaterials = new List<PhysicMaterial>();
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();


    private Dictionary<PhysicMaterial, AudioClip> hitClips = new Dictionary<PhysicMaterial, AudioClip>();

    //El pool es para tener varios audioSources y poder emitir varios sonidos a la vez, no creamos y destruimos objetos porque es menos eficiente.
    [SerializeField] private int poolSize = 10; // Número de AudioSources en el pool
    private Queue<AudioSource> audioSourcePool;


    [Header("Settings Stuff")]
    [SerializeField] private AudioMixerGroup audioMixer;
    [SerializeField] private Slider sliderVolume;

    private void Awake()
        {
        if(instance == null)instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        }

    private void Start()
        {
        audioMixer.audioMixer.SetFloat("Volume", PlayerPrefs.GetFloat("Volume", 50));
        if (sliderVolume) sliderVolume.value = PlayerPrefs.GetFloat("Volume", 50);

        for (int i = 0; i < physicsMaterials.Count; i++)
            {
            hitClips.Add(physicsMaterials[i], clips[i]); 
            }
        CreateAudioSourcePool();

        StartCoroutine(Wait(.2f));  //No fucking idea why I put this
        }

    private IEnumerator Wait(float time)
        {
        yield return new WaitForSeconds(time);
        }

    public void PlaySound(PhysicMaterial key, bool modPitch,bool music)
        {
        AudioSource audioSourceToUse = null;
        
        if (key != null && hitClips.TryGetValue(key, out AudioClip clip))
            {
            if (audioSourcePool.Count > 0)
                {
                audioSourceToUse = audioSourcePool.Dequeue();

                if (music) audioSourceToUse.outputAudioMixerGroup = mixerMusic;
                else audioSourceToUse.outputAudioMixerGroup = mixerFX;

                if (modPitch) audioSourceToUse.pitch = UnityEngine.Random.Range(.8f, 1.2f);
                audioSourceToUse.PlayOneShot(clip);
                }
            StartCoroutine(ReturnToPoolAfterPlayback(audioSourceToUse));
            }
        else
            {
            Debug.LogWarning($"No se encontró un AudioClip con la clave: {key}");
            }
        }

    public void PlaySound(AudioClip key, bool modPitch)
        {
        AudioSource audioSourceToUse = null;

        if (key != null)
            {
            if (audioSourcePool.Count > 0)
                {
                audioSourceToUse = audioSourcePool.Dequeue();
                if (modPitch) audioSourceToUse.pitch = UnityEngine.Random.Range(.8f, 1.2f);
                audioSourceToUse.PlayOneShot(key);
                }
            StartCoroutine(ReturnToPoolAfterPlayback(audioSourceToUse));
            }
        else
            {
            Debug.LogWarning($"No se encontró un AudioClip con la clave: {key}");
            }
        }

    private void CreateAudioSourcePool()
        {
        audioSourcePool = new Queue<AudioSource>();

        for (int i = 0; i < poolSize; i++)
            {
            // Crear un nuevo GameObject para cada AudioSource
            GameObject audioObject = new GameObject($"AudioSource_{i}");
            audioObject.transform.parent = transform;

            // Agregar un componente AudioSource
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.playOnAwake = false;

            // Añadirlo a la cola
            audioSourcePool.Enqueue(source);
            }
        }

    private IEnumerator ReturnToPoolAfterPlayback(AudioSource source)
        {
        // Esperar hasta que termine el sonido
        yield return new WaitWhile(() => source.isPlaying);

        // Limpiar el AudioSource
        source.clip = null;

        // Devolverlo al pool
        audioSourcePool.Enqueue(source);
        }

    public void Volume(float volume)
        {
        audioMixer.audioMixer.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("Volume", volume);
        }



    }
