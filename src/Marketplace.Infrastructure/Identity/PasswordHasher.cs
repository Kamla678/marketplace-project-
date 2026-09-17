using Marketplace.Application.Common.Interfaces;

namespace Marketplace.Infrastructure.Identity;

public class PasswordHasher : IPasswordHasher
{
    // work factor 12 — توازن كويس بين الأمان وسرعة الاستجابة
    private const int WorkFactor = 12;

    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    public bool Verify(string password, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
