using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Citadel
{
    public sealed class CameraController : MonoBehaviour
    {
        private const float MoveSpeed = 4f, ZoomSpeed = 1000f, RotateSpeed = 10f;
        
        [SerializeField] private Transform pivotTransform;

        [SerializeField] private InputActionReference moveAction, zoomAction, rotateAction;

        private void OnEnable()
        {
            moveAction.action.Enable();
            moveAction.action.performed += OnMove;
            
            zoomAction.action.Enable();
            zoomAction.action.performed += OnZoom;
            
            rotateAction.action.Enable();
            rotateAction.action.performed += OnRotate;
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
            moveAction.action.performed -= OnMove;
            
            zoomAction.action.Enable();
            zoomAction.action.performed -= OnZoom;
            
            rotateAction.action.Disable();
            rotateAction.action.performed -= OnRotate;
        }

        private void OnMove(InputAction.CallbackContext callbackContext)
        {
            Vector2 move = callbackContext.ReadValue<Vector2>() * Time.unscaledDeltaTime * MoveSpeed;
            pivotTransform.Translate(new Vector3(-move.x, 0f, -move.y), Space.Self);
        }

        private void OnZoom(InputAction.CallbackContext callbackContext)
        {
            float zoom = callbackContext.ReadValue<float>() * Time.unscaledDeltaTime * ZoomSpeed;
            transform.Translate(new Vector3(0f, 0f, zoom), Space.Self);
        }

        private void OnRotate(InputAction.CallbackContext callbackContext)
        {
            Vector2 rotate = callbackContext.ReadValue<Vector2>() * Time.unscaledDeltaTime * RotateSpeed;
            if (Math.Abs(rotate.x) > Math.Abs(rotate.y))
                rotate.y = 0;
            else
                rotate.x = 0;
            
            pivotTransform.Rotate(new Vector3(-rotate.y, rotate.x));

            Vector3 locked = pivotTransform.eulerAngles;
            locked.x = Mathf.Clamp(Mathf.DeltaAngle(0f, locked.x), -45f, 45f);
            locked.z = 0f;
            pivotTransform.eulerAngles = locked;
        }
    }
}