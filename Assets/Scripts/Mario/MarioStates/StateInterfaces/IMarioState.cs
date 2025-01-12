public interface IMarioState
{
    void EnterState(IContext context);
    void ExitState(IContext context);
    void Update(IContext context);
}