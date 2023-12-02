using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public bool ProcessHorizontalAxisInput()
    {
        var horizontalAxis = Input.GetAxis("Horizontal");
        if (horizontalAxis != 0)
        {
            var rotation = horizontalAxis * RotationSpeed;
            _desiredYRotation = rotation;

            if (horizontalAxis > 0)
            {
                RotateRightWheelsByAxis("Horizontal");
            }
            else
            {
                RotateLeftWheelsByAxis("Horizontal");
            }
        }

        return horizontalAxis != 0;
    }

    public bool ProcessVericalAxisInput()
    {
        var verticalAxis = Input.GetAxis("Vertical");
        if (verticalAxis != 0)
        {
            var movement = Time.deltaTime * (verticalAxis * MovementSpeed);
            transform.Translate(new Vector3(0f, 0f, movement));
            RotateLeftWheelsByAxis("Vertical");
            RotateRightWheelsByAxis("Vertical");
        }

        return verticalAxis != 0;
    }*/
}
