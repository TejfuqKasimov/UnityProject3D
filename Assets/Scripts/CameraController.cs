using UnityEngine;

public class PersonLook : MonoBehaviour
{
    [Space]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float smooth = 2f;
    [Space]
    [SerializeField] private float minClamp = -90f;
    [SerializeField] private float maxClamp = 90f;

    private Transform character;
    private Vector2 currentMouseLook;
    private Vector2 appliedMousedelta;

    private void Start() => character = GameObject.FindGameObjectWithTag("Player").transform;

    private void Update()
    {
        var smoothMouseDelta = Vector2.Scale(new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")), Vector2.one * sensitivity * smooth);

        appliedMousedelta = Vector2.Lerp(appliedMousedelta, smoothMouseDelta, 1 / smooth);

        currentMouseLook += appliedMousedelta;
        currentMouseLook.y = Mathf.Clamp(currentMouseLook.y, minClamp, maxClamp);

        character.localRotation = Quaternion.AngleAxis(currentMouseLook.x, Vector3.up);
        transform.localRotation = Quaternion.AngleAxis(-currentMouseLook.y, Vector3.right);
    }
}

