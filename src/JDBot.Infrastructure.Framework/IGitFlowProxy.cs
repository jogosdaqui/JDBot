namespace JDBot.Infrastructure.Framework
{
    public interface IGitFlowProxy
    {
        void StartRelease(SemanticVersioning version);
        void FinishRelease(SemanticVersioning version, string messag);
    }
}
