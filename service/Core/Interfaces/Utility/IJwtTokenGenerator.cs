namespace service.Core.Interfaces.Utility
{
    /// <summary>
    /// Interface defining functionalities for generating JWT tokens.
    /// </summary>
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Generates a JSON Web Token (JWT) based on the provided user information.
        /// </summary>
        /// <param name="userId">Unique identifier of the user.</param>
        /// <param name="username">Username of the user.</param>
        /// <param name="role">User's role within the system.</param>
        /// <returns>A string containing the generated JWT token.</returns>
        string GenerateToken(string userId, string username, string role);
    }
}
