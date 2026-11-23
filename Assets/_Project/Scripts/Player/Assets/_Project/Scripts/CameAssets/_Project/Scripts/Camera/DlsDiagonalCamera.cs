using UnityEngine;

namespace LagosStreetLeague.CameraSystem
{
    [DisallowMultipleComponent]
    public class DlsDiagonalCamera : MonoBehaviour
    {
        [Header("Targets")]
        [SerializeField] private Transform primaryTarget;
        [SerializeField] private Transform secondaryTarget;
        [SerializeField, Range(0f, 1f)] private float secondaryWeight = 0.25f;
        [SerializeField] private Vector3 focusOffset = new Vector3(0f, 0.5f, 0f);

        [Header("Diagonal Orientation & Bounds")]
        [SerializeField] private float yawAngle = 30f;
        [SerializeField, Range(20f, 65f)] private float pitchAngle = 42f;
        [SerializeField] private float minPitchAngle = 25f;
        [SerializeField] private float maxPitchAngle = 60f;

        [Header("Zoom & Distance")]
        [SerializeField, Range(0f, 1f)] private float zoom = 0.45f;
        [SerializeField, Min(2f)] private float minDistance = 14f;
        [SerializeField, Min(2f)] private float maxDistance = 28f;

        [Header("Smoothing & Pitch Clamp")]
        [SerializeField, Min(0.01f)] private float followSmoothTime = 0.14f;
        [SerializeField, Min(0.01f)] private float zoomSmoothTime = 0.12f;
        [SerializeField] private bool clampFocusToPitchBounds = true;
        [SerializeField] private Vector2 pitchHalfExtents = new Vector2(34f, 52f);
        [SerializeField] private Vector3 pitchCenter = Vector3.zero;

        [Header("Optional Camera Child")]
        [SerializeField] private Transform cameraChild;

        private Vector3 _currentFocus;
        private Vector3 _focusVelocity;
        private float _currentZoom;
        private float _zoomVelocity;
        private bool _initialized;

        public void SetZoom(float normalizedZoom)
        {
            zoom = Mathf.Clamp01(normalizedZoom);
        }

        public float GetZoom() => zoom;

        public void SetTargets(Transform primary, Transform secondary = null)
        {
            primaryTarget = primary;
            secondaryTarget = secondary;
        }

        private void OnValidate()
        {
            minPitchAngle = Mathf.Clamp(minPitchAngle, 15f, 75f);
            maxPitchAngle = Mathf.Clamp(
                maxPitchAngle, minPitchAngle + 1f, 80f);

            pitchAngle = Mathf.Clamp(
                pitchAngle, minPitchAngle, maxPitchAngle);

            if (maxDistance < minDistance)
                maxDistance = minDistance;

            zoom = Mathf.Clamp01(zoom);
        }

        private void Start()
        {
            if (cameraChild == null && transform.childCount > 0)
                cameraChild = transform.GetChild(0);

            SnapToTarget();
        }

        public void SnapToTarget()
        {
            Vector3 desiredFocus = ComputeDesiredFocus();

            _currentFocus = desiredFocus;
            _focusVelocity = Vector3.zero;

            _currentZoom = Mathf.Clamp01(zoom);
            _zoomVelocity = 0f;

            ApplyRigTransform(_currentFocus, _currentZoom);
            _initialized = true;
        }

        private void LateUpdate()
        {
            float dt = Time.deltaTime;

            if (dt <= 0f)
                return;

            if (!_initialized)
            {
                SnapToTarget();
                return;
            }

            Vector3 desiredFocus = ComputeDesiredFocus();

            _currentFocus = Vector3.SmoothDamp(
                _currentFocus,
                desiredFocus,
                ref _focusVelocity,
                followSmoothTime,
                Mathf.Infinity,
                dt);

            _currentZoom = Mathf.SmoothDamp(
                _currentZoom,
                Mathf.Clamp01(zoom),
                ref _zoomVelocity,
                zoomSmoothTime,
                Mathf.Infinity,
                dt);

            ApplyRigTransform(_currentFocus, _currentZoom);
        }

        private Vector3 ComputeDesiredFocus()
        {
            Vector3 point = transform.position;

            if (primaryTarget != null && secondaryTarget != null)
            {
                point = Vector3.Lerp(
                    primaryTarget.position,
                    secondaryTarget.position,
                    secondaryWeight);
            }
            else if (primaryTarget != null)
            {
                point = primaryTarget.position;
            }
            else if (secondaryTarget != null)
            {
                point = secondaryTarget.position;
            }

            point += focusOffset;

            if (clampFocusToPitchBounds)
            {
                float minX = pitchCenter.x - pitchHalfExtents.x;
                float maxX = pitchCenter.x + pitchHalfExtents.x;
                float minZ = pitchCenter.z - pitchHalfExtents.y;
                float maxZ = pitchCenter.z + pitchHalfExtents.y;

                point.x = Mathf.Clamp(point.x, minX, maxX);
                point.z = Mathf.Clamp(point.z, minZ, maxZ);
            }

            return point;
        }

        private void ApplyRigTransform(
            Vector3 focusPoint,
            float zoomValue)
        {
            float clampedPitch = Mathf.Clamp(
                pitchAngle, minPitchAngle, maxPitchAngle);

            Quaternion rigRotation = Quaternion.Euler(
                clampedPitch, yawAngle, 0f);

            float distance = Mathf.Lerp(
                minDistance, maxDistance, zoomValue);

            if (cameraChild != null)
            {
                transform.position = focusPoint;
                transform.rotation = rigRotation;

                cameraChild.localPosition = new Vector3(
                    0f, 0f, -distance);

                cameraChild.localRotation = Quaternion.identity;
            }
            else
            {
                Vector3 forwardDir = rigRotation * Vector3.forward;

                transform.position =
                    focusPoint - forwardDir * distance;

                transform.rotation = rigRotation;
            }
        }
    }
}
