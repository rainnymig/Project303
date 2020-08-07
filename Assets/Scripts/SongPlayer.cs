using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class SongPlayer : MonoBehaviour
{
    public float BPM = 57;
    public string SongName = "SummerWind";

    [SerializeField]
    private AudioSource mainPlayer;

    [HideInInspector]
    public SongPhrase[] SongPhrases;
    [HideInInspector]
    public int PhraseIdx = 0;
    [HideInInspector]
    public bool HasNewPhrase = false;
    [HideInInspector]
    public SongPhrase NewPhrase;

    [SerializeField]
    private GameObject GameOverPanel;
    [SerializeField]
    private TextMeshProUGUI GameOverText;

    private UnityEvent gameOverEvent = new UnityEvent();

    private bool songStarted = false;

    private int positionInSongInMillisFrame;
    private float positionInSongInMillis;
    public float PositionInSongInMillis
    {
        get
        {
            if (mainPlayer == null || mainPlayer.clip == null)
            {
                return 0;
            }
            // The samples of an AudioClip change concurrently,
            // even when they are queried in the same frame (e.g. Update() of different scripts).
            // For a given frame, the position in the song should be the same for all scripts,
            // which is why the value is only updated once per frame.
            if (positionInSongInMillisFrame != Time.frameCount)
            {
                positionInSongInMillisFrame = Time.frameCount;
                positionInSongInMillis = 1000.0f * mainPlayer.timeSamples / mainPlayer.clip.frequency;
            }
            return positionInSongInMillis;
        }
    }
    public float CurrentBeat
    {
        get
        {
            if (mainPlayer.clip == null)
            {
                return 0;
            }
            else
            {
                float millisInSong = PositionInSongInMillis;
                float result = MillisecondInSongToBeat(BPM, millisInSong);
                if (result < 0)
                {
                    result = 0;
                }
                return (float)result;
            }
        }
    }
    public bool IsPlaying
    {
        get
        {
            return mainPlayer.isPlaying;
        }
    }


    public void StartSong()
    {
        mainPlayer.Play();
        songStarted = true;
    }

    public void StopSong()
    {
        mainPlayer.Stop();
    }


    private void Awake()
    {
        Assert.IsNotNull(mainPlayer);
        loadSong(SongName);
    }

    private void Start()
    {
        StartSong();
    }

    private void Update()
    {
        if(IsPlaying)
        {
            float beat = CurrentBeat;
            if(PhraseIdx < SongPhrases.Length && beat > SongPhrases[PhraseIdx].StartBeat-NoteBar.NoteTravelBeats)
            {
                HasNewPhrase = true;
                NewPhrase = SongPhrases[PhraseIdx];
                PhraseIdx++;
            }
            else
            {
                HasNewPhrase = false;
            }
        }
        else
        {
            if(songStarted)
            {
                GameOver();
            }
        }
    }

    public static float MillisecondInSongToBeat(float BPM, float ms)
    {
        float beatsPerMinute = BPM * 4;
        float result = beatsPerMinute * ms / 1000.0f / 60.0f;
        return result;
    }

    private void loadSong(string songName)
    {
        TextAsset songText = Resources.Load<TextAsset>($"SongText/{songName}");
        string[] lines = songText.text.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        SongPhrases = new SongPhrase[lines.Length-1];
        for (int idx = 0; idx < lines.Length; idx++)
        {
            if(lines[idx].StartsWith("#BPM"))
            {
                BPM = float.Parse(lines[idx].Split(':')[1]);
            }
            else
            {
                string[] words = lines[idx].Split(',');
                SongPhrase p = new SongPhrase { StartBeat = int.Parse(words[0]), Duration = int.Parse(words[1]), MidiNote = int.Parse(words[2]) };
                SongPhrases[idx-1] = p;
            }
        }
    }

    public void AddGameOverListener(UnityAction listener)
    {
        gameOverEvent.AddListener(listener);
    }

    public void RemoveGameOverListener(UnityAction listener)
    {
        gameOverEvent.RemoveListener(listener);
    }

    public void GameOver()
    {
        mainPlayer.Stop();

        SanityCheck sanityChecker = FindObjectOfType<SanityCheck>();
        if(sanityChecker.SanityVal > 0.0f)
        {
            GameOverText.text = "Congrats! You keep the audience in control!";
        }
        else
        {
            GameOverText.text = "The audience lose all of their sanity...";
        }

        GameOverPanel.SetActive(true);

        gameOverEvent.Invoke();
    }
}