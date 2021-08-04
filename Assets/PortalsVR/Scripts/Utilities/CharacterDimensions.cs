using UnityEngine;

namespace PortalsVR
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterDimensions : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Transform head;

        private CharacterController characterController;
        #endregion

        #region Methods
        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }
        private void Update()
        {
            EvaluateDimensions();
        }
        private void OnEnable()
        {
            EvaluateDimensions();
        }

        public void EvaluateDimensions()
        {
            characterController.height = head.localPosition.y;
            characterController.center = new Vector3(head.localPosition.x, characterController.height / 2f + characterController.skinWidth, head.localPosition.z);
        }
        public void Recenter()
        {
            characterController.center = new Vector3(0, characterController.center.y, 0);
        }
        #endregion
    }
}