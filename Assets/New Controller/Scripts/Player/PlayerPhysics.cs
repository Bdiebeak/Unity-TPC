using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerPhysics : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private float rayOffset = 0.05f;
    [SerializeField] private float gravity;

    private bool _isGrounded;
    private Rigidbody _rigidbody;
    private RaycastHit _standingSurface;

    public bool IsGrounded => _isGrounded;
    public Vector3 GetSurfaceNormal() => _standingSurface.normal;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        CheckGround();
        
        _rigidbody.velocity += _rigidbody.transform.up * -gravity * Time.deltaTime;
    }

    private void CheckGround()
    {
        var rayDistance = playerCollider.height / 2 + rayOffset;
        var from = playerCollider.transform.TransformPoint(playerCollider.center);
        var to = new Vector3(from.x, from.y - rayDistance, from.z) - from;

        if (Physics.Raycast(from, to, out _standingSurface, rayDistance, groundLayers))
        {
            _isGrounded = true;
        }
        else
        {
            _standingSurface.normal = Vector3.up;
            _isGrounded = false;
        }
    }

    private void OnDrawGizmos()
    {
        var rayDistance = playerCollider.height / 2 + rayOffset;
        var from = playerCollider.transform.TransformPoint(playerCollider.center);
        var to = playerCollider.transform.TransformPoint(new Vector3(from.x, from.y - rayDistance, from.z) - from);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(from, to);
    }
}
