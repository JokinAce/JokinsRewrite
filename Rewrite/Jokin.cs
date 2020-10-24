using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Rewrite
{
    public static VAMemory vam;

    public Rewrite(string processss)
    {
        vam = new VAMemory(processss);
    }

    public long BaseAddr()
    {
        return vam.getBaseAddress;
    }

    public bool CheckProcess()
    {
        return vam.CheckProcess();
    }

    public void WriteInt(int offsets, int value)
    {
        vam.WriteInt32((IntPtr)offsets, value);
    }

    public int ReadInt(int offsets)
    {
        return vam.ReadInt32((IntPtr)offsets);
    }

    public void WriteString(int offsets, string value)
    {
        vam.WriteStringASCII((IntPtr)offsets, value);
    }

    public void WriteShort(int offsets, short value)
    {
        vam.WriteShort((IntPtr)offsets, value);
    }

    public short ReadShort(int offsets)
    {
        return vam.ReadShort((IntPtr)offsets);
    }

    public void WriteFloat(int offsets, float value)
    {
        vam.WriteFloat((IntPtr)offsets, value);
    }

    public float ReadFloat(int offsets)
    {
        return vam.ReadFloat((IntPtr)offsets);
    }

    public bool ReadBoolean(int offsets)
    {
        return vam.ReadBoolean((IntPtr)offsets);
    }

    public void WriteBoolean(int offsets, bool value)
    {
        vam.WriteBoolean((IntPtr)offsets, value);
    }
}