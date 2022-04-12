using UnityEngine;

public class FlightPhysics : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private Rigidbody playerRigidbody;
    
    private void Awake() => InitializeComponents();
    private void InitializeComponents()
    {
        if (playerRigidbody == null) playerRigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        playerRigidbody.freezeRotation = true;
        playerRigidbody.useGravity = false;
    }
}