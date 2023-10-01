namespace app.Handle;
public interface IRefreshTokenGenerator{
    string GeneratorToken(string username);
}