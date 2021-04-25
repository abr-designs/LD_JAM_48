using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioController : MonoBehaviour
{
    public enum SFX
    {
        HAND,
        BEEP,
        WATER,
        HIT_WALL,
        PICKUP,
        COMPUTER
    }

    //====================================================================================================================//
    
    
    private const string MASTER_VOLUME = "MasterVolume";
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string SFX_VOLUME = "SFXVolume";


    //====================================================================================================================//

    private static AudioController _instance;

    [SerializeField]
    public List<AudioClip> audioClips;

    [SerializeField]
    private AudioMixer mainMixer;
    
    [SerializeField]
    private AudioSource sfxAudioSource;

    [SerializeField]
    private AudioSource audioSourcePrefab;

    [SerializeField]
    private AudioMixerSnapshot startSnapshot, mainSnapShot;


    //====================================================================================================================//

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        mainMixer.TransitionToSnapshots(
            new []
        {
            startSnapshot,
            mainSnapShot
        },
            new [] {0f,1f},
            1.5f);
    }

    //====================================================================================================================//

    public static void PlaySound(in SFX effect, in float volume = 1f) => _instance?.PlaySoundEffect(effect, volume);
    public static void Volume(in float volume) => _instance?.SetVolume(volume);

    private void PlaySoundEffect(in SFX effect, in float volume = 1f)
    {
        var clip = GetAudioClip(effect);
        sfxAudioSource.PlayOneShot(clip, volume);
    }

    private void SetVolume(in float volume)
    {
        mainMixer.SetFloat(MASTER_VOLUME, Mathf.Lerp(-40, 0, volume));
    }

    private AudioClip GetAudioClip(in SFX effect)
    {
        switch (effect)
        {
            case SFX.HAND:
            case SFX.BEEP:
            case SFX.WATER:
            case SFX.HIT_WALL:
            case SFX.PICKUP:
            case SFX.COMPUTER:
                return audioClips[(int) effect];
            default:
                throw new ArgumentOutOfRangeException(nameof(effect), effect, null);
        }
    }

}
