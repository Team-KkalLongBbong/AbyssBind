using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawningPool : MonoBehaviour
{
    [SerializeField]
    int _monsterCount = 0;
    int _reserveCount = 0;
    [SerializeField]
    int _keepMonsterCount = 0;
    [SerializeField]
    Vector3 _spawnPos;
    [SerializeField]
    float _spawnRadious = 15.0f;
    [SerializeField]
    float _spawnTIme = 5.0f;

    public void AddMonsterCount(int value) { _monsterCount += value;  }
    public void SetKeepMonsterCount(int count) { _keepMonsterCount = count; }

    // Start is called before the first frame update
    void Start()
    {
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    // Update is called once per frame
    void Update()
    {
        while(_reserveCount + _monsterCount < _keepMonsterCount)
        {
            StartCoroutine("ReserveSpawn");
        }
    }

    IEnumerator ReserveSpawn()
    {
        _reserveCount++;
        yield return new WaitForSeconds(Random.Range(0, _spawnTIme));

        GameObject obj = Managers.Game.Spawn(Define.WorldObject.Monster, "Crusader tank_prefab");
        NavMeshAgent nma = obj.GetOrAddComponent<NavMeshAgent>();
        
        Vector3 randPos;

        while (true)
        {
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadious);
            randDir.y = 0;
            randPos = _spawnPos + randDir;

            //���� �ֳ� üũ
            NavMeshPath path = new NavMeshPath();
            if(nma.CalculatePath(randPos, path))
                break;
        }

        obj.transform.position = randPos;
        _reserveCount--;
    }
}
