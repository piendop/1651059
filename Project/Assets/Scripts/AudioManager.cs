using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

	private AudioSource audioSource;

    [Range(0f, 1f)]
	public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;

    [Range(0f, 0.5f)]
    public float randomValue = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;
    public void SetSource(AudioSource _source)
	{
		audioSource = _source;
		audioSource.clip = clip;
	}

    public void Play()
	{
		audioSource.volume = volume * (1 + Random.Range(-randomValue/2f, randomValue/2f));
		audioSource.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        audioSource.Play();
	}
    public void Stop()
    {
     
        audioSource.Stop();
    }
}
public class AudioManager : MonoBehaviour
{
 
    [SerializeField]
	Sound[] sounds;
   
    private void Start()
    {
        Debug.Log("audio manager start");
        for (int i = 0; i< sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }

    }

    public void PlaySound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if(sounds[i].name == _name)
            {
                sounds[i].Play();
                return;
            }
        }
        // no sound with that name
//        Debug.LogWarning("AudioManager sound not found: "+ _name);
    }

    public void StopSound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Stop();
                return;
            }
        }
        // no sound with that name
        Debug.LogWarning("AudioManager sound not found: " + _name);
    }
}
