
using Supabase;
using System.Threading.Tasks;

namespace macaroni_dev.Services
{
    public static class SupabaseClientProvider
    {
        private static Client? _supabaseClient;

        public static async Task<Client> GetClientAsync()
        {
            if (_supabaseClient == null)
            {
                var options = new Supabase.SupabaseOptions
                {
                    AutoConnectRealtime = true
                };

                _supabaseClient = new Client(
                    "https://ovbzwunivnrlfcncogve.supabase.co",  
                    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im92Ynp3dW5pdm5ybGZjbmNvZ3ZlIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDE3NDI1NTcsImV4cCI6MjA1NzMxODU1N30.HBZLj_ig4WxUGx_Nkouymj6qCRbOBbShTlFbyWsziGE",  // Replace with your Supabase API key
                    options
                );

                await _supabaseClient.InitializeAsync();
            }

            return _supabaseClient;
        }
    }
}