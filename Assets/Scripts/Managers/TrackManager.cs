using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class TrackManager : MonoBehaviour {

        private static TrackManager instance = null;
        public static TrackManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TrackManager>();
                }
                return instance;
            }
        }

        public float killZoneY = -30f;

        public enum State { Ready, Play, Finish, None };
        private State state;

        public delegate void SimpleDelegate();
        public event SimpleDelegate OnTrackCountDown;
        public event SimpleDelegate OnTrackInit;
        public event SimpleDelegate OnTrackStart;
        public event SimpleDelegate OnTrackFinish;
        public event SimpleDelegate OnTrackOutro;

        public delegate void FloatDelegate(float f);
        public event FloatDelegate OnTimeChange;

        public delegate void IntDelegate(int i);
        public event IntDelegate OnPosPlayerChange;
    

        public delegate void TwoIntDelegate(int i, int j);
        public event TwoIntDelegate OnPlayerLapCountChange;

        private float timeStart;
        private float timeTrack;

        GameObject pathGroup;
        List<Vector3> pathPoints;
        List<Vector3> crossPathPoints;

        GameObject player;
        List<GameObject> carList;
        List<CarInfo> carOrder;

        private int nbTrackLaps;
        public int NbTrackLaps
        {
            get { return nbTrackLaps; }
            set { nbTrackLaps = value; }
        }

        GameObject oneLabel;
        GameObject twoLabel;
        GameObject threeLabel;
        GameObject goLabel;
        GameObject finishLabel;

        public float reachDist = 10.0f;
        public float halfSegmentSize = 6f;

        private int lastPlayerPos;

        private GameObject hudObject;

        // Use this for initialization
        void Awake () { 
            state = State.None;
            GameManager.Instance.ToLevel += () =>
            {
                Init();
            };
        }

        public void Init ()
        {
            if (OnTrackInit != null)
                OnTrackInit();

            InitInterface();

            pathGroup = GameObject.FindGameObjectWithTag("Path");

            InitListPoints();
            InitVectorCheckPoints();
            InitTrack();

            state = State.Ready;

            FreezeCars();
            StartCoroutine(CountDownCoroutine());
        
        }

        void InitTrack ()
        {
            carList = new List<GameObject>();
            carOrder = new List<CarInfo>();
            GameObject[] tmpList = GameObject.FindGameObjectsWithTag("Enemy");
            Debug.Log("NB enemy : " + tmpList.Length);
            player = GameObject.Find("Player");
            foreach(GameObject o in tmpList)
            {
                if (o == null)
                    continue;
                carList.Add(o);
                carOrder.Add(o.GetComponent<CarInfo>());
            }
            carList.Add(player);
            CarInfo playerCarInfo = player.GetComponent<CarInfo>();
            playerCarInfo.OnLapCountChange += (int lap) =>
            {
                if (lap == nbTrackLaps + 1)
                {
                    EndOfTrack();
                    return;
                }
                if (OnPlayerLapCountChange != null)
                    OnPlayerLapCountChange(lap, nbTrackLaps);
            };
            carOrder.Add(playerCarInfo);
            carOrder.Sort();

            lastPlayerPos = carOrder.Count;
            if (OnPosPlayerChange != null)
                OnPosPlayerChange(lastPlayerPos);
        }

        void InitInterface ()
        {
            /*GameObject o = Instantiate(hudPrefab);
        o.transform.SetParent(GameObject.Find("Canvas").transform, false);*/

            oneLabel = GameObject.Find("One");
            twoLabel = GameObject.Find("Two");
            threeLabel = GameObject.Find("Three");
            goLabel = GameObject.Find("Go");
            finishLabel = GameObject.Find("Finish");

            oneLabel.SetActive(false);
            twoLabel.SetActive(false);
            threeLabel.SetActive(false);
            goLabel.SetActive(false);
            finishLabel.SetActive(false);
        }

        void StartOfTrack ()
        {
            state = State.Play;
            timeStart = Time.time;
            UnfreezeCars();
            if (OnTrackStart != null)
                OnTrackStart();
            StartCoroutine(TimerCoroutine());
        }

        void EndOfTrack ()
        {
            state = State.Finish;
            timeTrack = Time.time - timeStart;

            player.GetComponent<CarUserControl>().enabled = false;
            player.GetComponent<AICar>().enabled = true;

            new HighScoreManager(SceneManager.GetActiveScene().buildIndex)
                .AddScore(timeTrack);

            StartCoroutine(FinishCoroutine());
        }
	
        // Update is called once per frame
        void Update () {
            if (state == State.None)
                return;

            CheckCarAreInRoad();
            if (state == State.Finish)
                return;
            carOrder.Sort();
            if (GetPlayerPos() != lastPlayerPos)
            {
                lastPlayerPos = GetPlayerPos();
                if (OnPosPlayerChange != null)
                    OnPosPlayerChange(lastPlayerPos);
            }
        
        }

        void InitListPoints()
        {
            pathPoints = new List<Vector3>();
            if (pathGroup == null)
                return;
            Transform[] transformList = pathGroup.GetComponentsInChildren<Transform>();

            foreach (Transform t in transformList)
            {
                if (t != pathGroup.transform)
                    pathPoints.Add(t.position);
            }
        }

        void InitVectorCheckPoints()
        {
            crossPathPoints = new List<Vector3>();
            for (int i = 0; i < pathPoints.Count; i++)
            {
                Vector3 a = (pathPoints[PrevCheckPoint(i)] - pathPoints[i]).normalized;
                a.y = 0;
                Vector3 b = (pathPoints[NextCheckPoint(i)] - pathPoints[i]).normalized;
                b.y = 0;
                crossPathPoints.Add((a + b).normalized);
            }
        }

        void OnDrawGizmos()
        {
            InitListPoints();
            InitVectorCheckPoints();
            if (pathGroup == null)
                return;
            Vector3 cubeSize = new Vector3(halfSegmentSize * 2, halfSegmentSize * 2, halfSegmentSize * 2);
            Vector3 start, end;
            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
                Gizmos.DrawWireCube(pathPoints[i], cubeSize);
                Gizmos.color = Color.blue;
                start = pathPoints[i] + crossPathPoints[i] * halfSegmentSize ;
                end = pathPoints[i] - crossPathPoints[i] * halfSegmentSize ;
                Gizmos.DrawLine(start, end);
                //Gizmos.DrawSphere(start, 1f);
                //Gizmos.DrawSphere(end, 1f);
            }
            Gizmos.DrawWireCube(pathPoints[pathPoints.Count - 1], cubeSize);
            Gizmos.DrawLine(pathPoints[pathPoints.Count - 1], pathPoints[0]);
            start = pathPoints[pathPoints.Count - 1] + crossPathPoints[pathPoints.Count - 1] * halfSegmentSize;
            end = pathPoints[pathPoints.Count - 1] - crossPathPoints[pathPoints.Count - 1] * halfSegmentSize;
            Gizmos.DrawLine(start, end);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(pathPoints[0], cubeSize);
        }

        void RespawnCar (GameObject car)
        {
            CarInfo ai = car.GetComponent<CarInfo>();
            if (ai != null)
            {
                int nextPoint = NextCheckPoint(ai.LastPoint);
                ai.transform.position = pathPoints[ai.LastPoint] + new Vector3(0, 2, 0);
                ai.GetComponent<CarController>().Velocity = Vector3.zero;
                ai.GetComponent<CarController>().Speed = 0;
                ai.transform.LookAt(GetCheckPointPos(nextPoint));
            }
        }

        void CheckCarAreInRoad()
        {
            foreach (GameObject car in carList)
            {
                if (car == null)
                    continue;
                if (car.transform.position.y < killZoneY)
                    RespawnCar(car);
            }
        }

        public int GetPlayerLap ()
        {
            return player.GetComponent<CarInfo>().CurrentLap;
        }

        int NextCheckPoint(int point)
        {
            return (point + 1) % pathPoints.Count;
        }

        int PrevCheckPoint(int point)
        {
            if (point == 0)
                return pathPoints.Count - 1;
            return (point - 1);
        }

        public Vector3 GetCheckPointPos(int index)
        {
            return pathPoints[index];
        }

        public Vector3 GetCheckPointVec(int index)
        {
            return crossPathPoints[index];
        }

        public Vector3 GetNextCheckPointPos (int index)
        {
            return GetCheckPointVec(NextCheckPoint(index));
        }

        public int GetNextCheckPointIndex(int index)
        {
            return NextCheckPoint(index);
        }

        public int GetNbCheckPoint()
        {
            return pathPoints.Count;
        }

        IEnumerator FinishCoroutine()
        {
            finishLabel.SetActive(true);
            state = State.Finish;
            if (OnTrackFinish != null)
                OnTrackFinish();
            yield return new WaitForSeconds(3f);
            finishLabel.SetActive(false);
            yield return new WaitForSeconds(1f);
            if (OnTrackOutro != null)
                OnTrackOutro();
        }

        IEnumerator CountDownCoroutine ()
        {
            if (OnTrackCountDown != null)
                OnTrackCountDown();
            threeLabel.SetActive(true);
            yield return new WaitForSeconds(1f);
            threeLabel.SetActive(false);
            twoLabel.SetActive(true);
            yield return new WaitForSeconds(1f);
            twoLabel.SetActive(false);
            oneLabel.SetActive(true);
            yield return new WaitForSeconds(1f);
            oneLabel.SetActive(false);
            goLabel.SetActive(true);
            StartOfTrack();
            yield return new WaitForSeconds(1f);
            goLabel.SetActive(false);
            yield return null;
        }

        IEnumerator TimerCoroutine()
        {
            while (state == State.Play)
            {
                timeTrack = Time.time - timeStart;
                if (OnTimeChange != null)
                    OnTimeChange(timeTrack);
                yield return new WaitForSeconds(0.01f);
            }
        }

        int GetPlayerPos()
        {
            for (int i = 0; i < carOrder.Count; i++)
            {
                if (carOrder[i] == null)
                    continue;
                if (carOrder[i].CompareTag("Player"))
                {
                    return i + 1;
                }
            }
            return 1;
        }

        void FreezeCars()
        {
            foreach (GameObject o in carList)
            {
                if (o == null)
                    continue;
                o.GetComponent<CarController>().SetTopspeed(0);
            }
        }

        void UnfreezeCars()
        {
            foreach (GameObject o in carList)
            {
                if (o == null)
                    continue;
                o.GetComponent<CarController>().UnsetTopspeed();
            }
        }

        public CarInfo GetCarBefore (GameObject car)
        {
            int index = carOrder.IndexOf(car.GetComponent<CarInfo>());
            if (index == 0)
                return carOrder[carOrder.Count - 1];
            return  carOrder[index - 1];
        }

    }
}
