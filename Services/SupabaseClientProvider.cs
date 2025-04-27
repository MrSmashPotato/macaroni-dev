
using Supabase;
using System.Threading.Tasks;

namespace macaroni_dev.Services
{
    public class SupabaseClientProvider(Supabase.Client supabaseClient)
    {
        public Supabase.Client GetSupabaseClient()
        {
            return supabaseClient;
        }
        
    }
}