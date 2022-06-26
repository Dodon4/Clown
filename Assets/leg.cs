using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leg : MonoBehaviour
{
    public float rotationSpeed;
    private float rotation = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotation += rotationSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(rotation/2, rotation, rotation);
    }
}
