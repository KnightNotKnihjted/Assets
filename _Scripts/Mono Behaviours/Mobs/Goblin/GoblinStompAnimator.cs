using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinStompAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] frames;
    [SerializeField] private Vector2[] offsets;
    [SerializeField] private Vector2[] sizes;
    [SerializeField] private int frame;
    private int actualFrame;
    [SerializeField] private CapsuleCollider2D col;
    [SerializeField] private SpriteRenderer sr;
    public bool destroyOnEffectFinish;

    private void FixedUpdate()
    {
        actualFrame++;
        if (actualFrame % 3 == 0)
        {
            frame++;
        }
        if(frame >= frames.Length)
        {
            frame = 0;
            if (destroyOnEffectFinish) Destroy(gameObject);
        }

        if(sr != null)
        {
            sr.sprite = frames[frame];
        }
        if(col != null)
        {
            col.size = sizes[frame];
            col.offset = offsets[frame];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out IStompable stompable))
        {
            stompable.Stomp(this);
        }
    }
}
