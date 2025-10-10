using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class ElectricalPanelInteraction : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI interactionText;  

    [Header("Audio Settings")]
    public AudioClip humSound;
    public float maxVolumeDistance = 5f;     
    public float minVolumeDistance = 15f;    

    private AudioSource audioSource;

    private Transform player;

    private bool isInRange = false;

    private bool hasInteracted = false;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (humSound != null)
        {
            audioSource.clip = humSound;
            audioSource.loop = true;
            audioSource.playOnAwake = true;
            audioSource.spatialBlend = 0f; 
            audioSource.volume = 0f;
            audioSource.Play();
        }

   
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

     
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    void Update()
    {
     
        if (player != null && humSound != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            float t = Mathf.InverseLerp(minVolumeDistance, maxVolumeDistance, distance);
            audioSource.volume = Mathf.Lerp(1f, 0f, t); 
        }

     
        if (isInRange && !hasInteracted && Input.GetKeyDown(KeyCode.E))
        {
            hasInteracted = true;
            if (interactionText != null)
            {
                interactionText.text = "I should follow the cable";
                CancelInvoke();
                Invoke(nameof(HideInteractionText), 3f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            if (!hasInteracted && interactionText != null)
            {
                interactionText.text = "Press E to interact";
                interactionText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
        }
    }

    private void HideInteractionText()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }
}
