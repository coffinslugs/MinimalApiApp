namespace MinimalApi.Models;

public record AuthenticationData(string? Username, string? Password);
public record UserData(int Id, string FirstName, string LastName, string Username);
