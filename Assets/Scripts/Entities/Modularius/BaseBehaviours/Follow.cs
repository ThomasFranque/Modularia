using System;
using Entities.Modularius.ComposedBehaviours;
using Pathfinding;
using UnityEngine;
using Grid = Pathfinding.Grid;

namespace Entities.Modularius.BaseBehaviours
{
    public class Follow : MonoBehaviour
    {
        [SerializeField, ReadOnly] private Transform _target = default;
        [SerializeField, ReadOnly] private FollowType _type = default;
        [SerializeField, ReadOnly] private float _speed = default;
        public float Speed { get => _speed; set => _speed = value; }
        private Grid _grid;

        private Vector3[] _path;

        private void Awake()
        {
            this.enabled = false;
        }

        private void LateUpdate()
        {
            _path = _grid.GetPath(transform.position, _target.position);
            FollowAction.Invoke();
        }

        private void LerpFollow()
        {
            if (_path.Length <= 1) return;
            Vector3 targetPosition = _path[1];
            targetPosition.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position,
                targetPosition,
                Time.deltaTime * _speed);
        }

        private void LinearFollow()
        {
            if (_path.Length == 1) return;
            Vector3 targetPosition = _path[1];
            targetPosition.y = transform.position.y;
            transform.position =
                Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
        }

        public void StartFollowing(Transform target, FollowType type, Grid pathfindGrid, float speed = 2.0f)
        {
            _target = target;
            _speed = speed;
            _type = type;
            _grid = pathfindGrid;

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

        private void OnDrawGizmos()
        {
            if (_path != null && _path.Length > 0)
            {
                Vector3 prev = _path[0];
                for (int i = 1; i < _path.Length; i++)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(prev, _path[i]);
                    prev = _path[i];
                }
            }
        }
    }
}