using UnityEngine;

public class TuningController : MonoBehaviour
{
    [SerializeField]
    private SingerController[] singerControllers = new SingerController[3];

    private int singerIdx = 0;

    private SongPlayer songPlayer;

    private SingBar currentSingBar;
    public SingBar CurrentSingBar
    {
        get => currentSingBar;
        set
        {
            if(currentSingBar != null)
            {
                currentSingBar.Selected = false;
            }
            currentSingBar = value;
            currentSingBar.Selected = true;
            IsInitialized = true;
        }
    }

    [HideInInspector]
    public bool IsInitialized = false;

    private void Awake()
    {
        songPlayer = FindObjectOfType<SongPlayer>();
    }


    private void Update()
    {
        if (!songPlayer.IsPlaying || CurrentSingBar == null)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            //  prev singer
            selectNextSinger();
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            //  next singer
            selectPrevSinger();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //  prev bar
            selectPrevBar();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //  next bar
            selectNextBar();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            //  pitch up
            CurrentSingBar?.ModifyPitch(SingBar.ModAction.Up);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            //  pitch down
            CurrentSingBar?.ModifyPitch(SingBar.ModAction.Down);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //  volume up
            CurrentSingBar?.ModifyVolume(SingBar.ModAction.Up);
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            //  volume down
            CurrentSingBar?.ModifyVolume(SingBar.ModAction.Down);
        }
    }

    private void selectPrevSinger()
    {
        do
        {
            singerIdx = (singerIdx + 1) % singerControllers.Length;
            Debug.Log(singerIdx);
        } while (singerControllers[singerIdx] == null || !singerControllers[singerIdx].isActiveAndEnabled);
        //CurrentSingBar.Selected = false;
        CurrentSingBar = singerControllers[singerIdx].singBars[1];
        //CurrentSingBar.Selected = true;
    }

    private void selectNextSinger()
    {
        do
        {
            singerIdx = (singerIdx + singerControllers.Length - 1) % singerControllers.Length;
            Debug.Log(singerIdx);

        } while (singerControllers[singerIdx] == null || !singerControllers[singerIdx].isActiveAndEnabled);
        //CurrentSingBar.Selected = false;
        CurrentSingBar = singerControllers[singerIdx].singBars[1];
        //CurrentSingBar.Selected = true;
    }

    private void selectPrevBar()
    {
        if(CurrentSingBar == null)
        {
            return;
        }
        int idx = singerControllers[singerIdx].getPrevBarIdxOf(CurrentSingBar);
        if(idx == -1)
        {
            return;
        }
        Debug.Log(idx);
        //CurrentSingBar.Selected = false;
        CurrentSingBar = singerControllers[singerIdx].singBars[idx];
        //CurrentSingBar.Selected = true;
    }

    private void selectNextBar()
    {
        if (CurrentSingBar == null)
        {
            return;
        }
        int idx = singerControllers[singerIdx].getNextBarIdxOf(CurrentSingBar);
        if (idx == -1)
        {
            return;
        }
        //CurrentSingBar.Selected = false;
        CurrentSingBar = singerControllers[singerIdx].singBars[idx];
        //CurrentSingBar.Selected = true;
    }

}
