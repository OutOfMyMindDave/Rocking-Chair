using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour
{
    public AudioSource changingStations;
    public AudioSource currentSongSource;
    public List<AudioClip> songs;

    private int currentSong;

    private void OnTriggerEnter(Collider other)
    {
        if(changingStations.isPlaying == false)
        {
            changingStations.Play();

            if(songs.Count > 0)
            {
                currentSong = (currentSong + 1) % songs.Count;
                currentSongSource.clip = songs[currentSong];
                currentSongSource.PlayDelayed(2);
            }
        }
    }
}
