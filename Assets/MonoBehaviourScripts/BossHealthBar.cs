using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("References")]
    public Transform target;         
    public Vector3 offset = new Vector3(0, 3.0f, 0); 
    public Camera worldCamera;       
    public Slider slider;            

    private void Awake()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (target == null) return;

       
        transform.position = target.position + offset;

       
        if (worldCamera != null)
        {
            Vector3 dir = transform.position - worldCamera.transform.position;
          
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }


    public void SetMaxHealth(float max)
    {
        if (slider != null)
        {
            slider.maxValue = Mathf.Max(1f, max);
            slider.value = slider.maxValue;
        }
    }

   
    public void SetHealth(float current)
    {
        if (slider != null)
            slider.value = Mathf.Clamp(current, 0f, slider.maxValue);
    }
}
