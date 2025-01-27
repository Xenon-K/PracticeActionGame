using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotEffect : MonoBehaviour
{
    public Image UpperBound;
    public Image LowerBound;
    private Vector3 originalPositionUpper;
    private Vector3 originalPositionLower;
    public GameObject UIPanel;

    void Start()
    {
        // Manually initialize positions for UpperBound and LowerBound
        originalPositionUpper = UpperBound.transform.position;
        originalPositionLower = LowerBound.transform.position;
    }

    public void StartAndEndScene()
    {
        StartCoroutine(StartAndEndSequence());
    }

    private IEnumerator StartAndEndSequence()
    {
        UIPanel.SetActive(false);
        // Start both bounds moving simultaneously
        Vector3 targetPositionUpper = new Vector3(originalPositionUpper.x, originalPositionUpper.y - 200, originalPositionUpper.z);
        Vector3 targetPositionLower = new Vector3(originalPositionLower.x, originalPositionLower.y + 200, originalPositionLower.z);

        // Move both bounds down simultaneously
        yield return MoveBothBounds(UpperBound, LowerBound, originalPositionUpper, targetPositionUpper, originalPositionLower, targetPositionLower, 0.3f);

        // Wait for 1.5 seconds
        yield return new WaitForSeconds(1.5f);

        // Move both bounds back to their original positions simultaneously
        yield return MoveBothBounds(UpperBound, LowerBound, UpperBound.transform.position, originalPositionUpper, LowerBound.transform.position, originalPositionLower, 0.3f);
        UIPanel.SetActive(true);
    }

    //using used for quest start
    public void StartAndEndOpenScene()
    {
        StartCoroutine(StartAndEndOpenSequence());
    }

    private IEnumerator StartAndEndOpenSequence()
    {
        UIPanel.SetActive(false);
        // Start both bounds moving simultaneously
        Vector3 targetPositionUpper = new Vector3(originalPositionUpper.x, originalPositionUpper.y - 200, originalPositionUpper.z);
        Vector3 targetPositionLower = new Vector3(originalPositionLower.x, originalPositionLower.y + 200, originalPositionLower.z);

        // Move both bounds down simultaneously
        yield return MoveBothBounds(UpperBound, LowerBound, originalPositionUpper, targetPositionUpper, originalPositionLower, targetPositionLower, 0.3f);

        // Wait for 7 seconds
        yield return new WaitForSeconds(7f);

        // Move both bounds back to their original positions simultaneously
        yield return MoveBothBounds(UpperBound, LowerBound, UpperBound.transform.position, originalPositionUpper, LowerBound.transform.position, originalPositionLower, 0.3f);
        UIPanel.SetActive(true);
    }

    private IEnumerator MoveBothBounds(Image upperBound, Image lowerBound, Vector3 startUpper, Vector3 targetUpper, Vector3 startLower, Vector3 targetLower, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            // Smoothly interpolate positions for both bounds
            upperBound.transform.position = Vector3.Lerp(startUpper, targetUpper, elapsedTime / duration);
            lowerBound.transform.position = Vector3.Lerp(startLower, targetLower, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set final positions to ensure precision
        upperBound.transform.position = targetUpper;
        lowerBound.transform.position = targetLower;
    }
}