using System.Collections.Generic;
using UnityEngine;

public class QuestUIManager : MonoBehaviour
{
    [SerializeField] private GameObject questSlotPrefab;
    [SerializeField] private Transform contentParent;

    public void RefreshUI(List<Quest> questList)
    {
        // 기존 UI 정리
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // 퀘스트 슬롯 생성
        foreach (var quest in questList)
        {
            GameObject slotGO = Instantiate(questSlotPrefab, contentParent);
            QuestSlotUI slotUI = slotGO.GetComponent<QuestSlotUI>();
            slotUI.Setup(quest);
        }
    }
}
