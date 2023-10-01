using System.Security.Cryptography;

namespace app.Handle;
public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    private ApplicationDbContext dbContext;
    public RefreshTokenGenerator(ApplicationDbContext dbContext){
        this.dbContext = dbContext;
    }
    public string GeneratorToken(string username)
    {
        var randomNumber = new byte[32];
        using(var randomNumberGenerator = RandomNumberGenerator.Create()){
            randomNumberGenerator.GetBytes(randomNumber);
            string refreshToken = Convert.ToBase64String(randomNumber);
            return refreshToken;
        }
    }
}