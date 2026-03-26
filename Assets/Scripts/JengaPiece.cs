using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class JengaPiece : MonoBehaviour
{
    [Header("Tipo RPS")]
    public BlockType blockType;

    [Header("Audio")]
    public AudioClip[] clips; // asignado por TowerGenerator

    [HideInInspector] public bool hasBeenRemoved = false;
    [HideInInspector] public int row;

    private Grabbable _grabbable;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;

        _grabbable = GetComponent<Grabbable>();
        if (_grabbable == null)
        {
            Debug.LogWarning($"[JengaPiece] No Grabbable en {gameObject.name}");
            return;
        }
        _grabbable.WhenPointerEventRaised += OnPointerEvent;
    }

    private void OnDestroy()
    {
        if (_grabbable != null)
            _grabbable.WhenPointerEventRaised -= OnPointerEvent;
    }

    private void OnPointerEvent(PointerEvent evt)
    {
        switch (evt.Type)
        {
            case PointerEventType.Select:
                if (hasBeenRemoved) return;
                hasBeenRemoved = true;
                PlayRandomClip();
                GameManager.Instance.OnPieceRemoved(this);
                break;

            case PointerEventType.Unselect:
                GameManager.Instance.OnPieceReleased(this);
                break;
        }
    }

    private void PlayRandomClip()
    {
        if (clips == null || clips.Length == 0) return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip != null) _audioSource.PlayOneShot(clip);
    }
}