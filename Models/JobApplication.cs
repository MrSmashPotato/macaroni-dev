using System.ComponentModel;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace macaroni_dev.Models;

[Table("job_application")]
public class JobApplication : BaseModel, INotifyPropertyChanged
{
    private string _status;
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    [Column("job_id")]
    public int JobId { get; set; }
    [Column("user_id")]
    public Guid UserId { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("status")]
    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(nameof(Status)); }
    }

    [Column("is_rejected")]
    public bool IsRejected { get; set; }
    [Column("is_archived")]
    public bool IsArchived { get; set; }
    
    //Notifier
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
