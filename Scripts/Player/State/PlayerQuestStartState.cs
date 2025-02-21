using Cinemachine;
using System.Collections;
using UnityEngine;

public class PlayerQuestStartState : PlayerStateBase
{
    bool enterOnce = false;
    private Transform camTransform;

    public override void Enter()
    {
        base.Enter();

        // Switch to QuestStart Camera
        CameraManager.INSTANCE.cm_brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
        CameraManager.INSTANCE.freeLookCanmera.SetActive(false);
        CameraManager.INSTANCE.questStartCamera.gameObject.SetActive(true);
        //playerModel.QuestStartShot.SetActive(true);

        // Get camera transform reference
        camTransform = CameraManager.INSTANCE.questStartCamera.transform;

        // Play animation
        playerController.PlayAnimation("QuestStart", 0f);

        // Start camera movement
        CameraManager.INSTANCE.StartCoroutine(RotateAndLiftCamera());
    }

    public override void Update()
    {
        base.Update();

        if (!enterOnce)
        {
            playerController.shotPanel.StartAndEndOpenScene();
            enterOnce = true;
        }

        if (IsAnimationEnd())
        {
            // Stop camera movement
            CameraManager.INSTANCE.StopCoroutine(RotateAndLiftCamera());

            // Switch back to FreeLook
            playerModel.QuestStartShot.SetActive(false);
            CameraManager.INSTANCE.questStartCamera.gameObject.SetActive(false);
            CameraManager.INSTANCE.freeLookCanmera.SetActive(true);
            CameraManager.INSTANCE.ResetFreeLookCamera();

            // Switch to Idle
            playerController.SwitchState(PlayerState.Idle);
        }
    }

    private IEnumerator RotateAndLiftCamera()
    {
        float duration = 3.7f;
        float elapsedTime = 0f;

        Vector3 startPosition = camTransform.position;
        Quaternion startRotation = camTransform.rotation;

        // First movement: Rotate **90° right** & lift **1 unit up**
        Vector3 targetPosition1 = startPosition + new Vector3(0.9f, 1.2f, 2.2f);
        Quaternion targetRotation1 = Quaternion.Euler(startRotation.eulerAngles.x, startRotation.eulerAngles.y + 90f, startRotation.eulerAngles.z);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / duration); // Smooth transition

            camTransform.position = Vector3.Lerp(startPosition, targetPosition1, t);
            camTransform.rotation = Quaternion.Slerp(startRotation, targetRotation1, t);

            yield return null;
        }
        
        // Second movement: Rotate **60° right** & move **backward by -0.3f on Z-axis**
        duration = 1.7f;
        elapsedTime = 0f;
        startPosition = camTransform.position;
        startRotation = camTransform.rotation;

        Vector3 targetPosition2 = targetPosition1 + new Vector3(0.35f, 0f, -1f);  // Move backward
        Quaternion targetRotation2 = Quaternion.Euler(startRotation.eulerAngles.x, startRotation.eulerAngles.y + 100f, startRotation.eulerAngles.z);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / duration); // Smooth transition

            camTransform.position = Vector3.Lerp(startPosition, targetPosition2, t);
            camTransform.rotation = Quaternion.Slerp(startRotation, targetRotation2, t);

            yield return null;
        }

        duration = 0.8f;
        elapsedTime = 0f;
        startPosition = camTransform.position;
        startRotation = camTransform.rotation;

        Vector3 targetPosition3 = new Vector3(0f, 1.3f, 1.2f);  // Move backward
        Quaternion targetRotation3 = Quaternion.Euler(startRotation.eulerAngles.x+10f, 0, startRotation.eulerAngles.z);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / duration); // Smooth transition

            camTransform.position = Vector3.Lerp(startPosition, targetPosition3, t);
            camTransform.rotation = Quaternion.Slerp(startRotation, targetRotation3, t);

            yield return null;
        }

        duration = 1f;
        elapsedTime = 0f;
        startPosition = camTransform.position;
        startRotation = camTransform.rotation;

        Vector3 targetPosition4 = new Vector3(0f, 1.215f, -1f);  // Move backward
        Quaternion targetRotation4 = Quaternion.Euler(0f, 0f, 0f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / duration); // Smooth transition

            camTransform.position = Vector3.Lerp(startPosition, targetPosition4, t);
            camTransform.rotation = Quaternion.Slerp(startRotation, targetRotation4, t);

            yield return null;
        }

        //playerModel.QuestStartShot.SetActive(true);
    }
}
