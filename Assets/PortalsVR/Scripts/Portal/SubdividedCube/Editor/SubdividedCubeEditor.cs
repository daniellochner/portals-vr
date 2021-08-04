// Credit: https://github.com/andrewzimmer906/PocketPortalVR

using UnityEngine;
using UnityEditor;

namespace PortalsVR
{
    [CustomEditor(typeof(SubdividedCube))]
    [RequireComponent(typeof(MeshFilter))]
    public class SubdividedCubeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SubdividedCube cubeDivide = (SubdividedCube)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Render"))
            {
                cubeDivide.RenderCube();
            }
        }
    }
}