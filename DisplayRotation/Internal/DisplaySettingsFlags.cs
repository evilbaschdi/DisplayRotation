namespace DisplayRotation.Internal;

[Flags]
internal enum DisplaySettingsFlags
{
    CdsUpdateregistry = 1,
    CdsTest = 2,
    CdsFullscreen = 4,
    CdsGlobal = 8,
    CdsSetPrimary = 0x10,
    CdsReset = 0x40000000,
    CdsNoreset = 0x10000000
}