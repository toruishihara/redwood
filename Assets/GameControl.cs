using UnityEngine;
using System.Collections;

public class LogData
{
    int x;
    int z;
    int len;
    bool isVertical;
}

public class GameControl : MonoBehaviour {
    private CharactorManager _character;
    private ArrayList _logs;
    static public float prefabLogHeight = 2.7f;
    public GameObject LogPrefab;
    public GameObject RedLogPrefab;
    //public GameObject SingleLeg = (GameObject)Instantiate(Resources.Load("prefab/LogSingle"));
    private int[] _logData = {   1, 2, 4, 1, 
                                3, 2, 5, 1,
                                1, 7, 5, 0,
                                9, 4, 4, 0,
                             };

	// Use this for initialization
	void Start () {
        GameObject charactor = GameObject.FindGameObjectWithTag("Player");
        _character = charactor.GetComponent<CharactorManager>();
        _logs = new ArrayList();

        for (int i = 0; i < _logData.Length; i += 4) {
            GameObject go;
            if (i==0)
                go = (GameObject)Instantiate(RedLogPrefab, Vector3.zero, Quaternion.identity);
            else
                go = (GameObject)Instantiate(LogPrefab, Vector3.zero, Quaternion.identity);
            go.transform.localScale = new Vector3(_logData[i + 2] / prefabLogHeight, 1, 1);
            LogManager log = (LogManager)go.GetComponent<LogManager>();
            log._logName = "Log" + (i/4).ToString();
            if (_logData[i + 3] == 1)
            {
                go.transform.position = new Vector3(-7.5f + _logData[i], 0.5f, -8.0f + _logData[i + 1] + 0.5f*_logData[i + 2]);
                go.transform.Rotate(new Vector3(0, 90, 0));
                log._isVertical = true;
            }
            else
            {
                go.transform.position = new Vector3(-8.0f + _logData[i] + 0.5f*_logData[i + 2], 0.5f, -7.5f + _logData[i + 1]);
                log._isVertical = false;
            }
            log.GetComponent<Rigidbody>().mass = 1000.0f;
            if (i==0) {
                _character._onTheLog = log;
                log.GetComponent<Rigidbody>().mass = 1.0f;
            }
            _logs.Add(log);
        }
    }
	
	// Update is called once per frame
	void Update () {
        float h4 = Input.GetAxis("Horizontal4");
        float v5 = Input.GetAxis("Vertical5");
        //float h6 = Input.GetAxis("Horizontal");
        float v7 = Input.GetAxis("Vertical");
        if (Mathf.Abs(h4) > 0.2f || Mathf.Abs(v5) > 0.2f || Mathf.Abs(v7) > 0.2f)
        {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            Vector3 oldCar = camera.transform.position;
            Vector3 oldSph = Car2Sph(oldCar);
            Vector3 newCar = oldCar + new Vector3(-0.2f * h4, 0f, -0.2f * v5);
            Vector3 newSph = Car2Sph(newCar);
            float ratio = (1.0f - 0.01f * v7) * oldSph.x / newSph.x;
            Debug.Log("ratio=" + ratio + " old=" + oldCar + " new=" + newCar + "v7=" + v7);

            camera.transform.position = ratio * newCar;
            camera.transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
        }
        for (int x = -10; x <= 10; ++x)
        {
            Debug.DrawLine(new Vector3(x, 1, -10), new Vector3(x, 1, 10), Color.blue, 2, false);
            Debug.DrawLine(new Vector3(-10, 1, x), new Vector3(10, 1, x), Color.blue, 2, false);
        }

    }

    public LogManager FindLogByPos(Vector3 pos)
    {
        Debug.Log("FindLog pos=" + pos);
        foreach (LogManager log in _logs)
        {
            Debug.Log("each log v=" + log._isVertical + " pos=" + log.transform.position + " scale=" + log.transform.localScale.x);
            if (log._isVertical && Mathf.Abs(log.transform.position.x - pos.x) < 0.75f)
            {
                if (Mathf.Abs(log.transform.position.z - pos.z) < log.transform.localScale.x * prefabLogHeight/2)
                {
                    Debug.Log("set onTheLog=" + log);
                    return log;
                }
            }
            if (!log._isVertical && Mathf.Abs(log.transform.position.z - pos.z) < 0.75f)
            {
                if (Mathf.Abs(log.transform.position.x - pos.x) < log.transform.localScale.x * prefabLogHeight/2)
                {
                    Debug.Log("set onTheLog=" + log);
                    return log;
                }
            }
        }
        Debug.Log("return null");
        return null;
    }

    public LogManager[] FindPConnectedLog(LogManager inLog, Vector3 dir)
    {
        ArrayList ret = new ArrayList();
        bool debugBtn = Input.GetButtonDown("Fire2");
        float inLen = prefabLogHeight * inLog.transform.localScale.x;
        foreach (LogManager log in _logs)
        {
            Debug.DrawLine(new Vector3(inLog.transform.position.x, 2, inLog.transform.position.z + inLen/2), 
                    new Vector3(inLog.transform.position.x, 2, inLog.transform.position.z - inLen/2), Color.green, 2, false);
            if (log._isVertical == inLog._isVertical)
                continue;
            float len = prefabLogHeight * log.transform.localScale.x;
            if (inLog._isVertical)
            {
                Debug.DrawLine(new Vector3(-10, 2, inLog.transform.position.z + inLen/2), 
                    new Vector3(10, 2, inLog.transform.position.z + inLen/2), Color.blue, 2, false);
                Debug.DrawLine(new Vector3(-10, 2, inLog.transform.position.z - inLen/2), 
                    new Vector3(10, 2, inLog.transform.position.z - inLen/2), Color.blue, 2, false);
                if (inLog.transform.position.z + inLen/2 < log.transform.position.z)
                    continue;
                if (inLog.transform.position.z - inLen/2 > log.transform.position.z)
                    continue;
                //Debug.Log("Con check " +log.transform.position.x +":"+ inLog.transform.position.x  +":"+ inLen  +":"+ dir.x);
                //Debug.DrawLine(inLog.transform.position, log.transform.position, Color.red, 2, false);
                if (Mathf.Abs(inLog.transform.position.x + dir.x - log.transform.position.x + len / 2) < 0.35f)
                {
                    ret.Add(log);
                    Debug.Log("found connected " + log.name);
                }
                if (Mathf.Abs(inLog.transform.position.x + dir.x - log.transform.position.x - len / 2) < 0.35f)
                {
                    ret.Add(log);
                    Debug.Log("found connected " + log.name);
                }

            }
            else
            {
                if (log.transform.position.z + log.transform.localScale.x < inLog.transform.position.z)
                    continue;
                if (log.transform.position.z - log.transform.localScale.x > inLog.transform.position.z)
                    continue;
                Debug.Log("Con check " +log.transform.position.z +":"+ inLog.transform.position.z  +":"+ inLog.transform.localScale.x  +":"+ dir.z);
                if (Mathf.Abs(log.transform.position.z - inLog.transform.position.z + inLog.transform.localScale.x + dir.z) < 0.5f)
                {
                    ret.Add(log);
                    Debug.Log("found connected " + log.name);
                }
            }
        }
        return (LogManager[])ret.ToArray(typeof(LogManager));
    }

    Vector3 Car2Sph(Vector3 c) {
        float r = Mathf.Sqrt(c.x * c.x + c.y * c.y + c.z * c.z);
        float th = Mathf.Acos(c.y / r);
        float ps;
        if (Mathf.Abs(c.x) > Mathf.Abs(c.z))
            ps = Mathf.Asin(c.z / r);
        else
            ps = Mathf.Acos(c.x / r);
        return new Vector3(r, th, ps);
    }

    Vector3 Sph2Car(Vector3 s)
    {
        return new Vector3(s.x * Mathf.Sin(s.y) * Mathf.Cos(s.z), s.x * Mathf.Cos(s.y), s.x * Mathf.Sin(s.y) * Mathf.Sin(s.z));
    }
}
