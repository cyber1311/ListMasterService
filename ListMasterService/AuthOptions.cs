using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ListMasterService;

public class AuthOptions
{
    public const string ISSUER = "";
    public const string AUDIENCE = "";
    const string KEY = "";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => 
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}