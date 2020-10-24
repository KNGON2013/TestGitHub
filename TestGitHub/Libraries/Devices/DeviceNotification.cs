using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace TestGitHub.Libraries.Devices
{
    public static class DeviceNotification
    {
        private static IntPtr notificationHandle;

        /// <summary>
        /// Registers a window to receive notifications when devices are plugged or unplugged.
        /// </summary>
        /// <param name="windowHandle">Handle to the window receiving notifications.</param>
        /// <param name="classGuid">classGuid.</param>
        /// <param name="usbOnly">true to filter to USB devices only, false to be notified for all devices.</param>
        public static void RegisterDeviceNotification(IntPtr windowHandle, Guid classGuid, bool usbOnly = false)
        {
            var dbi = new DevBroadcastDeviceinterface
            {
                DeviceType = (int)DeviceBroadcast.DBT_ClassOfDevices,
                Reserved = 0,
                ClassGuid = classGuid,
                Name = 0,
            };

            dbi.Size = Marshal.SizeOf(dbi);
            var buffer = Marshal.AllocHGlobal(dbi.Size);
            Marshal.StructureToPtr(dbi, buffer, true);

            notificationHandle =
                RegisterDeviceNotification(windowHandle, buffer, usbOnly ? 0 : (int)RegisterDeviceNotificationFlags.DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
        }

        /// <summary>
        /// Registers a window to receive notifications when USB devices are plugged or unplugged.
        /// </summary>
        /// <param name="windowHandle">Handle to the window receiving notifications.</param>
        public static void RegisterUsbDeviceNotification(IntPtr windowHandle)
        {
            var dbi = new DevBroadcastDeviceinterface
            {
                DeviceType = (int)DeviceBroadcast.DBT_ClassOfDevices,
                Reserved = 0,
                ClassGuid = RegisteredGuid.USB,
                Name = 0,
            };

            dbi.Size = Marshal.SizeOf(dbi);
            var buffer = Marshal.AllocHGlobal(dbi.Size);
            Marshal.StructureToPtr(dbi, buffer, true);

            notificationHandle = RegisterDeviceNotification(windowHandle, buffer, 0);
        }

        /// <summary>
        /// Unregisters the window for USB device notifications.
        /// </summary>
        public static void UnregisterUsbDeviceNotification()
        {
            UnregisterDeviceNotification(notificationHandle);
        }

        /// <summary>
        /// Unregisters the window for device notifications.
        /// </summary>
        public static void UnregisterDeviceNotification()
        {
            UnregisterDeviceNotification(notificationHandle);
        }

        /// <inheritdoc/>
        public static IEnumerable<DeviceEntity> GetConnectedDevices()
        {
            var list = new List<DeviceEntity>();

            using (var entity = new ManagementClass("Win32_PnPEntity"))
            {
                var manageObject = entity.GetInstances();

                // var regex = new Regex(@"ven_([0-9a-f]+)&dev_([0-9a-f]+)", RegexOptions.IgnoreCase);
                var regexVid = new Regex(@"vid_([0-9a-f]+)" + @"|ven_([0-9a-f]+)", RegexOptions.IgnoreCase);  // VID / VEN
                var regexPid = new Regex(@"pid_([0-9a-f]+)" + @"|PROD_([0-9a-f]+)", RegexOptions.IgnoreCase); // PID / PROD
                var regexDev = new Regex(@"dev_([0-9a-f]+)", RegexOptions.IgnoreCase);                        // DEV

                foreach (var a in manageObject)
                {
                    if (a != null)
                    {
                        var guid = (string)a.GetPropertyValue("ClassGuid");
                        if (guid != null)
                        {
                            var deviceID = (string)a.GetPropertyValue("DeviceID");

                            var vidMatches = regexVid.Matches(deviceID);
                            var pidMatches = regexPid.Matches(deviceID);
                            var devMatches = regexDev.Matches(deviceID);

                            var vid = vidMatches.Count > 0 ? vidMatches[0].ToString() : string.Empty;
                            var pid = pidMatches.Count > 0 ? pidMatches[0].ToString() : string.Empty;
                            var dev = devMatches.Count > 0 ? devMatches[0].ToString() : string.Empty;

                            list.Add(new DeviceEntity()
                            {
                                ClassGuid = new Guid(guid),
                                DeviceID = deviceID,
                                Caption = (string)a.GetPropertyValue("Caption"),
                                Description = (string)a.GetPropertyValue("Description"),
                                Service = (string)a.GetPropertyValue("Service"),
                                Status = (string)a.GetPropertyValue("Status"),
                                StatusInfo = (string)a.GetPropertyValue("StatusInfo"),
                                Vendor = vid,
                                Product = pid,
                                ExtractDeviceID = dev,
                            });
                        }
                    }
                }
            }

            return list;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll")]
        private static extern bool UnregisterDeviceNotification(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        private struct DevBroadcastDeviceinterface
        {
            internal int Size;
            internal int DeviceType;
            internal int Reserved;
            internal Guid ClassGuid;
            internal short Name;
        }
    }
}
