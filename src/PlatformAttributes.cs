namespace System.Runtime.Versioning;

#if NETCOREAPP3_1

[AttributeUsage(AttributeTargets.Assembly |
                AttributeTargets.Class |
                AttributeTargets.Constructor |
                AttributeTargets.Enum |
                AttributeTargets.Event |
                AttributeTargets.Field |
                AttributeTargets.Interface |
                AttributeTargets.Method |
                AttributeTargets.Module |
                AttributeTargets.Property |
                AttributeTargets.Struct,
                AllowMultiple = true, Inherited = false)]
internal sealed class UnsupportedOSPlatformAttribute : Attribute
{
    public UnsupportedOSPlatformAttribute(string platformName)
        => PlatformName = platformName;

    public string PlatformName { get; }
}

#endif

#if NETCOREAPP3_1 || NET5_0

[AttributeUsage(AttributeTargets.Field |
                AttributeTargets.Method |
                AttributeTargets.Property,
                AllowMultiple = true, Inherited = false)]
internal sealed class SupportedOSPlatformGuardAttribute : Attribute
{
    public SupportedOSPlatformGuardAttribute(string platformName)
        => PlatformName = platformName;

    public string PlatformName { get; }
}

#endif
