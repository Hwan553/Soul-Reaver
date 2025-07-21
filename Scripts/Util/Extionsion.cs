using System.Collections.Generic;
using UnityEngine;

// 확정 메서드를 정의하는 정적 클래스
public static class Extionsion
{
    // GameObject에 특정 타입의 컴포넌트를 가져오거나, 없으면 추가하는 확장 메서드
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        // GameObject에서 T 타입의 컴포넌트를 가져온다.
        T component = obj.GetComponent<T>();

        // 컴포넌트가 없으면 추가
        if (component == null)
            obj.AddComponent<T>();

        // 가져오거나 추가한 컴포넌트 반환
        return component;
    }

    // 주어진 GameObject를 기준으로 특정 레이어 내 가장 가까운 타겟 GameObject를 찾는 기능
    public static GameObject GetNearestTarget(this GameObject gameObject, string layerName, float radius)
    {
        // 지정한 레이어 이름을 사용해 해당 레이어에 대한 LayerMask를 가져옴
        LayerMask layerMask = LayerMask.GetMask(layerName);

        // 현재 GameObject의 위치를 중심으로 주어진 반지름(radius) 안에 있는 Collider들을 감지
        // 해당 레이어(layerMask)에 포함된 오브젝트들만 탐지됨
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, radius, layerMask);

        // 가장 가까운 오브젝트를 저장할 변수. 처음에는 null로 초기화
        Collider shortObject = null;

        // 만약 탐지된 Collider가 하나라도 있다면
        if (colliders.Length > 0)
        {
            // 기준이 되는 GameObject 위치와 첫 번째 Collider 간의 거리 측정
            float shortDistance1 = Vector3.Distance(gameObject.transform.position, colliders[0].transform.position);

            // 일단 첫 번째 Collider를 가장 가까운 것으로 초기 설정
            shortObject = colliders[0];

            // 나머지 Collider들을 반복하면서 더 가까운 Collider가 있는지 검사
            foreach (Collider collider in colliders)
            {
                // 현재 검사 중인 Collider와의 거리 측정
                float shortDistance2 = Vector3.Distance(gameObject.transform.position, collider.transform.position);

                // 더 짧은 거리가 발견되면 그것을 가장 가까운 오브젝트로 갱신
                if (shortDistance1 > shortDistance2)
                {
                    shortDistance1 = shortDistance2;
                    shortObject = collider;
                }
            }
            // 가장 가까운 Collider의 GameObject를 반환
            return shortObject.gameObject;
        }
        else
        {
            // 탐지된 오브젝트가 하나도 없을 경우 null 반환
            return null;
        }
    }

    // 주어진 GameObject를 기준으로 특정 레이어 내 가장 가까운 타겟 GameObject를 찾는 기능
    public static List<GameObject> GetRandomTarget(this GameObject gameObject, string layerName, float radius, int minCount = 3, int maxCount = 10)
    {
        // 지정한 레이어 이름을 사용해 해당 레이어에 대한 LayerMask를 가져옴
        LayerMask layerMask = LayerMask.GetMask(layerName);

        // 현재 GameObject의 위치를 중심으로 주어진 반지름(radius) 안에 있는 Collider들을 감지
        // 해당 레이어(layerMask)에 포함된 오브젝트들만 탐지됨
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, radius, layerMask);

        // 감지된 타겟 목록
        List<GameObject> targetList = new List<GameObject>();

        // 타겟 후보 목록을 생성
        foreach (var col in colliders)
        {
            targetList.Add(col.gameObject);
        }

        // 선택할 타겟 수 결정 (최소값보다 적게 있으면 전체 반환)
        int countToSelect = Mathf.Min(targetList.Count, Random.Range(minCount, maxCount + 1));

        // 선택된 타겟 목록
        List<GameObject> selectedTargets = new List<GameObject>();

        // 랜덤으로 타겟 선택 (중복 없음)
        for (int i = 0; i < countToSelect; i++)
        {
            int randIndex = Random.Range(0, targetList.Count);
            selectedTargets.Add(targetList[randIndex]);
            targetList.RemoveAt(randIndex); // 중복 방지
        }

        return selectedTargets;
    }
}