using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    //cria as variaveis
    public Transform player;
    public Transform bulletSpawn;
    public Slider healthBar;   
    public GameObject bulletPrefab;
    //variavel para o componente agent
    NavMeshAgent agent;
    public Vector3 destination; // The movement destination.
    public Vector3 target;      // The position to aim to.

    float health = 100.0f;
    float rotSpeed = 5.0f;

    float visibleRange = 80.0f;
    float shotRange = 40.0f;

    void Start()
    {
        //pega o componente e atribui a varivel
        agent = this.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = shotRange - 5; //for a little buffer
        InvokeRepeating("UpdateHealth",5,0.5f);
    }

    void Update()
    {
        //pega o centro da camera
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        //valor da barra igual a health
        healthBar.value = (int)health;
        //posiciona a barra de vida
        healthBar.transform.position = healthBarPos + new Vector3(0,60,0);
    }

    //faz o personagem recuperar vida se ficar menor que 100
    void UpdateHealth()
    {
       if(health < 100)
        health ++;
    }

    //se colidir e a tag for bullet
    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "bullet")
        {
            health -= 10;
        }
    }
    [Task]
    //randomiza o destino
    public void PickRandomDestination()
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        agent.SetDestination(dest);
        Task.current.Succeed(); 
    }
    [Task]
    //move para o destino
    public void MoveToDestination()
    { if (Task.isInspected) Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }
}

