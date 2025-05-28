using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = System.Numerics.Vector3;

public enum SoundType
{
    Button,
    Beer,
    Drop,
    Pickup,
    Notify,
    Bullet,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Background Music")]
    public AudioClip backgroundClip;

    [Header("Effect Sounds")]
    public AudioClip buttonClip;
    public AudioClip dropClip;
    public AudioClip notifyClip;
    public AudioClip pickupClip;

    [Header("Source")]
    private AudioSource bgmSource;
    private AudioSource sfxSource;


    [Header("Sound Value")]
    public float bgmVolume = 0.5f;
    public float sfxVolume = 1f;

    [Header("Sound Controller")]
    public Slider bgmVolumeSlider;  
    public Slider sfxVolumeSlider;  
    
    private AudioClip? currentMoveClip = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        
        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;

        bgmSource.playOnAwake = false;
        sfxSource.playOnAwake = false;
    }

    private void Start()
    {
        PlayBackground();

        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.value = bgmVolume;
            bgmVolumeSlider.onValueChanged.AddListener(SetBgmVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = sfxVolume;
            sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        }
    }

    public void PlaySound(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Button:
                PlayEffect(buttonClip);
                break;
            case SoundType.Drop:
                PlayEffect(dropClip);
                break;

            case SoundType.Notify:
                PlayEffect(notifyClip);
                break;
            case SoundType.Pickup:
                PlayEffect(pickupClip);
                break;
            default:
                Debug.LogWarning("SoundType not handled: " + soundType);
                break;
        }
    }

    private void PlayBackground()
    {
        if (bgmSource.isPlaying) return;

        bgmSource.clip = backgroundClip;
        bgmSource.Play();
    }
    
    private void PlayEffect(AudioClip clip)
    {
        if (clip == null) return;
    
        sfxSource.loop = false; 
        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(clip); 
    }
    
    
    public void SetBgmVolume(float value)
    {
        bgmVolume = value;
        if (bgmSource != null)
            bgmSource.volume = value;
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = value;

        if (sfxSource != null)
            sfxSource.volume = value;
    }
}