using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Bind
{
    public string name;
    public string ingameName;
    public string description;
    public string category;
    public string subcategory;

    public string americanKey;
    public string localKey;
    public string scancode = null;
    
    public string secondAmericanKey;
    public string secondLocalKey;
    public string secondScancode = null;

    public string values; // For toggles 

    public void SetMouseKey(string key)
    {
        if (string.IsNullOrEmpty(scancode))
        {
            scancode = key;
            americanKey = key;
            localKey = key;
        } else
        {
            secondScancode = key;
            secondAmericanKey = key;
            secondLocalKey = key;
        }
    }

    public void UnbindMouseKey(string key)
    {
        if(scancode == key)
        {
            americanKey = null;
            localKey = null;
            scancode = null;
        } else
        {
            secondAmericanKey = null;
            secondLocalKey = null;
            secondScancode = null;
        } 
    }
}

public class BindList
{
    public List<Bind> binds;

    public void Add(Bind bind)
    {
        binds.Add(bind);
    }

    public BindList()
    {
        binds = new List<Bind>();
    }
}
