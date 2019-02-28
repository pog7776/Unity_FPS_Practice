using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public float damage = 10f;  //amount of damage done to target
    public float range = 100f;  //range of hitscan weapon
    public float impactForce = 60f; //how hard a projectile will hit an object (push it back)
    public float fireRate = 0.1f; //time between each shot

    public int clipSize = 20;    //size of the weapons clips (amount of ammo)
    public int ammo;    //amount of ammo currently in clip
    public bool infiniteAmmo;   //give weapon infinite ammo
    public float reloadTime = 3f;   //time it takes to reload
    public Text ammoCounter;

    public Camera fpsCam;   //where hitscan weapon fires from
    public ParticleSystem muzzleFlash;  //particle system that creates muzzle flash
    public GameObject impactEffect; //particle system of where the "bullet" lands

    public Animator animator;

    public AudioSource fireSound;   //audio played when weapon fired
    public AudioSource reloadSound; //audio played when reloading

    private float fireTimer = 0f;    //initialise the weapon speed management
    private float reloadTimer = 0f;  //initialise the reload timer
    private bool reloading = false; //is the weapon reloading
    private bool sprinting = false; //check if character is sprinting
    private bool isActive = false;  //checking if weapon is active

    private void Start()
    {
        ammo = clipSize;
        string ammoString = ammo.ToString();
        ammoCounter.text = ammoString;
    }

    // Update is called once per frame
    void Update()
    {

        if (gameObject.activeSelf)  //checking to see if weapon is active, if not stop any ongoing reloading
        {
            isActive = true;
        }
        if (!isActive)
        {
            reloading = false;
            animator.Play("weaponIdle 0", 0, 0f);  //attempt to reset idle animation
        }

        fireTimer = fireTimer - Time.deltaTime; //fire timer
        reloadTimer = reloadTimer - Time.deltaTime; //reload timer

        string ammoString = ammo.ToString();
        ammoCounter.text = ammoString;

        //Debug------------------
        //Debug.Log(ammo);
        //Debug.Log(reloadTimer);
        //-----------------------




        if (Input.GetButton("Fire1") && fireTimer <= 0 && ammo > 0 && !reloading && !sprinting)
        {
            Shoot();
            animator.SetBool("TriggerPulled", true);
        }

        if (Input.GetButtonUp("Fire1"))
        {
            animator.SetBool("TriggerPulled", false);
        }

        if (Input.GetButton("Reload") && ammo != clipSize && !reloading)
        {
            animator.SetBool("Reloading", true);
            reloadSound.Play();
            reloadTimer = reloadTime;
            reloading = true;
            Reload();
            return;
        }
        else if (reloading)
        {
            Reload();
            return;
        }

        if (Input.GetButton("Sprint"))
        {
            animator.SetBool("Sprinting", true);
            sprinting = true;
            reloading = false;
        }
        else if(Input.GetButtonUp("Sprint"))
        {
            animator.SetBool("Sprinting", false);
            sprinting = false;
        }
    }

    void Shoot() {
        muzzleFlash.Play();
        fireSound.Play();
        fireTimer = fireRate;
        if (!infiniteAmmo)
        {
            ammo--;
        }

        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            //Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            if (target != null){
                target.TakeDamage(damage);
            }

            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }

    }

    void Reload()
    {
        if (reloading)
        {
            if (reloadTimer <= 0)
            {
                ammo = clipSize;
                reloading = false;
                animator.SetBool("Reloading", false);
            }
        }
        else
        {
            reloadTimer = reloadTime;
        }
    }
}