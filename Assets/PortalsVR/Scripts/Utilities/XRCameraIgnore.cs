using UnityEngine;

namespace PortalsVR
{
    [RequireComponent(typeof(Camera))]
    public class XRCameraIgnore : MonoBehaviour
    {
        private void Awake()
        {
            UnityEngine.XR.XRDevice.DisableAutoXRCameraTracking(GetComponent<Camera>(), true);
        }
    }
}