using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<string, List<GameObject>> _poolDictionary = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, Transform> _parentGroups;

    protected override void Initialized()
    {
        base.Initialized();

        CacheParentGroups();
    }

    private void CacheParentGroups()
    {
        _parentGroups = new Dictionary<string, Transform>();

        string[] groupNames = { "Human Arrow", "Elf Arrow", "Devil Arrow", "Skeleton Arrow", "Red Fire Ball", "Green Fire Ball", "Blue Fire Ball", "DamageText", "Gold", "Diamond" };

        foreach (string name in groupNames)
        {
            GameObject parent = GameObject.Find(name);

            if (parent != null)
            {
                _parentGroups[name] = parent.transform;
            }
            else
            {
                Debug.LogWarning($"PoolManager : Parent '{name}' not found in scene.");
            }
        }
    }

    private string GetParentKeyFromName(string objName)
    {
        // 예: Human Arrow → Human Arrow / Red Fire Ball → Red Fire Ball
        if (objName.Contains("Human Arrow")) return "Human Arrow";
        if (objName.Contains("Elf Arrow")) return "Elf Arrow";
        if (objName.Contains("Devil Arrow")) return "Devil Arrow";
        if (objName.Contains("Skeleton Arrow")) return "Skeleton Arrow";
        if (objName.Contains("Red Fire Ball")) return "Red Fire Ball";
        if (objName.Contains("Green Fire Ball")) return "Green Fire Ball";
        if (objName.Contains("Blue Fire Ball")) return "Blue Fire Ball";

        if (objName.Contains("DamageText")) return "DamageText";
        if (objName.Contains("Gold")) return "Gold";
        if (objName.Contains("Diamond")) return "Diamond";

        return string.Empty;
    }

    public GameObject ActivateObj(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;

        if (!_poolDictionary.ContainsKey(key))
            _poolDictionary[key] = new List<GameObject>();

        foreach (var obj in _poolDictionary[key])
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.SetPositionAndRotation(position, rotation);
                obj.SetActive(true);

                return obj;
            }
        }

        GameObject newObj = Instantiate(prefab, position, rotation);
        newObj.name = key;

        _poolDictionary[key].Add(newObj);

        string parentKey = GetParentKeyFromName(key);

        if (_parentGroups != null && _parentGroups.TryGetValue(parentKey, out Transform parent))
        {
            newObj.transform.SetParent(parent);
        }

        return newObj;
    }

    public GameObject ActivateTextObj(GameObject prefab, Transform uiParent)
    {
        string key = prefab.name;

        if (!_poolDictionary.ContainsKey(key))
            _poolDictionary[key] = new List<GameObject>();

        foreach (var obj in _poolDictionary[key])
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.SetParent(uiParent, false); // UI 부모에 붙임 (false로 좌표 보정)
                obj.transform.position = Vector3.zero;
                obj.SetActive(true);

                return obj;
            }
        }

        // 최초 생성
        GameObject newObj = Instantiate(prefab);
        newObj.name = key;

        newObj.transform.SetParent(uiParent, false); // UI 부모에 바로 붙이기

        _poolDictionary[key].Add(newObj);

        return newObj;
    }

    public void DeactivateObj(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void PreloadDropItems(GameObject prefab, int amount)
    {
        string key = prefab.name;
        if (!_poolDictionary.ContainsKey(key))
            _poolDictionary[key] = new List<GameObject>();

        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.name = key;
            obj.SetActive(false);

            string parentKey = GetParentKeyFromName(key);
            if (_parentGroups != null && _parentGroups.TryGetValue(parentKey, out Transform parent))
                obj.transform.SetParent(parent);

            _poolDictionary[key].Add(obj);
        }
    }

    protected override void Clear()
    {
        base.Clear();

        _poolDictionary.Clear();
        _parentGroups.Clear();
    }
}