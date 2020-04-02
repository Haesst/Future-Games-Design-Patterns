using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileManager
{
    public static FileManager Instance { get; } = new FileManager();

    private FileManager()
    {

    }

    public string GetFileName()
    {
        return "MAP_1.txt";
    }
}
