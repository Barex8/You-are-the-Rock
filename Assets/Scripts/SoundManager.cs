using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour
    {
    public static SoundManager instance;

    [SerializeField] private List<PhysicMaterial> physicsMaterials = new List<PhysicMaterial>();
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();


    private Dictionary<PhysicMaterial, AudioClip> hitClips = new Dictionary<PhysicMaterial, AudioClip>();

    //El pool es para tener varios audioSources y poder emitir varios sonidos a la vez, no creamos e destruimos objetos porque es menos eficiente.
    [SerializeField] private int poolSize = 10; // Número de AudioSources en el pool
    private Queue<AudioSource> audioSourcePool;

    private void Awake()
        {
        instance = this;
        DontDestroyOnLoad(gameObject);
        }

    private void Start()
        {

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

    public void PlaySound(PhysicMaterial key, bool modPitch)
        {
        AudioSource audioSourceToUse = null;
        
        if (key != null && hitClips.TryGetValue(key, out AudioClip clip))
            {
            if (audioSourcePool.Count > 0)
                {
                audioSourceToUse = audioSourcePool.Dequeue();
                if (modPitch) audioSourceToUse.pitch = UnityEngine.Random.Range(.5f, 1.5f);
                audioSourceToUse.PlayOneShot(clip);
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



    }
