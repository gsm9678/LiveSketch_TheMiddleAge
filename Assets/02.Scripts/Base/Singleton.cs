using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindAnyObjectByType(typeof(T));

                if (_instance == null)
                {
                    SetupInstance();
                }
            }

            return _instance;
        }
    }

    public virtual void Awake()
    {
        RemoveDuplicates();
    }

    private static void SetupInstance()
    {
        _instance = (T)FindAnyObjectByType(typeof(T));

        if (_instance == null)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = typeof(T).Name;
            _instance = gameObject.AddComponent<T>();
            DontDestroyOnLoad(gameObject);
        }
    }

    private void RemoveDuplicates()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}