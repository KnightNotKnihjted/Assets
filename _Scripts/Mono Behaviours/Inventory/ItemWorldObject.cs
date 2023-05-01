using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorldObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Item myItem;
    [SerializeField] private int quantity;
    [SerializeField] private float radius = 1f;
    private float startCooldown = 1f;
    private float cooldown = 1f;
#if UNITY_EDITOR
    [SerializeField] private bool drawGizmos;
    [SerializeField] private Color radiusGizmoColour;
#endif

    private void LateUpdate()
    {
        sr.sprite = myItem.WorldSprite;
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
        else
        {
            if (quantity <= 0)
            {
                Destroy(gameObject);
            }
            foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, radius))
            {
                HandleCollide(col);
            }
        }
    }
    private void HandleCollide(Collider2D other)
    {
        if (other.TryGetComponent(out ItemWorldObject item))
        {
            item.GetValues(out Item itm, out int qty);
            if (itm == myItem)
            {
                // Check if this object has a lower instance ID than the other object
                if (GetInstanceID() < item.GetInstanceID())
                {
                    quantity += qty;
                    Destroy(other.gameObject);
                }
            }
        }
        else if (other.TryGetComponent<IPlayer>(out _))
        {
            PlayerInventoryManager.i.AddItem(myItem, quantity, out quantity);
        }
    }
    public void GetValues(out Item itm, out int qty)
    {
        itm = myItem;
        qty = quantity;
    }
    public void Setup(Item itm, int qty, float cldwn = 3f)
    {
        myItem = itm;
        quantity = qty;
        startCooldown = cldwn;
        cooldown = startCooldown;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        Gizmos.color = radiusGizmoColour;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
