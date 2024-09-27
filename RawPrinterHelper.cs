using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace PrintInvoice
{
    internal class RawPrinterHelper
    {
        public const int StatusPaused = 0x00000001;
        public const int StatusError = 0x00000002;
        public const int StatusPendingDeletion = 0x00000004;
        public const int StatusPaperJam = 0x00000008;
        public const int StatusPaperOut = 0x00000010;
        public const int StatusManualFeed = 0x00000020;
        public const int StatusPaperProblem = 0x00000040;
        public const int StatusOffline = 0x00000080;
        public const int StatusIoActive = 0x00000100;
        public const int StatusBusy = 0x00000200;
        public const int StatusPrinting = 0x00000400;
        public const int StatusOutputBinFull = 0x00000800;
        public const int StatusNotAvailable = 0x00001000;
        public const int StatusWaiting = 0x00002000;
        public const int StatusProcessing = 0x00004000;
        public const int StatusInitializing = 0x00008000;
        public const int StatusWarmingUp = 0x00010000;
        public const int StatusTonerLow = 0x00020000;
        public const int StatusNoToner = 0x00040000;
        public const int StatusPagePunt = 0x00080000;
        public const int StatusUserIntervention = 0x00100000;
        public const int StatusOutOfMemory = 0x00200000;
        public const int StatusDoorOpen = 0x00400000;
        public const int StatusServerUnknown = 0x00800000;
        public const int StatusPowerSave = 0x01000000;

        public const int JobStatusBlockedDevq = 0x00000200;
        public const int JobStatusComplete = 0x00001000;
        public const int JobStatusDeleted = 0x00000100;
        public const int JobStatusDeleting = 0x00000004;
        public const int JobStatusError = 0x00000002;
        public const int JobStatusOffline = 0x00000020;
        public const int JobStatusPaperOut = 0x00000040;
        public const int JobStatusPaused = 0x00000001;
        public const int JobStatusPrinted = 0x00000080;
        public const int JobStatusPrinting = 0x00000010;
        public const int JobStatusRestart = 0x00000800;
        public const int JobStatusSpooling = 0x00000008;
        public const int JobStatusUserIntervention = 0x00000400;

        [DllImport("winspool.drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern uint StartDocPrinter(IntPtr hPrinter, int level, [In] [MarshalAs(UnmanagedType.LPStruct)] DocInfoA di);

        [DllImport("winspool.drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        [DllImport("winspool.drv", EntryPoint = "GetPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetPrinter(IntPtr hPrinter, uint dwLevel, IntPtr pPrinter, uint dwBuf, ref uint dwNeeded);

        [DllImport("Winspool.drv", SetLastError = true, EntryPoint = "EnumJobsA", CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EnumJobs(IntPtr hPrinter, uint firstJob, uint noJobs, uint level, IntPtr pJob, uint cbBuf, out uint pcbNeeded, out uint pcReturned);

        // open()
        public static IntPtr Open(string aPrinterName)
        {
            return !OpenPrinter(aPrinterName, out var h, IntPtr.Zero) ? throw new Win32Exception() : h;
        }

        // startDoc()
        public static uint StartDoc(IntPtr aPrinterHandle, DocInfoA aDocInfo)
        {
            var jobId = StartDocPrinter(aPrinterHandle, 1, aDocInfo);
            return jobId == 0 ? throw new Win32Exception() : jobId;
        }

        public static void StartPage(IntPtr aPrinterHandle)
        {
            if (!StartPagePrinter(aPrinterHandle))
                throw new Win32Exception();
        }

        public static void Write(IntPtr aPrinterHandle, byte[] data)
        {
            var length = data.Length;

            // Allocate some unmanaged memory for those bytes.
            var unmanagedBytes = Marshal.AllocCoTaskMem(length);

            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(data, 0, unmanagedBytes, length);

            // Send the unmanaged bytes to the printer.
            var success = WritePrinter(aPrinterHandle, unmanagedBytes, length, out _);

            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(unmanagedBytes);

            if (!success)
                throw new Win32Exception();
        }

        public static void EndPage(IntPtr aPrinterHandle)
        {
            if (!EndPagePrinter(aPrinterHandle))
                throw new Win32Exception();
        }

        // endDoc()
        public static void EndDoc(IntPtr aPrinterHandle)
        {
            if (!EndDocPrinter(aPrinterHandle))
                throw new Win32Exception();
        }

        // close()
        public static void Close(IntPtr aPrinterHandle)
        {
            if (!ClosePrinter(aPrinterHandle))
                throw new Win32Exception();
        }

        // getPrinterInfo2
        public static Info2 GetPrinterInfo2(IntPtr aPrinterHandle)
        {
            // code snippet from http://www.pinvoke.net/default.aspx/winspool/GetPrinter.html
            uint cbNeeded = 0;
            var info2 = new Info2();
            var bRet = GetPrinter(aPrinterHandle, 2, IntPtr.Zero, 0, ref cbNeeded);

            if (cbNeeded > 0)
            {
                var pAddr = Marshal.AllocHGlobal((int)cbNeeded);
                bRet = GetPrinter(aPrinterHandle, 2, pAddr, cbNeeded, ref cbNeeded);
                
                if (bRet) info2 = (Info2)Marshal.PtrToStructure(pAddr, typeof(Info2));
            
                // Now use the info from Info2 structure etc
                Marshal.FreeHGlobal(pAddr);
            }

            return !bRet ? throw new Win32Exception() : info2;
        }

        public static List<JobInfo1> EnumJobs(IntPtr aPrinterHandle)
        {
            var printerInfo2 = GetPrinterInfo2(aPrinterHandle);
            var jobInfoList = new List<JobInfo1>();
            var bRet = EnumJobs(aPrinterHandle, 0, printerInfo2.cJobs, 1, IntPtr.Zero, 0, out var needed, out var returned);

            if (needed > 0)
            {
                var buf = Marshal.AllocHGlobal((int)needed);
                bRet = EnumJobs(aPrinterHandle, 0, printerInfo2.cJobs, 1, buf, needed, out needed, out returned);

                if (bRet)
                {
                    for (var i = 0; i < returned; i++)
                    {
                        var jobInfo = (JobInfo1)Marshal.PtrToStructure( IntPtr.Add(buf, Marshal.SizeOf(typeof(JobInfo1)) * i), typeof(JobInfo1));
                        jobInfoList.Add(jobInfo);
                    }
                }

                Marshal.FreeHGlobal(buf);
            }

            return jobInfoList;
        }

        public static string GetJobStatusString(JobInfo1 jobInfo1)
        {
            var l = new List<string>();

            var status = jobInfo1.Status;

            if (jobInfo1.StatusMessage != null)
                l.Add(jobInfo1.StatusMessage);

            if ((status & JobStatusBlockedDevq) > 0)
                l.Add("The driver cannot print the job");

            if ((status & JobStatusComplete) > 0)
                l.Add("Job is sent to the printer, but the job may not be printed yet");

            if ((status & JobStatusDeleted) > 0)
                l.Add("Job has been deleted");

            if ((status & JobStatusDeleting) > 0)
                l.Add("Job is being deleted");

            if ((status & JobStatusError) > 0)
                l.Add("An error is associated with the job");

            if ((status & JobStatusOffline) > 0)
                l.Add("Printer is offline");

            if ((status & JobStatusPaperOut) > 0)
                l.Add("Printer is out of paper");

            if ((status & JobStatusPaused) > 0)
                l.Add("Job is paused");

            if ((status & JobStatusPrinted) > 0)
                l.Add("Job has printed");

            if ((status & JobStatusRestart) > 0)
                l.Add("Job has been restarted");

            if ((status & JobStatusSpooling) > 0)
                l.Add("Job is spooling");

            if ((status & JobStatusUserIntervention) > 0)
                l.Add("Printer has an error that requires the user to do something");

            return string.Join(" | ", l.ToArray());
        }

        public static string GetPrinterStatusString(Info2 info2)
        {
            var l = new List<string>();

            var status = info2.Status;

            if ((status & StatusPaused) > 0)
                l.Add("The printer is paused");

            if ((status & StatusError) > 0)
                l.Add("The printer is in an error state");

            if ((status & StatusPendingDeletion) > 0)
                l.Add("The printer is being deleted");

            if ((status & StatusPaperJam) > 0)
                l.Add("Paper is jammed in the printer");

            if ((status & StatusPaperOut) > 0)
                l.Add("The printer is out of paper");

            if ((status & StatusManualFeed) > 0)
                l.Add("The printer is in a manual feed state");

            if ((status & StatusPaperProblem) > 0)
                l.Add("The printer has a paper problem");

            if ((status & StatusOffline) > 0)
                l.Add("The printer is offline");

            if ((status & StatusIoActive) > 0)
                l.Add("The printer is in an active input/output state");

            if ((status & StatusBusy) > 0)
                l.Add("The printer is busy");

            if ((status & StatusPrinting) > 0)
                l.Add("The printer is printing");

            if ((status & StatusOutputBinFull) > 0)
                l.Add("The printer is not available for printing");

            if ((status & StatusNotAvailable) > 0)
                l.Add("The printer is not available for printing");

            if ((status & StatusWaiting) > 0)
                l.Add("The printer is waiting");

            if ((status & StatusProcessing) > 0)
                l.Add("The printer is processing a print job");

            if ((status & StatusInitializing) > 0)
                l.Add("The printer is initializing");

            if ((status & StatusWarmingUp) > 0)
                l.Add("The printer is warming up");

            if ((status & StatusTonerLow) > 0)
                l.Add("The printer is low on toner");

            if ((status & StatusNoToner) > 0)
                l.Add("The printer is out of toner");

            if ((status & StatusPagePunt) > 0)
                l.Add("The printer cannot print the current page");

            if ((status & StatusUserIntervention) > 0)
                l.Add("The printer has an error that requires the user to do something");

            if ((status & StatusOutOfMemory) > 0)
                l.Add("The printer has run out of memory");

            if ((status & StatusDoorOpen) > 0)
                l.Add("The printer door is open");

            if ((status & StatusServerUnknown) > 0)
                l.Add("The printer status is unknown");

            if ((status & StatusPowerSave) > 0)
                l.Add("The printer is in power save mode");

            return string.Join(" | ", l.ToArray());
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DocInfoA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct Info2
        {
            [MarshalAs(UnmanagedType.LPTStr)] public string pServerName;
            [MarshalAs(UnmanagedType.LPTStr)] public string pPrinterName;
            [MarshalAs(UnmanagedType.LPTStr)] public string pShareName;
            [MarshalAs(UnmanagedType.LPTStr)] public string pPortName;
            [MarshalAs(UnmanagedType.LPTStr)] public string pDriverName;
            [MarshalAs(UnmanagedType.LPTStr)] public string pComment;
            [MarshalAs(UnmanagedType.LPTStr)] public string pLocation;
            public IntPtr pDevMode;
            [MarshalAs(UnmanagedType.LPTStr)] public string pSepFile;
            [MarshalAs(UnmanagedType.LPTStr)] public string pPrintProcessor;
            [MarshalAs(UnmanagedType.LPTStr)] public string pDatatype;
            [MarshalAs(UnmanagedType.LPTStr)] public string pParameters;
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
        public struct SystemTime
        {
            [MarshalAs(UnmanagedType.U2)] public short Year;
            [MarshalAs(UnmanagedType.U2)] public short Month;
            [MarshalAs(UnmanagedType.U2)] public short DayOfWeek;
            [MarshalAs(UnmanagedType.U2)] public short Day;
            [MarshalAs(UnmanagedType.U2)] public short Hour;
            [MarshalAs(UnmanagedType.U2)] public short Minute;
            [MarshalAs(UnmanagedType.U2)] public short Second;
            [MarshalAs(UnmanagedType.U2)] public short Milliseconds;

            public SystemTime(DateTime dt)
            {
                dt = dt.ToUniversalTime(); // SetSystemTime expects the SYSTEMTIME in UTC
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

#pragma warning disable CS0649
        public struct JobInfo1
        {
            public uint JobId;
            public string PrinterName;
            public string MachineName;
            public string UserName;
            public string Document;
            public string Datatype;
            public string StatusMessage;
            public uint Status;
            public uint Priority;
            public uint Position;
            public uint TotalPages;
            public uint PagesPrinted;
            public SystemTime Submitted;
        }
#pragma warning restore CS0649
    }
}