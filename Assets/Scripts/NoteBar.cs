using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBar : MonoBehaviour
{
    public static float SingPosX = 7.5f;
    public static float BoardcastPosX = -7.5f;
    public static float NoteTravelBeats = 48;
    public const int CENTER_MIDI_NOTE = 73;

    private SpriteRenderer spriteRenderer;

    private SongPhrase phrase;

    private Vector3 singPos, boardcastPos;

    private SongPlayer songPlayer;
    private SingerController singerController;

    private bool initialized = false;

    public void Init(SongPhrase p, SongPlayer sp, SingerController sc, Color barColor)
    {
        phrase = p;
        singPos = new Vector3(SingPosX, (phrase.MidiNote - CENTER_MIDI_NOTE) * 0.1f, -1);
        boardcastPos = new Vector3(BoardcastPosX, (phrase.MidiNote - CENTER_MIDI_NOTE) * 0.1f, -1);
        transform.localPosition = singPos;
        transform.localScale = new Vector3(phrase.Duration - 0.2f, 2, 1);
        songPlayer = sp;
        singerController = sc;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = barColor;
        initialized = true;
    }

    private void Update()
    {
        if(initialized)
        {
            float beat = songPlayer.CurrentBeat;
            transform.localPosition = Vector3.LerpUnclamped(singPos, boardcastPos, (beat-phrase.StartBeat+NoteTravelBeats)/NoteTravelBeats);

            if(transform.localPosition.x < BoardcastPosX-4)
            {
                Destroy(gameObject);
            }
        }
    }
}
