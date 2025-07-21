using UnityEngine;

public class Ground : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Start()
    {
        Setting();
    }

    private void Setting()
    {
        // GameObject의 컴포넌트 가져오기
        _rigidbody = GetComponent<Rigidbody>();

        // Rigidbody 설정
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
}