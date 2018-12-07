namespace JDBot.Infrastructure.Framework
{
    public interface IGitProxy
    {
        void Checkout(string branch);
        void Push();
        void PushTags();
    }
}
