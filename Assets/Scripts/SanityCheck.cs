using UnityEngine;
using UnityEngine.Assertions;

public class SanityCheck : MonoBehaviour
{
    [SerializeField]
    private Transform sanityBar;

    [SerializeField]
    private float sanityDropAmount = 1.0f;

    [SerializeField]
    private SongPlayer songPlayer;

    [HideInInspector]
    public float SanityVal = 1.0f;

    private void Awake()
    {
        Assert.IsNotNull(sanityBar);
    }

    public void DropSanity(float val)
    {
        SanityVal -= sanityDropAmount * val;
        sanityBar.transform.localScale = new Vector3(SanityVal, 1.0f, 1.0f);
        if(SanityVal <= 0)
        {
            songPlayer.GameOver();
        }
    }
}
