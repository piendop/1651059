using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField]
    private GameObject starPrefab;

    [SerializeField]
    private Text starText;

    [SerializeField]
    private AudioSource audioSource;

    public GameObject completeLevelUI;
    public GameObject player;
    private int collectedStars;
    public static GameManager Instance {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
        set => instance = value;
    }

    public GameObject StarPrefab { get => starPrefab; set => starPrefab = value; }
    public int CollectedStars {
        get => collectedStars;
        set
        {
            starText.text = value.ToString();
            collectedStars = value;
        }
    }

    private void Awake()
    {
        BackgroundScript.FindObjectOfType<AudioSource>().Pause();
    }
    // Start is called before the first frame update
    void Start()
    {
        audioSource.Play();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (collectedStars >= 3)
        {
            CompleteLevel();
        }
    }

    public void CompleteLevel()
    {
        completeLevelUI.SetActive(true);
        player.SetActive(false);
    }
}
