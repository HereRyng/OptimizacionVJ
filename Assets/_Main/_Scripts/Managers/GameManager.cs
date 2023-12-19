﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Updateable
{
    [SerializeField] private GameObject _popUp;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private int _maxGoals;
    [SerializeField] float _waitTimeToSpawn = 5f;
    [SerializeField] private int _bulletsInPool = 10;
    [SerializeField] private int _enemiesInPool = 100;

    [SerializeField] private Transform _posSpawnEnemies; 
    [SerializeField] private Transform _parentBullets;

    [Header("Prefabs")]
    [SerializeField] private Updateable _bullet;
    [SerializeField] private Updateable _enemy;

    private float _currTimeSpawn;

    public bool CanRunSpawn { get; private set; }
    public static ObjectPool<Updateable> BulletPool { get ; private set ; }
    public static ObjectPool<Updateable> EnemyPool { get ; private set ; }
    public static GameManager Instance { get; private set; }
    public int EntityDefeat { get ; private set ; }

    public override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            InitializationPool();
            Instance = this;
            CanRunSpawn = true;
            _currTimeSpawn = _waitTimeToSpawn;

        }
    }
    public override void CustomUpdate()
    {
        TimerToSpawn();
    }
    public void InitializationPool()
    {
        FactoryObject<Updateable> bulletFactory = CreateFactory(_parentBullets, _bullet);
        FactoryObject<Updateable> enemyFactory = CreateFactory(_posSpawnEnemies, _enemy);
        BulletPool = CreatePool(bulletFactory, _bulletsInPool);
        EnemyPool = CreatePool(enemyFactory, _enemiesInPool);
    }

    public void TimerToSpawn()
    {      
        if (CanRunSpawn)
        {
            _currTimeSpawn += Time.deltaTime;
            if (_currTimeSpawn >= _waitTimeToSpawn)
            {
               
                _currTimeSpawn = 0;
                var enemy = EnemyPool.GetPooledObject();
                enemy.transform.position = _posSpawnEnemies.position;
            }
        }
    }
    public void EntityCounter()
    {
        EntityDefeat++;
        _uiManager.UpdateEnemiesDefeat();
        if (EntityDefeat >= _maxGoals)
        {
            _posSpawnEnemies.gameObject.SetActive(false);
            CanRunSpawn = false;
            _popUp.SetActive(true);
            //TODO: Pop.Up GameOver Screen
        }
    }

    public void CheckIfPoolNotEmpty()
    {
        int countPool = EnemyPool.GetCountPool();  
        CanRunSpawn = countPool > 0;
    }

    public void ReturnMainScreen()
    {
        SceneManager.LoadScene("Menu");
    }
 
    public static FactoryObject<Updateable> CreateFactory(Transform parent, Updateable prefab)
    {
        FactoryObject<Updateable> bulletFactory = new FactoryObject<Updateable>(prefab, parent);
        return bulletFactory;
    }
    public static ObjectPool<Updateable> CreatePool(FactoryObject<Updateable> factoryObject, int countObj)
    {
        var pool = new ObjectPool<Updateable>(factoryObject);
        pool.Initialization(countObj);
        return pool;
    }
}

