using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeButtonController : MonoBehaviour
{

    public GameObject PlayImage, PauseImage;
    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        active = false;
        PlayImage.SetActive(true);
        PauseImage.SetActive(false);
    }

    public void Toggle()
    {
        active = !active;
        PlayImage.SetActive(!active);
        PauseImage.SetActive(active);
    }
}
