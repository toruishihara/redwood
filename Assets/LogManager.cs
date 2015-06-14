using UnityEngine;
using System.Collections;

public class LogManager : MonoBehaviour {
    public string _logName;
    public bool _isVertical = false;
    public ArrayList _colliding;
    void Start()
    {
        _colliding = new ArrayList();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;
        if (go.tag != "Log")
            return;
        LogManager log = go.GetComponent<LogManager>();
        if (_isVertical == log._isVertical)
            return;
        Debug.DrawLine(new Vector3(go.transform.position.x, 0, go.transform.position.z),
            new Vector3(go.transform.position.x, 20, go.transform.position.z),
            Color.red, 2, false);
        _colliding.Add(log);
    }

    void OnCollisionExit(Collision collision)
    {
        GameObject go = collision.gameObject;
        if (go.tag != "Log")
            return;
        LogManager log = go.GetComponent<LogManager>();
        if (_isVertical == log._isVertical)
            return;
        foreach(LogManager i in _colliding) {
            if (i == log)
            {
                _colliding.Remove(log);
                CharactorManager charactor = GameObject.FindGameObjectWithTag("Player").GetComponent<CharactorManager>();
                if (this == charactor._onTheLog)
                {
                    log.GetComponent<Rigidbody>().mass = 1000.0f;
                }
            }
        }
    }
}
