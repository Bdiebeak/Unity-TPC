using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public bool canShoot = true;

    public float range = 100f;
    public float shootingDelay = 10f;

    public ParticleSystem[] shootParticles;
    public ParticleSystem bulletCollisionParticles;
    public LayerMask layerMask;

    private Transform mainCameraTransform;
    private float currentTimeDelay;

    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    /// <summary>
    /// Функция, осуществляющая выстрел
    /// </summary>
    /// <param name="targetPosition"> Конечная точка стрельбы </param>
    public void Shoot(Vector3 targetPosition)
    {
        if (!canShoot) return;

        if (Time.time >= currentTimeDelay)
        {
            currentTimeDelay = Time.time + 1 / shootingDelay;

            foreach (var particle in shootParticles)
            {
                particle.Play();
            }

            Vector3 direction = (targetPosition - mainCameraTransform.position).normalized;

            RaycastHit hit;
            if (Physics.Raycast(mainCameraTransform.position, direction, out hit, range, layerMask))
            {
                bulletCollisionParticles.transform.position = hit.point;
                bulletCollisionParticles.transform.LookAt(mainCameraTransform);

                bulletCollisionParticles.Play();
            }
        }
    }
}
