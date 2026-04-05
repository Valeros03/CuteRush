using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenController : UIPanel
{
    [SerializeField] private GameObject continueButton;

    public void EnableContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.SetActive(true);
        }
    }

    public void DisableContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }
    }
}