namespace macaroni_dev.Services;

public class SupabaseError
{
    public int Code { get; set; }
    public string ErrorCode { get; set; }
    public string Msg { get; set; }
}