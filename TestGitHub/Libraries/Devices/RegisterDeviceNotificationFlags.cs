namespace TestGitHub.Libraries.Devices
{
    /// <summary>
    /// RegisterDeviceNotificationFlag.
    /// https://msdn.microsoft.com/en-us/library/aa363431(v=vs.85).aspx.
    /// </summary>
    [System.Flags]
    public enum RegisterDeviceNotificationFlags
    {
        /// <summary>
        /// The hRecipient parameter is a window handle.
        /// </summary>
        DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000,

        /// <summary>
        /// Notifies the recipient of device interface events for all device interface classes.
        /// (The dbcc_classguid member is ignored.)
        /// This value can be used only if the dbch_devicetype member
        /// is DBT_DEVTYP_DEVICEINTERFACE.
        /// </summary>
        DEVICE_NOTIFY_SERVICE_HANDLE = 0x00000001,

        /// <summary>
        /// The hRecipient parameter is a window handle.
        /// </summary>
        DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004,
    }
}
