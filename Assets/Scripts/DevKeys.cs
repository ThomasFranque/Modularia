using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevKeys : MonoBehaviour
{
    bool _paused;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        else if (Input.GetKeyDown(KeyCode.P))
            TogglePause();
    }

    private void TogglePause()
    {
        _paused = !_paused;
        if (_paused)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1;
    }
}