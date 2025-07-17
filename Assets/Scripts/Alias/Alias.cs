using UnityEngine;

public class Alias
{
    public string originalCommand = "";
    public string aliasCommand = "";

    public string description;

    private void Start()
    {
        description = "An alias is a different way of typing a command in the console\n" +
                        "For example:\n" +
                        "Original command = mp_restartgame 1 (restarts the game in practice mode)\n" +
                        "Alias command = rs" +
                        "Now typing \"rs\" in console, is like typing \"mp_restartgame 1\"";
    }

    public bool IsWritten()
    {
        return !string.IsNullOrEmpty(originalCommand) && !string.IsNullOrEmpty(aliasCommand);
    }

}
