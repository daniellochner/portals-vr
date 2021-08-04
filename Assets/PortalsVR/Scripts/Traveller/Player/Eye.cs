using System.Collections.Generic;
using UnityEngine;

namespace PortalsVR
{
    [RequireComponent(typeof(Camera))]
    public class Eye : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Camera.StereoscopicEye eye;
        #endregion

        #region Properties
        public Camera Camera { get; private set; }
        public List<Portal> Portals { get; set; } = new List<Portal>();
        #endregion

        #region Methods
        private void Awake()
        {
            Camera = GetComponent<Camera>();
        }
        private void OnPreCull()
        {
            for (int i = 0; i < Portals.Count; i++)
            {
                Portals[i].Render(eye);
            }
        }
        #endregion
    }
}