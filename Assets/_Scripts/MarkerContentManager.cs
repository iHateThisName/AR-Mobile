using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MarkerContentManager : MonoBehaviour {

    [SerializeField] private string markerName;
    [SerializeField] private GameObject content;

    private ARTrackedImageManager imageManager;
    private GameObject instancedContent;

    void Start() {
        this.imageManager = GetComponent<ARTrackedImageManager>();
        this.imageManager.trackablesChanged.AddListener(OnTrackablesChanged);
    }

    private void OnDestroy() {
        this.imageManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
    }

    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> trackableImageArgs) {
        Debug.Log($"Trackables changed: added={trackableImageArgs.added.Count}, updated={trackableImageArgs.updated.Count}, removed={trackableImageArgs.removed.Count}");
        foreach (ARTrackedImage addedImage in trackableImageArgs.added) {
            if (addedImage.referenceImage.name == markerName) {
                this.instancedContent = Instantiate(original: content,
                                                    position: addedImage.transform.position,
                                                    rotation: addedImage.transform.rotation);
                this.instancedContent.transform.SetParent(addedImage.transform);
            }
        }

        foreach (var removedImage in trackableImageArgs.removed) {
            if (removedImage.Value.referenceImage.name == markerName) {
                Destroy(this.instancedContent);
            }
        }


    }

}
