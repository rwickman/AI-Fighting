using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed = 10f;
    public bool isMoveable = true;
    //public float maxSpeed = 5;
    //public float stoppingLerpTerm = 0.01f;
    //public float dirMoveFactor = 1f;
    public float jumpForce = 3f;

    private float minVelocityMag = 0.5f;
    private Rigidbody rigidbody;
    private Sword sword;
    private GroundDetection groundDetection;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        sword = GetComponentInChildren<Sword>();
        groundDetection = transform.Find("Feet").GetComponent<GroundDetection>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (Input.GetButton("Fire1"))
        {
            sword.Attack();
        }
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool isJumping = Input.GetKey(KeyCode.Space);

        if ((h != 0 || v != 0 || isJumping) && isMoveable && groundDetection.isGrounded)
        {
            Move(h, v, isJumping);
        }
        //else if (rigidbody.velocity.magnitude <= minVelocityMag)
        //{
        //    rigidbody.velocity = Vector3.Slerp(rigidbody.velocity, Vector3.zero, stoppingLerpTerm * Time.deltaTime);
        //}
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, Camera.main.transform.eulerAngles.y, transform.localEulerAngles.z);
        //rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
    }

    void Move(float horizontalMove, float verticalMove, bool isJumping)
    {
        int hMove = 0;
        int vMove = 0;
        if (horizontalMove != 0)
        {
            hMove = horizontalMove > 0 ? 1 : -1;
        }
        if (verticalMove != 0)
        {
            vMove = verticalMove > 0 ? 1 : -1;
        }

        Vector3 moveDir = new Vector3(hMove, 0f, vMove).normalized;
        if (isJumping)
        {
            Debug.Log("Jumpping");
            moveDir += Vector3.up;
            rigidbody.AddRelativeForce(moveDir * jumpForce * Time.deltaTime, ForceMode.Impulse);
        }
        else
        {
            transform.Translate(moveDir * speed * Time.deltaTime);
        }
        
        //rigidbody.velocity = new Vector3(hMove, 0f, vMove) * speed * Time.deltaTime;
        //rigidbody.AddRelativeForce(new Vector3(horizontalMove * dirMoveFactor, 0f, verticalMove * dirMoveFactor).normalized * speed * Time.deltaTime);
    }


}