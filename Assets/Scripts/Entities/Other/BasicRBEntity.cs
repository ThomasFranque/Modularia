using UnityEngine;
using Projectiles;

namespace Entities
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicRBEntity : Entity
    {
        protected Rigidbody RB { get; private set; }

        protected override void OnAwake()
        {
            RB = GetComponent<Rigidbody>();
        }

        protected override void OnHit(RaycastHit hit, Projectile projectile, float damage)
        {
            Knockback(transform.position - hit.point, 200);
        }

        protected void Knockback(Vector3 dir, float amount, float yModifier = 1.5f)
        {
            //Vector3 torque;
            // torque.x = Random.Range(-200, 200);
            // torque.y = Random.Range(-200, 200);
            // torque.z = Random.Range(-200, 200);
            // RB.AddTorque(torque, ForceMode.Force);
            dir.y *= yModifier;
            RB.AddForce(dir * amount);
        }
    }
}