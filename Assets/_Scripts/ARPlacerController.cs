using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacerController : MonoBehaviour {
    [SerializeField] private ARRaycastManager raycastManager;

    private bool isPlacing = false;

    private void Start() {
        if (this.raycastManager == null) Debug.LogError("ARRaycastManager is not assigned.");
    }

    private void OnEnable() {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable() {
        EnhancedTouchSupport.Disable();
    }

    private void Update() {
        if (!this.raycastManager) return;
        if (this.isPlacing) return;
            

        if (Touchscreen.current != null) {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame) {
                this.isPlacing = true;
                PlaceObject(touch.position.ReadValue());
            }
        } else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
            this.isPlacing = true;
            PlaceObject(Mouse.current.position.ReadValue());
        }


        //if (!this.isPlacing && (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))) {
        //    this.isPlacing = true;

        //    if (Input.touchCount > 0) {
        //        // This is for touch input
        //        PlaceObject(Input.GetTouch(0).position);

        //    } else {
        //        // This is for mouse input
        //        PlaceObject(Input.mousePosition);
        //    }
        //}
    }

    private void PlaceObject(Vector2 position) {
        var rayHits = new List<ARRaycastHit>();

        // Perform the raycast
        this.raycastManager.Raycast(position, rayHits, TrackableType.Planes);

        // Handle the raycast hit
        if (rayHits.Count > 0) {
            Vector3 hitPosition = rayHits[0].pose.position;
            Quaternion hitRotation = rayHits[0].pose.rotation;

            //Instantiate(this.raycastManager.raycastPrefab, hitPosition, hitRotation);
            GameManager.Instance.OnTap(hitPosition, hitRotation);

        }

        StartCoroutine(PlacingCooldownCoroutine());
    }

    private IEnumerator PlacingCooldownCoroutine() {
        yield return new WaitForSeconds(0.5f);
        this.isPlacing = false;
    }
}
