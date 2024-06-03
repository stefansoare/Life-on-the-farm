using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    PlayerController playerCtrl;
    Land selectedLand = null;
    InteractableObject selectedObject = null;

    void Start()
    {
        playerCtrl = transform.parent.GetComponent<PlayerController>();
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1))
        {
            HandleHit(hit);
        }
    }

    void HandleHit(RaycastHit hitInfo)
    {
        Collider collider = hitInfo.collider;

        if (collider.tag == "Land")
        {
            Land land = collider.GetComponent<Land>();
            SelectLand(land);
            return;
        }

        if (collider.tag == "Item")
        {
            selectedObject = collider.GetComponent<InteractableObject>();
            return;
        }

        if (selectedObject != null)
        {
            selectedObject = null;
        }

        if (selectedLand != null)
        {
            selectedLand.Select(false);
            selectedLand = null;
        }
    }

    void SelectLand(Land land)
    {
        if (selectedLand != null)
        {
            selectedLand.Select(false);
        }

        selectedLand = land;
        land.Select(true);
    }

    public void Interact()
    {
        if (InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Item))
        {
            return;
        }

        if (selectedLand != null)
        {
            selectedLand.Interact();
            return;
        }

        Debug.Log("Not on any land!");
    }

    public void ItemInteract()
    {
        if (selectedObject != null)
        {
            selectedObject.Pickup();
        }
    }

    public void ItemKeep()
    {
        if (InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Item))
        {
            InventoryManager.Instance.HandToInventory(InventorySlot.InventoryType.Item);
            return;
        }
    }
}