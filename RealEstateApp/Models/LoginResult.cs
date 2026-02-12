using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateApp.Models;

public class LoginResult
{
    public bool Succeeded { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
