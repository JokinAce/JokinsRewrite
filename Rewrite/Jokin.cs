using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class Rewrite
{
    public static VAMemory vam;

    public Rewrite(string processss)
    {
        vam = new VAMemory(processss);
    }

    [DllImport("user32.dll")]
    static extern ushort GetAsyncKeyState(int vKey);

    public bool IsKeyPushedDown(Keys vKey)
    {
        return 0 != (GetAsyncKeyState((int)vKey) & 0x8000);
    }

    public int MultiPointer(long BaseAddr, int BaseOffset, int[] Pointers)
    {
        int Current = ReadInt((int)(BaseAddr + BaseOffset));

        if (Pointers != null)
        {
            for (int i = 0; i < Pointers.Length - 1; i++)
            {
                Current = ReadInt(Current + Pointers[i]);
            }
        }
        return Current + Pointers[Pointers.Length - 1];
    }

    public long GetDLL(string name)
    {
        long DLL = 0;
        if (CheckProcess())
        {
            Process[] p = Process.GetProcessesByName(vam.processName);
            foreach (ProcessModule m in p[0].Modules)
            {
                if (m.ModuleName == name)
                {
                    DLL = (long)m.BaseAddress;
                    return DLL;
                }
            }
        }
        return DLL;
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