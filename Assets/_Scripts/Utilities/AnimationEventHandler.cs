using UnityEngine;
using UnityEngine.Events;

public class AnimationEventHandler : MonoBehaviour {

    [SerializeField] private UnityEvent<EnumAnimationEvents> animationEvent;

    public void OnAnimationEvent(string eventName) {
        Debug.Log($"Animation Event Triggered: {eventName}");
        // You can add additional logic here to handle specific events based on the eventName
    }

    public void OnAnimationEvent(EnumAnimationEvents eventType) {
        Debug.Log($"Animation Event Triggered: {eventType}");

        this.animationEvent?.Invoke(eventType);
    }
}

[System.Serializable]
public enum EnumAnimationEvents {
    None,
    ObeliskAppearingEnd
}
