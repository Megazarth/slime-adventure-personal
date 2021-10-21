using Kaizen;
using System.Collections.Generic;
using UnityEngine;

namespace Slime
{
    public class CameraManager : SingletonComponent<CameraManager>
    {
        private List<GameObject> objects = new List<GameObject>(); // All the targets the camera needs to encompass.

        private float zoomSpeed;                      // Reference speed for the smooth damping of the orthographic size.
        private Vector3 moveVelocity;                 // Reference velocity for the smooth damping of the position.
        private Vector3 desiredPosition;              // The position the camera is moving towards.
        private float minX, minY, maxX, maxY;           // Camera boundary
        private Vector2 offset = new Vector2(4f, 4f);   // Camera boundary offset

        private float defaultMinSize;
        private Vector2 defaultOffset;

        public float dampTime = 0.2f;                 // Approximate time for the camera to refocus.
        public float screenEdgeBuffer = 4f;           // Space between the top/bottom most target and the screen edge.
        public float minSize = 5f;                    // The smallest orthographic size the camera can be.

        public Camera MainCamera { get; internal set; }

        protected override void Awake()
        {
            base.Awake();
            MainCamera = GetComponentInChildren<Camera>();

            defaultMinSize = minSize;
            defaultOffset = offset;
        }

        private void FixedUpdate()
        {
            if (objects.Count == 0)
                return;

            Move();
            //Zoom();
        }

        public void Move(bool snapping = false)
        {
            FindAveragePosition();
            if (snapping)
            {
                transform.position = desiredPosition;
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref moveVelocity, dampTime);
            }
        }


        private void FindAveragePosition()
        {
            var averagePos = new Vector3();
            var numTargets = 0;

            for (int i = 0; i < objects.Count; i++)
            {
                if (!objects[i].gameObject.activeSelf)
                    continue;

                averagePos += objects[i].transform.position;
                numTargets++;
            }

            if (numTargets > 0)
                averagePos /= numTargets;

            desiredPosition = new Vector3(Mathf.Clamp(averagePos.x, minX, maxX), Mathf.Clamp(averagePos.y, minY, maxY), transform.position.z);
        }


        public void Zoom(bool snapping = false)
        {
            var requiredSize = FindRequiredSize();
            if (snapping)
                MainCamera.orthographicSize = requiredSize;
            else
                MainCamera.orthographicSize = Mathf.SmoothDamp(MainCamera.orthographicSize, requiredSize, ref zoomSpeed, dampTime);
        }


        private float FindRequiredSize()
        {
            var desiredLocalPos = transform.InverseTransformPoint(desiredPosition);
            var size = 0f;

            for (int i = 0; i < objects.Count; i++)
            {
                if (!objects[i].gameObject.activeSelf)
                    continue;

                var targetLocalPos = transform.InverseTransformPoint(objects[i].transform.position);
                var desiredPosToTarget = targetLocalPos - desiredLocalPos;

                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / MainCamera.aspect);
            }

            size += screenEdgeBuffer;
            size = Mathf.Max(size, minSize);

            return size;
        }

        public void AddObjectToTrack(GameObject target)
        {
            objects.Add(target);
        }

        public void ClearTrackedObjects()
        {
            objects.Clear();
        }

        public void SetStartPositionAndSize()
        {
            FindAveragePosition();
            transform.position = desiredPosition;
            MainCamera.orthographicSize = FindRequiredSize();
        }

        public void SetCameraBoundary(Vector3 minTile, Vector3 maxTile)
        {
            minX = minTile.x + offset.x;
            minY = minTile.y + offset.y;
            maxX = maxTile.x - offset.x;
            maxY = maxTile.y - offset.y;
        }

        public void SetCustomOffset(Vector2 offset)
        {
            if (offset == Vector2.zero)
                offset = defaultOffset;

            this.offset = offset;
        }

        public void SetOrthographicMinSize(float size)
        {
            if (size == 0f)
                size = defaultMinSize;

            minSize = size;
            MainCamera.orthographicSize = minSize;
        }
    }
}