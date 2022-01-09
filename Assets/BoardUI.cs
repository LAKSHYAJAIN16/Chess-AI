using System;
using UnityEngine;
using UnityEngine.UI;

public class BoardUI : MonoBehaviour
{
    public GameObject Placeholder;
    public Transform Canvas;
    public BoardTheme Theme;
    public bool whiteIsAtBottom = true;
    public float size = 0.033333333f;

    internal const float delta = -3.5f;

    private void Awake()
    {
        InitUI();
    }

    internal void InitUI()
    {
        for (int file = 0; file < 8; file++)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                //Instantiate OBJ
                GameObject square = Instantiate(Placeholder);
                square.transform.SetParent(Canvas);

                //Create Image component
                square.AddComponent<Image>();
                Image image = square.GetComponent<Image>();
    
                //Assign Color
                image.color = ((file + rank) % 2 != 0) ? Theme.WhiteDefault : Theme.BlackDefault;

                //Assign Position
                square.transform.localPosition = GetPosition(file, rank);
            }
        }
    }

    internal Vector3 GetPosition(int file, int rank)
    {
        if (whiteIsAtBottom)
            return new Vector3(delta + (file / size), delta + (rank / size), 0f);
        else{
            return new Vector3(delta + 7 - (file / size), 7 - (rank / size), 0f);
        }
    }
}

[Serializable]
public class BoardTheme
{
    [Header("Default Colors")]
    public Color WhiteDefault = Color.white;
    public Color BlackDefault = Color.black;

    [Header("Win Colors")]
    public Color WhiteWin;
    public Color BlackWin;

    [Header("Background Colors")]
    public Color BackgroundColor = Color.black;
}
