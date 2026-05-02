using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
            Vector2 position = Touchscreen.current.primaryTouch.position.ReadValue();
            if (IsPointerOverUI(position)) return;
            PlaceObject(position);
        } else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
            Vector2 position = Mouse.current.position.ReadValue();
            if (IsPointerOverUI(position)) return;
            PlaceObject(position);
        }
    }

    private void PlaceObject(Vector2 position) {
        this.isPlacing = true;
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

    private bool IsPointerOverUI(Vector2 position) {
        if (EventSystem.current == null) return false;
        PointerEventData pointerData = new PointerEventData(EventSystem.current) {
            position = position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0;
    }

}
