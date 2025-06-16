using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Command
{
    private string name;
    private string ingameName;
    private string description;
    private object defaultValue;
    private string category;
    private string subCategory;
    
    private object min;
    private object max;

    private List<int> enumValues;
    private List<string> enumNames;
}
