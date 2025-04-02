using Supabase;
using Supabase.Gotrue;
using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;


namespace macaroni_dev.Services
{
    public class AuthService
    {
        private static AuthService? _instance;
        private readonly Supabase.Client _supabaseClient;

        // Private constructor to prevent direct instantiation
        private AuthService(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        // Singleton Instance Getter
        public static async Task<AuthService> GetInstanceAsync()
        {
            if (_instance == null)
            {
                var client = await SupabaseClientProvider.GetClientAsync();
                _instance = new AuthService(client);
            }

            return _instance;
        }

        // Sign Up 
        public async Task<User?> SignUpAsync(string email, string password)
        {
            try
            {
                var response = await _supabaseClient.Auth.SignUp(email, password);

                if (response.User != null)
                {
                    Console.WriteLine("Sign-up successful. Verification email sent.");
                    return response.User; // User created but still unverified
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Signup Error: " + ex.Message);
            }

            return null; // Signup failed
        }

        // Sign In 
        public async Task<User?> SignInAsync(string email, string password)
        {
            try
            {
                var result = await _supabaseClient.Auth.SignIn(email, password);
                if (result != null)
                {
                    await SecureStorage.SetAsync("session_token", result.AccessToken);
                    await SecureStorage.SetAsync("refresh_token", result.RefreshToken);

                    return result.User;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Login Error: " + ex.Message);
                throw new Exception(ex.Message);
                
            }
            return null;
        }
        public async Task<User?> RestoreSessionAsync(string sessionToken, string refreshToken)
        {
            try
            {
                var session = await _supabaseClient.Auth.SetSession(sessionToken, refreshToken);
                return session?.User;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Restore Session Error: " + ex.Message);
            }

            return null;
        }


        public async Task<bool> VerifyEmailOtpAsync(string email, string otpCode)
        {
            try
            {
                var session = await _supabaseClient.Auth.VerifyOTP(email, otpCode, Constants.EmailOtpType.Signup);

                if (session != null && session.User != null)
                {
                    Console.WriteLine("✅ Email verified successfully!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ OTP Verification Error: {ex.Message}");
            }

            return false;
        }

        // Logout
        public async Task<bool> SignOutAsync()
        {
            try
            {
                await _supabaseClient.Auth.SignOut();
                SecureStorage.Remove("session_token");
                SecureStorage.Remove("refresh_token");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SignOut Error: " + ex.Message);
                return false;
            }
        }

        // Check if user is logged in
        public bool IsUserLoggedIn()
        {
            return _supabaseClient.Auth.CurrentUser != null;
        }

        // Current User
        public User? GetCurrentUser()
        {
            return _supabaseClient.Auth.CurrentUser;
        }
        //Third


        // Third Party Sign in
        public async Task<User?> SignInWithThirdPartyAsync(Supabase.Gotrue.Constants.Provider provider)
        {
            try
            {
                var authService = await GetInstanceAsync();

                var auth = await _supabaseClient.Auth.SignIn(provider);
                if (auth == null || auth.Uri == null) throw new Exception("Failed to initiate OAuth flow.");

                var result = await WebAuthenticator.Default.AuthenticateAsync(auth.Uri, new Uri("jobilis://callback"));

                if (!string.IsNullOrEmpty(result?.AccessToken))
                {
                    // Attempt to retrieve the session from the callback URL
                    await _supabaseClient.Auth.GetSessionFromUrl(result.CallbackUri);

                    // Fetch the updated session
                    var session = _supabaseClient.Auth.CurrentSession;
                    if (session != null)
                    {
                        await SecureStorage.SetAsync("session_token", session.AccessToken);
                        await SecureStorage.SetAsync("refresh_token", session.RefreshToken);
                        return session.User;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("User canceled authentication.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OAuth Error: {ex.Message}");
            }

            return null; // Return null instead of an empty user to indicate failure
        }

        //internal async Task RestoreSessionAsync(Session session)
        //{
        //    throw new NotImplementedException();
        //}
    }
}