using EchoUtilities;
using UnityEngine;

public interface IDrop
{
    public abstract void SetCurrentNote(Note note);
    public abstract void RemoveOldNote(Note note);
    public abstract GameObject getGameObject();
}
