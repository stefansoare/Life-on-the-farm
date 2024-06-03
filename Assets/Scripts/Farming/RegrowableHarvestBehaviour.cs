using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegrowableHarvestBehaviour : InteractableObject
{
    CropBehaviour parentCrop; 

    public void SetParent(CropBehaviour parentCrop)
    {
        this.parentCrop = parentCrop;
    }

    public override void Pickup()
    {
        InventoryManager.Instance.EquipHandSlot(item);
        InventoryManager.Instance.RenderHand();
        parentCrop.Regrow();
    }
}