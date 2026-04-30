using System.Collections;
using UnityEngine;

public class PillarController : MonoBehaviour {

    [Header("Pillar Segments")]
    [SerializeField] private Transform topSegment;
    [SerializeField] private Transform middleSegment;
    [SerializeField] private Transform bottomSegment;
    [SerializeField] private Transform topSeprationSegment;
    [SerializeField] private Transform bottomSeprationSegment;

    [SerializeField] private Vector2 seperationRotationSpeedMinMax = new Vector2(2f, 4f);
    [SerializeField] private Vector2 rotationSpeedMinMax = new Vector2(32f, 35f);

    private enum EnumRunes { None, Cross, Triangle, Circle, Blank }
    public enum EnumSegment { Top, Middle, Bottom }

    private int rotationCost = 60;

    private EnumRunes currentTopRune;
    private EnumRunes currentMiddleRune;
    private EnumRunes currentBottomRune;

    private Coroutine currentTopRotationCoroutine, currentMiddleRotationCoroutine, currentBottomRotationCoroutine;

    private void Start() {

        float topSeparationRotationSpeed = Random.Range(seperationRotationSpeedMinMax.x, seperationRotationSpeedMinMax.y);
        float bottomSeparationRotationSpeed = Random.Range(seperationRotationSpeedMinMax.x, seperationRotationSpeedMinMax.y);

        StartCoroutine(IdleRotate(topSeprationSegment, topSeparationRotationSpeed));
        StartCoroutine(IdleRotate(bottomSeprationSegment, bottomSeparationRotationSpeed));

        this.currentTopRune = GetRune(EnumSegment.Top);
        this.currentMiddleRune = GetRune(EnumSegment.Middle);
        this.currentBottomRune = GetRune(EnumSegment.Bottom);
    }

    [ContextMenu("Debug Middle Side")] public void DebugMiddleSide() => Debug.Log($"Current Middle Rune: {GetRune(EnumSegment.Middle)}");
    [ContextMenu("Rotate Middle Left")] public void RotateMiddleLeft() => RotateSegment(EnumSegment.Middle, true);
    [ContextMenu("Rotate Middle Right")] public void RotateMiddleRight() => RotateSegment(EnumSegment.Middle, false);

    public void RotateLeftByEnumIndex(int segmentIndex) => RotateSegment((EnumSegment)segmentIndex, true);
    public void RotateRightByEnumIndex(int segmentIndex) => RotateSegment((EnumSegment)segmentIndex, false);

    private IEnumerator IdleRotate(Transform targetTransform, float rotationSpeed) {
        float maxRotation = this.rotationCost - 5f;

        while (true) {
            float amountRotated = 0f;

            // Rotate until we reach our target amount
            while (amountRotated < maxRotation) {
                // Calculate rotation step for this frame
                float step = rotationSpeed * Time.deltaTime;

                targetTransform.Rotate(Vector3.up, step);
                amountRotated += Mathf.Abs(step);

                yield return null;
            }

            // Reverse direction after reaching the target angle
            rotationSpeed = -rotationSpeed;
        }
    }

    private void RotateSegment(EnumSegment segment, bool postiveDirection) {
        float randomRotationSpeed = Random.Range(rotationSpeedMinMax.x, rotationSpeedMinMax.y);
        if (!postiveDirection) randomRotationSpeed *= -1;
        switch (segment) {
            case EnumSegment.Top:
                if (this.currentTopRotationCoroutine != null) break; // Prevent starting a new rotation if one is already in progress
                this.currentTopRotationCoroutine = StartCoroutine(RotateSegment(this.topSegment, EnumSegment.Top, randomRotationSpeed));
                break;
            case EnumSegment.Middle:
                if (this.currentMiddleRotationCoroutine != null) break; // Prevent starting a new rotation if one is already in progress
                this.currentMiddleRotationCoroutine = StartCoroutine(RotateSegment(this.middleSegment, EnumSegment.Middle, randomRotationSpeed));
                break;
            case EnumSegment.Bottom:
                if (this.currentBottomRotationCoroutine != null) break; // Prevent starting a new rotation if one is already in progress
                this.currentBottomRotationCoroutine = StartCoroutine(RotateSegment(this.bottomSegment, EnumSegment.Bottom, randomRotationSpeed));
                break;
        }
    }

    private IEnumerator RotateSegment(Transform transform, EnumSegment segment, float rotationSpeed) {
        EnumRunes startRune = GetRune(segment);
        EnumRunes currentRune;
        float amountRotated = 0f; // We need to rotate at least 120 degrees to change runes.

        if (segment == EnumSegment.Top) currentRune = this.currentTopRune;
        else if (segment == EnumSegment.Middle) currentRune = this.currentMiddleRune;
        else currentRune = this.currentBottomRune;

        bool isSuccessful = startRune == GetRune(segment) && startRune != currentRune;

        while (amountRotated <= this.rotationCost && !isSuccessful) {
            float step = rotationSpeed * Time.deltaTime; // Calculate rotation step for this frame
            transform.Rotate(Vector3.up, step); // Rotate the segment
            amountRotated += Mathf.Abs(step);
            yield return new WaitForEndOfFrame();
        }

        if (segment == EnumSegment.Top) this.currentTopRotationCoroutine = null;
        else if (segment == EnumSegment.Middle) this.currentMiddleRotationCoroutine = null;
        else this.currentBottomRotationCoroutine = null;

        // Snap the current rotation to the nearest valid angle to prevent drifting over multiple rotations
        SnapToValidAngle(transform, out int validAngle);

    }

    private bool SnapToValidAngle(Transform transform, out int validAngle) {
        int angle = Mathf.RoundToInt(transform.localEulerAngles.y); // Using localEulerAngles is safer in case the parent is rotated.
        bool result = IsValidAngle(angle, out validAngle);

        if (result) {
            Vector3 newEulerAngles = transform.localEulerAngles;
            newEulerAngles.y = validAngle;
            transform.localEulerAngles = newEulerAngles;
        }

        return result;
    }

    private bool IsValidAngle(int angle, out int validAngle) {
        int remainder = angle % this.rotationCost;
        int distanceToNearest = Mathf.Min(remainder, this.rotationCost - remainder);

        if (distanceToNearest > 2) {
            validAngle = angle;
            return false;
        }

        validAngle = (angle + distanceToNearest) % 360;
        if (validAngle % this.rotationCost != 0) {
            validAngle = (angle - distanceToNearest) % 360;
        }

        return true;
    }

    private EnumRunes GetRune(EnumSegment segment) {
        EnumRunes result = EnumRunes.None; // Default value if no valid rune is found
        Transform targetTransform; // The transform reference of the specified segment

        // Get the transform reference of the specified segment
        if (segment == EnumSegment.Middle) {
            targetTransform = this.middleSegment;
            // Using localEulerAngles is safer in case the parent is rotated.
        } else {
            Debug.LogWarning($"GetRune is not implemented for {segment}. Returning None.");
            return EnumRunes.None;
        }

        // Use a small tolerance for floating point inaccuracies since object transformations can result in slight drifting
        if (!SnapToValidAngle(targetTransform, out int validAngle)) {
            return EnumRunes.None;
        }

        Debug.Log($"Segment: {segment}, ValidAngle: {validAngle}");

        if (segment == EnumSegment.Middle) {
            if (validAngle == 0) {
                result = EnumRunes.Cross;
            } else if (validAngle == this.rotationCost * 2) {
                result = EnumRunes.Triangle;
            } else if (validAngle == this.rotationCost * 4) {
                result = EnumRunes.Circle;
            } else {
                result = EnumRunes.Blank;
            }

            // Update current middle rune if it has changed
            if (result != this.currentMiddleRune) {
                this.currentMiddleRune = result;
            }
        }

        return result;
    }
}
