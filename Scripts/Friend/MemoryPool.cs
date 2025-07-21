using UnityEngine;
using System.Collections.Generic;

public class MemoryPool
{
    private class PoolItem
    {
        public bool isActive;      
        public GameObject gameObject;
    }

    private int increaseCount = 5;         
    private int maxCount;                   
    private int activeCount;               
    private GameObject poolObject;     
    private Transform poolParent;       
    private List<PoolItem> poolItemList;   

    public int MaxCount => maxCount;    
    public int ActiveCount => activeCount;  

    public MemoryPool(GameObject poolObject, Transform poolParent = null)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObject = poolObject;
        this.poolParent = poolParent;

        poolItemList = new List<PoolItem>();

        InstantiateObjects();
    }

    // increaseCount 만큼 오브젝트를 새로 생성
    public void InstantiateObjects()
    {
        maxCount += increaseCount;

        for (int i = 0; i < increaseCount; ++i)
        {
            PoolItem poolItem = new PoolItem();

            poolItem.isActive = false;
            poolItem.gameObject = GameObject.Instantiate(poolObject);
            poolItem.gameObject.SetActive(false);
            poolItem.gameObject.transform.SetParent(poolParent);

            poolItemList.Add(poolItem);
        }
    }

    // 모든 오브젝트 파괴
    public void DestroyObjects()
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            GameObject.Destroy(poolItemList[i].gameObject);
        }

        poolItemList.Clear();
    }

    // 풀 안에서 비활성 상태인 오브젝트를 찾아 활성화해서 반환
    // 만약 모든 오브젝트가 활성 상태라면 새로운 오브젝트를 추가 생성 후 반환
    public GameObject ActivatePoolItem()
    {
        if (poolItemList == null) return null;

        if (maxCount == activeCount) InstantiateObjects();

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.isActive == false)
            {
                activeCount++;

                poolItem.isActive = true;
                poolItem.gameObject.SetActive(true);

                return poolItem.gameObject;
            }
        }

        return null;
    }

    // 특정 오브젝트를 비활성화 처리
    public void DeactivatePoolItem(GameObject removeObject)
    {
        if (poolItemList == null || removeObject == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject == removeObject)
            {
                activeCount--;

                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);

                return;
            }
        }
    }

    // 활성 상태인 오브젝트 중 하나를 찾아 비활성화 처리
    public GameObject DeactivatePoolItem()
    {
        if (poolItemList == null) return null;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject.activeSelf == true)
            {
                activeCount--;

                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);

                return poolItem.gameObject;
            }
        }

        return null;
    }

   // 모든 활성 오브젝트를 비활성화 처리
    public void DeactivateAllPoolItems()
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject != null && poolItem.isActive == true)
            {
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);
            }
        }

        activeCount = 0;
    }
}