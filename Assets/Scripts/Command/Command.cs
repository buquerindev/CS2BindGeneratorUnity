using Microsoft.Win32.SafeHandles;
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

    public string type;
    
    public object min;
    public object max;

    public List<int> enumValues;
    public List<string> enumNames;

    public List<string> options;
    public List<string> optionsNames;

    public object selectedValue;
    public float? convertedValue = null;

    public void ConvertSoundValue(bool convert)
    {
        if (type == "bool" || type == "enum" || type == "string")
            return;

        if (!convert)
            convertedValue = Mathf.Pow((float)(int)selectedValue / 100f, 2);
        else
            convertedValue = (float)(int)selectedValue / 100f;

        Debug.Log("Converted value: " + convertedValue.ToString());
    }
}

public class CommandList
{
    public List<Command> commands;

    public void Add(Command command)
    {
        commands.Add(command);
    }

    // Constructor
    public CommandList()
    {
        commands = new List<Command>();
    }
}
