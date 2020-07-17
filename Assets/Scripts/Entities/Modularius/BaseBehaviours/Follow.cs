using System;
using Entities.Modularius.ComposedBehaviours;
using UnityEngine;

namespace Entities.Modularius.BaseBehaviours
{
    public class Follow : MonoBehaviour
    {
        [SerializeField, ReadOnly] private Transform _target = default;
        [SerializeField, ReadOnly] private FollowType _type = default;
        [SerializeField, ReadOnly] private float _speed = default;
        public float Speed { get => _speed; set => _speed = value; }

        private void Awake()
        {
            this.enabled = false;
        }

        private void LateUpdate()
        {
            FollowAction.Invoke();
        }

        private void LerpFollow()
        {
            Vector3 targetPosition = _target.transform.position;
            targetPosition.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position,
                targetPosition,
                Time.deltaTime * _speed);
        }

        private void LinearFollow()
        {
            Vector3 targetPosition = _target.transform.position;
            targetPosition.y = transform.position.y;
            transform.position =
                Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
        }

        public void StartFollowing(Transform target, FollowType type, float speed = 2.0f)
        {
            _target = target;
            _speed = speed;
            _type = type;

            if (type == FollowType.Lerp)
                FollowAction = LerpFollow;
            else
                FollowAction = LinearFollow;

            this.enabled = true;
        }

        public void StopFollowing()
        {
            this.enabled = false;
        }

        private Action FollowAction;

    }
}