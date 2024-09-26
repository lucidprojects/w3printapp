/** 
* Code snippets from: 
* http://support.microsoft.com/default.aspx/kb/322091
* http://social.msdn.microsoft.com/forums/en-US/winforms/thread/776e44c2-4ade-4a2e-b347-66e9b06a706e/
*
* Other links:
 * http://support.microsoft.com/kb/160129/EN-US/
*/

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Generic;

namespace PrintInvoice
{
  class RawPrinterHelper
  {
    // Structure and API declarions
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class DOCINFOA
    {
      [MarshalAs(UnmanagedType.LPStr)]
      public string pDocName;
      [MarshalAs(UnmanagedType.LPStr)]
      public string pOutputFile;
      [MarshalAs(UnmanagedType.LPStr)]
      public string pDataType;
    }

    [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

    [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern uint StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

    [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

    [DllImport("winspool.Drv", EntryPoint = "GetPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern bool GetPrinter(IntPtr hPrinter, uint dwLevel, IntPtr pPrinter, uint dwBuf, ref uint dwNeeded);

    [DllImport("Winspool.drv", SetLastError = true, EntryPoint = "EnumJobsA", CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern bool EnumJobs(IntPtr hPrinter, UInt32 FirstJob, UInt32 NoJobs, UInt32 Level, IntPtr pJob, UInt32 cbBuf, out UInt32 pcbNeeded, out UInt32 pcReturned);

    [DllImport("Winspool.drv", SetLastError = true, EntryPoint = "GetJobA", CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern bool GetJob(IntPtr hPrinter, uint JobId, uint Level, IntPtr pJob, uint cbBuf, out uint pcbNeeded);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PRINTER_INFO_2
    {
      [MarshalAs(UnmanagedType.LPTStr)]
      public string pServerName;
      [MarshalAs(UnmanagedType.LPTStr)]
      public string pPrinterName;
      [MarshalAs(UnmanagedType.LPTStr)]
      public string pShareName;
      [MarshalAs(UnmanagedType.LPTStr)]
      public string pPortName;
      [MarshalAs(UnmanagedType.LPTStr)]
      public string pDriverName;
      [MarshalAs(UnmanagedType.LPTStr)]
      public string pComment;
      [MarshalAs(UnmanagedType.LPTStr)]
      public string pLocation;
      public IntPtr pDevMode;
      [MarshalAs(UnmanagedType.LPTStr)]
      public string pSepFile;
      [MarshalAs(UnmanagedType.LPTStr)]
      public string pPrintProcessor;
      [MarshalAs(UnmanagedType.LPTStr)]
      public string pDatatype;
      [MarshalAs(UnmanagedType.LPTStr)]
      public string pParameters;
      public IntPtr pSecurityDescriptor;
      public uint Attributes;
      public uint Priority;
      public uint DefaultPriority;
      public uint StartTime;
      public uint UntilTime;
      public uint Status;
      public uint cJobs;
      public uint AveragePPM;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEMTIME
    {
      [MarshalAs(UnmanagedType.U2)]
      public short Year;
      [MarshalAs(UnmanagedType.U2)]
      public short Month;
      [MarshalAs(UnmanagedType.U2)]
      public short DayOfWeek;
      [MarshalAs(UnmanagedType.U2)]
      public short Day;
      [MarshalAs(UnmanagedType.U2)]
      public short Hour;
      [MarshalAs(UnmanagedType.U2)]
      public short Minute;
      [MarshalAs(UnmanagedType.U2)]
      public short Second;
      [MarshalAs(UnmanagedType.U2)]
      public short Milliseconds;

      public SYSTEMTIME(DateTime dt)
      {
        dt = dt.ToUniversalTime();  // SetSystemTime expects the SYSTEMTIME in UTC
        Year = (short)dt.Year;
        Month = (short)dt.Month;
        DayOfWeek = (short)dt.DayOfWeek;
        Day = (short)dt.Day;
        Hour = (short)dt.Hour;
        Minute = (short)dt.Minute;
        Second = (short)dt.Second;
        Milliseconds = (short)dt.Millisecond;
      }
    }

    public struct JOB_INFO_1
    {
      public uint JobId;
      public string pPrinterName;
      public string pMachineName;
      public string pUserName;
      public string pDocument;
      public string pDatatype;
      public string pStatus;
      public uint Status;
      public uint Priority;
      public uint Position;
      public uint TotalPages;
      public uint PagesPrinted;
      public SYSTEMTIME Submitted;
    }

    public const int PRINTER_STATUS_PAUSED = 0x00000001;
    public const int PRINTER_STATUS_ERROR = 0x00000002;
    public const int PRINTER_STATUS_PENDING_DELETION = 0x00000004;
    public const int PRINTER_STATUS_PAPER_JAM = 0x00000008;
    public const int PRINTER_STATUS_PAPER_OUT = 0x00000010;
    public const int PRINTER_STATUS_MANUAL_FEED = 0x00000020;
    public const int PRINTER_STATUS_PAPER_PROBLEM = 0x00000040;
    public const int PRINTER_STATUS_OFFLINE = 0x00000080;
    public const int PRINTER_STATUS_IO_ACTIVE = 0x00000100;
    public const int PRINTER_STATUS_BUSY = 0x00000200;
    public const int PRINTER_STATUS_PRINTING = 0x00000400;
    public const int PRINTER_STATUS_OUTPUT_BIN_FULL = 0x00000800;
    public const int PRINTER_STATUS_NOT_AVAILABLE = 0x00001000;
    public const int PRINTER_STATUS_WAITING = 0x00002000;
    public const int PRINTER_STATUS_PROCESSING = 0x00004000;
    public const int PRINTER_STATUS_INITIALIZING = 0x00008000;
    public const int PRINTER_STATUS_WARMING_UP = 0x00010000;
    public const int PRINTER_STATUS_TONER_LOW = 0x00020000;
    public const int PRINTER_STATUS_NO_TONER = 0x00040000;
    public const int PRINTER_STATUS_PAGE_PUNT = 0x00080000;
    public const int PRINTER_STATUS_USER_INTERVENTION = 0x00100000;
    public const int PRINTER_STATUS_OUT_OF_MEMORY = 0x00200000;
    public const int PRINTER_STATUS_DOOR_OPEN = 0x00400000;
    public const int PRINTER_STATUS_SERVER_UNKNOWN = 0x00800000;
    public const int PRINTER_STATUS_POWER_SAVE = 0x01000000;

    public const int JOB_STATUS_BLOCKED_DEVQ = 0x00000200;
    public const int JOB_STATUS_COMPLETE = 0x00001000;
    public const int JOB_STATUS_DELETED = 0x00000100;
    public const int JOB_STATUS_DELETING = 0x00000004;
    public const int JOB_STATUS_ERROR = 0x00000002;
    public const int JOB_STATUS_OFFLINE = 0x00000020;
    public const int JOB_STATUS_PAPEROUT = 0x00000040;
    public const int JOB_STATUS_PAUSED = 0x00000001;
    public const int JOB_STATUS_PRINTED = 0x00000080;
    public const int JOB_STATUS_PRINTING = 0x00000010;
    public const int JOB_STATUS_RESTART = 0x00000800;
    public const int JOB_STATUS_SPOOLING = 0x00000008;
    public const int JOB_STATUS_USER_INTERVENTION = 0x00000400;

      // open()
    public static IntPtr open(string aPrinterName)
    {
      IntPtr h;
      if (!RawPrinterHelper.OpenPrinter(aPrinterName, out h, IntPtr.Zero))
      {
        Win32Exception e = new Win32Exception();
      }
      return h;
    }

    // startDoc()
    public static uint startDoc(IntPtr aPrinterHandle, DOCINFOA aDocInfo)
    {
      UInt32 jobId = RawPrinterHelper.StartDocPrinter(aPrinterHandle, 1, aDocInfo);
      if (jobId == 0)
        throw new Win32Exception();

      return jobId;
    }

    // startPage()
    public static void startPage(IntPtr aPrinterHandle)
    {
      if (!RawPrinterHelper.StartPagePrinter(aPrinterHandle))
        throw new Win32Exception();
    }

    public static void write(IntPtr aPrinterHandle, byte[] data)
    {
      IntPtr unmanagedBytes = new IntPtr(0);
      int length = data.Length;

      // Allocate some unmanaged memory for those bytes.
      unmanagedBytes = Marshal.AllocCoTaskMem(length);
      // Copy the managed byte array into the unmanaged array.
      Marshal.Copy(data, 0, unmanagedBytes, length);

      // Send the unmanaged bytes to the printer.
      int written;
      bool success = RawPrinterHelper.WritePrinter(aPrinterHandle, unmanagedBytes, length, out written);

      // Free the unmanaged memory that you allocated earlier.
      Marshal.FreeCoTaskMem(unmanagedBytes);

      if (!success)
        throw new Win32Exception();
    }

    /*private void write(string s)
    {
      System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
      write(encoding.GetBytes(s));
    }*/

    // endPage()
    public static void endPage(IntPtr aPrinterHandle)
    {
      if (!EndPagePrinter(aPrinterHandle))
        throw new Win32Exception();
    }

    // endDoc()
    public static void endDoc(IntPtr aPrinterHandle)
    {
      if (!EndDocPrinter(aPrinterHandle))
        throw new Win32Exception();
    }

    // close()
    public static void close(IntPtr aPrinterHandle)
    {
      if (!ClosePrinter(aPrinterHandle))
        throw new Win32Exception();
    }

    // getPrinterInfo2
    public static RawPrinterHelper.PRINTER_INFO_2 getPrinterInfo2(IntPtr aPrinterHandle)
    {
      // code snippet from http://www.pinvoke.net/default.aspx/winspool/GetPrinter.html
      uint cbNeeded = 0;
      PRINTER_INFO_2 info2 = new PRINTER_INFO_2();

      bool bRet = GetPrinter(aPrinterHandle, 2, IntPtr.Zero, 0, ref cbNeeded);
      if (cbNeeded > 0)
      {
        IntPtr pAddr = Marshal.AllocHGlobal((int)cbNeeded);
        bRet = GetPrinter(aPrinterHandle, 2, pAddr, cbNeeded, ref cbNeeded);
        if (bRet)
        {
          info2 = (PRINTER_INFO_2)Marshal.PtrToStructure(pAddr, typeof(PRINTER_INFO_2));
          // Now use the info from Info2 structure etc
        }
        Marshal.FreeHGlobal(pAddr);

      }

      if (!bRet)
        throw new Win32Exception();

      return info2;
    }

    public static List<JOB_INFO_1> enumJobs(IntPtr aPrinterHandle)
    {
      PRINTER_INFO_2 printerInfo2 = getPrinterInfo2(aPrinterHandle);
      uint needed;
      uint returned;

      List<JOB_INFO_1> jobInfoList = new List<JOB_INFO_1>();

      bool bRet = EnumJobs(aPrinterHandle, 0, printerInfo2.cJobs, 1, IntPtr.Zero, 0, out needed, out returned);
      if (needed > 0)
      {
        IntPtr buf = Marshal.AllocHGlobal((int)needed);
        bRet = EnumJobs(aPrinterHandle, 0, printerInfo2.cJobs, 1, buf, needed, out needed, out returned);

        if (bRet)
        {
          for (int i = 0; i < returned; i++)
          {
            JOB_INFO_1 jobInfo = new JOB_INFO_1();
            jobInfo = (JOB_INFO_1)Marshal.PtrToStructure(new IntPtr(buf.ToInt32() + Marshal.SizeOf(typeof(JOB_INFO_1)) * i), typeof(JOB_INFO_1));
            jobInfoList.Add(jobInfo);
          }
        }
        Marshal.FreeHGlobal(buf);
      }

      return jobInfoList;
    }

    private RawPrinterHelper.JOB_INFO_1 getJobInfo(IntPtr aPrinterHandle, uint jobId)
    {
      uint needed;

      bool bRet = RawPrinterHelper.GetJob(aPrinterHandle, jobId, 1, IntPtr.Zero, 0, out needed);
      if (needed > 0)
      {
        IntPtr buf = Marshal.AllocHGlobal((int)needed);
        bRet = RawPrinterHelper.GetJob(aPrinterHandle, jobId, 1, buf, needed, out needed);

        RawPrinterHelper.JOB_INFO_1 jobInfo = new RawPrinterHelper.JOB_INFO_1();
        if (bRet)
          jobInfo = (RawPrinterHelper.JOB_INFO_1)Marshal.PtrToStructure(new IntPtr(buf.ToInt32()), typeof(RawPrinterHelper.JOB_INFO_1));
        Marshal.FreeHGlobal(buf);
        if (bRet)
          return jobInfo;
      }

      throw new Win32Exception();
    }

    public static string getJobStatusString(JOB_INFO_1 jobInfo1)
    {
      List<string> l = new List<string>();

      uint status = jobInfo1.Status;

      if (jobInfo1.pStatus != null)
        l.Add(jobInfo1.pStatus);

      if ((status & RawPrinterHelper.JOB_STATUS_BLOCKED_DEVQ) > 0)
        l.Add("The driver cannot print the job");

      if ((status & RawPrinterHelper.JOB_STATUS_COMPLETE) > 0)
        l.Add("Job is sent to the printer, but the job may not be printed yet");

      if ((status & RawPrinterHelper.JOB_STATUS_DELETED) > 0)
        l.Add("Job has been deleted");

      if ((status & RawPrinterHelper.JOB_STATUS_DELETING) > 0)
        l.Add("Job is being deleted");

      if ((status & RawPrinterHelper.JOB_STATUS_ERROR) > 0)
        l.Add("An error is associated with the job");

      if ((status & RawPrinterHelper.JOB_STATUS_OFFLINE) > 0)
        l.Add("Printer is offline");

      if ((status & RawPrinterHelper.JOB_STATUS_PAPEROUT) > 0)
        l.Add("Printer is out of paper");

      if ((status & RawPrinterHelper.JOB_STATUS_PAUSED) > 0)
        l.Add("Job is paused");

      if ((status & RawPrinterHelper.JOB_STATUS_PRINTED) > 0)
        l.Add("Job has printed");

      //if ((status & JOB_STATUS_PRINTING) > 0)
      //  l.Add("Job is printing");

      if ((status & RawPrinterHelper.JOB_STATUS_RESTART) > 0)
        l.Add("Job has been restarted");

      if ((status & RawPrinterHelper.JOB_STATUS_SPOOLING) > 0)
        l.Add("Job is spooling");

      if ((status & RawPrinterHelper.JOB_STATUS_USER_INTERVENTION) > 0)
        l.Add("Printer has an error that requires the user to do something");

      return String.Join(" | ", l.ToArray());

    }

    public static string getPrinterStatusString(PRINTER_INFO_2 info2)
    {
      List<string> l = new List<string>();

      uint status = info2.Status;

      if ((status & RawPrinterHelper.PRINTER_STATUS_PAUSED) > 0)
        l.Add("The printer is paused");

      if ((status & RawPrinterHelper.PRINTER_STATUS_ERROR) > 0)
        l.Add("The printer is in an error state");

      if ((status & RawPrinterHelper.PRINTER_STATUS_PENDING_DELETION) > 0)
        l.Add("The printer is being deleted");

      if ((status & RawPrinterHelper.PRINTER_STATUS_PAPER_JAM) > 0)
        l.Add("Paper is jammed in the printer");

      if ((status & RawPrinterHelper.PRINTER_STATUS_PAPER_OUT) > 0)
        l.Add("The printer is out of paper");

      if ((status & RawPrinterHelper.PRINTER_STATUS_MANUAL_FEED) > 0)
        l.Add("The printer is in a manual feed state");

      if ((status & RawPrinterHelper.PRINTER_STATUS_PAPER_PROBLEM) > 0)
        l.Add("The printer has a paper problem");

      if ((status & RawPrinterHelper.PRINTER_STATUS_OFFLINE) > 0)
        l.Add("The printer is offline");

      if ((status & RawPrinterHelper.PRINTER_STATUS_IO_ACTIVE) > 0)
        l.Add("The printer is in an active input/output state");

      if ((status & RawPrinterHelper.PRINTER_STATUS_BUSY) > 0)
        l.Add("The printer is busy");

      if ((status & RawPrinterHelper.PRINTER_STATUS_PRINTING) > 0)
        l.Add("The printer is printing");

      if ((status & RawPrinterHelper.PRINTER_STATUS_OUTPUT_BIN_FULL) > 0)
        l.Add("The printer is not available for printing");

      if ((status & RawPrinterHelper.PRINTER_STATUS_NOT_AVAILABLE) > 0)
        l.Add("The printer is not available for printing");

      if ((status & RawPrinterHelper.PRINTER_STATUS_WAITING) > 0)
        l.Add("The printer is waiting");

      if ((status & RawPrinterHelper.PRINTER_STATUS_PROCESSING) > 0)
        l.Add("The printer is processing a print job");

      if ((status & RawPrinterHelper.PRINTER_STATUS_INITIALIZING) > 0)
        l.Add("The printer is initializing");

      if ((status & RawPrinterHelper.PRINTER_STATUS_WARMING_UP) > 0)
        l.Add("The printer is warming up");

      if ((status & RawPrinterHelper.PRINTER_STATUS_TONER_LOW) > 0)
        l.Add("The printer is low on toner");

      if ((status & RawPrinterHelper.PRINTER_STATUS_NO_TONER) > 0)
        l.Add("The printer is out of toner");

      if ((status & RawPrinterHelper.PRINTER_STATUS_PAGE_PUNT) > 0)
        l.Add("The printer cannot print the current page");

      if ((status & RawPrinterHelper.PRINTER_STATUS_USER_INTERVENTION) > 0)
        l.Add("The printer has an error that requires the user to do something");

      if ((status & RawPrinterHelper.PRINTER_STATUS_OUT_OF_MEMORY) > 0)
        l.Add("The printer has run out of memory");

      if ((status & RawPrinterHelper.PRINTER_STATUS_DOOR_OPEN) > 0)
        l.Add("The printer door is open");

      if ((status & RawPrinterHelper.PRINTER_STATUS_SERVER_UNKNOWN) > 0)
        l.Add("The printer status is unknown");

      if ((status & RawPrinterHelper.PRINTER_STATUS_POWER_SAVE) > 0)
        l.Add("The printer is in power save mode");

      return String.Join(" | ", l.ToArray());

    }

  }
}
