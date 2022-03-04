﻿using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orleans;
using Base.Serialize;
using Gate.Model.Network;
using Message;

namespace MO.Gateway.Network
{
    public class SocketChannelHandler : ChannelHandlerAdapter
    {
        private readonly ILogger _logger;
        private readonly IClusterClient _client;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;
        private GameSession? _session;

        public SocketChannelHandler(IClusterClient client, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _client = client;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<SocketChannelHandler>();
            _configuration = configuration;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
        }

        public override async void ChannelRead(IChannelHandlerContext context, object message)
        {
            try
            {
                var revBuffer = (message as IByteBuffer)!;
                if (revBuffer.ReadableBytes is > UInt16.MaxValue or <= 2)
                {
                    await context.CloseAsync();
                    return;
                }

                var dataBuffer = new byte[revBuffer.ReadableBytes];
                revBuffer.ReadBytes(dataBuffer);
                var msg = SerializeHelper.FromBinary<GRequest>(dataBuffer);
                if (_session != null)
                {
                    await _session.DispatchIncomingPacket(msg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ChannelRead Exception:\n{ex.Message}\n{ex.StackTrace}");
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            _logger.LogError($"Gateway Exception: {exception}");
            context.CloseAsync();
        }

        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            _session = new GameSession(_client, _loggerFactory, _configuration, context);
            base.ChannelRegistered(context);
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            _session?.Disconnect();
            base.ChannelUnregistered(context);
        }
    }
}