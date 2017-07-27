using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<AudioManager>();
            }

            return instance;
        }

    }

    public string audioTrackName = "";

    void Awake ()
    {
        GameManager.Instance.ToIntro += () => PlayMenu();
        TrackManager.Instance.OnTrackCountDown += () => PlayCountDown();
        TrackManager.Instance.OnTrackCountDown += () => StopMusicSpeaker();
        TrackManager.Instance.OnTrackStart += () => PlayTrack();
        TrackManager.Instance.OnTrackFinish += () => PlayGoal();
        TrackManager.Instance.OnTrackOutro += () => PlayOutro();
        TrackManager.Instance.OnPlayerLapCountChange += (lap, trackLaps) =>
        {
            if (trackLaps == 1)
                return;
            if (lap == trackLaps)
                PlayLastLap();
        };
        GameManager.Instance.ToLevelSelector += () => PlayLevelMenu();
    }
    public void PlayValidateButton()
    {
        AudioClip clip = Resources.Load<AudioClip>("AudioClips/buttonValidat");
        AudioSource audioPlayer = GameObject.Find("EventSpeaker").GetComponent<AudioSource>();
        audioPlayer.clip = clip;
        audioPlayer.Play();
    }

    public void PlayBackButton()
    {
        AudioClip clip = Resources.Load<AudioClip>("AudioClips/buttonBack");
        AudioSource audioPlayer = GameObject.Find("EventSpeaker").GetComponent<AudioSource>();
        audioPlayer.clip = clip;
        audioPlayer.Play();
    }

    public void PlayLastLap()
    {
        AudioClip clip = Resources.Load<AudioClip>("AudioClips/finalLap");
        AudioSource audioPlayer = GameObject.Find("EventSpeaker").GetComponent<AudioSource>();
        audioPlayer.clip = clip;
        audioPlayer.Play();
    }

    public void PlayCountDown()
    {
        AudioClip clip = Resources.Load<AudioClip>("AudioClips/countdown");
        AudioSource audioPlayer = GameObject.Find("EventSpeaker").GetComponent<AudioSource>();
        audioPlayer.clip = clip;
        audioPlayer.Play();
    }

    public void PlayRoulette()
    {
        AudioClip clip = Resources.Load<AudioClip>("AudioClips/roulette");
        AudioSource audioPlayer = GameObject.Find("EventSpeaker").GetComponent<AudioSource>();
        audioPlayer.clip = clip;
        audioPlayer.Play();
    }

    public void PlayGotItem()
    {
        AudioClip clip = Resources.Load<AudioClip>("AudioClips/gotItem");
        AudioSource audioPlayer = GameObject.Find("EventSpeaker").GetComponent<AudioSource>();
        audioPlayer.clip = clip;
        audioPlayer.Play();
    }

    public void PlayMenu()
    {
        AudioClip clip = Resources.Load<AudioClip>("AudioClips/menu");
        AudioSource audioPlayer = GameObject.Find("MusicSpeaker").GetComponent<AudioSource>();
        audioPlayer.clip = clip;
        audioPlayer.Play();
    }

    public void PlayLevelMenu()
    {
        StartCoroutine(LoadLevelMenuSound());
    }

    private IEnumerator LoadLevelMenuSound()
    {
        ResourceRequest op = Resources.LoadAsync<AudioClip>("AudioClips/levelMenu");
        yield return op;
        AudioSource audioPlayer = GameObject.Find("MusicSpeaker").GetComponent<AudioSource>();
        audioPlayer.clip = op.asset as AudioClip;
        audioPlayer.Play();
    }

    public void PlayTrack()
    {
        AudioClip clip = Resources.Load<AudioClip>( audioTrackName == "" ? 
            "AudioClips/track1" : "AudioClips/" + audioTrackName);
        AudioSource audioPlayer = GameObject.Find("MusicSpeaker").GetComponent<AudioSource>();
        audioPlayer.clip = clip;
        audioPlayer.Play();
    }

    public void PlayOutro()
    {
        AudioClip clip = Resources.Load<AudioClip>("AudioClips/outro");
        AudioSource audioPlayer = GameObject.Find("MusicSpeaker").GetComponent<AudioSource>();
        audioPlayer.clip = clip;
        audioPlayer.Play();
    }

    public void PlayGoal()
    {
        StopMusicSpeaker();
        AudioClip clip = Resources.Load<AudioClip>("AudioClips/goal");
        AudioSource audioPlayer = GameObject.Find("EventSpeaker").GetComponent<AudioSource>();
        audioPlayer.clip = clip;
        audioPlayer.Play();
    }

    public void StopEventSpeaker()
    {
        AudioSource audioPlayer = GameObject.Find("EventSpeaker").GetComponent<AudioSource>();
        audioPlayer.Stop();
    }

    public void StopMusicSpeaker()
    {
        AudioSource audioPlayer = GameObject.Find("MusicSpeaker").GetComponent<AudioSource>();
        audioPlayer.Stop();
    }

    bool IsSameAudio (AudioSource source, AudioClip clip)
    {
        return source.clip == clip;
    }

    public void PlayThrow (AudioSource source)
    {
        AudioClip clip = Resources.Load<AudioClip>("AudioClips/throw");
        source.clip = clip;
        source.Play();
    }

    public void PlayHit(AudioSource source)
    {
        AudioClip clip = Resources.Load<AudioClip>("AudioClips/spinout");
        source.clip = clip;
        source.Play();
    }

    public void PlayTurbo(AudioSource source)
    {
        AudioClip clip = Resources.Load<AudioClip>("AudioClips/turbo");
        source.clip = clip;
        source.Play();
    }

    public void PlayStarEffect(AudioSource source, float duration)
    {
        AudioClip clip = Resources.Load<AudioClip>("AudioClips/starEffect");
        source.clip = clip;
        source.Play();

        StartCoroutine(StopAudioSourceDelayed(source, duration));
    }

    IEnumerator StopAudioSourceDelayed(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        source.Stop();
    }

    public void SetAudioTrack (string name)
    {
        audioTrackName = name;
    }

}
