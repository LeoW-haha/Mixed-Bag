using UnityEngine;

public class GameMusicManager : MonoBehaviour
{
    public AudioSource musicSource;      // Assign your background music AudioSource
    public AudioClip timesUpClip;        // Assign your 'times up' AudioClip

    // Call this when time is up
    public void OnTimeUp()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            if (timesUpClip != null)
            {
                musicSource.pitch = 1.0f;
                musicSource.clip = timesUpClip;
                musicSource.loop = false;
                musicSource.Play();
            }
        }
    }
} 