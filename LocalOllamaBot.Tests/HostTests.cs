using Xunit;

namespace LocalOllamaBot.Tests;

public class HostTests
{
    [Fact]
    public void Program_HasMainMethod()
    {
        var programType = typeof(Program);
        var mainMethod = programType.GetMethod("<Main>$", 
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        
        Assert.NotNull(mainMethod);
    }
}