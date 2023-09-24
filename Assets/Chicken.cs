using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chicken : Creature
{
    
    public AnimalState animalstate;
    [SerializeField] private float energyHungryThreshold;
    [SerializeField] private float energyMateThreshold;
    [SerializeField]public float speed;
    [SerializeField]public float visionRadius;
    [SerializeField]public float visionDistance;
    [SerializeField]private float runAwayDistance;
    [SerializeField]private float allyPowerThreshold;
    [SerializeField]private int allyCountThreshold;
    [SerializeField]private int enemiesCountThreshold;
    [SerializeField] private int reproductionCount;
    [SerializeField] private float PatrolMinTime;
    [SerializeField] private float PatrolMaxTime;
    [SerializeField] private float PersuitMinTime;
    [SerializeField] private float PersuitMaxTime;
    [SerializeField] private float IdleMinTime;
    [SerializeField] private float IdleMaxTime;
    [SerializeField] private float restMinTime;
    [SerializeField] private float restMaxTime;
    [SerializeField] private float rotationSpeed = 2.0f;
    [SerializeField] private float energyDepletion;
    [SerializeField] private LayerMask animalLayer;
    [SerializeField] private GameObject NestPrefab;
    [SerializeField] private float pregnancyPeriod;
    
    private bool isPregnant = false;
    private float PatrolTimer = 0f;
    private float PersuitTimer = 0f;
    private float IdleTimer = 0f;
    private float restTimer = 0f;
    private bool isUnderAttack = false;
    private bool stopMoving = false;
    private GameObject attackTarget;

    public bool isMoving = false;
    private AnimatorState triggerAnimation;
    private Quaternion targetRotation; // Add this variable to store the target rotation


    // Adjust the rotation speed as needed


    protected void Start()
    {
        PatrolTimer = Random.Range(PatrolMinTime, PatrolMaxTime);
        IdleTimer = Random.Range(IdleMinTime, IdleMaxTime);
        PersuitTimer = Random.Range(PersuitMinTime, PersuitMaxTime);
        restTimer = Random.Range(restMinTime, restMaxTime);
        triggerAnimation = GetComponent<AnimatorState>();
    }

    protected override void Update()
    {
        checkConditions();
        updateActions();
    }



    private void checkConditions()
    {
        animalstate = energy >= energyMateThreshold ? AnimalState.Mate : energy <= energyHungryThreshold ? AnimalState.Hungry : AnimalState.Neutral;
    }
    

    private void updateActions()
    {
        if (energy <= 0)
        {
            Destroy(gameObject);
        }

        if (isUnderAttack)
        {
            if (!checkEnemies())
            {
                while (attackTarget != null)
                {
                    persuit(attackTarget);
                }
            }
        }
        
        if (!checkPredators())
        {
            switch (animalstate)
            {
                case AnimalState.Neutral:
                    patrol(false, false);
                    break;
                case AnimalState.Hungry:
                    hungry();
                    break;
                case AnimalState.Mate:
                    if (!isPregnant)
                    {
                        mate();
                    }
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        RaycastHit[] hits = findTarget();
        if (hits != null)
        {
            foreach (RaycastHit hit in hits)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(hit.point, 0.5f);
                Gizmos.DrawSphere(hit.point, 0.5f);
            }
        }
    }
    private RaycastHit[] findTarget()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, visionRadius, transform.forward, visionDistance, animalLayer);
        
        return hits;
    }
    private GameObject FindBestTarget(RaycastHit[] targets, Func<RaycastHit, bool> condition)
    {
        GameObject myTarget = null;
        foreach (RaycastHit target in targets)
        {
            if (condition(target))
            {
                if (myTarget == null)
                {
                    myTarget = target.collider.gameObject;
                }
                else if (myTarget.GetComponent<Creature>().attackPower > target.collider.gameObject.GetComponent<Creature>().attackPower)
                { 
                    myTarget = target.collider.gameObject;
                }
            }
        }
        return myTarget;
    }

    public bool isPrey(GameObject target)
    {
        if (target != null)
        {
            Creature creature;
            if (target.TryGetComponent(out creature))
            {
                return creature.attackPower < this.attackPower / 2;
            }
        }
        return false;
        
    }

    public bool isMate(GameObject target)
    {
        if (target != null)
        {
            Creature creature;
            if (target.TryGetComponent(out creature)){
                return creature.attackPower > this.attackPower / 2;
            }
        }
        return false;
    }

    public bool isPredator(GameObject target)
    {
        if (target != null)
        {
            Creature creature;
            if (target.TryGetComponent(out creature))
            {
                return creature.attackPower > this.attackPower * 2;
            }
        }
        return false;
    }
    
    private void hungry()
    {
        RaycastHit[] targets = findTarget();
        if (targets != null)
        {
            GameObject myTarget = FindBestTarget(targets, target => isPrey(target.transform.gameObject));
            if (myTarget != null)
            {
                print("persuit : " + myTarget);
                persuit(myTarget);
            }
            else
            {
                patrol(true, true);
            }
        }
    }

    private void mate()
    {
        RaycastHit[] targets = findTarget();
        if (targets != null)
        {
            GameObject myTarget = FindBestTarget(targets, target => isMate(target.transform.gameObject));
            if (myTarget != null)
            {
                persuit(myTarget);
            }
            else
            {
                patrol(true, true);
            }
        }
    }
    private bool checkPredators()
    {
        RaycastHit[] targets = findTarget();
        if (targets == null || targets.Length == 0)
        {
            return false;
        }
        GameObject myTarget = FindBestTarget(targets, target => isPredator(target.transform.gameObject));
        if (myTarget == null)
        {
            return false;
        }
        if (!checkAllies() || energy < energyHungryThreshold / 2)
        {
            animalstate = AnimalState.Run;
            runAway(myTarget);
        }
        else
        {
            persuit(myTarget);
            animalstate = AnimalState.Attack;
        }
        return true;
    }

    private bool checkAllies()
    {
        RaycastHit [] targets = findTarget();
        int allyCount = 0;
        foreach (RaycastHit target in targets)
        {
            Creature creature;
            target.collider.gameObject.TryGetComponent(out creature);
            float targetPower = creature.attackPower;
            if (targetPower < attackPower + allyPowerThreshold && targetPower > attackPower - allyPowerThreshold)
            {
                allyCount++;
            }
        }
        if (allyCount >= allyCountThreshold)
        {
            return true;
        }
        return false;
    }
    
    private bool checkEnemies()
    {
        RaycastHit [] targets = findTarget();
        int enemiesCount = 0;
        foreach (RaycastHit target in targets)
        {
            Creature creature;
            target.collider.gameObject.TryGetComponent(out creature);
            float targetPower = creature.attackPower;
            if (targetPower < attackPower/2)
            {
                enemiesCount++;
            }
        }
        if (enemiesCount >= enemiesCountThreshold)
        {
            return true;
        }
        return false;
    }

    private void runAway(GameObject target)
    {
        if (!stopMoving)
        {
            isMoving = true;
            Vector3 direction = transform.position - target.transform.position;
            direction.Normalize();
            Vector3 targetPosition = transform.position + direction * runAwayDistance;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
            transform.LookAt(transform.position + direction);
            energy -= Time.deltaTime * energyDepletion * speed;
        }
    }
    private void patrol(bool hasTarget, bool findSmaller)
    { 
        Vector3 move = Vector3.zero;
        if (PatrolTimer <= 0)
        {
            // Generate a random Y rotation
            float randomYRotation = Random.Range(-90f, 90f);
            targetRotation = Quaternion.Euler(0, randomYRotation, 0);

            IdleTimer -= Time.deltaTime * speed;
            isMoving = false;
            if (IdleTimer <= 0)
            {
                PatrolTimer = Random.Range(PatrolMinTime, PatrolMaxTime);
                IdleTimer = Random.Range(IdleMinTime, IdleMaxTime);
            }

            print("patrolDirectionPick");
        }
        else
        {
            if (!stopMoving)
            {
                isMoving = true;
                PatrolTimer -= Time.deltaTime;
                energy -= Time.deltaTime * energyDepletion * speed;

                // Smoothly rotate towards the targetRotation
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Move forward
                transform.position += transform.forward * Time.deltaTime * speed;
                print("patrolling");
            }
        }
    }
    
    private void persuit(GameObject target)
    {
        if (PersuitTimer <= 0)
        {
            isMoving = false;
            restTimer -= Time.deltaTime * speed;
            if (restTimer <= 0)
            {
                PersuitTimer = Random.Range(PersuitMinTime, PersuitMaxTime);
                restTimer = Random.Range(restMinTime, restMaxTime);
            }
        }

        if (!stopMoving)
        {
            isMoving = true;
            energy -= Time.deltaTime * energyDepletion * speed;
            var position = target.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);
            transform.LookAt(position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (animalstate)
        {
            case AnimalState.Neutral:
                break;
            case AnimalState.Hungry:
                if(triggerAnimation!=null)
                    triggerAnimation.PlayEating();
                eating(other.gameObject);
                print("playAnimation");
                StartCoroutine(IE_stopMoving(3));

                break;
            case AnimalState.Mate:
                if(triggerAnimation!=null)
                    triggerAnimation.PlayMating();
                mating(other.gameObject);
                StartCoroutine(IE_stopMoving(3));

                break;
            case AnimalState.Attack:
                if(triggerAnimation!=null)
                    triggerAnimation.PlayBiting();
                attacking(other.gameObject);
                StartCoroutine(IE_stopMoving(3));

                break;
        }   
    }

    private void fighting(GameObject collidingObj)
    {
        if (isPrey(collidingObj))
        {
            isUnderAttack = true;
            attackTarget = collidingObj;
        }
    }


    private void mating(GameObject collidingObj)
    {
        if (isMate(collidingObj))
        {
            Chicken chicken;
            collidingObj.TryGetComponent(out chicken);
            if (chicken != null)
            {
                if (!chicken.isPregnant)
                {
                    isPregnant = true;
                    StartCoroutine(IE_pregnancy(collidingObj));
                }
                energy = energyHungryThreshold;
                print("Mate");
            }
        }
    }

    IEnumerator IE_pregnancy(GameObject collidingObj)
    {
        yield return new WaitForSeconds(pregnancyPeriod);
        GameObject nest =Instantiate(NestPrefab);
        nest.GetComponent<NestController>().parents[0] = gameObject;
        nest.GetComponent<NestController>().parents[1] = collidingObj;
        isPregnant = false;
    }

    private void eating(GameObject collidingObj)
    {
        if (isPrey(collidingObj))
        {
            collidingObj.GetComponent<Creature>().energy -= attackPower;
            energy += attackPower;
            print("Attacked");
        }
    }

    private void attacking(GameObject collidingObj)
    {
        if (isPredator(collidingObj))
        {
            collidingObj.GetComponent<Creature>().getHurt(attackPower);
        }
    }

    IEnumerator IE_stopMoving(float seconds)
    {
        stopMoving = true;
        yield return new WaitForSeconds(seconds);
        stopMoving = false;
    }
    
    public override void getHurt(float damage)
    {
        StartCoroutine(IE_stopMoving(3));
        energy -= damage;
    }
   
}