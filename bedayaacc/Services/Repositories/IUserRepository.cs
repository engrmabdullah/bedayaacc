using System;
using System.Data;

using System.Threading.Tasks;
using Dapper;
using bedayaacc.Models;
using Microsoft.Extensions.Configuration;
using bedayaacc.Services;
using Microsoft.Data.SqlClient;

namespace bedayaacc.Repositories
{
    public interface IUserRepository
    {
        Task<RegistrationResult> RegisterUserAsync(RegisterModel model);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> UpdateLastLoginAsync(int userId, string? ipAddress = null, string? userAgent = null);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> VerifyEmailAsync(string token);
        Task<string> GenerateEmailVerificationTokenAsync(int userId);

        Task<List<string>> GetUserRolesAsync(int userId);

    }

    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly IPasswordHasher _passwordHasher;

        public UserRepository(IConfiguration configuration, IPasswordHasher passwordHasher)
        {
            _connectionString = configuration.GetConnectionString("BedayaDB")
                ?? throw new ArgumentNullException(nameof(configuration));
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Get database connection
        /// </summary>
        private IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        public async Task<RegistrationResult> RegisterUserAsync(RegisterModel model)
        {
            try
            {
                // Check if email already exists
                if (await EmailExistsAsync(model.Email))
                {
                    return new RegistrationResult
                    {
                        Success = false,
                        Message = "البريد الإلكتروني مسجل مسبقاً"
                    };
                }

                // Hash password
                var (hash, salt) = _passwordHasher.HashPassword(model.Password);

                using (var connection = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@FirstName", model.FirstName);
                    parameters.Add("@LastName", model.LastName);
                    parameters.Add("@Email", model.Email);
                    parameters.Add("@Phone", model.Phone);
                    parameters.Add("@PasswordHash", hash);
                    parameters.Add("@PasswordSalt", salt);
                    parameters.Add("@AcceptMarketing", model.AcceptMarketing);
                    parameters.Add("@UserId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "sp_RegisterUser",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    int userId = parameters.Get<int>("@UserId");

                    return new RegistrationResult
                    {
                        Success = true,
                        UserId = userId,
                        Message = "تم التسجيل بنجاح"
                    };
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Email already registered"))
            {
                return new RegistrationResult
                {
                    Success = false,
                    Message = "البريد الإلكتروني مسجل مسبقاً"
                };
            }
            catch (Exception ex)
            {
                // Log the exception here
                return new RegistrationResult
                {
                    Success = false,
                    Message = "حدث خطأ أثناء التسجيل. يرجى المحاولة مرة أخرى"
                };
            }
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var user = await connection.QueryFirstOrDefaultAsync<User>(
                        "sp_GetUserByEmail",
                        new { Email = email },
                        commandType: CommandType.StoredProcedure
                    );

                    return user;
                }
            }
            catch (Exception ex)
            {
                // Log the exception here
                return null;
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var sql = @"
                        SELECT * FROM Users 
                        WHERE UserId = @UserId 
                            AND IsActive = 1 
                            AND IsDeleted = 0";

                    var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
                    return user;
                }
            }
            catch (Exception ex)
            {
                // Log the exception here
                return null;
            }
        }

        /// <summary>
        /// Check if email exists
        /// </summary>
        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email AND IsDeleted = 0";
                    var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email });
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception here
                return false;
            }
        }

        /// <summary>
        /// Update last login timestamp
        /// </summary>
        public async Task<bool> UpdateLastLoginAsync(int userId, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.ExecuteAsync(
                        "sp_UpdateLastLogin",
                        new { UserId = userId, IpAddress = ipAddress, UserAgent = userAgent },
                        commandType: CommandType.StoredProcedure
                    );

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the exception here
                return false;
            }
        }

        /// <summary>
        /// Generate email verification token
        /// </summary>
        public async Task<string> GenerateEmailVerificationTokenAsync(int userId)
        {
            try
            {
                string token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
                DateTime expiresAt = DateTime.Now.AddHours(24);

                using (var connection = GetConnection())
                {
                    await connection.ExecuteAsync(
                        "sp_GenerateEmailVerificationToken",
                        new { UserId = userId, Token = token, ExpiresAt = expiresAt },
                        commandType: CommandType.StoredProcedure
                    );
                }

                return token;
            }
            catch (Exception ex)
            {
                // Log the exception here
                throw new Exception("Failed to generate verification token", ex);
            }
        }

        /// <summary>
        /// Verify email with token
        /// </summary>
        public async Task<bool> VerifyEmailAsync(string token)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.ExecuteAsync(
                        "sp_VerifyEmail",
                        new { Token = token },
                        commandType: CommandType.StoredProcedure
                    );

                    return true;
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid or expired token"))
            {
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception here
                return false;
            }
        }


        /// <summary>
        /// Get user roles
        /// </summary>

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var sql = @"
                    SELECT r.RoleName 
                    FROM UserRoles ur
                    INNER JOIN Roles r ON ur.RoleId = r.RoleId
                    WHERE ur.UserId = @UserId";

                    var roles = await connection.QueryAsync<string>(sql, new { UserId = userId });
                    return roles.ToList();
                }
            }
            catch (Exception ex)
            {
                // Log the exception here
                return new List<string>();
            }
        }
    }

    /// <summary>
    /// Authentication service
    /// </summary>
    public interface IAuthenticationService
    {
        Task<LoginResult> LoginAsync(LoginModel model);
        Task<RegistrationResult> RegisterAsync(RegisterModel model);
        Task<bool> SendVerificationEmailAsync(int userId);
        Task<List<string>> GetUserRolesAsync(int userId);

    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        // private readonly IEmailService _emailService; // Add this when implementing email

        public AuthenticationService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResult> LoginAsync(LoginModel model)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "البريد الإلكتروني أو كلمة المرور غير صحيحة"
                    };
                }

                if (!user.IsActive)
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "حسابك غير نشط. يرجى التواصل مع الدعم الفني"
                    };
                }

                // Verify password
                bool isPasswordValid = _passwordHasher.VerifyPassword(
                    model.Password,
                    user.PasswordHash,
                    user.PasswordSalt
                );

                if (!isPasswordValid)
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "البريد الإلكتروني أو كلمة المرور غير صحيحة"
                    };
                }

                // Update last login
                await _userRepository.UpdateLastLoginAsync(user.UserId);

                return new LoginResult
                {
                    Success = true,
                    User = user,
                    Message = "تم تسجيل الدخول بنجاح"
                };
            }
            catch (Exception ex)
            {
                // Log the exception here
                return new LoginResult
                {
                    Success = false,
                    Message = "حدث خطأ أثناء تسجيل الدخول"
                };
            }
        }

        public async Task<RegistrationResult> RegisterAsync(RegisterModel model)
        {
            var result = await _userRepository.RegisterUserAsync(model);

            if (result.Success)
            {
                // Send verification email
                await SendVerificationEmailAsync(result.UserId);
            }

            return result;
        }

        public async Task<bool> SendVerificationEmailAsync(int userId)
        {
            try
            {
                string token = await _userRepository.GenerateEmailVerificationTokenAsync(userId);

                // TODO: Send email with verification link
                // string verificationUrl = $"https://yourdomain.com/verify-email?token={token}";
                // await _emailService.SendVerificationEmailAsync(user.Email, verificationUrl);

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception here
                return false;
            }
        }

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            return await _userRepository.GetUserRolesAsync(userId);
        }

    }
}