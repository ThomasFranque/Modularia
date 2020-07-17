using UnityEngine;

// From https://answers.unity.com/questions/518399/simulate-child-parent-relationship.html
public class FakeChild : MonoBehaviour
{
    [SerializeField] private Transform _fakeParent = default;
    // If true, will attempt to scale the child accurately as the parent scales
    // Will not be accurate if starting rotations are different or irregular
    // Experimental
    [SerializeField, Tooltip("If true, will attempt to scale the child accurately as the parent scales. " +
        "Will not be accurate if starting rotations are different or irregular Experimental ")]
    private bool attemptChildScale = false;

    Vector3 startParentPosition;
    Quaternion startParentRotationQ;
    Vector3 startParentScale;

    Vector3 startChildPosition;
    Quaternion startChildRotationQ;
    Vector3 startChildScale;

    Matrix4x4 parentMatrix;

    void Start()
    {
        startParentPosition = _fakeParent.position;
        startParentRotationQ = _fakeParent.rotation;
        startParentScale = _fakeParent.lossyScale;

        startChildPosition = transform.position;
        startChildRotationQ = transform.rotation;
        startChildScale = transform.lossyScale;

        // Keeps child position from being modified at the start by the parent's initial transform
        startChildPosition = DivideVectors(Quaternion.Inverse(_fakeParent.rotation) * (startChildPosition - startParentPosition), startParentScale);
    }

    void LateUpdate()
    {

        parentMatrix =
            Matrix4x4.TRS(_fakeParent.position, _fakeParent.rotation, _fakeParent.lossyScale);

        transform.position = parentMatrix.MultiplyPoint3x4(startChildPosition);

        transform.rotation =
            (_fakeParent.rotation *
                Quaternion.Inverse(startParentRotationQ)) *
            startChildRotationQ;

        // Incorrect scale code; it scales the child locally not gloabally; Might work in some cases, but will be inaccurate in others
        if (attemptChildScale)
            transform.localScale = Vector3.Scale(startChildScale,
                DivideVectors(_fakeParent.lossyScale, startParentScale));
    }

    Vector3 DivideVectors(Vector3 num, Vector3 den) =>
        new Vector3(num.x / den.x, num.y / den.y, num.z / den.z);
}