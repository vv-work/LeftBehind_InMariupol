using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MonoBehaviours
{
    public class MouseWorldPosition : MonoBehaviour
    {
        public static MouseWorldPosition Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public Vector3 GetPosition()
        {
            Ray mouseCameraRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            if (plane.Raycast(mouseCameraRay, out float enter))
                return mouseCameraRay.GetPoint(enter);

            return Vector3.zero;
        }
    }
}