using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CarSound : MonoBehaviour
{

    private CarController controller = null;
    private AudioSource audio;
    public AudioClip[] audioClips;

    enum AudioIndex { Static, Accel, Deccel, MaxSpeed, None = -1 };
    AudioIndex audioPlaying;
    private float prevSpeed;

    public float speedOffset;
    private bool canGoToDiffState;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CarController>();
        audio = GetComponent<AudioSource>();

        prevSpeed = controller.Speed;
        canGoToDiffState = false;

        audio.clip = audioClips[(int)AudioIndex.Static];
        audio.Play();
        audioPlaying = AudioIndex.Static;


    }

    // Update is called once per frame
    void Update()
    {
        float speed = controller.Speed;


        if (audioPlaying == AudioIndex.Static)
        {
            if (IsAccelerating(speed))
            {
                PlayAudio(AudioIndex.Accel);
            }

        } else if (audioPlaying == AudioIndex.MaxSpeed)
        {
            if (IsSlowingDown(speed))
            {
                PlayAudio(AudioIndex.Deccel);
            }
        }
        else
        {
            if (IsAccelerating(speed))
            {
                PlayAudio(AudioIndex.Accel);
            }
            if (IsSlowingDown(speed))
            {
                PlayAudio(AudioIndex.Deccel);
            }
            if (canGoToDiffState)
            {
                if (audioPlaying == AudioIndex.Accel)
                    PlayAudio(AudioIndex.MaxSpeed);
                else if (audioPlaying == AudioIndex.Deccel)
                    PlayAudio(AudioIndex.Static);
            }
        }

        prevSpeed = speed;
    }

    void PlayAudio(AudioIndex index)
    {
        if (audioPlaying != index)
        {
            audio.loop = false;
            canGoToDiffState = false;
            audio.clip = audioClips[(int)index];
            audio.Play();
            audioPlaying = index;
            if (index == AudioIndex.Accel || index == AudioIndex.Deccel)
                StartCoroutine(playEngineSound());
            else
                audio.loop = true;
        }
    }

    IEnumerator playEngineSound()
    {
        yield return new WaitForSeconds(audio.clip.length);
        canGoToDiffState = true;
    }

    bool IsAccelerating (float speed)
    {
        return speed - prevSpeed > speedOffset;
    }

    bool IsSlowingDown (float speed)
    {
        return prevSpeed - speed > speedOffset;
    }
}
