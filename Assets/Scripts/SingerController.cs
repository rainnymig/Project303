using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SingerController : MonoBehaviour
{
    private static readonly int[] beatsArray = { 1, 1, 1, 3, 1, 3, 1, 2, 3, 1, 1, 1, 3, 1, 3, 1, 2, 3 };
    private static readonly int[] cumulatedBeatsArray = { 0, 1, 2, 3, 6, 7, 10, 11, 13, 16, 17, 18, 19, 22, 23, 26, 27, 29, 32 };

    private static readonly float pitchShiftAmount = 0.2f;
    private static readonly float volumeAmpAmount = 0.8f;

    public float BPM = 57;

    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private GameObject noteBarPrefab;
    [SerializeField]
    private GameObject singBarPrefab;

    [SerializeField]
    private Color referenceBarColor;
    [SerializeField]
    private Color singerBarColor;

    [SerializeField]
    private string volumeMixerParam;
    [SerializeField]
    private string pitchMixerParam;
    [SerializeField]
    private SanityCheck sanityChecker;

    public float OutOfControlPossibility = 0.1f;

    private AudioSource audioPlayer;

    [SerializeField]
    private SongPlayer songPlayer;

    [SerializeField]
    private TuningController tuner;

    private int cumulatedBeats = 0;
    private int PhraseIdx = 0;

    public List<SingBar> singBars = new List<SingBar>();

    private void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();
        Assert.IsNotNull(songPlayer);
        Assert.IsNotNull(tuner);
        Assert.IsNotNull(sanityChecker);
        songPlayer.AddGameOverListener(OnGameOver);
        
    }

    private void OnDestroy()
    {
        songPlayer.RemoveGameOverListener(OnGameOver);
    }

    private void LateUpdate()
    {

        if(songPlayer.IsPlaying)
        {
            if(!audioPlayer.isPlaying)
            {
                audioPlayer.Play();
                audioPlayer.time = songPlayer.PositionInSongInMillis / 1000.0f;
            }

            if (songPlayer.HasNewPhrase)
            {
                GameObject obj = Instantiate(noteBarPrefab, transform);
                NoteBar nb = obj.GetComponent<NoteBar>();
                nb.Init(songPlayer.NewPhrase, songPlayer, this, referenceBarColor);

                obj = Instantiate(singBarPrefab, transform);
                SingBar sb = obj.GetComponent<SingBar>();
                SongPhrase singPhrase = new SongPhrase { StartBeat = songPlayer.NewPhrase.StartBeat, 
                    Duration = songPlayer.NewPhrase.Duration, 
                    MidiNote = songPlayer.NewPhrase.MidiNote };

                if (RollOutOfControlDice())
                {
                    //Debug.Log($"Out of control! {PhraseIdx}");
                    singPhrase.PitchShift = getRandomPitchShift();
                    singPhrase.VolumeAmp = getRandomVolumeAmp();
                }
                else
                {
                    //Debug.Log($"In control. {PhraseIdx}");
                    singPhrase.PitchShift = 1.0f;
                    singPhrase.VolumeAmp = 0.0f;
                }
                sb.Init(singPhrase, songPlayer, this, singerBarColor);
                singBars.Add(sb);
                if(!tuner.IsInitialized)
                {
                    tuner.CurrentSingBar = sb;
                }
            }
        }
    }

    private float getRandomPitchShift()
    {
        float rnd = Random.value * 10;

        int levels = (int)(pitchShiftAmount / 0.1f);
        rnd = Mathf.CeilToInt(rnd / (10.0f / (levels + 1)));

        return 1.0f + (rnd - levels / 2) / 10.0f;
    }

    private float getRandomVolumeAmp()
    {
        float rnd = Random.value * 10;

        int levels = (int)(volumeAmpAmount / 0.1f);
        rnd = Mathf.CeilToInt(rnd / (10.0f / (levels + 1)));

        return (rnd - levels / 2) / 10.0f;
    }

    public void PlayPhrase(SongPhrase phrase)
    {
        //Debug.Log($"play out of control phrase: {phrase.VolumeAmp}, {phrase.PitchShift}");
        audioMixer.SetFloat(volumeMixerParam, phrase.VolumeAmp);
        audioMixer.SetFloat(pitchMixerParam, phrase.PitchShift);
        sanityChecker.DropSanity(Mathf.Abs(phrase.VolumeAmp) + Mathf.Abs(phrase.PitchShift - 1.0f));
    }

    private bool RollOutOfControlDice()
    {
        return Random.value < OutOfControlPossibility;
    }

    public int getPrevBarIdxOf(SingBar bar)
    {
        if (singBars == null || singBars.Count == 0)
        {
            return -1;
        }
        int currentIdx = singBars.FindIndex(b => b.StartBeat == bar.StartBeat);
        return currentIdx == 0 || currentIdx == -1 ? 0 : currentIdx - 1;
    }

    public int getNextBarIdxOf(SingBar bar)
    {
        if (singBars == null || singBars.Count == 0)
        {
            return -1;
        }
        int currentIdx = singBars.FindIndex(b => b.StartBeat == bar.StartBeat);
        return currentIdx == singBars.Count - 1 ? currentIdx : currentIdx + 1;
    }

    public void MoveSelectionToNextBar(SingBar bar)
    {
        int nextIdx = getNextBarIdxOf(bar);
        if(nextIdx != -1)
        {
            tuner.CurrentSingBar = singBars[nextIdx];
        }
    }

    private void OnGameOver()
    {
        audioPlayer.Stop();
    }
}
