namespace macaroni_dev.Services;

public interface IKeyboardService
{
    bool IsKeyboardVisible { get; }
    event EventHandler<bool> KeyboardVisibilityChanged;
}