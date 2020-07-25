using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalR_Client
{
    internal static class Program
    {
        private static readonly CancellationTokenSource __ReadLineCancellation = new CancellationTokenSource();

        private static async Task Main(string[] args)
        {
            Console.CancelKeyPress += OnConsole_CancelKeyPressed;

            const string address = "http://localhost:5000/information";
            var connection = new HubConnectionBuilder()
               .WithUrl(address)
               .Build();
            connection.On("ClientConnected", OnClientConnected);
            connection.On("ClientDisconnected", OnClientDisconnected);
            connection.On<DateTime, string>("IntroduceNewClient", OnIntroduceNewClient);
            connection.On<DateTime, string, string>("ClientMessage", ReceiveMessage);

            Console.Write("User name >");
            var user_name = Console.ReadLine();
            Console.WriteLine();

            await connection.StartAsync();
            await connection.SendAsync("IntroduceClient", user_name);

            while (!__ReadLineCancellation.IsCancellationRequested)
            {
                var message = await Task.Run(Console.ReadLine).WithCancellation(__ReadLineCancellation.Token);
                await connection.InvokeAsync("ServerMessage", user_name, message);
            }

            await connection.StopAsync();
        }

        private static void OnIntroduceNewClient(DateTime Time, string UserName)
        {
            Console.WriteLine("Новый клиент представился как: {0} [{1:dd.MM.yy HH:mm:ss}]", UserName, Time);
        }

        private static void OnClientConnected()
        {
            Console.WriteLine("Новый клиент");
        }

        private static void OnClientDisconnected()
        {
            Console.WriteLine("Клиент отключился");
        }

        private static void OnConsole_CancelKeyPressed(object Sender, ConsoleCancelEventArgs E)
        {
            __ReadLineCancellation.Cancel();
            E.Cancel = true;
        }

        private static void ReceiveMessage(DateTime Time, string User, string Message)
        {
            Console.WriteLine("{0:dd:MM:yy HH:mm:ss}[{1}]:{2}", Time, User, Message);
        }
    }

    internal class UserMessage
    {
        public DateTime Time { get; set; }
        public string User { get; set; }
        public string Message { get; set; }
    }

    internal static class TaskEx
    {
        public static async Task<T> WithCancellation<T>([NotNull] this Task<T> task, CancellationToken cancel)
        {
            var source = new TaskCompletionSource<object>();
            await using (cancel.Register(tcs => ((TaskCompletionSource<object>)tcs).TrySetResult(default), source))
                if (task != await Task.WhenAny(task, source.Task))
                    throw new TaskCanceledException();
            return await task;
        }
    }
}
