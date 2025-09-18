using System.Collections;
using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    public bool isLocked = true;

    public Transform doorTransform;

    public Vector3 openEuler = new Vector3(0f, 90f, 0f);

    public float openDuration = 1f;

    public AudioClip unlockSound;

    private AudioSource audioSource;

    private Quaternion closedRot;

    private Quaternion openRot;

    private bool isOpen = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (doorTransform == null) doorTransform = transform;
        closedRot = doorTransform.localRotation;
        openRot = Quaternion.Euler(openEuler) * closedRot;
    }

    public void Unlock()
    {
        if (!isLocked || isOpen) return;
        isLocked = false;
        if (unlockSound != null && audioSource != null) audioSource.PlayOneShot(unlockSound);
        StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor()
    {
        isOpen = true;
        float t = 0f;
        Quaternion start = doorTransform.localRotation;
        while (t < openDuration)
        {
            t += Time.deltaTime;
            doorTransform.localRotation = Quaternion.Slerp(start, openRot, t / openDuration);
            yield return null;
        }
        doorTransform.localRotation = openRot;
    }
}
