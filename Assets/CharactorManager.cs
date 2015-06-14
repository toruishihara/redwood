using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharactorManager : MonoBehaviour {
    public LogManager _onTheLog;
    public LogManager _pushingLog;
    public GameControl _gameControl;
    public GameObject _debugText;
    private int _lastMove = 0;

	// Use this for initialization
	void Start () {
        _gameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControl>();
        _debugText = GameObject.FindGameObjectWithTag("DebugText");
	}
	
	// Update is called once per frame
	void Update () {
        checkFlick();
        if (_onTheLog == null)
            return;
        _debugText.GetComponent<Text>().text = _onTheLog._logName;
        float horizontal = Input.GetAxis("Horizontal6");
        float vertical = Input.GetAxis("Vertical7");
        bool jump = Input.GetButtonDown("Jump");
        bool jumped = false;
        //if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
            //Debug.Log("log=" + _onTheLog.name + " v=" + vertical + " h=" + horizontal);
        if (_lastMove > 0)
        {
            horizontal = (int)(_lastMove / 10) - 2;
            vertical = (_lastMove % 10) - 2;
            vertical *= -1;
            if (Mathf.Abs(horizontal) > 0.1f && Mathf.Abs(vertical) > 0.1f)
                jumped = Jump(horizontal, vertical);
            Debug.Log("log=" + _onTheLog.name + " v=" + vertical + " h=" + horizontal);
            horizontal *= 10.0f;
            vertical *= 10.0f;
            _lastMove = 0;
        }
        if (jump)
            jumped = Jump(horizontal, vertical);
        if (_onTheLog._isVertical)
        {
            transform.position = new Vector3(_onTheLog.transform.position.x, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, _onTheLog.transform.position.z);
        }

        if (!jumped) {
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                if (!_onTheLog._isVertical)
                {
                    transform.position += new Vector3(0.1f * horizontal, 0, 0);
                    Debug.Log("Char move horizontal");
                }
                else
                {
                    PushLog(new Vector3(horizontal, 0, vertical));
                }
            }
            if (Mathf.Abs(vertical) > 0.1f)
            {
                if (_onTheLog._isVertical)
                {
                    transform.position += new Vector3(0, 0, 0.1f * vertical);
                    Debug.Log("Char move vertical");
                }
                else
                {
                    PushLog(new Vector3(horizontal, 0, vertical));
                }
            }
        }
        KeepOnTheLog();
    }

    public void move(int dir)
    {
        _lastMove = dir;
    }

    void PushLog(Vector3 dir)
    {
        float len = _onTheLog.transform.localScale.x * GameControl.prefabLogHeight/2;
        int cnt = 0;
        //LogManager[] logs = _gameControl.FindPConnectedLog(_onTheLog, dir);
        foreach(LogManager log in _onTheLog._colliding) {
            if (_onTheLog._isVertical)
            {
                if (Mathf.Abs(log.transform.position.z - _onTheLog.transform.position.z) < len)
                {
                    ++cnt;
                    _pushingLog = log;
                }
            }
            else
            {
                if (Mathf.Abs(log.transform.position.x - _onTheLog.transform.position.x) < len)
                {
                    ++cnt;
                    _pushingLog = log;
                }
            }
        }
        //LogManager[] logs = (LogManager[])_onTheLog._colliding.ToArray(typeof(LogManager));
        //_debugText.GetComponent<Text>().text = _onTheLog._logName + ":" + logs.Length;
        if (_onTheLog._isVertical)
        {
            _onTheLog.GetComponent<Rigidbody>().AddForce(new Vector3(10.0f * dir.x, 0, 0));
            Debug.Log("Log move horizontal");
            if (cnt == 1)
            {
                Debug.Log("2 Log move horizontal");
                _pushingLog.GetComponent<Rigidbody>().mass = 1.0f;
                _pushingLog.GetComponent<Rigidbody>().AddForce(new Vector3(10.0f * dir.x, 0, 0));
            }
        }
        else
        {
            _onTheLog.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 10.0f * dir.z));
            Debug.Log("Log move vertical");
            if (cnt == 1)
            {
                Debug.Log("2 Log move vertical");
                _pushingLog.GetComponent<Rigidbody>().mass = 1.0f;
                _pushingLog.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 10.0f * dir.z));
            }
        }

    }

    void KeepOnTheLog() {
        // If out of the edge of log, back to the edge
        float len = 1.3f * _onTheLog.transform.localScale.x;
        if (!_onTheLog._isVertical && transform.position.x > _onTheLog.transform.position.x + len)
        {
            transform.position = new Vector3(_onTheLog.transform.position.x + len, transform.position.y, transform.position.z);
            //Debug.Log("Char position fix1");
        }
        if (!_onTheLog._isVertical && transform.position.x < _onTheLog.transform.position.x - len)
        {
            transform.position = new Vector3(_onTheLog.transform.position.x - len, transform.position.y, transform.position.z);
            //Debug.Log("Char position fix2");
        }
        if (_onTheLog._isVertical && transform.position.z < _onTheLog.transform.position.z - len)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, _onTheLog.transform.position.z - len);
            //Debug.Log("Char position fix3");
        }
        if (_onTheLog._isVertical && transform.position.z > _onTheLog.transform.position.z + len)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, _onTheLog.transform.position.z + len);
            //Debug.Log("Char position fix4");
        }
	}

    bool Jump(float h, float v)
    {
        Vector3 newPos = transform.position + new Vector3(h, 0, v);
        LogManager newLog = _gameControl.FindLogByPos(newPos);
        if (newLog != null)
        {
            if (_pushingLog != null)
            {
                _pushingLog.GetComponent<Rigidbody>().mass = 1000.0f;
                _pushingLog = null;
            }
            _onTheLog.GetComponent<Rigidbody>().mass = 1000.0f;
            _onTheLog = newLog;
            _onTheLog.GetComponent<Rigidbody>().mass = 1.0f;
            return true;
        }
        return false;
    }
    Vector3 _touchStart;

    private void checkFlick()
    {
        Debug.Log("touches=" + Input.touchCount);
        if (Input.touchCount == 0)
            return;
        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                _touchStart = touch.deltaPosition;
                _touchStart.z = transform.position.z - Camera.main.transform.position.z;
                _touchStart = Camera.main.ScreenToWorldPoint(_touchStart);
                break;
            case TouchPhase.Moved:
                //Vector3 pos = touch.deltaPosition;
                //rigidbody.velocity = pos;
                break;
            case TouchPhase.Ended:
                Vector3 touchEnd = touch.deltaPosition;
                touchEnd.z = transform.position.z - Camera.main.transform.position.z;
                touchEnd = Camera.main.ScreenToWorldPoint(touchEnd);

                Vector3 dir = touchEnd - _touchStart;
                dir.z = dir.magnitude;
                dir.Normalize();

                //rigidbody.AddForce(dir * power);

                //StartCoroutine(ReturnBall());
                break;
        }
    }
}
