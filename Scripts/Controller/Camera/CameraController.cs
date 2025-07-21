using UnityEngine;

public class CameraController : Base_Controller
{
    private Transform _target;

    private Vector3 _cameraPosition;

    protected override void Initialized()
    {
        Setting();
    }

    private void FixedUpdate()
    {
        FindTarget();

        if (_target != null)
        {
            FollowCamera();
        }
    }

    private void Setting()
    {
        _cameraPosition = new Vector3(0.0f, 20.0f, -15.0f);
        transform.rotation = Quaternion.Euler(new Vector3(50.0f, 0.0f, 0.0f));
    }

    private void FindTarget()
    {
        if (_target != null) return;

        _target = FindAnyObjectByType<PlayerController>().gameObject.transform;
    }

    private void FollowCamera()
    {
        transform.position = _target.position + _cameraPosition;
    }
}