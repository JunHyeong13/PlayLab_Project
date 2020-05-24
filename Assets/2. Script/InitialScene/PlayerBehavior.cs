﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public Transform tr = null;

    private LineRenderer line;
    private int effectTurn = 0;


    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        line.startWidth = 0.1f;
        line.endWidth = 0.05f;

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(tr.position, tr.forward);
        RaycastHit hit;

        line.SetPosition(0, tr.position);
        if (Physics.Raycast(tr.position, tr.forward, out hit, 100f))
        {
            line.SetPosition(1, hit.point);
            if (hit.collider.gameObject.CompareTag("INTERACTABLE"))
            {
                hit.collider.gameObject.GetComponent<Outline>().SetActive(true);
            }
        }
        else line.SetPosition(1, ray.GetPoint(100.0f));

        StartCoroutine(this.ShowLaserBeam());
    }

    IEnumerator ShowLaserBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
        line.enabled = false;
    }
}