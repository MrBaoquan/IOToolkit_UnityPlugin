using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using IOToolkit.Extension;

namespace IOToolkit.NetIO
{
    /// <summary>
    ///
    /// NetIO 设备输出通道 240 - 255 为保留位
    /// NetIO 设备创建成功后 每5秒会自动广播一次自己的ip相关信息 广播端口为21000
    /// 240 表示输出数据使用方式    0 作为DI和AD共用    1 作为DI使用    2 作为AD使用   10 清空所有 11 清空DI   12 清空AD
    /// 241-244 表示输出目标UDP地址的各个字节
    /// 245 表示输出目标UDP端口
    /// 246 标识   247 通道号  248 输出值
    /// </summary>

    public struct NetIOKeyCode
    {
        public static readonly Key Ctrl = IOKeyCode.Button_00;
        public static readonly Key Home = IOKeyCode.Button_01;
        public static readonly Key Menu = IOKeyCode.Button_02;
        public static readonly Key Up = IOKeyCode.Button_03;
        public static readonly Key Down = IOKeyCode.Button_04;
        public static readonly Key Left = IOKeyCode.Button_05;
        public static readonly Key Right = IOKeyCode.Button_06;
        public static readonly Key Confirm = IOKeyCode.Button_07;
        public static readonly Key Back = IOKeyCode.Button_08;
        public static readonly Key VolumeUp = IOKeyCode.Button_09;
        public static readonly Key VolumeDown = IOKeyCode.Button_10;
        public static readonly Key Mute = IOKeyCode.Button_11;
        public static readonly Key Loop = IOKeyCode.Button_15;
        public static readonly Key Play = IOKeyCode.Button_16;
        public static readonly Key Pause = IOKeyCode.Button_17;
    }

    public enum EventMode
    {
        SetAll = 0,
        SetDI = 1,
        SetAD = 2,
        ZeroAll = 10,
        ZeroDI = 11,
        ZeroAD = 12
    }

    public static class NetIOExtension
    {
        static Key chFunc = "OAxis_240";
        static Key chPort = "OAxis_245";

        static Key chSetKeyFlag = "OAxis_246";
        static Key chChannel = "OAxis_247";
        static Key chValue = "OAxis_248";

        /// <summary>
        /// 设置NetIO设备的IP地址和端口
        /// </summary>
        /// <param name="netDev"></param>
        /// <param name="ip"></param>
        /// <param name="port">端口地址为IODevice 配置文件 Index + 20000</param>
        public static void SetRemoteAddress(this IODevice netDev, string ip, int port)
        {
            if (!IPAddress.TryParse(ip, out IPAddress ipAddress))
            {
                UnityEngine.Debug.LogWarning("Invalid IP address: " + ip);
                return;
            }

            var _bytes = ipAddress.GetAddressBytes();
            Enumerable
                .Range(0, 4)
                .ToList()
                .ForEach(i => netDev.SetDO((Key)$"OAxis_{241 + i}", _bytes[i]));
            netDev.SetDO(chPort, port);
        }

        public static void SetEventMode(this IODevice netDev, EventMode func)
        {
            netDev.SetDO(chFunc, (int)func);
        }

        public static EventMode GetEventMode(this IODevice netDev)
        {
            return (EventMode)netDev.GetDO(chFunc);
        }

        // DI通道置零
        public static void ZeroRemoteDI(this IODevice netDev)
        {
            netDev.SetEventMode(EventMode.ZeroDI);
            netDev.DOImmediate();
        }

        // AD通道置零
        public static void ZeroRemoteAD(this IODevice netDev)
        {
            netDev.SetEventMode(EventMode.ZeroAD);
            netDev.DOImmediate();
        }

        // 转发DI事件
        public static void PropagateDIEvents(
            this IODevice netDevice,
            IODevice extDevice,
            int startChannel,
            int count
        )
        {
            Enumerable
                .Range(startChannel, count)
                .ToList()
                .ForEach(_idx =>
                {
                    Key outputKey = $"OAxis_{_idx:D2}";
                    Key inputKey = $"Button_{_idx:D2}";

                    extDevice.BindKey(
                        inputKey,
                        InputEvent.IE_Pressed,
                        () => netDevice.SetRemoteKeyDown(outputKey)
                    );

                    extDevice.BindKey(
                        inputKey,
                        InputEvent.IE_Released,
                        () => netDevice.SetRemoteKeyUp(outputKey)
                    );
                });
        }

        public static void EmitNetEvent(this IODevice netDev, Key evtKey)
        {
            netDev.SetEventMode(EventMode.SetDI);
            netDev.SetRemoteChannel(evtKey, 1);
            netDev.DOImmediate();
            netDev.SetRemoteChannel(evtKey, 0);
            netDev.DOImmediate();
        }

        public static void SetRemoteKeyDown(this IODevice netDev, Key button)
        {
            netDev.SetEventMode(EventMode.SetDI);
            netDev.SetRemoteChannel(button, 1);
            netDev.DOImmediate();
        }

        public static void SetRemoteKeyUp(this IODevice netDev, Key button)
        {
            netDev.SetEventMode(EventMode.SetDI);
            netDev.SetRemoteChannel(button, 0);
            netDev.DOImmediate();
        }

        public static void SetRemoteAxis(this IODevice netDev, Key axisKey, float value)
        {
            var _doVal = value * 1000;
            netDev.SetEventMode(EventMode.SetAD);
            netDev.SetRemoteChannel(axisKey, _doVal);
            netDev.DOImmediate();
        }

        private static void SetRemoteChannel(this IODevice netDev, Key chKey, float val)
        {
            netDev.SetDO(chSetKeyFlag, 1);
            netDev.SetDO(chChannel, chKey.GetIntValue());
            netDev.SetDO(chValue, val);
        }
    }
}
