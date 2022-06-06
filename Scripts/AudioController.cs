using System;
using UnityEngine;

public class AudioController : MonoBehaviour
{
	public Sound currentMusic;
	public Sound currentSecondaryMusic;

	public Sound[] musicSounds;
	public Sound[] sounds;

	public static AudioController instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

			DontDestroyOnLoad(gameObject);

			Sound[] array = sounds;
			foreach (Sound sound in array)
			{
				sound.source = gameObject.AddComponent<AudioSource>();
				sound.source.clip = sound.clip;
				sound.source.volume = sound.volume;
				sound.source.pitch = sound.pitch;
				sound.source.loop = sound.looping;
			}

			Sound[] musicArray = musicSounds;
			foreach (Sound sound in musicArray)
			{
				sound.source = gameObject.AddComponent<AudioSource>();
				sound.source.clip = sound.clip;
				sound.source.volume = sound.volume;
				sound.source.pitch = sound.pitch;
				sound.source.loop = sound.looping;
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Play a sound
	public void Play(string name)
	{
		Sound playSound = Array.Find(sounds, (Sound sound) => sound.name == name);
		playSound.source.PlayOneShot(playSound.clip);
	}

	// Play a sound with specified pitch
	// Does not change the pitch of the played sound
	public void PlayWithPitch(string name, float newPitch)
	{
		Sound playSound = Array.Find(sounds, (Sound sound) => sound.name == name);

		// Save old pitch and change to chosen float
		float savePitch = playSound.source.pitch;

		playSound.source.pitch = newPitch;

		// Play with new pitch
		playSound.source.PlayOneShot(playSound.clip);

		// Return to original pitch
		//playSound.source.pitch = savePitch;
	}

	// Set ambient sounds or music. There can only be one sound playing in the background at any time
	// Also ensures that the sound loops
	public void SetMusic(string name)
	{
		Sound playSound = Array.Find(musicSounds, (Sound sound) => sound.name == name);

		if (playSound != null)
		{
			if (currentMusic.name != "")
			{
				currentMusic.source.Stop();
			}

			currentMusic = playSound;

			// ensure that music loops
			currentMusic.source.loop = true;
			currentMusic.source.Play();
		}
		else
		{
			if (currentMusic.name != "")
			{
				currentMusic.source.Stop();
			}
		}

		// Since you're playing only one song, stop secondary music
		if (currentSecondaryMusic.name != "")
		{
			currentSecondaryMusic.source.Stop();
		}
	}

	public void SetMusic(string name, string secondName)
	{
		// Set first track
		SetMusic(name);

		Sound playSound = Array.Find(musicSounds, (Sound sound) => sound.name == secondName);

		if (playSound != null)
		{
			if (currentSecondaryMusic.name != "")
			{
				currentSecondaryMusic.source.Stop();
			}

			currentSecondaryMusic = playSound;

			// ensure that music loops
			currentSecondaryMusic.source.loop = true;
			currentSecondaryMusic.source.Play();
		}
		else
		{
			if (currentSecondaryMusic.name != "")
			{
				currentSecondaryMusic.source.Stop();
			}
		}
	}
}
