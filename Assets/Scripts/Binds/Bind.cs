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

    public string scancode;
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
