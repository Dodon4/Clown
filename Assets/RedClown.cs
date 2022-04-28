using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedClown : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform TrashPoint;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetTrash()
    {
        transform.position = TrashPoint.position;
        transform.rotation = TrashPoint.rotation;
    }    
}
