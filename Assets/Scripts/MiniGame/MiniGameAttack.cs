using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameAttack
{
    public string attackName {get;}
    public string DescriptionReflect { get; }
    public string DescriptionSplit { get; }
    public string DescriptionNoEffect { get; }
    public string reflectedBy { get; }
    public string returnedBy { get; }
    public int damageDone { get; }

    public MiniGameAttack(string attackName, string descriptionReflect, string descriptionSplit, string descriptionNoEffect, string reflectedBy, string returnedBy, int damageDone)
    {
        this.attackName = attackName;
        DescriptionReflect = descriptionReflect;
        DescriptionSplit = descriptionSplit;
        DescriptionNoEffect = descriptionNoEffect;
        this.reflectedBy = reflectedBy;
        this.returnedBy = returnedBy;
        this.damageDone = damageDone;
    }


    public (int, string) GetDamage(string defenseName)
    {
        if (defenseName == this.returnedBy)
        {
            return (damageDone, "traitor");
        }
        if (defenseName == this.reflectedBy)
        {
            return (damageDone/2, "both");
        }
        return (damageDone, "knight");
    }
}
