using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessCodeManager : MonoBehaviour
{
    public enum AccessCodes { None, Red, Green, Blue, Yellow };

    [SerializeField]
    public static Dictionary<AccessCodes, AccessCode> accessCodes = new Dictionary<AccessCodes, AccessCode>() {
        { AccessCodes.Red, new AccessCode("RED", Color.red, false) },
        { AccessCodes.Green, new AccessCode("GREEN", Color.green, false) },
        { AccessCodes.Blue, new AccessCode("BLUE", Color.blue, false) },
        { AccessCodes.Yellow, new AccessCode("YELLOW", Color.yellow, false) },
    };

    [Header("Debug Access Code Toggles")]
    [SerializeField] bool isDebug = false;
    [SerializeField] bool red = false;
    [SerializeField] bool green = false;
    [SerializeField] bool blue = false;
    [SerializeField] bool yellow = false;

	private void Start()
	{
        if (!isDebug) return;

        accessCodes.TryGetValue(AccessCodes.Red, out AccessCode code);
        code.Access = red;
        accessCodes.TryGetValue(AccessCodes.Green, out code);
        code.Access = green;
        accessCodes.TryGetValue(AccessCodes.Blue, out code);
        code.Access = blue;
        accessCodes.TryGetValue(AccessCodes.Yellow, out code);
        code.Access = yellow;
    }

	public bool CheckAccess(AccessCodes checkCode)
	{
        accessCodes.TryGetValue(checkCode, out AccessCode result);

        return result.Access;
	}

    public void ModifyAccess(AccessCodes modifyCode, bool access)
	{
        accessCodes.TryGetValue(modifyCode, out AccessCode result);

        result.Access = access;
	}
}

public class AccessCode
{
    public string Name { get; }
    public Color Color { get; }
    public bool Access { get; set; }
    public AccessCode() { }
    public AccessCode(string name, Color color, bool access)
	{
        Name = name;
        Color = color;
        Access = access;
	}
}
