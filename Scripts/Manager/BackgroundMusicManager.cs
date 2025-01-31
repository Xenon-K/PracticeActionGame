using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioSource audioSource2;  //using for loop
    public AudioClip backgroundIntro;  // Intro part of background music
    public AudioClip backgroundLoop;   // Loop part of background music
    public AudioClip winningIntro;     // Intro part of winning music
    public AudioClip winningLoop;      // Loop part of winning music
    public AudioClip losingIntro;      // Intro part of losing music
    public AudioClip losingLoop;       // Loop part of losing music

    public float crossfadeDuration = 1f;  // Duration of the crossfade (in seconds)

    private bool isChangingMusic = false;
    private bool isPlayingWinningOrLosing = false;  // Flag to track if winning or losing music is playing
    private bool isBackgroundMusicPlaying = false; // Flag to track if background music is playing

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;  // Start with no looping
        audioSource2.loop = true; //looping section
        PlayBackgroundMusic();    // Play background music at the start
    }

    // Function to start playing the background music
    void PlayBackgroundMusic()
    {
        if (!isPlayingWinningOrLosing && !isBackgroundMusicPlaying)
        {
            StartCoroutine(CrossfadeToTrack(backgroundIntro, backgroundLoop));
            isBackgroundMusicPlaying = true;
        }
    }

    // Function to handle the transition to the background music loop
    IEnumerator CrossfadeToTrack(AudioClip introClip, AudioClip loopClip)
    {
        if (isPlayingWinningOrLosing) yield break; // Don't start background music if we're already in winning/losing state

        audioSource.clip = introClip;
        audioSource2.clip = loopClip;
        audioSource.Play();

        // Wait for the intro to almost finish
        yield return new WaitForSeconds(introClip.length - crossfadeDuration);

        //if already switched clip, drop the loop
        if (audioSource.clip == introClip)
        {
            audioSource2.Play();
        }
    }

    /*
    // Coroutine to play the second audio source after a delay
    IEnumerator PlayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource2.Play();
    }
    */

    // Function for winning state
    public void PlayWinningMusic()
    {
        if (isChangingMusic || isPlayingWinningOrLosing) return; // Prevent starting a new transition if one is already in progress
        isChangingMusic = true;
        isPlayingWinningOrLosing = true;  // Set flag to prevent background music switch
        StopBackgroundMusic(); // Immediately stop the background music
        StartCoroutine(SwitchToTrack(winningIntro, winningLoop));
    }

    // Function for losing state
    public void PlayLosingMusic()
    {
        if (isChangingMusic || isPlayingWinningOrLosing) return; // Prevent starting a new transition if one is already in progress
        isChangingMusic = true;
        isPlayingWinningOrLosing = true;  // Set flag to prevent background music switch
        StopBackgroundMusic(); // Immediately stop the background music
        StartCoroutine(SwitchToTrack(losingIntro, losingLoop));
    }

    // Switch to a different track (for winning/losing)
    IEnumerator SwitchToTrack(AudioClip introClip, AudioClip loopClip)
    {
        // Set the new intro music and play it
        audioSource.clip = introClip;
        audioSource2.clip = loopClip;
        audioSource.Play();

        // Wait until the intro finishes or crossfade is done
        yield return new WaitForSeconds(introClip.length);
        if(audioSource.clip == introClip)
        {
            audioSource2.Play();
        }
        isChangingMusic = false;
    }

    // Stop the background music completely
    void StopBackgroundMusic()
    {
        audioSource.Stop();
        audioSource2.Stop();

        // Reset the flags to prevent any background music interference
        isBackgroundMusicPlaying = false;
    }

    // Call this when the game is over or music should return to background music
    public void StopWinningOrLosingMusic()
    {
        if (!isPlayingWinningOrLosing) return;

        isPlayingWinningOrLosing = false;  // Reset the flag
        isBackgroundMusicPlaying = false; // Allow background music to play again
        PlayBackgroundMusic();  // Play the background music again
    }
}