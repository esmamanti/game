using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(Collider))]
public class RedEndZone : MonoBehaviour
{
    [SerializeField] private EndCinematicManager endCinematic;
    [SerializeField] private VideoClip overrideClip;
    [SerializeField] private bool onlyOnce = true;

    private bool done = false;

    void Awake()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
        if (Time.timeScale == 0f) Time.timeScale = 1f;
    }

    private bool IsPlayer(Transform t)
    {
        while (t != null)
        {
            if (t.CompareTag("Player")) return true;
            t = t.parent;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (done && onlyOnce) return;
        if (!IsPlayer(other.transform)) return;

        done = true;
        if (!endCinematic) { Debug.LogWarning("EndCinematicManager atanmadý!"); return; }

        if (overrideClip) endCinematic.PlayEnding(overrideClip);
        else endCinematic.PlayEnding();
    }
}