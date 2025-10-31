using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MBA.EducaOn.Core.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Retrieves the description associated with an enumeration value.
    /// </summary>
    /// <param name="value">The enumeration value for which to retrieve the description.</param>
    /// <returns>The description specified in the <see cref="DescriptionAttribute"/> applied to the enumeration value, or the
    /// string representation of the enumeration value if no description is defined.</returns>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());

        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description ?? value.ToString();
    }

    /// <summary>
    /// Retrieves the display name of the specified enumeration value.
    /// </summary>
    /// <remarks>This method uses reflection to retrieve the <see
    /// cref="System.ComponentModel.DataAnnotations.DisplayAttribute"/> associated with the enumeration value. If no
    /// such attribute is found, the method returns the string representation of the enumeration value.</remarks>
    /// <param name="value">The enumeration value for which to retrieve the display name.</param>
    /// <returns>The display name specified by the <see cref="System.ComponentModel.DataAnnotations.DisplayAttribute"/> applied
    /// to the enumeration value, or the enumeration value's name as a string if no display attribute is present.</returns>
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());

        var attribute = field?.GetCustomAttributes(typeof(DisplayAttribute), false)
                             .Cast<DisplayAttribute>()
                             .FirstOrDefault();

        return attribute?.Name ?? value.ToString();
    }
}
