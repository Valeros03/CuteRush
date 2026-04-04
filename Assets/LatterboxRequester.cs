using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatterboxRequester : MonoBehaviour
{
 
    public void StartLatterbox()
    {
        UIManager.Instance.ShowLatterbox();
    }

    public void FadeLatterbox()
    {
        UIManager.Instance.FadeLatterbox();
    }

}
