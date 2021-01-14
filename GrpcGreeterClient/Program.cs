using Grpc.Core;
using Grpc.Net.Client;
using GrpcGreeter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcGreeterClient
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    Console.WriteLine("Hello World!");
        //}
        //static async Task Main(string[] args)
        //{
        //    // The port number(5001) must match the port of the gRPC server.
        //    using var channel = GrpcChannel.ForAddress("https://localhost:5001");
        //    var client = new Greeter.GreeterClient(channel);
        //    var reply = await client.SayHelloAsync(
        //                      new HelloRequest { Name = "GreeterClient" });
        //    Console.WriteLine("Greeting: " + reply.Message);
        //    Console.WriteLine("Press any key to exit...");
        //    Console.ReadKey();
        //}


        static async Task Main(string[] args)
        {
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new exampler.examplerClient(channel);

            #region 一元调用

            //var reply = await client.UnaryCallAsync(new ExampleRequest { Id = 1, Name = "hda" });
            //Console.WriteLine("Greeting: " + reply.Msg);

            #endregion 一元调用

            #region  服务器流式处理调用

            //using var call = client.StreamingFromServer(new ExampleRequest { Id = 1, Name = "hda" });

            //while (await call.ResponseStream.MoveNext(CancellationToken.None))
            //{
            //    Console.WriteLine("Greeting: " + call.ResponseStream.Current.Msg);

            //}
            //如果使用 C# 8 或更高版本，则可使用 await foreach 语法来读取消息。 IAsyncStreamReader<T>.ReadAllAsync() 扩展方法读取响应数据流中的所有消息：
            //await foreach (var response in call.ResponseStream.ReadAllAsync())
            //{
            //    Console.WriteLine("Greeting: " + response.Msg);
            //    // "Greeting: Hello World" is written multiple times
            //}

            #endregion  服务器流式处理调用

            #region  客户端流式处理调用
            //using var call = client.StreamingFromClient();
            //for (int i = 0; i < 5; i++)
            //{
            //    await call.RequestStream.WriteAsync(new ExampleRequest { Id = i, Name = "hda" + i });
            //}
            //await call.RequestStream.CompleteAsync();
            //var response = await call;
            //Console.WriteLine($"Count: {response.Msg}");
            #endregion 客户端流式处理调用

            #region  双向流式处理调用

            //通过调用 EchoClient.Echo 启动新的双向流式调用。
            //使用 ResponseStream.ReadAllAsync() 创建用于从服务中读取消息的后台任务。
            //使用 RequestStream.WriteAsync 将消息发送到服务器。
            //使用 RequestStream.CompleteAsync() 通知服务器它已发送消息。
            //等待直到后台任务已读取所有传入消息。
            //双向流式处理调用期间，客户端和服务可在任何时间互相发送消息。 与双向调用交互的最佳客户端逻辑因服务逻辑而异。
            using var call = client.StreamingBothWays();
            Console.WriteLine("Starting background task to receive messages");
            var readTask = Task.Run(async () =>
            {
                await foreach (var response in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(response.Msg);
                    // Echo messages sent to the service
                }
            });
            Console.WriteLine("Starting to send messages");
            Console.WriteLine("Type a message to echo then press enter.");
            while (true)
            {
                var result = Console.ReadLine();
                if (string.IsNullOrEmpty(result))
                {
                    break;
                }

                await call.RequestStream.WriteAsync(new ExampleRequest { Id=1,Name= result });
            }

            Console.WriteLine("Disconnecting");
            await call.RequestStream.CompleteAsync();
            await readTask;
            #endregion 双向流式处理调用



            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
