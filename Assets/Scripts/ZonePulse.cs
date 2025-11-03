using UnityEngine;

public class ZonePulse : MonoBehaviour
{
    public float speed = 2f;
    public float amplitude = 0.08f;
    private Vector3 baseScale;

    void Start() { baseScale = transform.localScale; }

    void Update()
    {
        float s = 1f + Mathf.Sin(Time.time * speed) * amplitude;
        transform.localScale = baseScale * s;
    }
}