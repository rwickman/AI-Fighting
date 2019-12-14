using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed = 500f;
    public bool isMoveable = true;
    public float maxSpeed = 5;
    public float stoppingLerpTerm = 0.01f;
    public float dirMoveFactor = 1f;

    private float minVelocityMag = 0.005f;
    private Rigidbody rigidbody;
    private Sword sword;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        sword = GetComponentInChildren<Sword>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            sword.Attack();
        }
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        //bool isJumping = Input.GetKeyDown(KeyCode.Space)
        if ((h != 0 || v != 0) && isMoveable) {
            Move(h, v);
        } else if (rigidbody.velocity.magnitude > minVelocityMag)
        {
            rigidbody.velocity = Vector3.Slerp(rigidbody.velocity, Vector3.zero, stoppingLerpTerm * Time.deltaTime);
        }
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
    }

    void Move(float horizontalMove, float verticalMove) {
        rigidbody.AddRelativeForce(new Vector3(horizontalMove * dirMoveFactor, 0f, verticalMove * dirMoveFactor).normalized * speed * Time.deltaTime);
    }
}
