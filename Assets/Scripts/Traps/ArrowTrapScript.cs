﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrapScript : MonoBehaviour
{
    ObjectPooler objPool;
    public bool facingRight;
    public bool facingSouth;
    public bool facingNorth;
    public bool scriptEnabled;
    public AudioSource FIRE;

    // Start is called before the first frame update
    void Start()
    {
        objPool = ObjectPooler.SharedInstance;
      
        GetComponent<Animator>().enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        ///
        /// NOTE: shoot control migrated to animation-binding
        /// 
        ////wait x seconds
        //if (delayTime >= 100)
        //{
        //    delayTime = 0;
        //    Shoot();
        //} else
        //{
        //    delayTime++;
        //}
    }


    void Shoot()
    {
        if (scriptEnabled)
        {
            GameObject arrow = ObjectPooler.SharedInstance.GetPooledObject();
            if (arrow != null)
            {
                FIRE.PlayOneShot(FIRE.clip);
                arrow.GetComponent<ArrowScript>().facingRight = facingRight;
                arrow.GetComponent<ArrowScript>().facingNorth = facingNorth;
                arrow.GetComponent<ArrowScript>().facingSouth = facingSouth;
                if (facingRight)
                    arrow.transform.position = gameObject.transform.position + new Vector3(1, 0, 0);
                else if (facingNorth)
                    arrow.transform.position = gameObject.transform.position + new Vector3(0, 1, 0);
                else if (facingSouth)
                    arrow.transform.position = gameObject.transform.position + new Vector3(0, -1, 0);
                else
                    arrow.transform.position = gameObject.transform.position + new Vector3(-1, 0, 0);
                arrow.SetActive(true);
            }
        }
    }


    ////ENABLING/DISABLING/////
    /// <summary>
    /// On become invisible, stop shooting
    /// </summary>
    private void OnBecameInvisible()
    {
        if(scriptEnabled)
            GetComponent<Animator>().enabled = false;
    }

    /// <summary>
    /// On become visible, enable shooting
    /// </summary>
    private void OnBecameVisible()
    {
        if (scriptEnabled)
            GetComponent<Animator>().enabled = true;
    }

    public void SetDisabled()
    {
        scriptEnabled = false;
    }
    public void SetEnabled()
    {
        scriptEnabled = true;
    }

}
