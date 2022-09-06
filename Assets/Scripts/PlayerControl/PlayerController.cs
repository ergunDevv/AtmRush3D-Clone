using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private Transform playerVisual;
    [SerializeField] private Transform miniGamePos;
    [SerializeField] private Animator player_Animator;
    [SerializeField] private GameObject player_Object;


    // states
    private State stateCurrent, stateRun, stateIdle;

    [Header("Movers")]
    private float lastFrameFingerPositionX;
    private float moveFactorX;
    private float swerveSpeed = 1f;
    private float speed = 5f;


    //  [Header("Scriptss")]
    public float LastMoveFactorX => moveFactorX;





    void Start()
    {
        StackManager.Instance.StartQuantityWoods(0);
        //StackManager.Instance.StartQualityWoods(PlayerPrefs.GetInt("qualityStartItem"));

        GameManager.ActionGameStart += StartToMove;
        GameManager.ActionMiniGame += PerformMiniGame;

        stateRun = new State(Move, () => { }, () => { });
        stateIdle = new State(() => { }, () => { }, () => { });
        SetState(stateIdle);
    }

    void Update()
    {
        GetInput();
        stateCurrent.onUpdate();
    }

    private void SetState(State newState)
    {
        if (stateCurrent != null)
            stateCurrent.onStateExit();

        stateCurrent = newState;
        stateCurrent.onStateEnter();
    }

    private void Move()
    {
        // forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        // horizontal
        float swerveAmount = Time.deltaTime * swerveSpeed * LastMoveFactorX;

        transform.Translate(x: swerveAmount, y: 0, z: 0);
        if (transform.position.x > 4)
            transform.position = new Vector3(4, transform.position.y, transform.position.z);
        if (transform.position.x < -4)
            transform.position = new Vector3(-4, transform.position.y, transform.position.z);
    }

    private void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastFrameFingerPositionX = Input.mousePosition.x;


        }
        if (Input.GetMouseButton(0))
        {
            moveFactorX = Input.mousePosition.x - lastFrameFingerPositionX;
            lastFrameFingerPositionX = Input.mousePosition.x;


        }
        if (Input.GetMouseButtonUp(0))
        {
            moveFactorX = 0f;


        }
    }

    private IEnumerator PushBack()
    {
        SetState(stateIdle);

        rigidbody.isKinematic = false;

        rigidbody.AddForce(transform.forward * -10, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);

        rigidbody.isKinematic = true;
        SetState(stateRun);
    }

    private void StartToMove()
    {
        SetState(stateRun);
    }

    private void Stop()
    {
        SetState(stateIdle);
    }

    private void PerformMiniGame()
    {
        StartCoroutine(PerformMiniGameRoutine());
    }
    private IEnumerator PerformMiniGameRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        // set root transform's position
        var newPlayerPos = transform.position;
        newPlayerPos.z = miniGamePos.position.z;
        transform.position = newPlayerPos;
        // set visual's position
        playerVisual.position = transform.position;
        Debug.Log("1");
        player_Animator.Play("ChickenDance");
        player_Object.transform.Rotate(0f,180f, 0f);
        Debug.Log("2");
        var currentPoolObjectPos = miniGamePos.position;

        for (int i = 0; i < StackManager.Instance.CurrentStackValue; i++)
        {
            var poolObject = MiniGameItemPool.Instance.GetObject();
            poolObject.gameObject.SetActive(true);
            // set player's position
            newPlayerPos.y += MiniGameItemPool.Instance.GetObjectHeight();

            newPlayerPos.x = 0.146f;
            transform.position = newPlayerPos;
            // set pool object's position
            var newPoolObjectPos = currentPoolObjectPos;
            newPoolObjectPos.y += MiniGameItemPool.Instance.GetObjectHeight();
            poolObject.position = newPoolObjectPos;
            currentPoolObjectPos = newPoolObjectPos;

            yield return new WaitForSeconds(0.025f);
        }

        // end level action
        GameManager.ActionGameEnd?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Obstacle"))
        {
            StartCoroutine(PushBack());
        }

        if(other.CompareTag("FinishLine"))
        {
            Stop();
            GameManager.ActionMiniGame?.Invoke();
        }

        if(other.CompareTag("Multiplier"))
        {
            var newMultiplierPos = other.transform.position;
            newMultiplierPos.z -= 1;    
            other.transform.position = newMultiplierPos;

            MoneyManager.Instance.CurrentMultiplier = int.Parse(other.name);
        }
    }

    private void OnDestroy()
    {
        GameManager.ActionGameStart -= StartToMove;
        GameManager.ActionMiniGame -= PerformMiniGame;
    }
}
