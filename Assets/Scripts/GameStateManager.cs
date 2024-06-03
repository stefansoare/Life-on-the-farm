using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameStateManager : MonoBehaviour, ITimeTracker
{
    public static GameStateManager Instance { get; private set; }
    bool fadedOut;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        TimeManager.Instance.RegisterTracker(this);
    }

    public void ClockUpdate(GameTimestamp time)
    {
        CheckShipping(time);
        CheckFarm(time);

        if (time.hour == 0 && time.minute == 0)
        {
            ResetDay();
        }
    }

    void ResetDay()
    {
        Debug.Log("Day reset");
        foreach (NPCRelationshipState npc in RelationshipStats.relationships)
        {
            npc.hasTalkedToday = false;
        }
    }

    void CheckShipping(GameTimestamp timestamp)
    {
        if (timestamp.hour == ShippingBin.hourToShip && timestamp.minute == 0)
        {
            ShippingBin.ShipItems();
        }
    }

    void CheckFarm(GameTimestamp timestamp)
    {
        if (SceneTransitionManager.Instance.currentLocation != SceneTransitionManager.Location.Farm)
        {
            if (LandManager.farmData == null) return;

            List<LandSaveState> landData = LandManager.farmData.Item1;
            List<CropSaveState> cropData = LandManager.farmData.Item2;

            if (cropData.Count == 0) return;

            for (int i = 0; i < cropData.Count; i++)
            {
                CropSaveState crop = cropData[i];
                LandSaveState land = landData[crop.landID];

                if (crop.cropState == CropBehaviour.CropState.Wilted) continue;

                land.ClockUpdate(timestamp);
                if (land.landStatus == Land.LandStatus.Watered)
                {
                    crop.Grow();
                }
                else if (crop.cropState != CropBehaviour.CropState.Seed)
                {
                    crop.Wither();
                }

                cropData[i] = crop;
                landData[crop.landID] = land;
            }

            LandManager.farmData.Item2.ForEach((CropSaveState crop) => {
                Debug.Log(crop.seedToGrow + "\n Health: " + crop.health + "\n Growth: " + crop.growth + "\n State: " + crop.cropState.ToString());
            });
        }
    }

    public void Sleep()
    {
        UIManager.Instance.FadeOutScreen();
        fadedOut = false;
        StartCoroutine(NextDay());
    }

    IEnumerator NextDay()
    {
        GameTimestamp nextDay = TimeManager.Instance.GetGameTimestamp();
        nextDay.day += 1;
        nextDay.hour = 6;
        nextDay.minute = 0;
        Debug.Log(nextDay.day + " " + nextDay.hour + ":" + nextDay.minute);

        while (!fadedOut)
        {yield return new WaitForSeconds(1f);
        }
        TimeManager.Instance.SkipTime(nextDay);
        SaveManager.Save(ExportSave());
        fadedOut = false;
        UIManager.Instance.ResetFadeDefaults();
    }

    public void OnFadeOutComplete()
    {
        fadedOut = true;
    }

    public GameSaveState ExportSave()
    {
        List<LandSaveState> landData = LandManager.farmData.Item1;
        List<CropSaveState> cropData = LandManager.farmData.Item2;

        ItemSlotData[] toolSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Tool);
        ItemSlotData[] itemSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Item);

        ItemSlotData equippedTool = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool);
        ItemSlotData equippedItem = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Item);

        GameTimestamp time = TimeManager.Instance.GetGameTimestamp();
        return new GameSaveState(landData, cropData, toolSlots, itemSlots, equippedItem, equippedTool, time, PlayerStats.Money, RelationshipStats.relationships);
    }

    public void LoadSave()
    {
        GameSaveState save = SaveManager.Load();
        TimeManager.Instance.LoadTime(save.timestamp);

        ItemSlotData[] toolSlots = ItemSlotData.DeserializeArray(save.toolSlots);
        ItemSlotData equippedTool = ItemSlotData.DeserializeData(save.equippedToolSlot);
        ItemSlotData[] itemSlots = ItemSlotData.DeserializeArray(save.itemSlots);
        ItemSlotData equippedItem = ItemSlotData.DeserializeData(save.equippedItemSlot);
        InventoryManager.Instance.LoadInventory(toolSlots, equippedTool, itemSlots, equippedItem);

        LandManager.farmData = new System.Tuple<List<LandSaveState>, List<CropSaveState>>(save.landData, save.cropData);

        PlayerStats.LoadStats(save.money);

        RelationshipStats.LoadStats(save.relationships);
    }
}