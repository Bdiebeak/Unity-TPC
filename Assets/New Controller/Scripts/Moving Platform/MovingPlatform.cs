using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float onPointTimeout = 3f;
    [Space]
    [SerializeField] private bool isMoveAllowed = true;
    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    [SerializeField] private float destinationThreshold = 0.01f;
    
    private Rigidbody _rigidbody;
    private int _destinationIndex = 1;
    private Coroutine _currentTimeoutCoroutine;

    private void Awake()
    {
        InitializeRigidbody();
        InitializeWaypoints();
    }

    private void InitializeRigidbody()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
    }

    private void InitializeWaypoints()
    {
        if (waypoints == null)
        {
            Debug.LogError("You didn't initialize Waypoints for this Moving Platform.", this);
            return;
        }
        
        if (waypoints.Count <= 1)
        {
            Debug.LogError("You have to set atleast 2 waypoints for Moving Platform.", this);
            return;
        }
        
        var startingIndex = FindNearestWaypoint();
        _rigidbody.position = waypoints[startingIndex].position;
        _destinationIndex = CalculateNextIndex(startingIndex);
    }

    private void FixedUpdate() => MovePlatform();
    private void MovePlatform()
    {
        if (isMoveAllowed == false) return;
        if (waypoints == null || waypoints.Count <= 1) return;
        
        // Required values
        var destinationPosition = waypoints[_destinationIndex].position;
        var currentPosition = _rigidbody.position;
        
        // Calculate move vector
        var moveDirection = (destinationPosition - currentPosition).normalized;
        var moveVector = moveDirection * speed * Time.deltaTime;
        
        // Move Rigidbody
        var newPosition = currentPosition + moveVector;
        _rigidbody.MovePosition(newPosition);
        
        // Check destination reach
        if (Vector3.Distance(_rigidbody.position, destinationPosition) < destinationThreshold)
        {
            _rigidbody.position = destinationPosition;
            _destinationIndex = CalculateNextIndex(_destinationIndex);   
            
            // Start timeout coroutine
            if (_currentTimeoutCoroutine != null) StopCoroutine(_currentTimeoutCoroutine);
            _currentTimeoutCoroutine = StartCoroutine(WaitTimeoutCoroutine());
        }
    }

    private IEnumerator WaitTimeoutCoroutine()
    {
        isMoveAllowed = false;
        yield return new WaitForSeconds(onPointTimeout);
        isMoveAllowed = true;
    }

    private int FindNearestWaypoint()
    {
        var minDistance = float.MaxValue;
        var nearestIndex = 0;
        for (var i = 0; i < waypoints.Count; i++)
        {
            var currentDistance = Vector3.Distance(_rigidbody.position, waypoints[i].position); 
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    private int CalculateNextIndex(int index)
    {
        index++;
        if (index >= waypoints.Count) index = 0;

        return index;
    }
}