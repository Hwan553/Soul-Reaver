using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsCollectorManager : Singleton<GoodsCollectorManager>
{
    private readonly Queue<GoodsItem> _collectionQueue = new();
    [SerializeField] private int maxCollectPerFrame = 30; // 프레임당 최대 수집 처리 개수

    private void Update()
    {
        int processed = 0;

        while (_collectionQueue.Count > 0 && processed < maxCollectPerFrame)
        {
            var item = _collectionQueue.Dequeue();
            item.DoCollect(); // 실제 수집 처리
            processed++;
        }
    }

    public void RequestCollect(GoodsItem item)
    {
        if (!_collectionQueue.Contains(item))
            _collectionQueue.Enqueue(item);
    }
}

