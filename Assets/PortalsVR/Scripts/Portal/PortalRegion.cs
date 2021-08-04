using UnityEngine;

namespace PortalsVR
{
    public class PortalRegion : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Portal[] activePortals;
        #endregion

        #region Methods
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                foreach (Portal portal in activePortals)
                {
                    portal.IsActive = true;
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                foreach (Portal portal in activePortals)
                {
                    portal.IsActive = false;
                }
            }
        }
        #endregion
    }
}