namespace DisplayRotation.Core;

/// <summary />
[Flags]
public enum DisplayDeviceStateFlags
{
    /// <summary />
    AttachedToDesktop = 0x1,

    /// <summary />
    MultiDriver = 0x2,

    /// <summary />
    PrimaryDevice = 0x4,

    /// <summary />
    MirroringDriver = 0x8,

    /// <summary />
    VgaCompatible = 0x16,

    /// <summary />
    Removable = 0x20,

    /// <summary />
    ModesPruned = 0x8000000,

    /// <summary />
    Remote = 0x4000000,

    /// <summary />
    Disconnect = 0x2000000
}