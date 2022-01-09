using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ErosionUI : MonoBehaviour
{
    public Erosion Erosion;
    public HeightMapGenerator Generator;
    public CameraControl control;

    public Slider Slider_Erosion_Speed;
    public Slider Slider_Erosion_Gravity;
    public Slider Slider_Erosion_Lifetime;
    public Slider Slider_Erosion_SedimentCapacity;

    public Button LookAround;

    internal Quaternion rotation;
    internal Vector3 position;

    public void Start()
    {
        Slider.SliderEvent evente = new Slider.SliderEvent();
        evente.AddListener(new UnityAction<float>(OnErosionSpeedUpdate));
        Slider_Erosion_Speed.onValueChanged = evente;

        Slider.SliderEvent evente2 = new Slider.SliderEvent();
        evente2.AddListener(new UnityAction<float>(OnGravityUpdate));
        Slider_Erosion_Gravity.onValueChanged = evente2;

        Slider.SliderEvent evente3 = new Slider.SliderEvent();
        evente3.AddListener(new UnityAction<float>(OnLifetimeUpdate));
        Slider_Erosion_Lifetime.onValueChanged = evente3;

        Slider.SliderEvent evente4 = new Slider.SliderEvent();
        evente4.AddListener(new UnityAction<float>(OnSedimentUpdate));
        Slider_Erosion_SedimentCapacity.onValueChanged = evente4;

        rotation = control.transform.rotation;
        position = control.transform.position;
    }

    public void OnErosionSpeedUpdate(float target)
    {
        Erosion.erodeSpeed = target;
    }

    public void OnGravityUpdate(float target)
    {
        Erosion.gravity = target;
    }

    public void OnLifetimeUpdate(float target)
    {
        Erosion.maxDropletLifetime = (int)target;
    }

    public void OnSedimentUpdate(float target)
    {
        Erosion.sedimentCapacityFactor = target;
    }

    public void OnLookAround()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        control.enabled = true;
    }

    public void EnableLookAround()
    {
        LookAround.gameObject.SetActive(true);
    }

    public void DisableLookAround()
    {
        LookAround.gameObject.SetActive(false);
    }

    public void EndLook()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        control.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        control.transform.rotation = rotation;
        control.transform.position = position;
        control.enabled = false;
        DisableLookAround();
    }
}
