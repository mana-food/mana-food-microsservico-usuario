using ManaFood.Domain.Enums;
using ManaFood.Application.Constants;

namespace ManaFood.Application.Mappings;

public static class UserTypeMapper
{
    public static string ToRoleString(UserType userType)
    {
        return userType switch
        {
            UserType.ADMIN => Roles.ADMIN,
            UserType.CUSTOMER => Roles.CUSTOMER,
            UserType.KITCHEN => Roles.KITCHEN,
            UserType.OPERATOR => Roles.OPERATOR,
            UserType.MANAGER => Roles.MANAGER,
            _ => Roles.CUSTOMER
        };
    }

    public static UserType ToUserType(string role)
    {
        return role switch
        {
            Roles.ADMIN => UserType.ADMIN,
            Roles.CUSTOMER => UserType.CUSTOMER,
            Roles.KITCHEN => UserType.KITCHEN,
            Roles.OPERATOR => UserType.OPERATOR,
            Roles.MANAGER => UserType.MANAGER,
            _ => UserType.CUSTOMER
        };
    }

    public static bool HasPermission(UserType userType, params UserType[] allowedTypes)
    {
        return allowedTypes.Contains(userType);
    }

    public static bool HasPermission(string role, params string[] allowedRoles)
    {
        return allowedRoles.Contains(role);
    }
}