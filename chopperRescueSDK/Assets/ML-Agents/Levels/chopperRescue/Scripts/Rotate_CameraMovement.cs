 using UnityEngine;
 
 public class Rotate_CameraMovement : MonoBehaviour
 {
     public int Speed = 50;
     void Update()
     {

	float rotation = Input.GetAxis("Horizontal") * Speed;
	float yValue = 0.0f;

 
         if (Input.GetKey(KeyCode.Q))
         {
             yValue = -Speed;
         }
         if (Input.GetKey(KeyCode.E))
         {
             yValue = Speed;
         }

         transform.Rotate(0, rotation, 0);
     }
 }
