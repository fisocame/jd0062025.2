using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonPulse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public float hoverScale = 1.06f, pressScale = 0.97f, speed = 12f;
    Vector3 baseScale;
    float target = 1f;
    AudioSource audioSrc;

    void Awake()
    {
        baseScale = transform.localScale;
        audioSrc = GetComponent<AudioSource>();
        if (!audioSrc) audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
    }

    void Update()
    {
        var s = Mathf.Lerp(transform.localScale.x, baseScale.x * target, Time.deltaTime * speed);
        transform.localScale = new Vector3(s, s, s);
    }

    public void OnPointerEnter(PointerEventData e) => target = hoverScale;
    public void OnPointerExit(PointerEventData e) => target = 1f;
    public void OnPointerDown(PointerEventData e) { target = pressScale; if (audioSrc) audioSrc.Play(); }
    public void OnPointerUp(PointerEventData e) => target = hoverScale;
}