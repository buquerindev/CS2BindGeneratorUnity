using Microsoft.Win32.SafeHandles;
using NUnit.Framework;
using System;
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
        float numericValue;

        try
        {
            numericValue = Convert.ToSingle(selectedValue); // esto sirve para int, float, double, string numérico, etc.
        }
        catch (Exception e)
        {
            Debug.LogError($"[Command] No se pudo convertir selectedValue ({selectedValue}) a float. Error: {e.Message}");
            return;
        }

        if (!convert)
            convertedValue = Mathf.Pow(numericValue / 100f, 2);
        else
            convertedValue = numericValue / 100f;

        Debug.Log($"Converted value ({name}): {convertedValue}");
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
