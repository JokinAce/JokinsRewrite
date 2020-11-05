using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

// Token: 0x02000002 RID: 2
public class VAMemory
{
    // Token: 0x06000001 RID: 1
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, uint lpNumberOfBytesRead);

    // Token: 0x06000002 RID: 2
    [DllImport("kernel32.dll")]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, uint lpNumberOfBytesWritten);

    // Token: 0x06000003 RID: 3
    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    // Token: 0x06000004 RID: 4
    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr hObject);

    // Token: 0x06000005 RID: 5
    [DllImport("kernel32.dll")]
    private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

    // Token: 0x06000006 RID: 6
    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    // Token: 0x17000001 RID: 1
    // (get) Token: 0x06000007 RID: 7 RVA: 0x00002050 File Offset: 0x00000250
    // (set) Token: 0x06000008 RID: 8 RVA: 0x00002058 File Offset: 0x00000258
    public string processName { get; set; }

    // Token: 0x17000002 RID: 2
    // (get) Token: 0x06000009 RID: 9 RVA: 0x00002061 File Offset: 0x00000261
    public long getBaseAddress
    {
        get
        {
            this.baseAddress = (IntPtr)0;
            this.processModule = this.mainProcess[0].MainModule;
            this.baseAddress = this.processModule.BaseAddress;
            return (long)this.baseAddress;
        }
    }

    // Token: 0x0600000B RID: 11 RVA: 0x000020A6 File Offset: 0x000002A6
    public VAMemory(string pProcessName)
    {
        this.processName = pProcessName;
    }

    // Token: 0x0600000C RID: 12 RVA: 0x000020B8 File Offset: 0x000002B8
    public bool CheckProcess()
    {
        if (this.processName == null)
        {
            Console.WriteLine("Programmer, define process name first!");
            return false;
        }
        this.mainProcess = Process.GetProcessesByName(this.processName);
        if (this.mainProcess.Length == 0)
        {
            this.ErrorProcessNotFound(this.processName);
            return false;
        }
        this.processHandle = VAMemory.OpenProcess(2035711U, false, this.mainProcess[0].Id);
        if (this.processHandle == IntPtr.Zero)
        {
            this.ErrorProcessNotFound(this.processName);
            return false;
        }
        return true;
    }

    // Token: 0x0600000D RID: 13 RVA: 0x00002144 File Offset: 0x00000344
    public byte[] ReadByteArray(IntPtr pOffset, uint pSize)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        byte[] result;
        try
        {
            uint flNewProtect;
            VAMemory.VirtualProtectEx(this.processHandle, pOffset, (UIntPtr)pSize, 4U, out flNewProtect);
            byte[] array = new byte[pSize];
            VAMemory.ReadProcessMemory(this.processHandle, pOffset, array, pSize, 0U);
            VAMemory.VirtualProtectEx(this.processHandle, pOffset, (UIntPtr)pSize, flNewProtect, out flNewProtect);
            result = array;
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadByteArray" + ex.ToString());
            }
            result = new byte[1];
        }
        return result;
    }

    // Token: 0x0600000E RID: 14 RVA: 0x000021EC File Offset: 0x000003EC
    public string ReadStringUnicode(IntPtr pOffset, uint pSize)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        string result;
        try
        {
            result = Encoding.Unicode.GetString(this.ReadByteArray(pOffset, pSize), 0, (int)pSize);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadStringUnicode" + ex.ToString());
            }
            result = "";
        }
        return result;
    }

    // Token: 0x0600000F RID: 15 RVA: 0x00002260 File Offset: 0x00000460
    public string ReadStringASCII(IntPtr pOffset, uint pSize)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        string result;
        try
        {
            result = Encoding.ASCII.GetString(this.ReadByteArray(pOffset, pSize), 0, (int)pSize);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadStringASCII" + ex.ToString());
            }
            result = "";
        }
        return result;
    }

    // Token: 0x06000010 RID: 16 RVA: 0x000022D4 File Offset: 0x000004D4
    public char ReadChar(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        char result;
        try
        {
            result = BitConverter.ToChar(this.ReadByteArray(pOffset, 1U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadChar" + ex.ToString());
            }
            result = ' ';
        }
        return result;
    }

    // Token: 0x06000011 RID: 17 RVA: 0x00002340 File Offset: 0x00000540
    public bool ReadBoolean(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = BitConverter.ToBoolean(this.ReadByteArray(pOffset, 1U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadByte" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000012 RID: 18 RVA: 0x000023AC File Offset: 0x000005AC
    public byte ReadByte(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        byte result;
        try
        {
            result = this.ReadByteArray(pOffset, 1U)[0];
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadByte" + ex.ToString());
            }
            result = 0;
        }
        return result;
    }

    // Token: 0x06000013 RID: 19 RVA: 0x00002414 File Offset: 0x00000614
    public short ReadInt16(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        short result;
        try
        {
            result = BitConverter.ToInt16(this.ReadByteArray(pOffset, 2U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadInt16" + ex.ToString());
            }
            result = 0;
        }
        return result;
    }

    // Token: 0x06000014 RID: 20 RVA: 0x00002480 File Offset: 0x00000680
    public short ReadShort(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        short result;
        try
        {
            result = BitConverter.ToInt16(this.ReadByteArray(pOffset, 2U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadInt16" + ex.ToString());
            }
            result = 0;
        }
        return result;
    }

    // Token: 0x06000015 RID: 21 RVA: 0x000024EC File Offset: 0x000006EC
    public int ReadInt32(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        int result;
        try
        {
            result = BitConverter.ToInt32(this.ReadByteArray(pOffset, 4U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadInt32" + ex.ToString());
            }
            result = 0;
        }
        return result;
    }

    // Token: 0x06000016 RID: 22 RVA: 0x00002558 File Offset: 0x00000758
    public int ReadInteger(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        int result;
        try
        {
            result = BitConverter.ToInt32(this.ReadByteArray(pOffset, 4U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadInteger" + ex.ToString());
            }
            result = 0;
        }
        return result;
    }

    // Token: 0x06000017 RID: 23 RVA: 0x000025C4 File Offset: 0x000007C4
    public long ReadInt64(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        long result;
        try
        {
            result = BitConverter.ToInt64(this.ReadByteArray(pOffset, 8U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadInt64" + ex.ToString());
            }
            result = 0L;
        }
        return result;
    }

    // Token: 0x06000018 RID: 24 RVA: 0x00002630 File Offset: 0x00000830
    public long ReadLong(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        long result;
        try
        {
            result = BitConverter.ToInt64(this.ReadByteArray(pOffset, 8U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadLong" + ex.ToString());
            }
            result = 0L;
        }
        return result;
    }

    // Token: 0x06000019 RID: 25 RVA: 0x0000269C File Offset: 0x0000089C
    public ushort ReadUInt16(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        ushort result;
        try
        {
            result = BitConverter.ToUInt16(this.ReadByteArray(pOffset, 2U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadUInt16" + ex.ToString());
            }
            result = 0;
        }
        return result;
    }

    // Token: 0x0600001A RID: 26 RVA: 0x00002708 File Offset: 0x00000908
    public ushort ReadUShort(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        ushort result;
        try
        {
            result = BitConverter.ToUInt16(this.ReadByteArray(pOffset, 2U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadUShort" + ex.ToString());
            }
            result = 0;
        }
        return result;
    }

    // Token: 0x0600001B RID: 27 RVA: 0x00002774 File Offset: 0x00000974
    public uint ReadUInt32(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        uint result;
        try
        {
            result = BitConverter.ToUInt32(this.ReadByteArray(pOffset, 4U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadUInt32" + ex.ToString());
            }
            result = 0U;
        }
        return result;
    }

    // Token: 0x0600001C RID: 28 RVA: 0x000027E0 File Offset: 0x000009E0
    public uint ReadUInteger(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        uint result;
        try
        {
            result = BitConverter.ToUInt32(this.ReadByteArray(pOffset, 4U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadUInteger" + ex.ToString());
            }
            result = 0U;
        }
        return result;
    }

    // Token: 0x0600001D RID: 29 RVA: 0x0000284C File Offset: 0x00000A4C
    public ulong ReadUInt64(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        ulong result;
        try
        {
            result = BitConverter.ToUInt64(this.ReadByteArray(pOffset, 8U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadUInt64" + ex.ToString());
            }
            result = 0UL;
        }
        return result;
    }

    // Token: 0x0600001E RID: 30 RVA: 0x000028B8 File Offset: 0x00000AB8
    public long ReadULong(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        long result;
        try
        {
            result = (long)BitConverter.ToUInt64(this.ReadByteArray(pOffset, 8U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadULong" + ex.ToString());
            }
            result = 0L;
        }
        return result;
    }

    // Token: 0x0600001F RID: 31 RVA: 0x00002924 File Offset: 0x00000B24
    public float ReadFloat(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        float result;
        try
        {
            result = BitConverter.ToSingle(this.ReadByteArray(pOffset, 4U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadFloat" + ex.ToString());
            }
            result = 0f;
        }
        return result;
    }

    // Token: 0x06000020 RID: 32 RVA: 0x00002994 File Offset: 0x00000B94
    public double ReadDouble(IntPtr pOffset)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        double result;
        try
        {
            result = BitConverter.ToDouble(this.ReadByteArray(pOffset, 8U), 0);
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: ReadDouble" + ex.ToString());
            }
            result = 0.0;
        }
        return result;
    }

    // Token: 0x06000021 RID: 33 RVA: 0x00002A08 File Offset: 0x00000C08
    public bool WriteByteArray(IntPtr pOffset, byte[] pBytes)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            uint flNewProtect;
            VAMemory.VirtualProtectEx(this.processHandle, pOffset, (UIntPtr)((ulong)((long)pBytes.Length)), 4U, out flNewProtect);
            bool flag = VAMemory.WriteProcessMemory(this.processHandle, pOffset, pBytes, (uint)pBytes.Length, 0U);
            VAMemory.VirtualProtectEx(this.processHandle, pOffset, (UIntPtr)((ulong)((long)pBytes.Length)), flNewProtect, out flNewProtect);
            result = flag;
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteByteArray" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000022 RID: 34 RVA: 0x00002AA8 File Offset: 0x00000CA8
    public bool WriteStringUnicode(IntPtr pOffset, string pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, Encoding.Unicode.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteStringUnicode" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000023 RID: 35 RVA: 0x00002B18 File Offset: 0x00000D18
    public bool WriteStringASCII(IntPtr pOffset, string pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, Encoding.ASCII.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteStringASCII" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000024 RID: 36 RVA: 0x00002B88 File Offset: 0x00000D88
    public bool WriteBoolean(IntPtr pOffset, bool pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteBoolean" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000025 RID: 37 RVA: 0x00002BF0 File Offset: 0x00000DF0
    public bool WriteChar(IntPtr pOffset, char pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteChar" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000026 RID: 38 RVA: 0x00002C58 File Offset: 0x00000E58
    public bool WriteByte(IntPtr pOffset, byte pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes((short)pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteByte" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000027 RID: 39 RVA: 0x00002CC0 File Offset: 0x00000EC0
    public bool WriteInt16(IntPtr pOffset, short pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteInt16" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000028 RID: 40 RVA: 0x00002D28 File Offset: 0x00000F28
    public bool WriteShort(IntPtr pOffset, short pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteShort" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000029 RID: 41 RVA: 0x00002D90 File Offset: 0x00000F90
    public bool WriteInt32(IntPtr pOffset, int pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteInt32" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x0600002A RID: 42 RVA: 0x00002DF8 File Offset: 0x00000FF8
    public bool WriteInteger(IntPtr pOffset, int pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteInt" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x0600002B RID: 43 RVA: 0x00002E60 File Offset: 0x00001060
    public bool WriteInt64(IntPtr pOffset, long pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteInt64" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x0600002C RID: 44 RVA: 0x00002EC8 File Offset: 0x000010C8
    public bool WriteLong(IntPtr pOffset, long pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteLong" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x0600002D RID: 45 RVA: 0x00002F30 File Offset: 0x00001130
    public bool WriteUInt16(IntPtr pOffset, ushort pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteUInt16" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x0600002E RID: 46 RVA: 0x00002F98 File Offset: 0x00001198
    public bool WriteUShort(IntPtr pOffset, ushort pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteShort" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x0600002F RID: 47 RVA: 0x00003000 File Offset: 0x00001200
    public bool WriteUInt32(IntPtr pOffset, uint pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteUInt32" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000030 RID: 48 RVA: 0x00003068 File Offset: 0x00001268
    public bool WriteUInteger(IntPtr pOffset, uint pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteUInt" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000031 RID: 49 RVA: 0x000030D0 File Offset: 0x000012D0
    public bool WriteUInt64(IntPtr pOffset, ulong pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteUInt64" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000032 RID: 50 RVA: 0x00003138 File Offset: 0x00001338
    public bool WriteULong(IntPtr pOffset, ulong pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteULong" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000033 RID: 51 RVA: 0x000031A0 File Offset: 0x000013A0
    public bool WriteFloat(IntPtr pOffset, float pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteFloat" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000034 RID: 52 RVA: 0x00003208 File Offset: 0x00001408
    public bool WriteDouble(IntPtr pOffset, double pData)
    {
        if (this.processHandle == IntPtr.Zero)
        {
            this.CheckProcess();
        }
        bool result;
        try
        {
            result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
        }
        catch (Exception ex)
        {
            if (VAMemory.debugMode)
            {
                Console.WriteLine("Error: WriteDouble" + ex.ToString());
            }
            result = false;
        }
        return result;
    }

    // Token: 0x06000035 RID: 53 RVA: 0x00003270 File Offset: 0x00001470
    private void ErrorProcessNotFound(string pProcessName)
    {
        if (Application.MessageLoop)
        {
            // WinForms app
            MessageBox.Show(this.processName + " is not running or has not been found. Please check and try again", "Process Not Found", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
        else
        {
            // Console app
            Console.WriteLine(this.processName + " is not running or has not been found. Please check and try again");
        }
    }

    // Token: 0x04000001 RID: 1
    public static bool debugMode;

    // Token: 0x04000002 RID: 2
    private IntPtr baseAddress;

    // Token: 0x04000003 RID: 3
    private ProcessModule processModule;

    // Token: 0x04000004 RID: 4
    private Process[] mainProcess;

    // Token: 0x04000005 RID: 5
    private IntPtr processHandle;

    // Token: 0x02000003 RID: 3
    [Flags]
    private enum ProcessAccessFlags : uint
    {
        // Token: 0x04000008 RID: 8
        All = 2035711U,
        // Token: 0x04000009 RID: 9
        Terminate = 1U,
        // Token: 0x0400000A RID: 10
        CreateThread = 2U,
        // Token: 0x0400000B RID: 11
        VMOperation = 8U,
        // Token: 0x0400000C RID: 12
        VMRead = 16U,
        // Token: 0x0400000D RID: 13
        VMWrite = 32U,
        // Token: 0x0400000E RID: 14
        DupHandle = 64U,
        // Token: 0x0400000F RID: 15
        SetInformation = 512U,
        // Token: 0x04000010 RID: 16
        QueryInformation = 1024U,
        // Token: 0x04000011 RID: 17
        Synchronize = 1048576U
    }

    // Token: 0x02000004 RID: 4
    private enum VirtualMemoryProtection : uint
    {
        // Token: 0x04000013 RID: 19
        PAGE_NOACCESS = 1U,
        // Token: 0x04000014 RID: 20
        PAGE_READONLY,
        // Token: 0x04000015 RID: 21
        PAGE_READWRITE = 4U,
        // Token: 0x04000016 RID: 22
        PAGE_WRITECOPY = 8U,
        // Token: 0x04000017 RID: 23
        PAGE_EXECUTE = 16U,
        // Token: 0x04000018 RID: 24
        PAGE_EXECUTE_READ = 32U,
        // Token: 0x04000019 RID: 25
        PAGE_EXECUTE_READWRITE = 64U,
        // Token: 0x0400001A RID: 26
        PAGE_EXECUTE_WRITECOPY = 128U,
        // Token: 0x0400001B RID: 27
        PAGE_GUARD = 256U,
        // Token: 0x0400001C RID: 28
        PAGE_NOCACHE = 512U,
        // Token: 0x0400001D RID: 29
        PROCESS_ALL_ACCESS = 2035711U
    }
}
