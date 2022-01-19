using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
    public float movement_speed;
    public int collectible;
    public Text collectible_text;
    #region Unity Methods
    private void Update()
    {
        float x_movement = Input.GetAxisRaw("Horizontal");
        float z_movement = Input.GetAxisRaw("Vertical");
        transform.Translate(new Vector3(x_movement * movement_speed * Time.deltaTime, 0f, z_movement * movement_speed * Time.deltaTime) );
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag=="Collectible")
        {
            Destroy(collision.gameObject);
            collectible++;
            collectible_text.text = "Collectible Collected:" + collectible;
        }
    }
    #endregion
}
