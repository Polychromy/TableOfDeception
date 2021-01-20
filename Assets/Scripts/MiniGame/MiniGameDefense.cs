using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameDefense
{
    public string defenseName { get; set; }
    public string DescriptionReflect { get; }
    public string DescriptionSplit { get; }
    public string DescriptionNoEffect { get; }

    public MiniGameDefense(string defenseName, string descriptionReflect, string descriptionSpilt, string descriptionNoEffect)
    {
        this.defenseName = defenseName;
        DescriptionReflect = descriptionReflect;
        DescriptionSplit = descriptionSpilt;
        DescriptionNoEffect = descriptionNoEffect;
    }

}
