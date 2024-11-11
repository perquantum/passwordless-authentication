using System;

namespace PasswordlessAuthentication.AuthServer.PasswordlessAuthentication;

public class Result
{
    public string Token { get; set; }
    public Guid UserId { get; set; }
}
