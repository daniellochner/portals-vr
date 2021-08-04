// Credit: https://github.com/andrewzimmer906/PocketPortalVR

using UnityEngine;

namespace PortalsVR
{
    public class PlaneMeshDeformer : MeshDeformer
    {
        private bool direction;

        public override void AddDeformingForce(Vector3 point, float force, bool direction)
        {
            this.direction = direction;
            AddDeformingForce(point, force);
        }

        protected override void AddForceToVertex(int i, Vector3 point, float force)
        {
            if (lockXEdges && (originalVertices[i].x == -0.5f ||
                originalVertices[i].x == 0.5f))
            {
                return;
            }

            if (lockYEdges && (originalVertices[i].y == -0.5f ||
                originalVertices[i].y == 0.5f))
            {
                return;
            }

            if (lockZEdges && (originalVertices[i].z == -0.5f ||
                originalVertices[i].z == 0.5f))
            {
                return;
            }

            Vector3 pointToVertex = originalVertices[i] - point;
            pointToVertex = new Vector3(
                pointToVertex.x * transform.localScale.x,
                pointToVertex.y * transform.localScale.y,
                pointToVertex.z * transform.localScale.z);

            float attenuatedForce = Mathf.Max(Mathf.Abs(force) - pointToVertex.magnitude, 0.1f);
            Vector3 transformForce = new Vector3(0, 0, (this.direction ? -1 : 1) * attenuatedForce / transform.localScale.z);

            displacedVertices[i] = originalVertices[i] + (transformForce);
        }
    }
}