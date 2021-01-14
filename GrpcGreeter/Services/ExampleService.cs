using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcGreeter
{
    public class ExampleService :exampler.examplerBase
    {
        /// <summary>
        /// 一元方法以参数的形式获取请求消息，并返回响应。 返回响应时，一元调用完成。
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ExampleResponse> UnaryCall(ExampleRequest request, ServerCallContext context)
        {
            // return base.UnaryCall(request, context);
            return Task.FromResult(new ExampleResponse
            {
                Msg = "id :" + request.Id + "name : " + request.Name + " hello"

            }) ;
        }

        /// <summary>
        /// 服务器流式处理方法
        /// 服务器流式处理方法以参数的形式获取请求消息。 由于可以将多个消息流式传输回调用方，因此可使用 responseStream.WriteAsync 发送响应
        /// 消息。 当方法返回时，服务器流式处理调用完成。
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task StreamingFromServer(ExampleRequest request, IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {

            /**
             * 服务器流式处理方法启动后，客户端无法发送其他消息或数据。 某些流式处理方法设计为永久运行。 对于连续流式处理方法，客户端可以在*再需要调用时将其取消。 当发生取消时，客户端会将信号发送到服务器，并引发 ServerCallContext.CancellationToken。 应在服务器上通过异步方法使用 CancellationToken 标记，以实现以下目的：
               所有异步工作都与流式处理调用一起取消。
               该方法快速退出。
             **/
            //return base.StreamingFromServer(request, responseStream, context);

            //for (int i = 0; i < 5; i++)
            //{
            //    await responseStream.WriteAsync(new ExampleResponse { Msg = "我是服务端流for：" + i });
            //    await Task.Delay(TimeSpan.FromSeconds(1));
            //}
            int index = 0;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                index++;
                await responseStream.WriteAsync(new ExampleResponse { Msg = "我是服务端流while" + index+" "+request.Id+" "+request.Name });
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);

            }

        }


        /// <summary>
        /// 客户端流式处理方法
        /// 客户端流式处理方法在该方法没有接收消息的情况下启动。 requestStream 参数用于从客户端读取消息。 返回响应消息时，客户端流式处理调用
        /// 完成：
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ExampleResponse> StreamingFromClient(IAsyncStreamReader<ExampleRequest> requestStream, ServerCallContext context)
        {
            // return base.StreamingFromClient(requestStream, context);
            List<string> list = new List<string>();
            while (await requestStream.MoveNext())
            {
                //var message = requestStream.Current;
                var id = requestStream.Current.Id;
                var name = requestStream.Current.Name;

                list.Add($"{id}-{name}");
                // ...
            }
            return new ExampleResponse() { Msg = "我是客户端流while"+string.Join(',',list) };


            //await foreach (var message in requestStream.ReadAllAsync())
            //{
            //    // ...
            //}
            // return new ExampleResponse() { Msg= "我是客户端流foreach" };
        }

        /// <summary>
        /// 双向流式处理方法
        /// 双向流式处理方法在该方法没有接收到消息的情况下启动。 requestStream 参数用于从客户端读取消息。 
        /// 该方法可选择使用 responseStream.WriteAsync 发送消息。 当方法返回时，双向流式处理调用完成：
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task StreamingBothWays(IAsyncStreamReader<ExampleRequest> requestStream, IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            //return base.StreamingBothWays(requestStream, responseStream, context);
            await foreach (var message in requestStream.ReadAllAsync())
            {
               string str= message.Id + " " + message.Name;
                await responseStream.WriteAsync(new ExampleResponse() { Msg="我是双向流："+ str });
                
            }


            //// Read requests in a background task.
            //var readTask = Task.Run(async () =>
            //{
            //    await foreach (var message in requestStream.ReadAllAsync())
            //    {
            //        // Process request.
            //        string str = message.Id + " " + message.Name;
            //    }
            //});

            //// Send responses until the client signals that it is complete.
            //while (!readTask.IsCompleted)
            //{
            //    await responseStream.WriteAsync(new ExampleResponse());
            //    await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            //}
        }

    }
}
