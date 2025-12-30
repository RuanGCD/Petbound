[System.Serializable]
public class TeamSlot
{
    public PetRuntime pet;

    public bool IsEmpty => pet == null;
}
