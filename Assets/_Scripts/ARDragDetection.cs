using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class ARDragDetection : MonoBehaviour {
    // Unity Events
    [Header("Drag Events")]
    [SerializeField] private UnityEvent OnLeftDrag;
    [SerializeField] private UnityEvent OnRightDrag;

    private XRSimpleInteractable interactable;
    private Vector2 initialTouchPosition;
    private Vector2 currentTouchPosition;
    private bool isDragging;

    private void Awake() {
        this.interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable() {
        // Subscribe to interaction events
        this.interactable.selectEntered.AddListener(OnSelectEntered);
        this.interactable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable() {
        // Unsubscribe from interaction events
        this.interactable.selectEntered.RemoveListener(OnSelectEntered);
        this.interactable.selectExited.RemoveListener(OnSelectExited);
    }

    private void Update() {
        if (!this.isDragging) return; // Only update touch position if dragging

        bool hasTouch = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
        bool hasMouse = Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (hasTouch) {
            this.currentTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        } else if (hasMouse) {
            this.currentTouchPosition = Mouse.current.position.ReadValue();
        }

        Debug.Log($"Current Touch Position: {this.currentTouchPosition}");
    }
    private void OnSelectEntered(SelectEnterEventArgs arg0) {
        bool hasTouch = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
        bool hasMouse = Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (hasTouch) {
            this.initialTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            this.isDragging = true;
        } else if (hasMouse) {
            this.initialTouchPosition = Mouse.current.position.ReadValue();
            this.isDragging = true;
        }

        Debug.Log($"Initial Touch Position: {this.initialTouchPosition}");
    }
    private void OnSelectExited(SelectExitEventArgs arg0) {
        this.isDragging = false;

        Vector2 deltaDrag = this.currentTouchPosition - this.initialTouchPosition;
        HandleDrag(deltaDrag);

        Debug.Log($"Drag Delta: {deltaDrag}");
    }


    private void HandleDrag(Vector2 delta) {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) {
            if (delta.x > 0) this.OnRightDrag?.Invoke();
            else this.OnLeftDrag?.Invoke();
        }
    }
}
