using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        None = -1,  // �����
        Ready = 0,  // �غ� �Ϸ�
        Appear,     // ����
        Battle,     // ������
        Dead,       // ���
        Disappear,  // ����
    }

    /// <summary>
    /// ���� ���°�
    /// </summary>
    [SerializeField]
    State CurrentState = State.None;

    /// <summary>
    /// �ְ� �ӵ�
    /// </summary>
    const float MaxSpeed = 10.0f;

    /// <summary>
    /// �ְ� �ӵ��� �̸��� �ð�
    /// </summary>
    const float MaxSpeedTime = 0.5f;


    /// <summary>
    /// ��ǥ��
    /// </summary>
    [SerializeField]
    Vector3 TargetPosition;

    [SerializeField]
    float CurrentSpeed;

    /// <summary>
    /// ������ ����� �ӵ� ����
    /// </summary>
    Vector3 CurrentVelocity;

    float MoveStartTime = 0.0f; // �̵����� �ð�


    float BattleStartTime = 0.0f;   // ���� ���� �ð�

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            Appear(new Vector3(7, transform.position.y, transform.position.z));
        }
        //
        switch (CurrentState)
        {
            case State.None:
            case State.Ready:
            case State.Dead:
                return;
            case State.Appear:
            case State.Disappear:
                UpdateSpeed();
                UpdateMove();
                break;
            case State.Battle:
                UpdateBattle();
                break;
            default:
                Debug.LogError("Undefined State!");
                break;
        }
    }

    void UpdateSpeed()
    {
        // CurrentSpeed ���� MaxSpeed �� �����ϴ� ������ �帥 �ð���ŭ ���
        CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, (Time.time - MoveStartTime) / MaxSpeedTime);
    }

    void UpdateMove()
    {
        float distance = Vector3.Distance(TargetPosition, transform.position);
        if (distance == 0)
        {
            Arrived();
            return;
        }

        // �̵����� ���. �� ������ ���� ���� �̵����͸� ������ nomalized �� �������͸� ���Ѵ�. �ӵ��� ���� ���� �̵��� ���͸� ���
        CurrentVelocity = (TargetPosition - transform.position).normalized * CurrentSpeed;

        // �ڿ������� �������� ��ǥ������ ������ �� �ֵ��� ���
        // �ӵ� = �Ÿ� / �ð� �̹Ƿ� �ð� = �Ÿ�/�ӵ�
        transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref CurrentVelocity, distance / CurrentSpeed, MaxSpeed);
    }

    void UpdateBattle()
    {
        // �ӽ÷� 3�� ����� ����
        if (Time.time - BattleStartTime > 3.0f)
        {
            Disappear(new Vector3(-15, transform.position.y, transform.position.z));
        }
    }

    void Arrived()
    {
        CurrentSpeed = 0.0f;    // ���������Ƿ� �ӵ��� 0
        if (CurrentState == State.Appear)
        {
            CurrentState = State.Battle;
            BattleStartTime = Time.time;
        }
        else if (CurrentState == State.Disappear)
            CurrentState = State.None;
    }

    public void Appear(Vector3 targetPos)
    {
        TargetPosition = targetPos;
        CurrentSpeed = MaxSpeed;    // ��Ÿ������ �ְ� ���ǵ�� ����

        CurrentState = State.Appear;
        MoveStartTime = Time.time;
    }

    void Disappear(Vector3 targetPos)
    {
        TargetPosition = targetPos;
        CurrentSpeed = 0.0f;           // ��������� 0���� �ӵ� ����

        CurrentState = State.Disappear;
        MoveStartTime = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player)
            player.OnCrash(this);
    }

    public void OnCrash(Player player)
    {
        Debug.Log("OnCrash player = " + player);
    }

}