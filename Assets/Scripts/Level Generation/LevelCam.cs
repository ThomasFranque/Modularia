using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCam : MonoBehaviour
{
    [SerializeField] private Camera _playerCam = default;
    [SerializeField] private Camera _levelCam = default;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            _playerCam.enabled = false;
            _levelCam.enabled = true;
        }
        else if(Input.GetKeyUp(KeyCode.G))
        {
            _playerCam.enabled = true;
            _levelCam.enabled = false;
        }
    }
}