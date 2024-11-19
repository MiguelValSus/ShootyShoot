using UnityEngine;

public class PlayerAimMouse : MonoBehaviour
{
    [Header("Lookup values")]
    public Vector3 ForwardDirection;

    #region Mouse aiming
    private void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var rotationDir = mousePos - transform.position;

        rotationDir.z = 0;

        // Subtracting 90 degrees to align the sprite correctly
        var angle = Mathf.Atan2(rotationDir.y, rotationDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        ForwardDirection = transform.rotation * Vector3.up;
    }
    #endregion
}
