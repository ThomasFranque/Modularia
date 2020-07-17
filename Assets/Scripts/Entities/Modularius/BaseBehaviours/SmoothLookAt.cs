using System;
using System.Collections.Generic;
using Entities.Modularius.ComposedBehaviours;
using UnityEngine;

namespace Entities.Modularius.BaseBehaviours
{
    public class SmoothLookAt : MonoBehaviour
    {
        [SerializeField, ReadOnly] private Transform _target = default;
        [SerializeField, ReadOnly] private FollowType _type = default;
        [SerializeField, ReadOnly] private float _speed = default;
        [SerializeField, ReadOnly] private bool _lockY = default;
        public float Speed { get => _speed; set => _speed = value; }

        private void Awake()
        {
            this.enabled = false;
        }

        private void Update()
        {
            LookAction.Invoke();
        }

        private void LerpLookAt()
        {
            Vector3 targetPosition = _target.transform.position;

            if (_lockY) targetPosition.y = transform.position.y;

            Vector3 lookRotation = targetPosition - transform.position;
            if (lookRotation == Vector3.zero) return;

            Quaternion targetRotation = Quaternion.LookRotation(lookRotation);

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _speed * Time.deltaTime);
        }

        // From https://docs.unity3d.com/ScriptReference/Vector3.RotateTowards.html
        private void LinearLookAt()
        {
            Vector3 targetPosition = _target.transform.position;
            if (_lockY) targetPosition.y = transform.position.y;

            // Determine which direction to rotate towards
            Vector3 targetDirection = (targetPosition - transform.position).normalized;

            // The step size is equal to speed times frame time.
            float singleStep = _speed * Time.deltaTime;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        public void StartLooking(Transform target, FollowType type, float speed = 2.0f, bool lockY = true)
        {
            _target = target;
            _lockY = lockY;
            _speed = speed;
            _type = type;

            if (type == FollowType.Lerp)
                LookAction = LerpLookAt;
            else
                LookAction = LinearLookAt;

            this.enabled = true;
        }

        public void StopLooking()
        {
            this.enabled = false;
        }

        private Action LookAction;
    }
}