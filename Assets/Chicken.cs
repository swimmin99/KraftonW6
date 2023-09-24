using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chicken : Creature
{
    public AnimalState animalstate;
    public float maxAttackPower;

    [Header("Energy Threshold")] [SerializeField]
    private float energyHungryThreshold;

    [SerializeField] private float energyMateThreshold;

    [Header("Move Related Properties")] [SerializeField]
    public float speed;

    [SerializeField] private float runAwayDistance;
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

    [Header("Detection Related Properties")] [SerializeField]
    public float visionRadius;

    [SerializeField] public float visionDistance;
    [SerializeField] private LayerMask animalLayer;

    [Header("Ally Related Properties")] [SerializeField]
    private float allyPowerThreshold;

    [SerializeField] private int allyCountThreshold;
    [SerializeField] private int enemiesCountThreshold;

    [Header("Pregnancy Related Properties")] [SerializeField]
    private GameObject NestPrefab;

    [SerializeField] private float pregnancyPeriod;
    private bool isAdult = false;
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
    private Quaternion targetRotation;
    private int attackCount = 0;

    public string getStateInfo()
    {
        return
            "Name   " + gameObject.name +
            "\nState    " + animalstate.ToString() +
            "\nEnergy   " + energy.ToString() + "%" +
            "\nGrowth   " + ((int)(attackPower / maxAttackPower * 100)).ToString() + "%" +
            "\nisPregnant   " + isPregnant;
    }

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
        float mySize = 0.5f + attackPower / 100f;
        transform.localScale = new Vector3(mySize,mySize,mySize);
        checkConditions();
        updateActions();
    }


    private void checkConditions()
    {
        animalstate = energy >= energyMateThreshold ? AnimalState.Mate :
            energy <= energyHungryThreshold ? AnimalState.Hungry : AnimalState.Neutral;
    }


    private void updateActions()
    {
        if (!checkDanger())
        {
            checkStates();

        }
    }
    protected bool checkDanger()
    {
        //CheckEnergy
        if (energy <= 0)
        {
            Destroy(gameObject);
        }
        //CheckImmediateDanger
        if (isUnderAttack)
        {
            return false;
        }
        if (!checkEnemies())
        {
            while (attackTarget != null)
            {
                persuit(attackTarget);
            }
        }
        return checkPredators();
    }
    protected void checkStates()
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
                if (!isPregnant && attackPower >= maxAttackPower)
                {
                    findMate();
                }
                break;
        }
    }
    private RaycastHit[] hitRaycasts()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, visionRadius, transform.forward, visionDistance,
            animalLayer);

        return hits;
    }
    private GameObject FindBestTarget(RaycastHit[] targets, Func<RaycastHit, bool> condition)
    {
        GameObject myTarget = null;
        float closestDistance = float.MaxValue; // Initialize to a large value.

        foreach (RaycastHit target in targets)
        {
            if (condition(target) && target.collider.gameObject != gameObject)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.collider.gameObject.transform.position);

                if (myTarget == null || distanceToTarget < closestDistance)
                {
                    myTarget = target.collider.gameObject;
                    closestDistance = distanceToTarget;
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
            if (target.TryGetComponent(out creature))
            {
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
        RaycastHit[] targets = hitRaycasts();
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

    private void findMate()
    {
        RaycastHit[] targets = hitRaycasts();
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
        RaycastHit[] targets = hitRaycasts();
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
        RaycastHit[] targets = hitRaycasts();
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
        RaycastHit[] targets = hitRaycasts();
        int enemiesCount = 0;
        foreach (RaycastHit target in targets)
        {
            Creature creature;
            target.collider.gameObject.TryGetComponent(out creature);
            float targetPower = creature.attackPower;
            if (targetPower < attackPower / 2)
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
            direction.y = 0f; 
            direction.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation.x = 0f;
            targetRotation.z = 0f; 

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            Vector3 targetPosition = transform.position + direction * runAwayDistance;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
            energy -= Time.deltaTime * energyDepletion * speed;
        }
    }

    private void patrol(bool hasTarget, bool findSmaller)
    {
        Vector3 move = Vector3.zero;
        if (PatrolTimer <= 0)
        {
            float randomYRotation = Random.Range(-90f, 90f);
            targetRotation = Quaternion.Euler(0, randomYRotation, 0);

            IdleTimer -= Time.deltaTime * speed;
            isMoving = false;
            if (IdleTimer <= 0)
            {
                PatrolTimer = Random.Range(PatrolMinTime, PatrolMaxTime);
                IdleTimer = Random.Range(IdleMinTime, IdleMaxTime);
            }
        }
        else
        {
            if (!stopMoving)
            {
                isMoving = true;
                PatrolTimer -= Time.deltaTime;
                energy -= Time.deltaTime * energyDepletion * speed;

                Quaternion currentRotation = transform.rotation;
                targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.position += transform.forward * Time.deltaTime * speed;
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
                fighting(other.gameObject);
                break;
            case AnimalState.Hungry:
                if (triggerAnimation != null)
                    triggerAnimation.PlayEating();
                eating(other.gameObject);
                print("playAnimation");
                StartCoroutine(IE_stopMoving(3));

                break;
            case AnimalState.Mate:
                if (triggerAnimation != null)
                    triggerAnimation.PlayMating();
                mating(other.gameObject);
                StartCoroutine(IE_stopMoving(3));

                break;
            case AnimalState.Attack:
                if (triggerAnimation != null)
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
            if (chicken == null)
            {
                return;
            }
            if (!chicken.isPregnant)
            {
                isPregnant = true;
                StartCoroutine(IE_pregnancy(collidingObj));
            }
            energy = energyHungryThreshold;
            print("Mate");
        }
    }

    IEnumerator IE_pregnancy(GameObject collidingObj)
    {
        yield return new WaitForSeconds(pregnancyPeriod);
        GameObject nest = Instantiate(NestPrefab);
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
            if (attackPower < maxAttackPower) {
                attackPower += 1;
            }
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
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        RaycastHit[] hits = hitRaycasts();
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
}