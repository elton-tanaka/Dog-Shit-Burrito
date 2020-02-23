﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public Rigidbody2D theRB;
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Vector2 mouseInput;
    public float mouseSensitivity = 1f;
    public Camera viewCam;
    public GameObject bulletImpact;
    public int currentAmmo;
    public Animator gunAnim;
    public int currentHealth;
    public int maxHealth = 100;
    public GameObject deadScreen;
    private bool hasDied;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;;   
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasDied) {

            //player movement
            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            Vector3 moveHorizontal = transform.up * -moveInput.x;
            Vector3 moveVertical = transform.right * moveInput.y;

            theRB.velocity = (moveHorizontal + moveVertical) * moveSpeed;

            //player view control
            mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - mouseInput.x);
            viewCam.transform.localRotation = Quaternion.Euler(viewCam.transform.localRotation.eulerAngles + new Vector3(0f, mouseInput.y, 0f));

            //shooting
            if(Input.GetMouseButtonDown(0)) {
                if(currentAmmo > 0) {
                    Ray ray = viewCam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit)) {
                        Debug.Log(hit.transform.name);
                        Instantiate(bulletImpact, hit.point, transform.rotation);

                        if(hit.transform.tag == "Enemy") {
                            hit.transform.parent.GetComponent<EnemyController>().TakeDamage();
                        }
                    } else {
                        Debug.Log("Nada");
                    }
                }
                currentAmmo--;
                gunAnim.SetTrigger("Shoot");
            }
        }
    }

    public void TakeDamage(int damageAmmount) 
    {
        currentHealth -= damageAmmount;
        
        if(currentHealth <= 0) {
            deadScreen.SetActive(true);
            hasDied = true;
        }
    }

    public void AddHealth(int healAmmount)
    {
        currentHealth += healAmmount;
        if(currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
    }
}   