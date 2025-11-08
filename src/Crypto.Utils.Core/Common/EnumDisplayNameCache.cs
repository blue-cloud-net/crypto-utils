using System.Reflection;

namespace Cert.Utils.Common;

/// <summary>
/// 枚举显示名称缓存工具类
/// 使用静态构造函数预加载所有枚举值的 Display 特性
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
public static class EnumDisplayNameCache<TEnum> where TEnum : struct, Enum
{
    private static readonly ConcurrentDictionary<TEnum, string> _displayNames;

    static EnumDisplayNameCache()
    {
        _displayNames = new ConcurrentDictionary<TEnum, string>();

        foreach (var enumValue in Enum.GetValues<TEnum>())
        {
            var enumDisplayName = typeof(TEnum)
                .GetField(enumValue.ToString())
                ?.GetCustomAttribute<DisplayAttribute>()
                ?.Name;

            if (enumDisplayName is null) 
                continue;
            
            _displayNames[enumValue] = enumDisplayName;
        }
    }

    /// <summary>
    /// 获取枚举值的显示名称
    /// </summary>
    /// <param name="enumValue">枚举值</param>
    /// <returns>Display 特性中定义的名称,如果没有则返回枚举名</returns>
    public static string GetDisplayName(TEnum enumValue) => 
        _displayNames.TryGetValue(enumValue, out var name)
            ? name 
            : enumValue.ToString();

    /// <summary>
    /// 尝试获取枚举值的显示名称
    /// </summary>
    /// <param name="enumValue">枚举值</param>
    /// <param name="displayName">输出的显示名称</param>
    /// <returns>如果成功获取返回 true,否则返回 false</returns>
    public static bool TryGetDisplayName(TEnum enumValue, out string displayName) => 
        _displayNames.TryGetValue(enumValue, out displayName!);
}
