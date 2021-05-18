using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {
    //velocidade de movimento
	float speed = 20.0F;
    //velocidade de rotação
    float rotationSpeed = 120.0F;
    //variavel para o prefabe da bala
    public GameObject bulletPrefab;
    //local de spawn
    public Transform bulletSpawn;

    void Update() {
        //movimenta para frente e para tras
        float translation = Input.GetAxis("Vertical") * speed;
        //rotaciona 
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        //deixa a velocidade de rotação e movimentação constante independente da quantidade de fps
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
        //se apertar espaço
        if(Input.GetKeyDown("space"))
        {   //spawna a bala e adiciona fprça para ir na direção
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }
    }
}
