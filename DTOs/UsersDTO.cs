

using System.Text.Json.Serialization;

namespace ToDoApp.DTOs;

public record UsersDTO

{
    [JsonPropertyName("user_id")]
    public long UserId { get; set; }
     [JsonPropertyName("user_name")]
    public string UserName { get; set; }
     [JsonPropertyName("gender")]
    public string Gender { get; set; }

     [JsonPropertyName("passwor_d")]
    public string Password { get; set; }
      [JsonPropertyName("email")]
    public string Email { get; set; }
 
}

public record UsersCreateDTO

{

    [JsonPropertyName("user_name")]
    public string UserName { get; set; }
     [JsonPropertyName("passwor_d")]
    public string Password { get; set; }

     [JsonPropertyName("email")]
    public string Email { get; set; }
}