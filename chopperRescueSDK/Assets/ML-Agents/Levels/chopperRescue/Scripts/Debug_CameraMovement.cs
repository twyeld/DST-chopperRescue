 using UnityEngine;
 
 public class Debug_CameraMovement : MonoBehaviour
 {
     public int Speed = 50;
     void Update()
     {
         float xAxisValue = Input.GetAxis("Horizontal") * Speed;
         float zAxisValue = Input.GetAxis("Vertical") * Speed;
         float yValue = 0.0f;
	float rotation = Input.GetAxis("Vertical") * Speed;

 
         if (Input.GetKey(KeyCode.Q))
         {
             yValue = -Speed;
         }
         if (Input.GetKey(KeyCode.E))
         {
             yValue = Speed;
         }
 
         transform.position = new Vector3(transform.position.x + xAxisValue, transform.position.y + yValue, transform.position.z + zAxisValue);
         transform.Rotate(0, rotation, 0);
     }
 }
