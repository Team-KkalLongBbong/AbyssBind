using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    //���ȼ���=================================================================================
    [SerializeField]
    protected int _level;
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _maxHp;
    [SerializeField]
    protected int _attack;
    [SerializeField]
    protected int _denfense;
    [SerializeField]
    protected float _movespeed;


    //���� ������Ƽ ����=================================================================================
    public int Level { get { return _level; } set { _level = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _denfense; } set { _denfense = value; } }
    public float MoveSpeed { get { return _movespeed; } set { _movespeed = value; } }


    //Start�� ���� �ʱ�ȭ=================================================================================
    public void Start()
    {
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _attack = 10;
        _denfense = 5;
        _movespeed = 2.0f;
    }


    //������ ������ ȣ��=================================================================================
    public virtual void OnAttacked(Stat attacker)
    {
        int damage = Mathf.Max(0, attacker.Attack - Defense);
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead(attacker);
        }
    }


    //��� ȣ��=================================================================================
    protected virtual void OnDead(Stat attacker)
    {
        /*
        PlayerStat playerStat = attacker as PlayerStat;
        if (playerStat != null)
        {
            playerStat.Exp += 5;
        }
        */
        Managers.Game.Despawn(gameObject);
    }
}
