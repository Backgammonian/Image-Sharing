namespace MyWebApp.Data.Interfaces
{
    public interface IRandomGenerator
    {
        int GetRandomInt();
        uint GetRandomUInt();
        long GetRandomLong();
        ulong GetRandomULong();
        string GetRandomString(int length);
        string GetRandomId();
    }
}
