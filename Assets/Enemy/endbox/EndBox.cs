using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EndBox : MonoBehaviour
{
    [SerializeField] private GameObject deathTimeLine;
    [SerializeField] private GameObject youDied;
    [SerializeField] private GameObject slickBar;

    private void Start()
    {
        deathTimeLine.SetActive(false);
        youDied.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 0;
            deathTimeLine.SetActive(true);
            slickBar.SetActive(false);
            
        }
    }
}
