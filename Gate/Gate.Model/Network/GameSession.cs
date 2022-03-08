using Base;
using Base.Helper;
using Base.Serialize;
using DotNetty.Buffers;
using Interfaces.Gate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orleans;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using DotNetty.Transport.Channels;
using Interfaces.Home;
using Interfaces.Login;
using Message;
using Orleans.Runtime;
using IByteBuffer = MongoDB.Bson.IO.IByteBuffer;

namespace Gate.Model.Network
{
    public class GameSession
    {
        private readonly IClusterClient _client;

        private readonly ILogger _logger;

        //private readonly Guid _sessionId;
        private readonly IConfiguration _configuration;

        private OutcomingPacketObserver? _packetObserver;
        private IPacketObserver? _packetObserverRef;
        private IChannelHandlerContext _context;
        private IPlayerGrain? _player;

        private long? _grainId;

        private ulong PlayerId
        {
            get
            {
                if (_grainId != null)
                {
                    return (ulong) _grainId;
                }

                A.Abort("_playerId must not null");
                return 0;
            }
        }

        //private string _token;
        private string _md5Key;

        public GameSession(IClusterClient client, ILoggerFactory loggerFactory,
            IConfiguration configuration, IChannelHandlerContext context)
        {
            _client = client;
            _logger = loggerFactory.CreateLogger<GameSession>();
            _configuration = configuration;
            _context = context;
            //_sessionId = Guid.NewGuid();
            _md5Key = _configuration.GetValue<string>("MD5Key");
        }

        public async Task Disconnect()
        {
            if (_player != null)
            {
                await _player.UnbindPacketObserver();
            }
        }

        bool CheckSign(GRequest packet)
        {
            //todo 签名验证
            return true;
        }

        public async Task DispatchIncomingPacket(GRequest packet)
        {
            try
            {
                //先验证token--避免被攻击
                A.Ensure(CheckSign(packet), des: "first message must login", serious: true);
                //同步初始化
                if (_player == null)
                {
                    //第一条消息必须是登录--为了避免被攻击，直接断开
                    A.Ensure(packet.Opcode == OuterOpCode.Login.Int(), "first message must login",
                        serious: true);

                    var login = SerializeHelper.FromBinary<C2SLogin>(packet.Content);
                    _logger.Info($"player:{login.PlayerId} get login msg");

                    _player = _client.GetGrain<IPlayerGrain>((long) login.PlayerId);


                    //处理登录
                    var grainId = A.NotNull((long) login.PlayerId);
                    _grainId = grainId;
                    //_token = tokenInfo.Token;
                    _packetObserver = new OutcomingPacketObserver(this);
                    _player = _client.GetGrain<IPlayerGrain>(grainId);
                    _packetObserverRef = _client.CreateObjectReference<IPacketObserver>(_packetObserver).Result;
                    _player.BindPacketObserver(_packetObserverRef).Wait();
                    return;
                }

                //心跳
                if (packet.Opcode == (uint) OuterOpCode.Ping)
                {
                    var ret = new GResponse
                    {
                        Opcode = packet.Opcode, Sn = packet.Sn, Code = Code.Ok,
                        Content = new S2CPong {Time = TimeHelper.Now()}.ToBinary()
                    };
                    await DispatchOutcomingPacket(ret);
                    return;
                }

                // await _player.Tell() .SendPacket(packet);

                //watch.Stop();
                //Console.WriteLine($"{packet.UserId} {watch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "DispatchIncomingPacket异常:\n" +
                    $"{ex.Message}\n" +
                    $"{ex.StackTrace}\n" +
                    $"{JsonConvert.SerializeObject(packet)}");
            }
        }

        public async Task DispatchOutcomingPacket(IMessage packet)
        {
            try
            {
                if (_context.Channel.Active)
                {
                    var bytes = packet.ToBinary();
                    await _context.WriteAndFlushAsync(bytes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "DispatchOutComingPacket异常:\n" +
                    $"{ex.Message}\n" +
                    $"{ex.StackTrace}\n" +
                    $"{JsonConvert.SerializeObject(packet)}");
            }
        }

        public async Task Close()
        {
            await _context.CloseAsync();
        }

        class OutcomingPacketObserver : IPacketObserver
        {
            private readonly GameSession session;

            public OutcomingPacketObserver(GameSession session)
            {
                this.session = session;
            }

            public async void Close(IMessage? packet = null)
            {
                if (packet != null)
                    await session.DispatchOutcomingPacket(packet);
                await session.Close();
            }

            public async void SendPacket(IMessage packet)
            {
                await session.DispatchOutcomingPacket(packet);
            }
        }
    }
}