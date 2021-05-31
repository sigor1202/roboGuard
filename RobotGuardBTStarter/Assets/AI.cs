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
    //cria variaveis
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
        //se a vida for menor que 100
       if(health < 100)
            //adiciona 1 a vida
        health ++;
    }

    //se colidir e a tag for bullet
    void OnCollisionEnter(Collision col)
    {   //se colidir com um gameObject com a tag bullet
        if(col.gameObject.tag == "bullet")
        {
            //subtrai 10 da vida
            health -= 10;
        }
    }
    [Task]
    //randomiza o destino
    public void PickRandomDestination()
    {
        //randomiza os valores de x e zentre -100 e 100
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        //seta o destino com os valores de dest
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

  
    [Task]
    public void PickDestination(int x, int z)
    {
        //coloca o valor de x e z no vetor
        Vector3 dest = new Vector3(x, 0, z);
        //seta o destino com os valores do velor dest
        agent.SetDestination(dest);

        Task.current.Succeed();
    }
   
    [Task]
   
    bool SeePlayer()
    {
       //calcula a distancia entre o robo e o player
        Vector3 distance = player.transform.position - this.transform.position;
        //cria o RayCast hit
        RaycastHit hit;
        //cria uma variavel do tipo bool e seta como falsa 
        bool seeWall = false;
        //desnha uma linha vermelha
        Debug.DrawRay(this.transform.position, distance, Color.red);
       //se o ray cast detectar algo
        if (Physics.Raycast(this.transform.position, distance, out hit))
        {
            //se a tag do objeto etectado for igual a wall
            if (hit.collider.gameObject.tag == "wall")
            {
                //seta a variavel como true
                seeWall = true;
            }
        }
        //retorna o valor de see wallcomo debug
        if (Task.isInspected)
        {
            Task.current.debugInfo = string.Format("wall={0}", seeWall);
        }
        //se a distancia for menor que o raoi de visão e seeWall for falso
        if (distance.magnitude < visibleRange && !seeWall)
        {
            //retorna true
            return true;
        }
        //se não retorna falso
        else
        {
            return false;
        }
    }
    [Task]
    public void TargetPlayer()
    {
        //pega o transform do player como target
        target = player.transform.position;
        //task completa
        Task.current.Succeed();
    }

    [Task]
    
    public void LookAtTarget()
    {
        //calcula a direção
        Vector3 direction = target - this.transform.position;
        //rotaciona
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
        //se estiver rodando
        if (Task.isInspected)
        {
            //mostra o angulo como debug e olha para ele
            Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction));
        }
        //se a direferença do angulo for menor que 5 completa a tesk
        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public bool Fire()
    {
        //instamcia o prefab da bala e guarda na variavel bullet
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        //adiciona força na bala
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);
        //retorna true
        return true;
    }
 

  
    [Task]
    //faz ele virar no angulo escolhido
    bool Turn(float angle)
    {
        
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
        target = p;
        return true;
    }

    [Task]
    public bool IsHealthLessThan(float health)
    {
        //compara a vida com o valor escolhido no botAi e retorna esse valor 
        return this.health < health;
    }

    
    [Task]
    public bool Explode()
    {
        //destroi a barra de vida
        Destroy(healthBar.gameObject);
        //destroi a si mesmo
        Destroy(this.gameObject);
        //retorna true
        return true;
    }
}

