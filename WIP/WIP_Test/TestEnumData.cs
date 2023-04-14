using KorYmeLibrary.Attributes;
using KorYmeLibrary.PasEncoreNommé;
using UnityEngine;

public class TestEnumData : MonoBehaviour
{
    public enum TEXTSTYLES
    {
        MEDIUM,
        TINY,
        SMALL,
        BIG,
        HUGE,
        SO_HUGE,
    }

    [ForceInterface(typeof(IWeapon))]
    public Object weapon;

    [EnumData(typeof(TEXTSTYLES))]
    public TextData[] _textData;

    [SerializeField]
    EnumDataContainer<int, TEXTSTYLES> _container;
}

[System.Serializable]
public class TextData
{
    public int _size;
    public Color _color;
}