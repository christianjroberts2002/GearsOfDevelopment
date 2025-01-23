using UnityEngine;

public class DMMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] private float currentMoveSpeed;
    [SerializeField] float maxMoveSpeed;

    [SerializeField] float moveSpeedIncrement;

    [SerializeField] float lookSpeed;

    private Vector2 rotation = Vector2.zero;

    // Update is called once per frame

    private void Start()
    {
      currentMoveSpeed = moveSpeed;
    }
    void Update()
    {
        moveDM(moveSpeed);
        lookDM(lookSpeed);
    }

    void moveDM(float moveSpeed)
    {
        
        if (Input.GetKey(KeyCode.LeftShift) && currentMoveSpeed <= maxMoveSpeed)
        {
            {
                currentMoveSpeed += moveSpeedIncrement;
            }
        }
        else
        {
            if(currentMoveSpeed >= moveSpeed)
            {
                currentMoveSpeed -= moveSpeedIncrement + .5f;
            }
        }

        transform.Translate((Input.GetAxis("Horizontal") * currentMoveSpeed * Time.deltaTime), 0, Input.GetAxis("Vertical") * currentMoveSpeed * Time.deltaTime);
    }

    void lookDM(float lookSpeed)
    {
        rotation.y += Input.GetAxis("Mouse X");
        rotation.x += -Input.GetAxis("Mouse Y");
        rotation.x = Mathf.Clamp(rotation.x, -15f, 15f);
        //transform.eulerAngles = new Vector2(0, rotation.y) * lookSpeed;
        Camera.main.transform.localRotation = Quaternion.Euler(rotation.x * lookSpeed, rotation.y * lookSpeed, 0);
    }
}
