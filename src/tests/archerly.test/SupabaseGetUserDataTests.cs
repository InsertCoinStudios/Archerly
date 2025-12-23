using Xunit;

namespace archerly.tests;

public class SupabaseGetUserDataTests
{
    private readonly SupabaseUserRepository _users;
    //goal Number 1 get All of the User-Data?

    [Fact]
    public function getUserFromRepo()
    {
        var user = await _users.GetByIdAsync(userId);
        
    }
}