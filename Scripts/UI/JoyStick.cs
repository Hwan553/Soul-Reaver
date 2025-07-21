using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Vector3 _moveDirection;
    private Vector3 _touchPosition;
    private Vector3 _originPosition;

    private float _radius;

    public GameObject JoyStickObject;
    public GameObject HandlerObject;

    private void Start()
    {
        //안에 있는 핸들러가 갈수 있는거리 나누기 3은 범위 제한
        _radius = JoyStickObject.GetComponent<RectTransform>().sizeDelta.y / 4.0f;

        _originPosition = JoyStickObject.transform.position;

        SetActiveJoyStick(false);
    }

    private void SetActiveJoyStick(bool isActive)
    {
        JoyStickObject.SetActive(isActive);
        HandlerObject.SetActive(isActive);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        SetActiveJoyStick(true);

        _touchPosition = Input.mousePosition;
        JoyStickObject.transform.position = Input.mousePosition;
        HandlerObject.transform.position = Input.mousePosition;

    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 dargPosition = eventData.position;

        _moveDirection = (dargPosition - _touchPosition).normalized;

        float distance = (dargPosition - _originPosition).sqrMagnitude;

        Vector3 newPosition;

        //조이스틱이 반지름 안에 있는 경우
        if (distance < _radius)
        {
            newPosition = _touchPosition + (_moveDirection * distance);

        }
        else
        {
            newPosition = _touchPosition + (_moveDirection * _radius);
        }
        HandlerObject.transform.position = newPosition;

        // 조이스틱 이동 방향을 GameManager 이동 방향에 넣기
        GameManager.Instance.MoveDirection = new Vector3(_moveDirection.x, 0.0f, _moveDirection.y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _moveDirection = Vector2.zero;

        JoyStickObject.transform.position = _originPosition;
        HandlerObject.transform.position = _originPosition;

        GameManager.Instance.MoveDirection = _moveDirection; // 땟을때 이동 방향 GameManager 이동 방향에 넣기 반환

        SetActiveJoyStick(false);
    }
}