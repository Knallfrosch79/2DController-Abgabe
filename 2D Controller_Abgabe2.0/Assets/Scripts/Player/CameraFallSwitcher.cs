using UnityEngine;
using Unity.Cinemachine;

public class CameraFallOffset : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private CinemachineCamera cinemachineCam;
    [SerializeField] private CinemachineFollow followComponent;
    [SerializeField] private float fallThreshold = -2f;
    [SerializeField] private Vector3 fallOffset = new Vector3(0.3f, -1f, -10f);
    [SerializeField] private Vector3 normalOffset = new Vector3(0.3f, 1.6f, -10f);
    [SerializeField] private float lerpSpeed = 5f;

    private void Update()
    {
        Vector3 targetOffset = playerRb.linearVelocity.y < fallThreshold ? fallOffset : normalOffset;
        followComponent.FollowOffset = Vector3.Lerp(followComponent.FollowOffset, targetOffset, Time.deltaTime * lerpSpeed);
    }
}
