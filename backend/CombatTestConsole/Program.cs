// See https://aka.ms/new-console-template for more information
try
{
    throw new NotImplementedException();
    //dee-todo: There's no implementation for PlayGame
    //NewCombatLogic.Combat.PlayGame();//.Wait();

    Console.WriteLine("Game Started");
}catch(Exception ex)
{
    Console.Error.WriteLine($"failed {ex}");
}