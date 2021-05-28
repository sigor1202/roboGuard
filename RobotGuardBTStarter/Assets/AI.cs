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

    //cria o metodo para ir para o destino para fazer a patrulha
    [Task]
    public void PickDestination(int x, int z)
    {
        //Cria a variavel para pegar as posições dos eixos x e z
        Vector3 dest = new Vector3(x, 0, z);
        //Seta o destino do NPC para a variavel dest
        agent.SetDestination(dest);
        //fala que a ação foi concluida com sucesso
        Task.current.Succeed();
    }
    //*Fim Patroll.BT

    //*Attack.BT
    [Task]
    //metodo boolenao para olhar para o player
    bool SeePlayer()
    {
        //fala que a distancia dele vai ser a distancia dele menos a do player
        Vector3 distance = player.transform.position - this.transform.position;
        //cria o raycast
        RaycastHit hit;
        //booleano para ver a parede
        bool seeWall = false;
        //debug para desenhar o raycast
        Debug.DrawRay(this.transform.position, distance, Color.red);
        //se o raycast tocar
        if (Physics.Raycast(this.transform.position, distance, out hit))
        {
            //se o raycast colidir com um objeto com a tag wall seeWall fica true
            if (hit.collider.gameObject.tag == "wall")
            {
                //fica true
                seeWall = true;
            }
        }
        //se estiver rodando a aplicação
        if (Task.isInspected)
        {
            //mostra a distancia da wall
            Task.current.debugInfo = string.Format("wall={0}", seeWall);
        }
        //se a distancia dor menor que a distancia de visibilidade e caso seewall seja false retorne como true
        if (distance.magnitude < visibleRange && !seeWall)
        {
            //retorna como true
            return true;
        }
        //caso não retorne como falso
        else
        {
            //retorna como false
            return false;
        }
    }
    [Task]
    //metodo para ver o player
    public void TargetPlayer()
    {
        //pega a posição do player
        target = player.transform.position;
        //fala que a ação foi concluida com sucesso
        Task.current.Succeed();
    }

    [Task]
    //olha para o target
    public void LookAtTarget()
    {
        //cria a variavel para detectar a direção sendo menos que a posição do player
        Vector3 direction = target - this.transform.position;
        //faz a rotação ser mais suave
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
        //se estiver rodando a aplicação
        if (Task.isInspected)
        {
            //mostra o angle com debug
            Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction));
        }
        //se for menor que 5 complete a task
        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            //fala que a ação foi concluida com sucesso
            Task.current.Succeed();
        }
    }
    //atualização
    [Task]
    
    public bool Fire()
    {
        //estancia o bulletPrefab e o gruarda na variavel bullet
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        //adiciona força na variavel bullet
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);

        return true;
    }
 

  
    [Task]

    bool Turn(float angle)
    {
        //faz o robo rotacionar
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
       //alco igual a variavel utilizada na rotação
        target = p;
        return true;
    }
}

