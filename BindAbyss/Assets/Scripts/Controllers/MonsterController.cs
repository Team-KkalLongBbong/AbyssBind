using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{

    //��������=================================================================================
    Stat _stat;

    [SerializeField]
    float _scanRange = 10;
    [SerializeField]
    float _atkRange = 2;


    //Init=================================================================================
    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;
        _stat = gameObject.GetComponent<Stat>();

        if(gameObject.GetComponentInChildren<UI_HpBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HpBar>(transform);
    }


    //����� =================================================================================
    protected override void UpdateIdle()
    {
        GameObject player = Managers.Game.GetPlayer();
        if (player == null)
            return;

        //���� ����ȭ�� ���ؼ��� sqrMagnitude ���
        float distance = (player.transform.position - transform.position).magnitude;

        //�������� >> �̵�
        if(distance <= _scanRange)
        {
            _lockTarget = player;
            State = Define.State.Moving;
            return;
        }
    }


    //�̵���� =================================================================================
    protected override void UpdateMoving()
    {
        //�������� >> ����
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= _atkRange)
            {
                NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma.SetDestination(transform.position);

                State = Define.State.Skill;
                return;
            }
        }

        //float������ ���������� �׻� ���������� �ֱ� ������ �ؼҰ����� ���
        Vector3 dir = _destPos - transform.position;

        //�������� >> ����
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
            nma.SetDestination(_destPos);
            nma.speed = _stat.MoveSpeed;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }
    }


    //��ų��� �������� =================================================================================
    protected override void UpdateSkill()
    {
        Debug.Log("Monster UpdateSkill");

        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    //�ִϸ��̼� �̺�Ʈ: Ÿ�ݽ� �ߵ� =================================================================================
    void OnHitEvent()
    {
        Debug.Log("Monster OnHitEvent");

        if (_lockTarget != null)
        {
            //ü��
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            targetStat.OnAttacked(_stat);

            if(targetStat.Hp > 0)
            {
                float distance = (_lockTarget.transform.position - transform.position).magnitude;
                if (distance <= _atkRange)
                    State = Define.State.Skill;
                else
                    State = Define.State.Moving;
            }
            else
                State = Define.State.Idle;
        }
        else
        {
            State = Define.State.Idle;
        }
    }
}
