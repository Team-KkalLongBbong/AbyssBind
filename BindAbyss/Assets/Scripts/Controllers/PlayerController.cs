using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : BaseController
{

    //��������=================================================================================
    int _mouseMask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    Stat  _stat;

    [SerializeField]
    bool _stopSkill = false;


    //Init=================================================================================
    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Player;
        _stat = gameObject.GetComponent<Stat>();
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        if (gameObject.GetComponentInChildren<UI_HpBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HpBar>(transform);
    }


    /*
    State����.
    ���� �÷��̾��� ���¸� ���������� ������
    �� �� ������ ���¸��� �Լ��� ĸ��ȭ �ؼ� �ѹ��� �ϳ��� �ִϸ��̼� ����
    �ѹ��� �� ���¹ۿ� �ټ� ���ٴ� ������ ������ ���� �������� ����ϱ� ������.
    ������ �� ������ �ǳĸ�, �����̸鼭 �ֹ����Ҷ��� moving, skill �ΰ��� ���°� �ʿ��ϱ� ����.
     */


    //�̵�=================================================================================
    protected override void UpdateMoving()
    {
        //�������� >> ����
        if(_lockTarget != null)
        {
            float distance = (_destPos - transform.position).magnitude;
            if(distance <= 1.8f)
            {
                State = Define.State.Skill;
                return;
            }
        }

        Vector3 dir = _destPos - transform.position;
        dir.y = 0;

        //float������ ���������� �׻� ���������� �ֱ� ������ �ؼҰ����� ���
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up, dir.normalized, Color.green);
            if(Physics.Raycast(transform.position + Vector3.up, dir, 1.0f, LayerMask.GetMask("Block")))
            {
                if(Input.GetMouseButton(0) == false)
                    State = Define.State.Idle;
                return;
            }

            /*  normalize�� ���� ������ ���Ͱ�(����� �ӵ� ���� �������)�� �̹��� ������ ����.
                normalizs�� ���Ͱ��� 1�� �ٲ㼭 �ӵ��� �����ϰ� �ϰ� ���Ⱚ�� �����;���.
                Clamp�� ù��° �������� �ִ��� ������ �ι�°�� ����° �� ������ ������ ����� ���.  */

            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }
    }


    //����=================================================================================
    protected override void UpdateSkill()
    {
        if(_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    //�ִϸ��̼� �̺�Ʈ : Ÿ�ݽ� �ߵ�
    void OnHitEvent()
    {
        Debug.Log("player skill on");

        if(_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            Stat myStat = gameObject.GetComponent<Stat>();
            targetStat.OnAttacked(_stat);
        }

        if (_stopSkill)
        {
            Debug.Log("stopskill true");
            State = Define.State.Idle;
        }
        else
        {
            Debug.Log("stopskill false");
            State = Define.State.Skill;
        }

    }


    //���콺 ���� ó��=================================================================================
    void OnMouseEvent(Define.MouseEvent evt)
   {
        switch (State)
        {
            case Define.State.Die:
                break;
            case Define.State.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Skill:
                {
                    if (evt == Define.MouseEvent.PointerUp)
                        _stopSkill = true;
                }
                break;
        }
    }


    //���콺 ����ó�� 2 =================================================================================
    void OnMouseEvent_IdleRun(Define.MouseEvent evt)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mouseMask);
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {
                    if (raycastHit)
                    {
                        _destPos = hit.point;
                        State = Define.State.Moving;
                        _stopSkill = false;

                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                            _lockTarget = hit.collider.gameObject;
                        else
                        {
                            _lockTarget = null;
                        }
                    }
                }
                break;
            case Define.MouseEvent.Press:
                {
                        if (_lockTarget == null && raycastHit)
                        _destPos = hit.point;
                }
                break;
            case Define.MouseEvent.PointerUp:
                _stopSkill = true;
                break;
        }
    }
}
