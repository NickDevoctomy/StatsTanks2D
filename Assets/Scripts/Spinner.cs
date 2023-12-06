using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float SpeedX = 100.0f;
    public float SpeedY = 100.0f;
    public float SpeedZ = 100.0f;

    void Update()
    {
        transform.Rotate(
            SpeedX * Time.deltaTime,
            SpeedY * Time.deltaTime,
            SpeedZ * Time.deltaTime);
    }
}
