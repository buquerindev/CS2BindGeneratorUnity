using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Command
{
    public string name;
    public string ingameName;
    public string description;
    public object defaultValue;
    public string category;
    public string subcategory;
    
    public object min;
    public object max;

    public List<int> enumValues;
    public List<string> enumNames;

    public List<string> options;
    public List<string> optionsNames;

    public object selectedValue;
}

public class CommandList
{
    public List<Command> commands;

    // Constructor
    public CommandList()
    {
        commands = new List<Command>();
    }
}
