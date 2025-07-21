
using System.Collections.Generic;
using System.Text;

public class UserData
{
    public string Info = string.Empty;
    public Dictionary<string, int> Inventory = new();
    public List<string> Equipment = new();

    // 데이터를 디버깅하기 위한 함수입니다.(Debug.Log(UserData);)
    public override string ToString()
    {
        StringBuilder result = new StringBuilder();

        result.AppendLine($"Info : {Info}");
        result.AppendLine($"Inventory");

        foreach (var itemKey in Inventory.Keys)
        {
            result.AppendLine($"| {itemKey} : {Inventory[itemKey]}개");
        }

        result.AppendLine($"Equipment");

        foreach (var equip in Equipment)
        {
            result.AppendLine($"| {equip}");
        }

        return result.ToString();
    }
}

public class BackendGameData : Singleton<BackendGameData>
{
    public static UserData UserData;

    private string _gameDataRowInDate = string.Empty;

    private void Awake()
    {
        if (UserData == null)
        {
            UserData = new UserData();
        }
    }

    public void SaveGameData()
    {
        StatUpgradeManager.Instance.SaveStatsToBackend();
        CollectionManager.Instance.SaveCollectionDataToBackend();
        InventoryManager.Instance.SaveInventoryToBackend();
        DrawingPanelManager.Instance.SaveGachaDataToBackend();
        GameManager.Instance.SaveGameDataToBackend();
    }

    public void LoadGameData()
    {
        StatUpgradeManager.Instance.LoadStatsFromBackend();
        CollectionManager.Instance.LoadCollectionDataFromBackend();
        InventoryManager.Instance.LoadInventoryFromBackend();
        DrawingPanelManager.Instance.LoadGachaDataFromBackend();
        GameManager.Instance.LoadGameDataFromBackend();
    }

    public void GameDataUpdate()
    {
        StatUpgradeManager.Instance.UpdateStatsToBackend();
        CollectionManager.Instance.UpdateCollectionDataToBackend();
        InventoryManager.Instance.UpdateInventoryToBackend();
        DrawingPanelManager.Instance.UpdateGachaDataToBackend();
        GameManager.Instance.UpdateGameDataToBackend();
    }
}