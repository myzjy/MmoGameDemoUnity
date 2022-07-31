using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
/*
 *  单例模板
 * @author zjy
 * @time 2020/3/21 14:52
 **/
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance = null;

    public static T Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            Init();
        }
    }

    public void Start()
    {
    }

    protected virtual void Init()
    {
    }

    public static bool IsValid()
    {
        return instance != null;
    }

    protected bool isQuit = false;

    private void OnApplicationQuit()
    {
        instance = null;
        isQuit = true;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }

        OnRelease();
    }

    protected virtual void OnRelease()
    {
    }
}