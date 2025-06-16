using System;
using Unity.VisualScripting;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject StartPoint;
    [SerializeField] private GameObject Finish;
    [SerializeField] private GameObject Player;

    private Vector3 startPosition;
    private PlatformerMovement playerMovement;
    private bool exitActive = false;

    protected virtual void Awake()
    {
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        Instance = this;
        startPosition = StartPoint.transform.position;

        // Cache the PlatformerMovement component from the Player GameObject
        playerMovement = Player.GetComponent<PlatformerMovement>();
    }

    public void DeathZoneTrigger()
    {
        // Replace deprecated FindObjectOfType with FindFirstObjectByType
        var pm = GameObject.FindFirstObjectByType<PlatformerMovement>();
        if (pm != null)
            pm.Respawn(startPosition);
    }

    public void ExitTrigger()
    {
        if (exitActive)
        {
            
        }
    }

    private void Start()
    {
        Player.transform.position = StartPoint.transform.position;
        Player.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
