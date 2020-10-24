namespace TestGitHub.Libraries.Devices
{
    /// <summary>
    /// DeviceBroadcast Header(DBT).
    /// https://msdn.microsoft.com/ja-jp/library/windows/desktop/aa363246(v=vs.85).aspx.
    /// </summary>
    public enum DeviceBroadcast : int
    {
        /// <summary>
        /// DBT_DEVTYP_DEVICEINTERFACE.
        /// Class of devices. This structure is a DEV_BROADCAST_DEVICEINTERFACE structure.
        /// </summary>
        DBT_ClassOfDevices = 0x00000005,

        /// <summary>
        /// DBT_DEVTYP_HANDLE.
        /// OEM- or IHV-defined device type. This structure is a DEV_BROADCAST_OEM structure.
        /// </summary>
        DBT_FileSystemHandle = 0x00000006,

        /// <summary>
        /// DBT_DEVTYP_OEM.
        /// Port device (serial or parallel). This structure is a DEV_BROADCAST_PORT structure.
        /// </summary>
        DBT_OemOrIHV = 0x00000000,

        /// <summary>
        /// DBT_DEVTYP_PORT.
        /// Port device (serial or parallel). This structure is a DEV_BROADCAST_PORT structure.
        /// </summary>
        DBT_PortDvice = 0x00000003,

        /// <summary>
        /// DBT_DEVTYP_VOLUME.
        /// Logical volume. This structure is a DEV_BROADCAST_VOLUME structure.
        /// </summary>
        DBT_LogicalVolume = 0x00000002,
    }
}
