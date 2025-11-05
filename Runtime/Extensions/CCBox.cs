using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IOToolkit;

public static class CCBox
{
    public enum InputType
    {
        // 按钮
        Button = 1,

        // 摇杆
        Joystick = 2,
    }

    // 中控交互埋点
    public static void Interact(InputType hType = InputType.Button, int hId = 0)
    {
        Key BtnKey = "OAxis_" + hId.ToString("00");
        IODeviceController.GetIODevice("CCBOX").SetDO(BtnKey, (int)hType);
    }

    public static void Button(int BtnID)
    {
        Interact(InputType.Button, BtnID);
    }

    public static void Joystick(int JoystickID)
    {
        Interact(InputType.Joystick, JoystickID);
    }
}
