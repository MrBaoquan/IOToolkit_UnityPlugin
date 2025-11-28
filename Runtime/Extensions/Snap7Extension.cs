namespace IOToolkit.Snap7
{
    public static class Snap7Extension
    {
        // 通道240-249用于动态写入任意地址
        static Key areaCodeKey = "OAxis_240"; // 区域码: 1=DB, 2=PA, 3=PE, 4=MK
        static Key param1Key = "OAxis_241"; // DB区=DB号, 其他区=地址
        static Key param2Key = "OAxis_242"; // DB区=地址, 其他区=功能码
        static Key param3Key = "OAxis_243"; // DB区=功能码, 其他区=数据开始
        static Key param4Key = "OAxis_244"; // 数据字节1
        static Key param5Key = "OAxis_245"; // 数据字节2

        // ========== DB区写入方法 ==========

        // 写入DB块字节 (DB{dbNumber}.DBB{address} = value)
        public static void WriteDBByte(
            this IODevice snap7Device,
            int dbNumber,
            int address,
            byte value
        )
        {
            snap7Device.SetDO(areaCodeKey, 1);
            snap7Device.SetDO(param1Key, dbNumber);
            snap7Device.SetDO(param2Key, address);
            snap7Device.SetDO(param3Key, 1); // 功能码: 写字节
            snap7Device.SetDO(param4Key, value);
            snap7Device.DOImmediate();
        }

        // 写入DB块字 (DB{dbNumber}.DBW{address} = value)
        public static void WriteDBWord(
            this IODevice snap7Device,
            int dbNumber,
            int address,
            ushort value
        )
        {
            snap7Device.SetDO(areaCodeKey, 1);
            snap7Device.SetDO(param1Key, dbNumber);
            snap7Device.SetDO(param2Key, address);
            snap7Device.SetDO(param3Key, 2); // 功能码: 写字
            snap7Device.SetDO(param4Key, value);
            snap7Device.DOImmediate();
        }

        // 写入DB块双字 (DB{dbNumber}.DBD{address} = value)
        public static void WriteDBDWord(
            this IODevice snap7Device,
            int dbNumber,
            int address,
            uint value
        )
        {
            snap7Device.SetDO(areaCodeKey, 1);
            snap7Device.SetDO(param1Key, dbNumber);
            snap7Device.SetDO(param2Key, address);
            snap7Device.SetDO(param3Key, 3); // 功能码: 写双字
            snap7Device.SetDO(param4Key, (value >> 16) & 0xFFFF);
            snap7Device.SetDO(param5Key, value & 0xFFFF);
            snap7Device.DOImmediate();
        }

        // 写入DB块位 (DB{dbNumber}.DBX{address}.{bitOffset} = value)
        public static void WriteDBBit(
            this IODevice snap7Device,
            int dbNumber,
            int address,
            int bitOffset,
            bool value
        )
        {
            snap7Device.SetDO(areaCodeKey, 1);
            snap7Device.SetDO(param1Key, dbNumber);
            snap7Device.SetDO(param2Key, address);
            snap7Device.SetDO(param3Key, 4); // 功能码: 写位
            snap7Device.SetDO(param4Key, bitOffset);
            snap7Device.SetDO(param5Key, value ? 1 : 0);
            snap7Device.DOImmediate();
        }

        // ========== PA区写入方法 (物理输出) ==========

        // 写入物理输出字节 (QB{address} = value)
        public static void WriteOutputByte(this IODevice snap7Device, int address, byte value)
        {
            snap7Device.SetDO(areaCodeKey, 2);
            snap7Device.SetDO(param1Key, address);
            snap7Device.SetDO(param2Key, 1); // 功能码: 写字节
            snap7Device.SetDO(param3Key, value);
            snap7Device.DOImmediate();
        }

        // 写入物理输出字 (QW{address} = value)
        public static void WriteOutputWord(this IODevice snap7Device, int address, ushort value)
        {
            snap7Device.SetDO(areaCodeKey, 2);
            snap7Device.SetDO(param1Key, address);
            snap7Device.SetDO(param2Key, 2); // 功能码: 写字
            snap7Device.SetDO(param3Key, value);
            snap7Device.DOImmediate();
        }

        // 写入物理输出双字 (QD{address} = value)
        public static void WriteOutputDWord(this IODevice snap7Device, int address, uint value)
        {
            snap7Device.SetDO(areaCodeKey, 2);
            snap7Device.SetDO(param1Key, address);
            snap7Device.SetDO(param2Key, 3); // 功能码: 写双字
            snap7Device.SetDO(param3Key, (value >> 16) & 0xFFFF);
            snap7Device.SetDO(param4Key, value & 0xFFFF);
            snap7Device.DOImmediate();
        }

        // 写入物理输出位 (Q{address}.{bitOffset} = value)
        public static void WriteOutputBit(
            this IODevice snap7Device,
            int address,
            int bitOffset,
            bool value
        )
        {
            snap7Device.SetDO(areaCodeKey, 2);
            snap7Device.SetDO(param1Key, address);
            snap7Device.SetDO(param2Key, 4); // 功能码: 写位
            snap7Device.SetDO(param3Key, bitOffset);
            snap7Device.SetDO(param4Key, value ? 1 : 0);
            snap7Device.DOImmediate();
        }

        // ========== PE区写入方法 (物理输入) ==========

        // 写入物理输入字节 (IB{address} = value) - 仅用于测试
        public static void WriteInputByte(this IODevice snap7Device, int address, byte value)
        {
            snap7Device.SetDO(areaCodeKey, 3);
            snap7Device.SetDO(param1Key, address);
            snap7Device.SetDO(param2Key, 1); // 功能码: 写字节
            snap7Device.SetDO(param3Key, value);
            snap7Device.DOImmediate();
        }

        // 写入物理输入字 (IW{address} = value) - 仅用于测试
        public static void WriteInputWord(this IODevice snap7Device, int address, ushort value)
        {
            snap7Device.SetDO(areaCodeKey, 3);
            snap7Device.SetDO(param1Key, address);
            snap7Device.SetDO(param2Key, 2); // 功能码: 写字
            snap7Device.SetDO(param3Key, value);
            snap7Device.DOImmediate();
        }

        // 写入物理输入位 (I{address}.{bitOffset} = value) - 仅用于测试
        public static void WriteInputBit(
            this IODevice snap7Device,
            int address,
            int bitOffset,
            bool value
        )
        {
            snap7Device.SetDO(areaCodeKey, 3);
            snap7Device.SetDO(param1Key, address);
            snap7Device.SetDO(param2Key, 4); // 功能码: 写位
            snap7Device.SetDO(param3Key, bitOffset);
            snap7Device.SetDO(param4Key, value ? 1 : 0);
            snap7Device.DOImmediate();
        }

        // ========== MK区写入方法 (标志位) ==========

        // 写入标志位字节 (MB{address} = value)
        public static void WriteMarkerByte(this IODevice snap7Device, int address, byte value)
        {
            snap7Device.SetDO(areaCodeKey, 4);
            snap7Device.SetDO(param1Key, address);
            snap7Device.SetDO(param2Key, 1); // 功能码: 写字节
            snap7Device.SetDO(param3Key, value);
            snap7Device.DOImmediate();
        }

        // 写入标志位字 (MW{address} = value)
        public static void WriteMarkerWord(this IODevice snap7Device, int address, ushort value)
        {
            snap7Device.SetDO(areaCodeKey, 4);
            snap7Device.SetDO(param1Key, address);
            snap7Device.SetDO(param2Key, 2); // 功能码: 写字
            snap7Device.SetDO(param3Key, value);
            snap7Device.DOImmediate();
        }

        // 写入标志位双字 (MD{address} = value)
        public static void WriteMarkerDWord(this IODevice snap7Device, int address, uint value)
        {
            snap7Device.SetDO(areaCodeKey, 4);
            snap7Device.SetDO(param1Key, address);
            snap7Device.SetDO(param2Key, 3); // 功能码: 写双字
            snap7Device.SetDO(param3Key, (value >> 16) & 0xFFFF);
            snap7Device.SetDO(param4Key, value & 0xFFFF);
            snap7Device.DOImmediate();
        }

        // 写入标志位位 (M{address}.{bitOffset} = value)
        public static void WriteMarkerBit(
            this IODevice snap7Device,
            int address,
            int bitOffset,
            bool value
        )
        {
            snap7Device.SetDO(areaCodeKey, 4);
            snap7Device.SetDO(param1Key, address);
            snap7Device.SetDO(param2Key, 4); // 功能码: 写位
            snap7Device.SetDO(param3Key, bitOffset);
            snap7Device.SetDO(param4Key, value ? 1 : 0);
            snap7Device.DOImmediate();
        }

        // ========== S7-200 V区便捷方法 ==========

        // 写入S7-200 V区字节 (VB{address} = value, 通过DB1访问)
        public static void WriteVByte(this IODevice snap7Device, int address, byte value)
        {
            snap7Device.WriteDBByte(1, address, value);
        }

        // 写入S7-200 V区字 (VW{address} = value, 通过DB1访问)
        public static void WriteVWord(this IODevice snap7Device, int address, ushort value)
        {
            snap7Device.WriteDBWord(1, address, value);
        }

        // 写入S7-200 V区双字 (VD{address} = value, 通过DB1访问)
        public static void WriteVDWord(this IODevice snap7Device, int address, uint value)
        {
            snap7Device.WriteDBDWord(1, address, value);
        }

        // 写入S7-200 V区位 (V{address}.{bitOffset} = value, 通过DB1访问)
        public static void WriteVBit(
            this IODevice snap7Device,
            int address,
            int bitOffset,
            bool value
        )
        {
            snap7Device.WriteDBBit(1, address, bitOffset, value);
        }
    }
}
