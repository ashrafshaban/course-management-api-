namespace CourseManagement.Api.Auth;

/// <summary>JWT claim/role name constants for mock/API parity (ADR-003, ADR-004).</summary>
public static class AuthConstants
{
    public const string RoleAdmin = "Admin";
    public const string RoleInstructor = "Instructor";
    public const string RoleStudent = "Student";

    public static readonly string[] AllRoles = [RoleAdmin, RoleInstructor, RoleStudent];
}
