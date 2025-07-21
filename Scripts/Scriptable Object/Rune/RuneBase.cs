using UnityEngine;

public interface IRuneOption
{
    string GetOptionText();
}

public interface IRuneEffect
{
    void ApplyEffect(CharacterInventoryManager manager);
    void RemoveEffect(CharacterInventoryManager manager);
}


public abstract class RuneBase : ScriptableObject
{
    public Sprite ItemImage;        // 이미지
    public string ID;               // 고유 아이디
    public string Name;             // 이름
    public string Explanation;      // 설명
}
