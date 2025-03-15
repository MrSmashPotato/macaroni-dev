using Supabase;
using Supabase.Gotrue;
using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using Supabase;
using System;
using System.Threading.Tasks;


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
                return result.User;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Login Error: " + ex.Message);
                throw new Exception(ex.Message);
                return null;
            }
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
        public async Task<bool> SignInWithGoogleAsync()
        {
            try
            {
                var auth = await _supabaseClient.Auth.SignIn(Supabase.Gotrue.Constants.Provider.Google);
                String oAuthToken;

                WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
                    auth.Uri,
                    new Uri("jobilis://callback"));
                if (authResult.AccessToken != null)
                {
                    return true;
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

            return false;
        }

        public async Task<bool> SignInWithFacebookAsync()
        {
            
            return false;
        }
    }
}