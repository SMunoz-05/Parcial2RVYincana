using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImageSequence : MonoBehaviour
{
    [Header("AR")]
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    [Header("Sequence")]
    [SerializeField] private float delayBeforeShowUI = 1.8f;

    [Header("UI")]
    [SerializeField] private GameObject questionPanel;     // set inactive initially
    [SerializeField] private Animator questionPanelAnimator;
    [SerializeField] private string showTrigger = "Show";

    // Para no disparar mil veces por la misma tarjeta
    private readonly HashSet<TrackableId> playedFor = new();

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        // "added" suele ser el mejor para 1 sola vez
        foreach (var img in args.added)
            TryPlay(img);

        // opcional: si a veces no entra por added, puedes permitir updated cuando pasa a Tracking
        foreach (var img in args.updated)
            TryPlay(img);
    }

    private void TryPlay(ARTrackedImage img)
    {
        if (img.trackingState != TrackingState.Tracking)
            return;

        if (playedFor.Contains(img.trackableId))
            return;

        playedFor.Add(img.trackableId);

        // Aquí: instanciar portal/naves como hijo del img.transform si lo necesitas
        // Instantiate(portalPrefab, img.transform);

        StartCoroutine(ShowUIAfterDelay());
    }

    private IEnumerator ShowUIAfterDelay()
    {
        if (questionPanel != null)
            questionPanel.SetActive(false);

        yield return new WaitForSeconds(delayBeforeShowUI);

        if (questionPanel != null)
            questionPanel.SetActive(true);

        if (questionPanelAnimator != null)
            questionPanelAnimator.SetTrigger(showTrigger);
    }
}