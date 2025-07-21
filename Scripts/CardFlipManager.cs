using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipManager : Singleton<CardFlipManager>
{
    private KillCountCardFlipButton KillCountCardFlipButton;

    public List<FlidCard> CardsToFlip = new List<FlidCard>();
    //텍스트는 3개만 넣을거임
    public List<TMP_Text> BuffText = new List<TMP_Text>();
    public List<BuffData> BuffData = new List<BuffData>();

    private int _flipCount = 6;

    private bool isFlipping = false;

    public GameObject FlipCardUI;
    public Button StartFlipButton;
    public Button EndFlipButton;

    private void Start()
    {
        KillCountCardFlipButton = FindAnyObjectByType<KillCountCardFlipButton>();

        FlipCardUI.SetActive(false);

        StartFlipButton.onClick.AddListener(GoFlipCards);

        EndFlipButton.onClick.AddListener(EndFlipCards);
    }

    void GoFlipCards()
    {
        if (KillCountCardFlipButton == null || !KillCountCardFlipButton.OpenButton)
        {
            Debug.Log("조건 부족. 버튼 작동안함.");
            return;
        }
        KillCountCardFlipButton.ResetCount();
        FlipCardUI.SetActive(true);
        StartCoroutine(FlipAllCardsMultipleTimes());
    }

    public void EndFlipCards()
    {
        //여기에 EndFlipCard() 넣고싶음
        KillCountCardFlipButton.ResetCount();
        FlipCardUI.SetActive(false);
    }

    IEnumerator FlipAllCardsMultipleTimes()
    {
        isFlipping = true;

        for (int i = 0; i < _flipCount; i++)
        {
            List<Coroutine> flipRoutines = new List<Coroutine>();

            foreach (var card in CardsToFlip)
            {
                flipRoutines.Add(StartCoroutine(card.FlipCard()));
            }

            // 모든 카드 회전이 끝날 때까지 대기
            yield return new WaitUntil(() => AllCardsDoneFlipping());

            yield return new WaitForSeconds(0.05f); // 살짝 텀 주기 원하면
        }

        isFlipping = false;

        AssignRandomBuffDescriptions();
    }

    private bool AllCardsDoneFlipping()
    {
        foreach (var card in CardsToFlip)
        {
            if (card.IsFlipping) return false;
        }
        return true;
    }

    void AssignRandomBuffDescriptions()
    {
        List<BuffData> copyList = new List<BuffData>(BuffData);

        for (int i = 0; i < BuffText.Count; i++)
        {
            if (copyList.Count == 0) break;

            int rand = Random.Range(0, copyList.Count);
            BuffData selected = copyList[rand];
            copyList.RemoveAt(rand);

            BuffText[i].text = selected.description;

            if (i < CardsToFlip.Count)
            {
                CardsToFlip[i].SetupCard(selected); // 여기서 BuffData 할당됨
            }
        }
    }



}
