using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace PortalsVR
{
    public class Portal : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Portal linkedPortal;

        [Header("Settings")]
        [SerializeField] private int recursionLimit = 5;
        [SerializeField] private float nearClipOffset = 0.05f;
        [SerializeField] private float nearClipLimit = 0.2f;
        [SerializeField] private float offsetAmount = 0.044f;
        [SerializeField] private float deformPower = 0.25f;
        [SerializeField] private GameObject[] clippedObjects;

        [Header("Internal References")]
        [SerializeField] private Transform screens;
        [SerializeField] private PortalInfoDictionary portalInfo;

        private Material firstRecursionMat;
        private List<PortalTraveller> trackedTravellers;
        #endregion

        #region Properties
        private int SideOfPortal(Vector3 pos)
        {
            return System.Math.Sign(Vector3.Dot(pos - transform.position, transform.forward));
        }
        private bool SameSideOfPortal(Vector3 posA, Vector3 posB)
        {
            return SideOfPortal(posA) == SideOfPortal(posB);
        }

        public bool IsActive { get; set; } = true;
        #endregion

        #region Methods
        private void Awake()
        {
            trackedTravellers = new List<PortalTraveller>();

            foreach (Camera.StereoscopicEye eye in portalInfo.Keys)
            {
                portalInfo[eye].screenMeshFilter = portalInfo[eye].screen.GetComponent<MeshFilter>();
                portalInfo[eye].screen.material.SetInt("displayMask", 1);
                portalInfo[eye].viewTexture = new RenderTexture(XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight, 24);
                portalInfo[eye].camera.targetTexture = portalInfo[eye].viewTexture;
                linkedPortal.portalInfo[eye].screen.material.SetTexture("_MainTex", portalInfo[eye].viewTexture);
            }
        }
        private void LateUpdate()
        {
            HandleTravellers();
        }

        private void OnTriggerEnter(Collider other)
        {
            PortalTraveller traveller = other.GetComponent<PortalTraveller>();
            if (traveller && !traveller.InPortal)
            {
                OnTravellerEnterPortal(traveller);
                traveller.InPortal = true;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            PortalTraveller traveller = other.GetComponent<PortalTraveller>();
            if (traveller && trackedTravellers.Contains(traveller))
            {
                trackedTravellers.Remove(traveller);
                traveller.InPortal = false;
            }

            portalInfo[Camera.StereoscopicEye.Left].meshDeformer.ClearDeformingForce();
            portalInfo[Camera.StereoscopicEye.Right].meshDeformer.ClearDeformingForce();

            foreach (GameObject clippedObject in clippedObjects)
            {
                clippedObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            portalInfo[Camera.StereoscopicEye.Left].eye.Portals.Add(this);
            portalInfo[Camera.StereoscopicEye.Right].eye.Portals.Add(this);
        }
        private void OnDisable()
        {
            portalInfo[Camera.StereoscopicEye.Left].eye.Portals.Remove(this);
            portalInfo[Camera.StereoscopicEye.Right].eye.Portals.Remove(this);
        }

        public void Render(Camera.StereoscopicEye eye)
        {
            if (!CameraUtility.VisibleFromCamera(linkedPortal.portalInfo[eye].screen, portalInfo[eye].eye.Camera) || !IsActive)
            {
                return;
            }

            var localToWorldMatrix = portalInfo[eye].alias.localToWorldMatrix;
            var renderPositions = new Vector3[recursionLimit];
            var renderRotations = new Quaternion[recursionLimit];

            int startIndex = 0;
            portalInfo[eye].camera.projectionMatrix = portalInfo[eye].eye.Camera.projectionMatrix;
            for (int i = 0; i < recursionLimit; i++)
            {
                if (i > 0)
                {
                    if (!CameraUtility.BoundsOverlap(portalInfo[eye].screenMeshFilter, linkedPortal.portalInfo[eye].screenMeshFilter, portalInfo[eye].camera))
                    {
                        break;
                    }
                }
                localToWorldMatrix = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * localToWorldMatrix;
                int renderOrderIndex = recursionLimit - i - 1;
                renderPositions[renderOrderIndex] = localToWorldMatrix.GetColumn(3);
                renderRotations[renderOrderIndex] = localToWorldMatrix.rotation;

                portalInfo[eye].camera.transform.SetPositionAndRotation(renderPositions[renderOrderIndex], renderRotations[renderOrderIndex]);
                startIndex = renderOrderIndex;
            }

            linkedPortal.portalInfo[eye].screen.material.SetInt("displayMask", 0);
            for (int i = startIndex; i < recursionLimit; i++)
            {
                portalInfo[eye].camera.transform.SetPositionAndRotation(renderPositions[i], renderRotations[i]);

                SetNearClipPlane(eye);
                portalInfo[eye].camera.Render();

                if (i == startIndex)
                {
                    linkedPortal.portalInfo[eye].screen.material.SetInt("displayMask", 1);
                }
            }
        }

        private void HandleTravellers()
        {
            if (!IsActive) return;

            for (int i = 0; i < trackedTravellers.Count; i++)
            {
                PortalTraveller traveller = trackedTravellers[i];

                Transform travellerT = traveller.Target;
                var m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

                Vector3 offsetFromPortal = travellerT.position - transform.position;
                int portalSide = Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
                int portalSideOld = Math.Sign(Vector3.Dot(traveller.PreviousOffsetFromPortal, transform.forward));

                if (portalSide != portalSideOld)
                {
                    var positionOld = travellerT.position;
                    var rotOld = travellerT.rotation;

                    traveller.Teleport(transform, linkedPortal.transform, m.GetColumn(3), m.rotation);
                    linkedPortal.OnTravellerEnterPortal(traveller);
                    trackedTravellers.RemoveAt(i);

                    i--;
                }
                else
                {
                    traveller.PreviousOffsetFromPortal = offsetFromPortal;
                }

                AddDeformForce(travellerT.position);

                foreach (GameObject clippedObject in clippedObjects)
                {
                    clippedObject.SetActive(SameSideOfPortal(clippedObject.transform.position, travellerT.position));
                }
            }
        }
        private void OnTravellerEnterPortal(PortalTraveller traveller)
        {
            if (!trackedTravellers.Contains(traveller))
            {
                traveller.PreviousOffsetFromPortal = traveller.Target.position - transform.position;
                trackedTravellers.Add(traveller);

                AddDeformForce(traveller.Target.position);

                foreach (GameObject clippedObject in clippedObjects)
                {
                    clippedObject.SetActive(SameSideOfPortal(clippedObject.transform.position, traveller.Target.position));
                }
            }
        }
        private void AddDeformForce(Vector3 point)
        {
            portalInfo[Camera.StereoscopicEye.Left].meshDeformer.AddDeformingForce(point, deformPower, SideOfPortal(point) > 0);
            portalInfo[Camera.StereoscopicEye.Right].meshDeformer.AddDeformingForce(point, deformPower, SideOfPortal(point) > 0);
        }

        private void SetNearClipPlane(Camera.StereoscopicEye eye)
        {
            Transform clipPlane = transform;
            int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - portalInfo[eye].camera.transform.position));

            Vector3 camSpacePos = portalInfo[eye].camera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
            Vector3 camSpaceNormal = portalInfo[eye].camera.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
            float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

            if (Mathf.Abs(camSpaceDst) > nearClipLimit)
            {
                Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);
                portalInfo[eye].camera.projectionMatrix = portalInfo[eye].eye.Camera.CalculateObliqueMatrix(clipPlaneCameraSpace);
            }
            else
            {
                portalInfo[eye].camera.projectionMatrix = portalInfo[eye].eye.Camera.projectionMatrix;
            }
        }

        [ContextMenu("Center")]
        public void Center()
        {
            if (Physics.Raycast(transform.position, screens.transform.up, out RaycastHit upHitInfo) && Physics.Raycast(transform.position, -screens.transform.up, out RaycastHit downHitInfo) && Physics.Raycast(transform.position, -screens.transform.right, out RaycastHit leftHitInfo) && Physics.Raycast(transform.position, screens.transform.right, out RaycastHit rightHitInfo))
            {
                float left = leftHitInfo.distance;
                float right = rightHitInfo.distance;
                float up = upHitInfo.distance;
                float down = downHitInfo.distance;

                if (up > down)
                {
                    transform.position += screens.transform.up * (up - down) / 2f;
                }
                else
                {
                    transform.position -= screens.transform.up * (down - up) / 2f;
                }

                if (right > left)
                {
                    transform.position += screens.transform.right * (right - left) / 2f;
                }
                else
                {
                    transform.position -= screens.transform.right * (left - right) / 2f;
                }
            }
        }
        #endregion

        #region Inner Classes
        [Serializable] public class PortalInfoDictionary : SerializableDictionaryBase<Camera.StereoscopicEye, PortalInfo> { }

        [Serializable] public class PortalInfo
        {
            public MeshDeformer meshDeformer;
            public MeshRenderer screen;
            public Camera camera;
            public MeshFilter screenMeshFilter;
            [Space]
            public Eye eye;
            public Transform alias;

            [HideInInspector] public RenderTexture viewTexture;
        }
        #endregion
    }
}