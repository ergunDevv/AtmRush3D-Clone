//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SwerveMoment : MonoBehaviour
//{

//    private SwerveMechanic _swerveMechanic;

//    [SerializeField] private float swerveSpeed ;
    




//    void Awake()
//    {
//        _swerveMechanic = GetComponent<SwerveMechanic>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        float swerveAmount = Time.deltaTime * swerveSpeed * _swerveMechanic.LastMoveFactorX;
        
//        transform.Translate(x: swerveAmount, y:0, z:0);
//        if (transform.position.x > 4)
//            transform.position = new Vector3(4, transform.position.y, transform.position.z);
//        if (transform.position.x < -4)
//            transform.position = new Vector3(-4, transform.position.y, transform.position.z);
//    }
//    private void FixedUpdate()
//    {
        
//    }
//}
