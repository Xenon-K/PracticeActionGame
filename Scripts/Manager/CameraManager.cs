using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Camera manager
public class CameraManager : SingleMomoBase<CameraManager>
{
    //CM brain components
    public CinemachineBrain cm_brain;
    //FreeLockCamera
    public GameObject freeLookCanmera;
    //FreeLockCamera components
    public CinemachineFreeLook freeLook;
    //Quest start cam
    public CinemachineVirtualCamera questStartCamera;

    /// <summary>
    /// reset FreeLockCamera view
    public void ResetFreeLookCamera()
    {
        freeLook.m_YAxis.Value = 0.5f;
        freeLook.m_XAxis.Value = PlayerController.INSTANCE.playerModel.transform.eulerAngles.y;
    }

}
