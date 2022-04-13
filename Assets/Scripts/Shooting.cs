using UnityEngine;

namespace Bdiebeak.TPC.Shooting
{
    public class Shooting : MonoBehaviour
    {
        public bool canShoot = true;

        public float range = 100f;
        public float shootingDelay = 10f;

        public ParticleSystem[] shootParticles;
        public ParticleSystem bulletCollisionParticles; // ToDo: change this collision particles on auto from particle system
        public LayerMask layerMask;

        private Transform mainCameraTransform;
        private float currentTimeDelay;

        private void Awake() => InitializeComponents();
        private void InitializeComponents()
        {
            mainCameraTransform = UnityEngine.Camera.main.transform;
        }

        /// <summary>
        /// Функция, осуществляющая выстрел
        /// </summary>
        /// <param name="targetPosition"> Конечная точка стрельбы </param>
        public void Shoot(Vector3 targetPosition)
        {
            if (canShoot == false) return;

            if (Time.time >= currentTimeDelay)
            {
                currentTimeDelay = Time.time + 1 / shootingDelay;

                foreach (var particle in shootParticles)
                {
                    particle.Play();
                }

                var direction = (targetPosition - mainCameraTransform.position).normalized;
                if (Physics.Raycast(mainCameraTransform.position, direction, out var hit, range, layerMask))
                {
                    bulletCollisionParticles.transform.position = hit.point;
                    bulletCollisionParticles.transform.LookAt(mainCameraTransform);

                    bulletCollisionParticles.Play();
                }
            }
        }
    }
}