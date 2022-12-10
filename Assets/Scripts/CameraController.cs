using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] private KeyCode aimKeyCode = KeyCode.Mouse1;
    [SerializeField] private int priorityBoostAmount = 10;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject aimCrosshair;

    private CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        aimCrosshair.SetActive(false);
        crosshair.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(aimKeyCode))
        {
            StartAim();
        }

        if (Input.GetKeyUp(aimKeyCode))
        {
            CancelAim();
        }
    }

    private void StartAim()
    {
        _virtualCamera.Priority += priorityBoostAmount;
        aimCrosshair.SetActive(true);
        crosshair.SetActive(false);
    }

    private void CancelAim()
    {
        _virtualCamera.Priority -= priorityBoostAmount;
        aimCrosshair.SetActive(false);
        crosshair.SetActive(true);
    }
}