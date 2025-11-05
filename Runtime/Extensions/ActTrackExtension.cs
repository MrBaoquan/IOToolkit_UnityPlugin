namespace IOToolkit.ActTrack
{
    public enum InputType
    {
        Button = 1, // 按钮
        Handle, // 手柄
        Joystick, // 摇杆
        MotorStart, // 电机启动
        Lever, // 拉杆
        Knob, // 旋钮
        Sensor, // 传感器
        Handwheel, // 手轮
        Camera, // 摄像头
        Radar, // 雷达
        Touch, // 触摸键
        Key, // 按键
        RFID, // RFID
        Turntable, // 转盘
        Slider, // 滑杆
        Kinect // 体感器
    }

    // IOToolkit 数据采集设备拓展
    public static class ActTrackExtension
    {
        // 开始互动
        public static void NotifyStart(this IODevice actTrackDevice)
        {
            actTrackDevice.SetDO(IOKeyCode.OAxis_00, 1);
        }

        // 互动结束
        public static void NotifyEnd(this IODevice actTrackDevice)
        {
            actTrackDevice.SetDO(IOKeyCode.OAxis_00, 0);
        }

        /// <summary>
        /// 按钮互动
        /// </summary>
        /// <param name="actTrackDevice"></param>
        /// <param name="buttonID"></param>
        public static void NotifyButton(this IODevice actTrackDevice, int buttonID = 1)
        {
            actTrackDevice.NotifyInput(InputType.Button, buttonID);
        }

        // IO设备按钮
        public static void NotifyInput(
            this IODevice actTrackDevice,
            InputType inputType,
            int buttonID = 1
        )
        {
            Key _inputKey = "OAxis_" + ((int)inputType).ToString("00");
            actTrackDevice.SetDO(_inputKey, buttonID);
        }

        // 报警接口
        public static void NotifyAlarm(this IODevice actTrackDevice, int alarmID)
        {
            actTrackDevice.SetDO(IOKeyCode.OAxis_250, alarmID);
        }
    }
}
