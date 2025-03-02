using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour {
    public AudioSource audio_Bgm;
    public AudioSource audio_Sfx;

    [Header("Audio Clip")]
    [SerializeField]
    private AudioClip clip_BGM;
    [SerializeField]
    private AudioClip clip_ButtonHighlight;
    [SerializeField]
    private AudioClip clip_ButtonClicked;
    [SerializeField]
    private AudioClip clip_Correct;
    [SerializeField]
    private AudioClip clip_False;
    [SerializeField]
    private AudioClip clip_Alarm;
    [SerializeField]
    private AudioClip clip_Valve;

    public static AudioHandler Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }
    }

    public void PlayAudioBgm(AudioClip clip) {
        if (clip == null) {
            Debug.LogWarning("Audio clip not found!");
            return;
        }
        audio_Bgm.clip = clip;
        audio_Bgm.Play();
    }

    public void StopAudioBgm() {
        audio_Bgm.Stop();
    }

    public void PlayAudioSfx(AudioClip clip) {
        if (clip == null) {
            Debug.LogWarning("Audio clip not found!");
            return;
        }
        audio_Sfx.clip = clip;
        audio_Sfx.Play();
    }

    public void ChangeBGMVolume(float _volume)
    {
        audio_Bgm.volume = Mathf.Clamp(_volume, 0, 1) ;
    }

    public void StopAudioSfx() {
        audio_Sfx.Stop();
    }

    public void PlayBGM()
    {
        PlayAudioBgm(clip_BGM);
    }

    public void PlayButtonHighlight() {
        PlayAudioSfx(clip_ButtonHighlight);
    }

    public void PlayButtonClicked() {
        PlayAudioSfx(clip_ButtonClicked);
    }

    public void PlayCorrect() {
        PlayAudioSfx(clip_Correct);
    }

    public void PlayFalse() {
        PlayAudioSfx(clip_False);
    }

    public void PlayAlarm() {
        PlayAudioSfx(clip_Alarm);
    }

    public void PlayValve()
    {
        if (!audio_Sfx.isPlaying)
        PlayAudioSfx(clip_Valve);
    }
}
