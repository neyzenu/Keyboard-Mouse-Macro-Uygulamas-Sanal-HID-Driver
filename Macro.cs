using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace VirtualHidClient
{
    class Program
    {
        // Driver tarafında tanımladığınız IOCTL koduyla eşleşmeli
        private const int IOCTL_VHID_INJECT_REPORT =
            (0x22 << 16) | (0x801 << 2) | (0 << 14);
        // Yukarıdaki hesap, CTL_CODE(FILE_DEVICE_UNKNOWN, 0x801, METHOD_BUFFERED, FILE_ANY_ACCESS)
        // makrosunun eşdeğeridir.

        // WinAPI fonksiyonları
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern SafeFileHandle CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            int dwIoControlCode,
            byte[] lpInBuffer,
            int nInBufferSize,
            IntPtr lpOutBuffer,
            int nOutBufferSize,
            out int lpBytesReturned,
            IntPtr lpOverlapped
        );

        // Driver'ın kaydettiği device interface veya sembolik link
        // Gerçekte SetupAPI üzerinden alınan path'i kullanmanız önerilir.
        private const string DEVICE_PATH = @"\\.\HID_Virtual_Keyboard_Mouse";

        static void Main(string[] args)
        {
            // 0xC0000000 => GENERIC_READ | GENERIC_WRITE
            // 3 => OPEN_EXISTING
            using (SafeFileHandle handle = CreateFile(
                DEVICE_PATH,
                0xC0000000,
                0,
                IntPtr.Zero,
                3,
                0,
                IntPtr.Zero))
            {
                if (handle.IsInvalid)
                {
                    Console.WriteLine("Cihaza erişilemedi veya sembolik link bulunamadı!");
                    return;
                }

                // Örnek HID raporu (sanal klavye ya da fare formatına göre doldurmanız lazım)
                // Ör: Klavye raporu 8 byte olabilir (modifier, reserved, keycodes...)
                // Ör: Mouse raporu 4-5 byte (buttons, X, Y, wheel...)
                byte[] inputReport = new byte[] 
                {
                    0x00, // Modifier (ör. Ctrl, Shift vs. yok)
                    0x00, // Reserved
                    0x04, // Bir tuşa (ör. 'A') basma
                    0x00,
                    0x00,
                    0x00,
                    0x00,
                    0x00
                };

                // Driver'a IOCTL ile rapor gönder
                bool success = DeviceIoControl(
                    handle,
                    IOCTL_VHID_INJECT_REPORT,
                    inputReport,
                    inputReport.Length,
                    IntPtr.Zero,
                    0,
                    out int bytesReturned,
                    IntPtr.Zero);

                if (success)
                {
                    Console.WriteLine("Makro (rapor) başarıyla sürücüye gönderildi!");
                }
                else
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine("Makro gönderilemedi. Hata kodu: " + error);
                }
            }

            Console.WriteLine("Devam etmek için bir tuşa basın...");
            Console.ReadKey();
        }
    }
}
