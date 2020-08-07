using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingBar : MonoBehaviour
{
    public enum ModAction
    {
        Up, Down,
    }

    public static float SingPosX = 7.5f;
    public static float BoardcastPosX = -7.5f;
    public static float NoteTravelBeats = 48;
    public const int CENTER_MIDI_NOTE = 73;

    [SerializeField]
    private float pitchYOffsetAmount = 0.8f;
    [SerializeField]
    private float volumeYSizeAmount = 4.0f;

    [SerializeField]
    private Color selectedColor;
    private Color normalColor;

    private SpriteRenderer spriteRenderer;

    private SongPhrase phrase;
    public int StartBeat;

    private Vector3 singPos, boardcastPos;

    private SongPlayer songPlayer;
    private SingerController singerController;

    private bool initialized = false;
    private bool played = false;

    private bool selected = false;
    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            if(spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            spriteRenderer.color = selected ? selectedColor : normalColor;
        }
    }

    public void Init(SongPhrase p, SongPlayer sp, SingerController sc, Color barColor)
    {
        phrase = p;
        StartBeat = phrase.StartBeat;
        float posY = (phrase.MidiNote - CENTER_MIDI_NOTE) * 0.1f;
        posY += pitchYOffsetAmount * (phrase.PitchShift - 1.0f);
        singPos = new Vector3(SingPosX, posY, -2);
        boardcastPos = new Vector3(BoardcastPosX, posY, -2);
        transform.localPosition = singPos;
        transform.localScale = new Vector3(phrase.Duration - 0.2f, 2.0f + phrase.VolumeAmp * volumeYSizeAmount, 1);
        songPlayer = sp;
        singerController = sc;
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        normalColor = barColor;
        spriteRenderer.color = barColor;
        initialized = true;
    }

    private void Update()
    {
        if (initialized)
        {
            float beat = songPlayer.CurrentBeat;
            transform.localPosition = Vector3.LerpUnclamped(singPos, boardcastPos, (beat - phrase.StartBeat + NoteTravelBeats) / NoteTravelBeats);

            if(transform.localPosition.x < BoardcastPosX && !played)
            {
                singerController.PlayPhrase(phrase);
                if(selected)
                {
                    singerController.MoveSelectionToNextBar(this);
                }
                played = true;
                
                singerController.singBars.RemoveAt(0);
            }
            if (transform.localPosition.x < BoardcastPosX - 4)
            {
                Destroy(gameObject);
            }
        }
    }

    public void ModifyPitch(ModAction act)
    {
        if(played)
        {
            return;
        }
        if(act == ModAction.Up)
        {
            phrase.PitchShift += 0.1f;
        }
        else
        {
            phrase.PitchShift -= 0.1f;
        }
        float posY = (phrase.MidiNote - CENTER_MIDI_NOTE) * 0.1f;
        posY += pitchYOffsetAmount * (phrase.PitchShift - 1.0f);
        singPos = new Vector3(SingPosX, posY, -2);
        boardcastPos = new Vector3(BoardcastPosX, posY, -2);
    }

    public void ModifyVolume(ModAction act)
    {
        if (played)
        {
            return;
        }
        if (act == ModAction.Up)
        {
            phrase.VolumeAmp += 0.1f;
        }
        else
        {
            phrase.VolumeAmp -= 0.1f;
        }
        transform.localScale = new Vector3(phrase.Duration - 0.2f, 2.0f + phrase.VolumeAmp * volumeYSizeAmount, 1);

    }
}
