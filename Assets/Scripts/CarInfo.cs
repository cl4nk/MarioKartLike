using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Managers;

public class CarInfo : MonoBehaviour, System.IComparable<CarInfo>
{

    public delegate void IntDelegate(int i);
    public event IntDelegate OnLapCountChange;

    private int lastPoint = 0;
    public int LastPoint
    {
        get { return lastPoint; }
        private set { lastPoint = value; }
    }

    private int nextPoint;
    public int NextPoint
    {
        get { return nextPoint; }
        private set { nextPoint = value; }
    }

    public int currentLap;
    public int CurrentLap
    {
        get { return currentLap; }
        private set { currentLap = value; }
    }

    public float reachDist = 10.0f;
    public float halfSegmentSize = 6f;

    protected bool canMove;

    void Start ()
    {
        InitCheckPoints();
    }
    
    void InitCheckPoints () {
        nextPoint = TrackManager.Instance.GetNextCheckPointIndex(lastPoint);
        currentLap = 1;
        if (OnLapCountChange != null)
            OnLapCountChange(currentLap);
    }


    // Update is called once per frame
    void Update () {
        Vector3 nextPointVec = transform.InverseTransformPoint(TrackManager.Instance.GetCheckPointPos(nextPoint));
        nextPointVec.y = 0;

        if (nextPointVec.magnitude <= reachDist)
        {
            lastPoint = nextPoint;
            nextPoint = TrackManager.Instance.GetNextCheckPointIndex(nextPoint);
            if (lastPoint == 0)
            {
                currentLap++;
                if (OnLapCountChange != null)
                    OnLapCountChange(currentLap);
            }
        }
    }

    public float GetDistance()
    {
        float distFromNext = Vector3.Distance(TrackManager.Instance.GetCheckPointPos(nextPoint), transform.position) / 1000;

        return (-distFromNext)
        + lastPoint + currentLap * TrackManager.Instance.GetNbCheckPoint();
    }

    public int CompareTo(CarInfo other)
    {
        if (other.GetDistance() > GetDistance())
            return 1;
        if (other.GetDistance() < GetDistance())
            return -1;
        return 0;
    }
}
