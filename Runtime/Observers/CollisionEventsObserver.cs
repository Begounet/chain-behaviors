using AUE;
using UnityEngine;

namespace ChainBehaviors.Observers
{
    public class CollisionEventsObserver : MonoBehaviour
    {
        [System.Serializable]
        public class TriggerEvents
        {
            [SerializeField]
            private AUEEvent<Collider> _onTriggerEnter = null;
            public AUEEvent<Collider> OnTriggerEnter => _onTriggerEnter;

            [SerializeField]
            private AUEEvent<Collider> _onTriggerExit = null;
            public AUEEvent<Collider> OnTriggerExit => _onTriggerExit;
        }

        [System.Serializable]
        public class CollisionEvent
        {
            [SerializeField]
            private AUEEvent<Collider> _collider = null;
            public AUEEvent<Collider> Collider => _collider;

            [SerializeField]
            private AUEEvent<Collision> _collision = null;
            public AUEEvent<Collision> Collision => _collision;

            public void Raise(Collision collision)
            {
                _collider.Invoke(collision.collider);
                _collision.Invoke(collision);
            }
        }

        [System.Serializable]
        public class CollisionEvents
        {
            [SerializeField]
            private CollisionEvent _onCollisionEnter = null;
            public CollisionEvent OnCollisionEnter => _onCollisionEnter;

            [SerializeField]
            private CollisionEvent _onCollisionExit = null;
            public CollisionEvent OnCollisionExit => _onCollisionExit;
        }

        [SerializeField]
        private TriggerEvents _triggerEvents = null;

        [SerializeField]
        private CollisionEvents _collisionEvents = null;


        private void OnTriggerEnter(Collider other) => _triggerEvents.OnTriggerEnter.Invoke(other);

        private void OnTriggerExit(Collider other) => _triggerEvents.OnTriggerExit.Invoke(other);

        private void OnCollisionEnter(Collision collision) => _collisionEvents.OnCollisionEnter.Raise(collision);

        private void OnCollisionExit(Collision collision) => _collisionEvents.OnCollisionExit.Raise(collision);
    }
}