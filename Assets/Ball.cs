using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody _rigid;

    public float speed;
    // Start is called before the first frame update
    void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
        _rigid.AddForce(0, - 1000 * speed, 0);
        Destroy( gameObject, 5f);
    }
    
}
