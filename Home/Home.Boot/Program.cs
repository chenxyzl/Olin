// See https://aka.ms/new-console-template for more information

using Base;

namespace Home.Boot;

class Program
{
    static async Task Main()
    {
        var a = new StartUp().Build();
        await a.StartAsync();
    }
}