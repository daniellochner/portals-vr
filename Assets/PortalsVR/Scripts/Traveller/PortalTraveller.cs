using UnityEngine;

namespace PortalsVR
{
    public class PortalTraveller : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Transform target;
        #endregion

        #region Properties
        public Transform Target => target;

        public Vector3 PreviousOffsetFromPortal { get; set; }
        public bool InPortal { get; set; }
        #endregion

        #region Methods
        public virtual void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
        {
            transform.position += pos - target.position;
            Physics.SyncTransforms();
        }
        #endregion
    }
}