using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacerController : MonoBehaviour {
    [SerializeField] private ARRaycastManager raycastManager;

    private bool isPlacing = false;

    private void Start() {
        if (this.raycastManager == null) Debug.LogError("ARRaycastManager is not assigned.");
    }

    private void Update() {
        if (!this.raycastManager) return;
        if (this.isPlacing) return;
            

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) {
            this.isPlacing = true;
            PlaceObject(Touchscreen.current.primaryTouch.position.ReadValue());
        } else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
            this.isPlacing = true;
            PlaceObject(Mouse.current.position.ReadValue());
        }
    }

    private void PlaceObject(Vector2 position) {
        var rayHits = new List<ARRaycastHit>();

        // Perform the raycast
        this.raycastManager.Raycast(position, rayHits, TrackableType.Planes);

        // Handle the raycast hit
        if (rayHits.Count > 0) {
            Vector3 hitPosition = rayHits[0].pose.position;
            Quaternion hitRotation = rayHits[0].pose.rotation;

            GameManager.Instance.OnTap(hitPosition, hitRotation);
        }
        StartCoroutine(PlacingCooldownCoroutine());
    }

    private IEnumerator PlacingCooldownCoroutine() {
        yield return new WaitForSeconds(0.5f);
        this.isPlacing = false;
    }

}
