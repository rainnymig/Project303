using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UnlockSingers : MonoBehaviour
{
    [SerializeField]
    private SongPlayer songPlayer;

    public GameObject[] singers = new GameObject[3];

    private void Awake()
    {
        Assert.IsNotNull(songPlayer);
    }

    private void Update()
    {
        if(songPlayer.CurrentBeat > 256 && !singers[1].activeSelf)
        {
            singers[1].SetActive(true);
        }
        if (songPlayer.CurrentBeat > 448 && !singers[2].activeSelf)
        {
            singers[2].SetActive(true);
        }
    }
}
