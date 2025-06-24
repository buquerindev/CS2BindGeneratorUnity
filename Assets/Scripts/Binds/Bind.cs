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

    public void SetMouseKey(string key)
    {
        americanKey = key;
        localKey = key;
        scancode = key;
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
